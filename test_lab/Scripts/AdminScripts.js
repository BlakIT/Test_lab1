var clickIndex = 0; //на какую ссылку мы нажали   3 - Входящие, 4 - Отправленные
var clickedRowIndex = 0;  //номер строки на  которую нажали  в таблице клиентов
var boxWidth = 500;   //ширина  pop UP окна

function LoadPartialView(e) {

    $('#infoTD').html("");
    $('#mainCenter').load('@Url.Action("LoadPartial", "Admin")' + '?index=' + e);
    clickIndex = e;

}

function LoadLastDialogs() {

    $('#dialogsParialDiv').html("");

    var childrens = $("#ClientRows").children();
    var childrens = $(childrens[2 * clickedRowIndex]).children();
    var id = $(childrens[0]).text();

    $('#dialogsParialDiv').load('@Url.Action("LoadDialogsInfoPartial", "Admin")' + '?id=' + id.trim());

    $('.popup-link-4').trigger('click');
}

function ClickOkPopUp5() {
    var t = $('input[name="radioEmpl"]:checked').attr('id');
    $("#popup-link-5").text(t);

    var e = $('input[name="radioEmpl"]:checked').val();
    $("#client_ID_Employee").val(e);

    $('.close').trigger('click');
}

function onEmployeesTablesTrClick(e) {


    var childrens = $(e).children();
    var secretFiled = $(childrens[6]).children();

    var allRows = $(e).parent().children();

    for (var t = 0; t < allRows.length; t++)
        $(allRows[t]).css('background-color', 'transparent');

    $(e).css('background-color', 'yellow');

    var currentEmployee = {
        Name: $(childrens[0]).html(),
        City: $(childrens[5]).html(),
        Post: $(childrens[1]).html(),
        Role: $(secretFiled[1]).html(),
        RegDate: $(secretFiled[0]).html()
    }

    $('#infoTD').html(
        '<h4>Имя:</h4><b style="color:red;">' + currentEmployee.Name + '</b>' +
        '<h4>Адресс:</h4><b style="color:red;">' + currentEmployee.City + '</b>' +
        '<h4>Должность:</h4><b style="color:red;">' + currentEmployee.Post + '</b>' +
        '<h4>Роль:</h4><b style="color:red;">' + currentEmployee.Role + '</b>' +
        '<h4>Дата регистрации:</h4><b style="color:red;">' + currentEmployee.RegDate + '</b>'
        );

}

function onClientsTablesTrClick(e) {

    $('#saveButton').attr('disabled', false);
    $('#clearButton').attr('disabled', false);
    $('#mainInput').attr('disabled', false);

    var childrens = $(e).children();

    clickedRowIndex = $(e).index() / 2;
    // $(MainSaveBut).removeAttr('disabled');

    var allRows = $(e).parent().children();

    for (var t = 0; t < allRows.length; t++)
        $(allRows[t]).css('background-color', 'transparent');

    $(e).css('background-color', 'yellow');

    var currentClient = {
        Name: $(childrens[1]).html(),
        City: $(childrens[2]).html(),
        Contact: $(childrens[3]).html(),
        Post: $(childrens[4]).html(),
        Telephone: $(childrens[5]).html(),
        DinnerTime: $(childrens[6]).html()
    }

    $('#infoTD').html(
        '<h4>Название:</h4> <b style="color:red;">' + currentClient.Name + '</b>' +
        '<h4>Адресс:</h4> <b style="color:red;">' + currentClient.City + '</b>' +
        '<h4>Контактное лицо:</h4> <b style="color:red;">' + currentClient.Contact + '</b>' +
        '<h4>Должность:</h4> <b style="color:red;">' + currentClient.Post + '</b>' +
        '<h4>Телефон:</h4> <b style="color:red;">' + currentClient.Telephone + '</b>' +
        '<h4>Время обеда:</h4> <b style="color:red;">' + currentClient.DinnerTime + '</b>' +
        '<h4 ><a href=\'javascript:LoadLastDialogs();\' style="color:red; text-decoration: underline;">Последние разговоры</a></h4>'
        );

}

function LoadMail(e) {
    $('#mainCenter').load('@Url.Action("LoadMail", "Admin")' + '?index=' + e + '&type=' + clickIndex);
}

function SaveDialog() {

    var textBoxValue = $('#mainInput').val();

    $.ajax(
        {
            url: "/Admin/SaveDialog",
            data: { text: textBoxValue, index: clickedRowIndex },
            type: 'POST',
            success: function () {
                alert('ok');
                $('#mainInput').val('');
            }
        });
}

function SaveReminder() {

    var text = $('#textNap').val();
    var date = $('#dateNap').val();
    var time = $('#timeNap').val();

    $.ajax(
        {
            url: "/Admin/SaveReminder",
            data: { text: text, date: date, time: time },
            type: 'POST',
            success: function () {
                alert('ok');
                $('#textNap').val('');
                $('#timeNap').val('');
                $('#dateNap').val('');
            }
        });
}

function CleanDialog() {
    $('#mainInput').val("");
}

function DeleteClientClick() {

    var childrens = $("#ClientsTable").children();
    var childrens = $(childrens).children();
    var childrens = $(childrens[clickedRowIndex]).children();

    $.ajax(
        {
            url: "/Admin/DeleteClient",
            data: { id: $(childrens[6]).text() },
            type: 'POST',
            success: function (result) {
                alert("Item deleted");
                location.reload();
            }
        });

}

function ClientDblClick(e) {

    var childrens = $(e).children();

    $('#infoTD').html("");

    var id = $(childrens[0]).text();

    var id = id.trim();

    if (id != '')
        $('#mainCenter').load('@Url.Action("LoadClientsInfoPartial", "Admin")' + '?id=' + id);
}

function EmployeesDblClick(e) {

    var childrens = $(e).children();

    $('#infoTD').html("");

    var id = $(childrens[6]).text();

    var id = id.trim();

    if (id != '')
        $('#mainCenter').load('@Url.Action("LoadEmployeesInfoPartial", "Admin")' + '?id=' + id);
}

function centerBox() {

    /* определяем нужные данные */
    var winWidth = $(window).width();
    var winHeight = $(document).height();
    var scrollPos = $(window).scrollTop();

    /* Вычисляем позицию */

    var disWidth = (winWidth - boxWidth) / 2
    var disHeight = scrollPos + 150;

    /* Добавляем стили к блокам */
    $('.popup-box').css({ 'width': boxWidth + 'px', 'left': disWidth + 'px', 'top': disHeight + 'px' });
    $('#blackout').css({ 'width': winWidth + 'px', 'height': winHeight + 'px' });

    return false;
}

$(window).resize(centerBox);
$(window).scroll(centerBox);
centerBox();

function ReminderTimer() {

    $.ajax(
        {
            url: "/Admin/GetReminder",
            type: 'POST',
            success: function (data) {

                if (data != null)
                    alert(data.Text);
            }
        });
}

$(document).ready(function () {
    //$(MainSaveBut).attr('disabled', 'disabled');      

    var id = setInterval(ReminderTimer, 10000);

    $('body').append('<div id="blackout"></div>');

    $('[class*=popup-link]').click(function (e) {

        /* Предотвращаем действия по умолчанию */
        e.preventDefault();
        e.stopPropagation();
        centerBox();
        /* Получаем id (последний номер в имени класса ссылки) */
        var name = $(this).attr('class');
        var id = name[name.length - 1];
        var scrollPos = $(window).scrollTop();

        /* Корректный вывод popup окна, накрытие тенью, предотвращение скроллинга */
        $('#popup-box-' + id).show();
        $('#blackout').show();
        $('html,body').css('overflow', 'hidden');

        /* Убираем баг в Firefox */
        $('html').scrollTop(scrollPos);
    });

    $('[class*=popup-box]').click(function (e) {
        /* Предотвращаем работу ссылки, если она являеться нашим popup окном */
        e.stopPropagation();
    });
    $('html').click(function () {
        var scrollPos = $(window).scrollTop();
        /* Скрыть окно, когда кликаем вне его области */
        $('[id^=popup-box-]').hide();
        $('#blackout').hide();
        $("html,body").css("overflow", "auto");
        $('html').scrollTop(scrollPos);
    });
    $('.close').click(function () {
        var scrollPos = $(window).scrollTop();
        /* Скрываем тень и окно, когда пользователь кликнул по X */
        $('[id^=popup-box-]').hide();
        $('#blackout').hide();
        $("html,body").css("overflow", "auto");
        $('html').scrollTop(scrollPos);
    });
});
