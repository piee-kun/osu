// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Game.Localisation;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Screens.Edit.Commands;
using osuTK;

namespace osu.Game.Rulesets.Osu.Edit.Commands
{
    public class MoveCommand : ICommand
    {
        private readonly OsuHitObject[] targets;
        private readonly Vector2[] oldPositions;
        private Vector2[] newPositions;

        public MoveCommand(IEnumerable<OsuHitObject> targets, IEnumerable<Vector2> newPositions)
        {
            this.targets = targets.ToArray();
            oldPositions = this.targets.Select(t => t.Position).ToArray();
            this.newPositions = newPositions.ToArray();
        }

        public void Apply()
        {
            for (int i = 0; i < targets.Length; i++)
                targets[i].Position = newPositions[i];
        }

        public ICommand GetInverseCommand()
        {
            return new MoveCommand(targets, oldPositions);
        }

        public bool CanMerge(ICommand other)
        {
            return other is MoveCommand otherMoveCommand && otherMoveCommand.targets.SequenceEqual(targets);
        }

        public void Merge(ICommand other)
        {
            newPositions = ((MoveCommand)other).newPositions;
        }

        public string Description => EditorStrings.MoveObjects(targets.Length).ToString();
    }
}
