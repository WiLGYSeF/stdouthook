namespace Wilgysef.StdoutHook.Profiles.Dtos
{
    public class ArgumentPatternDto
    {
        public bool? Enabled { get; set; }

        public string? ArgumentExpression { get; set; }

        public int? MinPosition { get; set; }

        public int? MaxPosition { get; set; }

        public bool? MustNotMatch { get; set; }
    }
}
