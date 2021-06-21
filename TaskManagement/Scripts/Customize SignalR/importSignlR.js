$(document).ready(function () {
    let sessionUserId = '@Session["UserID"]';
    var userid = $('#sessionDiv').attr('data-id');
    var notifier = $.connection.commentHub;
    // Create a function that the hub can call back to display messages.
    var highestId = 0;
    var totalUnRead = 0;

    getNotifyByCurrentuser();
    $.ajax({
        type: "GET",
        url: "/Notifier/TotalNotifierUnRead",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            console.log("UnRead: ", data);
            totalUnRead = data;
            getTotalUnRead(data);
        },
        error: function (data) {
            //alert('error');
        }
    });
   
    $.ajax({
        type: "GET",
        url: "/Notifier/GetHighestNotifyId",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            highestId = data;
        },
    });
    window.commentSignlR = $.connection.commentHub;
    window.commentSignlR.client.sendNotifier = function (changeBy, assignFrom, assignTo, changeByFullName, assignFromFullName, assignToFullName, taskId, taskName) {
        if (userid == assignTo || userid == assignFrom) {
            highestId = highestId + 1;
            totalUnRead = totalUnRead + 1;
            getTotalUnRead(totalUnRead);
            let tagGoToTask = `<a href="/Admin/TaskManages/Edit?id=${taskId}&source=${highestId}" class="dropdown-item-text1 font-weight-bold text-primary"><i class="fas fa-file mr-2"></i><span style="white-space: pre-line;">Task: ${taskName}đã được giao cho ${assignToFullName}</span></a`;
            let today = new Date();
            let hours = today.getHours();
            hours = (hours % 12) || 12;
            let minutes = today.getMinutes();
            let seconds = today.getSeconds();
            let AmOrPm = hours >= 12 ? 'AM' : 'PM';
          
            $('#notification').prepend(
                '<div class="container" style="border-bottom:1px solid #80808045">'
                + '<div class="row">'
                + '<div class="col-10">'
                + tagGoToTask + '<br/>'
                + `<span class="float-left text-muted text-sm" style=" width:100%;">+ Được giao bởi: ${changeByFullName}</span>`
                + `<span class="float-left text-muted text-sm" style="width:100%">+ Giao cho: ${assignToFullName}</span>`
                + `<span class="float-left text-muted text-sm" style="margin-left: 4.3rem;width:100%; text-decoration: line-through;">${assignFromFullName}</span>`
                + '<span class="float-left text-muted text-sm" style="width:100% ">' + '+ At: ' + (today.getDate() < 10 ? ("0" + today.getDate()) : today.getDate()) + "-" + (today.getMonth() < 9 ? ("0" + (today.getMonth() + 1)) : (today.getMonth() + 1)) + "-" + today.getFullYear() + " " + (hours < 10 ? ("0" + hours) : hours) + ":" + (minutes < 10 ? ("0" + minutes) : minutes) + ":" + (seconds < 10 ? ("0" + seconds) : seconds) + " " + AmOrPm + '</span>'
                + '</div>'
                + '<div class="col-2" style="border-left:1px solid #80808045;text-align: center;padding-top: 50px;">'
                + '<a href="#" class="dropdown-item-text1"><i class="fas fa-trash"></i></a>'
                + '</div>'
                + '</div>'
                + '</div>'
            );
        }
    };
    window.hubReady = $.connection.hub.start();
    window.hubReady.done(function () {
        $("#formSubmit").submit(function (event) {
            event.preventDefault();
            let userAssignId = $("#UserAssign").val();
            let taskEditId = $("#taskEditId").val();
            $.ajax({
                type: "GET",
                url: "/Notifier/GetInfoToSendNotify?taskId=" + taskEditId + `&userAssign=${userAssignId}`,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data) {
                        window.commentSignlR.server.sendNotifier(data.ChangeById, data.AssignFromId, data.AssignToId, data.ChangeByFullName, data.AssignFromFullName, data.AssignToFullName, data.TaskId, data.TaskName);
                    }
                    //
                },
            });
            setTimeout(function () { $('#formSubmit').unbind('submit').submit(); }, 2000);
        });
    });
    //$.connection.hub.start().done(function () {

    // });
});
function getTotalUnRead(total) {
    
    $('#totalUnRead').text(total); 
}
function getNotifyByCurrentuser() {

    //var date = item.CreatedDate.toString("dd-MM-yyyy");
    $.ajax({
        type: "GET",
        url: "/Notifier/GetAllNotifyByUserId",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data && data.length) {
                data.forEach(item => {
                    let datePath = item.CreatedDate.substr(6);
                    let currentTime = new Date(parseInt(datePath));
                    let month = currentTime.getMonth() + 1;
                    let day = currentTime.getDate();
                    let year = currentTime.getFullYear();
                    let hours = currentTime.getHours();
                    hours = (hours % 12) || 12;
                    let minutes = currentTime.getMinutes();
                    let seconds = currentTime.getSeconds();
                    let AmOrPm = hours >= 12 ? 'AM' : 'PM';
                    let date = (day < 10 ? ("0" + day) : day) + "-" + (month < 10 ? ("0" + month) : month) + "-" + year + " " + (hours < 10 ? ("0" + hours) : hours) + ":" + (minutes < 10 ? ("0" + minutes) : minutes) + ":" + (seconds < 10 ? ("0" + seconds) : seconds) + " " + AmOrPm;
                    console.log(item.IsRead);
                    let tagGoToTask = `<a href="/Admin/TaskManages/Edit?id=${item.TaskId}&source=${item.OrderId}" class="dropdown-item-text1 ${item.IsRead ? '' : 'font-weight-bold text-primary'}" ><i class="fas fa-file mr-2"></i><span style="white-space: pre-line;">Task: ${item.TaskName} đã được giao cho ${item.AssignToFullName}</span></a`;
                    $('#notification').append(
                        '<div class="container" style="border-bottom:1px solid #80808045">'
                        + '<div class="row">'
                        + '<div class="col-10">'
                        + tagGoToTask + '<br/>'
                        + `<span class="float-left text-muted text-sm" style=" width:100%;">+ Được giao bởi: ${item.ChangeByFullName}</span>`
                        + `<span class="float-left text-muted text-sm" style="width:100%">+ Giao cho: ${item.AssignToFullName}</span>`
                        + `<span class="float-left text-muted text-sm" style="margin-left: 4.3rem;width:100%; text-decoration: line-through;">${item.AssignFromFullName}</span>`
                        + `<span class="float-left text-muted text-sm" style="width:100% ">+ At: ${date}</span>`
                        + '</div>'
                        + '<div class="col-2" style="border-left:1px solid #80808045;text-align: center;padding-top: 50px;">'
                        + '<a href="#" class="dropdown-item-text1"><i class="fas fa-trash"></i></a>'
                        + '</div>'
                        + '</div>'
                        + '</div>'

                    );
                });
            }
        },
        error: function (data) {
            //alert('error');
        }
    });
}