using System.Linq;
using ChefPlusPlus.Utils;
using HarmonyLib;
using Newtonsoft.Json.Linq;
using TwitchLib.Communication.Events;
using TwitchLib.PubSub;

namespace ChefPlusPlus.Patches
{
    [HarmonyPatch(typeof(TwitchPubSub), "OnMessage")]
    public class TwitchPubSub_Patch_Images
    {
        private static void Prefix(object sender, ref OnMessageEventArgs e)
        {
            JObject jObject = JObject.Parse(e.Message);
            if (jObject["type"].ToString() != "MESSAGE")
                return;

            if (jObject["data"]["topic"].ToString() != "channel-points-channel-v1." + TwitchAuth.GetUser())
                return;

            JObject message = JObject.Parse(jObject["data"]["message"].ToString());

            if (message["data"]["redemption"]["reward"]["image"].ToArray().Length > 0)
            {
                ChannelPointUtils.AddRewardIcon(message["data"]["redemption"]["reward"]["id"].ToString(), "url_1x", message["data"]["redemption"]["reward"]["image"]["url_1x"].ToString());
                ChannelPointUtils.AddRewardIcon(message["data"]["redemption"]["reward"]["id"].ToString(), "url_2x", message["data"]["redemption"]["reward"]["image"]["url_2x"].ToString());
                ChannelPointUtils.AddRewardIcon(message["data"]["redemption"]["reward"]["id"].ToString(), "url_4x", message["data"]["redemption"]["reward"]["image"]["url_4x"].ToString());
            }

            message["data"]["redemption"]["reward"]["image"] = "";

            jObject["data"]["message"] = message;

            e = new OnMessageEventArgs
            {
                Message = jObject.ToString()
            };
        }
    }
}