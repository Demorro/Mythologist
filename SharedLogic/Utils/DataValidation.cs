using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SharedLogic.Utils.Constants;

namespace SharedLogic.Utils
{
	public class DataValidation
	{
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

		//For use in mudblazor validation slots. Null means all good. Return value is the error that appears
		public static string? SanitizeStringMudBlazorValidation(string text) {
			try {
				SanitizeString(text);
			}
			catch(Exception ex) {
				return ex.Message;
			}

			return null;
		}

		public static string? SanitizeStringWithErrorSnackbar(string? text, ISnackbar snackBar) {
			try {
				return SanitizeString(text);
			}
			catch(Exception ex) {
				snackBar.Add(ex.Message, Severity.Error);
				return null;
			}
		}

        public static string SanitizeString(string text) {
			if (text == null) {
                throw new Exception("Text is null");
            }
            if (text.Length < 3) {
                throw new Exception($"'{text}' is too short. Must be longer than 3 characters");
            }
            if (text.Length > 255) {
                throw new Exception($"'{text}' is too long. Must be shorter than 256 characters");
            }

            return text;
        }

		public static string? SanitizeMessageStringWithErrorSnackbar(string? message, ISnackbar snackBar) {
			try {
				return SanitizeMessageString(message);
			}
			catch(Exception ex) {
				snackBar.Add(ex.Message, Severity.Error);
				return null;
			}
		}

        public static string SanitizeMessageString(string text) {
			if (text == null) {
                throw new Exception("Message is null");
            }
            if (text.Length > 1023) {
                throw new Exception($"Message too long. Must be shorter than 1024 characters");
            }

            return text;
        }

	}
}
