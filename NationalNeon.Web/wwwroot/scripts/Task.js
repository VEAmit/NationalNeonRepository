﻿
$(document).ready(function () {
    $('#opener').click(function () {
        TaskId = $(this).attr('TaskId');
        return false;
    });
});

function callGetTask(TaskId) {
    $.ajax({
        type: 'POST',
        url: '/Task/GetTask?id=' + TaskId + '',
        success: function (data) {
            var targetCompletionDate = new Date(data.data.targetCompletionDate);
            var ttargerdate = ('0' + (targetCompletionDate.getMonth() + 1)).slice(-2) + '/' + targetCompletionDate.getDate() + '/' + targetCompletionDate.getFullYear();         
            $('#TaskId').val(data.data.taskId);
            $('#jobId').val(data.data.jobId);
            $('#departmentId').val(data.data.departmentId);
            $('#TaskName').val(data.data.taskName);
            $('#BudgetedHours').val(data.data.budgetedHours);
            $('#TargetCompletionDate').val(ttargerdate);
            $('#Employee').val(data.data.employee);
            $('#Status').val(data.data.status);
            $('#userId').val(data.data.userId);
           
        }
    });
}

function showTaskDetails(TaskId) {
    callGetTask(TaskId);
 }

function Dialogueclose() {
    $(".ui-dialog-titlebar-close").trigger("click");
}
