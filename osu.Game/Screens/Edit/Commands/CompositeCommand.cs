// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Screens.Edit.Commands
{
    public class CompositeCommand : ICommand
    {
        private readonly List<ICommand> commands = new List<ICommand>();

        public void AddCommand(ICommand command)
        {
            if (commands.Count > 0 && commands[^1].CanMerge(command))
            {
                commands[^1].Merge(command);
                return;
            }

            commands.Add(command);
        }

        public int Count => commands.Count;

        public void Apply()
        {
            foreach (var command in commands)
            {
                command.Apply();
            }
        }

        public ICommand GetInverseCommand()
        {
            var inverse = new CompositeCommand();

            for (int i = commands.Count - 1; i >= 0; i--)
            {
                inverse.AddCommand(commands[i].GetInverseCommand());
            }

            return inverse;
        }

        public bool CanMerge(ICommand other) => false;

        public void Merge(ICommand other)
        {
            throw new System.NotImplementedException();
        }

        public string Description => string.Join(", ", commands.Select(c => c.Description).Distinct());
    }
}
