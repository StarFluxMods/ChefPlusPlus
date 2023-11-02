using System.Collections.Generic;
using ChefPlusPlus.Utils;
using Kitchen;
using Kitchen.Modules;
using TwitchLib.Api.Helix.Models.ChannelPoints;
using TwitchLib.Api.Helix.Models.ChannelPoints.GetCustomReward;
using UnityEngine;

namespace ChefPlusPlus.Menus.PlateUp
{
    public class TwitchSettings : TMenu<PauseMenuAction>
    {
        public TwitchSettings(Transform container, ModuleList module_list) : base(container, module_list)
        {
        }

        public override void Setup(int player_id)
        {
            Redraw(player_id);
        }

        public async void Redraw(int player_id)
        {
            ModuleList.Clear();

            AddLabel("Managed Rewards");

            GetCustomRewardsResponse managableRewards = await Mod.api.Helix.ChannelPoints.GetCustomReward(TwitchAuth.GetUser(), new List<string>(), true);

            foreach (CustomReward reward in managableRewards.Data)
                AddButton(reward.Title, delegate
                {
                    CustomRewardSettings.reward = reward;
                    RequestSubMenu(typeof(CustomRewardSettings));
                });

            New<SpacerElement>();

            AddButton("Manage New Reward", delegate { RequestSubMenu(typeof(CreateNewReward)); });

            New<SpacerElement>();
            New<SpacerElement>();

            AddButton("Back", delegate { RequestPreviousMenu(); });

            FixTheFuckingPanel();
        }
    }
}