using ChefPlusPlus.Enums;
using ChefPlusPlus.Interfaces;
using ChefPlusPlus.Utils;
using TwitchLib.PubSub.Events;

namespace ChefPlusPlus.Triggers
{
    public class Trigger_OnFollow : ITrigger
    {
        public EffectTrigger trigger => EffectTrigger.OnFollow;

        public string Replace(string input, object args)
        {
            OnFollowArgs _args = (OnFollowArgs)args;

            if (_args == null)
                return input;

            string output = input;

            output = output.Replace("%USER_DISPLAYNAME%", _args.DisplayName);

            return output;
        }

        public void OnRun(object sender, object e)
        {
            OnFollowArgs args = (OnFollowArgs)e;

            foreach (IEffect effect in Loader.GetEffectsFromTrigger(trigger))
            {
                OnTriggerResult result = effect.OnTrigger(this, e);
                LogUtils.Log(result, effect.GetType(), trigger);
            }
        }
    }
}