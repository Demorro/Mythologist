using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MudBlazor;
using System.Drawing;
using Color = System.Drawing.Color;

namespace SharedLogic.Styling
{
    public class Styling
    {
        static Random rand = new Random();

        public static MudTheme MythologistTheme()
        {
            double hue = (rand.NextDouble() * 360);
            double contrast = ((rand.NextDouble() * 0.5) - 0.5);
            Console.WriteLine("Hue" + hue);
            Console.WriteLine("Contrast" + contrast);
            var pallette = ColorTheme.GenerateTheme((float)hue, (float)contrast, 0.0f);
            PrintPallette(pallette);
            return new MudTheme()
            {
                PaletteDark = pallette
            };
        }

        public static void PrintPallette(PaletteDark pallette)
        {
            Console.WriteLine("---------------------- PALLETTE ----------------------");
            Console.WriteLine($"Primary : {pallette.Primary}");
            Console.WriteLine($"Surface : {pallette.Surface}");
            Console.WriteLine($"Background : {pallette.Background}");
            Console.WriteLine($"BackgroundGray : {pallette.BackgroundGray}");
            Console.WriteLine($"AppbarText : {pallette.AppbarText}");
            Console.WriteLine($"AppbarBackground : {pallette.AppbarBackground}");
            Console.WriteLine($"DrawerBackground : {pallette.DrawerBackground}");
            Console.WriteLine($"ActionDefault : {pallette.ActionDefault}");
            Console.WriteLine($"ActionDisabled : {pallette.ActionDisabled}");
            Console.WriteLine($"ActionDisabledBackground : {pallette.ActionDisabledBackground}");
            Console.WriteLine($"TextPrimary : {pallette.TextPrimary}");
            Console.WriteLine($"TextSecondary : {pallette.TextSecondary}");
            Console.WriteLine($"TextDisabled : {pallette.TextDisabled}");
            Console.WriteLine($"DrawerIcon : {pallette.DrawerIcon}");
            Console.WriteLine($"DrawerText : {pallette.DrawerText}");
            Console.WriteLine($"GrayLight : {pallette.GrayLight}");
            Console.WriteLine($"GrayLighter : {pallette.GrayLighter}");
            Console.WriteLine($"Info : {pallette.Info}");
            Console.WriteLine($"Success : {pallette.Success}");
            Console.WriteLine($"Warning : {pallette.Warning}");
            Console.WriteLine($"Error : {pallette.Error}");
            Console.WriteLine($"LinesDefault : {pallette.LinesDefault}");
            Console.WriteLine($"TableLines : {pallette.TableLines}");
            Console.WriteLine($"Divider : {pallette.Divider}");
            Console.WriteLine($"OverlayLight : {pallette.OverlayLight}");
        }
    }

    public class ColorTheme
    {
        public static PaletteDark GenerateTheme(float baseHue, float saturationShift = 0f, float brightnessShift = 0f)
        {
            var theme = new PaletteDark
            {
                Primary = AdjustColor(new HSBColor(baseHue, 0.7f, 0.8f), saturationShift, brightnessShift),
                Surface = AdjustColor(new HSBColor(baseHue - 15, 0.5f, 0.2f), saturationShift, brightnessShift),
                Background = AdjustColor(new HSBColor(baseHue - 20, 0.5f, 0.1f), saturationShift, brightnessShift),
                BackgroundGray = AdjustColor(new HSBColor(baseHue - 20, 0.4f, 0.08f), saturationShift, brightnessShift),
                AppbarText = AdjustColor(new HSBColor(baseHue + 45, 0.2f, 0.7f), saturationShift, brightnessShift),
                AppbarBackground = AdjustColor(new HSBColor(baseHue - 20, 0.4f, 0.1f), saturationShift, brightnessShift),
                DrawerBackground = AdjustColor(new HSBColor(baseHue - 20, 0.4f, 0.1f), saturationShift, brightnessShift),
                ActionDefault = AdjustColor(new HSBColor(baseHue - 5, 0.3f, 0.5f), saturationShift, brightnessShift),
                ActionDisabled = AdjustColor(new HSBColor(baseHue - 5, 0.1f, 0.7f), saturationShift, brightnessShift),
                ActionDisabledBackground = AdjustColor(new HSBColor(baseHue - 30, 0.2f, 0.4f), saturationShift, brightnessShift),
                TextPrimary = AdjustColor(new HSBColor(baseHue + 50, 0.2f, 0.85f), saturationShift, brightnessShift),
                TextSecondary = AdjustColor(new HSBColor(baseHue + 30, 0.15f, 0.75f), saturationShift, brightnessShift),
                TextDisabled = AdjustColor(new HSBColor(baseHue + 50, 0.2f, 0.85f, 0.2f), saturationShift, brightnessShift),
                DrawerIcon = AdjustColor(new HSBColor(baseHue + 45, 0.2f, 0.7f), saturationShift, brightnessShift),
                DrawerText = AdjustColor(new HSBColor(baseHue + 45, 0.2f, 0.7f), saturationShift, brightnessShift),
                GrayLight = AdjustColor(new HSBColor(baseHue - 10, 0.3f, 0.25f), saturationShift, brightnessShift),
                GrayLighter = AdjustColor(new HSBColor(baseHue - 15, 0.5f, 0.2f), saturationShift, brightnessShift),
                Info = AdjustColor(new HSBColor(baseHue + 120, 0.5f, 0.7f), saturationShift, brightnessShift),
                Success = AdjustColor(new HSBColor(baseHue + 90, 0.5f, 0.6f), saturationShift, brightnessShift),
                Warning = AdjustColor(new HSBColor(baseHue + 40, 0.7f, 0.8f), saturationShift, brightnessShift),
                Error = AdjustColor(new HSBColor(baseHue, 0.7f, 0.7f), saturationShift, brightnessShift),
                LinesDefault = AdjustColor(new HSBColor(baseHue - 15, 0.4f, 0.3f), saturationShift, brightnessShift),
                TableLines = AdjustColor(new HSBColor(baseHue - 15, 0.4f, 0.3f), saturationShift, brightnessShift),
                Divider = AdjustColor(new HSBColor(baseHue - 20, 0.4f, 0.2f), saturationShift, brightnessShift),
                OverlayLight = AdjustColor(new HSBColor(baseHue - 15, 0.5f, 0.2f, 0.5f), saturationShift, brightnessShift),
            };

            return theme;
        }

        private static string AdjustColor(HSBColor hsbColor, float saturationShift, float brightnessShift)
        {
            hsbColor.Saturation = Math.Max(0, Math.Min(1, hsbColor.Saturation + saturationShift));
            hsbColor.Brightness = Math.Max(0, Math.Min(1, hsbColor.Brightness + brightnessShift));
            return ColorTranslator.ToHtml(hsbColor.ToColor());
        }
    }

    public struct HSBColor
    {
        public float Hue { get; set; }
        public float Saturation { get; set; }
        public float Brightness { get; set; }
        public float Alpha { get; set; }

        public HSBColor(float hue, float saturation, float brightness, float alpha = 1.0f)
        {
            Hue = hue;
            Saturation = saturation;
            Brightness = brightness;
            Alpha = alpha;
        }

        public Color ToColor()
        {
            int r = 0, g = 0, b = 0;
            if (Saturation == 0)
            {
                r = g = b = (int)(Brightness * 255.0f + 0.5f);
            }
            else
            {
                float h = (Hue == 360) ? 0 : Hue / 60;
                int i = (int)Math.Floor(h);
                float f = h - i;
                float p = Brightness * (1.0f - Saturation);
                float q = Brightness * (1.0f - Saturation * f);
                float t = Brightness * (1.0f - (Saturation * (1.0f - f)));
                switch (i)
                {
                    case 0:
                        r = (int)(Brightness * 255.0f + 0.5f);
                        g = (int)(t * 255.0f + 0.5f);
                        b = (int)(p * 255.0f + 0.5f);
                        break;
                    case 1:
                        r = (int)(q * 255.0f + 0.5f);
                        g = (int)(Brightness * 255.0f + 0.5f);
                        b = (int)(p * 255.0f + 0.5f);
                        break;
                    case 2:
                        r = (int)(p * 255.0f + 0.5f);
                        g = (int)(Brightness * 255.0f + 0.5f);
                        b = (int)(t * 255.0f + 0.5f);
                        break;
                    case 3:
                        r = (int)(p * 255.0f + 0.5f);
                        g = (int)(q * 255.0f + 0.5f);
                        b = (int)(Brightness * 255.0f + 0.5f);
                        break;
                    case 4:
                        r = (int)(t * 255.0f + 0.5f);
                        g = (int)(p * 255.0f + 0.5f);
                        b = (int)(Brightness * 255.0f + 0.5f);
                        break;
                    default:
                        r = (int)(Brightness * 255.0f + 0.5f);
                        g = (int)(p * 255.0f + 0.5f);
                        b = (int)(q * 255.0f + 0.5f);
                        break;
                }
            }
            return Color.FromArgb((int)(Alpha * 255.0f + 0.5f), r, g, b);
        }
    }
}
