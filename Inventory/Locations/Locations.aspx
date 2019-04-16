<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Locations.aspx.cs" Inherits="Kesco.App.Web.Inventory.Locations" %>
<%@ Register TagPrefix="csg" Namespace="Kesco.Lib.Web.Controls.V4.Grid" Assembly="Controls.V4" %>
<%@ Register TagPrefix="cstv" Namespace="Kesco.Lib.Web.Controls.V4.TreeView" Assembly="Controls.V4" %>
<%@ Register TagPrefix="v4control" Namespace="Kesco.Lib.Web.Controls.V4" Assembly="Controls.V4" %>
<%@ Register TagPrefix="uc" TagName="MainMenu" Src="MainMenu.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Расположения и розетки</title>
    <link rel="stylesheet" type="text/css" href="Kesco.Locations.css" />
    <script src="Kesco.Locations.js" type="text/javascript"></script>
</head>
<body>

    <form id="mvcDialogResult" method="post">
        <input type="hidden" name="escaped" value="0" />
		<input type="hidden" name="control" value="" />
        <input type="hidden" name="multiReturn" value="" />
		<input type="hidden" name="value" value="" />
    </form>

    <uc:mainmenu id="InvMainMenu" runat="server"></uc:mainmenu>
    <div style="margin-top: 10px; margin-left:11px; font-weight: bold;"><%=Resx.GetString("Inv_lblLocationsAndSockets")%></div>
    <div id="divContainer" >
        <div id="divMyTreeContainer" class="ui-widget-content">
            <cstv:TreeView runat="server" ID="tvLocation"/>
        </div>
        <div id="divMyData" class="ui-widget-content">
            <div id = "divLocationInfo" style="margin-top:10px"></div>
            <div id="tabs" style="display: none;">
                <ul>
                    <li><a href="#tabs-1"><nobr>&nbsp;<IMG src="/styles/Tools.gif" align=absMiddle/>&nbsp;<%=Resx.GetString("Inv_lblSpecifications")%></nobr></a></li>
                    <li><a href="#tabs-2"><nobr>&nbsp;<IMG src="/styles/DocMain.gif" align=absMiddle/>&nbsp;<%=Resx.GetString("Inv_lblSockets")%></nobr></a></li>
                    <li><a href="#tabs-3"><nobr>&nbsp;<IMG src="/styles/users.gif" align=absMiddle/>&nbsp;<%=Resx.GetString("Inv_lblEmployees")%></nobr></a></li>
                    <li><a href="#tabs-4"><nobr>&nbsp;<IMG src="/styles/Service.gif" align=absMiddle/>&nbsp;<%=Resx.GetString("Inv_lblEquipment")%></nobr></a></li>
                </ul>
                 <div id="tabs-1">
                </div>
                <div id="tabs-2">
                </div> 
                <div id="tabs-3">
                </div>
                <div id="tabs-4">
                </div>
            </div>
            <v4control:Button ID="btnSocketAdd" runat="server" style="display:none"></v4control:Button>
            <div id = "divSockets" style="margin-top: 10px">
                <csg:Grid runat="server" ID="gridSockets" GridAutoSize="False"/>   
            </div>
            <div id="divSpecifications"></div>
            <div id="divPrinter" style="margin-top:10px"></div>
            <div id = "divEmployees" style="margin-top:10px">
                <csg:Grid runat="server" ID="gridEmployees" GridAutoSize="False"/>            
            </div>
            <div id = "divEquipment">
                <csg:Grid runat="server" ID="gridEquipment" GridAutoSize="False"/> 
            </div>
            <div id = "divPersonLocations" style="margin-top:10px">
                <csg:Grid runat="server" ID="gridPersonLocations" GridAutoSize="False"/>            
            </div>
            <div id="lstorage"></div>
        </div>
    </div>
    
    <div id="divEditNode" style="display: none;">
        <table cellspacing="0" cellpadding="5" border="0">
            <tr>
                <td colspan="2"><v4control:Div runat="server" ID="divLocationPatch"></v4control:Div> </td>
            </tr>
            <tr>
                <td><%=Resx.GetString("Inv_lblLocation")%>:</td>            
                <td><v4control:TextBox ID="tbNode" runat="server" Width="400px" NextControl="btn_Apply"></v4control:TextBox></td>
            </tr>
        </table>
    </div>
    
    <div id="divSocketAdd" style="display: none;">
        <div class="predicate_block">
        <div class="label"><%=Resx.GetString("Inv_lblSocket")%>:</div>
            <v4control:TextBox ID="tbSocket" runat="server" Width="250px" IsRequired="True" NextControl="cbWork"></v4control:TextBox>
        </div>

        <div class="predicate_block">
        <div class="label"><%=Resx.GetString("Inv_lblWorks")%>:</div>
            <v4control:CheckBox ID="cbWork" runat="server" NextControl="tbNote"></v4control:CheckBox>
        </div>
                      
        <div class="area_block">
        <div class="label"><%=Resx.GetString("Inv_lblNote")%>:</div>
            <v4control:TextArea ID="tbNote" runat="server" Width="250px" Height="50px" CSSClass="aligned_control"></v4control:TextArea>
        </div>
    </div>

</body>

<script type="text/javascript">
   
    $("#divMyTreeContainer").resizable({ handles: 'e' });

    $(document).ready(function () {

        v4_setResizableInDialog();

        /* Умещаем дерево в пределах окна, в зависимости от высоты других элементов (второй параметр), при готовности страницы и при изменении размеров окна */
        v4_treeViewHandleResize('tvLocation');
        SetDataSize();

        $(window).resize(function () {
            v4_treeViewHandleResize('tvLocation');
            SetDataSize();
        });

        /*
        $("#tabs").tabs({
            activate: function(event, ui) {
                var tabId = ui.newPanel[0].id;
                switch (tabId) {
                case "tabs-2":
                    window.v4_insert = function() { cmd('cmd', 'SocketAdd'); };
                    break;
                default:
                    window.v4_insert = function() {};
                    break;
                }
            }
        });
        */
    });

    function SetDataSize() {
        var hDivData = $(window).height() - 60;
        $('#divMyData').css({ 'height': hDivData + 'px' });
    }
</script>
</html>
