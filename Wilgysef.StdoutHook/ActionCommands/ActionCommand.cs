namespace Wilgysef.StdoutHook.ActionCommands
{
    public abstract class ActionCommand
    {
        public virtual bool Enabled { get; set; } = true;

        public virtual bool Terminal { get; set; }

        public abstract bool Predicate(ActionCommandState state);

        public abstract string Execute(ActionCommandState state);
    }
}
