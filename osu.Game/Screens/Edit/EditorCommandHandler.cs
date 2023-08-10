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

        private readonly LinkedList<ICommand> undoStack = new LinkedList<ICommand>();
        private readonly Stack<ICommand> redoStack = new Stack<ICommand>();

        private CompositeCommand currentTransaction = new CompositeCommand();

        public event Action? OnStateChange;
        public event Action? CommandLogged;

        public const int MAX_UNDO_LENGTH = 50;

        public void ApplyCommand(ICommand command)
        {
            command.Apply();
            LogCommand(command);
        }

        public void LogCommand(ICommand command)
        {
            currentTransaction.AddCommand(command);
            CommandLogged?.Invoke();
        }

        protected override void UpdateState()
        {
            if (currentTransaction.Count == 0)
                return;

            if (undoStack.Count >= MAX_UNDO_LENGTH)
                undoStack.RemoveFirst();

            undoStack.AddLast(currentTransaction);
            redoStack.Clear();
            currentTransaction = new CompositeCommand();

            OnStateChange?.Invoke();
            updateBindables();
        }

        public void Undo()
        {
            var command = undoStack.Last?.Value;

            if (command is null)
                return;

            undoStack.RemoveLast();
            command.GetInverseCommand().Apply();
            redoStack.Push(command);

            OnStateChange?.Invoke();
            updateBindables();
        }

        public void Redo()
        {
            if (redoStack.Count == 0)
                return;

            var command = redoStack.Pop();
            command.Apply();
            undoStack.AddLast(command);

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
