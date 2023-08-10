// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Screens.Edit.Commands
{
    public class CompositeCommand<TContext> : ICommand<TContext>
    {
        public Guid Id { get; } = Guid.NewGuid();

        private readonly List<ICommand<TContext>> commands = new List<ICommand<TContext>>();

        public void AddCommand(ICommand<TContext> command)
        {
            if (commands.Count > 0 && commands[^1].CanMerge(command))
            {
                commands[^1].Merge(command);
                return;
            }

            commands.Add(command);
        }

        public int Count => commands.Count;

        public void Apply(TContext context)
        {
            foreach (var command in commands)
            {
                command.Apply(context);
            }
        }

        public ICommand<TContext> GetInverseCommand()
        {
            var inverse = new CompositeCommand<TContext>();

            for (int i = commands.Count - 1; i >= 0; i--)
            {
                inverse.AddCommand(commands[i].GetInverseCommand());
            }

            return inverse;
        }

        public bool CanMerge(ICommand<TContext> other) => false;

        public void Merge(ICommand<TContext> other)
        {
            throw new System.NotImplementedException();
        }

        public string Description => string.Join(", ", commands.Select(c => c.Description).Distinct());

        public void OnAcknowledged(TContext context)
        {
            foreach (var command in commands)
            {
                command.OnAcknowledged(context);
            }
        }

        public void OnReceived(TContext context, IEnumerable<ICommand<TContext>> pendingCommands)
        {
            foreach (var command in commands)
            {
                command.OnReceived(context, pendingCommands);
            }
        }
    }
}
