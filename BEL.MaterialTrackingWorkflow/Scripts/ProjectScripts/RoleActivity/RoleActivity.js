var GetRoleActivityUrl = "/RoleActivity/GetRoleActivityByRoleId";
var UpdateRoleActivityUrl = "/RoleActivity/UpdateRoleActivity";
var AddNewRoleUrl = "/RoleActivity/AddNewRole";


jQuery(document).ready(function () {
    $('#divNoOptionFound').hide();
    $('#cmbMainMenu').select2();

    GetRoleActivityByRoleId();

    $('.collapse.in').prev('.panel-heading').addClass('active');
    $('.collapse.in').prev('.panel-headings').addClass('active');
    jQuery(document)
        .on('show.bs.collapse', '#bs-collapse', function (a) {
            $(a.target).prev('.panel-heading').addClass('active');
            $(a.target).prev('.panel-headings').addClass('active');
            
        })
        .on('hide.bs.collapse', '#bs-collapse', function (a) {
            $(a.target).prev('.panel-heading').removeClass('active');
            $(a.target).prev('.panel-headings').removeClass('active');
        });



    var searchTerm, panelContainerId;
    // Create a new contains that is case insensitive
    $.expr[':'].containsCaseInsensitive = function (n, i, m) {
        return jQuery(n).text().toUpperCase().indexOf(m[3].toUpperCase()) >= 0;
    };


    jQuery(document).on('change keyup paste click', '#accordion_search_bar', function () {
        searchTerm = $(this).val();
        var allChecked = true;
        $('#divNoOptionFound').hide();
        $('#divSelectAllOption').show();
        if ($('#bs-collapse > .panel').is(':visible')) {
            $('#bs-collapse > .panel').each(function () {
                panelContainerId = '#' + $(this).attr('id');
                $(panelContainerId + ':not(:containsCaseInsensitive(' + searchTerm + '))').hide();
                $(panelContainerId + ':containsCaseInsensitive(' + searchTerm + ')').show();
            });
        }
        else {
            $('#divSelectAllOption').hide();
            $('#divNoOptionFound').show();
        }
        if (searchTerm == '' || searchTerm == undefined) {
            $('#bs-collapse > .panel').show();
            $('#divNoOptionFound').hide();
            $('#divSelectAllOption').show();
            checkAllParetntCheckbox();
        }
    });

    jQuery(document).on("click", ".parentChks", function (e) {
        if (this.checked == false) {
            $('#bs-collapse > .panel').each(function () {
                if (searchTerm != '' && searchTerm != undefined) {
                    panelContainerId = '#' + $(this).attr('id');
                    $(panelContainerId + ':containsCaseInsensitive(' + searchTerm + ')').find('input:checkbox[save="save"]').attr('checked', false);
                }
                else {
                    jQuery(document).find('input:checkbox[save="save"]').attr('checked', false);
                }
            });
        }
        else if (this.checked == true) {
            $('#bs-collapse > .panel').each(function () {
                if (searchTerm != '' && searchTerm != undefined) {
                    panelContainerId = '#' + $(this).attr('id');
                    $(panelContainerId + ':containsCaseInsensitive(' + searchTerm + ')').find('input:checkbox[save="save"]').attr('checked', true);
                }
                else {
                    jQuery(document).find('input:checkbox[save="save"]').attr('checked', true);
                }
            });
        }
    });


    jQuery(document).on("click", ".parentChk", function (e) {
        var allChecked = true;
        var dataAttr = jQuery(this).closest('input:checkbox').attr('data');
        if (jQuery(this).closest("input:checkbox").attr('checked')) {
            jQuery("input:checkbox[data='" + dataAttr + "C']").attr('checked', true);          
        }
        else {
            jQuery("input:checkbox[data='" + dataAttr + "C']").attr('checked', false);           
        }
        checkAllParetntCheckbox();
    });
    jQuery(document).on("click", ".childCheck", function (e) {
        var allChecked = true;
        var dataAttr = jQuery(this).closest('input:checkbox').attr('data').toString().substring(0, jQuery(this).closest('input:checkbox').attr('data').toString().length - 1);

        jQuery("input:checkbox[data='" + dataAttr + "C']").each(function () {
            if (this.checked == false) {
                allChecked = false;
                jQuery("input:checkbox[data='" + dataAttr + "']").attr('checked', false);           
            }
        })
        if (allChecked == true) {
            jQuery("input:checkbox[data='" + dataAttr + "']").attr('checked', true);            
        }
        checkAllParetntCheckbox();
    });
    jQuery(document).on('change', '#cmbRole', function () { GetRoleActivityByRoleId(); });

    jQuery(document).on('click', '#btnSave', function () { UpdateRoleActivity(); });
    jQuery('#cmbRole').focus();

    jQuery('#btnAddRole').on('click', function () {
        jQuery('#cmbRole').removeClass('input-validation-error');
        jQuery('#txtRoleName').removeClass('input-validation-error');
        if (jQuery('#btnAddRole').val() == "Save Role") {
            // jQuery('#btnCancel').addClass('hidden');

            var roleName = jQuery('#txtRoleName').val();
            if (roleName != "") {
                var attachmsg = 'Are you sure you want to Save "' + roleName + '" Role?';

                ConfirmationDailog({
                    title: "Confirm", message: attachmsg, okCallback: function (data) {
                        ShowWaitDialog();
                        AjaxCall({
                            url: AddNewRoleUrl, postData: JSON.stringify({ roleName: roleName }), httpmethod: 'POST', calldatatype: 'JSON', contentType: 'application/json; charset=utf-8',
                            sucesscallbackfunction: function (data) {
                                if (data.IsSucceed == true) {
                                    HideWaitDialog();
                                    AlertModal("Success", data.Messages[0], false, onSuccess);
                                    jQuery('#RoleMappingGrid').show();
                                    jQuery('.roleGroup').show();
                                }
                                else {
                                    AlertModal("Error", data.Messages[0], false);
                                    jQuery('#txtRoleName').val("");
                                }
                            }
                        });
                    }
                });
            }
            else {
                jQuery('#txtRoleName').addClass('input-validation-error');
            }

        }
        else {
            jQuery('#cmbRole').removeClass('input-validation-error');
            jQuery('#txtRoleName').removeClass('input-validation-error');
            jQuery('#txtRoleName').removeClass('hidden');
            jQuery('#btnAddRole').attr("value", "Save Role");
            jQuery('#RoleMappingGrid').hide();
            jQuery('.roleGroup').hide();
            jQuery('#btnCancel').removeClass('hidden');
        }
    });

    jQuery('#btnDeleteRole').on('click', function () {
        jQuery('#cmbRole').removeClass('input-validation-error');
        jQuery('#txtRoleName').removeClass('input-validation-error');
        var Id = jQuery('#cmbRole option:selected').val();
        var roleName = jQuery('#cmbRole option:selected').text().toLowerCase().trim();
        if (Id == "") {
            jQuery('#cmbRole').addClass('input-validation-error');
        }
        if (Id > 0) {

            if (roleName == 'admin' || roleName == 'hod' || roleName == 'recipient 1' || roleName == 'recipient 2' || roleName == 'tester') {
                AlertModal("Validation", "You cannot delete this role.", false);
            }
            else {
                ConfirmationDailog({
                    title: "Remove", message: 'Are you sure you want to delete selected "' + jQuery('#cmbRole option:selected').text() + '" Role?', id: Id, url: "/RoleActivity/DeleteRoleById", okCallback: function (id, data) {
                        if (data.IsSucceed == true) {
                            HideWaitDialog();
                            AlertModal("Success", data.Messages[0], false, onSuccess);
                        }
                        else {
                            AlertModal("Error", data.Messages[0], false);
                        }
                    }
                });
            }
        }
    });

    jQuery('#btnCancel').on('click', function () {
        jQuery('#txtRoleName').val("");
        jQuery('#txtRoleName').addClass('hidden');
        jQuery('#btnAddRole').attr("value", "Add New Role");
        jQuery('#RoleMappingGrid').hide();
        jQuery('.roleGroup').show();
        jQuery('#btnCancel').addClass('hidden');
        jQuery('#cmbRole').val("");
    });


});

function changeAccordianContentBasedOnChildren() {
    var parentConteainerId;
    if ($('#bs-collapse > .panel').is(":Visible")) {
        $('#bs-collapse > .panel').each(function () {
            parentConteainerId = '#' + $(this).attr('id');
            if (jQuery(parentConteainerId).find('div[id=Collapse_' + $(this).attr('id') + ']').find('div').length == 0) {           
                jQuery(parentConteainerId).find('div[id=Collapse_' + $(this).attr('id') + ']').prev('.panel-heading').removeClass('panel-heading').addClass('panel-headings');
            }
        });
    }
}

function checkAllParetntCheckbox() {
    var allChecked = true;
    jQuery(document).find("input:checkbox[class='parentChk']").each(function () {
        if (this.checked == false) {
            allChecked = false;
            jQuery(document).find(".parentChks").attr('checked', false);
        }
    });
    if (allChecked == true) {
        jQuery(document).find(".parentChks").attr('checked', true);
    }
}

function onSuccess() {
    window.location = window.location.href;
    jQuery('#txtRoleName').hide();
    jQuery('#btnAddRole').attr("value", "Add New Role");
}

function GetRoleActivityByRoleId() {

    if (jQuery('#cmbRole option:selected').val() != "" && jQuery('#cmbRole option:selected').val() != undefined) {
        ShowWaitDialog();
        var RoleName = jQuery('#cmbRole option:selected').text().trim();
        var data = {
            roleName: RoleName
        }
        AjaxCall({
            url: GetRoleActivityUrl, postData: data, httpmethod: 'GET', calldatatype: 'HTML', contentType: 'application/json; charset=utf-8',
            sucesscallbackfunction: function (data) {
                jQuery('#RoleMappingGrid').show();
                jQuery('#RoleActivityGrid').html(data);
                changeAccordianContentBasedOnChildren();
                HideWaitDialog();
            }
        });
    }
    else {
        jQuery('#RoleMappingGrid').hide();
    }
}

function UpdateRoleActivity() {
    if (jQuery('#cmbRole option:selected').val() != "" && jQuery('#cmbRole option:selected').val() != undefined) {
        var attachmsg = 'Are you sure you want to Save Role Activity Screen Matrix for "' + jQuery('#cmbRole option:selected').text() + '" Role?';
        ConfirmationDailog({
            title: "Confirm", message: attachmsg, okCallback: function (data) {
                var RoleActivityArray = new Array();
                var roleId = jQuery('#cmbRole option:selected').val();
                var roleName = jQuery('#cmbRole option:selected').text();
                jQuery(document).find('input:checkbox[save="save"]').each(function (i, e) {

                    var IdValueData = {
                        ID: jQuery(e).attr('roleActivityId'),
                        RoleId: roleId,
                        ParentMenuName: jQuery(e).attr('parentMenuName'),
                        ChildMenuName: jQuery(e).attr('childMenuName'),
                        IsAuthorized: (jQuery(e).attr('checked')) ? true : false,
                        RoleName: roleName
                    };

                    RoleActivityArray.push(IdValueData);
                });

                AjaxCall({
                    url: UpdateRoleActivityUrl, postData: JSON.stringify({ lstRoleActivity: RoleActivityArray }), httpmethod: 'POST', calldatatype: 'JSON', contentType: 'application/json; charset=utf-8',
                    sucesscallbackfunction: function (data) {
                        if (data.IsSucceed) {
                            AlertModal(("Success"), data.Messages[0], false, function () { window.location = window.location.href; });
                        }
                    }
                });
            }
        });
    }
    else {
        jQuery('#cmbRole').addClass('input-validation-error');
    }
}
