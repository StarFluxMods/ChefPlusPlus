using System.Collections.Generic;
using TwitchLib.PubSub.Events;

namespace ChefPlusPlus.Utils
{
    public class ChannelPointUtils
    {
        private static readonly Dictionary<string, string> userInputs = new();
        public static Dictionary<(string, string), string> rewardIcons = new();

        public static void AddRewardIcon(string rewardId, string iconScale, string icon)
        {
            if (rewardIcons.ContainsKey((rewardId, iconScale)))
                rewardIcons[(rewardId, iconScale)] = icon;
            else
                rewardIcons.Add((rewardId, iconScale), icon);
        }

        public static string GetRewardIcon(string rewardId, string iconScale)
        {
            if (rewardIcons.ContainsKey((rewardId, iconScale)))
                return rewardIcons[(rewardId, iconScale)];
            return "";
        }

        public static void AddUserInput(string key, string value)
        {
            if (userInputs.ContainsKey(key))
                userInputs[key] = value;
            else
                userInputs.Add(key, value);
        }

        public static string GetUserInput(OnChannelPointsRewardRedeemedArgs args)
        {
            if (userInputs.ContainsKey(args.RewardRedeemed.Redemption.Id))
            {
                string result = userInputs[args.RewardRedeemed.Redemption.Id];
                return result;
            }

            return "";
        }
    }
}