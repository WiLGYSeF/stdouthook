using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Formatters
{
    internal class FormatComputeState
    {
        public DataState DataState { get; private set; }

        public ColorList? Colors { get; private set; }

        public int StartPosition { get; }

        public int Position { get; private set; }

        public FormatComputeState(DataState dataState, int position = 0)
        {
            DataState = dataState;
            StartPosition = position;
            Position = position;
        }

        public void SetPosition(int position)
        {
            Position = position;
        }

        public void AddColor(string color, int offset = 0)
        {
            Colors ??= new();
            Colors.AddColor(Position + offset, color, 0, color.Length);
        }
    }
}
