using System;
using ChefPlusPlus.Enums;

namespace ChefPlusPlus.Utils
{
    public class LogUtils
    {
        public static void Log(OnTriggerResult onTriggerResult, Type effect, EffectTrigger trigger)
        {
            if (onTriggerResult.result == OnTriggerResultType.Info)
                Mod.LogInfo($"Effect {effect}, {trigger} returned a message: {onTriggerResult.message}");
            else if (onTriggerResult.result == OnTriggerResultType.Failure) Mod.LogWarning($"Effect {effect}, {trigger} failed to run: {onTriggerResult.message}");
        }
    }
}