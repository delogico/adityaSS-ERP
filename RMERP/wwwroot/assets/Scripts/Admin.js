
var route = siteUrl;
$("document").ready(function () {   
   // listAdminUsers();
});

function onAddClick(id) {
    debugger;
    var Url = siteUrl + "Admin/AddEditAdmin?id=" + id;
    $.ajax({
        type: "GET",
        url: Url,
        success: function (data) {
            $('#divAddEdit').html(data);
            $('#add_user').modal('show');
        }
    })
};
function onSaveClick() {
    debugger;
    var Url = siteUrl + "Admin/saveEditAdmin";    
    var form_data = $("#frmAddEdit").serialize();
    $.ajax({
        type: "POST",
        data: form_data,
        url: Url,
        success: function (data) {
            if (data.length > 0) {
                alert(data);
            }
            else {
                ClosePopup();               
            }
        },
            Error: function() {
                alert("errror");
        }
        
    })
};
//function ResetFormClientValidation() {
//    //----To Resolve issue in fire model level validation on client side----  
//    var form = $("#frmAddEdit")
//        .removeData("validator") /* added by the raw jquery.validate plugin */
//        .removeData("unobtrusiveValidation");  /* added by the jquery unobtrusive plugin*/
//    $.validator.unobtrusive.parse(form);
//    //---------------------------------------------------------//
//};
function ClosePopup() {
    debugger;
    $('#add_user').modal('hide');
    listAdminUsers();
};
function listAdminUsers() {
    $.ajax({
        type: "GET",
        url: siteUrl + "Admin/listAdminUsers",
        success: function (data) {
          //  $('#divAdminUsersList').html(data);
        }
    })
};
function onDeleteClick(id) {
    debugger;
    var Url = siteUrl + "Admin/deleteAdminUser?id="+id;
    $.ajax({
        type: "GET",
        url: Url,
        success: function (data) {
           // listAdminUsers();          
        }
    })
};
function onDeleteShow(id) {
    debugger;
    $('#delete_user').modal('show');
    $('#dltAdminBtn').val(id);
};

$(document).on("click", "#dltAdminBtn", function () {
    debugger;
    var AdmID = $('#dltAdminBtn').val();
    var Url = siteUrl + "Admin/deleteAdminUser?id=" + AdmID;
    $.ajax({
        type: "GET",
        url: Url,
        success: function (data) {
           // listAdminUsers();
        }
    })
});
