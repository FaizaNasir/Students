var StudentAcademy = {} || StudentAcademy;
StudentAcademy.student = {};
StudentAcademy.Tools = {
    alertbox: function (title, message, type) {
        swal({
            title: title,
            text: message,
            type: type,
            showCancelButton: false,
            confirmButtonText: "OK",
            closeOnconfirm: false,
        }, function (closeOnconfirm) {
            if (closeOnconfirm) {
                swal.close();
            }
        });
    },
    AjaxCall: function (RequestType, RequestUrl, RequestData, RequestDataType, SuccessCallBackFunction, ErrorCallBackFunction, asyncBool) {
        if (RequestType == null)
            RequestType = "GET";
        if (RequestUrl == null)
            return "";
        if (RequestData == null)
            RequestData = "html";
        if (asyncBool == undefined || asyncBool == null)
            asyncBool = true;
        $.ajax({
            type: RequestType,
            async: asyncBool,
            url: RequestUrl,
            data: RequestData,
            dataType: RequestDataType,
            success: function (result) {
                SuccessCallBackFunction(result, null, null);
            },
            error: function (ErrorObject, opts, error) {
                if (typeof ErrorCallBackFunction == null || typeof ErrorCallBackFunction == 'undefined' || ErrorCallBackFunction == null) {
                    StudentAcademy.Tools.alertbox(ErrorObject.status, ErrorObject.responseText, "error");
                }
                else {
                    ErrorCallBackFunction(ErrorObject.status, ErrorObject.responseText, "error");
                }
            }
        });
    },

}
