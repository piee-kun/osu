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
        private EditorBeatmap beatmap = null!;
        private EditorCommandHandler handler = null!;

        [SetUp]
        public void SetUp()
        {
            stateChangedFired = 0;

            beatmap = new EditorBeatmap(new OsuBeatmap
            {
                BeatmapInfo =
                {
                    Ruleset = new OsuRuleset().RulesetInfo,
                },
            });
            beatmap.Add(new HitCircle { StartTime = 2760, Position = Vector2.Zero });

            handler = new EditorCommandHandler();
            handler.OnStateChange += () => stateChangedFired++;
        }

        [Test]
        public void TestSaveRestoreStateUsingTransaction()
        {
            Assert.That(handler.CanUndo.Value, Is.False);
            Assert.That(handler.CanRedo.Value, Is.False);
            assertOriginalPos();

            handler.BeginChange();

            Assert.That(stateChangedFired, Is.EqualTo(0));

            addArbitraryChange();
            handler.EndChange();

            Assert.That(stateChangedFired, Is.EqualTo(1));

            Assert.That(handler.CanUndo.Value, Is.True);
            Assert.That(handler.CanRedo.Value, Is.False);
            assertChangedPos();

            handler.Undo();

            Assert.That(handler.CanUndo.Value, Is.False);
            Assert.That(handler.CanRedo.Value, Is.True);
            assertOriginalPos();

            Assert.That(stateChangedFired, Is.EqualTo(2));
        }

        [Test]
        public void TestSaveRestoreState()
        {
            Assert.That(handler.CanUndo.Value, Is.False);
            Assert.That(handler.CanRedo.Value, Is.False);

            handler.SaveState();
            Assert.That(stateChangedFired, Is.EqualTo(0));

            addArbitraryChange();
            handler.SaveState();

            Assert.That(stateChangedFired, Is.EqualTo(1));

            Assert.That(handler.CanUndo.Value, Is.True);
            Assert.That(handler.CanRedo.Value, Is.False);

            handler.Undo();

            Assert.That(handler.CanUndo.Value, Is.False);
            Assert.That(handler.CanRedo.Value, Is.True);

            Assert.That(stateChangedFired, Is.EqualTo(2));
        }

        [Test]
        public void TestApplyThenUndoThenApplySameChange()
        {
            Assert.That(handler.CanUndo.Value, Is.False);
            Assert.That(handler.CanRedo.Value, Is.False);
            assertOriginalPos();

            handler.SaveState();
            Assert.That(stateChangedFired, Is.EqualTo(0));

            addArbitraryChange();
            handler.SaveState();

            Assert.That(handler.CanUndo.Value, Is.True);
            Assert.That(handler.CanRedo.Value, Is.False);
            Assert.That(stateChangedFired, Is.EqualTo(1));
            assertChangedPos();

            // undo a change without saving
            handler.Undo();

            Assert.That(stateChangedFired, Is.EqualTo(2));
            assertOriginalPos();

            addArbitraryChange();
            handler.SaveState();

            Assert.That(stateChangedFired, Is.EqualTo(3));
            assertChangedPos();
        }

        [Test]
        public void TestSaveSameStateDoesNotSave()
        {
            Assert.That(handler.CanUndo.Value, Is.False);
            Assert.That(handler.CanRedo.Value, Is.False);

            handler.SaveState();
            Assert.That(stateChangedFired, Is.EqualTo(0));

            addArbitraryChange();
            handler.SaveState();

            Assert.That(handler.CanUndo.Value, Is.True);
            Assert.That(handler.CanRedo.Value, Is.False);
            Assert.That(stateChangedFired, Is.EqualTo(1));

            // save a save without making any changes
            handler.SaveState();

            Assert.That(stateChangedFired, Is.EqualTo(1));

            handler.Undo();

            // we should only be able to restore once even though we saved twice.
            Assert.That(handler.CanUndo.Value, Is.False);
            Assert.That(handler.CanRedo.Value, Is.True);
            Assert.That(stateChangedFired, Is.EqualTo(2));
        }

        [Test]
        public void TestMaxStatesSaved()
        {
            handler.SaveState();
            Assert.That(stateChangedFired, Is.EqualTo(0));
            assertOriginalPos();

            Assert.That(handler.CanUndo.Value, Is.False);

            for (int i = 0; i < EditorChangeHandler.MAX_SAVED_STATES; i++)
            {
                Assert.That(stateChangedFired, Is.EqualTo(i));

                addArbitraryChange();
                handler.SaveState();
            }

            Assert.That(handler.CanUndo.Value, Is.True);
            assertChangedPos();

            for (int i = 0; i < EditorChangeHandler.MAX_SAVED_STATES; i++)
            {
                Assert.That(handler.CanUndo.Value, Is.True);
                handler.Undo();
            }

            Assert.That(handler.CanUndo.Value, Is.False);
            assertOriginalPos();
        }

        [Test]
        public void TestMaxStatesExceeded()
        {
            Assert.That(handler.CanUndo.Value, Is.False);
            assertOriginalPos();

            for (int i = 0; i < EditorChangeHandler.MAX_SAVED_STATES * 2; i++)
            {
                addArbitraryChange();
                handler.SaveState();
            }

            Assert.That(handler.CanUndo.Value, Is.True);
            assertChangedPos();

            for (int i = 0; i < EditorChangeHandler.MAX_SAVED_STATES; i++)
            {
                Assert.That(handler.CanUndo.Value, Is.True);
                handler.Undo();
            }

            Assert.That(handler.CanUndo.Value, Is.False);
            assertChangedPos();
        }

        private void addArbitraryChange()
        {
            handler.ApplyCommand(new MoveCommand(new[] { (OsuHitObject)beatmap.HitObjects[0] }, new[] { new Vector2(100, 100) }));
        }

        private void assertOriginalPos()
        {
            Assert.That(((OsuHitObject)beatmap.HitObjects[0]).Position, Is.EqualTo(Vector2.Zero));
        }

        private void assertChangedPos()
        {
            Assert.That(((OsuHitObject)beatmap.HitObjects[0]).Position, Is.EqualTo(new Vector2(100, 100)));
        }
    }
}
