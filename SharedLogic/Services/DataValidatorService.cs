using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLogic.Services
{
    public class DataValidatorService : IDataValidatorService
    {
        public bool ValidImage(Uri imagePath)
        {
            List<string> validImageTypes = new List<string>{
                ".jpg",
                ".jpeg",
                ".png",
                ".gif",
                ".webp",
                ".bmp",
                ".tiff"
            };

            var path = Path.GetExtension(imagePath.AbsolutePath);
            return validImageTypes.Contains(path);
        }
        public bool ValidAudio(Uri audioPath)
        {
            List<string> validAudioTypes = new List<string>{
                ".mp3",
                ".ogg",
            };

            var path = Path.GetExtension(audioPath.AbsolutePath);
            return validAudioTypes.Contains(path);
        }
    }
}
