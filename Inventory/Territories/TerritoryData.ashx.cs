using Kesco.Lib.BaseExtention;
using Kesco.Lib.BaseExtention.Enums.Docs;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Controls.V4.TreeView;
using Kesco.Lib.Web.Settings;
using Kesco.Lib.Web.Settings.Parameters;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using System.Web;
using SQLQueries = Kesco.Lib.Entities.SQLQueries;

namespace Kesco.App.Web.Inventory.Territories
{
    /// <summary>
    /// Сводное описание для TerritoryData
    /// </summary>
    public sealed class TerritoryData : TreeViewDataHandler
    {
        public override void ProcessRequest(HttpContext context)
        {
            DbSourceSettings = new TreeViewDbSourceSettings
            {
                TableName = "Территории",
                ViewName = "vwТерритории",
                ConnectionString = Config.DS_user,
                PkField = "КодТерритории",
                NameField = "Территория",
                PathField = "",
                RootName = "Территории"
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
                    openStrNodeList += "\"" + row["КодТерритории"] + "\"";
                }

                var state = "{\"core\":{\"open\":[" + openStrNodeList +
                            "],\"loaded\":[],\"scroll\":{\"left\":0,\"top\":0},\"selected\":[\"" + loadById +
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
                    return string.Format(SQLQueries.SELECT_ТерриторииДанныеДляДерева, orderBy);
                return string.Format(SQLQueries.SELECT_ТерриторииДанныеДляДерева_State, orderBy, openList);
            }

            searchText = searchParam == "1" ? (searchText + "%") : ("%" + searchText + "%");
            return string.Format(SQLQueries.SELECT_ТерриторииДанныеДляДереваФильтр, orderBy, searchText);
        }

        protected override string GetPrefixIcon(DataRow dt)
        {
            var iconsString = new StringBuilder();

            if (ReturnId == "1")
                iconsString.Append(
                    String.Format(
                        "<img style='border:none' src='/styles/BackToList.gif' title='Выбрать значение' onclick=\"v4_returnValue({0},'{1}');\">&nbsp;",
                        dt["id"], dt["text"]));

            return iconsString.ToString();
        }
        protected override string GetPrefixIcon(DataRowView dt)
        {
            return GetPrefixIcon(dt.Row);
        }
    }

    internal class TreeViewState
    {
        internal List<Core> core { get; set; }
    }

    public class Core
    {
        public List<string> open;
        public List<string> loaded;
        public List<Scroll> scroll;
        public List<string> selected;
    }

    public class Scroll
    {
        public int left;
        public int top;
        public List<string> scroll;
    }
}