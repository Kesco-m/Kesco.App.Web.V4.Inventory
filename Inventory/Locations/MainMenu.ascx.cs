using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Principal;
using System.Web.UI;
using Kesco.Lib.BaseExtention.Enums.Controls;
using Kesco.Lib.DALC;
using Kesco.Lib.Entities;
using Kesco.Lib.Web.Controls.V4;
using Kesco.Lib.Web.Settings;

namespace Kesco.App.Web.Inventory
{
    /// <summary>
    ///     Класс элемента управления Menu
    /// </summary>
    public partial class MainMenu : UserControl
    {
        /// <summary>
        ///     Событие загрузки страницы
        /// </summary>
        /// <param name="sender">Объект страницы</param>
        /// <param name="e">Аргументы</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            RenderMainMenu();
        }

        /// <summary>
        ///     Отриcовка меню
        /// </summary>
        private void RenderMainMenu()
        {
            var inventoryRootItem = new Menu.MenuTree
            {
                ItemID = "inventoryRootItem",
                NameRUS = "Инвентаризация",
                Order = 1,
                ButtonWidth = 150
            };

            var dissonanceRootItem = new Menu.MenuTree
            {
                ItemID = "dissonanceRootItem",
                NameRUS = "Несоответствия",
                Order = 2,
                //BeforeButtonSeparator = true,
                ButtonWidth = 150
            };

            var reportRootItem = new Menu.MenuTree
            {
                ItemID = "reportRootItem",
                NameRUS = "Отчеты",
                Order = 3,
                // BeforeButtonSeparator = true,
                ButtonWidth = 110
            };

            var operationRootItem = new Menu.MenuTree
            {
                ItemID = "operationRootItem",
                NameRUS = "Операции",
                Order = 4,
                // BeforeButtonSeparator = true,
                ButtonWidth = 110
            };

            var settingRootItem = new Menu.MenuTree
            {
                ItemID = string.Format("subItem_{0}_mgSettings", 5),
                NameRUS = "Настройки инвентаризации",
                Order = 5,
                ButtonWidth = 200,
                ActionType = MenuButtonActionType.UrlAction,
                Action = new Menu.MenuActionParametrs
                {
                    Url = "/Inventory/Default/Queries.aspx",
                    OpenNewWindow = false,
                    Width = 1024,
                    Height = 768,
                    Location = false,
                    Menubar = false,
                    Target = "_self"
                },
                Image = "tools.gif"
            };

            FillRootMenuSubItems(inventoryRootItem, 1);
            FillRootMenuSubItems(dissonanceRootItem, 2, 350);
            FillRootMenuSubItems(reportRootItem, 3, 350);

            FillOperationSubItems(operationRootItem, 4, 180);

            menuControl.MenusItems.Add(inventoryRootItem);
            menuControl.MenusItems.Add(dissonanceRootItem);
            menuControl.MenusItems.Add(reportRootItem);
            menuControl.MenusItems.Add(operationRootItem);

            if (Page.User.IsInRole(@"BUILTIN\Administrators") || Page.User.IsInRole(@"TEST\Programists"))
                menuControl.MenusItems.Add(settingRootItem);
        }

        /// <summary>
        ///     Отриcовка меню инвентаризации, несоответствий и отчетов
        /// </summary>
        private void FillRootMenuSubItems(Menu.MenuTree rootItem, int queryType, int widthItem = 230)
        {
            var sqlParams = new Dictionary<string, object> {{"@КодТипаЗапроса", queryType}};

            var dt = DBManager.GetData(SQLQueries.SELECT_ЗапросыПоТипу,
                Config.DS_user, CommandType.Text, sqlParams);

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
                        Url = dt.Rows[i]["ФормаСписка"].ToString().Length == 0
                            ? "/Inventory/default/grid.aspx?id=" + dt.Rows[i]["КодЗапроса"]
                            : dt.Rows[i]["ФормаСписка"].ToString(),
                        OpenNewWindow = false,
                        Width = 1024,
                        Height = 768,
                        Location = false,
                        Menubar = false,
                        Target = "_self"
                    },
                    Image = dt.Rows[i]["Пиктограмма"].ToString().Replace("/styles/", "")
                };

                rootItem.ItemsList.Add(subItem);
            }
        }

        /// <summary>
        ///     Отризовка меню операций
        /// </summary>
        private void FillOperationSubItems(Menu.MenuTree rootItem, int queryType, int widthItem = 230)
        {
            var subItem = new Menu.MenuTree
            {
                ItemID = string.Format("subItem_{0}_Equipment_move", queryType),
                NameRUS = "Передача оборудования",
                Order = 0,
                ButtonWidth = widthItem,
                ActionType = MenuButtonActionType.UrlAction,
                Action = new Menu.MenuActionParametrs
                {
                    Url = "/Inventory/inv/Equipment_move.aspx",
                    OpenNewWindow = false,
                    Width = 1024,
                    Height = 768,
                    Location = false,
                    Menubar = false,
                    Target = "_self"
                },
                Image = "bin.gif"
            };

            rootItem.ItemsList.Add(subItem);

            subItem = new Menu.MenuTree
            {
                ItemID = string.Format("subItem_{0}_sticker", queryType),
                NameRUS = "Печать этикеток",
                Order = 0,
                ButtonWidth = widthItem,
                ActionType = MenuButtonActionType.UrlAction,
                Action = new Menu.MenuActionParametrs
                {
                    Url = "/Inventory/default/grid.aspx?id=1001&sticker=1",
                    OpenNewWindow = false,
                    Width = 1024,
                    Height = 768,
                    Location = false,
                    Menubar = false,
                    Target = "_self"
                },
                Image = "print.gif"
            };

            rootItem.ItemsList.Add(subItem);
        }

    }
}