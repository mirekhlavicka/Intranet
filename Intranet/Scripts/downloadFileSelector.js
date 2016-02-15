var fileSelected = function (id, name, url) {
    $('#id_file').val(id);
    $('#fileName').text(name);
    $('#fileName').prop('href', url);
};

var openFileBrowser = function () {
    var w = screen.width * 0.8, h = screen.height * 0.8;
    var left = Number((screen.width / 2) - (w / 2)), top = Number((screen.height / 2) - (h / 2));

    window.open('/UserFiles/Browse?select=2', 'SelectFile', 'scrollbars=yes, resizable=yes,toolbar=no, location=no, directories=no, status=no, menubar=no, copyhistory=no, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
};

$(function () {
    $(document ).on('click', '#btnSelectFile', function () {
        openFileBrowser();
    });
})