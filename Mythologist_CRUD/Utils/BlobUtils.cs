using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using static Mythologist_CRUD.Components.Pages.ManageMedia;
using SharedLogic.Utils;
using System.Configuration;

namespace Mythologist_CRUD.Utils
{
	public class BlobUtils
	{
		// The gumph to make sure we're all setup and connected to the games blob storage.
		// Will fill in nulls if they're not set yet
		// You wanna set the clients on the outside to the return values, so you don't end up making new ones every time
		// Not a great pattern tbh, could be cleaner if async let you have out params.
		// You'll wanna all this before you do blob stuff generally. I dunno man, i'm not you're dad.
		public static async Task<(BlobServiceClient, BlobContainerClient)> SetupOrVerifyBlobStorageGumph(string gameName, string connectionString, BlobServiceClient? blobServiceClient, BlobContainerClient? gameContainer)
		{
			if (gameName == null)
			{
				throw new Exception("Expected gameName to be populated");
			}
			if (connectionString == null)
			{
				throw new Exception("Expected connectionString to be populated");
			}
			if (blobServiceClient == null)
			{
				var blobconnectionString = connectionString;
				blobServiceClient = new BlobServiceClient(blobconnectionString);
			}
			if (gameContainer == null)
			{
				gameContainer = blobServiceClient.GetBlobContainerClient(gameName);

				if (!await gameContainer.ExistsAsync())
				{
					await gameContainer.CreateAsync(PublicAccessType.BlobContainer);
				}
			}

			return (blobServiceClient, gameContainer);
		}

        public class MediaMetadata
        {
            public MediaMetadata(string _name, string _fullUrl, double _sizeInBytes)
            {
                Name = _name;
                FullUrl = _fullUrl; //This is what we pass in to <image> or <audio> tags. Use the ContainerLocalUrl to interact with the blob sdk mainly
                FileSizeInBytes = _sizeInBytes;
                FileSizeStr = MakeFileSizeString(FileSizeInBytes);
            }

            public string Name { get; set; } // file.png
            public double FileSizeInBytes { get; set; } //13.23
            public string FullUrl;  // /azurewebsite/account/images/file.png
            public string FileSizeStr; //13.23MB

            //In the game containers. Images are stored under "images/" and audio under "audio/" etc
            public string ContainerLocalUrl(Constants.MediaItemType itemType)
            {
                switch (itemType)
                {
                    case Constants.MediaItemType.Image:
                        return "images/" + Name;
                    case Constants.MediaItemType.Audio:
                        return "audio/" + Name;
                }

                throw new Exception("Unknown Item Type");
            }

            // Overrides for Equals, GetHashCode and ToString are important for MudSelect
            public override bool Equals(object o)
            {
                var other = o as MediaMetadata;
                return other?.Name == Name;
            }
            public override int GetHashCode() => Name?.GetHashCode() ?? 0;

            public override string ToString()
            {
				return Name;
            }

            //Convert from a double of bytes to a string with "MB" at the end.
            private string MakeFileSizeString(double? sizeInBytes)
            {
                if (sizeInBytes == null)
                {
                    return "";
                }

                double bytes = sizeInBytes.Value;
                double MB = bytes / (1024 * 1024);
                return MB.ToString("F2") + "MB";
            }
        };

		public static Uri MakeFullStorageUri(BlobServiceClient? blobServiceClient, string gameName, Constants.MediaItemType itemType, string fileName)
		{
			if (blobServiceClient == null)
			{
				throw new Exception("Expected non null blob service client");
			}

			//Need to branch here? Wtf
            return new Uri(blobServiceClient?.Uri, $"{gameName}/{BlobUtils.MediaContainerPrefix(itemType)}{fileName}");
        }


        //The thing to call if you want a list of files available for your game.
        public static async Task<List<MediaMetadata>> GetMediaMetaData(string gameName, Constants.MediaItemType itemType, BlobContainerClient? gameContainer, BlobServiceClient? blobServiceClient)
		{
			if (gameContainer == null)
			{
				throw new Exception("Expected non-null GameContainer");
			}
			if (blobServiceClient == null)
			{
				throw new Exception("Expected non-null BlobServiceClient");
			}

			List<MediaMetadata> metaDatas = new List<MediaMetadata>();

			await foreach (BlobItem blobItem in gameContainer.GetBlobsAsync(prefix: MediaContainerPrefix(itemType)))
			{
				double? sizeBytesMaybe = blobItem.Properties.ContentLength;
				double sizeBytes = sizeBytesMaybe != null ? sizeBytesMaybe.Value : 0;
				string filename = blobItem.Name.Substring(MediaContainerPrefix(itemType).Length);

				//Remove the "images/" from the front so we just store the actual file name
				MediaMetadata mediaMetadata = new MediaMetadata(
					filename,
					BlobUtils.MakeFullStorageUri(blobServiceClient, gameName, itemType, filename).ToString(),
					sizeBytes);

				metaDatas.Add(mediaMetadata);
			}

			return metaDatas;
		}


		// blobs are stored inside the game container in folders like "image/" 
		// This return a string with a trailing slash
		public static string MediaContainerPrefix(Constants.MediaItemType itemType)
		{
			switch (itemType)
			{
				case Constants.MediaItemType.Image:
					return "images/";
				case Constants.MediaItemType.Audio:
					return "audio/";
			}

			throw new Exception("Unexpected ItemType");
		}

	}
}
