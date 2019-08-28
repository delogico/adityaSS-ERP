function onClientSelectionClick(WAG_Id, FRM_Id, action, controller) {  
    var Url = siteUrl + "Reports/_ClientsSelection?WAG_Id=" + WAG_Id + "&FRM_Id=" + FRM_Id + "&action=" + action + "&controller=" + controller;
    $.ajax({
        type: "GET",
        url: Url,
        success: function (data) {
            $('#ModalSelection').html(data);
            $('#myModalSelection').modal('show');
        }
    })
};
