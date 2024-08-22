using MudBlazor.Utilities;

namespace Mythologist_Client_WASM.Client.Utils
{
    public class Utils
    {

        static Random random = new Random();

        public static bool IsDiscord()
        {
            #if DISCORD
                return true;
            #else
                return false;
            #endif
        }

        public static string RandomName()
        {
            List<string> names = new List<string>
            {
                "Odysseus",
                "Polites",
                "Telemachus",
                "Eurylochus",
                "Polyphemus",
                "Aoelus",
                "Hermes",
                "Poseidon",
                "Scylla",
                "Zues",
                "Athena",
                "Circe",
                "Orpheus",
                "Eurydice",
                "Hades",
                "Persephone",
                "Icarus",
                "Daedalus",
                "Sisyphus",
                "Dionysius",
                "Narcissus",
                "Chiron",
                "Medusa",
                "Pandora",
                "Achilles",
                "Eros",
                "Theseus",
                "Aphrodite",
                "Ares",
                "Hephaestus",
                "Apollo",
                "Persephone",
                "Demeter",
                "Artemis",
                "Hera",
                "Charon",
                "Helen",
                "Patroclus",
                "Tantalus",
                "Midas",
                "Jason"
            };

            int index = random.Next(names.Count);
            return names[index];
        }

		public static string GenerateHTMLColor(string seed)
		{
			// Convert the seed character to an integer, and use it to seed the Random object
			Random random = new Random(seed.Length * seed[0]);

			// Generate a random hue (H component), while saturation (S) and value (V) are maxed
			double h1 = random.NextDouble(); // Hue: a random number between 0.0 and 1.0
			double s1 = 0.7; // Saturation is something. Full bright is a bit harsh
			double v = 1.0; // Value (brightness) is maxed

			// Convert the first HSV color to RGB
			var (r1, g1, b1) = HSVToRGB(h1, s1, v);

			// Convert RGB to HTML color code
			return $"#{r1:X2}{g1:X2}{b1:X2}";

		}

		private static (int r, int g, int b) HSVToRGB(double h, double s, double v)
		{
			double r = 0, g = 0, b = 0;

			int i = (int)(h * 6);
			double f = h * 6 - i;
			double p = v * (1 - s);
			double q = v * (1 - f * s);
			double t = v * (1 - (1 - f) * s);

			switch (i % 6)
			{
				case 0: r = v; g = t; b = p; break;
				case 1: r = q; g = v; b = p; break;
				case 2: r = p; g = v; b = t; break;
				case 3: r = p; g = q; b = v; break;
				case 4: r = t; g = p; b = v; break;
				case 5: r = v; g = p; b = q; break;
			}

			return ((int)(r * 255), (int)(g * 255), (int)(b * 255));
		}
	}
}
