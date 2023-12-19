using PdfSharp.Fonts;

namespace THC_CHURCH_WEBSITE.Pages
{
    public class MyFontResolver : IFontResolver
    {
            private static bool _fontResolverSet = false;

            public MyFontResolver()
            {
                if (!_fontResolverSet)
                {
                    // Set the font resolver here
                    GlobalFontSettings.FontResolver = this;
                    _fontResolverSet = true;
                }
            }

            public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
            {
                // Add logic to resolve the typeface here
                if (familyName.Equals("Times New Roman", StringComparison.OrdinalIgnoreCase))
                {
                    if (isBold && isItalic)
                    {
                        return new FontResolverInfo("TimesNewRoman#bi");
                    }
                    else if (isBold)
                    {
                        return new FontResolverInfo("TimesNewRoman#b");
                    }
                    else if (isItalic)
                    {
                        return new FontResolverInfo("TimesNewRoman#i");
                    }
                    else
                    {
                        return new FontResolverInfo("TimesNewRoman");
                    }
                }
                return PlatformFontResolver.ResolveTypeface(familyName, isBold, isItalic);
            }

            public byte[] GetFont(string faceName)
            {
                // Add logic to get the font data here
                switch (faceName)
                {
                    case "TimesNewRoman":
                        return FontHelper.TimesNewRomanRegularData;
                    case "TimesNewRoman#b":
                        return FontHelper.TimesNewRomanBoldData;
                    case "TimesNewRoman#i":
                        return FontHelper.TimesNewRomanItalicData;
                    case "TimesNewRoman#bi":
                        return FontHelper.TimesNewRomanBoldItalicData;
                }
                return null;
            }
        }


        public static class FontHelper
        {
            public static byte[] TimesNewRomanRegularData { get; set; }
            public static byte[] TimesNewRomanBoldData { get; set; }
            public static byte[] TimesNewRomanItalicData { get; set; }
            public static byte[] TimesNewRomanBoldItalicData { get; set; }

            static FontHelper()
            {
                // Load the font data from files
                TimesNewRomanRegularData = File.ReadAllBytes("Fonts/times.ttf");
                TimesNewRomanBoldData = File.ReadAllBytes("Fonts/timesbd.ttf");
                TimesNewRomanBoldItalicData = File.ReadAllBytes("Fonts/timesbi.ttf");
                TimesNewRomanItalicData = File.ReadAllBytes("Fonts/timesi.ttf");
            }
        }

    }


