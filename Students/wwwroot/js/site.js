
//generic function to edit any record
var urldomain = "https://localhost:44392";
function EditRecord(id, Url, ModalId, FormId) {
    $.ajax({
        type: 'GET',
        url: Url,
        data: {
            Id: id
        },
        success: function (response) {
            $(FormId).html(response);
            $(ModalId).modal("show");
            if ($('#DateOfBirth').length > 0) {
                var dob = new Date($("#DateOfBirth").val());
                var today = new Date();
                var age = Math.floor((today - dob) / (365.25 * 24 * 60 * 60 * 1000));
                if (age > 18) {
                    $("#CNICNum").show();
                }
                else {
                    $("#CNICNum").hide();
                }
            }
            if ($('#StudentId').length > 0) {
                var StudentId = $("#StudentId").val();
                $("#StudentId").val(StudentId);
            }
        },
        error: function (err) {
            StudentAcademy.Tools.alertbox("Oops", "Some Error Occurred", "warning");
        }
    })
}
function DeleteRecord(id, URL) {
    $("#ConfirmationModalForDeletion").modal('show');
    $("#ConfirmationButtonForDeletion").on("click", function () {
        $.ajax({
            type: "DELETE",
            dataType: "Json",
            url: URL,
            data: { Id: id },
            success: function () {
                window.location.reload();
            },
            error: function (ErrorObject, opts, error) {
                $("#ConfirmationModalForDeletion").modal('hide');
                StudentAcademy.Tools.alertbox(ErrorObject.status, ErrorObject.responseText, "error");
            }
        });
    });
}
function HandleErrorOnCompletionOfPostRequestFromForm(response) {
    console.log(response);
    if (response.status != 200) {
        StudentAcademy.Tools.alertbox(response.status, response.responseText, "error");
    }
    else {
        window.location.reload();
    }

}

function setContentHeight() {
    // reset height
    $('.right_col').css('min-height', $(window).height());
    var bodyHeight = $('body').outerHeight(),
        footerHeight = $('body').hasClass('footer_fixed') ? -10 : $('footer').height(),
        leftColHeight = $('.left_col').eq(1).height() + $('.sidebar-footer').height(),
        contentHeight = bodyHeight < leftColHeight ? leftColHeight : bodyHeight;

    // normalize content
    contentHeight -= $('.nav_menu').height() + footerHeight;

    $('.right_col').css('min-height', contentHeight);
};
function openUpMenu() {
    $('#sidebar-menu').find('li').removeClass('active active-sm');
    $('#sidebar-menu').find('li ul').slideUp();
}

$(document).ready(function () {
    $('.modal').on('shown.bs.modal', function () {
        $('#first').focus();
        $('.first').focus();
    });
    $('#first').focus();
    $('.first').focus();

    var userinfo = JSON.parse($.session.get("UserInfo"));


    $(".datepicker").datepicker({
        //dateFormat: 'dd/mm/yy'
        //format: 'dd/mm/yyyy',
        //todayHighlight: 'TRUE',
        //autoclose: true,
    });


    $('#CreateMember').on('show.bs.modal', function (event) {
        $(".datepicker").datepicker({
            //    //dateFormat: 'dd/mm/yy'
            //    //format: 'dd/mm/yyyy',
            //    //todayHighlight: 'TRUE',
            //    //autoclose: true,
        });
        $(".datepicker").attr("autocomplete", "off");
        $("#DateOfBirth").on("change", function (e) {
            //Calculate Age 
            var dob = new Date(this.value);
            var today = new Date();
            var age = Math.floor((today - dob) / (365.25 * 24 * 60 * 60 * 1000));
            if (age > 18) {
                $("#CNICNum").show();
            }
            else if (age < 6) {
                alert("Age should be greater than 5 years");
            }
            else {
                $("#CNICNum").hide();
            }
        });
    });

    //Jquery Validation
    $("#StudentForm").validate({
        rules: {
            FirstName: {
                required: true
            },
            LastName: {
                required: true
            },
            Gender: {
                required: true
            },
            Address: {
                required: true
            },
            ContactNo: {
                required: true,
                PhoneNo: true
            },
            Email: {
                CheckEmail: true
            },
            DateOfBirth: {
                required: true
            },
            EmergencyContactNumber: {
                required: true,
                PhoneNo: true
            },
            ParentId: {
                required: true
            },
        }
    });

   
    jQuery.validator.addMethod("CheckAge", function () {
        var check = true;
        var dob = new Date($("#DateOfBirth").val());
        var today = new Date();
        var age = Math.floor((today - dob) / (365.25 * 24 * 60 * 60 * 1000));
        if (age < 18 && $("#ParentOrGuardian_Name").val().length <= 0) {
            $.validator.messages.CheckAge = 'This field is required';
            check = false;
        }
        return check;
    });
    $("#DateOfBirth").on("change", function () {
        var dateOfBirth = $('#DateOfBirth').val();
        var dob = new Date(dateOfBirth);
        var today = new Date();
        var age = Math.floor((today - dob) / (365.25 * 24 * 60 * 60 * 1000));
        console.log(age);
        $('#Age').val(age);
    });
    jQuery.validator.addMethod("CheckEmail", function (value, element) {
        if (value != "") {
            if (/(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@[*[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+]*/.test(value)) {
                return true;
            } else {
                $.validator.messages.CheckEmail = 'Invalid Email';
                return false;
            }
        }
        else {
            return true;
        }
    });

    jQuery.validator.addMethod("PhoneNo", function (value, element) {
        if (/^((\+92)?(0092)?(92)?(0)?)(3)([0-9]{9})$/gm.test(value)) {
            return true;
        } else {
            return false;
        };
    }, "Invalid phone number");
    jQuery.validator.addMethod("NumberValidator", function (value, element) {
        if (parseFloat(value) > 0) {
            return true;
        } else {
            return false;
        };
    }, "This Field is Required");

    jQuery.validator.addMethod("CheckCNIC", function (value, element) {
        if (value != "") {
            if (/\d{5}-\d{7}-\d/.test(value)) {
                return true;
            } else {
                return false;
            }
        }
        else {
            return true;
        }
    }, "Invalid CNIC No");





    $("#ParentCreateForm").validate({
        rules: {
            FirstName: {
                required: true
            },
            Phone: {
                required: true,
                NumberValidator: true
            },
            NumberOfClasses: {
                required: true,
                NumberValidator: true
            },
            DaysLimit: {
                required: true,
                NumberValidator: true
            }
        }
    });

    
    // check active menu
    $('#sidebar-menu').find('a[href="' + CURRENT_URL + '"]').parent('li').parent('ul').collapse('toggle');
    $('#sidebar-menu').find('a[href="' + CURRENT_URL + '"]').addClass("currentpage");

});

function ValidCNIC(CNICNo) {
    var idToTest = CNICNo.value,
        myRegExp = new RegExp(/\d{5}-\d{7}-\d/);

    if (myRegExp.test(idToTest)) {
        $("#validcnic").remove();
    }
    else {
        if ($("#validcnic").length > 0) {
            $("#validcnic").remove();
        }
        $(".CNICNODiv").append("<label id='validcnic' class='error'>Enter Valid CNIC No</label>");
    }
}

function CustomerRemainingBal() {
    var rembal = $('#RemainingBalance').text();
    var amount = $('#PaymentAmount').val();
    var Trembal = rembal - amount;
    $('#RemainingBal').text(Trembal);
}