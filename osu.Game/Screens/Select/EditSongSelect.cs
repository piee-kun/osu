// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osu.Game.Beatmaps;
using osu.Game.Graphics.UserInterface;
using osu.Game.Resources.Localisation.Web;
using osu.Game.Rulesets.Mods;
using osu.Game.Screens.Play;
using osu.Game.Users;
using osuTK.Input;

namespace osu.Game.Screens.Select
{
    public class EditSongSelect : SongSelect
    {
        public override bool AllowExternalScreenChange => true;

        public override Func<BeatmapInfo, MenuItem>[] CustomMenuItems =>
            new Func<BeatmapInfo, MenuItem>[]
            {
                b => new OsuMenuItem(CommonStrings.ButtonsEdit, MenuItemType.Highlighted, () => Edit(b)),
                b => new OsuMenuItem("Test", MenuItemType.Standard, () => TestPlay())
            };

        protected override UserActivity InitialActivity => new UserActivity.ChoosingBeatmap();

        [BackgroundDependencyLoader]
        private void load()
        {
            //BeatmapOptions.AddButton(@"Edit", @"beatmap", FontAwesome.Solid.PencilAlt, colours.Yellow, () => Edit());
        }

        protected override BeatmapDetailArea CreateBeatmapDetailArea()
        {
            return new EditBeatmapDetailArea();
        }

        protected override bool OnKeyDown(KeyDownEvent e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                case Key.KeypadEnter:
                    // this is a special hard-coded case; we can't rely on OnPressed (of SongSelect) as GlobalActionContainer is
                    // matching with exact modifier consideration (so Ctrl+Enter would be ignored).
                    FinaliseSelection();
                    return true;
            }

            return base.OnKeyDown(e);
        }

        protected override bool OnStart()
        {
            Edit();
            return true;
        }

        protected bool TestPlay()
        {
            SampleConfirm?.Play();

            this.Push(new PlayerLoader(createPlayer));
            return true;

            Player createPlayer()
            {
                Player player;

                var replayGeneratingMod = Mods.Value.OfType<ICreateReplayData>().FirstOrDefault();

                if (replayGeneratingMod != null)
                {
                    player = new ReplayPlayer((beatmap, mods) => replayGeneratingMod.CreateScoreFromReplayData(beatmap, mods));
                }
                else
                {
                    player = new SoloPlayer();
                }

                return player;
            }
        }
    }
}
