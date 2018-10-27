
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
