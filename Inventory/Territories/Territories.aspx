<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Territories.aspx.cs" Inherits="Kesco.App.Web.Inventory.Territories.Territories" %>
<%@ Register TagPrefix="csg" Namespace="Kesco.Lib.Web.Controls.V4.Grid" Assembly="Controls.V4" %>
<%@ Register TagPrefix="cstv" Namespace="Kesco.Lib.Web.Controls.V4.TreeView" Assembly="Controls.V4" %>
<%@ Register TagPrefix="v4control" Namespace="Kesco.Lib.Web.Controls.V4" Assembly="Controls.V4" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title><%=Resx.GetString("Inv_lblTerritories")%></title>
    <link rel="stylesheet" type="text/css" href="Kesco.Territories.css" />
    <script src="Kesco.Territories.js" type="text/javascript"></script>
</head>
<body>

    <form id="mvcDialogResult" method="post">
        <input type="hidden" name="escaped" value="0" />
		<input type="hidden" name="control" value="" />
        <input type="hidden" name="multiReturn" value="" />
		<input type="hidden" name="value" value="" />
    </form>

    <div class="marginD"><%=RenderDocumentHeader()%></div>  
    <div id="divContainer">
        <div id="divMyTreeContainer" class="ui-widget-content">
            <cstv:TreeView runat="server" ID="tvTerritory"/>
        </div>
        <div id="divMyData" class="ui-widget-content">
            <div id="tabs">
                <ul>
                    <li id="tabs1"><a href="#tabs-1"><nobr>&nbsp;<%=Resx.GetString("Inv_lblTerritories")%></nobr></a></li>
                    <li id="tabs2"><a href="#tabs-2"><nobr>&nbsp;<%=Resx.GetString("Inv_lblPhoneCodes")%></nobr></a></li>
                </ul>
                <div id="tabs-1">
                    <div class="marginT"></div>
                    <div id="divGridTerritory">
                        <csg:Grid runat="server" ID="gridTerritory" MarginBottom="138"/>
                    </div>
                </div>
                <div id="tabs-2">

                </div> 
            </div>
        </div>
    </div>
</body>
</html>