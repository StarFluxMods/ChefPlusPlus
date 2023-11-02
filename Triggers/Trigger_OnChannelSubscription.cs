using ChefPlusPlus.Enums;
using ChefPlusPlus.Interfaces;
using ChefPlusPlus.Utils;
using TwitchLib.PubSub.Events;

namespace ChefPlusPlus.Triggers
{
    public class Trigger_OnChannelSubscription : ITrigger
    {
        public EffectTrigger trigger => EffectTrigger.OnChannelSubscription;

        public string Replace(string input, object args)
        {
            OnChannelSubscriptionArgs _args = (OnChannelSubscriptionArgs)args;

            if (_args == null)
                return input;

            string output = input;

            output = output.Replace("%PURCHASER_DISPLAYNAME%", _args.Subscription.DisplayName);
            output = output.Replace("%RECIEVER_DISPLAYNAME%", _args.Subscription.RecipientDisplayName);
            output = output.Replace("%SUBSCRIPTION_TYPE%", _args.Subscription.SubscriptionPlan.ToString());
            output = output.Replace("%MONTHS%", _args.Subscription.Months.ToString());
            output = output.Replace("%TOTAL_MONTHS%", _args.Subscription.CumulativeMonths.ToString());
            output = output.Replace("%STREAK_MONTHS%", _args.Subscription.StreakMonths.ToString());
            output = output.Replace("%MESSAGE%", _args.Subscription.SubMessage.Message);

            return output;
        }

        public void OnRun(object sender, object e)
        {
            OnChannelSubscriptionArgs args = (OnChannelSubscriptionArgs)e;

            foreach (IEffect effect in Loader.GetEffectsFromTrigger(trigger))
            {
                OnTriggerResult result = effect.OnTrigger(this, e);
                LogUtils.Log(result, effect.GetType(), trigger);
            }
        }
    }
}