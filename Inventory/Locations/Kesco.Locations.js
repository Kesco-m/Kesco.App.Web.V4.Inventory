//Глобальные переменные должны быть инициализированы во время загрузки страницы, обычно в методе Page_Load()
var callbackUrl;
var control;
var multiReturn;

var mvc = 0;
var domain = '';

function socket_edit(id) {
    socket_RecordsEdit("Розетка", id);
}

function socket_delete(id) {
    cmd("cmd", "SocketDelete", "SocketId", id);
}

socket_RecordsEdit.form = null;

function socket_RecordsEdit(titleForm, id) {
    cmd("cmd", "SocketEdit", "SocketId", id);
}

function ShowUsers(id) {
    $('#img_'+id).attr('title','load...');

    window.setTimeout(function () { cmdasync('cmd', 'RenderUserCount', 'id', id); }, 10);
}


// -------------------------------------------------------------------------------------------------------
locationEdit_dialogShow.form = null;
function locationEdit_dialogShow(title, oktext, canceltext, type) {
    var idContainer = "divEditNode";
    if (null == locationEdit_dialogShow.form) {
        var width = 555;
        var height = 150;
        var onOpen = function () { v4_openLocationForm(); };
        var onClose = function () { v4_closeLocationForm(); };
        var buttons = [
            {
                id: "btn_Apply",
                text: oktext,
                icons: {
                    primary: v4_buttonIcons.Ok
                },
                click: function () {
                    Wait.render(true);
                    cmdasync('cmd', 'SetLocation', 'type', type);
                }
            },
            {
                id: "btn_Cancel",
                text: canceltext,
                icons: {
                    primary: v4_buttonIcons.Cancel
                },
                click: v4_closeLocationForm
            }
        ];

        locationEdit_dialogShow.form = v4_dialog(idContainer, $("#" + idContainer), title, width, height, onOpen, onClose, buttons);
    }

    $("#divEditNode").dialog("option", "title", title);
    locationEdit_dialogShow.form.dialog("open");
}

function v4_closeLocationForm() {
    if (null != locationEdit_dialogShow.form) {
        locationEdit_dialogShow.form.dialog("close");
        locationEdit_dialogShow.form = null;
    }
}

function v4_openLocationForm() {
    if (null != locationEdit_dialogShow.form) {

    }
}
// -------------------------------------------------------------------------------------------------------

socketEdit_dialogShow.form = null;
function socketEdit_dialogShow(title, oktext, canceltext) {
    var idContainer = "divSocketAdd";
    if (null == socketEdit_dialogShow.form) {
        var width = 385;
        var height = 150;
        var onOpen = function () { v4_openSocketForm(); };
        var onClose = function () { v4_closeSocketForm(); };
        var buttons = [
            {
                id: "btn_SocketApply",
                text: oktext + " (F2)",
                icons: {
                    primary: v4_buttonIcons.Ok
                },
                click: function () { cmd('cmd', 'SetSocket'); }
            },
            {
                id: "btn_SocketCancel",
                text: canceltext,
                icons: {
                    primary: v4_buttonIcons.Cancel
                },
                width: 75,
                click: v4_closeSocketForm
            }
        ];

        socketEdit_dialogShow.form = v4_dialog(idContainer, $("#" + idContainer), title, width, height, onOpen, onClose, buttons);

        window.v4_save = function () {
            $("#btn_SocketApply").focus();
            cmd('cmd', 'SetSocket');
        };
    }

    $("#divSocketAdd").dialog("option", "title", title);
    socketEdit_dialogShow.form.dialog("open");
    $("#divSocketAdd").find($('input[type=text]')).focus();
}

function v4_closeSocketForm() {
    if (null != socketEdit_dialogShow.form) {
        socketEdit_dialogShow.form.dialog("close");
        socketEdit_dialogShow.form = null;
        window.v4_save = function () { };
    }
}

function v4_openSocketForm() {
    if (null != socketEdit_dialogShow.form) {

    }
}

function tabActivate(n) {
    $("#tabs").tabs({ active: n });
}

function _return(id, name) {
    v4_returnValue(id, name);
}

$(document).ready(function () {
    window.v4_insert = function () {
        $("#btnSocketAdd").focus();
        cmd('cmd', 'SocketAdd');
    };
});
              

// -------------------------------------------------------------------------------------------------------
