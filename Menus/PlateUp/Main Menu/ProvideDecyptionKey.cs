using ChefPlusPlus.Utils;
using Kitchen;
using Kitchen.Modules;
using KitchenLib;
using Mono.WebBrowser;
using UnityEngine;

namespace ChefPlusPlus.Menus.PlateUp
{
    public class ProvideDecyptionKey : KLMenu<MainMenuAction>
    {
        public ProvideDecyptionKey(Transform container, ModuleList module_list) : base(container, module_list)
        {
        }

        public override void Setup(int player_id)
        {
            TextInputView.RequestTextInput("Set Decyption Key", "", 40, AssignDecyptionKey);
        }

        private void AssignDecyptionKey(TextInputView.TextInputState result, string key)
        {
            try
            {
                TwitchAuth.DecryptionKey = key;

                RequestPreviousMenu();
            }
            catch (Exception e)
            {
                Mod.LogInfo(e);
            }
        }
    }
}