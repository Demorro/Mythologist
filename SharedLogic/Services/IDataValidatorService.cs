using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLogic.Services
{
    public interface IDataValidatorService
    {
        public bool ValidImage(Uri imagePath);
        public bool ValidAudio(Uri audioPath);
    }
}
