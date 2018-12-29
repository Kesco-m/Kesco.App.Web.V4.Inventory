using System;
using System.Data;
using System.Runtime.Remoting.Activation;
using System.Text;
using System.Web;

namespace Kesco.App.Web.Inventory
{
    /// <summary>
    /// Сводное описание для LocationJsonData
    /// </summary>
    public class LocationJsonData : IHttpHandler
    {
        /*
         * http://mohyuddin.blogspot.com/2009/06/binding-jstree-with-database-in-aspnet.html
         * 
       WITH Tbl(КодРасположения, Расположение, level, Parent) AS (
SELECT КодРасположения, Расположение, 0, Parent FROM vwРасположения WHERE Parent IS NULL
UNION ALL SELECT vwРасположения.КодРасположения, vwРасположения.Расположение, level + 1, vwРасположения.Parent 
          FROM vwРасположения INNER JOIN 
		Tbl X ON vwРасположения.Parent = X.КодРасположения)
SELECT * FROM Tbl
         */

        public void ProcessRequest(HttpContext context)
        {
            var id = string.IsNullOrWhiteSpace(context.Request.QueryString["loadid"]) || context.Request.QueryString["loadid"] == "#" ? 0 : int.Parse(context.Request.QueryString["loadid"]);
            ReturnId = string.IsNullOrWhiteSpace(context.Request.QueryString["return"]) ? "" : context.Request.QueryString["return"];
            string json = GetTreedata(id);
            context.Response.ContentType = "text/json";
            context.Response.Write(json);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private static string ReturnId { get; set;}

        private string GetTreedata(int loadid)
        {
            var sql = @"SELECT r.КодРасположения Id, r.R-L ЕстьДети, r.Расположение Text, ISNULL(r.Parent,0) ParentId, r.Офис Office, r.РабочееМесто WorkPlace, ISNULL(x.ЕстьСотрудники,0) ЕстьСотрудники
                        FROM vwРасположения r  
                        LEFT JOIN (SELECT count(РабочиеМеста.КодРасположения) ЕстьСотрудники,РабочиеМеста.КодРасположения
									FROM РабочиеМеста
									INNER JOIN Сотрудники ON РабочиеМеста.КодСотрудника = Сотрудники.КодСотрудника
									WHERE Сотрудники.Состояние=0
									GROUP BY КодРасположения) x ON r.КодРасположения = x.КодРасположения
                        WHERE r.Parent IS NULL
                        AND r.Закрыто=0                        
                        ORDER BY r.L";

            if (loadid != 0)
            {
                sql = @"SELECT r.КодРасположения Id, r.R-L ЕстьДети, r.Расположение Text, ISNULL(r.Parent,0) ParentId, r.Офис Office, r.РабочееМесто WorkPlace, ISNULL(x.ЕстьСотрудники,0) ЕстьСотрудники
                        FROM vwРасположения r 
                        LEFT JOIN (SELECT count(РабочиеМеста.КодРасположения) ЕстьСотрудники,РабочиеМеста.КодРасположения
									FROM РабочиеМеста
									INNER JOIN Сотрудники ON РабочиеМеста.КодСотрудника = Сотрудники.КодСотрудника
									WHERE Сотрудники.Состояние=0
									GROUP BY КодРасположения) x ON r.КодРасположения = x.КодРасположения
                       WHERE r.Parent =" + loadid + " " + @"
                       AND r.Закрыто=0                        
                       ORDER BY r.L";
            }

            var dt = Kesco.Lib.DALC.DBManager.GetData(sql, Kesco.Lib.Web.Settings.Config.DS_user);

            Node root = null;
            if (loadid == 0)
                root = new Node { id = "0", children = { }, text = "Расположения", state = new NodeState { opened = true, selected = false, loaded = true } };
            else
            {
                sql = @"SELECT TOP 1 r.КодРасположения Id, r.R-L ЕстьДети, r.Расположение Text, ISNULL(r.Parent,0) ParentId, r.Офис Office, r.РабочееМесто WorkPlace, ISNULL(x.ЕстьСотрудники,0) ЕстьСотрудники
                        FROM vwРасположения r 
                        LEFT JOIN (SELECT count(РабочиеМеста.КодРасположения) ЕстьСотрудники,РабочиеМеста.КодРасположения
									FROM РабочиеМеста
									INNER JOIN Сотрудники ON РабочиеМеста.КодСотрудника = Сотрудники.КодСотрудника
									WHERE Сотрудники.Состояние=0
									GROUP BY КодРасположения) x ON r.КодРасположения = x.КодРасположения
                       WHERE r.КодРасположения=" + loadid;
                var rootNode = Kesco.Lib.DALC.DBManager.GetData(sql, Kesco.Lib.Web.Settings.Config.DS_user).Rows[0];
                root = new Node
                {
                    id = loadid.ToString(),
                    text = GetIconByOffice(loadid.ToString(), rootNode["Office"].ToString(), rootNode["WorkPlace"].ToString()) + rootNode["text"] + GetUserIcon(loadid.ToString(), rootNode["ЕстьСотрудники"].ToString()), 
                    children = { }, 
                    state = new NodeState { opened = true, selected = false, loaded = true }
                };
            }

            var view = new System.Data.DataView(dt);
            view.RowFilter = "ParentId="+ loadid;

            foreach (System.Data.DataRowView kvp in view)
            {
                string parentId = kvp["Id"].ToString();
                Node node = null;
                if ((int)kvp["ЕстьДети"] == 1)
                    node = new Node
                    {
                        id = kvp["Id"].ToString(),
                        text = GetIconByOffice(kvp["id"].ToString(), kvp["Office"].ToString(), kvp["WorkPlace"].ToString()) + kvp["text"] + GetUserIcon(kvp["id"].ToString(), kvp["ЕстьСотрудники"].ToString()),
                        state = new NodeState { opened = false, selected = false, loaded = true }
                    };
                else
                    node = new Node
                    {
                        id = kvp["Id"].ToString(),
                        children = { },
                        text = GetIconByOffice(kvp["id"].ToString(), kvp["Office"].ToString(), kvp["WorkPlace"].ToString()) + kvp["text"] + GetUserIcon(kvp["id"].ToString(), kvp["ЕстьСотрудники"].ToString()), 
                        state = new NodeState { opened = false, selected = false, loaded = false }
                    };


                if (root != null)
                {
                    root.children.Add(node);
                    AddChildItems(dt, node, parentId);
                }

            }
            return (new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(root));

        }

        private void AddChildItems(System.Data.DataTable dt, Node parentNode, string ParentId)
        {
            System.Data.DataView viewItem = new System.Data.DataView(dt);
            viewItem.RowFilter = "ParentId=" + ParentId;
            foreach (System.Data.DataRowView childView in viewItem)
            {
                Node node = new Node {
                    id = childView["Id"].ToString(),
                    text = GetIconByOffice(childView["id"].ToString(), childView["Office"].ToString(), childView["WorkPlace"].ToString()) + childView["text"] + GetUserIcon(childView["id"].ToString(), childView["ЕстьСотрудники"].ToString()),
                    state = new NodeState { opened = false, selected = false } };
                parentNode.children.Add(node);
                string pId = childView["Id"].ToString();
                AddChildItems(dt, node, pId);
            }
        }

        private static string GetIconByOffice(string id, string office, string workPlace)
        {
            var iconsString = new StringBuilder();


            if (!string.IsNullOrWhiteSpace(ReturnId))
                iconsString.Append(String.Format("<img style='border:none' src='/styles/BackToList.gif' title='Выбрать значение' onclick='ReturnValue({0},{1});'>&nbsp;", id, office));

            if (office.Equals("1"))
                iconsString.Append(String.Format("<img style='border:none' src='/styles/House.gif' title='офис'>&nbsp;"));

            switch (workPlace)
            {
                case "1":
                    iconsString.Append(String.Format("<img style='border:none' src='/styles/Portofolio.gif' title='номер в гостинице'>&nbsp;"));
                    break;
                case "2":
                    iconsString.Append(String.Format("<img style='border:none' src='/styles/bed.gif' title='номер в гостинице'>&nbsp;"));
                    break;
                case "3":
                    iconsString.Append(String.Format("<img style='border:none' src='/styles/service.gif' title='номер в гостинице'>&nbsp;"));
                    break;
                case "4":
                    iconsString.Append(String.Format("<img style='border:none' src='/styles/Store.gif' title='номер в гостинице'>&nbsp;"));
                    break;
                case "5":
                    iconsString.Append(String.Format("<img style='border:none' src='/styles/Notebook.gif' title='номер в гостинице'>&nbsp;"));
                    break;
                default:
                    iconsString.Append(String.Format("<img style='border:none' src='/styles/Empty.gif' title='номер в гостинице'>&nbsp;"));
                    break;
            }

            return iconsString.ToString();
        }

        private static string GetUserIcon(string id, string hasEmployee)
        {
            var iconsString = new StringBuilder();

            if (hasEmployee.Equals("1"))
                iconsString.Append(String.Format("&nbsp;<img id='img_{0}' src='/styles/user.gif' onmouseover='ShowUsers({0})'>", id));
            else if (!hasEmployee.Equals("0"))
                iconsString.Append(String.Format("&nbsp;<img id='img_{0}' src='/styles/users.gif' onmouseover='ShowUsers({0})'>", id));

            return iconsString.ToString();
        }

        class Node
        {
            public Node()
            {
                children = new System.Collections.Generic.List<Node>();
            }

            public string id { get; set; }
            public string text { get; set; }
            public string icon { get; set; }
            public NodeState state { get; set; }
            public System.Collections.Generic.List<Node> children { get; set; }
        }

        class NodeState {

            public bool opened { get; set; }
            public bool selected { get; set; }
            public bool loaded { get; set; }
        }

    }
}