using Kesco.Lib.BaseExtention;
using Kesco.Lib.BaseExtention.Enums.Controls;
using Kesco.Lib.Entities;
using Kesco.Lib.Log;
using Kesco.Lib.Web.Controls.V4;
using Kesco.Lib.Web.Controls.V4.Common;
using Kesco.Lib.Web.Settings;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;

namespace Kesco.App.Web.Inventory.Territories
{
    /// <summary>
    ///     Форма Территории
    /// </summary>
    public partial class Territories : EntityPage
    {
        protected int _territoryId;

        /// <summary>
        ///     Ссылка на справку
        /// </summary>
        public override string HelpUrl { get; set; }

        /// <summary>
        ///     Инициализирует новый экземпляр класса Territories
        /// </summary>
        public Territories()
        {
            HelpUrl = "hlp/help.htm?id=1";
        }

        /// <summary>
        ///     Отрисовка верхней панели меню
        /// </summary>
        /// <returns>Строка, получаемая из StringWriter</returns>
        protected string RenderDocumentHeader()
        {
            using (var w = new StringWriter())
            {
                try
                {
                    ClearMenuButtons();
                    SetMenuButtons();
                    RenderButtons(w);
                }
                catch (Exception e)
                {
                    var dex = new DetailedException(Resx.GetString("Inv_errFailedGenerateButtons") + ": " + e.Message, e);
                    Logger.WriteEx(dex);
                    throw dex;
                }

                return w.ToString();
            }
        }

        /// <summary>
        ///     Инициализация/создание кнопок меню
        /// </summary>
        private void SetMenuButtons()
        {
            var btnSearch = new Button
            {
                ID = "btnSearch",
                V4Page = this,
                Text = Resx.GetString("lblSearch"), // Поиск
                Title = Resx.GetString("lblSearch"),
                Width = 125,
                IconJQueryUI = ButtonIconsEnum.Search,
                OnClick = string.Format("v4_ShowSearchTreeView('{0}');", tvTerritory.ClientID)
            };
            var buttons = new[] { btnSearch };
            AddMenuButton(buttons);
        }

        /// <summary>
        ///     Обработчик события загрузки страницы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Параметр события</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            IsRememberWindowProperties = true;
            WindowParameters = new WindowParameters("InvTerWndLeft", "InvTerWndTop", "InvTerWndWidth", "InvTerWndHeight");
            IsSilverLight = false;

            tvTerritory.SetJsonData("TerritoryData.ashx");
            tvTerritory.SetDataSource("Инвентаризация.dbo.Территории", "vwТерритории", Config.DS_user, "КодТерритории", "Территория", "");

            tvTerritory.IsLoadData = true;
            tvTerritory.LoadData = LoadTreeViewData;

            tvTerritory.IsOrderMenu = true;
            tvTerritory.RootVisible = true;
            tvTerritory.Resizable = true;
            //tvTerritory.Dock = Lib.Web.Controls.V4.TreeView.TreeView.DockStyle.None;
            tvTerritory.IsSaveState = true;
            tvTerritory.ParamName = "InvTerTreeState";
            tvTerritory.ClId = ClId;

            tvTerritory.IsContextMenu = true;
            tvTerritory.ContextMenuAdd = true;
            tvTerritory.ContextMenuRename = true;
            tvTerritory.ContextMenuDelete = true;

            var loadById = 0;
            if (!Request.QueryString["id"].IsNullEmptyOrZero())
                int.TryParse(Request.QueryString["id"], out loadById);
            else if (!Request.QueryString["idloc"].IsNullEmptyOrZero())
                int.TryParse(Request.QueryString["idloc"], out loadById);
            if (loadById != 0)
                tvTerritory.LoadById = loadById;

            InitGridTerritory();
            LoadDataGridTerritory();
        }

        /// <summary>
        ///     Загрузка данных в соотвтетствии с выбранным значением в дереве
        /// </summary>
        /// <param name="id">Идентификатор выбранного значения в дереве</param>
        private void LoadTreeViewData(string id)
        {
            LoadDataTerritory(int.Parse(id));
            RestoreCursor();
        }

        /// <summary>
        ///     Загрузка данных в соответствии с выбранным значением в дереве
        /// </summary>
        /// <param name="id">Идентификатор выбранного значения в дереве</param>
        private void LoadDataTerritory(int id)
        {
            if (id == 0)
            {
                ////JS.Write("$('#tabs').hide();");
                //JS.Write("$('#divLocationInfo').html('');");
                //JS.Write("$('#divSpecifications').html('');");
                //JS.Write("$('#divPrinter').html('');");
                gridTerritory.ClearGridData();
                return;
            }

            _territoryId = id;
            LoadDataGridTerritory();

            //_location = new Location(id.ToString());
            //JS.Write("$('#divLocationInfo').html('{0}');",
            //    HttpUtility.JavaScriptStringEncode("[" + _location.Id + "] " + _location.NamePath1));
            //RenderLocationSpecifications();
            //RenderNearPrinter();
            //JS.Write("$('#btnSocketAdd').show();");
            //SetSocketGridDataSource();
            //SetEmployeeGridDataSource();
            //SetEqupmentGridDataSource();
            ////JS.Write("$('#tabs').show();");
        }


        /// <summary>
        ///     Инициализация таблицы Территории
        /// </summary>
        private void InitGridTerritory()
        {
            gridTerritory.ExistServiceColumn = true;
            //gridTerritory.SetServiceColumnAdd("rate_add", Resx.GetString("ExRate_btnAddPosition"));
            gridTerritory.EmptyDataString = Resx.GetString("Inv_msgNoTerritories");
            gridTerritory.ShowGroupPanel = false;
            gridTerritory.RowsPerPage = 47;
        }

        /// <summary>
        ///     Загрузка данных в таблицу Территории
        /// </summary>
        private void LoadDataGridTerritory()
        {
            var sqlParams = GetSqlParamsForGridTerritory();

            gridTerritory.SetDataSource(SQLQueries.SELECT_ТерриторииПодчиненные, Config.DS_user, CommandType.Text, sqlParams);

            SetSettingsGridTerritory();

            gridTerritory.RefreshGridData();
        }

        /// <summary>
        /// Получение параметров SQL-запроса для таблицы Территории
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, object> GetSqlParamsForGridTerritory()
        {
            var sqlParams = new Dictionary<string, object>();

            sqlParams.Add("@Код", _territoryId);

            return sqlParams;
        }

        /// <summary>
        ///     Задание настроек для таблицы Территории
        /// </summary>
        private void SetSettingsGridTerritory()
        {

        }
    }
}