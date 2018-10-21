<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Location.aspx.cs" Inherits="Kesco.App.Web.Inventory.Location" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Расположения</title>
    
    <link rel="stylesheet" href="/Styles/Kesco.V4/JS/themes/default/style.min.css" />
	<script src='/Styles/Kesco.V4/JS/jstree.min.js' type='text/javascript'></script>
       <style type="text/css">
           #divContainer {
               width:99%;
               margin-top:10px;
               
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
          
    </style>
</head>
<body>
    <div id="divContainer" >
        <div id="divMyTreeContainer" class="ui-widget-content">
            <div id="divMyTree"></div>
        </div>
        <div id="divMyData" >           
                         
        </div>
    </div>
</body>
<script type="text/javascript">
    $("#divMyTreeContainer").resizable({ handles: 'e' });

    $(document).ready(function () {

        $("#divMyTree").jstree({
            'core': {
                'data': {
                    'url': function (node) {
                        var uri = 'LocationJsonData.ashx';
                        return uri;
                    },
                    'data': function (node) {
                        return { 'loadid': node.id };
                    }
                }
            }
        });
        $('#divMyTree').on("select_node.jstree", function (e, data) {
            $('#divMyData').html("node_id: " + data.node.id + " -> " + "text: " + data.instance.get_path(data.node, '/'));

        });

        $("#divMyTree").bind("hover_node.jstree", function (e, data) {            
            $("#"+data.node.id ).attr("title",data.node.id + " -> " +data.node.text);
        });

        function handleResize() {
            var h = $(window).height();
            

            $('#divContainer').css({ 'height': (h - 18) + 'px' });
            $('#divMyTree').css({ 'height': (h - 20) + 'px' });
            $('#divMyData').css({ 'height': (h - 20) + 'px' });
            
            
        }

        $(function () {
            handleResize();

            $(window).resize(function () {
                handleResize();
            });
        });

        

    });
</script>
</html>
