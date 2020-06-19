// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.

$('#Kontrola_StatusId').on('change', function () {
    if ($('#Kontrola_StatusId').val() == 40) {
        $('#Kontrola_Uwagi').val("Nie stwierdzono nieprawidłowości, nie wydano zaleceń.");
    }
    else if ($('#Kontrola_Uwagi').val() == "Nie stwierdzono nieprawidłowości, nie wydano zaleceń.") {
        $('#Kontrola_Uwagi').val("");
    }
});

$('#Kontrola_Szukaj').click(function (e) {
    var numer = $("#SearchString").val().split('/');
    if (numer.length > 1 && numer[0].length > 0) {
        e.preventDefault();
        $("#numer").val($("#SearchString").val());
        $.post("/Kontrole", $("#SearchForm").serialize())
            .done(function (url) {
                $(location).attr('href', url);
            });
    }
});

$('#searchGroup').on('hidden.bs.collapse', function () {
    $('#searchIcon').toggleClass("fa-angle-double-up");
    $('#searchIcon').toggleClass("fa-angle-double-down");
});

$('#searchGroup').on('shown.bs.collapse', function () {
    $('#searchIcon').toggleClass("fa-angle-double-down");
    $('#searchIcon').toggleClass("fa-angle-double-up");
});

$(function () {
    $(".datepicker").datepicker();
});

$('#AddKomorka').click(function () {
    var i = $(".thingRow").length;
    $.ajax({
        url: '/Kontrole/Create?handler=AddKomorka&index=' + i,
        success: function (data) {
            $('#Komorki > tbody').append(data);
        },
        error: function (a, b, c) {
            console.log(a, b, c);
        }
    });
    $.ajax({
        url: "/Kontrole/Create?handler=Komorki",
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: JSON,
        success: function (result) {
            $(".json2:last").append($("<option></option>").val(0).html("Wybierz"));
            $(result).each(function () {
                $(".json2:last").append($("<option></option>").val(this.id).html(this.nazwa));
            });
        },
        error: function (data) { }
    });
    if ($('#Komorki').length > 0) {
        $('#RemoveKomorka').prop("disabled", false);
    }
});
$('#RemoveKomorka').on('click', function () {
    $('#Komorki tr:last').remove();
    if ($('.thingRow').length == 0) {
        $('#RemoveKomorka').prop("disabled", true);
    }
});

$('.datepicker').datepicker(
    {
        changeYear: true
    });

$('a[data-toggle="tooltip"]').tooltip(
    {
    });

function td(row, index) {
    return $(row).children().eq(index - 1)[0].innerText
}

$('.history>tbody>tr').each(function (i, row) {
    if (row.children[1].childElementCount == 0) {
        var span = null;
        var before = td(row, 2);
        var after = td(row, 3);

        var diff = Diff.diffWords(before, after),
            fragment = document.createDocumentFragment();
        diff.forEach(function (part) {
            if (typeof part.added !== 'undefined') {
                formatting = '#ddfbe6';
                span = document.createElement('span');
                span.style.backgroundColor = formatting;
                span.appendChild(document
                    .createTextNode(part.value));
            }
            else {
                formatting = part.removed ? 'line-through' : 'inherit';
                span = document.createElement('span');
                span.style.textDecoration = formatting;
                span.appendChild(document
                    .createTextNode(part.value));
            }
            fragment.appendChild(span);
        });

        $(row).children().eq(2)[0].innerHTML = '';
        $(row).children().eq(2)[0].appendChild(fragment);
    }
});

$(function () {
    $('a.print').click(function () {
        var url = $(this).attr('href');
        window.open(url, "popupWindow", "width=1920,height=1080,scrollbars=yes");
    });
    return false;
});

function getCSS(prop, fromClass) {

    var $inspector = $("<div>").css('display', 'none').addClass(fromClass);
    $("body").append($inspector); // add to DOM, in order to read the CSS property
    try {
        return $inspector.css(prop);
    } finally {
        $inspector.remove(); // and remove from DOM
    }
};