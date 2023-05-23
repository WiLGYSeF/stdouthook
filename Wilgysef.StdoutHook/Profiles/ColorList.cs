using System;
using System.Collections.Generic;

namespace Wilgysef.StdoutHook.Profiles
{
    internal class ColorList
    {
        public ColorEntry this[int index] => _colors[index];

        public int Count { get; private set; }

        private readonly List<ColorEntry> _colors = new List<ColorEntry>();

        public void AddColor(int position, string str, int startIndex, int endIndex)
        {
            _colors.Add(new ColorEntry(position, str, startIndex, endIndex));
            Count++;
        }

        public int GetColorIndex(int position, int start = 0)
        {
            var colorIndex = start;
            for (; colorIndex < Count && _colors[colorIndex].Position < position; colorIndex++) ;

            return colorIndex;
        }

        public void Clear()
        {
            _colors.Clear();
            Count = 0;
        }

        public class ColorEntry
        {
            public int Position { get; }

            public ReadOnlySpan<char> Color => _str.AsSpan()[_startIndex.._endIndex];

            private readonly string _str;
            private readonly int _startIndex;
            private readonly int _endIndex;

            public ColorEntry(int position, string str, int startIndex, int endIndex)
            {
                Position = position;
                _str = str;
                _startIndex = startIndex;
                _endIndex = endIndex;
            }
        }
    }
}
