// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;

namespace osu.Game.Screens.Edit.Commands
{
    public interface ICommand<TContext>
    {
        Guid Id { get; }
        void Apply(TContext context);

        ICommand<TContext> GetInverseCommand();

        bool CanMerge(ICommand<TContext> other);

        void Merge(ICommand<TContext> other);

        string Description { get; }

        void OnAcknowledged(TContext context);

        void OnReceived(TContext context, IEnumerable<ICommand<TContext>> pendingCommands);
    }
}
