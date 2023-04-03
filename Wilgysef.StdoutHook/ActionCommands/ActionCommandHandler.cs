using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Wilgysef.StdoutHook.ActionCommands
{
    public class ActionCommandHandler
    {
        private readonly IList<ActionCommand> _outputCommands;
        private readonly IList<ActionCommand> _errorCommands;

        public ActionCommandHandler(IList<ActionCommand> outputCommands, IList<ActionCommand> errorCommands)
        {
            _outputCommands = outputCommands;
            _errorCommands = errorCommands;
        }

        public void HandleOutput(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                HandleOutput(e.Data);
            }
        }

        public void HandleError(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                HandleError(e.Data);
            }
        }

        public void HandleOutput(string data)
        {
            Execute(new ActionCommandState(data), _outputCommands, OutputFinalAction);
        }

        public void HandleError(string data)
        {
            Execute(new ActionCommandState(data), _errorCommands, ErrorFinalAction);
        }

        private void Execute(ActionCommandState state, IList<ActionCommand> commands, Action<string> finalAction)
        {
            foreach (var command in commands)
            {
                if (command.Enabled && command.Predicate(state))
                {
                    state.Data = command.Execute(state);
                    if (command.Terminal)
                    {
                        break;
                    }
                }
            }

            finalAction(state.Data);
        }

        private static void OutputFinalAction(string data)
        {
            Console.Write(data);
        }

        private static void ErrorFinalAction(string data)
        {
            Console.Error.Write(data);
        }
    }
}
