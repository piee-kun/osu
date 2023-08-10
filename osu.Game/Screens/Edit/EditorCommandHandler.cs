// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Framework.Bindables;
using osu.Game.Screens.Edit.Commands;

namespace osu.Game.Screens.Edit
{
    /// <summary>
    /// Tracks changes to the <see cref="Editor"/>.
    /// </summary>
    public partial class EditorCommandHandler : TransactionalCommitComponent, IEditorCommandHandler
    {
        public readonly Bindable<bool> CanUndo = new Bindable<bool>();
        public readonly Bindable<bool> CanRedo = new Bindable<bool>();

        private readonly LinkedList<ICommand<EditorBeatmap>> undoStack = new LinkedList<ICommand<EditorBeatmap>>();
        private readonly Stack<ICommand<EditorBeatmap>> redoStack = new Stack<ICommand<EditorBeatmap>>();

        private readonly EditorBeatmap editorBeatmap;

        public EditorCommandHandler(EditorBeatmap editorBeatmap)
        {
            this.editorBeatmap = editorBeatmap;
        }

        private CompositeCommand<EditorBeatmap> currentTransaction = new CompositeCommand<EditorBeatmap>();

        public event Action? OnStateChange;

        private bool isRestoring;

        public const int MAX_SAVED_STATES = 50;

        public void ApplyCommand(ICommand<EditorBeatmap> command)
        {
            command.Apply(editorBeatmap);
            currentTransaction?.AddCommand(command);

            CommandApplied?.Invoke(command);
        }

        public event Action<ICommand<EditorBeatmap>>? CommandApplied;

        protected override void UpdateState()
        {
            if (isRestoring)
                return;

            if (currentTransaction.Count == 0)
                return;

            if (undoStack.Count >= MAX_SAVED_STATES)
                undoStack.RemoveFirst();

            undoStack.AddLast(currentTransaction);
            redoStack.Clear();
            currentTransaction = new CompositeCommand<EditorBeatmap>();

            OnStateChange?.Invoke();
            updateBindables();
        }

        public void Undo()
        {
            var command = undoStack.Last?.Value;

            if (command is null)
                return;

            isRestoring = true;

            undoStack.RemoveLast();
            command.GetInverseCommand().Apply(editorBeatmap);
            redoStack.Push(command);

            isRestoring = false;

            OnStateChange?.Invoke();
            updateBindables();
        }

        public void Redo()
        {
            if (redoStack.Count == 0)
                return;

            isRestoring = true;

            var command = redoStack.Pop();
            command.Apply(editorBeatmap);
            undoStack.AddLast(command);

            isRestoring = false;

            OnStateChange?.Invoke();
            updateBindables();
        }

        private void updateBindables()
        {
            CanUndo.Value = undoStack.Count > 0;
            CanRedo.Value = redoStack.Count > 0;
        }
    }
}
