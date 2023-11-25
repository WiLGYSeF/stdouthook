namespace Wilgysef.StdoutHook.Rules
{
    internal class MatchGroup
    {
        public MatchGroup(string value, string name, int index)
        {
            Value = value;
            Name = name;
            Index = index;
            EndIndex = Index + Value.Length;
        }

        public string Value { get; internal set; }

        public string Name { get; }

        public int Index { get; }

        public int EndIndex { get; }
    }
}
