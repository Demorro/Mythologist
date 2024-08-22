using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLogic.Utils
{
	public class Constants
	{
		public enum MediaItemType
		{
			Image,
			Audio
		};

		public static string[] ALLOWED_IMAGE_FORMATS =
		{
			".jpg",
			".jpeg",
			".png",
			".gif",
			".webp",
			".bmp",
			".tiff"
		};

		public static string[] ALLOWED_AUDIO_FORMATS =
		{
			".mp3",
			".ogg"
		};

		//Good for use in file browser allowed lists, will be like ".jpg, .png..."
		public static string AllowedImageFormatString()
		{
			return String.Join(", ", ALLOWED_IMAGE_FORMATS);
		}

		public static string AllowedAudioFormatString()
		{
			return String.Join(", ", ALLOWED_AUDIO_FORMATS);
		}

		public static bool AllowedFormat(string file, MediaItemType itemType)
		{
			switch (itemType)
			{
				case MediaItemType.Image:
					return AllowedImageFormat(file);
				case MediaItemType.Audio:
					return AllowedAudioFormat(file);
			}

			throw new Exception("unexpected item type");
		}

		//Just does a basic check to see if the last characters of a file match one of the allowed formats
		public static bool AllowedImageFormat(string file)
		{
			foreach(var format in ALLOWED_IMAGE_FORMATS)
			{
				if (file.EndsWith(format))
				{
					return true;
				}
			}
			return false;
		}

		public static bool AllowedAudioFormat(string file)
		{
			foreach (var format in ALLOWED_AUDIO_FORMATS)
			{
				if (file.EndsWith(format))
				{
					return true;
				}
			}
			return false;
		}
	}
}
