// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Beatmaps;
using osu.Game.Rulesets.Objects;

namespace osu.Game.Screens.Edit.Commands
{
    public class EditorCommandContext<THitObject>
        where THitObject : HitObject
    {
        public readonly EditorBeatmap EditorBeatmap;

        public IBeatmap<THitObject> Beatmap => (IBeatmap<THitObject>)EditorBeatmap.PlayableBeatmap;

        public EditorCommandContext(EditorBeatmap editorBeatmap)
        {
            EditorBeatmap = editorBeatmap;
        }
    }
}
