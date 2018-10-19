<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Location.aspx.cs" Inherits="Kesco.App.Web.Inventory.Location" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Расположения</title>
    
    <link rel="stylesheet" href="/Styles/Kesco.V4/JS/themes/default/style.min.css" />
	<script src='/Styles/Kesco.V4/JS/jstree.min.js' type='text/javascript'></script>
</head>
<body>
    <div id="html" class="demo">
		<ul>
			<li data-jstree='{ "opened" : true }'>Root node
				<ul>
					<li data-jstree='{ "selected" : true }'>Child node 1</li>
					<li>Child node 2</li>
				</ul>
			</li>
		</ul>
	</div>
</body>
<script>
    $(document).ready(function() {

        $("#html").jstree({
            'core': {
                'data': {
                    "url": "LocationJsonData.ashx",
                    "dataType": "json" // needed only if you do not supply JSON headers
                }
            }
        });


    });
</script>
</html>
