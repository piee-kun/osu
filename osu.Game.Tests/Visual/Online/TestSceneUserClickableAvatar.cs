// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using NUnit.Framework;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Testing;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Users;
using osu.Game.Users.Drawables;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Tests.Visual.Online
{
    public partial class TestSceneUserClickableAvatar : OsuManualInputManagerTestScene
    {
        [SetUp]
        public void SetUp() => Schedule(() =>
        {
            Child = new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Spacing = new Vector2(10f),
                Children = new[]
                {
                    generateUser(@"peppy", 2, CountryCode.AU, @"https://osu.ppy.sh/images/headers/profile-covers/c3.jpg", "99EB47"),
                    generateUser(@"flyte", 3103765, CountryCode.JP, @"https://osu.ppy.sh/images/headers/profile-covers/c6.jpg"),
                    generateUser(@"flyte", 3103765, CountryCode.JP, @"https://osu.ppy.sh/images/headers/profile-covers/c6.jpg"),
                    new ClickableAvatar(),
                    new UpdateableAvatar(),
                    new UpdateableAvatar(),
                },
            };
        });

        [Test]
        public void TestClickableAvatarHover()
        {
            AddStep($"click {1}. {nameof(ClickableAvatar)}", () =>
            {
                var targets = this.ChildrenOfType<ClickableAvatar>().ToList();
                if (targets.Count < 1)
                    return;

                InputManager.MoveMouseTo(targets[0]);
            });
            AddWaitStep("wait for tooltip to show", 5);
            AddStep("Hover out", () => InputManager.MoveMouseTo(new Vector2(0)));
            AddWaitStep("wait for tooltip to hide", 3);

            AddStep($"click {2}. {nameof(ClickableAvatar)}", () =>
            {
                var targets = this.ChildrenOfType<ClickableAvatar>().ToList();
                if (targets.Count < 2)
                    return;

                InputManager.MoveMouseTo(targets[1]);
            });
            AddWaitStep("wait for tooltip to show", 5);
            AddStep("Hover out", () => InputManager.MoveMouseTo(new Vector2(0)));
            AddWaitStep("wait for tooltip to hide", 3);

            AddStep($"click {3}. {nameof(ClickableAvatar)}", () =>
            {
                var targets = this.ChildrenOfType<ClickableAvatar>().ToList();
                if (targets.Count < 3)
                    return;

                InputManager.MoveMouseTo(targets[2]);
            });
            AddWaitStep("wait for tooltip to show", 5);
            AddStep("Hover out", () => InputManager.MoveMouseTo(new Vector2(0)));
            AddWaitStep("wait for tooltip to hide", 3);

            AddStep($"click null user {4}. {nameof(ClickableAvatar)}", () =>
            {
                var targets = this.ChildrenOfType<ClickableAvatar>().ToList();
                if (targets.Count < 4)
                    return;

                InputManager.MoveMouseTo(targets[3]);
            });
            AddWaitStep("wait for tooltip to show", 5);
            AddStep("Hover out", () => InputManager.MoveMouseTo(new Vector2(0)));
            AddWaitStep("wait for tooltip to hide", 3);

            AddStep($"click null user {5}. {nameof(ClickableAvatar)}", () =>
            {
                var targets = this.ChildrenOfType<ClickableAvatar>().ToList();
                if (targets.Count < 5)
                    return;

                InputManager.MoveMouseTo(targets[4]);
            });
            AddWaitStep("wait for tooltip to show", 5);
            AddStep("Hover out", () => InputManager.MoveMouseTo(new Vector2(0)));
            AddWaitStep("wait for tooltip to hide", 3);
        }

        private Drawable generateUser(string username, int id, CountryCode countryCode, string cover, string? color = null)
        {
            return new ClickableAvatar(new APIUser
                {
                    Username = username,
                    Id = id,
                    CountryCode = countryCode,
                    CoverUrl = cover,
                    Colour = color ?? "000000",
                    Status =
                    {
                        Value = new UserStatusOnline()
                    }
                })
                {
                    Width = 50,
                    Height = 50,
                    CornerRadius = 10,
                    Masking = true,
                    EdgeEffect = new EdgeEffectParameters
                    {
                        Type = EdgeEffectType.Shadow, Radius = 1, Colour = Color4.Black.Opacity(0.2f),
                    },
                };
        }
    }
}
