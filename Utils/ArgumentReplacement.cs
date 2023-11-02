using TwitchLib.PubSub.Events;

namespace ChefPlusPlus.Utils
{
    public class ArgumentReplacement
    {
        public static string Replace(string input, OnChannelPointsRewardRedeemedArgs args)
        {
            input.Replace("%USER_ID%", args.RewardRedeemed.Redemption.User.Id);
            input.Replace("%USER_DISPLAYNAME%", args.RewardRedeemed.Redemption.User.DisplayName);

            input.Replace("%REWARD_TITLE%", args.RewardRedeemed.Redemption.Reward.Title);
            input.Replace("%REWARD_PROMPT%", args.RewardRedeemed.Redemption.Reward.Prompt);
            //input.Replace("%REWARD_COST%", args.RewardRedeemed.Redemption.Reward.Cost);


            input.Replace("%REWARD_TITLE%", args.RewardRedeemed.Redemption.Reward.Title);

            return input;
        }
    }
}