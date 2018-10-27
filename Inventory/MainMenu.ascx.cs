using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Kesco.Lib.BaseExtention.Enums.Controls;
using Kesco.Lib.Web.Controls.V4;
using Menu = Kesco.Lib.Web.Controls.V4.Menu;

namespace Kesco.App.Web.Inventory
{
    public partial class MainMenu : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RenderMainMenu();
        }

        private void RenderMainMenu()
        {
            var inventoryRootItem = new Menu.MenuTree
            {
                ItemID = "inventoryRootItem",
                NameRUS = "Инвентаризация",
                Order = 1,
                ButtonWidth = 150,
            };

            var dissonanceRootItem = new Menu.MenuTree
            {
                ItemID = "dissonanceRootItem",
                NameRUS = "Несоответствия",
                Order = 2,
                //BeforeButtonSeparator = true,
                ButtonWidth = 150,
            };

            var reportRootItem = new Menu.MenuTree
            {
                ItemID = "reportRootItem",
                NameRUS = "Отчеты",
                Order = 3,
                // BeforeButtonSeparator = true,
                ButtonWidth = 110,
            };

            var operationRootItem = new Menu.MenuTree
            {
                ItemID = "operationRootItem",
                NameRUS = "Операции",
                Order = 4,
                // BeforeButtonSeparator = true,
                ButtonWidth = 110,
            };

            FillRootMenuSubItems(inventoryRootItem, 1);
            FillRootMenuSubItems(dissonanceRootItem, 2, 350);
            FillRootMenuSubItems(reportRootItem, 3, 350);


            menuControl.MenusItems.Add(inventoryRootItem);
            menuControl.MenusItems.Add(dissonanceRootItem);
            menuControl.MenusItems.Add(reportRootItem);
            menuControl.MenusItems.Add(operationRootItem);
        }

        private void FillRootMenuSubItems(Menu.MenuTree rootItem, int queryType, int widthItem=230)
        {
            var sqlParams = new Dictionary<string, object> {{"@КодТипаЗапроса", queryType}};
            
            DataTable dt = Kesco.Lib.DALC.DBManager.GetData(Kesco.Lib.Entities.SQLQueries.SELECT_ЗапросыПоТипу,
                Kesco.Lib.Web.Settings.Config.DS_user, CommandType.Text, sqlParams);

            for (var i = 0; i < dt.Rows.Count; i++)
            {
                var subItem = new Menu.MenuTree
                {
                    ItemID = string.Format("subItem_{0}_{1}", queryType, dt.Rows[i]["КодЗапроса"]),
                    NameRUS = dt.Rows[i]["Запрос"].ToString(),
                    Order = i,
                    ButtonWidth = widthItem,
                    ActionType = MenuButtonActionType.UrlAction,
                    Action = new Menu.MenuActionParametrs
                    {
                        Url = (dt.Rows[i]["ФормаСписка"].ToString().Length == 0) ? "http://ktz-nick.testcom.com/Inventory/default/grid.aspx?id=" + dt.Rows[i]["КодЗапроса"] : dt.Rows[i]["ФормаСписка"].ToString(),
                        OpenNewWindow = false,
                        Width = 1024,
                        Height = 768,
                        Location = false,
                        Menubar = false
                        
                    },
                    Image = dt.Rows[i]["Пиктограмма"].ToString().Replace("/styles/", "")
                };

                rootItem.ItemsList.Add(subItem);
            }




        }
    }
}