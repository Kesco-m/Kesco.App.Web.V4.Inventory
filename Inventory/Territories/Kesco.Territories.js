//Глобальные переменные должны быть инициализированы во время загрузки страницы, обычно в методе Page_Load()
var callbackUrl;
var control;
var multiReturn;

var mvc = 0;
var domain = '';

$("#divMyTreeContainer").resizable({ handles: 'e' });

$(document).ready(function () {
    $(function () {
        /* Установить способность изменять расположение inline элементов при отображении страницы в диалоге IE */
        v4_setResizableInDialog();

        /* Умещаем дерево в пределах окна, в зависимости от высоты других элементов (второй параметр), при готовности страницы и при изменении размеров окна */
        v4_treeViewHandleResize('tvTerritory', 40);
        $(window).resize(function () {
            v4_treeViewHandleResize('tvTerritory', 40);
        });

        $("#tabs").find($('select')).focus();
    });

    $("#tabs").tabs({
        activate: function (event, ui) {
            var tabId = ui.newPanel[0].id;
            switch (tabId) {
                default:
                    window.v4_insert = function () { };
                    $(this).find($('select')).focus();
                    break;
            }
        }
    });
});