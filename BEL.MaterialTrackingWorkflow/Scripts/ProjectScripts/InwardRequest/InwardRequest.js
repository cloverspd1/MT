
$(document).ready(function () {
    $('span#negativeSign').hide();
    $(".sectionDetailType").change();
    $('#OutwardId').select2();
    $('#ProjectName').select2();
    var outwardid = null;
    BindUserTags("");
    outwardid = $("#OutwardIdOld").val();
    if (outwardid != null && outwardid != "") {
        var value = document.getElementById("InwardAgainstOutward").checked = true;
        if (value == true) {
            $("#InwOutwardDiv").show();
        }

    }
    else {
        var value = document.getElementById("NewInward").checked = true;
        if (value == true) {
            $("#InwOutwardDiv").hide();
            var a = $(".token-input-token").val();
            if (a != null && a != "") {
                $(".token-input-token").val() = "";
            }
        }
    }

    $('#ProjectName').on('change', function (e) { onProjectNameChange(e) });

    $('#OutwardId').on('change', function (e) { onOutwardIdChange(e) });

    GetAllProjectDetails();

    if (jQuery(document).find('#divrecipient1Section').find('#Status').val() != "Submitted") {
        GetOutwardRequests();
    }

    if ($("#Recipient1Attachment").length != 0) {
        BindFileUploadControl({
            ElementId: 'AttachmentInwardRecipient1', Params: {}, Url: "UploadFile",
            AllowedExtensions: [],
            MultipleFiles: true,
            CallBack: "OnFileUploadedInwardRecipient1"
        });
        uploadedFiles = BindFileList("Recipient1Attachment", "AttachmentInwardRecipient1");
    }
    if ($("#Recipient2Attachment").length != 0) {
        BindFileUploadControl({
            ElementId: 'AttachmentInwardRecipient2', Params: {}, Url: "UploadFile",
            AllowedExtensions: [],
            MultipleFiles: true,
            CallBack: "OnFileUploadedInwardRecipient2"
        });
        uploadedFiles1 = BindFileList("Recipient2Attachment", "AttachmentInwardRecipient2");
    }


    $("select#BUName").off("change").on("change", function () {
        var BusinessUnitvalue = $("#BUName option:selected").val();

        if (BusinessUnitvalue != undefined) {

            $("#ProductCategory").html("<option value=''>Select</option>");

            $(ProductCategoryList).each(function (i, item) {
                if (item.BusinessUnit == BusinessUnitvalue) {
                    var opt = $("<option/>");
                    opt.text(item.Title);
                    opt.attr("value", item.Value);
                    opt.appendTo("#ProductCategory");
                }
            });
        }
        var selectedValue = $("#ProductCategory").attr("data-selected");
        if ($("#ProductCategory").find("option[value='" + selectedValue + "']").length > 0) {
            $("#ProductCategory").val(selectedValue).change();
        } else {
            $("#ProductCategory").val('').change();
        }
    }).change();



    $("#MaterialHandedoverto").off("change").on("change", function () {

        var userid = $("#MaterialHandedoverto option:selected").val();
        var MaterialHandedovertoAliasNames = $("#MaterialHandedoverto option:selected").text();
        if (userid != '') {
            $("#MaterialHandedovertoUserID").val(userid);
            $("#MaterialHandedovertoAliasNames").val(MaterialHandedovertoAliasNames);
        }
        else {
            $("#MaterialHandedovertoUserID").val("");
            $("#MaterialHandedovertoAliasNames").val("");
        }
    }).change();

});


function onOutwardIdChange(evt) {
    var outwardvalue = $("#OutwardId option:selected").val();
    if (outwardvalue != undefined && outwardvalue != "") {
        var outwarddetailitem = JSON.parse($("#OutwardId option:selected").attr('outwarddetailsbyid'));
        $("#TypeofMaterial").val(outwarddetailitem.TypeofMaterial);
        $("#SenderName").val(outwarddetailitem.SenderName);
        $("#Location").val(outwarddetailitem.Location);
        $("#CourierNameHandDelivery").val(outwarddetailitem.CourierDetails);
        $("#AWBNameofdeliveryperson").val(outwarddetailitem.AWDNo);
        $("#Particulars").val(outwarddetailitem.Particulars);
        $("#ProjectName").val(outwarddetailitem.ProjectName);
    }
    else {
        //$("#TypeofMaterial").val("");
        //$("#SenderName").val("");
        //$("#Location").val("");
        //$("#CourierNameHandDelivery").val("");
        //$("#AWBNameofdeliveryperson").val("");
        //$("#Particulars").val("");
    }
}

$(function () {
    if ($(".token-input-token") == null && $(".token-input-token") == "") {
        $(".token-input-token").hide();
    }
});

$(function () {
    $("input[id='NewInward']").click(function () {
        if ($("#NewInward").is(":checked")) {
            $("#InwOutwardDiv").hide();
            $("#InwardAgainstOutward").prop('checked', false);
        }
    });
});

$(function () {
    $("input[id='InwardAgainstOutward']").click(function () {
        if ($("#InwardAgainstOutward").is(":checked")) {
            $("#InwOutwardDiv").show();
        }
    });
});


var uploadedFiles = [], uploadedFiles1 = [];
function OnFileUploadedInwardRecipient1(result) {
    uploadedFiles.push(result);
    $("#Recipient1Attachment").val(JSON.stringify(uploadedFiles)).blur();
}
function OnFileUploadedInwardRecipient2(result) {
    uploadedFiles1.push(result);
    $("#Recipient2Attachment").val(JSON.stringify(uploadedFiles1)).blur();
}

function AttachmentInwardRecipient1RemoveImage(ele) {
    var Id = $(ele).attr("data-id");
    var li = $(ele).parents("li.qq-upload-success");
    var itemIdx = li.index();

    ConfirmationDailog({
        title: "Remove", message: "Are you sure to remove file?", id: Id, url: "/InwardRequest/RemoveUploadFile", okCallback: function (id, data) {
            li.find(".qq-upload-status-text").remove();
            $('<span class="qq-upload-spinner"></span>').appendTo(li);
            li.removeClass("qq-upload-success");
            var idx = -1;
            var tmpList = [];
            $(uploadedFiles).each(function (i, item) {
                if (idx == -1 && item.FileId == id) {
                    idx = i;
                    if (item.Status == 0) {
                        item.Status = 2;
                        tmpList.push(item);
                    }
                } else {
                    tmpList.push(item);
                }
            });
            if (idx >= 0) {
                uploadedFiles = tmpList;
                li.remove();
                if (uploadedFiles.length == 0) {
                    $("#Recipient1Attachment").val("").blur();
                } else {
                    $("#Recipient1Attachment").val(JSON.stringify(uploadedFiles)).blur();
                }
            }
        }
    });
}

function AttachmentInwardRecipient2RemoveImage(ele) {
    var Id = $(ele).attr("data-id");
    var li = $(ele).parents("li.qq-upload-success");
    var itemIdx = li.index();

    ConfirmationDailog({
        title: "Remove", message: "Are you sure to remove file?", id: Id, url: "/InwardRequest/RemoveUploadFile", okCallback: function (id, data) {
            li.find(".qq-upload-status-text").remove();
            $('<span class="qq-upload-spinner"></span>').appendTo(li);
            li.removeClass("qq-upload-success");
            var idx = -1;
            var tmpList = [];
            $(uploadedFiles1).each(function (i, item) {
                if (idx == -1 && item.FileId == id) {
                    idx = i;
                    if (item.Status == 0) {
                        item.Status = 2;
                        tmpList.push(item);
                    }
                } else {
                    tmpList.push(item);
                }
            });
            if (idx >= 0) {
                uploadedFiles1 = tmpList;
                li.remove();
                if (uploadedFiles1.length == 0) {
                    $("#Recipient2Attachment").val("").blur();
                } else {
                    $("#Recipient2Attachment").val(JSON.stringify(uploadedFiles1)).blur();
                }
            }
        }
    });
}

var onPlant1adding = false, onPlant1removing = false;
function onProjectNameChange(evt) {
    //if (!onPlant1removing && !onPlant1adding) {
    onPlant1adding = true;
    var ProjectCodevalue = $("#ProjectName option:selected").attr('projectcode');
    if (ProjectCodevalue != undefined && ProjectCodevalue != "") {
        AjaxCall({
            url: "/InwardRequest/GetTesters?projectCode=" + ProjectCodevalue,
            httpmethod: "GET",
            sucesscallbackfunction: function (result) {
                $("#MaterialHandedoverto").html("<option value=''>Select</option>");                
                $(result).each(function (i, item) {
                    var selectedValue = $("#MaterialHandedoverto").attr("data-selected");
                    var opt = $("<option/>");
                    opt.text(item.AliasNames);
                    opt.attr("value", item.UserID);
                    if (selectedValue == item.AliasNames) {
                        opt.attr("selected", 'selected');
                        $("#MaterialHandedovertoUserID").val(item.UserID);
                        $("#MaterialHandedovertoAliasNames").val(item.AliasNames);
                    }
                    opt.appendTo("#MaterialHandedoverto");
                });
            }
        });
    }
    //   onPlant1adding = false;
    //}
}

function GetAllProjectDetails() {

    AjaxCall({
        url: "/InwardRequest/GetAllProjects",
        httpmethod: "GET",
        isAsync: false,
        sucesscallbackfunction: function (result) {
            $("#ProjectName").html("<option value=''>Select</option>");
            $(result).each(function (i, item) {
                var opt = $("<option/>");
                var itemname = item.ProjectCode + " - " + item.Title;
                opt.text(itemname);
                opt.attr("value", itemname);
                opt.attr("projectcode", item.ProjectCode);
                if (itemname == $("#ProjectName").attr("data-selected")) {
                    opt.attr("selected", 'selected');
                }
                opt.appendTo("#ProjectName");
            });

            $("#ProjectName").trigger("change");           
            var selectedItem = $("#ProjectName option:selected").text();
            if (selectedItem != '')
                $("#ProjectName").val(selectedItem);
        }
    });

}


//var onPlant1adding = false, onPlant1removing = false;
//function ProjectAdded(ele, id, text) {

//    if (!onPlant1removing && !onPlant1adding) {
//        onPlant1adding = true;
//        $("#ProjectName").tokenInput("clear");
//        $("#ProjectName").tokenInput("add", { id: decodeURIComponent(text), name: decodeURIComponent(text) });
//        $("#ProjectName").val(id).change();
//        //  ShowWaitDialog();
//        AjaxCall({
//            url: "/InwardRequest/GetTesters?ProjectName=" + id,
//            httpmethod: "GET",
//            sucesscallbackfunction: function (result) {
//                $("#MaterialHandedoverto").html("<option value=''>Select</option>");

//                $(result).each(function (i, item) {
//                    var selectedValue = $("#MaterialHandedoverto").attr("data-selected");

//                    var opt = $("<option/>");
//                    opt.text(item.AliasNames);
//                    opt.attr("value", item.UserID);
//                    if (selectedValue == item.AliasNames) {
//                        opt.attr("selected", 'selected');
//                        $("#MaterialHandedovertoUserID").val(item.UserID);
//                        $("#MaterialHandedovertoAliasNames").val(item.AliasNames);
//                    }
//                    opt.appendTo("#MaterialHandedoverto");
//                });
//            }
//        });
//    }
//    onPlant1adding = false;
//}
function ProjectRemoved(ele) {
    if (!onPlant1adding && !onPlant1removing) {
        var hidden = null;
        onPlant1removing = true;
        $("#ProjectName").tokenInput("clear");
        $("#MaterialHandedoverto option").remove();
        $("#MaterialHandedoverto").append('<option>Select</option>');
        $("#ProjectName").val("");
    }
    onPlant1removing = false;
}
function GetOutwardRequests() {

    AjaxCall({
        url: "/InwardRequest/GetOutwardRequests",
        httpmethod: "GET",
        isAsync: false,
        sucesscallbackfunction: function (result) {

            $("#OutwardId").html("<option value=''>Select</option>");

            $(result).each(function (i, item) {
                var opt = $("<option/>");
                opt.text(item.OutwardId);
                opt.attr("value", item.OutwardId);
                opt.attr("outwarddetailsbyid", JSON.stringify(item));
                if (item.OutwardId == $("#OutwardId").attr("data-selected")) {
                    opt.attr("selected", 'selected');
                }
                opt.appendTo("#OutwardId");

            });

            $("#OutwardId").trigger("change");
            var selectedItem = $("#OutwardId option:selected").val();
            if (selectedItem != '')
                $("#OutwardId").val(selectedItem);
        }
    });

}
