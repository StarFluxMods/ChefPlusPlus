using System.Collections.Generic;
using System.Linq;
using ChefPlusPlus.Effects;
using ChefPlusPlus.Enums;
using ChefPlusPlus.Interfaces;
using ChefPlusPlus.Utils;
using TwitchLib.PubSub.Events;

namespace ChefPlusPlus.Triggers
{
    public class Trigger_OnBitsReceivedV2 : ITrigger
    {
        public EffectTrigger trigger => EffectTrigger.OnBitsReceivedV2;

        public string Replace(string input, object args)
        {
            OnBitsReceivedV2Args _args = (OnBitsReceivedV2Args)args;

            if (_args == null)
                return input;

            string output = input;

            List<string> messageparts = _args.ChatMessage.Split(' ').ToList();
            for (int i = 0; i < messageparts.Count; i++)
                if (messageparts[i].ToLower().Contains("cheer"))
                    messageparts.RemoveAt(i);
            string message = string.Join(" ", messageparts);
            messageparts.Clear();

            output = output.Replace("%USER_NAME%", _args.UserName);
            output = output.Replace("%ANONYMOUS%", _args.IsAnonymous.ToString());
            output = output.Replace("%BITS%", _args.BitsUsed.ToString());
            output = output.Replace("%TOTAL_BITS%", _args.TotalBitsUsed.ToString());
            output = output.Replace("%CHAT_MESSAGE%", _args.ChatMessage);
            output = output.Replace("%CHAT_MESSAGE_CLEANED%", message);

            return output;
        }

        public void OnRun(object sender, object e)
        {
            OnBitsReceivedV2Args args = (OnBitsReceivedV2Args)e;

            foreach (IEffect effect in Loader.GetEffectsFromTrigger(trigger))
            {
                if (!int.TryParse(((BaseEffect)effect).MinimumBits, out int minbits))
                {
                    LogUtils.Log(new OnTriggerResult(OnTriggerResultType.Failure, "MinimumBits is not a valid int"), effect.GetType(), trigger);
                    continue;
                }

                if (!int.TryParse(((BaseEffect)effect).MaximumBits, out int maxbits))
                {
                    LogUtils.Log(new OnTriggerResult(OnTriggerResultType.Failure, "MaximumBits is not a valid int"), effect.GetType(), trigger);
                    continue;
                }

                if ((args.BitsUsed >= minbits && args.BitsUsed <= maxbits) || (minbits == -1 && maxbits == -1))
                {
                    OnTriggerResult result = effect.OnTrigger(this, e);
                    LogUtils.Log(result, effect.GetType(), trigger);
                }
            }
        }
    }
}