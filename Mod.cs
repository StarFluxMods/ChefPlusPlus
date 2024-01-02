using System;
using System.Collections.Generic;
using System.Reflection;
using ChefPlusPlus.Enums;
using ChefPlusPlus.Menus.PlateUp;
using ChefPlusPlus.Menus.PlateUp.Event_Emulation;
using ChefPlusPlus.Triggers;
using ChefPlusPlus.Utils;
using Kitchen;
using KitchenLib;
using KitchenLib.Event;
using KitchenLib.Preferences;
using KitchenMods;
using TwitchLib.Api;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Unity;
using UnityEngine;

namespace ChefPlusPlus
{
    public class Mod : BaseMod, IModSystem
    {
        public const string MOD_GUID = "com.starfluxgames.chefplusplus";
        public const string MOD_NAME = "Chef PlusPlus";
        public const string MOD_VERSION = "0.1.0";
        public const string MOD_BETA_VERSION = "7";
        public const string MOD_AUTHOR = "StarFluxGames";
        public const string MOD_GAMEVERSION = ">=1.1.8";

        public static PreferenceManager manager;
        public static PreferenceManager tokenManager;
        public static List<Action> actions = new();

        public static PubSub _pubSub;
        public static TwitchClient _client;
        public static TwitchAPI api;

        public Mod() : base(MOD_GUID, MOD_NAME, MOD_AUTHOR, MOD_VERSION, MOD_BETA_VERSION, MOD_GAMEVERSION, Assembly.GetExecutingAssembly())
        {
        }

        protected override void OnInitialise()
        {
            LogWarning($"{MOD_GUID} v{MOD_VERSION} in use!");

            if (TwitchAuth.GetToken() == "decryption_error")
            {
                LogError("Decryption key is invalid, please try a different one.");
                return;
            }

            if (!TwitchAuth.IsTokenValid(TwitchAuth.GetToken()))
            {
                LogError("Token is invalid, please generate a new one.");
                return;
            }

            _pubSub = new PubSub();

            _client = new TwitchClient(new WebSocketClient());
            _client.Initialize(new ConnectionCredentials(TwitchAuth.GetTokenOwner(TwitchAuth.GetToken()), TwitchAuth.GetToken()), TwitchAuth.GetTokenOwner(TwitchAuth.GetToken()));


            foreach (Action action in actions) action.Invoke();

            _pubSub.OnPubSubServiceConnected += (sender, args) =>
            {
                LogInfo("PubSubServiceConnected!");
                _pubSub.SendTopics(TwitchAuth.GetToken());
            };


            _pubSub.Connect();
            _client.Connect();

            api = new TwitchAPI();
            api.Settings.ClientId = TwitchAuth.ClientID;
            api.Settings.AccessToken = TwitchAuth.GetToken();
            api.Settings.SkipDynamicScopeValidation = true;

            Loader.LoadConfigs();
        }

        protected override void OnPostActivate(KitchenMods.Mod mod)
        {
            SetupPreferences();
            SetupMenus();
            SetupBaseTriggers();
        }

        private void SetupBaseTriggers()
        {
            actions.Add(() =>
            {
                _pubSub.OnPubSubServiceConnected += (sender, args) =>
                {
                    _pubSub.ListenToFollows(TwitchAuth.GetUser());
                    _pubSub.ListenToBitsEventsV2(TwitchAuth.GetUser());
                    _pubSub.ListenToChannelPoints(TwitchAuth.GetUser());
                    _pubSub.ListenToSubscriptions(TwitchAuth.GetUser());
                };

                _pubSub.OnChannelPointsRewardRedeemed += (sender, e) => { Loader.triggers[EffectTrigger.OnChannelPointsRewardRedeemed].OnRun(sender, e); };

                _pubSub.OnBitsReceivedV2 += (sender, e) => { Loader.triggers[EffectTrigger.OnBitsReceivedV2].OnRun(sender, e); };

                _pubSub.OnChannelSubscription += (sender, e) => { Loader.triggers[EffectTrigger.OnChannelSubscription].OnRun(sender, e); };

                _pubSub.OnFollow += (sender, e) => { Loader.triggers[EffectTrigger.OnFollow].OnRun(sender, e); };

                _client.OnRaidNotification += (sender, args) => { Loader.triggers[EffectTrigger.OnRaidNotification].OnRun(sender, args); };

                _client.OnChatCommandReceived += (sender, args) => { Loader.triggers[EffectTrigger.OnChatCommandReceived].OnRun(sender, args); };
            });

            Loader.RegisterConfig(EffectTrigger.OnChannelPointsRewardRedeemed, "OnChannelPointsRewardRedeemed");
            Loader.RegisterConfig(EffectTrigger.OnBitsReceivedV2, "OnBitsReceivedV2");
            Loader.RegisterConfig(EffectTrigger.OnChannelSubscription, "OnChannelSubscription");
            Loader.RegisterConfig(EffectTrigger.OnFollow, "OnFollow");
            Loader.RegisterConfig(EffectTrigger.OnRaidNotification, "OnRaidNotification");
            Loader.RegisterConfig(EffectTrigger.OnChatCommandReceived, "OnChatCommandReceived");

            Loader.RegisterTrigger<Trigger_OnChannelPointsRewardRedeemed>();
            Loader.RegisterTrigger<Trigger_OnBitsReceivedV2>();
            Loader.RegisterTrigger<Trigger_OnChannelSubscription>();
            Loader.RegisterTrigger<Trigger_OnFollow>();
            Loader.RegisterTrigger<Trigger_OnRaidNotification>();
            Loader.RegisterTrigger<Trigger_OnChatCommandReceived>();

            Loader.RegisterScope("bits:read");
            Loader.RegisterScope("chat:read");
            Loader.RegisterScope("chat:edit");
            Loader.RegisterScope("channel:read:subscriptions");
            Loader.RegisterScope("channel:read:redemptions");
            Loader.RegisterScope("channel:manage:redemptions");
            Loader.RegisterScope("channel:manage:raids");
            Loader.RegisterScope("moderator:read:followers");

            Loader.RegisterScope("channel:read:hype_train");
            Loader.RegisterScope("channel:read:polls");
            Loader.RegisterScope("channel:read:predictions");
            Loader.RegisterScope("channel:read:goals");
            Loader.RegisterScope("moderator:read:shoutouts");
            Loader.RegisterScope("channel:read:guest_star");
            Loader.RegisterScope("moderator:read:guest_star");
        }

        private void SetupPreferences()
        {
            manager = new PreferenceManager(MOD_GUID);
            tokenManager = new PreferenceManager(MOD_GUID + ".token");
            tokenManager.RegisterPreference(new PreferenceString("USER_ID", ""));
            tokenManager.RegisterPreference(new PreferenceString("ACCESS_TOKEN", ""));
            manager.RegisterPreference(new PreferenceInt("TOKEN_STORAGE_METHOD"));
            tokenManager.Load();
            manager.Load();
        }

        private void SetupMenus()
        {
            ModsPreferencesMenu<PauseMenuAction>.RegisterMenu("Chef PlusPlus", typeof(NavigationMenu<PauseMenuAction>), typeof(PauseMenuAction));
            ModsPreferencesMenu<MainMenuAction>.RegisterMenu("Chef PlusPlus", typeof(NavigationMenu<MainMenuAction>), typeof(MainMenuAction));

            Events.PreferenceMenu_PauseMenu_CreateSubmenusEvent += (s, args) =>
            {
                args.Menus.Add(typeof(NavigationMenu<PauseMenuAction>), new NavigationMenu<PauseMenuAction>(args.Container, args.Module_list));
                args.Menus.Add(typeof(PreferencesMenu<PauseMenuAction>), new PreferencesMenu<PauseMenuAction>(args.Container, args.Module_list));
                args.Menus.Add(typeof(CreateNewReward), new CreateNewReward(args.Container, args.Module_list));
                args.Menus.Add(typeof(TwitchSettings), new TwitchSettings(args.Container, args.Module_list));
                args.Menus.Add(typeof(CustomRewardSettings), new CustomRewardSettings(args.Container, args.Module_list));
                args.Menus.Add(typeof(EventEmulation), new EventEmulation(args.Container, args.Module_list));
                args.Menus.Add(typeof(OnFollowEmulation), new OnFollowEmulation(args.Container, args.Module_list));
                args.Menus.Add(typeof(OnBitsEmulation), new OnBitsEmulation(args.Container, args.Module_list));
            };
            ;

            Events.PreferenceMenu_MainMenu_CreateSubmenusEvent += (s, args) =>
            {
                args.Menus.Add(typeof(NavigationMenu<MainMenuAction>), new NavigationMenu<MainMenuAction>(args.Container, args.Module_list));
                args.Menus.Add(typeof(PreferencesMenu<MainMenuAction>), new PreferencesMenu<MainMenuAction>(args.Container, args.Module_list));
                args.Menus.Add(typeof(TwitchMenu), new TwitchMenu(args.Container, args.Module_list));
                args.Menus.Add(typeof(ProvideToken), new ProvideToken(args.Container, args.Module_list));
                args.Menus.Add(typeof(ProvideDecyptionKey), new ProvideDecyptionKey(args.Container, args.Module_list));
            };
            ;
        }

        #region Logging

        public static void LogInfo(string _log)
        {
            Debug.Log($"[{MOD_NAME}] " + _log);
        }

        public static void LogWarning(string _log)
        {
            Debug.LogWarning($"[{MOD_NAME}] " + _log);
        }

        public static void LogError(string _log)
        {
            Debug.LogError($"[{MOD_NAME}] " + _log);
        }

        public static void LogInfo(object _log)
        {
            LogInfo(_log.ToString());
        }

        public static void LogWarning(object _log)
        {
            LogWarning(_log.ToString());
        }

        public static void LogError(object _log)
        {
            LogError(_log.ToString());
        }

        #endregion
    }
}