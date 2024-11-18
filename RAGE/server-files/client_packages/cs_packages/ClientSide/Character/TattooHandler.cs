using System;
using System.Collections.Generic;
using System.Text;
using RAGE;
using RAGE.NUI;
namespace ClientSide.Character
{
    internal class TattooHandler : Events.Script
    {
        
        public TattooHandler() 
        {
            Events.Add("SERVER:CLIENT:OpenTattooMenu", OpenTattooMenu);
        }

        private void OpenTattooMenu(object[] args)
        {
            
            MenuPool menuPool = new MenuPool();

            UIMenu tattooMenu = new UIMenu("Tattoo Menu", "Choose the tattoo you want");

            menuPool.Add(tattooMenu);


            var tattooNames = new List<dynamic>
            {
                "haa",
                "fdsfsd"
            };
            UIMenuListItem tattoos = new UIMenuListItem("tatoos", tattooNames, 0);
            tattooMenu.AddItem(tattoos);

            UIMenuItem applyButton = new UIMenuItem("Apply");
            tattooMenu.AddItem(applyButton);

            string selectedTattoo = string.Empty;

            tattooMenu.OnListChange += (sender, item, index) =>
            {
                if (sender == tattooMenu)
                {
                    if(item == tattoos)
                    {
                        selectedTattoo = item.IndexToItem(index).ToString();
                    }    
                }
            };
            tattooMenu.OnItemSelect += (sender, item, index) =>
            {
                if(sender == tattooMenu)
                    if(item == applyButton)
                    {
                        RAGE.Chat.Output(tattoos.CurrentItem());
                    }
            };
            tattooMenu.Visible = true;
            tattooMenu.FreezeAllInput = true;
            tattooMenu.RefreshIndex();

            Events.Tick += (name) =>
            {
                menuPool.ProcessMenus();
            };
        }
    }
}
