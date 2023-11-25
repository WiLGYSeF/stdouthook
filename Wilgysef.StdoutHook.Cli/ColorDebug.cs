using Wilgysef.StdoutHook.Formatters;

namespace Wilgysef.StdoutHook.Cli;

public static class ColorDebug
{
    public static void WriteColorDebug(TextWriter writer)
    {
        var formatter = new ColorFormatter();

        var reset = formatter.Format("%Cz");

        writer.WriteLine("styles:");

        var styles = new[]
        {
            "nor", "bol", "dim", "ita", "und", "bli", "inv", "hid", "str", "ove",
        };

        foreach (var style in styles)
        {
            writer.Write(formatter.Format($"%C({style}) {style} %Cz"));
        }

        writer.WriteLine(reset);

        writer.WriteLine();
        writer.WriteLine("bold on and off");
        writer.WriteLine(formatter.Format("%C(bol) on %Cz%C(^bol) off %Cz"));

        writer.WriteLine();
        writer.WriteLine("3-bit color palette (normal and light):");
        Write3BitColors(false, false);
        Write3BitColors(false, true);
        Write3BitColors(true, false);
        Write3BitColors(true, true);

        writer.WriteLine();
        writer.WriteLine("8-bit color palette:");
        Write8BitColors();

        writer.WriteLine();
        writer.WriteLine("24-bit color palette sample:");
        Write24BitColors(false);
        Write24BitColors(true);

        void Write3BitColors(bool background, bool light)
        {
            var colors = new[]
            {
                "black", "red", "green", "yellow", "blue", "magenta", "cyan", "gray",
            };

            var backgroundStr = background ? "^" : "";
            var lightStr = light ? "light" : "";

            foreach (var color in colors)
            {
                writer.Write(formatter.Format($"%C({backgroundStr}{lightStr}{color}) {color[..3]} "));
            }

            writer.WriteLine(reset);
        }

        void Write8BitColors()
        {
            for (var row = 0; row < 32; row++)
            {
                foreach (var toggle in new[] { "", "^" })
                {
                    for (var col = 0; col < 8; col++)
                    {
                        var val = row * 8 + col;
                        writer.Write(formatter.Format($"%C({toggle}{val}) {val,3} "));
                    }

                    writer.Write(reset);
                }

                writer.WriteLine(reset);
            }
        }

        void Write24BitColors(bool background)
        {
            var colWidth = 80;
            var rows = 24;

            var hIncr = (int)MathF.Ceiling(256f / (colWidth / 8));
            var vIncr = (int)MathF.Ceiling(256f / (rows - 1));

            var backgroundStr = background ? "^" : "";

            for (var v = 0; v < 256; v += vIncr)
            {
                WriteRow((float)v / 256);
            }

            WriteRow(1);

            void WriteRow(float v)
            {
                for (var h = 0; h < 256; h += hIncr)
                {
                    HsvToRgb((float)h / 256, 1, v, out var r, out var g, out var b);
                    var color = (r << 16) | (g << 8) | b;

                    writer.Write(formatter.Format($"%C({backgroundStr}0x{color:X6}) {color:X6} "));
                }

                writer.WriteLine(reset);
            }
        }

        static void HsvToRgb(float h, float s, float v, out int r, out int g, out int b)
        {
            float i = MathF.Floor(h * 6);
            float f = h * 6 - i;
            float p = v * (1 - s);
            float q = v * (1 - f * s);
            float t = v * (1 - (1 - f) * s);

            switch (i % 6)
            {
                case 0: r = (int)(v * 255); g = (int)(t * 255); b = (int)(p * 255); break;
                case 1: r = (int)(q * 255); g = (int)(v * 255); b = (int)(p * 255); break;
                case 2: r = (int)(p * 255); g = (int)(v * 255); b = (int)(t * 255); break;
                case 3: r = (int)(p * 255); g = (int)(q * 255); b = (int)(v * 255); break;
                case 4: r = (int)(t * 255); g = (int)(p * 255); b = (int)(v * 255); break;
                case 5: r = (int)(v * 255); g = (int)(p * 255); b = (int)(q * 255); break;
                default: r = g = b = 0; break; // never reached
            }
        }
    }
}
