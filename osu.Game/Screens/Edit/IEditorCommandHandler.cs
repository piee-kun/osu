// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Screens.Edit.Commands;

namespace osu.Game.Screens.Edit
{
    public interface IEditorCommandHandler : IEditorChangeHandler
    {
        public void ApplyCommand(ICommand<EditorBeatmap> command);

        public event Action<ICommand<EditorBeatmap>> CommandApplied;
    }
}
