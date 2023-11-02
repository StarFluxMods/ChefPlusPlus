using ChefPlusPlus.Utils;
using Kitchen.Modules;
using UnityEngine;

namespace ChefPlusPlus.Menus.PlateUp
{
    public class PreferencesMenu<T> : TMenu<T>
    {
        private bool Is_Reloaded;

        public PreferencesMenu(Transform container, ModuleList module_list) : base(container, module_list)
        {
        }

        public override void Setup(int player_id)
        {
            Is_Reloaded = false;
            Redraw(player_id);
        }

        public async void Redraw(int player_id)
        {
            ModuleList.Clear();
            AddLabel("Preferences");

            if (Is_Reloaded)
            {
                New<SpacerElement>();
                AddInfo("Configs Reloaded.");
            }

            AddButton("Reload Configs", delegate
            {
                Is_Reloaded = true;
                Loader.LoadConfigs();
                Redraw(player_id);
            });

            New<SpacerElement>();
            New<SpacerElement>();

            AddButton("Back", delegate { RequestPreviousMenu(); });
            FixTheFuckingPanel();
        }
    }
}