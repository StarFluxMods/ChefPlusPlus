using ChefPlusPlus.Menus.PlateUp.Event_Emulation;
using Kitchen;
using Kitchen.Modules;
using UnityEngine;

namespace ChefPlusPlus.Menus.PlateUp
{
    public class EventEmulation : TMenu<PauseMenuAction>
    {
        public EventEmulation(Transform container, ModuleList module_list) : base(container, module_list)
        {
        }

        public override void Setup(int player_id)
        {
            Redraw(player_id);
        }

        public async void Redraw(int player_id)
        {
            ModuleList.Clear();

            AddLabel("Event Emulation");

            AddButton("On Follow Event", delegate { RequestSubMenu(typeof(OnFollowEmulation)); });

            AddButton("On Bits Event", delegate { RequestSubMenu(typeof(OnBitsEmulation)); });

            New<SpacerElement>();
            New<SpacerElement>();

            AddButton("Back", delegate { RequestPreviousMenu(); });
            FixTheFuckingPanel();
        }
    }
}