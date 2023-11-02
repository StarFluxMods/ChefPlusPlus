using ChefPlusPlus.Utils;

namespace ChefPlusPlus.Interfaces
{
    public interface IEffect
    {
        string Type { get; }
        OnTriggerResult OnTrigger(ITrigger trigger, object args);
    }
}