// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Game.Screens.Edit;
using osu.Game.Screens.Edit.Commands;

namespace osu.Game.Online.Editor
{
    public class EditorConnector
    {
        private readonly EditorBeatmap editorBeatmap;
        private readonly LocalEditorServer server;

        private readonly Guid clientId = Guid.NewGuid();

        private readonly Queue<ICommand<EditorBeatmap>> pendingCommands = new Queue<ICommand<EditorBeatmap>>();

        public EditorConnector(
            EditorCommandHandler commandHandler,
            EditorBeatmap editorBeatmap,
            LocalEditorServer server
        )
        {
            this.editorBeatmap = editorBeatmap;
            this.server = server;

            commandHandler.CommandApplied += onCommandApplied;
            server.CommandReceived += onCommandReceived;
        }

        private void onCommandApplied(ICommand<EditorBeatmap> command)
        {
            pendingCommands.Enqueue(command);
            server.SendCommand(command);
        }

        private void onCommandReceived(Guid senderId, ICommand<EditorBeatmap> command)
        {
            if (senderId == clientId)
            {
                var pendingCommand = pendingCommands.Dequeue();

                if (pendingCommand.Id != command.Id)
                {
                    throw new InvalidOperationException($"Received command with id {command.Id} but expected {pendingCommand.Id}");
                }

                command.OnAcknowledged(editorBeatmap);
            }
            else
            {
                command.OnReceived(editorBeatmap, pendingCommands);
            }
        }
    }
}
