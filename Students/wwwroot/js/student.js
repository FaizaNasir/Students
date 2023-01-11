StudentAcademy.student = {
   
    GetParentDetail: function (id) {
        if (id > 0 == false) return;
        StudentAcademy.Tools.AjaxCall("GET", "/Parent/GetParentById/" + id, "", "Json",
            function (res, status, err) {
            },
            null,
            true);
    },
  
}