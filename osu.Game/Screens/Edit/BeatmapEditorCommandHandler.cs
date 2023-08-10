// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Screens.Edit.Commands;

namespace osu.Game.Screens.Edit
{
    public partial class BeatmapEditorCommandHandler : EditorCommandHandler
    {
        /// <summary>
        /// Creates a new <see cref="BeatmapEditorCommandHandler"/>.
        /// </summary>
        /// <param name="editorBeatmap">The <see cref="EditorBeatmap"/> to track the <see cref="ICommand"/>s of.</param>
        public BeatmapEditorCommandHandler(EditorBeatmap editorBeatmap)
        {
            editorBeatmap.TransactionBegan += BeginChange;
            editorBeatmap.TransactionEnded += EndChange;
            editorBeatmap.SaveStateTriggered += SaveState;
            editorBeatmap.CommandApplied += LogCommand;
        }
    }
}
