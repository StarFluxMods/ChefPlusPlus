using ChefPlusPlus.Enums;

namespace ChefPlusPlus.Utils
{
    public class OnTriggerResult
    {
        public string message;
        public OnTriggerResultType result;

        public OnTriggerResult(OnTriggerResultType result, string message = "")
        {
            this.result = result;
            this.message = message;
        }
    }
}