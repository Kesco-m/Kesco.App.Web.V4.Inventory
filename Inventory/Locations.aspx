<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Locations.aspx.cs" Inherits="Kesco.App.Web.Inventory.Locations" %>
<%@ Register TagPrefix="csg" Namespace="Kesco.Lib.Web.Controls.V4.Grid" Assembly="Controls.V4" %>
<%@ Register TagPrefix="v4control" Namespace="Kesco.Lib.Web.Controls.V4" Assembly="Controls.V4" %>
<%@ Register TagPrefix="uc" TagName="MainMenu" Src="MainMenu.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Расположения и розетки</title>
    
    <link rel="stylesheet" href="/Styles/Kesco.V4/JS/themes/default/style.min.css" />
	<script src='/Styles/Kesco.V4/JS/jstree.min.js' type='text/javascript'></script>
    <script src="/styles/Kesco.V4/JS/jquery.floatThead.min.js" type="text/javascript"></script>    
    <script src="/styles/Kesco.V4/JS/Kesco.Grid.js" type="text/javascript"></script>
    <script src="Kesco.Locations.js" type="text/javascript"></script>
    

       <style type="text/css">
           #divContainer {
               width:97%;
               margin-top:10px;
               margin-left:11px;
               
           }
           #divMyTreeContainer {
               
               min-width:300px;
               min-height: 300px;
               width:300px;
               max-width:70%;
               float: left;
               
           }
           #divMyTree {
              
               border-right: 2px dotted dimgray;
               min-width:300px;
               min-height: 300px;
               
               overflow:auto;
           }
          
           #divMyData {  
                padding-left:10px;
                overflow:auto;

           }
           
           /*++++++++++++++++++++++++++++++++++++++++++++++++++*/
           #divLocationInfo 
           {
               margin-bottom: 10px;
               font-weight: bold;
           }
            
           #divSocketInfo 
           {
               font-weight: bold;
           }
           
           .marginT {
               margin-top: 10px;
           }
          
    </style>
</head>
<body>
    <uc:mainmenu id="MainMenu1" runat="server"></uc:mainmenu>
    <div style="margin-top: 10px; margin-left:11px; font-weight: bold;">Расположения и розетки</div>
    <div id="divContainer" >
        <div id="divMyTreeContainer" class="ui-widget-content">
            <div id="divMyTree"></div>
        </div>
        <div id="divMyData" >
            <div id = "divLocationInfo"></div>

            <div id="tabs" style="display: none;">
                <ul>
                    <li><a href="#tabs-1"><nobr>&nbsp;<IMG src="/styles/Tools.gif" align=absMiddle/>&nbsp;Характеристики</nobr></a></li>
                    <li><a href="#tabs-2"><nobr>&nbsp;<IMG src="/styles/DocMain.gif" align=absMiddle/>&nbsp;Розетки</nobr></a></li>
                    <li><a href="#tabs-3"><nobr>&nbsp;<IMG src="/styles/users.gif" align=absMiddle/>&nbsp;Сотрудники</nobr></a></li>
                    <li><a href="#tabs-4"><nobr>&nbsp;<IMG src="/styles/Service.gif" align=absMiddle/>&nbsp;Оборудование</nobr></a></li>
                </ul>
                 <div id="tabs-1">
                    <div id="divSpecifications">
                        
                    </div>
                    <div id="divPrinter" style="margin-top:50px">
                        
                    </div>
                </div>
                <div id="tabs-2">
                     <v4control:Button ID="btnSocketAdd" runat="server"></v4control:Button>
                    <div id = "divSockets" style="margin-top: 10px">
                        <csg:Grid runat="server" ID="gridSockets" MarginBottom="223" ShowGroupPanel = "True" />            
                    </div>
                </div> 
                <div id="tabs-3">
                     <div id = "divEmployees">
                        <csg:Grid runat="server" ID="gridEmployees" MarginBottom="190" />            
                    </div>
                </div>
                <div id="tabs-4">
                    <div id = "divEquipment">
                        <csg:Grid runat="server" ID="gridEquipment" MarginBottom="190" />            
                    </div>
                </div>

            </div>
        </div>
    </div>
    
    <div id="draggable" class="ui-widget-content" style="width:50px" >
          <p>Drag me to my target</p>
    </div>
 
    <div id="droppable" class="ui-widget-header" style="width:50px">
      <p>Drop here</p>
    </div>

</body>
<script type="text/javascript">
    $("#divMyTreeContainer").resizable({ handles: 'e' });

    $(document).ready(function () {

        $("#divMyTree").jstree({
            'plugins': ["state"],
            'core': {
                'data': {
                    'url': function (node) {
                        var uri = 'LocationJsonData.ashx';
                        return uri;
                    },
                    'data': function (node) {
                        return { 'loadid': node.id };
                    },
                    "check_callback": true
                }
            }
            
        });

        $('#divMyTree').bind("select_node.jstree", function (e, data) {
            Wait.render(true);
            cmdasync('cmd', 'LoadDataLocation', 'Id', data.node.id);
        });

        $("#divMyTree").bind("hover_node.jstree", function (e, data) {            
            $("#"+data.node.id ).attr("title",data.node.id + " -> " +data.node.text);
        });

        function handleResize() {
            var h = $(window).height();
            

            $('#divContainer').css({ 'height': (h - 73) + 'px' });
            $('#divMyTree').css({ 'height': (h - 78) + 'px' });
            $('#divMyData').css({ 'height': (h - 75) + 'px' });
        }

        $(function () {
            handleResize();

            $(window).resize(function () {
                handleResize();
            });
        });

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

    $(function () {
        $("#draggable").draggable();
        $("#droppable").droppable({
            drop: function (event, ui) {
                $(this)
          .addClass("ui-state-highlight")
          .find("p")
            .html("Dropped!");
            }
        });
    });


    });
</script>
</html>
