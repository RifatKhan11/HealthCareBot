$(function () {
    $('#totaBoxId').val('police' + Math.round(Math.random() * 10000));
    $('.fa-minus').click(function () {
        $(this).closest('.chatbox').toggleClass('chatbox-min');
    });
    $('.fa-close').click(function () {
        $(this).closest('.chatbox').hide();
    });
});

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chat")
    .build();

connection.start().catch(err => console.error(err.toString()));

connection.on('Send', (nick, message) => {
    console.log(nick + 'Saying: '+ message);
    AddServerMessage(message);
});

document.getElementById('totaMessageSubmit').addEventListener('click', event => {
    let message = $('#totaMessage').val();
    let nick = $('#totaBoxId').val();
    alert('totaboxjs 25');
    $('#totaMessage').val('');

    AddSendMessage(message);
    connection.invoke('Send', nick, message);
    event.preventDefault();
});

function AddServerMessage(message) {
   
    document.getElementById("totaMessagesContainer").innerHTML += "<div class='message-box-holder'><div class='message-box message-partner'>" + message + ".</div></div>";
}

function AddSendMessage(message) {
    document.getElementById("totaMessagesContainer").innerHTML +="<div class='message-box-holder'><div class='message-box'> "+ message + "</div></div>";
}

