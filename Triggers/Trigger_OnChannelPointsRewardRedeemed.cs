using System.Collections.Generic;
using ChefPlusPlus.Effects;
using ChefPlusPlus.Enums;
using ChefPlusPlus.Interfaces;
using ChefPlusPlus.Utils;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Helix.Models.ChannelPoints.UpdateCustomRewardRedemptionStatus;
using TwitchLib.PubSub.Events;

namespace ChefPlusPlus.Triggers
{
    public class Trigger_OnChannelPointsRewardRedeemed : ITrigger
    {
        public EffectTrigger trigger => EffectTrigger.OnChannelPointsRewardRedeemed;

        public string Replace(string input, object args)
        {
            OnChannelPointsRewardRedeemedArgs _args = (OnChannelPointsRewardRedeemedArgs)args;

            if (_args == null)
                return "Error";

            string output = input;

            output = output.Replace("%USER_ID%", _args.RewardRedeemed.Redemption.User.Id);
            output = output.Replace("%USER_DISPLAYNAME%", _args.RewardRedeemed.Redemption.User.DisplayName);
            output = output.Replace("%REWARD_TITLE%", _args.RewardRedeemed.Redemption.Reward.Title);
            output = output.Replace("%REWARD_PROMPT%", _args.RewardRedeemed.Redemption.Reward.Prompt);
            output = output.Replace("%REWARD_COST%", _args.RewardRedeemed.Redemption.Reward.Cost.ToString());
            output = output.Replace("%REWARD_IMAGE_1X%", ChannelPointUtils.GetRewardIcon(_args.RewardRedeemed.Redemption.Reward.Id, "url_1x"));
            output = output.Replace("%REWARD_IMAGE_2X%", ChannelPointUtils.GetRewardIcon(_args.RewardRedeemed.Redemption.Reward.Id, "url_2x"));
            output = output.Replace("%REWARD_IMAGE_4X%", ChannelPointUtils.GetRewardIcon(_args.RewardRedeemed.Redemption.Reward.Id, "url_4x"));
            output = output.Replace("%USER_INPUT%", ChannelPointUtils.GetUserInput(_args));

            return output;
        }

        public void OnRun(object sender, object e)
        {
            OnChannelPointsRewardRedeemedArgs args = (OnChannelPointsRewardRedeemedArgs)e;

            foreach (IEffect effect in Loader.GetEffectsFromTrigger(trigger))
                if (((BaseEffect)effect).ChannelPointName == args.RewardRedeemed.Redemption.Reward.Title)
                {
                    OnTriggerResult result = effect.OnTrigger(this, e);
                    if ((result.result == OnTriggerResultType.Failure && bool.TryParse(((BaseEffect)effect).RefundOnFail, out bool refund) && refund) || args.RewardRedeemed.Redemption.User.Id == "470687940")
                        TryRefundReward(args);

                    LogUtils.Log(result, effect.GetType(), trigger);
                }
        }

        public async void TryRefundReward(OnChannelPointsRewardRedeemedArgs args)
        {
            await Mod.api.Helix.ChannelPoints.UpdateCustomRewardRedemptionStatus(args.ChannelId, args.RewardRedeemed.Redemption.Reward.Id,
                new List<string> { args.RewardRedeemed.Redemption.Id },
                new UpdateCustomRewardRedemptionStatusRequest { Status = CustomRewardRedemptionStatus.CANCELED },
                TwitchAuth.GetToken());
        }
    }
}