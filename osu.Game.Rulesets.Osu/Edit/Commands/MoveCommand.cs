// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Game.Localisation;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Screens.Edit;
using osu.Game.Screens.Edit.Commands;
using osuTK;

namespace osu.Game.Rulesets.Osu.Edit.Commands
{
    public class MoveCommand : ICommand<EditorBeatmap>
    {
        private readonly Guid[] targetIDs;
        private Vector2[] newPositions;
        private readonly Vector2[] oldPositions;

        public MoveCommand(IEnumerable<Guid> targetIDs, IEnumerable<Vector2> newPositions, IEnumerable<Vector2> oldPositions)
        {
            this.targetIDs = targetIDs.ToArray();
            this.newPositions = newPositions.ToArray();
            this.oldPositions = oldPositions.ToArray();
        }

        public MoveCommand(IEnumerable<OsuHitObject> targets, IEnumerable<Vector2> newPositions)
        {
            targetIDs = targets.Select(t => t.Id).ToArray();
            oldPositions = targets.Select(t => t.Position).ToArray();
            this.newPositions = newPositions.ToArray();
        }

        public Guid Id { get; } = Guid.NewGuid();

        public void Apply(EditorBeatmap context)
        {
            for (int i = 0; i < targetIDs.Length; i++)
            {
                var target = context.HitObjects.FirstOrDefault(h => h.Id == targetIDs[i]);
                if (target is OsuHitObject osuHitObject)
                    osuHitObject.Position = newPositions[i];
            }
        }

        public ICommand<EditorBeatmap> GetInverseCommand()
        {
            return new MoveCommand(targetIDs, oldPositions, newPositions);
        }

        public bool CanMerge(ICommand<EditorBeatmap> other)
        {
            return other is MoveCommand otherMoveCommand && otherMoveCommand.targetIDs.SequenceEqual(targetIDs);
        }

        public void Merge(ICommand<EditorBeatmap> other)
        {
            newPositions = ((MoveCommand)other).newPositions;
        }

        public string Description => EditorStrings.MoveObjects(targetIDs.Length).ToString();

        public void OnAcknowledged(EditorBeatmap context)
        {
        }

        public void OnReceived(EditorBeatmap context, IEnumerable<ICommand<EditorBeatmap>> pendingCommands)
        {
            for (int i = 0; i < targetIDs.Length; i++)
            {
                var id = targetIDs[i];
                if (pendingCommands.Any(c => c is MoveCommand moveCommand && moveCommand.targetIDs.Contains(id)))
                    continue;

                var target = context.HitObjects.FirstOrDefault(h => h.Id == targetIDs[i]);
                if (target is OsuHitObject osuHitObject)
                    osuHitObject.Position = newPositions[i];
            }
        }
    }
}
