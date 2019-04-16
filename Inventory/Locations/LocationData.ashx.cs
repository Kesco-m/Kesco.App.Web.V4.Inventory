using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Controls.V4.TreeView;
using Kesco.Lib.Web.Settings;
using Kesco.Lib.Web.Settings.Parameters;
using System.Collections.Specialized;
using System.Web.Script.Serialization;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.BaseExtention.Enums.Docs;
using Kesco.Lib.Localization;
using SQLQueries = Kesco.Lib.Entities.SQLQueries;

namespace Kesco.App.Web.Inventory
{
    /// <summary>
    /// Сводное описание для LocationData
    /// </summary>
    public sealed class LocationData : TreeViewDataHandler
    {
        /// <summary>
        ///     Обработчик запроса к странице
        /// </summary>
        /// <param name="context"></param>
        public override void ProcessRequest(HttpContext context)
        {
            DbSourceSettings = new TreeViewDbSourceSettings
            {
                TableName = "Расположения",
                ViewName = "vwРасположения",
                ConnectionString = Config.DS_user,
                PkField = "КодРасположения",
                NameField = "Расположение",
                PathField = "РасположениеPath1",
                RootName = "Расположения"
            };

            var type = context.Request.QueryString["type"];
            if (type == "get_state")
            {
                var clid = context.Request.QueryString["Clid"];
                var paramName = context.Request.QueryString["ParamName"];
                var parametersManager = new AppParamsManager(Convert.ToInt32(clid), new StringCollection { paramName });
                var appParam = parametersManager.Params.Find(p => p.Name == paramName);
                
                if (null != appParam)
                {
                    context.Response.Write(appParam.Value);
                }
            }
            else if (type == "save_state")
            {
                var clid = context.Request.QueryString["Clid"];
                var paramName = context.Request.QueryString["ParamName"];
                var state = context.Request.Form["state"];
                var parametersManager = new AppParamsManager(Convert.ToInt32(clid), new StringCollection());
                parametersManager.Params.Add(new AppParameter(paramName, state, AppParamType.SavedWithClid));
                parametersManager.SaveParams();
            }
            else if (type == "create_state")
            {
                var loadById = context.Request.QueryString["loadid"];
                var sqlParams = new Dictionary<string, object> { { "@id", loadById } };
                var dt = DBManager.GetData(SQLQueries.SELECT_Родители, Config.DS_user, CommandType.Text, sqlParams);
                var openStrNodeList = "";

                foreach (DataRow row in dt.Rows)
                {
                    if (openStrNodeList != "") openStrNodeList += ",";
                    openStrNodeList += "\"" + row["КодРасположения"] + "\"";
                }

                var state = "{\"core\":{\"open\":[" + openStrNodeList +
                            "],\"loaded\":[],\"selected\":[\"" + loadById +
                            "\"]}}";

                context.Response.Write(state);
            }
            else
            {
                base.ProcessRequest(context);
            }
        }

        protected override string GetTreedata_Sql(string orderBy = "L", string searchText = "", string searchParam = "", string openList = "")
        {
            if (orderBy != "L") orderBy = DbSourceSettings.NameField;

            if (searchText.IsNullEmptyOrZero())
            {
                if (openList.IsNullEmptyOrZero())
                    return string.Format(SQLQueries.SELECT_РасположенияДанныеДляДерева, orderBy);
                return string.Format(SQLQueries.SELECT_РасположенияДанныеДляДерева_State, orderBy, openList);
            }

            searchText = searchParam == "1" ? (searchText + "%") : ("%" + searchText + "%");
            return string.Format(SQLQueries.SELECT_РасположенияДанныеДляДерева_Фильтр, orderBy, searchText);
        }

        /// <summary>
        /// Вывод иконок до названия
        /// </summary>
        /// <param name="dt">DataRow</param>
        /// <returns>возвращаямая строка, содержащая готовую разметку</returns>  
        protected override string GetPrefixIcon(DataRow dt)
        {
            var iconsString = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(ReturnId) && ReturnId != "2" && ReturnType != "socket")
                iconsString.Append(
                    String.Format(
                        "<img style='border:none' src='/styles/BackToList.gif' title='Выбрать значение' onclick='v4_returnValue({0},\"{1}\");'>&nbsp;",
                        dt["id"], dt["Text"]));

            if (dt["office"].ToString().Equals("1"))
                iconsString.Append(String.Format("<img style='border:none' src='/styles/House.gif' title='{0}'>&nbsp;", Resources.Resx.GetString("Inv_lblOffice")));

            switch (dt["workPlace"].ToString())
            {
                case "1":
                    if (dt["ЕстьОборудование"].ToString().Equals("0"))
                        iconsString.Append(String.Format("<img style='border:none' src='/styles/NotebookGrayed.gif' title='{0}'>&nbsp;", Resources.Resx.GetString("Inv_lblUnorganizedComputerizedWorkplace")));
                    else
                        iconsString.Append(String.Format("<img style='border:none' src='/styles/Notebook.gif' title='{0}'>&nbsp;", Resources.Resx.GetString("Inv_ComputerizedWorkplace")));
                    break;
                case "2":
                    iconsString.Append(String.Format("<img style='border:none' src='/styles/bed.gif' title='{0}'>&nbsp;", Resources.Resx.GetString("Inv_lblHotelRoom")));
                    break;
                case "3":
                    iconsString.Append(String.Format("<img style='border:none' src='/styles/service.gif' title='{0}'>&nbsp;", Resources.Resx.GetString("Inv_lblEquipment")));
                    break;
                case "4":
                    iconsString.Append(String.Format("<img style='border:none' src='/styles/Store.gif' title='{0}'>&nbsp;", Resources.Resx.GetString("Inv_lblEquipmentWarehouse")));
                    break;
                case "5":
                    iconsString.Append(String.Format("<img style='border:none' src='/styles/Chat.png' title='{0}'>&nbsp;", Resources.Resx.GetString("Inv_lblGuestWorkplace")));
                    break;
                default:
                    iconsString.Append(String.Format("<img style='border:none' src='/styles/Empty.gif' title=''>&nbsp;"));
                    break;
            }

            return iconsString.ToString();
        }

        /// <summary>
        /// Вывод иконок до названия
        /// </summary>
        /// <param name="dt">DataRowView</param>
        /// <returns>возвращаямая строка, содержащая готовую разметку</returns>  
        protected override string GetPrefixIcon(DataRowView dt)
        {
            return GetPrefixIcon(dt.Row);
        }

        /// <summary>
        /// Вывод иконок после названия
        /// </summary>
        /// <param name="dt">DataRow</param>
        /// <returns>возвращаямая строка, содержащая готовую разметку</returns>    
        protected override string GetPostFixIcon(DataRow dt)
        {
            var iconsString = new StringBuilder();

            if (dt["ЕстьСотрудники"].ToString().Equals("1"))
                iconsString.Append(
                    String.Format("&nbsp;<img id='img_{0}' src='/styles/user.gif' onmouseover='ShowUsers({0})'>", dt["id"]));
            else if (!dt["ЕстьСотрудники"].ToString().Equals("0"))
                iconsString.Append(
                    String.Format("&nbsp;<img id='img_{0}' src='/styles/users.gif' onmouseover='ShowUsers({0})'>", dt["id"]));

            return iconsString.ToString();
        }

        /// <summary>
        /// Вывод иконок после названия
        /// </summary>
        /// <param name="dt">DataRowView</param>
        /// <returns>возвращаямая строка, содержащая готовую разметку</returns>
        protected override string GetPostFixIcon(DataRowView dt)
        {
            return GetPostFixIcon(dt.Row);
        }

    }

    /// <summary>
    /// Класс состояния дерева
    /// </summary>
    internal class TreeViewState
    {
        internal List<Core> core { get; set; }
    }

    /// <summary>
    /// Сущности состояния
    /// </summary>
    public class Core
    {
        public List<string> open;
        public List<string> loaded;
        public List<Scroll> scroll;
        public List<string> selected;
    }

    /// <summary>
    /// Позиция дерева
    /// </summary>
    public class Scroll
    {
        public int left;
        public int top;
        public List<string> scroll;
    }
}