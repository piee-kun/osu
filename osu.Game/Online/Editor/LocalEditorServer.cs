// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Screens.Edit;
using osu.Game.Screens.Edit.Commands;

namespace osu.Game.Online.Editor
{
    public class LocalEditorServer : IEditorServer
    {
        public event Action<Guid, ICommand<EditorBeatmap>>? CommandReceived;

        public void SendCommand(ICommand<EditorBeatmap> command)
        {
            CommandReceived?.Invoke(Guid.Empty, command);
        }
    }
}
