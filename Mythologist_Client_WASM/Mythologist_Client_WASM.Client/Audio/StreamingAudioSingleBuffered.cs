using KristofferStrube.Blazor.DOM;
using KristofferStrube.Blazor.WebAudio;
using Microsoft.JSInterop;
using MudBlazor;
using Mythologist_Client_WASM.Client.Services;

namespace Mythologist_Client_WASM.Client.Audio
{
    //This needs to be IDisposable
    public class StreamingAudioSingleBuffered : IAsyncDisposable
    {
        private ISignalRHubClientService signalRHub;
        private IJSRuntime JSRuntime;
        private ISnackbar snackBar;

        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private AudioContext? context;
        private AudioDestinationNode? destinationNode;

        private double startedPlayingInitialTime = 0.0;

        public StreamingAudioSingleBuffered(ISignalRHubClientService _signalRHub, IJSRuntime _JSRuntime, ISnackbar _snackBar)
        {
            signalRHub = _signalRHub;
            JSRuntime = _JSRuntime;
            snackBar = _snackBar;
        }

        /*
         * Annoying that you cant do this shit in constructors
         */
        public async Task Initialize()
        {
            AudioContextOptions options = new AudioContextOptions();
            options.SampleRate = 44100;
            context = await AudioContext.CreateAsync(JSRuntime, options);
            destinationNode = await context.GetDestinationAsync();
        }

        /* 
         * Begins downloading byte packets for the background audio, using async SignalR streams.
         * Does not start playing anything
         * You shouldn't await this method
         */
        public async Task StreamBackgroundAudio(string gameName, string sceneName)
        {
            if ((context == null) || (destinationNode == null))
            {
                throw new Exception("No Audio Context. Call Initialize");
            }

            IAsyncEnumerable<byte[]> stream = signalRHub.GetBackgroundAudioStream(gameName, sceneName, cancellationTokenSource.Token);

            //We'll gather all the chunks then stitch them together before doing a hotswap
            List<byte[]> allByteChunks = new List<byte[]>();

            AudioBuffer? initialBuffer = null;
            await using AudioBufferSourceNode initialSourceNode = await context.CreateBufferSourceAsync();

            //Begin recieveing bytes
            int chunkCount = 0;
            await foreach (byte[] bytes in stream)
            {
                Console.WriteLine($"Recieved {bytes.Length} audio bytes");
                try
                {
                    if (chunkCount == 0)
                    {
                        Console.WriteLine($"Playing Initial Source 1");
                        initialBuffer = await context.DecodeAudioDataAsync(bytes);
                        Console.WriteLine($"Playing Initial Source 2");
                        await initialSourceNode.SetBufferAsync(initialBuffer);
                        Console.WriteLine($"Playing Initial Source 3");
                        await initialSourceNode.ConnectAsync(destinationNode);
                        Console.WriteLine($"Playing Initial Source 4");
                        await initialSourceNode.StartAsync();
                        startedPlayingInitialTime = await context.GetCurrentTimeAsync();
                    }
        
                    allByteChunks.Add(bytes);
                    chunkCount++;

                }
                catch (Exception ex)
                {
                    snackBar.Add(ex.Message, Severity.Error);
                }
            }

            try
            {
                //Now we've all the bytes, join them into one big array
                byte[] fullAudio = allByteChunks.SelectMany(b => b).ToArray();
                await using AudioBuffer fullBuffer = await context.DecodeAudioDataAsync(fullAudio);
                await using AudioBufferSourceNode fullSourceNode = await context.CreateBufferSourceAsync();
                await fullSourceNode.SetBufferAsync(fullBuffer);
                await fullSourceNode.ConnectAsync(destinationNode);
                Console.WriteLine($"Playing Full Source");
                await initialSourceNode.StopAsync();
                await fullSourceNode.StartAsync(0, await context.GetCurrentTimeAsync() - startedPlayingInitialTime);
            }
            catch (Exception ex)
            {
                snackBar.Add(ex.Message, Severity.Error);
            }

            await initialBuffer.DisposeAsync();
            await initialSourceNode.DisposeAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);

            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
        }
    }
}
