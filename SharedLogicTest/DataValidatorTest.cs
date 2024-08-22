using SharedLogic.Services;
using System.Collections.Generic;

namespace SharedLogicTest
{
    [TestClass]
    public class DataValidatorTest
    {
        [TestMethod]
        public void TestValidImages()
        {
            List<Uri> validImages = new List<Uri>()
            {
                new Uri("https://example.com/images/sample1.jpg"),
                new Uri("https://example.com/images/sample1.png"),
                new Uri("https://example.com/images/sample1.webp")
            };

            List<Uri> invalidImages = new List<Uri>()
            {
                new Uri("https://example.com/images/sample1.notreal"),
                new Uri("https://example.com/images"),
                new Uri("https://example.png")
            };

            DataValidatorService service = new DataValidatorService();

            foreach (Uri uri in validImages) {
                Assert.IsTrue(service.ValidImage(uri));
            }

            foreach (Uri uri in invalidImages)
            {
                Assert.IsFalse(service.ValidImage(uri));
            }
        }

        [TestMethod]
        public void TestValidAudios()
        {
            List<Uri> validAudios = new List<Uri>()
            {
                new Uri("https://example.com/audio/sample1.mp3"),
                new Uri("https://example.com/audio/sample1.ogg"),
                new Uri("https://example.com/audio/sample1.aac")
            };

            List<Uri> invalidAudios = new List<Uri>()
            {
                new Uri("https://example.com/audio/sample1.notreal"),
                new Uri("https://example.com/audio"),
                new Uri("https://example.png")
            };

            DataValidatorService service = new DataValidatorService();

            foreach (Uri uri in validAudios)
            {
                Assert.IsTrue(service.ValidAudio(uri));
            }

            foreach (Uri uri in invalidAudios)
            {
                Assert.IsFalse(service.ValidAudio(uri));
            }
        }
    }
}