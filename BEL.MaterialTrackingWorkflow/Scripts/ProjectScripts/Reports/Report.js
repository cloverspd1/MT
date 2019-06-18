var ProjectWiseReportExportToExcelUrl = BASEPATHURL + "/Report/ProjectWiseExportToExcel";
var ProjectWiseReportSearchUrl = "/Report/ProjectWiseReportSearch";

var BUWiseReportExportToExcelUrl = BASEPATHURL + "/Report/BUWiseExportToExcel";
var BUWiseReportSearchUrl = "/Report/BUWiseReportSearch";

var MaterialLocationWiseReportExportToExcelUrl = BASEPATHURL + "/Report/MaterialLocationWiseExportToExcel";
var MaterialLocationWiseReportSearchUrl = "/Report/MaterialLocationWiseReportSearch";

var SerialNumberWiseReportExportToExcelUrl = BASEPATHURL + "/Report/SerialNumberWiseExportToExcel";
var SerialNumberWiseReportSearchUrl = "/Report/SerialNumberWiseReportSearch";

var DateWiseReportExportToExcelUrl = BASEPATHURL + "/Report/DateWiseExportToExcel";
var DateWiseReportSearchUrl = "/Report/DateWiseReportSearch";


$(document).ready(function () {
    BindUserTags('');

    $('#BUName,#MaterialLocation').multiselect({
        includeSelectAllOption: true
    });

    $(document).on('click', '#btnSearchByProject', function () {
        SearchByProject();
    });
    $(document).on('click', '#btnProjectWiseExportToExcel', function () {
        ProjectWiseExportToExcel();
    });

    $(document).on('click', '#btnSearchByBU', function () {
        SearchByBU();
    });
    $(document).on('click', '#btnBUWiseExportToExcel', function () {
        BUWiseExportToExcel();
    });

    $(document).on('click', '#btnSearchByMaterialLocation', function () {
        SearchByMaterialLocation();
    });
    $(document).on('click', '#btnMaterialLocationWiseExportToExcel', function () {
        MaterialLocationWiseExportToExcel();
    });

    $(document).on('click', '#btnSearchBySerialNumber', function () {
        SearchBySerialNumber();
    });
    $(document).on('click', '#btnSerialNumberWiseExportToExcel', function () {
        SerialNumberWiseExportToExcel();
    });

    $(document).on('click', '#btnSearchByDate', function () {
        SearchByDate();
    });
    $(document).on('click', '#btnDateWiseExportToExcel', function () {
        DateWiseExportToExcel();
    });

});



function SearchReportAjaxCall(url, postData) {
    AjaxCall({
        url: url,
        postData: postData, httpmethod: 'POST', calldatatype: 'HTML', sucesscallbackfunction: function (data) {
            jQuery('#tblReport').html(data);
            BindDataTable();
        }
    });
}

function filterCriteria(filterControlId) {
    var controlID = '#' + filterControlId;
    var filterValue = '';
    jQuery(controlID + " option:selected").each(function (i, e) {
        filterValue = filterValue + jQuery(e).text().trim() + ",";
    });
    return filterValue != '' ? filterValue.substring(0, filterValue.length - 1) : '';
}

function SearchByProject() {
    var postData = {
        filter: jQuery("#ProjectName").val()
    }
    SearchReportAjaxCall(ProjectWiseReportSearchUrl, postData);
}

function ProjectWiseExportToExcel() {
    window.open(ProjectWiseReportExportToExcelUrl + "?filter=" + jQuery('#ProjectName').val(), '_self');
}

function SearchByBU() {
    var filterBU = filterCriteria('BUName');

    var postData = {
        filter: filterBU
    }

    SearchReportAjaxCall(BUWiseReportSearchUrl, postData);
}

function BUWiseExportToExcel() {
    var filterBU = filterCriteria('BUName');
    window.open(BUWiseReportExportToExcelUrl + "?filter=" + filterBU, '_self');
}

function SearchByMaterialLocation() {
    var filterLocation = filterCriteria('MaterialLocation');

    var postData = {
        filter: filterLocation
    }
    SearchReportAjaxCall(MaterialLocationWiseReportSearchUrl, postData);
}

function MaterialLocationWiseExportToExcel() {
    var filterLocation = filterCriteria('MaterialLocation');
    window.open(MaterialLocationWiseReportExportToExcelUrl + "?filter=" + filterLocation, '_self');
}

function SearchBySerialNumber() {
    var postData = {
        filter: jQuery("#SerialNumber").val()
    }
    SearchReportAjaxCall(SerialNumberWiseReportSearchUrl, postData);
}

function SerialNumberWiseExportToExcel() {
    window.open(SerialNumberWiseReportExportToExcelUrl + "?filter=" + jQuery('#SerialNumber').val(), '_self');
}

function SearchByDate() {
    var postData = {
        filter: jQuery("#DateValues").val()
    }
    SearchReportAjaxCall(DateWiseReportSearchUrl, postData);
}

function DateWiseExportToExcel() {
    window.open(DateWiseReportExportToExcelUrl + "?filter=" + jQuery('#DateValues').val(), '_self');
}

$(document).ready(function () {
    BindDataTable();
});

function BindDataTable() {
    $("#MTReports").DataTable({
        "paging": true,
        "searching": false,
        "info": false,
        "order": [[1, "asc"]],
        "fixedHeader": true,
        "autoWidth": false,
        //"fnDrawCallback": function (oSettings) {
        //    if ($('#MTReports tr').length < 10) {
        //        $('.dataTables_paginate').hide();
        //    }
        //}
    });
}


//function Html2CSV(tableId, filename, alinkButtonId) {
//    var array = [];
//    var headers = [];
//    var arrayItem = [];
//    var csvData = new Array();
//    $('#' + tableId + ' th').each(function (index, item) {
//        headers[index] = '"' + $(item).html() + '"';
//    });
//    csvData.push(headers);

//    $('#' + tableId + ' tr').has('td').each(function () {
//        array = [];
//        arrayItem = [];
//        $('td', $(this)).each(function (index, item) {
//            arrayItem[index] = '"' + $(item).html() + '"';
//        });
//        array.push(arrayItem);
//        csvData.push(array);
//    });


//    var fileName = filename + '.csv';
//    var buffer = csvData.join("\n");
//    var blob = new Blob([buffer], {
//        "type": "text/csv;charset=utf8;"
//    });
//    var link = document.getElementById(alinkButtonId);

//    if (link.download !== undefined) { // feature detection
//        // Browsers that support HTML5 download attribute
//        link.setAttribute("href", window.URL.createObjectURL(blob));
//        link.setAttribute("download", fileName);
//    }
//    else if (navigator.msSaveBlob) { // IE 10+
//        link.setAttribute("href", "#");
//        link.addEventListener("click", function (event) {
//            navigator.msSaveBlob(blob, fileName);
//        }, false);
//    }
//    else {
//        // it needs to implement server side export
//        link.setAttribute("href", "http://www.example.com/export");
//    }
//}


function ProjectAdded(ele, id, text) {
    $("#ProjectName").tokenInput("add", { id: decodeURIComponent(text), name: decodeURIComponent(text) });
    $("#ProjectName").val(id);
}

function ProjectRemoved(ele) {

    $("#ProjectName").tokenInput("clear");
    $("#ProjectName").val("");
}

function SerialNumberAdded(ele, id, text) {
    $("#SerialNumber").tokenInput("add", { id: decodeURIComponent(text), name: decodeURIComponent(text) });
    $("#SerialNumber").val(id);
}

function SerialNumberRemoved(ele) {

    $("#SerialNumber").tokenInput("clear");
    $("#SerialNumber").val("");
}
