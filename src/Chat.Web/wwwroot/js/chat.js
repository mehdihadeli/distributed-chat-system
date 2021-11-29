var BASE_API_ADDRESS = "https://localhost:7001/api";
var SEND_URL = BASE_API_ADDRESS + "/chat/send-message";
var LOAD_INITIAL_MESSAGES_URL = BASE_API_ADDRESS + "/chat/load-messages";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatSignalr")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.onclose(reconnect);
startConnection();

function startConnection() {
    console.log('connecting...');
    connection.start()
        .then(() => console.log('connected!'))
        .catch(reconnect);
}

function reconnect() {
    console.log('reconnecting...');
    setTimeout(startConnection, 2000);
}

connection.on("SendForReceiveMessage", function (chatMessage) {
    console.log(chatMessage);
    var date = new Date(chatMessage.messageDate);
    var datetime = date.getDate() + "/"
        + (date.getMonth() + 1) + "/"
        + date.getFullYear() + " @ "
        + date.getHours() + ":"
        + date.getMinutes() + ":"
        + date.getSeconds();

    $('#MessageList').append(`<li style="color: dodgerblue"><i class="fas fa-cloud-download-alt"></i> ${datetime} - ${chatMessage.senderUserName}: ${chatMessage.message}</li>`);
});

function sendMessage(senderUserName, sender, e) {
    e.preventDefault();

    var targetUserName = $('#TargetUserName').val();
    var message = $('#Message').val();
    $('#Message').val('');

    // // calling server side hub method sample
    // connection.invoke("SendMessage", {},"").catch(function (err) {
    //     return console.error(err.toString());
    // });

    $.ajax({
        url: SEND_URL,
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            targetUserName: targetUserName,
            senderUserName: senderUserName,
            message: message
        }),
        dataType: 'json',
        success: function () {
            var currentdate = new Date();
            var datetime = currentdate.getDate() + "/"
                + (currentdate.getMonth() + 1) + "/"
                + currentdate.getFullYear() + " @ "
                + currentdate.getHours() + ":"
                + currentdate.getMinutes() + ":"
                + currentdate.getSeconds();
            $('#MessageList').append(`<li style="color: #34ce57"><i class="fas fa-cloud-upload-alt"></i> ${datetime} - ${senderUserName} to ${targetUserName}: ${message}</li>`);
        },
        error: function (request, status, error) {
            Swal.fire({
                icon: 'error',
                title: 'Oops...',
                text: request.responseJSON.detail,
            });
        }
    });
}

function loadInitialMessagesForUser(userName, numberOfMessages) {
    $.ajax({
        url: LOAD_INITIAL_MESSAGES_URL + "/" + userName + "?numberOfMessages=" + numberOfMessages,
        type: 'GET',
        contentType: 'application/json',
        dataType: 'json',
        success: function (data) {
            data.forEach(chatMessage => {

                var date = new Date(chatMessage.messageDate);
                var datetime = date.getDate() + "/"
                    + (date.getMonth() + 1) + "/"
                    + date.getFullYear() + " @ "
                    + date.getHours() + ":"
                    + date.getMinutes() + ":"
                    + date.getSeconds();
                if (chatMessage.senderUserName === userName)
                    $('#MessageList').append(`<li style="color: #34ce57"><i class="fas fa-cloud-upload-alt"></i> ${datetime} - ${chatMessage.senderUserName}  to ${chatMessage.targetUserName}: ${chatMessage.message}</li>`);
                else
                    $('#MessageList').append(`<li style="color: dodgerblue"><i class="fas fa-cloud-download-alt"></i> ${datetime} - ${chatMessage.senderUserName}: ${chatMessage.message}</li>`);
            });
        },
        error: function (request, status, error) {
            Swal.fire({
                icon: 'error',
                title: 'Oops...',
                text: request.responseJSON.detail,
            });
        }
    });
}


