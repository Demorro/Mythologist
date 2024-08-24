using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MudBlazor;
using System.Drawing;
using Color = System.Drawing.Color;
using MudBlazor.Interfaces;

namespace SharedLogic.Styling
{
    public class Styling
    {
        static Random rand = new Random();

        public static MudTheme MythologistTheme()
        {
            return new MudTheme()
            {
                PaletteDark = MythologistDarkPallete(),
                Typography = MythologistTypo(),
                LayoutProperties = new LayoutProperties()
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


        static string SatinSheenGold = "#84a343";
        static string SatinSheenGoldFaded = "#586d2c";
        static string FernGreen = "#486336";
        static string FernGreenFaded = "#304224";
        static string Pumpkin = "#FA7921";
        static string PrincetonOrange = "#FE9920";
        static string IndigoDye = "#0C4767";
        static string Ivory = "fefae0";

        private static PaletteDark MythologistDarkPallete()
        {
            return new PaletteDark() {
                Primary = SatinSheenGold,
                Secondary = FernGreen,
                Tertiary = Pumpkin,
                PrimaryContrastText = Ivory,
                SecondaryContrastText = Ivory,
                TertiaryContrastText = Ivory,
                Surface = "rgba(16,20,22,0.9)",
                Background = "#0f1417",
                BackgroundGray = "#171c1f",
                AppbarText = "#f8fbfe",
                AppbarBackground = "#101416",
                DrawerBackground = "rgba(16,20,22,1.0)",
                ActionDefault = SatinSheenGoldFaded,
                ActionDisabled = "#36434a",
                ActionDisabledBackground = "#0b0f11",
                TextPrimary = "#f8fbfe",
                TextSecondary = "#c3ccd3",
                TextDisabled = "#9ba4ab",
                DrawerIcon = "#E4B363",
                DrawerText = "#E8E9EB",
                GrayLight = "#E0DFD5",
                GrayLighter = "#E0DFD5",
                Info = Ivory,
                Success = SatinSheenGold,
                Warning = "#e3890f",
                Error = "#b63030",
                LinesDefault = "rgba(155,164,171,0.14)",
                TableLines = "rgba(155,164,171,0.14)",
                Divider = "rgba(155,164,171,0.14)",
                OverlayLight = "#FFFFFFAA"
            };
        }

        //You'll need to deploy the fonts and .css into the individual wwwroot folders
        public static Typography MythologistTypo() {
             return new Typography() {
                H1 = new H1(){
                        FontFamily = new[] { "Agreloy", "serif" },
                },
                H2 = new H2()
                {
                        FontFamily = new[] { "Agreloy", "serif" },
                        FontSize = "4.5rem"

                },
                H3 = new H3()
                {
                        FontFamily = new[] { "SuperNormal", "serif" }
                },
                H4 = new H4()
                {
                        FontFamily = new[] { "SuperNormal", "serif" }
                },
                H5 = new H5()
                {
                        FontFamily = new[] { "SuperNormal", "serif" }
                },
                Body1 = new Body1()
                {
                        FontFamily = new[] { "Vegur", "sans-serif" },
                },
                Body2 = new Body2()
                {
                        FontFamily = new[] { "Vegur", "sans-serif" },
                },
                Subtitle1 = new Subtitle1()
                {
                        FontFamily = new[] { "SuperNormal", "sans-serif" },
                },
                Subtitle2 = new Subtitle2()
                {
                        FontFamily = new[] { "Vegur", "sans-serif" },
                        FontSize = "1.1rem",
                },
                Button = new Button()
                {
                        FontFamily = new[] { "SuperNormal", "sans-serif" },
                        FontSize = "1rem",
                },
                Input = new Input()
                {
                        FontFamily = new[] { "Vegur", "sans-serif" },
                },
                Caption = new Caption()
                {
                        FontFamily = new[] { "Vegur", "sans-serif" },
                },
                Overline = new Overline()
                {
                        FontFamily = new[] { "Vegur", "sans-serif" },
                }
            };
        }
    }
}
