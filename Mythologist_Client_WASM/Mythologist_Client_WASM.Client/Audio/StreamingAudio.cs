using KristofferStrube.Blazor.DOM;
using KristofferStrube.Blazor.WebAudio;
using Microsoft.JSInterop;
using MudBlazor;
using Mythologist_Client_WASM.Client.Services;

namespace Mythologist_Client_WASM.Client.Audio
{
    //This needs to be IDisposable
    public class StreamingAudio : IAsyncDisposable
    {
        private ISignalRHubClientService signalRHub;
        private IJSRuntime JSRuntime;
        private ISnackbar snackBar;

        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private AudioContext? context;
        private AudioDestinationNode? destinationNode;

        //Remember to dispose these when you're done with em
        private List<(AudioBuffer, AudioBufferSourceNode)> audioStream = new List<(AudioBuffer, AudioBufferSourceNode)>();
        private int currentBufferIndex = 0;
        private double lastBufferLoadedDuration = 0.0;

        private const int NO_PACKET_RETRY_DELAY_MS = 1 * 1000;
        double startTime = 0;

        public StreamingAudio(ISignalRHubClientService _signalRHub, IJSRuntime _JSRuntime, ISnackbar _snackBar)
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
            //130kbps
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

            //Begin recieveing bytes
            await foreach (byte[] bytes in stream)
            {
                Console.WriteLine($"Recieved {bytes.Length} audio bytes");
                try
                {
                    AudioBuffer buffer = await context.DecodeAudioDataAsync(bytes);
                    AudioBufferSourceNode sourceNode = await context.CreateBufferSourceAsync();
                    await sourceNode.SetBufferAsync(buffer);
                    await sourceNode.ConnectAsync(destinationNode);

                    if (startTime < await context.GetCurrentTimeAsync())
                    {
                        startTime = await context.GetCurrentTimeAsync();
                    }

                    startTime += lastBufferLoadedDuration;
                    await sourceNode.StartAsync(startTime);
                    audioStream.Add((buffer, sourceNode));

                    lastBufferLoadedDuration = await buffer.GetDurationAsync();
                }
                catch (Exception ex)
                {
                    snackBar.Add(ex.Message, Severity.Error);
                }
            }
        }

        /*
         * Public wrapper for NextBuffer
         * Call and dont await. Expects `StreamBackgroundAudio` to have been called already.
         */
        public async Task BeginPlayback()
        {
            //await NextBuffer();
        }

        /*
         * Recursively work through the buffers in the stream, waiting and retrying if one isn't there yet.
         * Expects `StreamBackgroundAudio` to have been called already.
         * Dont await
         */
        private async Task NextBuffer()
        {
            if (cancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            if ((context == null) || (destinationNode == null))
            {
                throw new Exception("No Audio Context. Call Initialize");
            }

            while (audioStream.Count <= currentBufferIndex)
            {
                await Task.Delay(NO_PACKET_RETRY_DELAY_MS);
            }

            if (this.startTime < await this.context.GetCurrentTimeAsync())
            {
                this.startTime = await this.context.GetCurrentTimeAsync();
            }

            await audioStream[currentBufferIndex].Item2.StartAsync(startTime);
            this.startTime += await audioStream[currentBufferIndex].Item1.GetDurationAsync();
            currentBufferIndex++;
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);

            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            foreach (var buffers in audioStream)
            {
                await buffers.Item1.DisposeAsync();
                await buffers.Item2.DisposeAsync();
            }
            audioStream.Clear();
        }
    }
}
