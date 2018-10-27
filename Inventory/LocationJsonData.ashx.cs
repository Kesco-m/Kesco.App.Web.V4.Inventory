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
            var id = string.IsNullOrWhiteSpace(context.Request.QueryString["loadid"])|| context.Request.QueryString["loadid"] =="#" ? 0 : int.Parse(context.Request.QueryString["loadid"]);
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

        private string GetTreedata(int loadid)
        {
            var sql = "SELECT КодРасположения Id, R-L ЕстьДети, Расположение Text, ISNULL(Parent,0) ParentId, CASE WHEN Офис=1 THEN '/styles/house.gif' ELSE 'jstree-file' END icon FROM vwРасположения WHERE Parent IS NULL ORDER BY L";

            if (loadid != 0)
            {
                sql = "SELECT КодРасположения Id, R-L ЕстьДети, Расположение Text, ISNULL(Parent,0) ParentId, CASE WHEN Офис=1 THEN '/styles/house.gif' ELSE 'jstree-file' END icon FROM vwРасположения WHERE Parent =" + loadid + " ORDER BY L";
            }

            var dt = Kesco.Lib.DALC.DBManager.GetData(sql, Kesco.Lib.Web.Settings.Config.DS_user);

            Node root = null;
            if (loadid == 0)
                root = new Node { id = "0", children = { }, text = "Расположения", state = new NodeState { opened = true, selected = false, loaded = true } };
            else
            {
                sql = "SELECT Расположение FROM vwРасположения WHERE КодРасположения=@id";
                var valueText=Kesco.Lib.DALC.DBManager.ExecuteScalar(sql, loadid, System.Data.CommandType.Text, Kesco.Lib.Web.Settings.Config.DS_user);
                root = new Node { id = loadid.ToString(), text = valueText.ToString(), children = { }, state = new NodeState { opened = true, selected = false, loaded = true } };
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
                        text = kvp["text"].ToString(),
                        icon = kvp["icon"].ToString(),
                        state = new NodeState { opened = false, selected = false, loaded = true }
                    };
                else
                    node = new Node
                    {
                        id = kvp["Id"].ToString(),
                        children = { },
                        text = kvp["text"].ToString(),
                        icon = kvp["icon"].ToString(),
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
                    text = childView["text"].ToString(),
                    icon = childView["icon"].ToString(),
                    state = new NodeState { opened = false, selected = false } };
                parentNode.children.Add(node);
                string pId = childView["Id"].ToString();
                AddChildItems(dt, node, pId);
            }
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