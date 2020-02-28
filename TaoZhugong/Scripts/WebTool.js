//於指定的Form顯示提示訊息
function ShowAlert(formId, level, msg, strongMsg = "") {
    var alertMsgDiv = $("#" + formId).find("#AlertMsg");

    var alertDiv = "<div class='alert alert-" + level + " alert-dismissible fade show' role='alert'>";
    var alertContent = "<strong>" + strongMsg + "</strong>  " + msg;
    var closeBtn = "<button type='button' class='close' data-dismiss='alert' aria-label='Close'> <span aria-hidden='true'>&times;</span> </button> </div>";

    alertMsgDiv.append(alertDiv + alertContent + closeBtn);

}