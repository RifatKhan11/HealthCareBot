
$(document).ready(function () {
    //$("#window-close").click(function () {

    //    $("#chat-window").fadeOut(300);

    //    $("#chat-button-theme-bubble").fadeIn(1000);
    //   $("#chatbot-chat", parent.document).css({ "height": "100px", "width": "100px" });
    //});

    //$("#chat-button-theme-bubble").click(function () {

    //    $(this).fadeOut(300);
    //    $("#chat-window").fadeIn(1000);
    //    $("#chat-window").css({ "width": "500px;" });

    //    let nick = $('#totaBoxId').val();
    //    alert
    //    //if (nick == null && nick == "")
    //    //{
    //    //    nick = "c9feddda-bcf4-4dc3-9b1b-ddb38aeb8e35";

    //    //}

    //$("#chatbot-chat", parent.document).css({ "height": "700px", "width": "410px" });
    //   // $("#chatbot-chat").css({ "height": "700px", "width": "410px" });
    //    GenerateResponseJsonAndSend("", "wstart", nick);

    //});

    //var botKey = localStorage.getItem('botKey');
    var botKey = $('.botKeyDefault').val();
    //var botKey2 = getCookie('botKey');
    //const cookies = document.cookie.split('; ');

    
    //console.log(document.cookie);

    //for (let i = 0; i < cookies.length; i++) {
    //    const cookie = cookies[i].split('=');
    //    alert(500);
    //    alert($('.window-content #botKey').val());
    //    console.log(cookies);
    //    alert(22);
    //    if (cookie[0] === 'botKey') {
    //        // Decode the cookie value before returning
    //        botKey = cookie[1];
    //        alert(3);
    //        alert(botKey);
    //    }
    //}

    //alert(2);
    //alert(botKey);



    $('#totaBoxId').val(generateGUID());
    $('#totaBoxNId').val(generateGUID());

    $("#chatbot-chat").width(410).height(700);
    $("#chat - button - theme - bubble").fadeOut(300);
    $("#chat-window").fadeIn(1000);
    $("#chat-window").css({ "width": "500px;" });

    let nick = $('#totaBoxId').val();
    let obj = new Object();
    obj.pageId = '106518517401564';
    obj.message = "wstart";
    obj.postback = "";


    $.ajax({
        url: "/MasterData/GreetingsMessage?botKey=" + botKey,

    }).done(function (data) {
        opp = data;
        AddServerMessage(data);
        setTimeout(function () {
            let message = 'menu';
            let nick = $('#totaBoxId').val();
            GenerateResponseJsonAndSend(message, cahtboxpostback, nick, 0);
            cahtboxpostback = "";
            //event.preventDefault();
        }, 500)

        //AddServerMessage("How can I help you?<br><br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; color: white; cursor: pointer;'>Book Appointment with Department</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(-1)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; color: white; cursor: pointer;'>Book Appointment with Doctor</button><button onclick='ClickedMenuMenuQQ(" + 1 + "," + 0 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px;color: white; cursor: pointer;'>Manage Your Booking</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(0)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; color: white; cursor: pointer;'>Book Teleconsultation</button>");
    });

    //alert(obj);
    // connection.invoke('Send', nick, JSON.stringify(obj));
    //let opp = "";
    //$.ajax({
    //    url: "/MasterData/FirstOperator/",

    //}).done(function (data) {
    //    console.log(data);
    //    opp = data;
    //    AddServerMessage("Hello I am " + data + ".How can I help you?<br><br>Have you applied for police clearance?" + "<br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(-1)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><button onclick='ClickedMenuMenuQQ(" + 1 + "," + 0 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(0)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>");

    //    });
    ////Messages.Add("{ \"msg\":\"" + "Have you applied for police clearance?" + "<br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(-1)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><button onclick='ClickedMenuMenuQQ(" + 1 + "," + 0 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(0)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");





    // alert(nick);
    //GenerateResponseJsonAndSend("", "wstart", nick);
    // alert(generateGUID());
    //lastElementTop = $('#totaMessagesContainer .anitem:last-child').position().top;
    //scrollAmount = lastElementTop - 200;
    //$('#totaMessagesContainer').animate({scrollTop: scrollAmount }, 200);


    //$("#totaMessage").keypress(function () {

    //    window.location.href = "#totaMessagesContainerLast";

    //});



});

//$("#totaMessageSubmit").onclick(function () {
//    window.location.href = "#totaMessagesContainerLast";
//});

setInterval(function () {
    LoadBasicFormData();
    console.log('1 second has passed');
}, 1000);

function clickFun() {
    window.location.href = "#totaMessagesContainerLast";
}


const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chat", {
        accessTokenFactory: () => getUserId()
    })
    .configureLogging(signalR.LogLevel.Information)
    .build();
var cahtboxpostback = "";

function getUserId() {
    let userId = localStorage.getItem("userId");
    if (!userId) {
        userId = crypto.randomUUID(); // Generate a new unique ID
        localStorage.setItem("userId", userId);
    }
    return userId;
}

connection.start().catch(err => console.error(err.toString()));

console.log('ConnectionId-'+connection.start().catch(err => console.error(err.toString())));
console.log("Connected to SignalR with UserID:", getUserId());

connection.on('Send', (nick, message) => {
    //console.log('ConnectionId-'+connection);
    //console.log(nick + 'Saying: ' + message);


    ManageMessageInput(message);


    var obj = JSON.parse(message);
    if (typeof obj.postback !== 'undefined') {
        cahtboxpostback = obj.postback;
    }

    AddServerMessage(obj.msg);
});


document.getElementById("totaMessage").addEventListener("keyup", function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();
        document.getElementById("totaMessageSubmit").click();

        window.location.href = "#totaMessagesContainerLast";
        $("#totaMessage").focus();

    }
});

document.getElementById('totaMessageSubmit').addEventListener('click', event => {
    let message = $('#totaMessage').val();
    let nick = $('#totaBoxId').val();
    $('#totaMessage').val('');
    if (message != "") {
        if (localStorage.getItem('wrapperId') == 2) {
            localStorage.setItem('doctorKey', message);
        }
        if (localStorage.getItem('wrapperId') == 1) {
            localStorage.setItem('depmartmentKey', message);
        }

        AddSendMessage(message);
        GenerateResponseJsonAndSend(message, cahtboxpostback, nick, 0);
        cahtboxpostback = "";
        event.preventDefault();

        $(".conversation-wrapper").animate({ scrollTop: $(".conversation-wrapper")[0].scrollHeight }, 200);
    }
});

// Generate Response Json And Send Message.
function GenerateResponseJsonAndSend(message, postback, nick, qId) {
    //alert(postback);
    //alert('chatjs 154');
    //var botKey = localStorage.getItem('botKey');

    //var botKey = getCookie('botKey');

    //cookies = document.cookie.split('; ');
    //for (let i = 0; i < cookies.length; i++) {
    //    const cookie = cookies[i].split('=');
    //    if (cookie[0] === 'botKey') {
    //        // Decode the cookie value before returning
    //        botKey = cookie[1];
    //    }
    //}
    //alert(botKey);


    $('.formMobile').html('');


    let obj = new Object();
    obj.pageId = '106518517401564';
    obj.message = message;
    obj.postback = postback;
    obj.botKey = $('.botKeyDefault').val();
    obj.qId = qId;
    let userId = $('#totaBoxNId').val();
    $('.conversation-wrapper').append(`<div class="loader-box"><img src="https://i.gifer.com/ZhKG.gif""></div>`);
    $(".conversation-wrapper").animate({ scrollTop: $(".conversation-wrapper")[0].scrollHeight }, 200);

    //setTimeout(function () {
    //    $('.conversation-wrapper .loader-box').remove();

    //    connection.invoke('Send', obj.pageId, JSON.stringify(obj), userId); // Invoking HUB Connection
    //}, 2000);


    $('.conversation-wrapper').append('<div class="loader-box">Loading...</div>');
    connection.invoke('Send', obj.pageId, JSON.stringify(obj), userId)
        .then(function () {
            $('.conversation-wrapper .loader-box').remove();
        })
        .catch(function (err) {
            console.error(err);
            $('.conversation-wrapper .loader-box').remove();
        });

    msgLoaded = true;
    return;
}

function generateGUID() { // Responsible for unique user ID
    var d = new Date().getTime();

    var guid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = (d + Math.random() * 16) % 16 | 0;
        d = Math.floor(d / 16);
        return (c == 'x' ? r : (r & 0x3 | 0x8)).toString(16);
    });
    return guid;
}

function AddServerMessage(message) {
    console.log('message>', message);
    console.log(222);

    $('.btnGroup-box').remove();

    var isBtnGroup = message.indexOf('btn-group-message') >= 0;
    var isDept = message.indexOf('handleDeptCardClick') >= 0;
    var isSpec = message.indexOf('handleSpecCardClick') >= 0;
    var isDr = message.indexOf('handleDrCardClick') >= 0;
    var isinput = message.indexOf('SubmitInputData') >= 0;
    var isinput4 = message.indexOf('SubmitInputField') >= 0;
    var isinput2 = message.indexOf('SubmitInputUhidField') >= 0;
    var isinputOtp = message.indexOf('SubmitOtp') >= 0;
    var isinput3 = message.indexOf('SubmitInputMobileField') >= 0;
    var isinputUhid = message.indexOf('SubmitUHID') >= 0;
    var isOtp = message.indexOf('SubmitOTPCode') >= 0;
    var isOtp2 = message.indexOf('GetNextMessage') >= 0;
    // var sevenDays = message.indexOf('NEXT7DAYS') >= 0;
    var sevenDays = JSON.stringify(message).indexOf('NEXT7DAYS') >= 0;
    var iscard = message.indexOf('bluedualcard3 ') >= 0;

    //var sevenDays = JSON.stringify(message).indexOf('frmBasic') >= 0;
    var timeslots = message.indexOf('TIMESLOTS') >= 0;
    //var isIframe = message.indexOf('iframe') >= 0;
    console.log(isBtnGroup);
    var wrapperId = localStorage.getItem('wrapperId');
    console.log('wrapperId-'+wrapperId)
    if (message.slice(-8) == "textSend" && ["21", "2", "3","4"].includes(wrapperId))
    {
        document.getElementById("totaMessagesContainer").innerHTML += message.slice(0,-8);
    }
    else {
        if (message == "<img style='height: 39px;width:73px;margin:-15px' src='/images/tenor.gif'/>") {

            //document.getElementById("totaMessagesContainer").innerHTML += "<div id='gif' class='response'><div bot='' class='response-wrapper avatar-offset'><div class='avatar' style='background-image: url(&quot;http://115.127.99.113:239/chatbox/TOTA.png&quot;);'></div> <div class='text response-content' style='width:270px;'><div id='gif' class='message' style='background: rgb(233, 238, 244); border-color: rgb(233, 238, 244); color: rgb(100, 100, 100);'>" + message + "</div></div></div></div>";
            document.getElementById("totaMessagesContainer").innerHTML += "<div id='gif' class='response'><div bot='' class='response-wrapper avatar-offset'><div class='avatar' style='background-image: url(&quot;https://healthylabs.s3-ap-southeast-1.amazonaws.com/chatbot/evercare.png&quot;);'></div> <div class='text response-content' style='width:270px;'><div id='gif' class='message' style='background: rgb(233, 238, 244); border-color: rgb(233, 238, 244); color: rgb(100, 100, 100);'>" + message + "</div></div></div></div>";
            //document.getElementById("totaMessagesContainer").innerHTML += "<div id='gif' class='response'><div bot='' class='response-wrapper avatar-offset'><div class='avatar' style='background-image: url(&quot;http://c5529874.ngrok.io/chatbox/TOTA.png&quot;);'></div> <div class='text response-content'><div id='gif' class='message' style='background: rgb(233, 238, 244); border-color: rgb(233, 238, 244); color: rgb(100, 100, 100);'>" + message + "</div></div></div></div>";


            //document.getElementById("totaMessagesContainer").innerHTML += "<div id='gif' class='response'><div bot='' class='response-wrapper avatar-offset'><div class='avatar' style='background-image: url(&quot;https://dataqbd.com/chatbox/TOTA.png&quot;);'></div> <div class='text response-content'><div id='gif' class='message' style='background: rgb(233, 238, 244); border-color: rgb(233, 238, 244); color: rgb(100, 100, 100);'>" + message + "</div></div></div></div>";
        }
        else {
            $('#gif').remove();
            //document.getElementById("totaMessagesContainer").innerHTML += "<div class='response'><div class='response-date' style='color: rgba(0, 0, 0, 0.7);'>Today: " + CurrentTimeString() + "</div> <div bot='' class='response-wrapper avatar-offset'><div class='avatar' style='background-image: url(&quot;http://115.127.99.113:239/chatbox/TOTA.png&quot;);'></div> <div class='text response-content' style='width:270px;'><div id='message' class='message' style='background: rgb(233, 238, 244); border-color: rgb(233, 238, 244); color: rgb(100, 100, 100);'>" + message + "</div></div></div></div>";
            //document.getElementById("totaMessagesContainer").innerHTML += "<div class='response'><div class='response-date' style='color: rgba(0, 0, 0, 0.7);'>Today: " + CurrentTimeString() + "</div> <div bot='' class='response-wrapper avatar-offset'><div class='avatar' style='background-image: url(&quot;http://c5529874.ngrok.io/chatbox/TOTA.png&quot;);'></div> <div class='text response-content'><div id='message' class='message' style='background: rgb(233, 238, 244); border-color: rgb(233, 238, 244); color: rgb(100, 100, 100);'>" + message + "</div></div></div></div>";
            //document.getElementById("totaMessagesContainer").innerHTML += "<div class='response'><div class='response-date' style='color: rgba(0, 0, 0, 0.7);'>Today: " + CurrentTimeString() + "</div> <div bot='' class='response-wrapper avatar-offset'><div class='avatar' style='background-image: url(&quot;https://dataqbd.com/chatbox/TOTA.png&quot;);'></div> <div class='text response-content'><div id='message' class='message' style='background: rgb(233, 238, 244); border-color: rgb(233, 238, 244); color: rgb(100, 100, 100);'>" + message + "</div></div></div></div>";

            if (isBtnGroup || isDept || isDr || timeslots || isOtp || isOtp2 || isSpec) {
                /*document.getElementById("totaMessagesContainer").innerHTML += "<div class='response btnGroup-box'><div class='response-date' style='color: rgba(0, 0, 0, 0.7);display:none;'>Today: " + CurrentTimeString() + "</div> <div bot='' class='response-wrapper avatar-offset'><div class='avatar' style='background-image: url(&quot;https://healthylabs.s3-ap-southeast-1.amazonaws.com/chatbot/evercare.png&quot;);display:none;'></div> <div class='text response-content' style='width:400px;'><div id='message' class='message' style='background: #f5f5f5; box-shadow: 0px 1px 5px 0px rgba(0, 0, 0, 0.2), 0px 2px 2px 0px rgba(0, 0, 0, 0.14), 0px 3px 1px -2px rgba(0, 0, 0, 0.12); border-color: rgb(233, 238, 244); color: rgb(100, 100, 100);'>" + message + "</div></div></div></div>";*/
                document.getElementById("totaMessagesContainer").innerHTML += "<div class='response btnGroup-box'><div class='response-date' style='color: rgba(0, 0, 0, 0.7);display:none;'>Today: " + CurrentTimeString() + "</div> <div bot='' class='response-wrapper avatar-offset'><div class='text response-content' style='width:340px;text-align: center;'><div id='message' class='message' style='width:340px !important;background: #ffffff;color: rgb(100, 100, 100);margin-left:-40px;'>" + message + "</div></div></div></div>";
            }

            else if (sevenDays) {
                /*document.getElementById("totaMessagesContainer").innerHTML += "<div class='response btnGroup-box'><div class='response-date' style='color: rgba(0, 0, 0, 0.7);display:none;'>Today: " + CurrentTimeString() + "</div> <div bot='' class='response-wrapper avatar-offset'><div class='avatar' style='background-image: url(&quot;https://healthylabs.s3-ap-southeast-1.amazonaws.com/chatbot/evercare.png&quot;);display:none;'></div> <div class='text response-content' style='width:400px;'><div id='message' class='message' style='background: #f5f5f5; box-shadow: 0px 1px 5px 0px rgba(0, 0, 0, 0.2), 0px 2px 2px 0px rgba(0, 0, 0, 0.14), 0px 3px 1px -2px rgba(0, 0, 0, 0.12); border-color: rgb(233, 238, 244); color: rgb(100, 100, 100);'>" + message + "</div></div></div></div>";*/
                document.getElementById("totaMessagesContainer").innerHTML += "<div class='response btnGroup-box'><div class='response-date' style='color: rgba(0, 0, 0, 0.7);display:none;'>Today: " + CurrentTimeString() + "</div> <div bot='' class='response-wrapper avatar-offset'><div class='text response-content' style='width:340px;text-align: center;'><div id='message' class='message' style='width:340px !important;background: #ffffff;color: rgb(100, 100, 100);margin-left:-20px;'>" + message + "</div></div></div></div>";

            }


            else if (isinput || isinput2 || isinput3 || isinput4 || isinputOtp || isinputUhid) {
                document.getElementById("totaMessagesContainer").innerHTML += "<div class='response'><div class='response-date' style='color: rgba(0, 0, 0, 0.7);display:none;'>Today: " + CurrentTimeString() + "</div> <div bot='' class='response-wrapper avatar-offset'><div class='text response-content' style='width:324px;text-align: center;'><div id='message' class='message' style='width:330px !important;margin-left:0px;'>" + message + "</div></div></div></div>";

            }
            else if (iscard) {
                document.getElementById("totaMessagesContainer").innerHTML += "<div class='response'><div class='response-date' style='color: rgba(0, 0, 0, 0.7);display:none;'>Today: " + CurrentTimeString() + "</div> <div bot='' class='response-wrapper avatar-offset'><div class='text response-content' style='width:400px;text-align: center;'><div id='message' class='message' style='width:335px !important;margin-left:-20px;'>" + message + "</div></div></div></div>";

            }
            else {
                document.getElementById("totaMessagesContainer").innerHTML += "<div class='response'><div class='response-date' style='color: rgba(0, 0, 0, 0.7); display:none;'>Today: " + CurrentTimeString() + "</div> <div bot='' class='response-wrapper avatar-offset'><div class='avatar' style='background-image: url(&quot;https://healthylabs.s3-ap-southeast-1.amazonaws.com/chatbot/evercare.png&quot;);'></div> <div class='text response-content' style='width:300px;'><div id='message' class='message' style='background: #f5f5f5; box-shadow: 0px 1px 5px 0px rgba(0, 0, 0, 0.2), 0px 2px 2px 0px rgba(0, 0, 0, 0.14), 0px 3px 1px -2px rgba(0, 0, 0, 0.12); border-color: rgb(233, 238, 244); color: rgb(100, 100, 100);'>" + message + "</div></div></div></div>";
            }
        }
    }
    if (cname != '' && cphone != '' && cemail != '' && cdob != '') {
        $('#name input[type="text"]').val(cname);
        $('#phone input[type="text"]').val(cphone);
        $('#email input[type="text"]').val(cemail);
        $('#dob input[type="date"]').val(cdob);
    }





    var element = document.getElementById("totaMessagesContainer");
    element.scrollTop = element.scrollHeight;

    document.getElementById("message").focus();

    $(".conversation-wrapper").animate({ scrollTop: $(".conversation-wrapper")[0].scrollHeight }, 200);
}


//function updateScroll() {
//    var element = document.getElementById("totaMessagesContainer");
//    element.scrollTop = element.scrollHeight;
//}
//setInterval(updateScroll, 100);


function AddSendMessage(message) {

    // document.getElementById("totaMessagesContainer").innerHTML += "<div class='response'><div class='response-date' style='color: rgba(0, 0, 0, 0.7);'>Today: " + CurrentTimeString() + "</div><div user='' class='response-wrapper'><div class='user response-content' style='width:270px;'><div id='message' class='message' style='color: rgb(255, 255, 255); background: rgb(0, 108, 255); border-color: rgb(0, 108, 255);'>" + message + "</div> <div class='info' style='color: rgb(100, 100, 100);'></div></div></div></div>";
    document.getElementById("totaMessagesContainer").innerHTML += "<div class='response'><div class='response-date' style='color: rgba(0, 0, 0, 0.7);'></div><div user='' class='response-wrapper'><div class='user response-content' style='width:270px;'><div id='message' class='message' style='color: rgb(255, 255, 255); background: rgb(0, 108, 255); border-color: rgb(0, 108, 255);'>" + message + "</div> <div class='info' style='color: rgb(100, 100, 100);'></div></div></div></div>";

    document.getElementById("message").focus();

}



function CurrentTimeString() {
    return new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
}

function ScrollDown() {
    var myElement = document.getElementById('totaMessagesContainer');
    var topPos = myElement.offsetTop;
}

// Drop Down Region

function myFunction() {
    document.getElementById("myDropdown").classList.toggle("show");
}
function ClickedComplainS(data) {
    let complain = $("#complain").val();

    $.ajax({
        url: "/MasterData/SaveComplain/" + complain.toString() + "/" + data.toString(),

    }).done(function (data) {

        AddServerMessage("Thanks for your complain.");
    });
}
function ClickedSuggestionS(data) {
    let complain = $("#Suggestion").val();
    $.ajax({
        url: "/MasterData/SaveSuggestion/" + complain.toString() + "/" + data.toString(),

    }).done(function (data) {

        AddServerMessage("Thanks for your suggestion.");
    });
}

function ClickedComplain(data) {

    let nick = $('#totaBoxId').val();
    AddSendMessage("complain");
    GenerateResponseJsonAndSend("", "complain-data," + data, nick, 0);


    window.location.href = "#totaMessagesContainerLast";

}
function ClickedSuggestion(data) {
    let nick = $('#totaBoxId').val();
    AddSendMessage("Suggestion");
    GenerateResponseJsonAndSend("", "Suggestion-data," + data, nick, 0);


    window.location.href = "#totaMessagesContainerLast";
}
function ClickedMenu(data) {

    if (data != 5) {
        AddSendMessage(data);
    }

    let nick = $('#totaBoxId').val();
    if (data == "Health Care Helpdesk") {
        GenerateResponseJsonAndSend("", "passport", nick, 0);
    } else if (data == "নতুন করে শুরু") {
        //GenerateResponseJsonAndSend("", "nstart", nick);
        window.location.reload();
    }
    else if (data == "5") {
        GenerateResponseJsonAndSend("", "menues", nick, 0);
    }


    window.location.href = "#totaMessagesContainerLast";

}



function RefreshBot() {
    $('#myModal').modal('show');
}

function HideExitModal() {
    $('#myModal').modal('hide');
}

function ClickedMenuMenul(data, Ldata) {
    var menuname = "";
    if (data == 0) {
        menuname = "Bangla";
        AddSendMessage(menuname);
        Ldata = data;
        AddServerMessage("<img style='height: 39px;width:73px;margin:-15px' src='/images/tenor.gif'/>");


    }
    else if (data == -1) {
        menuname = "English";
        AddSendMessage(menuname);
        Ldata = data;
        AddServerMessage("<img style='height: 39px;width:73px;margin:-15px' src='/images/tenor.gif'/>");
    }
    else if (data == -3) {
        menuname = "না";
        AddSendMessage(menuname);
        Ldata = 0;
        data = 0;
        AddServerMessage("<img style='height: 39px;width:73px;margin:-15px' src='/images/tenor.gif'/>");
    }
    else if (data == -4) {
        menuname = "No";
        AddSendMessage(menuname);
        Ldata = -1;
        data = -1;
        AddServerMessage("<img style='height: 39px;width:73px;margin:-15px' src='/images/tenor.gif'/>");
    }
    else {

        $.ajax({
            url: "/Knowledge/getdatabyId/" + data,

        })
            .done(function (data) {
                if (Ldata == 0) {

                    menuname = data.menuName;
                    AddSendMessage(menuname);
                    AddServerMessage("<img style='height: 39px;width:73px;margin:-15px' src='/images/tenor.gif'/>");
                }
                else {

                    menuname = data.menuNameEN;
                    AddSendMessage(menuname);
                    AddServerMessage("<img style='height: 39px;width:73px;margin:-15px' src='/images/tenor.gif'/>");
                }
            });
    }

    setTimeout(function () {
        console.log("data")
        let nick = $('#totaBoxId').val();
        GenerateResponseJsonAndSend("", "menuel," + data + "," + menuname + "," + Ldata, nick, 0);
        //GenerateResponseJsonAndSend("", "menues," + data + "," + menuname + "," + Ldata, nick);

        window.location.href = "#totaMessagesContainerLast";
    }, 5000)



}

function GenerateResponseJsonAndSendK(message, postback, nick, qId) {
    let obj = new Object();
    obj.pageId = '106518517401564';
    obj.message = message;
    obj.postback = postback;
    obj.qId = qId;
    let userId = $('#totaBoxNId').val();
    connection.invoke('Send', nick, JSON.stringify(obj), userId); // Invoking HUB Connection
    return;
}

function ClickedMenuMenuK(data, Ldata) {

    let postback = "";
    let message = "";
    if (data != 0) {
        postback = "postback:KWA;" + data;

    }
    if (Ldata == -1) {
        message = "yes";
    }
    else if (Ldata == 1) {
        message = "no";
    }
    else if (Ldata == 2) {
        message = "না";
    }
    else if (Ldata == 0) {
        message = "হ্যাঁ ";
    }
    AddSendMessage(message);
    let nick = $('#totaBoxId').val();
    GenerateResponseJsonAndSendK(message, postback, nick);
    window.location.href = "#totaMessagesContainerLast";
    //setTimeout(function () {
    //    let nick = $('#totaBoxId').val();
    //    //GenerateResponseJsonAndSend("", "menuel," + data + "," + menuname + "," + Ldata, nick);
    //    GenerateResponseJsonAndSend("", "menues," + data + "," + menuname + "," + Ldata, nick);

    //    window.location.href = "#totaMessagesContainerLast";
    //}, 5000)



}

function ClickedMenuMenu(data, Ldata) {
    var menuname = "";
    if (data == 0) {
        //menuname = "Bangla";
        menuname = "না";
        AddSendMessage(menuname);
        Ldata = data;
        AddServerMessage("<img style='height: 39px;width:73px;margin:-15px' src='/images/tenor.gif'/>");


    }
    else if (data == -1) {
        //menuname = "English";
        menuname = "No";
        AddSendMessage(menuname);
        Ldata = data;
        AddServerMessage("<img style='height: 39px;width:73px;margin:-15px' src='/images/tenor.gif'/>");
    }
    else if (data == -3) {
        menuname = "না";
        AddSendMessage(menuname);
        Ldata = 0;
        data = 0;
        AddServerMessage("<img style='height: 39px;width:73px;margin:-15px' src='/images/tenor.gif'/>");
    }
    else if (data == -4) {
        menuname = "No";
        AddSendMessage(menuname);
        Ldata = -1;
        data = -1;
        AddServerMessage("<img style='height: 39px;width:73px;margin:-15px' src='/images/tenor.gif'/>");
    }
    else {
        //botKey = localStorage.getItem('botKey');
        //const cookies = document.cookie.split('; ');
        //for (let i = 0; i < cookies.length; i++) {
        //    const cookie = cookies[i].split('=');
        //    if (cookie[0] === 'botKey') {
        //        // Decode the cookie value before returning
        //        botKey = cookie[1];
        //    }
        //}

        botKey = $('.botKeyDefault').val();


        $.ajax({
            url: "/Knowledge/getdatabyId?Id=" + data,
            async: false

        })
            .done(function (data) {
                console.log('data> ', data);
                if (Ldata == 0) {

                    menuname = data.menuName;
                    AddSendMessage(menuname);
                    AddServerMessage("<img style='height: 39px;width:73px;margin:-15px' src='/images/tenor.gif'/>");
                }
                else {

                    menuname = data.menuNameEN;
                    AddSendMessage(menuname);
                    AddServerMessage("<img style='height: 39px;width:73px;margin:-15px' src='/images/tenor.gif'/>");
                }
            });
    }

    setTimeout(function () {
        let nick = $('#totaBoxId').val();
        //GenerateResponseJsonAndSend("", "menuel," + data + "," + menuname + "," + Ldata, nick);

        GenerateResponseJsonAndSend("", "menues," + data + "," + menuname + "," + Ldata, nick, 0);

        //window.location.href = "#totaMessagesContainerLast";
    }, 5000)

}

function ClickedAppointment(data, Ldata, sls, type) {
    let nick = $('#totaBoxId').val();
    let pageId = '106518517401564';
    var menuname = "";
    if (data == 0) {
        //menuname = "Bangla";
        menuname = "May I know your gender?";

        AddSendMessage(menuname);
        Ldata = data;
        AddServerMessage("<img style='height: 39px;width:73px;margin:-15px' src='/images/tenor.gif'/>");


    }
    else if (data == -1) {
        //menuname = "English";
        menuname = "No";
        AddSendMessage(menuname);
        Ldata = data;
        AddServerMessage("<img style='height: 39px;width:73px;margin:-15px' src='/images/tenor.gif'/>");
    }
    else if (data == -3) {
        menuname = "না";
        AddSendMessage(menuname);
        Ldata = 0;
        data = 0;
        AddServerMessage("<img style='height: 39px;width:73px;margin:-15px' src='/images/tenor.gif'/>");
    }
    else if (data == -4) {
        menuname = "No";
        AddSendMessage(menuname);
        Ldata = -1;
        data = -1;
        AddServerMessage("<img style='height: 39px;width:73px;margin:-15px' src='/images/tenor.gif'/>");
    }
    else {

        $.ajax({
            url: "/Knowledge/getDoctordatabyId/" + data,

        })
            .done(function (datas) {

                if (sls == 1) {
                    var mn = '';
                    mn = datas.name;
                    menuname = '';
                    AddSendMessage(mn);
                    var msg = "<span>May I know your gender?</span>";
                    msg += `<div style="width:200%;text-align: center;" class="row">
                        <button onclick =ClickedAppointment("${data}","${Ldata}",2,'male') id ="male" class="actionButton"><i class="fa fa-male"></i> Male</button>&nbsp;&nbsp;<button onclick =ClickedAppointment("${data}","${Ldata}",2,'female') id = "female" class="actionButton"><i class="fa fa-male"></i> Female</button>
                    </div>`;
                    AddServerMessage(msg);
                }
                else if (sls == 2) {
                    var mn = '';
                    if (type == 'male') {
                        mn = 'Male';
                    } else {
                        mn = 'Female';
                    }
                    menuname = '';
                    AddSendMessage(mn);
                    var msg = "<span>Do you have registered mobile ?</span>";
                    msg += `<div style="width:200%;text-align: center;" class="row">
                        <button onclick =ClickedAppointment("${data}","${Ldata}",3,'yes') id ="yes" class="actionButton">Yes</button>&nbsp;&nbsp;<button onclick =ClickedAppointment("${data}","${Ldata}",3,'no') id = "female" class="actionButton">No</button>
                    </div>`;
                    AddServerMessage(msg);
                }
                else if (sls == 3) {
                    var mn = '';
                    if (type == 'yes') {
                        mn = 'Yes';
                        menuname = '';
                        AddSendMessage(mn);
                        var msg = "Please enter your UHID";

                        AddServerMessage(msg);

                        var newmsg = `<table class="table table-responsive" style="color:black;width:100%;">
                                <tr id="tr_6"><td><input id="txt_6" placeholder="Enter UHID" style="width:100%;" type="text" class=""></td><td><span id="btn_6" class="btn btn-sm btn-info" onclick="fnProfile(6)" style="cursor:pointer"><i id="ic_6" class="fa fa-angle-right"></i></span></td></tr>
                            </table>`;
                        newmsg += `<input type="hidden" id="hdnData6" value="${data}" />`
                        newmsg += `<input type="hidden" id="hdnLData6" value="${Ldata}" />`
                        AddSendMessage(newmsg);
                    } else {
                        mn = 'No';

                        menuname = '';
                        AddSendMessage(mn);
                        var msg = "Please fill the patient's details";

                        AddServerMessage(msg);

                        var newmsg = `<table class="table table-responsive" style="color:black;width:100%;">
                                <tr id="tr_1"><td><input id="txt_1" placeholder="Enter patient's fullname" style="width:100%;" type="text" class=""></td><td><span id="btn_1" class="btn btn-sm btn-info" onclick="fnProfile(1)" style="cursor:pointer"><i id="ic_1" class="fa fa-angle-right"></i></span></td></tr>
                                <tr id="tr_2" style="display:none;"><td><input id="txt_2" placeholder="Enter mobile no" style="width:100%;" type="text" class=""><span id="msg_2" style="color:red;">*</span></td><td><span id="btn_2" class="btn btn-sm btn-info" onclick="fnProfile(2)" style="cursor:pointer"><i id="ic_2" class="fa fa-angle-right"></i></span></td></tr>
                                <tr id="tr_3" style="display:none;"><td><input id="txt_3" placeholder="Email id" style="width:100%;" type="text" class=""><span id="msg_3" style="color:red;"></span></td><td><span id="btn_3" class="btn btn-sm btn-info" onclick="fnProfile(3)" style="cursor:pointer"><i id="ic_3" class="fa fa-angle-right"></i></span></td></tr>
                                <tr id="tr_4" style="display:none;"><td><input id="txt_4" placeholder="Date Of Birth (DD/MM/YEAR)" style="width:100%;" type="text" class=""></td><td><span id="btn_4" class="btn btn-sm btn-info" onclick="fnProfile(4)" style="cursor:pointer"><i id="ic_4" class="fa fa-angle-right"></i></span></td></tr>
                            </table>`;
                        newmsg += `<input type="hidden" id="hdnData" value="${data}" />`
                        newmsg += `<input type="hidden" id="hdnLData" value="${Ldata}" />`
                        AddSendMessage(newmsg);
                    }

                }
                else if (sls == 4) {

                    menuname = '';
                    //AddSendMessage(mn);
                    var msg = `<span style="color:black;">Please enter otp received on your number</span>`;

                    AddServerMessage(msg);

                    var newmsg = `<table class="table table-striped" style="color:black;width:100%;">
                                    <caption style="color:black;">Please enter OTP received on your mobile</caption>
                                <tr id="tr_1"><td><input id="otp_1" placeholder="X  X  X  X  X  X" style="width:100%;" type="text" class=""><span id="msgOTP" class="text-danger"></span><input type="hidden" id="hdnMobile"></td><td><span id="btn_1" class="btn btn-sm btn-info" onclick="fnProfile(5)" style="cursor:pointer"><i id="ic_5" class="fa fa-angle-right"></i></span></td></tr>

                            </table>`;
                    newmsg += `<input type="hidden" id="hdnData2" value="${data}" />`
                    newmsg += `<input type="hidden" id="hdnLData2" value="${Ldata}" />`
                    AddSendMessage(newmsg);
                }
                else if (sls == 5) {
                    var mn = type;
                    menuname = '';
                    AddSendMessage(mn);
                    var msg = "<span>Please select the time slot for appointment</span>";
                    msg += `<div style="width:200%;text-align: center;" class="row">
                        <button onclick =ClickedAppointment("${data}","${Ldata}",3,'yes') id ="yes" class="actionButton">Yes</button>&nbsp;&nbsp;<button onclick =ClickedAppointment("${data}","${Ldata}",3,'no') id = "female" class="actionButton">No</button>
                    </div>`;
                    AddServerMessage(msg);
                }
                else {

                    menuname = data.name;
                    AddSendMessage(menuname);
                    AddServerMessage("<img style='height: 39px;width:73px;margin:-15px' src='/images/tenor.gif'/>");
                }
            });
    }
    setTimeout(function () {
        let nick = $('#totaBoxId').val();
        //GenerateResponseJsonAndSend("", "menuel," + data + "," + menuname + "," + Ldata, nick);
        if (menuname != '') {
            GenerateResponseJsonAndSend("", "appointment," + data + "," + menuname + "," + Ldata, nick, 0);

            window.location.href = "#totaMessagesContainerLast";
        }
    }, 5000)

}

function fnProfile(id) {
    //alert(newID);
    var newID = id + 1;
    if (id == 2) {
        var mobile = $('#txt_2').val();
        if (!(mobile.startsWith('01') && mobile.length == 11)) {
            $('#msg_2').html('Invalid mobile!');
            return false;
        } else {
            $('#msg_2').html('');
        }
    }
    if (id == 3) {
        const email = $('#txt_3').val();

        const emailRegex = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;

        if (!emailRegex.test(email)) {
            $('#msg_3').html('Invalid email!');
            return false;
        } else {
            $('#msg_3').html('');
        }
    }
    $('#txt_' + id).attr('disabled', 'disabled');
    $('#btn_' + id).removeClass('btn btn-sm btn-info').addClass('btn btn-sm btn-success');
    $('#ic_' + id).removeClass('fa fa-angle-right').addClass('glyphicon glyphicon-ok');
    $('#tr_' + newID).show();
    if (newID == 5) {
        var data = $('#hdnData').val();
        var ldata = $('#hdnLData').val();
        let nick = $('#totaBoxId').val();
        let pageId = '106518517401564';
        let name = $('#txt_1').val();
        let mobile = $('#txt_2').val();
        let email = $('#txt_3').val();
        let dob = $('#txt_4').val();
        var allTrx = new Object();
        allTrx.doctorId = data;
        allTrx.weekId = ldata;
        allTrx.nick = nick;
        allTrx.pageId = pageId;
        allTrx.name = name;
        allTrx.mobile = mobile;
        allTrx.email = email;
        allTrx.dob = dob;
        //allTrx.gender = ldata;

        $.ajax({
            url: "/Appointment/SavePatientInfo",
            type: 'POST',
            data: allTrx,
        })
            .done(function (datas) {
                $('#hdnMobile').val(datas);
                ClickedAppointment(data, ldata, 4, 'otp')
            });

    }
    else if (newID == 6) {
        let nick = $('#totaBoxId').val();
        let pageId = '106518517401564';
        let otp = $('#otp_1').val();
        let combinedId = pageId + nick;
        $.ajax({
            url: "/api/Appointment/OTPVerified/" + combinedId + "/" + otp,
        })
            .done(function (res) {
                if (res.msg == 'success') {
                    $('#msgOTP').html('');
                    AddSendMessage(otp);
                    var msg = "<span>Please select the date for appointment:</span>";
                    $.each(res.dates, function (i, e) {
                        msg += `<button onclick=fnProfileDate("${e}") id ="appDate" class="actionDate">${e}</button>`;
                    });

                    AddServerMessage(msg);
                }
                else {
                    $('#msgOTP').html('Invalid OTP!');
                }
            });
    }
    else if (newID == 7) {
        var data = $('#hdnData6').val();
        var ldata = $('#hdnLData6').val();
        let uhid = $('#txt_6').val();
        AddSendMessage(uhid);
        var msg = "<span>No patient details found!</span>";
        AddServerMessage(msg);

        var msg2 = "Please fill the patient's details";

        AddServerMessage(msg2);

        var newmsg = `<table class="table table-responsive" style="color:black;width:100%;">
                                <tr id="tr_1"><td><input id="txt_1" placeholder="Enter patient's fullname" style="width:100%;" type="text" class=""></td><td><span id="btn_1" class="btn btn-sm btn-info" onclick="fnProfile(1)" style="cursor:pointer"><i id="ic_1" class="fa fa-angle-right"></i></span></td></tr>
                                <tr id="tr_2" style="display:none;"><td><input id="txt_2" placeholder="Enter mobile no" style="width:100%;" type="text" class=""><span id="msg_2" style="color:red;"></span></td><td><span id="btn_2" class="btn btn-sm btn-info" onclick="fnProfile(2)" style="cursor:pointer"><i id="ic_2" class="fa fa-angle-right"></i></span></td></tr>
                                <tr id="tr_3" style="display:none;"><td><input id="txt_3" placeholder="Email id" style="width:100%;" type="text" class=""><span id="msg_3" style="color:red;"></span></td><td><span id="btn_3" class="btn btn-sm btn-info" onclick="fnProfile(3)" style="cursor:pointer"><i id="ic_3" class="fa fa-angle-right"></i></span></td></tr>
                                <tr id="tr_4" style="display:none;"><td><input id="txt_4" placeholder="Date Of Birth (DD/MM/YEAR)" style="width:100%;" type="text" class=""></td><td><span id="btn_4" class="btn btn-sm btn-info" onclick="fnProfile(4)" style="cursor:pointer"><i id="ic_4" class="fa fa-angle-right"></i></span></td></tr>
                            </table>`;
        newmsg += `<input type="hidden" id="hdnData" value="${data}" />`
        newmsg += `<input type="hidden" id="hdnLData" value="${ldata}" />`
        AddSendMessage(newmsg);
    }
}

function fnProfileDate(date) {
    let nick = $('#totaBoxId').val();
    let pageId = '106518517401564';
    let combinedId = pageId + nick;
    $.ajax({
        url: "/api/Appointment/SaveAppointmentDate/" + combinedId + "/" + date,
    })
        .done(function (res) {
            if (res.msg == 'success') {
                AddSendMessage(date);
                var msg = "<span>Please select the time slot for appointment,</span><br />";
                msg += 'Morning:';
                $.each(res.lstMorning, function (i, e) {
                    msg += `<button onclick =fnProfileTime("${e}") id ="appTime" class="actionTime">${e}</button>`;
                });
                msg += '<br /><span>Evening:</span>'
                $.each(res.lstEvening, function (i, e) {
                    msg += `<button onclick =fnProfileTime("${e}") id ="appTime" class="actionTime">${e}</button>`;
                });

                AddServerMessage(msg);
            }
            else {
                $('#msgOTP').html('Invalid OTP!');
            }
        });
}


function fnProfileTime(time) {
    let nick = $('#totaBoxId').val();
    let pageId = '106518517401564';
    let combinedId = pageId + nick;
    $.ajax({
        url: "/api/Appointment/SaveAppointmentTime/" + combinedId + "/" + time,
    })
        .done(function (res) {

            if (res.msg == 'success') {
                AddSendMessage(time);
                //var msg = `<span" class="row"><span>Appointment with</span><br />
                //<span>${res.doctorName}</span><br /><span>${res.departmentName}</span><br/>
                //<div class="row"><div class="col-md-6">${res.date}</div><div class="col-md-6">${res.time}</div></div></span>`;
                var msg = `<span>Appointment with:</span>`;
                msg += `<div><span>${res.doctorName},</span><br /><span>${res.departmentName}</span><br />
                                                    <div style='text -align:center'><span class='actionDate'>${res.date}</span>&nbsp;<span class='actionTime'>${res.time}</span></div>
                                                    </div>`;
                AddServerMessage(msg);

                var msgLast = `<span>Please confirm if you want to book this appointment!</span><br />
                                <div style = "width:200%;text-align: center;" class="row" >
                    <button onclick=ClickedAppointmentConfirm('yes') id = "confirmYes" class="actionButton" > Confirm</button >& nbsp;& nbsp; <button onclick=ClickedAppointmentConfirm('no') id = "confirmNo" class="actionButton" > Cancel</button >
                    </div >`;
                AddServerMessage(msgLast);
            }
            else {
                $('#msgOTP').html('Invalid OTP!');
            }
        });
}


function ClickedAppointmentConfirm(type) {

    if (type == 'yes') {
        AddSendMessage('Confirm');
        let nick = $('#totaBoxId').val();
        let pageId = '106518517401564';
        let combinedId = pageId + nick;
        $.ajax({
            url: "/api/Appointment/SaveAppointmentConfirm/" + combinedId,
        })
            .done(function (res) {
                if (res.msg == 'success') {

                    var msg = `<span>Thanks for booking an appointment with us.Your booking details: Booking No:${res.bookingNo},${res.doctorName},${res.designationName},${res.date},${res.time}. Thank you for reaching out to us.</span>`;

                    AddServerMessage(msg);

                    var msg2 = `<span>Please let us know, what more I can help you with.</span>`;

                    AddServerMessage(msg2);
                    let message = 'menu';
                    let nick = $('#totaBoxId').val();
                    GenerateResponseJsonAndSend(message, cahtboxpostback, nick, 0);
                    cahtboxpostback = "";
                    event.preventDefault();
                }
                else {
                    AddSendMessage('Fail');
                }
            });
    } else {
        AddSendMessage('Cancel');

        var msg = `<span>Sorry, the appointment cannot be completed. Please let us know, what more I can help you with.</span>`;

        AddServerMessage(msg);
        let message = 'menu';
        let nick = $('#totaBoxId').val();
        GenerateResponseJsonAndSend(message, cahtboxpostback, nick, 0);
        cahtboxpostback = "";
        event.preventDefault();
    }

}


function ClickedMenuMenuQQ(data, Ldata) {
    var menuname = "";
    if (data == 1) {
        menuname = "হ্যাঁ";
        AddSendMessage(menuname);
        //Ldata = data;
        // AddServerMessage("<img style='height: 39px;width:73px;margin:-15px' src='/images/tenor.gif'/>");
        let nick = $('#totaBoxId').val();
        GenerateResponseJsonAndSend("", "passport,Bangla", nick, 0);

    }
    else if (data == 2) {
        menuname = "Yes";
        AddSendMessage(menuname);
        //Ldata = data;
        //  AddServerMessage("<img style='height: 39px;width:73px;margin:-15px' src='/images/tenor.gif'/>");
        let nick = $('#totaBoxId').val();
        GenerateResponseJsonAndSend("", "passport,English", nick, 0);
    }
    //else {

    //    $.ajax({
    //        url: "/Knowledge/getdatabyId/" + data,

    //    })
    //        .done(function (data) {
    //            if (Ldata == 0) {

    //                menuname = "হ্যাঁ";
    //                AddSendMessage(menuname);
    //                AddServerMessage("<img style='height: 39px;width:73px;margin:-15px' src='/images/tenor.gif'/>");
    //            }
    //            else {

    //                menuname = "yes";
    //                AddSendMessage(menuname);
    //                AddServerMessage("<img style='height: 39px;width:73px;margin:-15px' src='/images/tenor.gif'/>");
    //            }
    //        });
    //}

    //setTimeout(function () {
    //    let nick = $('#totaBoxId').val();

    //    GenerateResponseJsonAndSend("", "menuesq," + data + "," + menuname + "," + Ldata, nick);

    //    window.location.href = "#totaMessagesContainerLast";
    //}, 5000)



}
function ClickedMenuMenuQ(data, Ldata) {
    var menuname = "";
    if (data == -2 && Ldata == 0) {
        menuname = "না";
        AddSendMessage(menuname);
        //Ldata = data;
        AddServerMessage("<img style='height: 39px;width:73px;margin:-15px' src='/images/tenor.gif'/>");


    }
    else if (data == -2 && Ldata == -1) {
        menuname = "No";
        AddSendMessage(menuname);
        //Ldata = data;
        AddServerMessage("<img style='height: 39px;width:73px;margin:-15px' src='/images/tenor.gif'/>");
    }
    else {

        $.ajax({
            url: "/Knowledge/getdatabyId/" + data,

        })
            .done(function (data) {
                if (Ldata == 0) {

                    menuname = "হ্যাঁ";
                    AddSendMessage(menuname);
                    AddServerMessage("<img style='height: 39px;width:73px;margin:-15px' src='/images/tenor.gif'/>");
                }
                else {

                    menuname = "yes";
                    AddSendMessage(menuname);
                    AddServerMessage("<img style='height: 39px;width:73px;margin:-15px' src='/images/tenor.gif'/>");
                }
            });
    }

    setTimeout(function () {
        let nick = $('#totaBoxId').val();

        GenerateResponseJsonAndSend("", "menuesq," + data + "," + menuname + "," + Ldata, nick, 0);

        window.location.href = "#totaMessagesContainerLast";
    }, 5000)



}
var elmnt = document.getElementById("totaMessagesContainer");

function scrollToTop() {
    elmnt.scrollIntoView(false); // Top

}

//function showbubble() {
//    elmnt.attr("src",)
//}

// Close the dropdown if the user clicks outside of it
window.onclick = function (event) {
    if (!event.target.matches('.dropbtni')) {
        var dropdowns = document.getElementsByClassName("dropdown-content");
        var i;
        for (i = 0; i < dropdowns.length; i++) {
            var openDropdown = dropdowns[i];
            if (openDropdown.classList.contains('show')) {
                openDropdown.classList.remove('show');
            }
        }
    }
}



function setCookie(name, value, days) {
    const expirationDate = new Date();
    expirationDate.setDate(expirationDate.getDate() + days);
    const cookieValue = `${name}=${value}; expires=${expirationDate.toUTCString()}; path=/`;
    document.cookie = cookieValue;
}

function getCookie(name) {
    const cookies = document.cookie.split('; ');
    for (let i = 0; i < cookies.length; i++) {
        const cookie = cookies[i].split('=');
        if (cookie[0] === name) {
            // Decode the cookie value before returning
            return decodeURIComponent(cookie[1]);
        }
    }
    return null;
}

var msgLoaded = false;
function handleButtonClick(buttonElement, id) {
    //alert(id);
    //alert(buttonText);
    //console.log("btn val> ", id);
    var buttonText = $(buttonElement).text();

    if (buttonText == 'Male' || buttonText == 'Female') {
        localStorage.setItem('patient-gender', buttonText);
    }
    if (buttonText == 'Yes' || buttonText == 'No') {
        localStorage.setItem('patient-registered', buttonText);
    }

    AddSendMessage(buttonText);

    console.log(buttonText);
    localStorage.setItem('sentMessage', buttonText);

    GenerateResponseJsonAndSend(buttonText, '', id.toString(), id.toString());

}


function handleDrCardClick(buttonElement) {
    console.log(buttonElement);
    var buttonText = $(buttonElement).find('.drname').text();
    var drApiId = $(buttonElement).find('.drApiId').text();
    localStorage.setItem('drApiId', drApiId);
    var drdtalTop = $(buttonElement).find('.drdtal-top').html();
    var docaval = $(buttonElement).find('.doc-aval').html();
    var drBox = `<div id='single-doc-box'><div class='dr-box' style='align-content: center;background-color: white !important; width: 400px !important;margin-bottom: -20px; margin-left: -100px !important;  margin-top: -10px; padding: 10px;  height: 400px'><div class='graycard drdtal' style='align-content: center; background: white;border-radius: 6px; box-shadow: 0 0 5px 2px rgba(0,0,0,.2)!important;border-top: 2px solid blue;padding: 10px;'><div class='drdtal-top' style='color:black'>${drdtalTop}</div><div class='doc-aval'> ${docaval}</div><a class='btn btn-btm' href='javascript:void(0);' style='margin-left:50px;margin-top: 10px'>Request Appointment</a></div></div></div>`;
    AddSendMessage(drBox);
    AddSendMessage(buttonText);
    //localStorage.setItem('single-doc-box', drBox);
    //localStorage.getItem('single-doc-box');
    ////console.log(savedDrBox);
    GenerateResponseJsonAndSend(buttonText, '', 'docSearch', 0);
}

function clickOnAppointDate(buttonElement) {
    console.log(buttonElement);
    var buttonText = $(buttonElement).find('.app-date').val();
    localStorage.setItem('app-date', buttonText);
}
function handleSpecCardClick(buttonElement) {
    console.log(buttonElement);
    var buttonText = $(buttonElement).find('.specName').text(); 
    var speBox = `<div><div class='dept-box' style='align-content: center; background-color: white !important; width: 400px !important;margin-bottom: -20px; margin-left: -100px !important;  margin-top: -20px; padding: 10px;  height: 200px'><div class='graycard drdtal' style='background: white;box-shadow: 0 0 5px 2px rgba(0,0,0,.2) !important; border-radius: 6px;border-top: 2px solid blue;padding: 10px; width: 280px !important;' ><div class='drdtal-top'> <div class='drdtal-topright' style='text-align: center;'><p class='specName' style='text-align: center;color: black;'> ${buttonText}</p><small></small> <a class='btn btn-btm btn-sm' style='margin:1px 10px;width: 95%;'>Request Appointment</a></div></div></div></div></div>`;
    AddSendMessage(speBox);
    AddSendMessage(buttonText);
    localStorage.setItem('dr-box', speBox);
    //localStorage.getItem('dept-box');
    //console.log(savedDrBox);
    GenerateResponseJsonAndSend(buttonText, 'docSearch', 'docSearchBySpec', 0);
    
}
function handleDeptCardClick(buttonElement) {
    console.log(buttonElement);
    var buttonText = $(buttonElement).find('.deptName').text();
     
    AddSendMessage(buttonText);
    GenerateResponseJsonAndSend(buttonText, 'docSearch', 'docSearchByDept', 0);
}

 

function handleAppointmentDateClick(buttonElement, id) {
    var buttonText = $(buttonElement).text();
    var drApiId = localStorage.getItem('drApiId');

    var slotStatus = false;

    $.ajax({
        type: 'GET',
        url: '/Knowledge/GetDoctorByDoctorApiId?id=' + drApiId + '&date=' + buttonText,
        async: false,
        success: function (data) {
            console.log('Success:', data);
            slotStatus = data;
        },
        error: function (error) {
            console.error('Error:', error);
        }
    });
    if (slotStatus == true) { //slot available
        AddSendMessage(buttonText);
        GenerateAppointmentDateResponseJsonAndSend(buttonText, '', id.toString(), buttonText);
    }
    else {
        alert("Please choose another date, there is no schedule available.");
    }



}

function handleTimeSlotClick(buttonElement, id, slotId) {
    var buttonText = $(buttonElement).text();
    AddSendMessage(buttonText);
    GenerateTimeSlotResponseJsonAndSend(buttonText, '', id.toString(), slotId);

}


function GenerateAppointmentDateResponseJsonAndSend(message, postback, nick, date) {
    let obj = new Object();
    obj.pageId = '106518517401564';
    obj.message = message;
    obj.postback = postback;
    obj.botKey = $('.botKeyDefault').val();
    obj.dateTxt = date;
    let userId = $('#totaBoxNId').val();

    connection.invoke('Send', obj.pageId, JSON.stringify(obj), userId); // Invoking HUB Connection


    return;
}


function GenerateTimeSlotResponseJsonAndSend(message, postback, nick, slotId) {
    let obj = new Object();
    obj.pageId = '106518517401564';
    obj.message = message;
    obj.postback = postback;
    obj.botKey = $('.botKeyDefault').val();
    obj.slotId = slotId;
    let userId = $('#totaBoxNId').val();

    connection.invoke('Send', obj.pageId, JSON.stringify(obj), userId); // Invoking HUB Connection


    return;
}




function SubmitInputData(id, nextNodeId) {

    $('#dobbtn').hide();
    $('#dobbtn2').show();
    $('#phonebtn').hide();
    $('#phonebtn2').show();
    var prefix = $('select[name="numberCode"] :selected').val();
    var phone = $('input[name="valueText"]:last').val();
    var allControlsHaveValue = true;

    //$('.inp-control').each(function () {
    //    if (!$(this).val()) {
    //        allControlsHaveValue = false;
    //        return false;
    //    }
    //});

    if (allControlsHaveValue) {
        var data = $('#frmBasic').serialize();
        $.ajax({
            type: 'GET',
            url: '/Knowledge/AddMessageLogByQuestionId?id=' + id,
            async: false,
            success: function (data) {
                console.log('Success:', data);
            },
            error: function (error) {
                console.error('Error:', error);
            }
        });


        $.ajax({
            url: '/Knowledge/SaveInputGroup',
            type: 'POST',
            async: false,
            data: data,
            success: function (response) {
                console.log('response', response);


                if (response.length > 0) {
                    $('.frmBasic').attr('id', '');

                    if (countOccurrences(data, 'valueText') == 1) {
                        AddSendMessage(prefix + phone);
                    }

                    $('.conversation-wrapper').append(`<div class="loader-box"><img src="https://i.gifer.com/ZhKG.gif""></div>`);
                    $(".conversation-wrapper").animate({ scrollTop: $(".conversation-wrapper")[0].scrollHeight }, 200);

                    setTimeout(function () {
                        $.each(response, function (key, value) {
                            AddServerMessage(value);
                        });

                        $('.conversation-wrapper .loader-box').remove();
                    }, 200);




                }
            },
            error: function (error) {
                console.error('Error:', error);
            }
        });

        console.log(countOccurrences(data, 'valueText'));
    } else {
        alert('Please fill all the fields.1');
    }

}


function countOccurrences(data, searchTerm) {
    var regex = new RegExp(searchTerm, 'g');
    var matches = data.match(regex);
    return matches ? matches.length : 0;
}




function SubmitOTPCode() {
    var otp = $('#otpCode').val();

    var uhid = localStorage.getItem('uhidData');
    
    if (otp != '') {

        $.ajax({
            url: '/Knowledge/CheckAndValidateOTP?otp=' + otp +'&uhid='+ uhid,
            method: 'GET',
            async: false,
            success: function (response) {
                console.log('otp', response);
                $('.otpCode').attr('id', '');

                if (response.length > 0) {
                    AddSendMessage(otp);

                    $('.conversation-wrapper').append(`<div class="loader-box"><img src="https://i.gifer.com/ZhKG.gif""></div>`);
                    $(".conversation-wrapper").animate({ scrollTop: $(".conversation-wrapper")[0].scrollHeight }, 200);

                    setTimeout(function () {
                        $.each(response, function (key, value) {
                            AddServerMessage(value);
                        });

                        $('.conversation-wrapper .loader-box').remove();
                    }, 200);

                }
                else {
                    alert('Invalid OTP SEND!');
                }
            },
            error: function (error) {
                alert('Invalid OTP!');
                console.error('Error:', error);

            }
        });
    }
}




function ManageMessageInput(message) {
    if (message.indexOf('<div') >= 0 && wrapperId != 10 && wrapperId != 6 && wrapperId != 7) {
        console.log(false);
        $('.typing').hide();

    }
    else {
        console.log(true);
        $('.typing').show();
        $('.typing').css('margin-top', '-47px;');
    }
}



function RescheduleSchedule(scheduleId, doctorId, nodeId, nextNodes, doctorName, scheduleTime, scheduleDate, phone, connectionId, botKey, uhid) {
    if (doctorName != 'doctorName') { 
        localStorage.setItem('drApiId', doctorId);
        localStorage.setItem('drname', doctorName);
        localStorage.setItem('uhidData', uhid);
        $.ajax({
            url: '/Knowledge/SaveDoctorNameInServiceFlow?nextNodeId=' + nextNodes + '&connectionId=' + connectionId + '&botKey=' + botKey + '&doctorName=' + doctorName + "&scheduleId=" + scheduleId,
            method: "GET",
            async: false,
            success: function (data) {
                console.log(data);
            },
            error: function (error) {

            }
        });
    }




    $.ajax({
        url: '/Knowledge/GetNextMessageByNodeId?nextNodeId=' + nextNodes + '&connectionId=' + connectionId + '&botKey=' + botKey,
        type: 'GET',
        success: function (response) {
            console.log(response);

            if (response.length > 0) {
                AddSendMessage(doctorName);

                $('.conversation-wrapper').append(`<div class="loader-box"><img src="https://i.gifer.com/ZhKG.gif""></div>`);
                $(".conversation-wrapper").animate({ scrollTop: $(".conversation-wrapper")[0].scrollHeight }, 200);

                setTimeout(function () {
                    $.each(response, function (key, value) {
                        AddServerMessage(value);
                    });

                    $('.conversation-wrapper .loader-box').remove();
                }, 200);

            }
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
        }
    });
}



function ChangeSchedule(nextNodes, connectionId, botKey) {

    $.ajax({
        url: '/Knowledge/GetNextMessageByNodeId?nextNodeId=' + nextNodes + '&connectionId=' + connectionId + '&botKey=' + botKey,
        type: 'GET',
        success: function (response) {
            console.log(response);

            if (response.length > 0) {
                AddSendMessage(doctorName);

                $('.conversation-wrapper').append(`<div class="loader-box"><img src="https://i.gifer.com/ZhKG.gif""></div>`);
                $(".conversation-wrapper").animate({ scrollTop: $(".conversation-wrapper")[0].scrollHeight }, 200);

                setTimeout(function () {
                    $.each(response, function (key, value) {
                        AddServerMessage(value);
                    });

                    $('.conversation-wrapper .loader-box').remove();
                }, 200);

            }
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
        }
    });
}



function GetNextMessage(nextNodes, connectionId, botKey) {


    var otp = $('#otpCode').val();

    var uhid = localStorage.getItem('uhidData');
    if (otp != '') {

        $.ajax({
            url: '/Knowledge/CheckAndValidateOTP?otp=' + otp + '&uhid=' + uhid,
            method: 'GET',
            async: false,
            success: function (response) {
                console.log('otp', response);
                $('.otpCode').attr('id', '');

                if (response.length > 0) {


                    $.ajax({
                        url: '/Knowledge/GetNextMessageByNodeId?nextNodeId=' + nextNodes + '&connectionId=' + connectionId + '&botKey=' + botKey,
                        type: 'GET',
                        success: function (response) {
                            console.log(response);

                            if (response.length > 0) {

                                $('.conversation-wrapper').append(`<div class="loader-box"><img src="https://i.gifer.com/ZhKG.gif""></div>`);
                                $(".conversation-wrapper").animate({ scrollTop: $(".conversation-wrapper")[0].scrollHeight }, 200);

                                setTimeout(function () {
                                    $.each(response, function (key, value) {
                                        AddServerMessage(value);
                                    });

                                    $('.conversation-wrapper .loader-box').remove();
                                }, 200);

                            }
                        },
                        error: function (xhr, status, error) {
                            console.error(xhr.responseText);
                        }
                    });


                    AddSendMessage(otp);
                }
                else {
                    alert('Invalid OTP SEND!');
                }
            },
            error: function (error) {
                alert('Invalid OTP!');
                console.error('Error:', error);

            }
        });
    }
}



function CancelSchedule(scheduleId, doctorId, nodeId, nextNodes, doctorName, scheduleTime, scheduleDate, phone, connectionId, botKey) {
    $.ajax({
        url: '/Knowledge/CancelAppointmentById?scheduleId=' + scheduleId + '&connectionId=' + connectionId + '&botKey=' + botKey,
        method: 'GET',
        dataType: 'json',
        success: function (response) {
            console.log(response);


            $('.conversation-wrapper').append(`<div class="loader-box"><img src="https://i.gifer.com/ZhKG.gif""></div>`);
            $(".conversation-wrapper").animate({ scrollTop: $(".conversation-wrapper")[0].scrollHeight }, 200);

            setTimeout(function () {
                $.each(response, function (key, value) {
                    AddServerMessage(value);
                });

                $('.conversation-wrapper .loader-box').remove();
            }, 200);


        },
        error: function (xhr, status, error) {
            console.error('Error:', status, error);
        }
    });


}






function openshift(timeshift, id) {
    if (timeshift == 2) {
        $('#evening').show();
        $('#morning').hide();
    }
    else if (timeshift == 1) {
        $('#evening').hide();
        $('#morning').show();
    }
    else {

    }
}




function changeDate() {

}


function validatePhone(inputElement) {
    var inputValue = $(inputElement).val().replace(/\D/g, '');

    if (inputValue.length > 0 && inputValue[0] === '0') {
        inputValue = inputValue.slice(1);
    }

    if (inputValue.length > 10) {
        inputValue = inputValue.slice(0, 10);
    }

    $(inputElement).val(inputValue);
}

function validateEmail(input) {
    var emailValue = $(input).val();

    if (/^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/.test(emailValue)) {
        $(input).css('border', '1px solid lightgrey');
    } else {
        $(input).css('border', '1px solid red');
    }
}

//function SubmitInputField2(buttonElement) {
//    debugger;
//    $('.formMobile').remove();
//    $('#name input[type="text"]').attr('id', '');
//    $('#phone input[type="text"]').attr('id', '');
//    $('#email input[type="text"]').attr('id', '');
//    $('#dob input[type="text"]').attr('id', '');

//}
function SubmitInputField(fieldtype, id) {
    if (fieldtype == 1) {
        //alert($('#name input[type="text"]').val());
        if ($('#name input[type="text"]').val() && $('#name input[type="text"]').val().length > 2) {
            $('#name').show();
            $('#namebtn').hide();
            $('#namebtn2').show();
            $('#phone').show();
            $('#email').hide();
            $('#dob').hide();
            $('#name input[type="text"]').attr('readonly', true);
            $('#step1').hide();
            $('#step2').show();
        }

    }
    else if (fieldtype == 2) {
        if ($('#phone input[type="text"]').val() && $('#phone input[type="text"]').val().length >= 10) {
            $('#name').show();
            $('#phone').show();
            $('#email').show();
            $('#dob').hide();
            $('#namebtn2').show();
            $('#phonebtn2').show();
            $('#phonebtn').hide();
            $('#phone input[type="text"]').attr('readonly', true);
            $('#inputGroupSelect04').attr('readonly', true);
            $('#step1').hide();
            $('#step2').hide();
            $('#step3').show();
        }

    }
    else if (fieldtype == 3) {
        var email = $('#email input[type="text"]').val();

        if (email && email.indexOf('@') > 0 && email.indexOf('.') > 0) {
            var domain = email.split('.').pop();
            if (domain.length >= 2) {

                console.log('Email is valid');
                $('#name').show();
                $('#phone').show();
                $('#email').show();
                $('#dob').show();
                $('#namebtn2').show();
                $('#phonebtn2').show();
                $('#phonebtn').hide();
                $('#emailbtn2').show();
                $('#emailbtn').hide();
                $('#dobbtn2').hide();
                $('#dobbtn').show();
                $('#email input[type="text"]').attr('readonly', true);
                $('#step1').hide();
                $('#step2').hide();
                $('#step3').hide();
                $('#step4').show();
            } else {

                console.log('Domain should have at least 2 characters');
            }
        } else {

            console.log('Email is invalid');
        }
    }
    else {

    }

}
function SubmitInputMobileField(id, nextNodeId) {
    //debugger
    if ($('#phone input[type="text"]').val()) {
        var uhid = "";
        var nextNodeId = $('#nextNodeId').val();
        var botKey = $('#botKey').val();
        var connectionId = $('#connectionId').val();
        var prefix = $('select[name="numberCode"] :selected').val();
        var phone = $('input[name="valueText"]').val();
        var allControlsHaveValuedata = true;


        if (allControlsHaveValuedata) {
            var data = $('#frmBasic').serialize();
            $.ajax({
                type: 'GET',
                url: '/Knowledge/AddMessageLogByQuestionId?id=' + id,
                async: false,
                success: function (data) {
                    console.log('Success:', data);

                },
                error: function (error) {
                    console.error('Error:', error);
                }
            });

            $.ajax({
                url: '/Home/CheckApiMobile?uhid=' + uhid + '&prefix=' + prefix + '&phone=' + phone + '&nextNodeId=' + nextNodeId + '&botKey=' + botKey + '&connectionId=' + connectionId,
                method: "GET",
                async: false,
                success: function (response) {
                    console.log('response', response);

                    if (response.length > 0) {
                        $('.frmBasic').attr('id', '');
                       // $('.frmBasic').remove();
                        if (countOccurrences(data, 'valueText') == 1) {
                            AddSendMessage(prefix + phone);
                        }

                        $('.conversation-wrapper').append(`<div class="loader-box"><img src="https://i.gifer.com/ZhKG.gif""></div>`);
                        $(".conversation-wrapper").animate({ scrollTop: $(".conversation-wrapper")[0].scrollHeight }, 200);

                        setTimeout(function () {
                            $.each(response, function (key, value) {
                                AddServerMessage(value);
                            });

                            $('.conversation-wrapper .loader-box').remove();
                        }, 200);

                        $('#valueText').prop('id', '');

                    }
                },
                error: function (error) {
                    console.error('Error:', error);
                    alert('Invalid value');
                }
            });

        } else {
            alert('Please fill all the fields1.');
        }
    }
    else {
        var uhid = "";
        var nextNodeId = $('#nextNodeId').val();
        var botKey = $('#botKey').val();
        var connectionId = $('#connectionId').val();
        var prefix = $('select[name="numberCode"] :selected').val();
        //var phone = $('input[name="valueText"]').val();

        var phone = $('#valueText').val();
        var allControlsHaveValuedata = true;


        if (allControlsHaveValuedata) {
            var data = $('#frmBasic').serialize();
            $.ajax({
                type: 'GET',
                url: '/Knowledge/AddMessageLogByQuestionId?id=' + id,
                async: false,
                success: function (data) {
                    console.log('Success:', data);

                },
                error: function (error) {
                    console.error('Error:', error);
                }
            });

            $.ajax({
                url: '/Home/CheckApiMobile?uhid=' + uhid + '&prefix=' + prefix + '&phone=' + phone + '&nextNodeId=' + nextNodeId + '&botKey=' + botKey + '&connectionId=' + connectionId,
                method: "GET",
                async: false,
                success: function (response) {
                    console.log('response', response);

                    if (response.length > 0) {
                        $('.frmBasic').attr('id', '');

                        if (countOccurrences(data, 'valueText') == 1) {
                            AddSendMessage(prefix + phone);
                        }

                        $('.conversation-wrapper').append(`<div class="loader-box"><img src="https://i.gifer.com/ZhKG.gif""></div>`);
                        $(".conversation-wrapper").animate({ scrollTop: $(".conversation-wrapper")[0].scrollHeight }, 200);

                        setTimeout(function () {
                            $.each(response, function (key, value) {
                                AddServerMessage(value);
                            });

                            $('.conversation-wrapper .loader-box').remove();
                        }, 200);


                        $('#valueText').prop('id', '');
                    }
                },
                error: function (error) {
                    console.error('Error:', error);
                    alert('Invalid value');
                }
            });

        } else {
            alert('Please fill all the fields1.');
        }
        //alert('Please fill all the fields2.');
    }
}
function SubmitInputUhidField(id, nextNodeId) { 
     
    if ($('#uhid input[type="text"]').val()) {
        var uhid = $('#uhid input[type="text"]').val();
        var nextNodeId = $('#nextNodeId').val();
        var botKey = $('#botKey').val();
        var connectionId = $('#connectionId').val();
        var phone = "";
        var prefix = "";
        var allControlsHaveValuedata = true;
        localStorage.setItem('uhidData', uhid); 

        if (allControlsHaveValuedata) {
            var data = $('#frmBasic').serialize();
            $.ajax({
                type: 'GET',
                url: '/Knowledge/AddMessageLogByQuestionId?id=' + id,
                async: false,
                success: function (data) {
                    console.log('Success:', data);

                },
                error: function (error) {
                    console.error('Error:', error);
                }
            });

            $.ajax({
                url: '/Home/CheckUhid?uhid=' + uhid + '&prefix=' + prefix + '&phone=' + phone + '&nextNodeId=' + nextNodeId + '&botKey=' + botKey + '&connectionId=' + connectionId,
                method: "GET",
                async: false,
                success: function (response) {
                    console.log('response', response);

                    if (response.length > 0) {
                        $('.frmBasic').attr('id', '');
                        //$('.frmBasic').remove();
                        if (countOccurrences(data, 'valueText') == 1) {
                             
                            AddSendMessage(uhid);
                             
                        }
                        $('.conversation-wrapper').append(`<div class="loader-box"><img src="https://i.gifer.com/ZhKG.gif""></div>`);
                        $(".conversation-wrapper").animate({ scrollTop: $(".conversation-wrapper")[0].scrollHeight }, 200);

                        setTimeout(function () {
                            $.each(response, function (key, value) {
                                AddServerMessage(value);
                            });

                            $('.conversation-wrapper .loader-box').remove();
                        }, 200);

                        $('#valueText').prop('id', '');

                    }
                },
                error: function (error) {
                    console.error('Error:', error);
                }
            });


        } else {
            alert('Please fill all the fields.');
        }
    }
    else {
        var uhid = $('#uhid input[type="text"]').val();
        var nextNodeId = $('#nextNodeId').val();
        var botKey = $('#botKey').val();
        var connectionId = $('#connectionId').val();
        var phone = "";
        var prefix = "";
        var allControlsHaveValuedata = true;
        localStorage.setItem('uhidData', uhid);

        if (allControlsHaveValuedata) {
            var data = $('#frmBasic').serialize();
            $.ajax({
                type: 'GET',
                url: '/Knowledge/AddMessageLogByQuestionId?id=' + id,
                async: false,
                success: function (data) {
                    console.log('Success:', data);

                },
                error: function (error) {
                    console.error('Error:', error);
                }
            });

            $.ajax({
                url: '/Home/CheckUhid?uhid=' + uhid + '&prefix=' + prefix + '&phone=' + phone + '&nextNodeId=' + nextNodeId + '&botKey=' + botKey + '&connectionId=' + connectionId,
                method: "GET",
                async: false,
                success: function (response) {
                    console.log('response', response);

                    if (response.length > 0) {
                        $('.frmBasic').attr('id', '');
                        //$('.frmBasic').remove();
                        //if (countOccurrences(data, 'valueText') == 1) {
                        //    AddSendMessage(uhid);
                        //}
                        $('.conversation-wrapper').append(`<div class="loader-box"><img src="https://i.gifer.com/ZhKG.gif""></div>`);
                        $(".conversation-wrapper").animate({ scrollTop: $(".conversation-wrapper")[0].scrollHeight }, 200);

                        setTimeout(function () {
                            $.each(response, function (key, value) {
                                AddServerMessage(value);
                            });

                            $('.conversation-wrapper .loader-box').remove();
                        }, 200);

                        $('#valueText').prop('id', '');

                    }
                },
                error: function (error) {
                    console.error('Error:', error);
                }
            });


        } else {
            alert('Please fill all the fields.');
        }
        //alert('Please fill all the fields.');
    }

}

function SubmitUHID() {

    if ($('#uhid').val()) {
        var uhid = $('#uhid').val();
        var nextNodeId = $('#nextNodeId').val();
        var botKey = $('#botKey').val();
        var connectionId = $('#connectionId').val();
        
        var phone = "";
        var prefix = "";
        var allControlsHaveValuedata = true;

        localStorage.setItem('uhidData', uhid);
        if (allControlsHaveValuedata) {
            var data = $('#frmBasic2').serialize();
             
            $.ajax({
                url: '/Home/CheckUhid?uhid=' + uhid + '&prefix=' + prefix + '&phone=' + phone + '&nextNodeId=' + nextNodeId + '&botKey=' + botKey + '&connectionId=' + connectionId,
                method: "GET",
                async: false,
                success: function (response) {
                    console.log('response', response);

                    if (response.length > 0) {
                        $('.frmBasic').attr('id', '');
                         
                        $('.conversation-wrapper').append(`<div class="loader-box"><img src="https://i.gifer.com/ZhKG.gif""></div>`);
                        $(".conversation-wrapper").animate({ scrollTop: $(".conversation-wrapper")[0].scrollHeight }, 200);

                        setTimeout(function () {
                            $.each(response, function (key, value) {
                                AddServerMessage(value);
                            });

                            $('.conversation-wrapper .loader-box').remove();
                        }, 200);

                        $('#valueText').prop('id', '');

                    }
                },
                error: function (error) {
                    console.error('Error:', error);
                }
            });


        } else {
            alert('Please fill all the fields.');
        }
    }
    else {
        var uhid = $('#uhid input[type="text"]').val();
        var nextNodeId = $('#nextNodeId').val();
        var botKey = $('#botKey').val();
        var connectionId = $('#connectionId').val();
        var phone = "";
        var prefix = "";
        var allControlsHaveValuedata = true;
        localStorage.setItem('uhidData', uhid);

        if (allControlsHaveValuedata) {
            var data = $('#frmBasic2').serialize();
            $.ajax({
                type: 'GET',
                url: '/Knowledge/AddMessageLogByQuestionId?id=' + id,
                async: false,
                success: function (data) {
                    console.log('Success:', data);

                },
                error: function (error) {
                    console.error('Error:', error);
                }
            });

            $.ajax({
                url: '/Home/CheckUhid?uhid=' + uhid + '&prefix=' + prefix + '&phone=' + phone + '&nextNodeId=' + nextNodeId + '&botKey=' + botKey + '&connectionId=' + connectionId,
                method: "GET",
                async: false,
                success: function (response) {
                    console.log('response', response);

                    if (response.length > 0) {
                        $('.frmBasic').attr('id', '');
                        //$('.frmBasic').remove();
                        //if (countOccurrences(data, 'valueText') == 1) {
                        //    AddSendMessage(uhid);
                        //}
                        $('.conversation-wrapper').append(`<div class="loader-box"><img src="https://i.gifer.com/ZhKG.gif""></div>`);
                        $(".conversation-wrapper").animate({ scrollTop: $(".conversation-wrapper")[0].scrollHeight }, 200);

                        setTimeout(function () {
                            $.each(response, function (key, value) {
                                AddServerMessage(value);
                            });

                            $('.conversation-wrapper .loader-box').remove();
                        }, 200);

                        $('#valueText').prop('id', '');

                    }
                },
                error: function (error) {
                    console.error('Error:', error);
                }
            });


        } else {
            alert('Please fill all the fields.');
        }
        //alert('Please fill all the fields.');
    }

}


function validateInput() {
    var inputField = document.querySelector('input[name="valueText"]');
    var submitBtn = document.getElementById('namebtn');

    if (inputField.value.trim() !== '') {
        submitBtn.removeAttribute('disabled');
    } else {
        submitBtn.setAttribute('disabled', 'disabled');
    }
}

var cname = '';
var cphone = '';
var cemail = '';
var cdob = '';


 
function SubmitInputDataField(id, nextNodeId) {
    if ($('#dob input[type="date"]').val()) {
        cname = $('#name input[type="text"]').val();
        cphone = $('#phone input[type="text"]').val();
        cemail = $('#email input[type="text"]').val();
        cdob = $('#dob input[type="date"]').val();

        $('#dobbtn').hide();
        $('#dobbtn2').show();
        $('#dob input[type="date"]').attr('readonly', true);

        var prefix = $('select[name="numberCode"] :selected').val();
        var phone = $('input[name="valueText"]').val();
        var allControlsHaveValuedata = true;


        if (allControlsHaveValuedata) {
            var data = $('#frmBasic').serialize();
            $.ajax({
                type: 'GET',
                url: '/Knowledge/AddMessageLogByQuestionId?id=' + id,
                async: false,
                success: function (data) {
                    console.log('Success:', data);

                },
                error: function (error) {
                    console.error('Error:', error);
                }
            });


            $.ajax({
                url: '/Knowledge/SaveInputGroup',
                type: 'POST',
                async: false,
                data: data,
                success: function (response) {
                    console.log('response', response);

                    if (response.length > 0) {
                        $('.frmBasic').attr('id', '');

                        if (countOccurrences(data, 'valueText') == 1) {
                            AddSendMessage(prefix + phone);
                        }

                        $('.conversation-wrapper').append(`<div class="loader-box"><img src="https://i.gifer.com/ZhKG.gif""></div>`);
                        $(".conversation-wrapper").animate({ scrollTop: $(".conversation-wrapper")[0].scrollHeight }, 200);

                        setTimeout(function () {
                            $.each(response, function (key, value) {
                                AddServerMessage(value);
                            });

                            $('.conversation-wrapper .loader-box').remove();
                        }, 200);



                    }
                },
                error: function (error) {
                    console.error('Error:', error);
                }
            });

            $('#name input[type="text"]').val(cname);
            $('#phone input[type="text"]').val(cphone);
            $('#email input[type="text"]').val(cemail);
            $('#dob input[type="date"]').val(cdob);

            console.log(countOccurrences(data, 'valueText'));
        } else {
            alert('Please fill all the fields.4');
        }
    }
    else {
        alert('Please fill all the fields.5');
    }


    localStorage.setItem('patient-name', $('#nameValue').val());
    localStorage.setItem('patient-phone', $('#phoneValue').val());
    localStorage.setItem('patient-email', $('#emailValue').val());
    localStorage.setItem('patient-dob', $('#dobValue').val());


}

 
function LoadBasicFormData() {
    var patientName = localStorage.getItem('patient-name');
    var patientPhone = localStorage.getItem('patient-phone');
    var patientEmail = localStorage.getItem('patient-email');
    var patientDob = localStorage.getItem('patient-dob');

    if (patientName != null && patientName != '' && patientName != undefined) {
        $('#nameValue').val(patientName);
        $('#nameValue').attr('readonly', 'readonly');
    }
    if (patientPhone != null && patientPhone != '' && patientPhone != undefined) {
        $('#phoneValue').val(patientPhone);
        $('#inputGroupSelect04').attr('readonly', 'readonly');
        $('#phoneValue').attr('readonly', 'readonly');
    }
    if (patientEmail != null && patientEmail != '' && patientEmail != undefined) {
        $('#emailValue').val(patientEmail);
        $('#emailValue').attr('readonly', 'readonly');
    }
    if (patientDob != null && patientDob != '' && patientDob != undefined) {
        $('#dobValue').val(patientDob);
        $('#dobValue').attr('readonly', 'readonly');
    }
}
function SubmitOtp(nextNodeId) {
    //debugger

    var botKey = $('#botKey').val();
    var connectionId = $('#connectionId').val();
    var allControlsHaveValuedata = true;
    var uhid = localStorage.getItem('uhidData');

    if (allControlsHaveValuedata) {


        $.ajax({
            url: '/Home/CheckOtp?nextNodeId=' + nextNodeId + '&botKey=' + botKey + '&connectionId=' + connectionId,
            method: "GET",
            async: false,
            success: function (response) {
                console.log('response', response);

                if (response.length > 0) {

                    $('.conversation-wrapper').append(`<div class="loader-box"><img src="https://i.gifer.com/ZhKG.gif""></div>`);
                    $(".conversation-wrapper").animate({ scrollTop: $(".conversation-wrapper")[0].scrollHeight }, 200);

                    setTimeout(function () {
                        $.each(response, function (key, value) {
                            AddServerMessage(value);
                        });

                        $('.conversation-wrapper .loader-box').remove();
                    }, 200);

                    /*$('#valueText').prop('id', '');*/

                }
            },
            error: function (error) {
                console.error('Error:', error);
                alert('Invalid value');
            }
        });

    } else {
        alert('Please fill all the fields1.');
    }
}




























