
var Month = {
    short: {
        1: "Jan", 2: "Feb", 3: "Mar", 4: "Apr",
        5: "May", 6: "Jun", 7: "Jul", 8: "Aug",
        9: "Sep", 10: "Oct", 11: "Nov", 12: "Dec"
    }, long: {
        1: "January", 2: "February", 3: "March", 4: "April",
        5: "May", 6: "Juny", 7: "July", 8: "August",
        9: "September", 10: "October", 11: "November", 12: "December"
    }
}

function toCurrency(number, currency = "USD") {
    return new Intl.NumberFormat("en-US", { style: "currency", currency: currency }).format(number);
}

function ApiFetchError() {
    showAlert("Error", "Server didn't respond, please check your connection & consider to contact the administrator.");
}

//arrAddData -> [ [<input name>, <value>], ... ], override? yea u can
function SendAPI(idForm = '', url, isSerialize = false, cbSuccess = function () { }, cbError = function () { }, arrAddData = null) {
    //var cbReturn = false;
    var submit = {}, procData, contentType;
    showLoading();
    //if (isSerialize == false) {
        procData = false; contentType = false;
        if (idForm != '')
            submit = new FormData(document.getElementById(idForm));
        else
            submit = new FormData();
        if (arrAddData !== null)
            if (arrAddData.constructor === Array) {
                arrAddData.forEach(function (val) {
                    if (val[0] != null && val[1] != null)
                        submit.set(val[0], val[1]);
                });
            }
    /*} else {
        procData = true;
        //contentType = 'application/x-www-form-urlencoded; charset=UTF-8';
        contentType = 'application/json';
        //contentType = 'multipart/form-data';
        submit = $('#' + idForm).serializeArray();
        if (arrAddData !== null)
            if (arrAddData.constructor === Array) {
                arrAddData.forEach(function (val) {
                    if (val[0] != null && val[1] != null)
                        submit.push({'name': val[0], 'value': val[1]});
                });
            }
        //submit = JSON.stringify(submit);
    }*/
    
    $.ajax({
        url: url,
        method: 'POST',
        data: submit,
        processData: procData,
        contentType: contentType,
        dataType: 'json',
        accepts: 'application/json',
        success: function (data, status, xhr) { hideLoading(); cbSuccess(data, status, xhr); },
        error: function (xhr, status, err) { hideLoading(); cbError(xhr, status, err); }
    })/*.always(function () {
        hideLoading();
    })*/;//.done(function () { cbReturn = true }).fail(function () { cbReturn = false });
}
function SendDxForm(idForm, url, withFile = false, cbSuccess = function () { }, cbError = function () { }, arrAddData = null) {
    showLoading();
    var submit = new FormData();
    for (iSubmit in $('#' + idForm).dxForm('option').formData) {
        submit.set(iSubmit, $('#' + idForm).dxForm('option').formData[iSubmit]);
    }
    if (withFile != false)
        for (iFor in $('#' + withFile + ' input')[0].files) {
            if (isNaN(iFor)) continue;
            submit.append('files', $('#' + withFile + ' input')[0].files[iFor],
                $('#' + withFile + ' input')[0].files[iFor].name);
        }
    if (arrAddData !== null) {
        if (arrAddData.constructor === Array) {
            arrAddData.forEach(function (val) {
                if (val[0] != null && val[1] != null)
                    submit.append(val[0], val[1]);
            });
        }
    }
    $.ajax({
        url: url,
        method: 'POST',
        data: submit,
        processData: false,
        contentType: false,
        dataType: 'json',
        success: function (data, status, xhr) { hideLoading(); cbSuccess(data, status, xhr); },
        error: function (xhr, status, err) { hideLoading(); cbError(xhr, status, err); }
    });
}
function dxTVSetSelected(id, arrKey) {
    if ($('#' + id).dxTreeView('option').items.length == 0) return false;
    $('#' + id).dxTreeView('unselectAll');
    //console.log($('#' + id).dxTreeView('option').items);
    if (arrKey.constructor != Array)
        arrKey = arrKey.split(',');
    for (index in arrKey) {
        /*$('#' + id).dxTreeView('option').items.find(function (el) {
            return el.id == arrKey[index];
        }).selected = true;*/
        $('#' + id).dxTreeView('selectItem', arrKey[index]);
    }
    $('#' + id).dxTreeView('selectItem', arrKey[index]);
    return true;
}
function dxTVGetSelected(id) {
    var cb = [];
    for (index in $('#' + id).dxTreeView('option').items) {
        if ($('#' + id).dxTreeView('option').items[index].selected == true)
            cb.push($('#' + id).dxTreeView('option').items[index].id);
    }
    return cb;
}
function showLoading() {
    $('#loader').addClass('active');
    $('#loader').modal({
        closeExisting: false, escapeClose: false,
        clickClose: false, showClose: false, blockerClass: 'nope', modalClass: ''
    });
}
function hideLoading() {
    $('#loader').removeClass('active');
    $.modal.close();
}
function showAlert(head, msg) {
    $('#modal-public').off('**');
    $('#modal-public .modal-head').html(head);
    $('#modal-public .modal-text').html(msg);
    //var btnOk = '<a rel="modal:close"><button class="btn" id="btn-ok">OK</button></a>';
    var btnOk = '<button class="btn" id="btn-ok">OK</button>';
    $('#modal-public .modal-foot').html(btnOk);
    $('#modal-public').modal({
        closeExisting: false,
        escapeClose: true,
        clickClose: false,
        showClose: false,
        blockerClass: 'nope'
    });
    $('#btn-ok').focus();
}
function showConfirm(head, msg) {
    $('#modal-public').off('**');
    $('#modal-public .modal-head').html(head);
    $('#modal-public .modal-text').html(msg);
    var btnYes = '<button class="btn" id="btn-yes">Yes</button>';
    var btnNo = '<button class="btn" id="btn-no">No</button>';
    $('#modal-public .modal-foot').html(btnNo + ' &nbsp; ' + btnYes);
    $('#modal-public').modal({
        closeExisting: false,
        escapeClose: true,
        clickClose: false,
        showClose: false,
        blockerClass: 'nope'
    });
    $('#btn-yes').focus();
}

$(document).ready(function () {
    $('.content').css({
        //'min-height': 'calc'$('.navbar').height() - $('')
    });

    $(document).on('click', '#btn-ok', function (ev) {
        $.modal.close();
    });

    $(document).on('mouseover', '.dropdown', function (ev) {
        var child = $(this).children('.dropdown-menu');
        if (child.length == 0) return false;
        var childLeft = child.offset().left, childWidth = child.width();
        if (childLeft + childWidth > $(window).width() || $(document).width() > $(window).width()) {
            child.css('margin-left', '-' + (childWidth - $(this).width()) + 'px');
        }
    });
    $(document).on('mouseover', '.dropside', function (ev) {
        if ($(window).width() <= 720) return;

        var child = $(this).children('.dropdown-menu');
        //child.css('margin-left', $(this).width() + 'px');
        var childLeft = child.offset().left + $(this).width(), childWidth = child.width();
        //console.log(childLeft + ' - ' + childWidth);
        if (child.css('margin-left') == '' || child.css('margin-left') == null || child.css('margin-left') == '0px') {
            if (childLeft + childWidth > $(window).width() || $(document).width() > $(window).width())
                child.css('margin-left', '-' + child.width() + 'px');
            else
                child.css('margin-left', $(this).width() + 'px');
            child.css('margin-top', '-' + $(this).height() + 'px');
        }
    });
    /*$(document).on('mouseleave', '.dropdown, .dropside', function () {
        $(this).children('.dropdown-menu').css('margin-left', '');
        $(this).children('.dropdown-menu').css('margin-top', '');
        $(this).css('margin-left', '');
    });*/


    /*$(document).on('keydown', 'input', function (ev) {
        if (ev.which == 13 && !($(this).is('button') || $(this).is('submit') || $(this).is('reset') ||
            $(this).attr('type') == 'button' || $(this).attr('type') == 'submit' || $(this).attr('type') == 'reset') ) {
            ev.preventDefault();
            $(this).trigger($.Event('keydown', {which: 9, keyCode: 9 }));
        }
    });*/
    $('#mnu-secondary').on('click', function (ev) {
        ev.preventDefault();
        if ($('#mnu-primary').css('display') == 'block') {
            $('#mnu-primary').css('display', 'none');
            $('#mnu-secondary').removeClass('menu-active');
            $('#mnu-primary ol > .dropdown').removeClass('dropside');
            $('#mnu-primary ol > .dropdown > .dropdown-menu').removeData('drop');
        } else {
            $('#mnu-primary').css('display', 'block');
            $('#mnu-secondary').addClass('menu-active');
            $('#mnu-primary ol > .dropdown').addClass('dropside');
            $('#mnu-primary ol > .dropdown > .dropdown-menu').data('drop', "side");
        }
    });
});
