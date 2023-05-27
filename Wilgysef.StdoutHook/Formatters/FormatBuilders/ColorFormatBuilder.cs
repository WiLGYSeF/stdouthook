using System;
using System.Collections.Generic;
using System.Linq;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Formatters.FormatBuilders
{
    internal class ColorFormatBuilder : FormatBuilder
    {
        public static char Separator = ';';

        public static char Toggle = '^';

        private static readonly Dictionary<string, Color> Colors = new();

        static ColorFormatBuilder()
        {
            #region Builtin Colors

            AddColor("black", "k", 30, 40);
            AddColor("red", "r", 31, 41);
            AddColor("green", "g", 32, 42);
            AddColor("yellow", "y", 33, 43);
            AddColor("blue", "b", 34, 44);
            AddColor("magenta", "m", 35, 45);
            AddColor("cyan", "c", 36, 46);
            AddColor("gray", null, 37, 47);
            AddColor("grey", "e", 37, 47);
            AddColor("default", null, 39, 49);

            AddColor("darkgray", null, 90, 100);
            AddColor("darkgrey", "de", 90, 100);
            AddColor("lightblack", "lk", 90, 100);
            AddColor("lightred", "lr", 91, 101);
            AddColor("lightgreen", "lg", 92, 102);
            AddColor("lightyellow", "ly", 93, 103);
            AddColor("lightblue", "lb", 94, 104);
            AddColor("lightmagenta", "lm", 95, 105);
            AddColor("lightcyan", "lc", 96, 106);
            AddColor("lightgray", null, 97, 107);
            AddColor("lightgrey", "le", 97, 107);
            AddColor("white", "w", 97, 107);

            AddColor("z", null, 0, 0);
            AddColor("reset", "res", 0, 0);
            AddColor("normal", "nor", 0, 0);
            AddColor("bold", "bol", 1, 21);
            AddColor("bright", "bri", 1, 21);
            AddColor("dim", null, 2, 22);
            AddColor("italic", "ita", 3, 23);
            AddColor("underline", "ul", 4, 24);
            AddColor("underlined", "und", 4, 24);
            AddColor("blink", "bli", 5, 25);
            AddColor("invert", "inv", 7, 27);
            AddColor("reverse", "rev", 7, 27);
            AddColor("hidden", "hid", 8, 28);
            AddColor("conceal", "con", 8, 28);
            AddColor("strike", "str", 9, 29);
            AddColor("strikethrough", null, 9, 29);
            AddColor("crossed", "cro", 9, 29);
            AddColor("crossedout", null, 9, 29);

            AddColor("overline", "ol", 53, 55);
            AddColor("overlined", "ove", 53, 55);

            #endregion

            #region Custom Colors

            AddCustomColor("aliceblue", 0xf0f8ff);
            AddCustomColor("antiquewhite", 0xfaebd7);
            AddCustomColor("aqua", 0x00ffff);
            AddCustomColor("aquamarine", 0x7fffd4);
            AddCustomColor("azure", 0xf0ffff);
            AddCustomColor("beige", 0xf5f5dc);
            AddCustomColor("bisque", 0xffe4c4);
            AddCustomColor("blanchedalmond", 0xffebcd);
            AddCustomColor("blueviolet", 0x8a2be2);
            AddCustomColor("brown", 0xa52a2a);
            AddCustomColor("burlywood", 0xdeb887);
            AddCustomColor("cadetblue", 0x5f9ea0);
            AddCustomColor("chartreuse", 0x7fff00);
            AddCustomColor("chocolate", 0xd2691e);
            AddCustomColor("coral", 0xff7f50);
            AddCustomColor("cornflowerblue", 0x6495ed);
            AddCustomColor("cornsilk", 0xfff8dc);
            AddCustomColor("crimson", 0xdc143c);
            AddCustomColor("darkblue", 0x00008b);
            AddCustomColor("darkcyan", 0x008b8b);
            AddCustomColor("darkgoldenrod", 0xb8860b);
            AddCustomColor("darkgreen", 0x006400);
            AddCustomColor("darkkhaki", 0xbdb76b);
            AddCustomColor("darkmagenta", 0x8b008b);
            AddCustomColor("darkolivegreen", 0x556b2f);
            AddCustomColor("darkorange", 0xff8c00);
            AddCustomColor("darkorchid", 0x9932cc);
            AddCustomColor("darkred", 0x8b0000);
            AddCustomColor("darksalmon", 0xe9967a);
            AddCustomColor("darkseagreen", 0x8fbc8f);
            AddCustomColor("darkslateblue", 0x483d8b);
            AddCustomColor("darkslategray", 0x2f4f4f);
            AddCustomColor("darkslategrey", 0x2f4f4f);
            AddCustomColor("darkturquoise", 0x00ced1);
            AddCustomColor("darkviolet", 0x9400d3);
            AddCustomColor("deeppink", 0xff1493);
            AddCustomColor("deepskyblue", 0x00bfff);
            AddCustomColor("dimgray", 0x696969);
            AddCustomColor("dimgrey", 0x696969);
            AddCustomColor("dodgerblue", 0x1e90ff);
            AddCustomColor("firebrick", 0xb22222);
            AddCustomColor("floralwhite", 0xfffaf0);
            AddCustomColor("forestgreen", 0x228b22);
            AddCustomColor("fuchsia", 0xff00ff);
            AddCustomColor("gainsboro", 0xdcdcdc);
            AddCustomColor("ghostwhite", 0xf8f8ff);
            AddCustomColor("gold", 0xffd700);
            AddCustomColor("goldenrod", 0xdaa520);
            AddCustomColor("greenyellow", 0xadff2f);
            AddCustomColor("honeydew", 0xf0fff0);
            AddCustomColor("hotpink", 0xff69b4);
            AddCustomColor("indianred", 0xcd5c5c);
            AddCustomColor("indigo", 0x4b0082);
            AddCustomColor("ivory", 0xfffff0);
            AddCustomColor("khaki", 0xf0e68c);
            AddCustomColor("lavender", 0xe6e6fa);
            AddCustomColor("lavenderblush", 0xfff0f5);
            AddCustomColor("lawngreen", 0x7cfc00);
            AddCustomColor("lemonchiffon", 0xfffacd);
            AddCustomColor("lightcoral", 0xf08080);
            AddCustomColor("lightgoldenrodyellow", 0xfafad2);
            AddCustomColor("lightpink", 0xffb6c1);
            AddCustomColor("lightsalmon", 0xffa07a);
            AddCustomColor("lightseagreen", 0x20b2aa);
            AddCustomColor("lightskyblue", 0x87cefa);
            AddCustomColor("lightslategray", 0x778899);
            AddCustomColor("lightslategrey", 0x778899);
            AddCustomColor("lightsteelblue", 0xb0c4de);
            AddCustomColor("lime", 0x00ff00);
            AddCustomColor("limegreen", 0x32cd32);
            AddCustomColor("linen", 0xfaf0e6);
            AddCustomColor("maroon", 0x800000);
            AddCustomColor("mediumaquamarine", 0x66cdaa);
            AddCustomColor("mediumblue", 0x0000cd);
            AddCustomColor("mediumorchid", 0xba55d3);
            AddCustomColor("mediumpurple", 0x9370db);
            AddCustomColor("mediumseagreen", 0x3cb371);
            AddCustomColor("mediumslateblue", 0x7b68ee);
            AddCustomColor("mediumspringgreen", 0x00fa9a);
            AddCustomColor("mediumturquoise", 0x48d1cc);
            AddCustomColor("mediumvioletred", 0xc71585);
            AddCustomColor("midnightblue", 0x191970);
            AddCustomColor("mintcream", 0xf5fffa);
            AddCustomColor("mistyrose", 0xffe4e1);
            AddCustomColor("moccasin", 0xffe4b5);
            AddCustomColor("navajowhite", 0xffdead);
            AddCustomColor("navy", 0x000080);
            AddCustomColor("oldlace", 0xfdf5e6);
            AddCustomColor("olive", 0x808000);
            AddCustomColor("olivedrab", 0x6b8e23);
            AddCustomColor("orange", 0xffa500);
            AddCustomColor("orangered", 0xff4500);
            AddCustomColor("orchid", 0xda70d6);
            AddCustomColor("palegoldenrod", 0xeee8aa);
            AddCustomColor("palegreen", 0x98fb98);
            AddCustomColor("paleturquoise", 0xafeeee);
            AddCustomColor("palevioletred", 0xdb7093);
            AddCustomColor("papayawhip", 0xffefd5);
            AddCustomColor("peachpuff", 0xffdab9);
            AddCustomColor("peru", 0xcd853f);
            AddCustomColor("pink", 0xffc0cb);
            AddCustomColor("plum", 0xdda0dd);
            AddCustomColor("powderblue", 0xb0e0e6);
            AddCustomColor("purple", 0x800080);
            AddCustomColor("rebeccapurple", 0x663399);
            AddCustomColor("rosybrown", 0xbc8f8f);
            AddCustomColor("royalblue", 0x4169e1);
            AddCustomColor("saddlebrown", 0x8b4513);
            AddCustomColor("salmon", 0xfa8072);
            AddCustomColor("sandybrown", 0xf4a460);
            AddCustomColor("seagreen", 0x2e8b57);
            AddCustomColor("seashell", 0xfff5ee);
            AddCustomColor("sienna", 0xa0522d);
            AddCustomColor("silver", 0xc0c0c0);
            AddCustomColor("skyblue", 0x87ceeb);
            AddCustomColor("slateblue", 0x6a5acd);
            AddCustomColor("slategray", 0x708090);
            AddCustomColor("slategrey", 0x708090);
            AddCustomColor("snow", 0xfffafa);
            AddCustomColor("springgreen", 0x00ff7f);
            AddCustomColor("steelblue", 0x4682b4);
            AddCustomColor("tan", 0xd2b48c);
            AddCustomColor("teal", 0x008080);
            AddCustomColor("thistle", 0xd8bfd8);
            AddCustomColor("tomato", 0xff6347);
            AddCustomColor("turquoise", 0x40e0d0);
            AddCustomColor("violet", 0xee82ee);
            AddCustomColor("wheat", 0xf5deb3);
            AddCustomColor("whitesmoke", 0xf5f5f5);
            AddCustomColor("yellowgreen", 0x9acd32);

            #endregion

            static void AddColor(string name, string? shortName, int value, int toggleValue)
            {
                var color = new Color(value, toggleValue, true);
                Colors.Add(name, color);

                if (shortName != null)
                {
                    Colors.Add(shortName, color);
                }
            }

            static void AddCustomColor(string name, int value)
            {
                Colors.Add(name, new Color(value, value, false));
            }
        }

        public override string? Key => "color";

        public override char? KeyShort => 'C';

        public IDictionary<string, string> CustomColors { get; set; } = new Dictionary<string, string>();

        public override Func<DataState, string> Build(FormatBuildState state, out bool isConstant)
        {
            isConstant = true;

            if (state.Contents.StartsWith("raw"))
            {
                var rawColor = state.Contents[3..];
                return _ => $"\x1b[{rawColor}m";
            }

            var colors = new List<string>(state.Contents.Split(Separator));

            for (var i = 0; i < colors.Count; i++)
            {
                var color = colors[i].Trim();

                if (CustomColors.TryGetValue(color, out var colorAlias))
                {
                    var aliasColors = colorAlias.Split(Separator);
                    if (aliasColors.Length > 1)
                    {
                        colors[i] = aliasColors[0].Trim();
                        colors.InsertRange(
                            i + 1,
                            aliasColors.Skip(1).Select(c => c.Trim()));
                    }
                    else
                    {
                        colors[i] = colorAlias.Trim();
                    }
                }
                else
                {
                    colors[i] = color;
                }
            }

            var colorResults = new List<int>(colors.Count);

            for (var i = 0; i < colors.Count; i++)
            {
                var colorStr = colors[i];
                if (colorStr.Length == 0)
                {
                    continue;
                }

                var toggle = false;

                if (colorStr[0] == Toggle)
                {
                    colorStr = colorStr[1..];
                    toggle = true;
                }

                if (Colors.TryGetValue(colorStr, out var color))
                {
                    if (color.IsSimple)
                    {
                        colorResults.Add(toggle ? color.ToggleValue : color.Value);
                    }
                    else
                    {
                        AddColor24Bit(colorResults, color.Value, toggle);
                    }
                }
                else if (int.TryParse(colorStr, out var colorInt) && colorInt >= 0 && colorInt <= 255)
                {
                    colorResults.Add(toggle ? 48 : 38);
                    colorResults.Add(5);
                    colorResults.Add(colorInt);
                }
                else if (TryParseHexToInt(colorStr, out var colorHex))
                {
                    AddColor24Bit(colorResults, colorHex, toggle);
                }
            }

            if (colorResults.Count == 0)
            {
                return _ => "";
            }

            // only close over the result
            var result = $"\x1b[{string.Join(';', colorResults)}m";
            return _ => result;

            static void AddColor24Bit(List<int> colors, int value, bool toggle)
            {
                colors.Add(toggle ? 48 : 38);
                colors.Add(2);
                colors.Add((value >> 16) & 0xFF);
                colors.Add((value >> 8) & 0xFF);
                colors.Add(value & 0xFF);
            }
        }

        private static bool TryParseHexToInt(string hex, out int value)
        {
            if (hex.StartsWith("0x"))
            {
                if (hex.Length != 8)
                {
                    value = 0;
                    return false;
                }

                hex = hex[2..];
            }
            else if (hex.Length != 6)
            {
                value = 0;
                return false;
            }

            try
            {
                value = Convert.ToInt32(hex, 16);
                return true;
            }
            catch
            {
                value = 0;
                return false;
            }
        }

        private class Color
        {
            public int Value { get; }

            public int ToggleValue { get; }

            public bool IsSimple { get; }

            public Color(int value, int toggleValue, bool isSimple)
            {
                Value = value;
                ToggleValue = toggleValue;
                IsSimple = isSimple;
            }
        }
    }
}
