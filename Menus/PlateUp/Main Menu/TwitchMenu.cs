using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using ChefPlusPlus.Auth;
using ChefPlusPlus.Enums;
using ChefPlusPlus.Utils;
using Kitchen;
using Kitchen.Modules;
using KitchenLib;
using KitchenLib.Preferences;
using UnityEngine;

namespace ChefPlusPlus.Menus.PlateUp
{
    public class TwitchMenu : KLMenu<MainMenuAction>
    {
        public static WebServer server;

        private readonly Option<int> token_storage_method = new(new List<int> { (int)TokenStorageOption.Automatic, (int)TokenStorageOption.Manual, (int)TokenStorageOption.None }, Mod.manager.GetPreference<PreferenceInt>("TOKEN_STORAGE_METHOD").Value, new List<string> { "Automatic", "Manual", "None" });

        public TwitchMenu(Transform container, ModuleList module_list) : base(container, module_list)
        {
        }

        public override void Setup(int player_id)
        {
            token_storage_method.OnChanged += delegate(object _, int result)
            {
                Mod.manager.GetPreference<PreferenceInt>("TOKEN_STORAGE_METHOD").Set(result);
                Mod.manager.Save();
                Redraw(player_id);
            };
            Redraw(player_id);
        }

        private void Redraw(int player_id = 0)
        {
            ModuleList.Clear();

            int storageSetting = Mod.manager.GetPreference<PreferenceInt>("TOKEN_STORAGE_METHOD").Value;

            if (storageSetting == 0) // Automatic
            {
                if (!string.IsNullOrEmpty(Mod.tokenManager.GetPreference<PreferenceString>("USER_ID").Value) && !string.IsNullOrEmpty(Mod.tokenManager.GetPreference<PreferenceString>("ACCESS_TOKEN").Value))
                {
                    if (TwitchAuth.IsTokenValid(TwitchAuth.GetToken()))
                        AddLabel("You are authenticated as " + TwitchAuth.GetTokenOwner(TwitchAuth.GetToken()));
                    else
                        AddLabel("Your token is invalid, please generate a new one.");
                    New<SpacerElement>();
                }
            }
            else if (storageSetting == 1) // Manual
            {
                if (!string.IsNullOrEmpty(Mod.tokenManager.GetPreference<PreferenceString>("USER_ID").Value) && !string.IsNullOrEmpty(Mod.tokenManager.GetPreference<PreferenceString>("ACCESS_TOKEN").Value))
                {
                    if (!string.IsNullOrEmpty(TwitchAuth.DecryptionKey))
                    {
                        if (TwitchAuth.GetToken() == "decryption_error")
                        {
                            AddLabel("Decryption key is invalid, please try a different one.");
                        }
                        else
                        {
                            if (TwitchAuth.IsTokenValid(TwitchAuth.GetToken()))
                                AddLabel("You are authenticated as " + TwitchAuth.GetTokenOwner(TwitchAuth.GetToken()));
                            else
                                AddLabel("Your token is invalid, please generate a new one.");
                        }
                    }
                    else
                    {
                        AddLabel("You must enter your decryption key.");
                    }

                    New<SpacerElement>();
                }
            }
            else if (storageSetting == 2) // None
            {
                if (!string.IsNullOrEmpty(TwitchAuth.SessionKey))
                {
                    if (TwitchAuth.IsTokenValid(TwitchAuth.GetToken()))
                        AddLabel("You are authenticated as " + TwitchAuth.GetTokenOwner(TwitchAuth.GetToken()));
                    else
                        AddLabel("Your token is invalid, please generate a new one.");
                }
                else
                {
                    AddLabel("You must set a token this session.");
                }

                New<SpacerElement>();
            }

            AddLabel("Token Storage Method");
            AddInfo("Automatic: Automatically encrypts and decrypts your token.");
            AddInfo("Manual: You must manually enter decryption key each time you start the game.");
            AddInfo("None: You must manually enter your token each time you start the game.");
            AddSelect(token_storage_method);
            New<SpacerElement>();

            if (storageSetting == 1)
                AddButton("Set Decryption Key", delegate { RequestSubMenu(typeof(ProvideDecyptionKey)); });

            if (storageSetting == 1)
            {
                if (!string.IsNullOrEmpty(TwitchAuth.DecryptionKey))
                    AddButton("Generate Token", delegate
                    {
                        MainAsync();
                        Process.Start(TwitchAuth.URLBuilder(Loader.registeredScopes.ToArray()));
                        RequestSubMenu(typeof(ProvideToken));
                    });
            }
            else
            {
                AddButton("Generate Token", delegate
                {
                    MainAsync();
                    Process.Start(TwitchAuth.URLBuilder(Loader.registeredScopes.ToArray()));
                    RequestSubMenu(typeof(ProvideToken));
                });
            }


            New<SpacerElement>();
            New<SpacerElement>();

            AddButton("Back", delegate { RequestPreviousMenu(); });
        }

        private static async Task MainAsync()
        {
            server = new WebServer("http://localhost:8080/redirect/");
            await server.Listen();
        }
    }
}