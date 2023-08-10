// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Game.Screens.Edit.Commands;

namespace osu.Game.Screens.Edit
{
    /// <summary>
    /// Interface for a component that manages changes with <see cref="ICommand"/>s in the <see cref="Editor"/>.
    /// </summary>
    [Cached]
    public interface IEditorCommandHandler : IEditorChangeHandler
    {
        /// <summary>
        /// Applies a command and logs it to the current transaction.
        /// </summary>
        /// <param name="command">The command to apply and log.</param>
        void ApplyCommand(ICommand command);

        /// <summary>
        /// Logs a command to the current transaction without applying it.
        /// </summary>
        /// <param name="command">The command to log.</param>
        void LogCommand(ICommand command);

        /// <summary>
        /// Fired whenever a <see cref="ICommand"/> is logged.
        /// </summary>
        event Action? CommandLogged;
    }
}
