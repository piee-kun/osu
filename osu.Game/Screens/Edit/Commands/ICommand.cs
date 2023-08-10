// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;

namespace osu.Game.Screens.Edit.Commands
{
    public interface ICommand
    {
        /// <summary>
        /// Applies the effect of this command.
        /// </summary>
        void Apply();

        /// <summary>
        /// Gets a command that is the inverse of this command.
        /// </summary>
        /// <returns>The command that is the inverse of this command.</returns>
        ICommand GetInverseCommand();

        /// <summary>
        /// Checks whether this command can be combined with another future command.
        /// </summary>
        /// <param name="other">The other future command.</param>
        /// <returns>Shether this command can be combined with another future command.</returns>
        bool CanConsume(ICommand other);

        /// <summary>
        /// Consumes another future command, combining the effects with this one.
        /// </summary>
        /// <param name="other">The future command to consume.</param>
        void Consume(ICommand other);

        /// <summary>
        /// The name of this command.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the objects that are affected by this command.
        /// </summary>
        /// <returns>The objects that are affected by this command.</returns>
        IEnumerable<object> GetTargets();
    }
}
