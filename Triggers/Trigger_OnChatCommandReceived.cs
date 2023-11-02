using ChefPlusPlus.Enums;
using ChefPlusPlus.Interfaces;
using ChefPlusPlus.Utils;
using TwitchLib.Client.Events;

namespace ChefPlusPlus.Triggers
{
    public class Trigger_OnChatCommandReceived : ITrigger
    {
        public EffectTrigger trigger => EffectTrigger.OnChatCommandReceived;

        public string Replace(string input, object args)
        {
            return input;
        }

        public void OnRun(object sender, object e)
        {
            OnChatCommandReceivedArgs args = (OnChatCommandReceivedArgs)e;

            foreach (IEffect effect in Loader.GetEffectsFromTrigger(trigger))
            {
                OnTriggerResult result = effect.OnTrigger(this, e);
                LogUtils.Log(result, effect.GetType(), trigger);
            }
        }
    }
}