using Kitchen;
using Kitchen.Modules;
using KitchenLib;
using UnityEngine;

namespace ChefPlusPlus.Menus.PlateUp
{
    public class NavigationMenu<T> : KLMenu<T>
    {
        public NavigationMenu(Transform container, ModuleList module_list) : base(container, module_list)
        {
        }

        public override void Setup(int player_id)
        {
            AddLabel("Chef PlusPlus");
            if (typeof(T) == typeof(PauseMenuAction))
            {
                AddButton("Preferences", delegate { RequestSubMenu(typeof(PreferencesMenu<T>)); });

                AddButton("Twitch Settings", delegate { RequestSubMenu(typeof(TwitchSettings)); });

                AddButton("Event Emulation", delegate { RequestSubMenu(typeof(EventEmulation)); });
            }

            if (typeof(T) == typeof(MainMenuAction))
                AddButton("Twitch Authentication", delegate { RequestSubMenu(typeof(TwitchMenu)); });

            New<SpacerElement>();
            New<SpacerElement>();

            AddButton("Back", delegate { RequestPreviousMenu(); });
        }
    }
}