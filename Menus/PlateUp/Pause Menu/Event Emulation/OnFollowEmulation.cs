using ChefPlusPlus.Utils;
using Kitchen;
using Kitchen.Modules;
using UnityEngine;

namespace ChefPlusPlus.Menus.PlateUp.Event_Emulation
{
    public class OnFollowEmulation : TMenu<PauseMenuAction>
    {
        private string emulationString = "";

        public OnFollowEmulation(Transform container, ModuleList module_list) : base(container, module_list)
        {
        }

        public override void Setup(int player_id)
        {
            Redraw(player_id);
        }

        public async void Redraw(int player_id)
        {
            ModuleList.Clear();

            AddLabel("On Follow Event");

            AddButton("Emulate Follow", delegate
            {
                emulationString = "{\"type\": \"MESSAGE\",\"data\": {\"topic\": \"following." + TwitchAuth.GetUser() + "\",\"message\": \"{\\\"display_name\\\":\\\"StarFluxGames\\\",\\\"username\\\":\\\"starfluxgames\\\",\\\"user_id\\\":\\\"123456789\\\"}\"}}";
                Mod._pubSub.TestMessageParser(emulationString);
            });

            New<SpacerElement>();
            New<SpacerElement>();

            AddButton("Back", delegate { RequestPreviousMenu(); });
            FixTheFuckingPanel();
        }
    }
}