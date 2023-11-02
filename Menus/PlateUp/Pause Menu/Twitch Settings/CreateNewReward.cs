using ChefPlusPlus.Utils;
using Kitchen;
using Kitchen.Modules;
using KitchenLib;
using Mono.WebBrowser;
using TwitchLib.Api.Helix.Models.ChannelPoints.CreateCustomReward;
using UnityEngine;

namespace ChefPlusPlus.Menus.PlateUp
{
    public class CreateNewReward : KLMenu<PauseMenuAction>
    {
        public CreateNewReward(Transform container, ModuleList module_list) : base(container, module_list)
        {
        }

        public override void Setup(int player_id)
        {
            TextInputView.RequestTextInput("Point Reward Name", "", 40, AssignToken);
        }

        private async void AssignToken(TextInputView.TextInputState result, string name)
        {
            try
            {
                if (!string.IsNullOrEmpty(name))
                    await Mod.api.Helix.ChannelPoints.CreateCustomRewards(TwitchAuth.GetUser(), new CreateCustomRewardsRequest
                    {
                        Cost = 1,
                        Title = name,
                        Prompt = "Channel Redemtion Generated for Chef++"
                    }, TwitchAuth.GetToken());
                RequestPreviousMenu();
            }
            catch (Exception e)
            {
                Mod.LogInfo(e);
            }
        }
    }
}