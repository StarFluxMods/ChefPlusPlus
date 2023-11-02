using ChefPlusPlus.Utils;
using Kitchen;
using Kitchen.Modules;
using KitchenLib;
using Mono.WebBrowser;
using TwitchLib.Unity;
using UnityEngine;

namespace ChefPlusPlus.Menus.PlateUp
{
    public class ProvideToken : KLMenu<MainMenuAction>
    {
        public ProvideToken(Transform container, ModuleList module_list) : base(container, module_list)
        {
        }

        public override void Setup(int player_id)
        {
            TextInputView.RequestTextInput("Provide your Token", "", 40, AssignToken);
        }

        private void AssignToken(TextInputView.TextInputState result, string token)
        {
            try
            {
                if (!string.IsNullOrEmpty(token))
                {
                    Api _api = new();
                    _api.Settings.ClientId = TwitchAuth.ClientID;
                    _api.Settings.AccessToken = token;
                    string id = _api.Helix.Users.GetUsersAsync(accessToken: token).Result.Users[0].Id;

                    TwitchAuth.SetToken(token);
                    TwitchAuth.SetUser(id);

                    _api = null;
                    TwitchMenu.server = null;
                }

                TwitchMenu.server = null;
                RequestPreviousMenu();
            }
            catch (Exception e)
            {
                Mod.LogInfo(e);
            }
        }
    }
}