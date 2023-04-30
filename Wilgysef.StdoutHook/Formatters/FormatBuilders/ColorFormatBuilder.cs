using System;
using System.Collections.Generic;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Formatters.FormatBuilders
{
    internal class ColorFormatBuilder : FormatBuilder
    {
        public static char Separator = ';';

        public static char Toggle = '^';

        private static readonly Dictionary<string, Color> Colors = new Dictionary<string, Color>();

        static ColorFormatBuilder()
        {
            AddColor(new Color("black", "k", 30));
            AddColor(new Color("red", "r", 31));
            AddColor(new Color("green", "g", 32));
            AddColor(new Color("yellow", "y", 33));
            AddColor(new Color("blue", "b", 34));
            AddColor(new Color("magenta", "m", 35));
            AddColor(new Color("cyan", "c", 36));
            AddColor(new Color("gray", 37));
            AddColor(new Color("grey", "e", 37));
            AddColor(new Color("default", 39));

            AddColor(new Color("darkgray", 90));
            AddColor(new Color("darkgrey", "de", 90));
            AddColor(new Color("lightblack", "lk", 90));
            AddColor(new Color("lightred", "lr", 91));
            AddColor(new Color("lightgreen", "lg", 92));
            AddColor(new Color("lightyellow", "ly", 93));
            AddColor(new Color("lightblue", "lb", 94));
            AddColor(new Color("lightmagenta", "lm", 95));
            AddColor(new Color("lightcyan", "lc", 96));
            AddColor(new Color("lightgray", 97));
            AddColor(new Color("lightgrey", "le", 97));
            AddColor(new Color("white", "w", 97));

            AddColor(new Style("z", 0));
            AddColor(new Style("reset", "res", 0));
            AddColor(new Style("normal", "nor", 0));
            AddColor(new Style("bold", "bol", 1));
            AddColor(new Style("bright", "bri", 1));
            AddColor(new Style("dim", 2));
            AddColor(new Style("italic", "ita", 3));
            AddColor(new Style("underline", "ul", 4));
            AddColor(new Style("underlined", "und", 4));
            AddColor(new Style("blink", "bli", 5));
            AddColor(new Style("invert", "inv", 7));
            AddColor(new Style("reverse", "rev", 7));
            AddColor(new Style("hidden", "hid", 8));
            AddColor(new Style("conceal", "con", 8));
            AddColor(new Style("strike", "str", 9));
            AddColor(new Style("strikethrough", 9));
            AddColor(new Style("crossed", "cro", 9));
            AddColor(new Style("crossedout", 9));

            AddColor(new Style("overline", "ol", 53, 55));
            AddColor(new Style("overlined", "ove", 53, 55));

            static void AddColor(Color color)
            {
                Colors.Add(color.Name, color);

                if (color.ShortName != null)
                {
                    Colors.Add(color.ShortName, color);
                }
            }
        }

        public override string? Key => "color";

        public override char? KeyShort => 'C';

        public override Func<DataState, string> Build(FormatBuildState state, out bool isConstant)
        {
            var colors = state.Contents.Split(Separator);
            var colorResults = new List<int>(colors.Length);

            for (var i = 0; i < colors.Length; i++)
            {
                var colorStr = colors[i].Trim();

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
                    colorResults.Add(color.GetValue(toggle));
                }
            }

            isConstant = true;

            if (colorResults.Count == 0)
            {
                return _ => "";
            }

            var result = $@"\x1b[{string.Join(';', colorResults)}m";

            // only close over the result
            return _ => result;
        }

        private class Color
        {
            public string Name => _name;

            public string? ShortName => _shortName;

            protected readonly int _value;
            private readonly string _name;
            private readonly string? _shortName;

            public Color(string name, int value) : this(name, null, value) { }

            public Color(string name, string? shortName, int value)
            {
                _name = name;
                _shortName = shortName;
                _value = value;
            }

            public virtual int GetValue(bool toggle = false) => toggle ? _value + 10 : _value;
        }

        private class Style : Color
        {
            private readonly int? _toggleValue;

            public Style(string name, int value, int? toggleValue = null) : base(name, value)
            {
                _toggleValue = toggleValue;
            }

            public Style(string name, string? shortName, int value, int? toggleValue = null) : base(name, shortName, value)
            {
                _toggleValue = toggleValue;
            }

            public override int GetValue(bool toggle = false) => toggle
                ? _toggleValue ?? _value + 20
                : _value;
        }
    }
}
