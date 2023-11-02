using System.Reflection;
using Kitchen;
using Kitchen.Modules;
using KitchenLib;
using KitchenLib.Utils;
using UnityEngine;

namespace ChefPlusPlus.Menus.PlateUp
{
    public class TMenu<T> : KLMenu<T>
    {
        private readonly Transform container;
        private readonly PlayerPauseView view;

        public TMenu(Transform container, ModuleList module_list) : base(container, module_list)
        {
            view = container.transform.parent.parent.parent.GetComponent<PlayerPauseView>();
            this.container = container;
        }

        protected void FixTheFuckingPanel()
        {
            MethodInfo setparneltarget = ReflectionUtils.GetMethod<PlayerPauseView>("SetPanelTarget");
            container.transform.parent.parent.parent.localPosition = -ModuleList.BoundingBox.center;
            setparneltarget.Invoke(view, new object[] { ModuleList });
        }
    }
}