using ChefPlusPlus.Enums;
using ChefPlusPlus.Interfaces;
using ChefPlusPlus.Utils;
using TwitchLib.Client.Events;

namespace ChefPlusPlus.Triggers
{
    public class Trigger_OnRaidNotification : ITrigger
    {
        public EffectTrigger trigger => EffectTrigger.OnRaidNotification;

        public string Replace(string input, object args)
        {
            OnRaidNotificationArgs _args = (OnRaidNotificationArgs)args;

            if (_args == null)
                return input;

            string output = input;

            output = output.Replace("%RAID_VIEWER_COUNT%", _args.RaidNotification.MsgParamViewerCount);
            output = output.Replace("%RAIDER_DISPLAY_NAME%", _args.RaidNotification.DisplayName);
            output = output.Replace("%IS_RAIDER_MODERATOR%", _args.RaidNotification.Moderator.ToString());
            output = output.Replace("%IS_RAIDER_SUBSCRIBER%", _args.RaidNotification.Subscriber.ToString());
            output = output.Replace("%IS_RAIDER_TURBO%", _args.RaidNotification.Turbo.ToString());

            return output;
        }

        public void OnRun(object sender, object e)
        {
            OnRaidNotificationArgs args = (OnRaidNotificationArgs)e;

            foreach (IEffect effect in Loader.GetEffectsFromTrigger(trigger))
            {
                OnTriggerResult result = effect.OnTrigger(this, e);
                LogUtils.Log(result, effect.GetType(), trigger);
            }
        }
    }
}