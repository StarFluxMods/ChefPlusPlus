using System.Collections.Generic;
using ChefPlusPlus.Utils;
using Kitchen;
using Kitchen.Modules;
using UnityEngine;

namespace ChefPlusPlus.Menus.PlateUp.Event_Emulation
{
    public class OnBitsEmulation : TMenu<PauseMenuAction>
    {
        private readonly Option<int> bits = new(new List<int> { 1, 10, 50, 100, 200, 500 }, 1, new List<string> { "1", "10", "50", "100", "200", "500" });

        private string emulationString = "";

        private int selectedBits = 1;

        public OnBitsEmulation(Transform container, ModuleList module_list) : base(container, module_list)
        {
        }

        public override void Setup(int player_id)
        {
            Redraw(player_id);

            bits.OnChanged += (sender, i) => { selectedBits = i; };
        }

        public async void Redraw(int player_id)
        {
            ModuleList.Clear();

            AddLabel("On Bits Event");

            New<SpacerElement>();

            AddLabel("How many bits?");
            AddSelect(bits);

            AddButton("Emulate Bits", delegate
            {
                emulationString = "{\"type\": \"MESSAGE\",\"data\": {\"topic\": \"channel-bits-events-v2." + TwitchAuth.GetUser() + "\",\"message\": \"{\\\"data\\\":{\\\"user_name\\\":\\\"starfluxgames\\\",\\\"channel_name\\\":\\\"" + TwitchAuth.GetTokenOwner(TwitchAuth.GetToken()) + "\\\",\\\"user_id\\\":\\\"962371836\\\",\\\"channel_id\\\":\\\"470687940\\\",\\\"time\\\":\\\"2023-10-18T08:03:31.283778701Z\\\",\\\"chat_message\\\":\\\"Hello Cheer" + selectedBits + " World\\\",\\\"bits_used\\\":" + selectedBits + ",\\\"total_bits_used\\\":" + selectedBits + ",\\\"is_anonymous\\\":false,\\\"context\\\":\\\"cheer\\\",\\\"badge_entitlement\\\":null},\\\"version\\\":\\\"1.0\\\",\\\"message_type\\\":\\\"bits_event\\\",\\\"message_id\\\":\\\"013af654-d057-50d2-b58b-6cae9e4d4485\\\"}\"}}";
                Mod._pubSub.TestMessageParser(emulationString);
            });

            New<SpacerElement>();
            New<SpacerElement>();

            AddButton("Back", delegate { RequestPreviousMenu(); });
            FixTheFuckingPanel();
        }
    }
}