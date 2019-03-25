
$(document).ready(function () { 
    $('.dateCalender').datepicker({
        // format: 'dd-MM-yyyy',
        autoclose: true
    });
    $('.dateCalenderClose').datepicker({
        // format: 'dd-MM-yyyy',
        autoclose: true
    });



    $('#opener').click(function () {
        jobId = $(this).attr('jobId');
        return false;
    });
});

function callGetJob(jobId) {
    $.ajax({
        type: 'POST',
        url: '/Jobs/GetJobs?id=' + jobId + '',
        success: function (data) {
            var targetcompletiondate = new Date(data.data.target_completion_date);
            var ttargerdate = ('0' + (targetcompletiondate.getMonth() + 1)).slice(-2) + '/' + targetcompletiondate.getDate() + '/' + targetcompletiondate.getFullYear();
            var scheduleddate = new Date(data.data.scheduled_date);
            var tscheduledate = ('0' + (scheduleddate.getMonth() + 1)).slice(-2) + '/' + scheduleddate.getDate() + '/' + scheduleddate.getFullYear();
            $('#jobId').val(data.data.jobId);
            $('#job_number').val(data.data.job_number);
            $('#job_name').val(data.data.job_name);
            $('#description').val(data.data.description);
            $('#target_completion_date').val(ttargerdate);
            $('#status').val(data.data.status);
            $('#revenue').val(data.data.revenue);
            $('#sales_person').val(data.data.sales_person);
            $('#special_Notes').val(data.data.special_Notes);
            $('#EmployeeId').val(data.data.employeeId);

            var scheduledDate = data.data.scheduled_date;

            if (scheduledDate == "0001-01-01T00:00:00") {
                $("#scheduled_date").val();

            }
            else {
                if (scheduledDate !== "" && scheduledDate !== undefined) {
                    var scheduledDateParameters = scheduledDate.split('T')[0];
                    scheduledDateParameters = scheduledDateParameters.split('-');
                    scheduledDate = scheduledDateParameters[1] + "/" + scheduledDateParameters[2] + "/" + scheduledDateParameters[0];
                    $("#scheduled_date").val(scheduledDate);
                }
            }
        }
    });

}
 

$("input").attr('disabled', 'disabled');
function showJobDetails(jobId) {
    $("input,select").removeAttr('disabled');
    $("#editChangesdiv").addClass('hidden');
    $("#saveChangesdiv").removeClass('hidden');
    callGetJob(jobId);   
}

function saveChanges() {
    //sucess pe ye likhna hai
    // $("input").attr('disabled', 'disabled');
    $("#editChangesdiv").removeClass('hidden');
    $("#saveChangesdiv").addClass('hidden');
    let isValid = true;
    var formValue = $("#addUpdateJobForm11").serialize();
    if (isValid) {
        $('#divAddPopup [id^="invalid_"]').hide();
        if (setJobDatess()) {
          
            saveOrUpdateJobs(formValue);
        }
    }
}

function setJobDatess() {
    var targetCompleteDate = $("#target_completion_date").val();
    if (targetCompleteDate != null && targetCompleteDate != undefined && targetCompleteDate != "") {
        var dateParameters = targetCompleteDate.split('/');
        var month = dateParameters[0];
        var day = dateParameters[1];
        var year = dateParameters[2];
        $("#TargetCompletionDate").val(year + "-" + month + "-" + day);
        $("#TargetCompletionDate").attr('value', year + "-" + month + "-" + day);
        return true;
    }
    else {
        return false;
    }
}

function saveOrUpdateJobs(formValue) {
    //$.noConflict();
    $.ajax({
        type: 'POST',
        url: "/Jobs/AddJobs",
        async: false,
        data: formValue,
        //success: function (result) {
        //    if (result.success) {
        //        jobList();
        //        $('#closePop').click();
        //        window.location.reload();

        //    }
        //}
        success: function (result) {
            if (result.success) {
                $('.close').click();
                $.notify({
                    title: result.title,
                    message: result.message,
                },
                    {
                        type: result.type,
                        allow_dismiss: false,
                        showProgressbar: true,
                        delay: '100',
                        newest_on_top: true,
                    });
            }

            setTimeout(() => {
                window.location.reload();
            }, 2000);
        }
    });
}

function Dialogueclose() {
    $(".ui-dialog-titlebar-close").trigger("click");
}



