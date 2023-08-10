// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using NUnit.Framework;
using osu.Game.Rulesets.Osu;
using osu.Game.Rulesets.Osu.Beatmaps;
using osu.Game.Rulesets.Osu.Edit.Commands;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Screens.Edit;
using osuTK;

namespace osu.Game.Tests.Editing
{
    [TestFixture]
    public class EditorCommandHandlerTest
    {
        private int stateChangedFired;
        private int hitObjectUpdatedFired;
        private EditorBeatmap editorBeatmap = null!;
        private EditorCommandHandler handler = null!;

        [SetUp]
        public void SetUp()
        {
            stateChangedFired = 0;
            hitObjectUpdatedFired = 0;

            editorBeatmap = new EditorBeatmap(new OsuBeatmap
            {
                BeatmapInfo =
                {
                    Ruleset = new OsuRuleset().RulesetInfo,
                },
            });
            editorBeatmap.Add(new HitCircle { StartTime = 2760, Position = Vector2.Zero });

            handler = new BeatmapEditorCommandHandler(editorBeatmap);
            handler.OnStateChange += () => stateChangedFired++;
            editorBeatmap.HitObjectUpdated += _ => hitObjectUpdatedFired++;
        }

        [Test]
        public void TestUndoUsingTransaction()
        {
            Assert.That(handler.CanUndo.Value, Is.False);
            Assert.That(handler.CanRedo.Value, Is.False);
            assert0Pos();

            handler.BeginChange();

            Assert.That(handler.CurrentStateHash, Is.EqualTo("0:0"));
            Assert.That(stateChangedFired, Is.EqualTo(0));

            move100();
            handler.EndChange();

            Assert.That(handler.CurrentStateHash, Is.EqualTo("1:0"));
            Assert.That(stateChangedFired, Is.EqualTo(1));
            Assert.That(handler.CanUndo.Value, Is.True);
            Assert.That(handler.CanRedo.Value, Is.False);
            assert100Pos();

            handler.Undo();

            Assert.That(handler.CanUndo.Value, Is.False);
            Assert.That(handler.CanRedo.Value, Is.True);
            assert0Pos();

            Assert.That(handler.CurrentStateHash, Is.EqualTo("0:0"));
            Assert.That(stateChangedFired, Is.EqualTo(2));
        }

        [Test]
        public void TestUndoRedoMergedCommandUsingTransaction()
        {
            Assert.That(handler.CanUndo.Value, Is.False);
            Assert.That(handler.CanRedo.Value, Is.False);
            assert0Pos();

            handler.BeginChange();

            Assert.That(stateChangedFired, Is.EqualTo(0));

            move100();
            move200();
            handler.EndChange();

            Assert.That(stateChangedFired, Is.EqualTo(1));
            Assert.That(handler.CurrentStateHash, Is.EqualTo("1:0"));

            Assert.That(handler.CanUndo.Value, Is.True);
            Assert.That(handler.CanRedo.Value, Is.False);
            assert200Pos();

            handler.Undo();

            Assert.That(handler.CanUndo.Value, Is.False);
            Assert.That(handler.CanRedo.Value, Is.True);
            Assert.That(stateChangedFired, Is.EqualTo(2));
            Assert.That(handler.CurrentStateHash, Is.EqualTo("0:0"));
            assert0Pos();

            handler.Redo();

            Assert.That(handler.CanUndo.Value, Is.True);
            Assert.That(handler.CanRedo.Value, Is.False);
            Assert.That(stateChangedFired, Is.EqualTo(3));
            Assert.That(handler.CurrentStateHash, Is.EqualTo("1:0"));
            assert200Pos();
            Assert.That(hitObjectUpdatedFired, Is.EqualTo(0));
        }

        [Test]
        public void TestSaveState()
        {
            Assert.That(handler.CanUndo.Value, Is.False);
            Assert.That(handler.CanRedo.Value, Is.False);

            handler.SaveState();
            Assert.That(stateChangedFired, Is.EqualTo(0));

            move100();
            handler.SaveState();

            Assert.That(stateChangedFired, Is.EqualTo(1));

            Assert.That(handler.CanUndo.Value, Is.True);
            Assert.That(handler.CanRedo.Value, Is.False);

            handler.Undo();

            Assert.That(handler.CanUndo.Value, Is.False);
            Assert.That(handler.CanRedo.Value, Is.True);

            Assert.That(stateChangedFired, Is.EqualTo(2));
            Assert.That(hitObjectUpdatedFired, Is.EqualTo(0));

            editorBeatmap.SaveState();

            Assert.That(hitObjectUpdatedFired, Is.EqualTo(1));
        }

        [Test]
        public void TestApplyThenUndoThenApplySameChange()
        {
            Assert.That(handler.CanUndo.Value, Is.False);
            Assert.That(handler.CanRedo.Value, Is.False);
            assert0Pos();

            handler.SaveState();
            Assert.That(stateChangedFired, Is.EqualTo(0));

            move100();
            handler.SaveState();

            Assert.That(handler.CanUndo.Value, Is.True);
            Assert.That(handler.CanRedo.Value, Is.False);
            Assert.That(stateChangedFired, Is.EqualTo(1));
            assert100Pos();

            // undo a change without saving
            handler.Undo();

            Assert.That(stateChangedFired, Is.EqualTo(2));
            assert0Pos();

            move100();
            handler.SaveState();

            Assert.That(stateChangedFired, Is.EqualTo(3));
            assert100Pos();
        }

        [Test]
        public void TestSaveSameStateDoesNotSave()
        {
            Assert.That(handler.CanUndo.Value, Is.False);
            Assert.That(handler.CanRedo.Value, Is.False);
            Assert.That(handler.CurrentStateHash, Is.EqualTo("0:0"));

            handler.SaveState();
            Assert.That(stateChangedFired, Is.EqualTo(0));
            Assert.That(handler.CurrentStateHash, Is.EqualTo("0:1"));

            move100();
            handler.SaveState();

            Assert.That(handler.CanUndo.Value, Is.True);
            Assert.That(handler.CanRedo.Value, Is.False);
            Assert.That(stateChangedFired, Is.EqualTo(1));
            Assert.That(handler.CurrentStateHash, Is.EqualTo("1:1"));

            // save a save without making any changes
            handler.SaveState();

            Assert.That(stateChangedFired, Is.EqualTo(1));
            Assert.That(handler.CurrentStateHash, Is.EqualTo("1:2"));

            handler.Undo();

            // we should only be able to restore once even though we saved twice.
            Assert.That(handler.CanUndo.Value, Is.False);
            Assert.That(handler.CanRedo.Value, Is.True);
            Assert.That(stateChangedFired, Is.EqualTo(2));
            Assert.That(handler.CurrentStateHash, Is.EqualTo("0:2"));
        }

        [Test]
        public void TestMaxUndo()
        {
            handler.SaveState();
            Assert.That(stateChangedFired, Is.EqualTo(0));
            assert0Pos();

            Assert.That(handler.CanUndo.Value, Is.False);

            for (int i = 0; i < EditorChangeHandler.MAX_SAVED_STATES; i++)
            {
                Assert.That(stateChangedFired, Is.EqualTo(i));

                move100();
                handler.SaveState();
            }

            Assert.That(handler.CanUndo.Value, Is.True);
            assert100Pos();

            for (int i = 0; i < EditorChangeHandler.MAX_SAVED_STATES; i++)
            {
                Assert.That(handler.CanUndo.Value, Is.True);
                handler.Undo();
            }

            Assert.That(handler.CanUndo.Value, Is.False);
            assert0Pos();
        }

        [Test]
        public void TestMaxUndoExceeded()
        {
            Assert.That(handler.CanUndo.Value, Is.False);
            assert0Pos();

            for (int i = 0; i < EditorChangeHandler.MAX_SAVED_STATES * 2; i++)
            {
                move100();
                handler.SaveState();
            }

            Assert.That(handler.CanUndo.Value, Is.True);
            assert100Pos();

            for (int i = 0; i < EditorChangeHandler.MAX_SAVED_STATES; i++)
            {
                Assert.That(handler.CanUndo.Value, Is.True);
                handler.Undo();
            }

            Assert.That(handler.CanUndo.Value, Is.False);
            assert100Pos();
        }

        [Test]
        public void TestUndoEditorBeatmapTransaction()
        {
            Assert.That(handler.CanUndo.Value, Is.False);
            Assert.That(handler.CanRedo.Value, Is.False);
            assert0Pos();

            editorBeatmap.BeginChange();

            Assert.That(stateChangedFired, Is.EqualTo(0));

            move100();

            Assert.That(hitObjectUpdatedFired, Is.EqualTo(0));

            editorBeatmap.EndChange();

            Assert.That(stateChangedFired, Is.EqualTo(1));
            Assert.That(hitObjectUpdatedFired, Is.EqualTo(1));

            Assert.That(handler.CanUndo.Value, Is.True);
            Assert.That(handler.CanRedo.Value, Is.False);
            assert100Pos();

            handler.Undo();

            Assert.That(handler.CanUndo.Value, Is.False);
            Assert.That(handler.CanRedo.Value, Is.True);
            assert0Pos();

            Assert.That(stateChangedFired, Is.EqualTo(2));
        }

        private void move100()
        {
            editorBeatmap.ApplyCommand(new MoveCommand(new[] { (OsuHitObject)editorBeatmap.HitObjects[0] }, new[] { new Vector2(100, 100) }));
        }

        private void move200()
        {
            editorBeatmap.ApplyCommand(new MoveCommand(new[] { (OsuHitObject)editorBeatmap.HitObjects[0] }, new[] { new Vector2(200, 200) }));
        }

        private void assert0Pos()
        {
            Assert.That(((OsuHitObject)editorBeatmap.HitObjects[0]).Position, Is.EqualTo(Vector2.Zero));
        }

        private void assert100Pos()
        {
            Assert.That(((OsuHitObject)editorBeatmap.HitObjects[0]).Position, Is.EqualTo(new Vector2(100, 100)));
        }

        private void assert200Pos()
        {
            Assert.That(((OsuHitObject)editorBeatmap.HitObjects[0]).Position, Is.EqualTo(new Vector2(200, 200)));
        }
    }
}
