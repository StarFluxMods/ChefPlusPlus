using ChefPlusPlus.Utils;
using HarmonyLib;
using Newtonsoft.Json.Linq;
using TwitchLib.Communication.Events;
using TwitchLib.PubSub;

namespace ChefPlusPlus.Patches
{
    [HarmonyPatch(typeof(TwitchPubSub), "OnMessage")]
    public class TwitchPubSub_Patch_UserInput
    {
        private static void Prefix(object sender, ref OnMessageEventArgs e)
        {
            JObject jObject = JObject.Parse(e.Message);
            if (jObject["type"].ToString() != "MESSAGE")
                return;

            if (jObject["data"]["topic"].ToString() != "channel-points-channel-v1." + TwitchAuth.GetUser())
                return;

            JObject message = JObject.Parse(jObject["data"]["message"].ToString());

            if (message["data"]["redemption"]["user_input"] != null) ChannelPointUtils.AddUserInput(message["data"]["redemption"]["id"].ToString(), message["data"]["redemption"]["user_input"].ToString());
        }
    }
}