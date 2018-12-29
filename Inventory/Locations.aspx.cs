using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Web;
using Kesco.Lib.DALC;
using Kesco.Lib.Entities;
using Kesco.Lib.Web.Controls.V4.Common;
using Kesco.Lib.Web.Settings;
using Menu = Kesco.Lib.Web.Controls.V4.Menu;

namespace Kesco.App.Web.Inventory
{
    public partial class Locations : EntityPage
    {
        private Location _location;
        protected override string HelpUrl { get; set; }
        protected int LocationId;

        protected void Page_Load(object sender, EventArgs e)
        {
            btnSocketAdd.OnClick = string.Format("cmd('cmd','SocketAdd');");
            btnSocketAdd.Text = "Добавить розетку" + "&nbsp;(Ins)";
        }
        
        protected override void ProcessCommand(string cmd, NameValueCollection param)
        {
            switch (cmd)
            {
                case "LoadDataLocation":
                    LoadDataLocation(int.Parse(param["Id"]));
                    RestoreCursor();
                    break;
                case "SocketDelete":
                    ShowMessage("SocketDelete");
                    break;
                case "SocketAdd":
                    ShowMessage("SocketAdd");
                    break;
                case "RenderUserCount":
                    LoadUserCount(int.Parse(param["Id"]));
                    break;
                case "AddLocation":
                case "EditLocation":
                    LocationId = int.Parse(param["Id"]);
                    var sqlParams = new Dictionary<string, object> { { "@id", LocationId } };
                    var dt = DBManager.GetData(SQLQueries.SELECT_РасположениеПоID, Config.DS_user, CommandType.Text, sqlParams);
                    if (dt.Rows.Count > 0)
                    {
                        tbNode.Value = cmd=="AddLocation" ? "" : dt.Rows[0]["Расположение"].ToString();
                        divLocationPatch.Value = dt.Rows[0]["РасположениеPath1"].ToString().Replace("_", " "); ;
                        JS.Write("locationEdit_dialogShow('{0}','{1}','{2}','{3}');", cmd == "AddLocation" ? "Добавление" : "Редактирование" + " расположения", "Сохранить", "Отмена", cmd);
                    }
                    break;
                case "SetLocation":
                    SaveData(param["type"]);
                    break;
                default:
                    base.ProcessCommand(cmd, param);
                    break;
            }
        }

        private void LoadDataLocation(int id)
        {
            if (id == 0)
            {
                JS.Write("$('#tabs').hide();");
                JS.Write("$('#divLocationInfo').html('');");
                JS.Write("$('#divSpecifications').html('');");
                JS.Write("$('#divPrinter').html('');");
                gridSockets.ClearGridData();
                gridEquipment.ClearGridData();
                return;
            }

            _location = new Location(id.ToString());
            JS.Write("$('#divLocationInfo').html('{0}');",
                HttpUtility.JavaScriptStringEncode("[" + _location.Id + "] " + _location.NamePath1));
            RenderLocationSpecifications();
            RenderNearPrinter();
            SetSocketGridDataSource();
            SetEmployeeGridDataSource();
            SetEqupmentGridDataSource();
            JS.Write("$('#tabs').show();");
        }

        private void RenderLocationSpecifications()
        {
            var w = new StringWriter();
            var tempParent = (string.IsNullOrEmpty(_location.Parent) ? -1 : int.Parse(_location.Parent));

            w.Write(
                "<div><input id=\"cbWorkPlace\" TabIndex={0} {2} type=checkbox onclick=\"doSrvCmd    ('TypeWorkPlaceChanged', {1}, 1, this.checked ? 1 : 0, {3}, {4}, '{5}');\" />&nbsp;<label for=\"cbWorkPlace\">Рабочее место</label></div>",
                1, _location.Id, _location.WorkPlace == 1 ? "checked" : "", tempParent, _location.L, _location.Name);
            w.Write(
                "<div class=\"marginT\"><input id=\"cbGuest\" TabIndex={0} {2} type=checkbox onclick=\"doSrvCmd('TypeWorkPlaceChanged', {1}, 5, this.checked ? 1 : 0, {3}, {4}, '{5}');\" />&nbsp;<label for=\"cbGuest\">Гостевое рабочее место, посменная работа</label></div>",
                2, _location.Id, _location.WorkPlace == 5 ? "checked" : "", tempParent, _location.L, _location.Name);
            w.Write(
                "<div class=\"marginT\"><input id=\"cbHotel\" TabIndex={0} {2} type=checkbox onclick=\"doSrvCmd('TypeWorkPlaceChanged', {1}, 2, this.checked ? 1 : 0, {3}, {4}, '{5}');\" />&nbsp;<label for=\"cbHotel\">Номер гостинницы</label></div>",
                3, _location.Id, _location.WorkPlace == 2 ? "checked" : "", tempParent, _location.L, _location.Name);
            w.Write(
                "<div class=\"marginT\"><input id=\"cbStore\" TabIndex={0} {2} type=checkbox onclick=\"doSrvCmd('TypeWorkPlaceChanged', {1}, 4, this.checked ? 1 : 0, {3}, {4}, '{5}');\" />&nbsp;<label for=\"cbStore\">Склад оборудования</label></div>",
                4, _location.Id, _location.WorkPlace == 4 ? "checked" : "", tempParent, _location.L, _location.Name);
            w.Write(
                "<div class=\"marginT\"><input id=\"cbEquipment\" TabIndex={0} {2} type=checkbox onclick=\"doSrvCmd('TypeWorkPlaceChanged', {1}, 3, this.checked ? 1 : 0, {3}, {4}, '{5}');\" />&nbsp;<label for=\"cbEquipment\">В расположении может находиться оборудование</label></div>",
                5, _location.Id, _location.WorkPlace == 3 ? "checked" : "", tempParent, _location.L, _location.Name);
            w.Write(
                "<div class=\"marginT\"><input id=\"cbOffice\" TabIndex={0} {2} type=checkbox onclick=\"doSrvCmd('OfficeChanged', {1}, 1, this.checked ? 1 : 0);\" />&nbsp;<label for=\"cbOffice\">Офис</label></div>",
                6, _location.Id, _location.Office == 1 ? "checked" : "");
            w.Write(
                "<div class=\"marginT\"><input id=\"cbServer\" TabIndex={0} {2} type=checkbox onclick=\"doSrvCmd('OfficeChanged', {1}, 3, this.checked ? 1 : 0);\" />&nbsp;<label for=\"cbServer\">Расположение с отдельной серверной</label></div>",
                7, _location.Id, _location.Office == 3 ? "checked" : "");

            JS.Write("$('#divSpecifications').html('{0}');", HttpUtility.JavaScriptStringEncode(w.ToString()));
        }

        private void RenderNearPrinter()
        {
            var w =new StringWriter();
            var sqlParams = new Dictionary<string, object>
            {
                {"@L", _location.L},
                {"@R", _location.R}
            };

            var nearPrinter = DBManager.ExecuteScalar(SQLQueries.SELECT_БлижайшийПринтер, CommandType.Text, Config.DS_user, sqlParams);

            if (nearPrinter != null && nearPrinter != DBNull.Value && !String.IsNullOrEmpty(nearPrinter.ToString()))
                w.Write("Ближайший принтер: " + nearPrinter);
            else
                w.Write("");

            JS.Write("$('#divPrinter').html('{0}');", HttpUtility.JavaScriptStringEncode(w.ToString()));
        }

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
                "ИзменилФИО",
                "Изменил",
                "Изменено"
            };
            var listBitColumn = new List<string>
            {
                "Работает"
            };

            var listTextAlignCenter = new List<string>
            {
                "Работает"
            };

            gridSockets.ExistServiceColumn = true;
            gridSockets.ExistServiceColumnDetail = false;
            gridSockets.EmptyDataString = "На данном расположении нет розеток";

            gridSockets.SetDataSource(SQLQueries.SELECT_РозеткиВРасположении, Config.DS_user, CommandType.Text,
                sqlParams);
            gridSockets.Settings.SetColumnDisplayVisible(listColumnVisible, false);
            gridSockets.Settings.SetColumnBitFormat(listBitColumn);
            gridSockets.Settings.SetColumnTextAlign(listTextAlignCenter, "center");

            gridSockets.Settings.SetColumnHrefByClause("Подключено",
                new Dictionary<string, Dictionary<string, string>>
                {
                    {"КодТелефонногоНомера", new Dictionary<string, string> {{"ПодключеноТелефонныйНомер", Config.tel_form}}},
                    {"КодОборудования", new Dictionary<string, string> {{"ПодключеноЛокальнаяСеть", Config.equipment_form}}}
                });

            gridSockets.SetServiceColumnDelete("socket_delete", new List<string> {"КодРозетки"},
                new List<string> {"Розетка"}, Resx.GetString("TTN_btnDeletePosition"));
            gridSockets.SetServiceColumnEdit("socket_edit", new List<string> {"КодРозетки"},
                Resx.GetString("TTN_btnEditPosition"));


            gridSockets.RefreshGridData();
        }

        private void SetEmployeeGridDataSource()
        {
            var sqlParams = new Dictionary<string, object> { { "@КодРасположения", _location.Id } };

            var listColumnVisible = new List<string>
            {
                "КодСотрудника"
            };

            gridEmployees.ExistServiceColumn = false;
            gridEmployees.ExistServiceColumnDetail = false;
            gridEmployees.EmptyDataString = "На данном расположении нет сотрудников";

            gridEmployees.SetDataSource(SQLQueries.SELECT_СотрудникиВРасположении, Config.DS_user, CommandType.Text, sqlParams);
            
            gridEmployees.Settings.SetColumnDisplayVisible(listColumnVisible, false);
            gridEmployees.Settings.SetColumnNoWrapText("Сотрудник");
            gridEmployees.Settings.SetColumnHrefEmployee("Сотрудник", "КодСотрудника");

            gridEmployees.RefreshGridData();

        }

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
            gridEquipment.EmptyDataString = "На данном расположении нет оборудования";

            gridEquipment.SetDataSource(SQLQueries.SELECT_ОборудованиеВРасположении, Config.DS_user, CommandType.Text,
                sqlParams);

            gridEquipment.Settings.SetColumnHeaderAlias("МодельОборудования", "Оборудование");
            gridEquipment.Settings.SetColumnDisplayVisible(listColumnVisible, false);
            gridEquipment.Settings.SetColumnBitFormat(listBitColumn);
            gridEquipment.Settings.SetColumnTextAlign(listTextAlignCenter, "center");
            gridEquipment.Settings.SetColumnNoWrapText("Сотрудник");

            gridEquipment.Settings.SetColumnHref("МодельОборудования", "КодОборудования", Config.equipment_form);
            gridEquipment.Settings.SetColumnHrefEmployee("Сотрудник", "КодСотрудника");

            gridEquipment.RefreshGridData();
        }

        private void LoadUserCount(int id)
        {
            var userList = string.Empty;
            var sqlParams = new Dictionary<string, object> {{"@КодРасположения", id}};
            var dt = DBManager.GetData(SQLQueries.SELECT_РаботающиеСотрудникиВРасположении, Config.DS_user, CommandType.Text, sqlParams);

            for (var i = 0; i < dt.Rows.Count; i++)
            {
                userList += dt.Rows[i]["Сотрудник"] + "\\r\\n";
            }

            if (userList.Length > 0)
            {
                JS.Write("$('#img_{0}').attr('title','{1}');", id, userList);
            }
        }

        private void SaveData(string type)
        {
            if (type == "AddLocation")
            {
                var sqlParams = new Dictionary<string, object>
                {
                    {"@Расположение", tbNode.Value},
                    {"@РабочееМесто", 0},
                    {"@Parent", LocationId}
                };
                DBManager.ExecuteNonQuery(SQLQueries.INSERT_Расположения, CommandType.Text, Config.DS_user, sqlParams);
            }
            else
            {
                var sqlParams = new Dictionary<string, object>
                {
                    {"@КодРасположения", LocationId},
                    {"@Расположение", tbNode.Value}
                };
                DBManager.ExecuteNonQuery(SQLQueries.UPDATE_Расположения, CommandType.Text, Config.DS_user, sqlParams);                
            }
            JS.Write("v4_closeLocationForm();");
            LoadDataLocation(LocationId);

            //JS.Write("TreeReload();");
            //JS.Write("$('#divMyTree').jstree(true).refresh(true);");
            //$('#treeId').jstree(true).settings.core.data = newData;
            //$('#treeId').jstree(true).refresh();

        }
    }
}