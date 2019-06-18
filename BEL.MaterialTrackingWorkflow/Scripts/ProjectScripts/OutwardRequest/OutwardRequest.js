var hidden = null;
var tm = null;
var sn = null;
var itemId = null;
var la = null;
var pc = null;
var rc = null;
var hiddenInwardidforchange = null;
var projectNamehidden = null;
var currentUserAlise = null;
$(document).ready(function () {
    $('span#negativeSign').hide();
    $(".sectionDetailType").change();
    hidden = $("#InwardIdhidden").val();
    hiddenInwardidforchange = $("#InwardIdhidden").val();
    tm = $("#TypeofMaterial").val();
    sn = $("#RecipientName").val();
    la = $("#LocationAddress").val();
    pc = $("#Particulars").val();
    rc = $("#Recipient2Particulars").val();
    currentUserAlise = $("#ProposedByAlise").val();

    projectNamehidden = $("#ProjectName").val();
    itemId = $('#ListDetails_0__ItemId').val();
    BindUserTags("");

    if (jQuery("input:hidden[id='Action']").length > 0) {

        var value = jQuery("input:hidden[id='Action']").val();
        var IsReworkRequired = $('#IsReworkRequired').val() == "False" ? false : true;

        if (value == "Select") {
            $('ul.header-nav > li > a:contains("Confirm")').html('<i class="fa fa-save"></i> Confirm');
            $('ul.header-nav > li > a:contains("Rework")').html('<i class="fa fa-save"></i> Confirm');
            $('ul.header-nav > li > a:contains("Confirm")').attr('data-original-title', 'Request will send to next approver.')
        }
        else if (value == "Approve") {

            $('ul.header-nav > li > a:contains("Rework")').html('<i class="fa fa-save"></i> Confirm')
            $('ul.header-nav > li > a:contains("Confirm")')
                .attr('data-original-title', 'Request will send to next approver.')
        } else if (value == "Rework") {

            $('ul.header-nav > li > a:contains("Confirm")').html('<i class="fa fa-share"></i> Rework');
            $('ul.header-nav > li > a:contains("Rework")')
                .attr('data-original-title', 'Request will send back to Creator for rework.')
        }

        if (!IsReworkRequired) {
            $('ul.header-nav > li > a:contains("Rework")').html('<i class="fa fa-save"></i> Confirm')
            $('ul.header-nav > li > a:contains("Confirm")')
                .attr('data-original-title', 'Request will send to next approver.')
        }
    };

    $(document).on("change", "select#Action",
        function () {
            var IsReworkRequired = $('#IsReworkRequired').val() == "False" ? false : true;
            var value = $(this).find("option:selected").text();
            if (value == '') {
                value = jQuery("input:hidden[id='Action']").val()
            }

            if (value == "Select") {

                $('ul.header-nav > li > a:contains("Rework")').html('<i class="fa fa-save"></i> Confirm')
                $('ul.header-nav > li > a:contains("Confirm")').attr('data-original-title',
                    'Request will move to the next approval level.')
            } else if (value == "Approve") {

                $('ul.header-nav > li > a:contains("Rework")').html('<i class="fa fa-save"></i> Confirm')
                $('ul.header-nav > li > a:contains("Confirm")').attr('data-original-title',
                    'Request will move to the next approval level.')
            } else if (value == "Rework") {

                $('ul.header-nav > li > a:contains("Confirm")').html('<i class="fa fa-share"></i> Rework');
                $('ul.header-nav > li > a:contains("Rework")').attr('data-original-title',
                    'Request will send back to Creator for rework.')
            }
            if (!IsReworkRequired) {
                $('ul.header-nav > li > a:contains("Rework")').html('<i class="fa fa-save"></i> Confirm')
                $('ul.header-nav > li > a:contains("Confirm")')
                    .attr('data-original-title', 'Request will send to next approver.')
            }
        }).change();

    //Attachment section for Assembled
    //Used for assembled tester section
    if ($("#TesterAttachment").length != 0) {
        BindFileUploadControl({
            ElementId: 'AttachmentAssTester',
            Params: {},
            Url: "UploadFile",
            AllowedExtensions: [],
            MultipleFiles: true,
            CallBack: "OnFileUploadedTesterAssAttachment"
        });
        uploadedFiles1 = BindFileList("TesterAttachment", "AttachmentAssTester");
    }

    //Used for assembled recepient section
    if ($("#RecipientAttachment").length != 0) {
        BindFileUploadControl({
            ElementId: 'AttachmentRecipientResult',
            Params: {},
            Url: "UploadFile",
            AllowedExtensions: [],
            MultipleFiles: true,
            CallBack: "OnFileUploadedRecipientAttachment"
        });
        uploadedFiles1 = BindFileList("RecipientAttachment", "AttachmentRecipientResult");
    }

    //Attachment section for Single
    //Used for assembled Single tester section
    if ($("#TesterSingleAttachment").length != 0) {
        BindFileUploadControl({
            ElementId: 'AttachmentSingleTester',
            Params: {},
            Url: "UploadFile",
            AllowedExtensions: [],
            MultipleFiles: true,
            CallBack: "OnFileUploadedTesterSingleAttachment"
        });
        uploadedFiles1 = BindFileList("TesterSingleAttachment", "AttachmentSingleTester");
    }

    //Used for single recepient section
    if ($("#RecipientSingleAttachment").length != 0) {
        BindFileUploadControl({
            ElementId: 'AttachmentRecipientSingleResult',
            Params: {},
            Url: "UploadFile",
            AllowedExtensions: [],
            MultipleFiles: true,
            CallBack: "OnFileUploadedRecipientSingleAttachment"
        });
        uploadedFiles1 = BindFileList("RecipientSingleAttachment", "AttachmentRecipientSingleResult");
    }

    //$("#ProjectName").on("change", function () {
    //    if ($("#ProjectName").val() != '') {
    //        // var projectName = $("#ProjectName option:selected").val();
    //        var projectName = $("#ProjectName :selected").text();
    //        if (hidden != null && projectName != "Select")
    //        { GetInwardRequests(projectName, itemId, hidden); }
    //        else if (projectNamehidden == projectName && projectName != "Select")
    //        {
    //            GetInwardRequests(projectName, itemId, hiddenInwardidforchange);
    //        }
    //        else {
    //            GetInwardRequests(projectName, itemId, null);
    //        }
    //    } else {
    //        $("#InwardId").val('');
    //        $("#ProjectName").val('');
    //        $('#RecipientName').val('');
    //        $("#TypeofMaterial").val('').change();
    //        $("#Particulars").val('');
    //        $('#LocationAddress').val('');
    //        $("#InwardId option").remove();
    //        $("#InwardId").append('<option>Select</option>');
    //        $("#InwardId").attr("data-original-title", "");
    //    }

    //}).change();

    $("#InwardId").on("change",
        function () {
            if ($("#InwardId").val() != '') {
                var inwardID = $("#InwardId option:selected").val();
                GetInwardRequestDetails(inwardID);
            } else {
                $("#InwardId").val('');
                $('#RecipientName').val('');
                $("#TypeofMaterial").val('').change();
                $("#Particulars").val('');
                $("#Recipient2Particulars").val('');
                $('#LocationAddress').val('');
            }
        }).change();


});


var onPlantadding = false, onPlantremoving = false;
function ProjectNameAdded(ele, id, text, items) {

    if (!onPlantremoving && !onPlantadding) {
        onPlantadding = true;
        $("#ProjectName").tokenInput("clear");
        $("#ProjectName").tokenInput("add", { id: decodeURIComponent(text), name: decodeURIComponent(text) });
       
        $("#ProjectName").val(id).change();
        var projectCode = '';
        if ($("#ProjectName").val() != '' && ele != null) {
            var projectName = $("#ProjectName").val();

            if (items != undefined) {
                var item = JSON.parse(items);
                projectCode = item.ProjectCode;
                $('#ProjectCode').val(projectCode);               
            }
            else if (items == undefined) {
                projectCode = $('#ProjectCode').val();                 
            }

            if (hidden != null) {
                GetInwardRequests(projectName, projectCode, itemId, hidden, currentUserAlise);
            }
            else if (projectNamehidden == projectName) {
                GetInwardRequests(projectName, projectCode, itemId, hiddenInwardidforchange, currentUserAlise);
            }
            else {
                GetInwardRequests(projectName, projectCode, itemId, null, currentUserAlise);
            }
        } else {
            $("#InwardId").val('');
            $("#ProjectName").val('');
            $('#ProjectCode').val('');
            $('#RecipientName').val('');
            $("#TypeofMaterial").val('').change();
            $("#Particulars").val('');
            $("#Recipient2Particulars").val('');
            $('#LocationAddress').val('');
            $("#InwardId option").remove();
            $("#InwardId").append('<option>Select</option>');
            $("#InwardId").attr("data-original-title", "");
        }
    }
    onPlantadding = false;
}
function ProjectNameRemoved(ele) {
    if (!onPlantadding && !onPlantremoving) {
        onPlantremoving = true;
        $("#ProjectName").tokenInput("clear");
        $("#ProjectName").val("");
        $("#InwardId").val('');
        $("#ProjectName").val('');
        $('#ProjectCode').val('');
        $('#RecipientName').val('');
        $("#TypeofMaterial").val('').change();
        $("#Particulars").val('');
        $("#Recipient2Particulars").val('');
        $('#LocationAddress').val('');
        $("#InwardId option").remove();
        $("#InwardId").append('<option>Select</option>');
        $("#InwardId").attr("data-original-title", "");
    }

    onPlantremoving = false;
}

var uploadedFiles = [], uploadedFiles1 = [], uploadedFiles2 = [], uploadedFiles3 = [];

//Attachment section for Assembled
function OnFileUploadedTesterAssAttachment(result) {
    uploadedFiles.push(result);
    $("#TesterAttachment").val(JSON.stringify(uploadedFiles)).blur();
}

function OnFileUploadedRecipientAttachment(result) {
    uploadedFiles2.push(result);
    $("#RecipientAttachment").val(JSON.stringify(uploadedFiles2)).blur();
}

//Attachment section for Single
function OnFileUploadedTesterSingleAttachment(result) {
    uploadedFiles1.push(result);
    $("#TesterSingleAttachment").val(JSON.stringify(uploadedFiles1)).blur();
}

function OnFileUploadedRecipientSingleAttachment(result) {
    uploadedFiles3.push(result);
    $("#RecipientSingleAttachment").val(JSON.stringify(uploadedFiles3)).blur();
}


function AttachmentAssTesterRemoveImage(ele) {
    var Id = $(ele).attr("data-id");
    var li = $(ele).parents("li.qq-upload-success");
    var itemIdx = li.index();
    ConfirmationDailog({
        title: "Remove", message: "Are you sure to remove file?", id: Id, url: "/OutwardRequest/RemoveUploadFile", okCallback: function (id, data) {
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
                    $("#TesterAttachment").val("").blur();
                } else {
                    $("#TesterAttachment").val(JSON.stringify(uploadedFiles)).blur();
                }
            }
        }
    });
}

function AttachmentSingleTesterRemoveImage(ele) {
    var Id = $(ele).attr("data-id");
    var li = $(ele).parents("li.qq-upload-success");
    var itemIdx = li.index();
    ConfirmationDailog({
        title: "Remove", message: "Are you sure to remove file?", id: Id, url: "/OutwardRequest/RemoveUploadFile", okCallback: function (id, data) {
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
                    $("#TesterSingleAttachment").val("").blur();
                } else {
                    $("#TesterSingleAttachment").val(JSON.stringify(uploadedFiles1)).blur();
                }
            }
        }
    });
}

function AttachmentRecipientResultRemoveImage(ele) {
    var Id = $(ele).attr("data-id");
    var li = $(ele).parents("li.qq-upload-success");
    var itemIdx = li.index();

    ConfirmationDailog({
        title: "Remove", message: "Are you sure to remove file?", id: Id, url: "/OutwardRequest/RemoveUploadFile", okCallback: function (id, data) {
            li.find(".qq-upload-status-text").remove();
            $('<span class="qq-upload-spinner"></span>').appendTo(li);
            li.removeClass("qq-upload-success");
            var idx = -1;
            var tmpList = [];
            $(uploadedFiles2).each(function (i, item) {
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
                uploadedFiles2 = tmpList;
                li.remove();
                if (uploadedFiles2.length == 0) {
                    $("#RecipientAttachment").val("").blur();
                } else {
                    $("#RecipientAttachment").val(JSON.stringify(uploadedFiles2)).blur();
                }
            }
        }
    });
}

function AttachmentRecipientSingleResultRemoveImage(ele) {
    var Id = $(ele).attr("data-id");
    var li = $(ele).parents("li.qq-upload-success");
    var itemIdx = li.index();

    ConfirmationDailog({
        title: "Remove", message: "Are you sure to remove file?", id: Id, url: "/OutwardRequest/RemoveUploadFile", okCallback: function (id, data) {
            li.find(".qq-upload-status-text").remove();
            $('<span class="qq-upload-spinner"></span>').appendTo(li);
            li.removeClass("qq-upload-success");
            var idx = -1;
            var tmpList = [];
            $(uploadedFiles3).each(function (i, item) {
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
                uploadedFiles3 = tmpList;
                li.remove();
                if (uploadedFiles3.length == 0) {
                    $("#RecipientSingleAttachment").val("").blur();
                } else {
                    $("#RecipientSingleAttachment").val(JSON.stringify(uploadedFiles3)).blur();
                }
            }
        }
    });
}



function GetInwardRequests(projectName, projectCode, itemId, selectedInwardID, currentUserAlise) {
    AjaxCall({
        url: "/OutwardRequest/GetInwardRequests",
        httpmethod: "Get",
        postData: {
            projectName: projectName, projectCode: projectCode, itemId: itemId, selectedInwardID: selectedInwardID, currentUserAlise: currentUserAlise
        },
        sucesscallbackfunction: function (result) {
            $("#InwardId").html("<option value=''>Select</option>").change();

            $(result).each(function (i, item) {

                var opt = $("<option/>");
                opt.text(item.InwardID);
                opt.attr("value", item.InwardID);
                opt.appendTo("#InwardId");

            });
            if (hidden != "") {
                $("#InwardId").find("option[value='" + hidden + "']").attr("selected", "selected");
                $("#TypeofMaterial").find("option[value='" + tm + "']").attr("selected", "selected");
                $('#RecipientName').val(sn);
                $('#LocationAddress').val(la);
                $('#Particulars').val(pc);
                $("#Recipient2Particulars").val(rc);
                $("#InwardIdhidden").val('');
                sn = null;
                hidden = null;
                tm = null;
                la = null;
                pc = null;
                rc = null;
            }
        }
    });

}

function GetInwardRequestDetails(inwardId) {
    AjaxCall({
        url: "/OutwardRequest/GetInwardRequestDetails",
        httpmethod: "Get",
        postData: { inwardID: inwardId },
        sucesscallbackfunction: function (result) {

            $('#RecipientName').val(result[0].SenderName);
            $('#Particulars').val(result[0].Particulars);
            $("#Recipient2Particulars").val(result[0].Recipient2Particulars);
            $('#LocationAddress').val(result[0].Location);
            var selectedValue = result[0].TypeofMaterial;
            if ($("#TypeofMaterial").find("option[value='" + selectedValue + "']").length > 0) {

                $("#TypeofMaterial").find("option[value='" + selectedValue + "']").attr("selected", "selected");
            } else {
                $("#TypeofMaterial").val('').change();
            }
        }
    });

}




