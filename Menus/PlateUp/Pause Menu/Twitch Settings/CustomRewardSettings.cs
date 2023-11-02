using System.Collections.Generic;
using ChefPlusPlus.Utils;
using Kitchen;
using Kitchen.Modules;
using TwitchLib.Api.Helix.Models.ChannelPoints;
using TwitchLib.Api.Helix.Models.ChannelPoints.UpdateCustomReward;
using UnityEngine;

namespace ChefPlusPlus.Menus.PlateUp
{
    public class CustomRewardSettings : TMenu<PauseMenuAction>
    {
        public static CustomReward reward;
        private bool enabled = false;

        private Option<bool> isRewardActive;

        public CustomRewardSettings(Transform container, ModuleList module_list) : base(container, module_list)
        {
        }


        public override void Setup(int player_id)
        {
            isRewardActive = new Option<bool>(new List<bool> { true, false }, reward.IsEnabled, new List<string> { "Enabled", "Disabled" });

            isRewardActive.OnChanged += (sender, b) => { Mod.api.Helix.ChannelPoints.UpdateCustomReward(TwitchAuth.GetUser(), reward.Id, new UpdateCustomRewardRequest { IsEnabled = b }, TwitchAuth.GetToken()); };

            Redraw(player_id);
        }

        public async void Redraw(int player_id)
        {
            AddLabel("Managing : " + reward.Title);

            New<SpacerElement>();

            AddLabel("Enabled");
            AddSelect(isRewardActive);

            New<SpacerElement>();
            AddButton("Delete Reward", async delegate
            {
                await Mod.api.Helix.ChannelPoints.DeleteCustomReward(TwitchAuth.GetUser(), reward.Id, TwitchAuth.GetToken());
                RequestPreviousMenu();
            });

            New<SpacerElement>();

            AddButton("Back", delegate { RequestPreviousMenu(); });
        }
    }
}