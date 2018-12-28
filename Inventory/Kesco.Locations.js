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

    alert(id);
}

function ShowUsers(id) {
    $('#img_'+id).attr('title','load...');

    window.setTimeout(function () { cmdasync('cmd', 'RenderUserCount', 'id', id); }, 0);
}

function ReturnValuePostForm(value, doEscape) {
    var form = document.getElementById('mvcDialogResult');
    if (!form) return;

    var val = JSON.stringify(value);
    form.action = callbackUrl;
    form.elements["value"].value = doEscape ? escape(val) : val;
    form.elements["escaped"].value = doEscape ? "1" : "0";
    form.elements["control"].value = control;
    form.elements["multiReturn"].value = multiReturn;
    form.submit();
    //setTimeout(function () { parent.closeWindow(); }, 2000);
}

function ReturnValueSetCookie(id) {
    document.cookie = 'DlgRez=1;domain=' + domain + ';path=/';
    document.cookie = 'RetVal=' + id + ';domain=' + domain + ';path=/';
    document.cookie = 'ParentAction=0;domain=' + domain + ';path=/';

    try {
        window.returnValue = id;
    }
    catch (e) { }

    var version = parseFloat(navigator.appVersion.split('MSIE')[1]);
    if (version < 7)
        window.opener = this;
    else
        window.open('', '_parent', '');
    window.close();
}

function ReturnValue(id, name) {
    if (mvc == 1 || mvc == 4) {
        var result = [];

        result[0] = {
            value: id,
            label: name
        };

        ReturnValuePostForm(result, true);
    }
    else {
        ReturnValueSetCookie(id);
    }
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
                click: function () { cmd('cmd', 'SetLocation', 'type', type); }
            },
            {
                id: "btn_Cancel",
                text: canceltext,
                icons: {
                    primary: v4_buttonIcons.Cancel
                },
                width: 75,
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