using ChefPlusPlus.Enums;

namespace ChefPlusPlus.Interfaces
{
    public interface ITrigger
    {
        EffectTrigger trigger { get; }
        void OnRun(object sender, object e);
        string Replace(string input, object args);
    }
}