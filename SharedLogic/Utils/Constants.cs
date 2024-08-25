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
	}
}
