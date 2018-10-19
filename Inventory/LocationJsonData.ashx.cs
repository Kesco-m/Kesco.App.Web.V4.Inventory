using System;
using System.Collections.Generic;
using System.Linq;
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
            context.Response.ContentType = "application/json";
            context.Response.Write("[{\"id\":1,\"text\":\"Root node\",\"children\":[{\"id\":2,\"text\":\"Первый\"},{\"id\":3,\"text\":\"Второй\"}]}]");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }

    public class Child
    {
        public string id { get; set; }
        public string name { get; set; }
    }
    public class RootObject
    {
        public string id { get; set; }
        public string name { get; set; }
        public List<Child> child { get; set; }
    }

}