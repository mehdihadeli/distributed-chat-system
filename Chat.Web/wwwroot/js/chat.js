var BASE_API_ADDRESS = "https://localhost:7001/api";
var SEND_URL = BASE_API_ADDRESS + "/chat/send-message";

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

connection.on("SendForReceiveMessage", function (message) {
    console.log(message);
    $('#MessageList').append('<li><strong><i class="fas fa-long-arrow-alt-right"></i> ' + message + '</strong></li>');
});

connection.on("ReceiveChatNotification", function (message) {
    console.log(message);
});

function sendMessage(senderEmail, sender, e) {
    e.preventDefault();

    var targetEmail = $('#TargetEmail').val();
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
            targetEmail: targetEmail,
            senderEmail: senderEmail,
            message: message
        }),
        dataType: 'json',
        success: function (data) {
            $('#MessageList')
                .append('<li><i class="fas fa-long-arrow-alt-left"></i> ' + abp.currentUser.userName + ': ' + message + '</li>');
        }
    });
}
