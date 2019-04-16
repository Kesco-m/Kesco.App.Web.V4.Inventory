using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Web;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.BaseExtention.Enums.Controls;
using Kesco.Lib.DALC;
using Kesco.Lib.Entities;
using Kesco.Lib.Web.Controls.V4.Common;
using Kesco.Lib.Web.Settings;

namespace Kesco.App.Web.Inventory
{
    /// <summary>
    ///     Класс формы Locations
    /// </summary>
    public partial class Locations : EntityPage
    {
        private Location _location;
        protected int LocationId;
        protected int SocketId;
        private int _loadById;

        /// <summary>
        ///     Конструктор по умолчанию
        /// </summary>
        public Locations()
        {
            HelpUrl = "hlp/help.htm?id=1";
        }

        public override string HelpUrl { get; set; }

        /// <summary>
        ///     Событие пред-загрузки страницы
        /// </summary>
        /// <param name="sender">Объект страницы</param>
        /// <param name="e">Аргументы</param>
        protected void Page_PreInit(object sender, EventArgs e)
        {
            tvLocation.SetJsonData("LocationData.ashx");
            tvLocation.SetService("AddLocation", "EditLocation", "");
            tvLocation.SetDataSource("Инвентаризация.dbo.Расположения", "vwРасположения", Config.DS_user,
                "КодРасположения", "Расположение", "РасположениеPath1");

            tvLocation.IsLoadData = true;
            tvLocation.LoadData = LoadTreeViewData;

            tvLocation.IsSaveState = true;
            tvLocation.ParamName = "InvLocationTreeState";
            tvLocation.ClId = ClId;

            tvLocation.IsContextMenu = true;
            tvLocation.ContextMenuAdd = true;
            tvLocation.ContextMenuRename = true;
            tvLocation.ContextMenuDelete = false;

            tvLocation.IsOrderMenu = false;
            tvLocation.IsSearchMenu = true;
            tvLocation.Resizable = true;

            ReturnId = string.IsNullOrEmpty(Request.QueryString["return"]) ? "" : Request.QueryString["return"];
            if (!ReturnId.IsNullEmptyOrZero())
                if (Request.QueryString["socket"] == "1")
                {
                    tvLocation.ReturnType = "socket";
                    //JS.Write("tabActivate(1);");
                }

            if (!Request.QueryString["id"].IsNullEmptyOrZero())
                int.TryParse(Request.QueryString["id"], out _loadById);
            else if (!Request.QueryString["idloc"].IsNullEmptyOrZero())
                int.TryParse(Request.QueryString["idloc"], out _loadById);
            if (_loadById != 0)
            {
                if (Request.QueryString["socket"] == "1")
                {
                    var sqlParams = new Dictionary<string, object> { { "@id", _loadById } };
                    var dt = DBManager.GetData(SQLQueries.SELECT_РозеткаПоID, Config.DS_user, CommandType.Text, sqlParams);
                    if (dt.Rows.Count > 0)
                    {
                        tvLocation.LoadById = Convert.ToInt32(dt.Rows[0]["КодРасположения"].ToString());
                    }
                }
                else
                    tvLocation.LoadById = _loadById;
            }


            IsSilverLight = false;
        }

        /// <summary>
        ///     Событие загрузки страницы
        /// </summary>
        /// <param name="sender">Объект страницы</param>
        /// <param name="e">Аргументы</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            btnSocketAdd.OnClick = "cmd('cmd','SocketAdd');";
            btnSocketAdd.Text = Resx.GetString("Inv_lblAddSocket") + "&nbsp;(Ins)";

            IsRememberWindowProperties = true;
            WindowParameters = new WindowParameters("InvLocSrchWndLeft", "InvLocSrchWndTop", "InvLocSrchWidth", "InvLocSrchHeight");
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
        }

        protected void Page_UnLoad(object sender, EventArgs e)
        {
            if (_loadById != 0)
            {
                JS.Write("$('#{0}_anchor').focus();", _loadById);
            }
        }

        /// <summary>
        ///     Загрузка данных в соотвтетствии с выбранным значением в дереве
        /// </summary>
        /// <param name="id">Идентификатор выбранного значения в дереве</param>
        private void LoadTreeViewData(string id)
        {
            LoadDataLocation(int.Parse(id));
            RestoreCursor();
        }

        /// <summary>
        ///     Обработка клиентских команд
        /// </summary>
        /// <param name="cmd">Команды</param>
        /// <param name="param">Параметры</param>
        protected override void ProcessCommand(string cmd, NameValueCollection param)
        {
            List<string> validList;
            switch (cmd)
            {
                case "LoadTreeViewData":
                    LoadDataLocation(int.Parse(param["Id"]));
                    RestoreCursor();
                    break;
                case "SocketDelete":
                    if (!param["SocketId"].IsNullEmptyOrZero())
                    {
                        var socketId = Convert.ToInt32(param["SocketId"]);
                        DBManager.ExecuteNonQuery(SQLQueries.DELETE_ID_Розетки, socketId, CommandType.Text,
                            Config.DS_user);
                        LoadDataLocation(Convert.ToInt32(_location.Id));
                    }

                    break;
                case "SocketAdd":
                    SocketId = 0;
                    tbSocket.Value = "";
                    cbWork.Checked = true;
                    tbNote.Value = "";
                    JS.Write("socketEdit_dialogShow('{0}','{1}','{2}','{3}');", Resx.GetString("Inv_lblAddingSocket"),
                        Resx.GetString("cmdSave"), Resx.GetString("cmdCancel"), cmd);
                    break;
                case "SocketEdit":
                {
                    SocketId = int.Parse(param["SocketId"]);
                    var sqlParams = new Dictionary<string, object> {{"@id", SocketId}};
                    var dt = DBManager.GetData(SQLQueries.SELECT_РозеткаПоID, Config.DS_user, CommandType.Text,
                        sqlParams);
                    if (dt.Rows.Count > 0)
                    {
                        tbSocket.Value = dt.Rows[0]["Розетка"].ToString();
                        cbWork.Checked = Convert.ToBoolean(dt.Rows[0]["Работает"]);
                        tbNote.Value = dt.Rows[0]["Примечание"].ToString();
                    }

                    JS.Write("socketEdit_dialogShow('{0}','{1}','{2}','{3}');", Resx.GetString("Inv_lblSocketEditing"),
                        Resx.GetString("cmdSave"), Resx.GetString("cmdCancel"), cmd);
                }
                    break;
                case "RenderUserCount":
                    LoadUserCount(int.Parse(param["Id"]));
                    break;
                case "AddLocation":
                case "EditLocation":
                {
                    LocationId = int.Parse(param["Id"]);
                    var sqlParams = new Dictionary<string, object> {{"@id", LocationId}};
                    var dt = DBManager.GetData(SQLQueries.SELECT_РасположениеПоID, Config.DS_user, CommandType.Text,
                        sqlParams);
                    if (LocationId == 0 || dt.Rows.Count > 0)
                    {
                        tbNode.Value = cmd == "AddLocation" ? "" : dt.Rows[0]["Расположение"].ToString();
                        divLocationPatch.Value = LocationId == 0 ? "" : dt.Rows[0]["РасположениеPath1"].ToString().Replace("_", " ");
                        JS.Write("locationEdit_dialogShow('{0}','{1}','{2}','{3}');", (cmd == "AddLocation"
                                                                                          ? Resx.GetString(
                                                                                              "lblAddition")
                                                                                          : Resx.GetString("lblEdit")) +
                                                                                      " " + Resx.GetString(
                                                                                          "Inv_msgLocation"),
                            Resx.GetString("cmdSave"), Resx.GetString("cmdCancel"), cmd);
                    }
                }
                    break;
                case "SetLocation":
                    if (ValidateLocation(out validList))
                        SaveData(param["type"]);
                    else
                        RenderErrors(validList, "<br/> " + Resx.GetString("_Msg_НеСохраняется"));
                    break;
                case "SetSocket":
                    if (ValidateSocket(out validList))
                        SaveSocketData();
                    else
                        RenderErrors(validList, "<br/> " + Resx.GetString("_Msg_НеСохраняется"));
                    break;
                case "TypeWorkPlaceChanged":
                {
                    var id = param["Id"];
                    var val = param["val"];
                    var check = param["check"];
                    //var parent = param["parent"];
                    //var l = param["l"];
                    //var name = param["name"];
                    var cbid = param["cbid"];
                    
                    if (id == "-1") break;
                    var isErr = false;
                    //var ItemID_L = id + "," + l;

                    var sqlParams = new Dictionary<string, object>
                        {
                            {"@КодРасположения", id},
                            {"@РабочееМесто", check == "1" ? val : "0"}
                        };

                    try
                    {
                        DBManager.ExecuteNonQuery(SQLQueries.UPDATE_Расположения_РабочееМесто, CommandType.Text, Config.DS_user, sqlParams);
                    }
                    catch (Exception ex)
                    {
                        ShowMessage(ex.Message, Resx.GetString("alertError"));
                        isErr = true;                        
                    }


                    if (isErr)
                        ResetCurCheckBox(val, check);
                    if (check == "1" && !isErr)
                        ResetCheckBox(val);

                    JS.Write("document.getElementById('{0}').disabled = false; ", cbid);
                    
                    if (!isErr)
                        JS.Write("v4_reloadNode('tvLocation');");

                }
                    break;
                case "OfficeChanged":
                {
                    var id = param["Id"];
                    var val = param["val"];
                    var check = param["check"];
                    //var parent = param["parent"];
                    //var l = param["l"];
                    //var name = param["name"];
                    var cbid = param["cbid"];
                    
                    if (id == "-1") break;
                    var isErr = false;
                    //var ItemID_L = id + "," + l;

                    var sqlParams = new Dictionary<string, object>
                        {
                            {"@КодРасположения", id},
                            {"@Офис", check == "1" ? val : "0"}
                        };
                    try
                    {
                        DBManager.ExecuteNonQuery(SQLQueries.UPDATE_Расположения_Офис, CommandType.Text, Config.DS_user, sqlParams);
                    }
                    catch (Exception ex)
                    {
                        ShowMessage(ex.Message, Resx.GetString("alertError"));
                        isErr = true;                        
                    }

                    if (check == "1" && val == "3" && !isErr)
                    {
                        JS.Write(@"document.getElementById(""cbOffice"").checked = false;");
                        JS.Write("v4_reloadNode('tvLocation');");
                    }
                    else if (check == "1" && val == "1" && !isErr)
                    {
                        JS.Write(@"document.getElementById(""cbServer"").checked = false;");
                        JS.Write("v4_reloadNode('tvLocation');");
                    }
                    if (check == "0" && val == "3" && !isErr)
                    {
                        JS.Write("v4_reloadNode('tvLocation');");
                    }
                    else if (check == "0" && val == "1" && !isErr)
                    {
                        JS.Write("v4_reloadNode('tvLocation');");
                    }
                    else if (check == "1" && val == "3" && isErr)
                    {
                        JS.Write(@"document.getElementById(""cbServer"").checked = false;");
                    }
                    else if (check == "1" && val == "1" && isErr)
                    {
                        JS.Write(@"document.getElementById(""cbOffice"").checked = false;");
                    }
                    else if (check == "0" && val == "3" && isErr)
                    {
                        JS.Write(@"document.getElementById(""cbServer"").checked = true;");
                    }
                    else if (check == "0" && val == "1" && isErr)
                    {
                        JS.Write(@"document.getElementById(""cbOffice"").checked = true;");
                    }

                    JS.Write("document.getElementById('{0}').disabled = false; ", cbid);

                }
                    break;
                default:
                    base.ProcessCommand(cmd, param);
                    break;
            }
        }

        private void ResetCurCheckBox(string name, string val)
        {
            if (name == "1" && val == "1")
                JS.Write(@"document.getElementById(""cbWorkPlace"").checked = false;");
            if (name == "5" && val == "1")
                JS.Write(@"document.getElementById(""cbGuest"").checked = false;");
            if (name == "2" && val == "1")
                JS.Write(@"document.getElementById(""cbHotel"").checked = false;");
            if (name == "4" && val == "1")
                JS.Write(@"document.getElementById(""cbStore"").checked = false;");
            if (name == "3" && val == "1")
                JS.Write(@"document.getElementById(""cbEquipment"").checked = false;");
            if (name == "1" && val == "0")
                JS.Write(@"document.getElementById(""cbWorkPlace"").checked = true;");
            if (name == "5" && val == "0")
                JS.Write(@"document.getElementById(""cbGuest"").checked = true;");
            if (name == "2" && val == "0")
                JS.Write(@"document.getElementById(""cbHotel"").checked = true;");
            if (name == "4" && val == "0")
                JS.Write(@"document.getElementById(""cbStore"").checked = true;");
            if (name == "3" && val == "0")
                JS.Write(@"document.getElementById(""cbEquipment"").checked = true;");
        }

        private void ResetCheckBox(string name)
        {
            if (name != "1")
                JS.Write(@"document.getElementById(""cbWorkPlace"").checked = false;");
            if (name != "5")
                JS.Write(@"document.getElementById(""cbGuest"").checked = false;");
            if (name != "2")
                JS.Write(@"document.getElementById(""cbHotel"").checked = false;");
            if (name != "4")
                JS.Write(@"document.getElementById(""cbStore"").checked = false;");
            if (name != "3")
                JS.Write(@"document.getElementById(""cbEquipment"").checked = false;");
        }

        /// <summary>
        ///     Загрузка данных в соотвтетствии с выбранным значением в дереве
        /// </summary>
        /// <param name="id">Идентификатор выбранного значения в дереве</param>
        private void LoadDataLocation(int id)
        {
            if (id == 0)
            {
                //JS.Write("$('#tabs').hide();");
                JS.Write("$('#divLocationInfo').html('');");
                JS.Write("$('#divSpecifications').html('');");
                JS.Write("$('#divPrinter').html('');");
                gridSockets.ClearGridData();
                gridEquipment.ClearGridData();
                gridPersonLocations.ClearGridData();
                return;
            }

            _location = new Location(id.ToString());
            JS.Write("$('#divLocationInfo').html('{0}');",
                HttpUtility.JavaScriptStringEncode("[" + _location.Id + "] " + _location.NamePath1));
            RenderLocationSpecifications();
            RenderNearPrinter();
            JS.Write("$('#btnSocketAdd').show();");
            SetSocketGridDataSource();
            SetEmployeeGridDataSource();
            SetEqupmentGridDataSource();
            SetPersonLocationsGridDataSource();
            //JS.Write("$('#tabs').show();");
        }

        /// <summary>
        ///     Отрисовка закладки "Характеристики"
        /// </summary>
        private void RenderLocationSpecifications()
        {
            var w = new StringWriter();
            var tempParent = string.IsNullOrEmpty(_location.Parent) ? -1 : int.Parse(_location.Parent);

            var spec = new Dictionary<int, List<string>>
            {
                {1, new List<string> {"cbWorkPlace", "TypeWorkPlaceChanged", "1", Resx.GetString("Inv_lblWorkplace"), _location.WorkPlace == 1 ? "checked" : ""}},
                {2, new List<string> {"cbGuest", "TypeWorkPlaceChanged", "5", Resx.GetString("Inv_lblGuestWorkplaceShiftWork"), _location.WorkPlace == 5 ? "checked" : ""}},
                {3, new List<string> {"cbHotel", "TypeWorkPlaceChanged", "2", Resx.GetString("Inv_lblHotelRoom"), _location.WorkPlace == 2 ? "checked" : ""}},
                {4, new List<string> {"cbStore", "TypeWorkPlaceChanged", "4", Resx.GetString("Inv_lblEquipmentWarehouse"), _location.WorkPlace == 4 ? "checked" : ""}},
                {5, new List<string> {"cbEquipment", "TypeWorkPlaceChanged", "3", Resx.GetString("Inv_lblEquipmentLocated"), _location.WorkPlace == 3 ? "checked" : ""}},
                {6, new List<string> {"cbOffice", "OfficeChanged", "1", Resx.GetString("Inv_lblOffice"), _location.Office == 1 ? "checked" : ""}},
                {7, new List<string> {"cbServer", "OfficeChanged", "3", Resx.GetString("Inv_lblLocationServer"), _location.Office == 3 ? "checked" : ""}}
            };

            foreach (var sp in spec)
            {
                w.Write(
                    "<div class='predicate_block'>" +
                    "<div class='inline_predicate_block_text'><input id=\"{0}\" TabIndex={1} {2} type=checkbox " +
                    "onclick=\"this.disabled=true; cmd('cmd', '{3}', 'id', {4}, 'val', {5}, 'check', this.checked ? 1 : 0, 'parent', {6}, 'l', {7}, 'name', '{8}', 'cbid', this.id);\" " +
                    "/></div><div class='inline_predicate_block_text'>&nbsp;<label for=\"{0}\">{9}</label></div></div>",
                    sp.Value[0], sp.Key, sp.Value[4], sp.Value[1], _location.Id, sp.Value[2], tempParent, _location.L, _location.Name, sp.Value[3]);
            }

            JS.Write("$('#divSpecifications').html('{0}');", HttpUtility.JavaScriptStringEncode(w.ToString()));
        }

        /// <summary>
        ///     Отрисовка названий близлежащих принтеров
        /// </summary>
        private void RenderNearPrinter()
        {
            var w = new StringWriter();
            var sqlParams = new Dictionary<string, object>
            {
                {"@L", _location.L},
                {"@R", _location.R}
            };

            var nearPrinter = DBManager.ExecuteScalar(SQLQueries.SELECT_БлижайшийПринтер, CommandType.Text,
                Config.DS_user, sqlParams);

            if (nearPrinter != null && nearPrinter != DBNull.Value && !string.IsNullOrEmpty(nearPrinter.ToString()))
                w.Write(Resx.GetString("Inv_lblNearestPrinter") + ": " + nearPrinter);
            else
                w.Write("");

            JS.Write("$('#divPrinter').html('{0}');", HttpUtility.JavaScriptStringEncode(w.ToString()));
        }

        /// <summary>
        ///     Отрисовка таблицы розеток
        /// </summary>
        private void SetSocketGridDataSource()
        {
            var sqlParams = new Dictionary<string, object> {{"@КодРасположения", _location.Id}};
            var listColumnVisible = new List<string>
            {
                "КодРозетки",
                "ПодключеноТелефонныйНомер",
                "ПодключеноЛокальнаяСеть",
                "ПодключеноТелефонныйНомер",
                "КодТелефонногоНомера",
                "КодОборудования",
                "Изменил"
            };
            var listBitColumn = new List<string>
            {
                "Работает"
            };

            var listTextAlignCenter = new List<string>
            {
                "Работает"
            };

            gridSockets.ShowGroupPanel = false;
            gridSockets.ExistServiceColumn = true;
            gridSockets.ExistServiceColumnDetail = false;
            gridSockets.RowsPerPage = 100;
            gridSockets.ShowPageBar = false;
            gridSockets.EmptyDataString = Resx.GetString("Inv_msgNoSocketsAtLocation");
            gridSockets.SetDataSource(SQLQueries.SELECT_РозеткиВРасположении, Config.DS_user, CommandType.Text,
                sqlParams);

            gridSockets.Settings.SetColumnDisplayVisible(listColumnVisible, false);
            gridSockets.Settings.SetColumnNoWrapText("ИзменилФИО");
            gridSockets.Settings.SetColumnHeaderAlias("ИзменилФИО", Resx.GetString("lblChangedBy"));
            gridSockets.Settings.SetColumnHrefEmployee("ИзменилФИО", "Изменил");

            gridSockets.Settings.SetColumnNoWrapText("Изменено");
            gridSockets.Settings.SetColumnLocalTime("Изменено");
            gridSockets.Settings.SetColumnHeaderAlias("Изменено", Resx.GetString("lblChanged"));

            gridSockets.Settings.SetColumnBitFormat(listBitColumn);
            gridSockets.Settings.SetColumnTextAlign(listTextAlignCenter, "center");

            if (tvLocation.ReturnType == "socket")
            {
                gridSockets.ExistServiceColumnReturn = true;
                gridSockets.SetServiceColumnReturn("_return", new List<string> {"КодРозетки", "КодРозетки"},
                    Resx.GetString("ppBtnChoose"));
                var condition = new List<object> {""};

                if (!gridSockets.RenderConditionServiceColumnReturn.ContainsKey("Подключено"))
                    gridSockets.RenderConditionServiceColumnReturn.Add("Подключено", condition);
            }

            gridSockets.Settings.SetColumnHrefByClause("Подключено",
                new Dictionary<string, Dictionary<string, string>>
                {
                    {
                        "КодТелефонногоНомера",
                        new Dictionary<string, string> {{"ПодключеноТелефонныйНомер", Config.tel_form}}
                    },
                    {
                        "КодОборудования",
                        new Dictionary<string, string> {{"ПодключеноЛокальнаяСеть", Config.equipment_form}}
                    }
                });

            gridSockets.Settings.SetColumnHeaderAlias("Подключено", Resx.GetString("Inv_lblConnected"));
            gridSockets.Settings.SetColumnHeaderAlias("Работает", Resx.GetString("Inv_lblWorks"));
            gridSockets.Settings.SetColumnHeaderAlias("Розетка", Resx.GetString("Inv_lblSocket"));
            gridSockets.Settings.SetColumnHeaderAlias("Примечание", Resx.GetString("Inv_lblNote"));

            gridSockets.SetServiceColumnDelete("socket_delete", new List<string> {"КодРозетки"},
                new List<string> {"Розетка"}, Resx.GetString("TTN_btnDeletePosition"));
            gridSockets.SetServiceColumnEdit("socket_edit", new List<string> {"КодРозетки"},
                Resx.GetString("TTN_btnEditPosition"));

            gridSockets.RefreshGridData();
        }

        /// <summary>
        ///     Отрисовка таблицы сотрудников
        /// </summary>
        private void SetEmployeeGridDataSource()
        {
            var sqlParams = new Dictionary<string, object> {{"@КодРасположения", _location.Id}};

            var listColumnVisible = new List<string>
            {
                "КодСотрудника"
            };

            gridEmployees.ShowGroupPanel = false;
            gridEmployees.ExistServiceColumn = false;
            gridEmployees.ExistServiceColumnDetail = false;
            gridEmployees.RowsPerPage = 100;
            gridEmployees.ShowPageBar = false;
            gridEmployees.EmptyDataString = Resx.GetString("Inv_msgNoEmployeesAtLocation");
            gridEmployees.SetDataSource(SQLQueries.SELECT_СотрудникиВРасположении, Config.DS_user, CommandType.Text,
                sqlParams);
            gridEmployees.Settings.SetColumnDisplayVisible(listColumnVisible, false);
            gridEmployees.Settings.SetColumnNoWrapText("Сотрудник");
            gridEmployees.Settings.SetColumnHrefEmployee("Сотрудник", "КодСотрудника");
            gridEmployees.Settings.SetColumnHeaderAlias("Сотрудник", Resx.GetString("Inv_lblEmployee"));
            //gridEmployees.Settings.TableColumns.
            gridEmployees.RefreshGridData();
        }

        /// <summary>
        ///     Отрисовка таблицы оборудования
        /// </summary>
        private void SetEqupmentGridDataSource()
        {
            var sqlParams = new Dictionary<string, object> {{"@КодРасположения", _location.Id}};

            var listColumnVisible = new List<string>
            {
                "КодОборудования",
                "КодСотрудника"
            };
            var listBitColumn = new List<string>
            {
                "IT"
            };

            var listTextAlignCenter = new List<string>
            {
                "IT"
            };
            gridEquipment.ExistServiceColumn = false;
            gridEquipment.ExistServiceColumnDetail = false;
            gridEquipment.RowsPerPage = 50;
            gridEquipment.EmptyDataString = Resx.GetString("Inv_msgNoEquipmentAtLocation");

            gridEquipment.SetDataSource(SQLQueries.SELECT_ОборудованиеВРасположении, Config.DS_user, CommandType.Text,
                sqlParams);

            gridEquipment.Settings.SetColumnHeaderAlias("МодельОборудования", Resx.GetString("Inv_lblEquipment"));
            gridEquipment.Settings.SetColumnDisplayVisible(listColumnVisible, false);
            gridEquipment.Settings.SetColumnBitFormat(listBitColumn);
            gridEquipment.Settings.SetColumnTextAlign(listTextAlignCenter, "center");
            gridEquipment.Settings.SetColumnNoWrapText("Сотрудник");
            gridEquipment.Settings.SetColumnHeaderAlias("Сотрудник", Resx.GetString("Inv_lblEmployee"));

            gridEquipment.Settings.SetColumnHref("МодельОборудования", "КодОборудования", Config.equipment_form);
            gridEquipment.Settings.SetColumnHrefEmployee("Сотрудник", "КодСотрудника");

            gridEquipment.RefreshGridData();
        }

        /// <summary>
        ///     Отрисовка таблицы расположения Лиц
        /// </summary>
        private void SetPersonLocationsGridDataSource()
        {
            var sqlParams = new Dictionary<string, object> { { "@КодРасположения", _location.Id } };
/*
            var listColumnVisible = new List<string>
            {
                "КодСотрудника"
            };
*/
            gridPersonLocations.ShowGroupPanel = false;
            gridPersonLocations.ExistServiceColumn = false;
            gridPersonLocations.ExistServiceColumnDetail = false;
            gridPersonLocations.RowsPerPage = 100;
            gridPersonLocations.ShowPageBar = false;
            //gridPersonLocations.EmptyDataString = Resx.GetString("Inv_msgNoEmployeesAtLocation");
            gridPersonLocations.SetDataSource(SQLQueries.SELECT_РасположенияЛицВРасположении, Config.DS_user, CommandType.Text, sqlParams);
            //gridPersonLocations.Settings.SetColumnDisplayVisible(listColumnVisible, false);
            gridPersonLocations.Settings.SetColumnNoWrapText("Компания");
            gridPersonLocations.Settings.SetColumnHeaderAlias("Компания", Resx.GetString("Inv_lblCompany"));

            //gridPersonLocations.Settings.SetColumnHrefEmployee("Сотрудник", "КодСотрудника");
            gridPersonLocations.RefreshGridData();
            gridPersonLocations.Visible = gridPersonLocations.GеtRowCount() > 0;
        }

        /// <summary>
        ///     Отрисовка количества сотрудников в расположении
        /// </summary>
        private void LoadUserCount(int id)
        {
            var userList = string.Empty;
            var sqlParams = new Dictionary<string, object> {{"@КодРасположения", id}};
            var dt = DBManager.GetData(SQLQueries.SELECT_РаботающиеСотрудникиВРасположении, Config.DS_user,
                CommandType.Text, sqlParams);

            for (var i = 0; i < dt.Rows.Count; i++) userList += dt.Rows[i]["Сотрудник"] + "\\r\\n";

            if (userList.Length > 0) JS.Write("$('#img_{0}').attr('title','{1}');", id, userList);
        }

        /// <summary>
        ///     Сохранение расположений
        /// </summary>
        private void SaveData(string type)
        {
            try
            {
                if (type == "AddLocation")
                {
                    var sqlParams = new Dictionary<string, object>
                {
                    {"@Расположение", tbNode.Value},
                    {"@РабочееМесто", 0},
                    {"@Parent", LocationId}
                };
                    var locationId = DBManager.ExecuteScalar(SQLQueries.INSERT_Расположения, CommandType.Text,
                        Config.DS_user, sqlParams);
                }
                else
                {
                    var sqlParams = new Dictionary<string, object>
                {
                    {"@КодРасположения", LocationId},
                    {"@Расположение", tbNode.Value}
                };
                    DBManager.ExecuteNonQuery(SQLQueries.UPDATE_Расположения, CommandType.Text, Config.DS_user, sqlParams);
                    LoadDataLocation(LocationId);
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message, Resx.GetString("alertError"));
            }

            JS.Write("Wait.render(false);");
            JS.Write("v4_closeLocationForm();");
            JS.Write("v4_reloadNode('tvLocation');");
        }

        /// <summary>
        ///     Сохранение розеток
        /// </summary>
        private void SaveSocketData()
        {
            if (SocketId == 0)
            {
                var sqlParams = new Dictionary<string, object>
                {
                    {"@КодРасположения", _location.Id},
                    {"@Розетка", tbSocket.Value},
                    {"@Работает", cbWork.Checked},
                    {"@Примечание", tbNote.Value}
                };
                DBManager.ExecuteNonQuery(SQLQueries.INSERT_Розетки, CommandType.Text, Config.DS_user, sqlParams);
            }
            else
            {
                var sqlParams = new Dictionary<string, object>
                {
                    {"@КодРозетки", SocketId},
                    {"@КодРасположения", _location.Id},
                    {"@Розетка", tbSocket.Value},
                    {"@Работает", cbWork.Checked},
                    {"@Примечание", tbNote.Value}
                };
                DBManager.ExecuteNonQuery(SQLQueries.Update_Розетки, CommandType.Text, Config.DS_user, sqlParams);
            }

            JS.Write("v4_closeSocketForm();");
            LoadDataLocation(Convert.ToInt32(_location.Id));
        }

        /// <summary>
        ///     Проверка корректности вводимых полей расположения
        /// </summary>
        protected bool ValidateLocation(out List<string> errors)
        {
            errors = new List<string>();

            if (tbNode.Value.IsNullEmptyOrZero())
                errors.Add(Resx.GetString("Inv_msgNoLocationName"));

            return errors.Count <= 0;
        }

        /// <summary>
        ///     Проверка корректности вводимых полей розетки
        /// </summary>
        protected bool ValidateSocket(out List<string> errors)
        {
            errors = new List<string>();

            if (tbSocket.Value.IsNullEmptyOrZero())
                errors.Add(Resx.GetString("Inv_msgNoSocketName"));

            return errors.Count <= 0;
        }

        /// <summary>
        ///     Сформировать сообщение об ошибках
        /// </summary>
        public void RenderErrors(List<string> li, string text = null)
        {
            using (var w = new StringWriter())
            {
                foreach (var l in li)
                    w.Write("<div style='white-space: nowrap;'>{0}</div>", l);

                ShowMessage(w + text, Resx.GetString("errIncorrectlyFilledField"), MessageStatus.Error, "", 500, 200);
            }
        }
    }
}