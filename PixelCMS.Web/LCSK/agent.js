var myHub = null;

var agent = {
    id: '',
    name: '',
    isOnline: false
};

var chatMessages = [];

$(function () {
    $('#agent-chat').hide();

    myHub = $.connection.chatHub;

    $('#login-btn').click(function () {
        var agentName = $('#login-name').val();
        var agentPass = $('#login-pass').val();
        
        $.connection.hub.start()
            .done(function () {
                myHub.server.agentConnect(agentName, agentPass);
            })
            .fail(function () { alert('unable to connect'); });
    });

    $('#show-real-time-visits').click(function () {
        $('.chat-session').removeClass('active');
        showChat('rt');
        $("#divusers").hide();
    });

    $('#show-internal-chat').click(function () {
        $("#divusers").show();
        $('.chat-session').removeClass('active');
        showChat('internal');

        var internalBadge = $('#show-internal-chat').find('.badge');
        if (internalBadge != null && internalBadge != undefined) {
            if (internalBadge.hasClass('badge-warning')) {
                internalBadge.removeClass('badge-warning');
            }
            internalBadge.text('...');
        }
    });

    $('#change-status').click(function () {
        agent.isOnline = !agent.isOnline;
        myHub.server.changeStatus(agent.isOnline);

        showStatus();
    });

    $('#chat-sessions').on({
        click: function () {
            var chatId = $(this).data('id');

            $('.chat-session').removeClass('active');
            $(this).addClass('active');

            for (var i = 0; i < chatMessages[chatId].length; i++) {
                $('#chatmsgs' + chatId).append('<p>' + chatMessages[chatId][i] + '</p>');
            }

            scrollDiv($('#chatmsgs' + chatId));

            showChat(chatId);

            chatMessages[chatId] = [];

            var badge = $(this).find('.badge');
            if (badge != null && badge != undefined) {
                badge.removeClass('badge-warning');
                badge.text('0');
            }
        }
    }, '.chat-session');

    $('#chat-sessions').on({
        click: function (event) {
            event.preventDefault();
            event.stopPropagation();

            var div = $(this).parent().parent();
            var chatId = div.data('id');

            myHub.server.closeChat(chatId);

            div.remove();
            $('#chatmsgs' + chatId).remove();

            $('.chat-session').removeClass('active');
            showChat('rt');

            return false;
        }
    }, '.close-chat');

    $('#post-msg').keypress(function (e) {
        if (e.keyCode == 13) {
            e.preventDefault();
            e.stopPropagation();
            postMsg();
        }
    });

    $('#post-btn').click(function () {
        postMsg();
    });

    $('#current-visits').on({
        click: function (e) {
            e.preventDefault();

            var connectionId = $(this).data('id');

            myHub.server.engageVisitor(connectionId);
        }
    }, '.engage-visitor');

    myHub.client.loginResult = function (status, id, name) {
        if (status) {
            agent.id = id;
            agent.name = name;
            agent.isOnline = true;

            showStatus();
            $('#login').remove();
            $('#agent-chat').show();

            // Let's display the real time visitor
            showChat('rt');

        } else {
            var msg = id == 'config' ?
                'No configuration file found. Please configure LCSK before using it' :
                'No agent matches those credentials.';

            $('#login-alerts').html('<div class="alert alert-error">' +
                '<button type="button" class="close" data-dismiss="alert">×</button>' + 
                '<strong>Oops!</strong> ' + msg + ' </div>');
        }
    };

    myHub.client.newVisit = function (page, referrer, chatWith, connectionId) {
        var d = new Date();
        $('#current-visits > tbody').prepend(
            '<tr><td><abbr class="timeago" title="' + d.toISOString() + '">' + d.toISOString() + '</abbr></td>' +
            '<td>' + page + '</td>' +
            '<td>' + referrer + '</td>' +
            '<td>' + (chatWith != null ? chatWith : 'đang không chat') + '</td>' +
            '<td><a href="#" class="engage-visitor" data-id="' + connectionId + '">Gửi lời chào</a></td></tr>');

        $('#current-visits > tbody:first').find('abbr.timeago').timeago();
    };

    myHub.client.newChat = function (id) {
        var snd = new Audio('assets/sounds/newchat.mp3');
        snd.play();

        var d = new Date();

        var session = [];
        session.push('Chat started at ' + d.getHours() + ':' + d.getMinutes());
        
        chatMessages.push(id);
        chatMessages[id] = session;

        $('#chat-sessions').prepend(
            '<div id="chat' + id + '" class="row chat-session" data-id="' + id + '">' +
            '<div class="col-md-6"><abbr class="timeago" title="' + d.toISOString() + '">' + d.toISOString() + '</abbr></div>' +
            '<div class="col-md-6" style="text-align: right;"><a class="close-chat btn btn-mini" href="#"><span class="glyphicon glyphicon-remove"></span></a>' +
            '<p>New message(s) <span class="badge badge-warning">0</span></p></div>' +
            '</div>');

        $('#all-chatbox').append(
            '<div id="chatmsgs' + id + '" class="chat-msgs"></div>'
        );

        $('#chatmsgs' + id).hide();

        $('#chat' + id).find('abbr.timeago').timeago();
    };

    myHub.client.visitorSwitchPage = function (lastId, newId, newPage) {
        chatMessages.splice(lastId);

        var session = [];
        session.push('<strong>system</strong> Khách đang truy cập trang: ' + newPage);

        chatMessages.push(newId);
        chatMessages[newId] = session;
        

        // are we currently viewing that chat session?
        if (lastId == getCurrentChatId()) {
            $('#chatmsgs' + lastId).append('<p><strong>system</strong> Khách đang truy cập trang: ' + newPage + '</p>');

            scrollDiv($('#chatmsgs' + lastId));

            chatMessages[newId] = [];
        } else {
            var badge = $('#chat' + lastId).find('.badge');
            if (badge != null && badge != undefined) {
                if (!badge.hasClass('badge-warning')) {
                    badge.addClass('badge-warning');
                }
                badge.text(1);
            }
        }

        $('#chat' + lastId).attr('id', 'chat' + newId);
        $('#chatmsgs' + lastId).attr('id', 'chatmsgs' + newId);

        $('#chat' + newId).data('id', newId);
    };

    myHub.client.addMessage = function (id, from, value) {
        if (id == 'internal') {
            $('#chatmsgsinternal').append('<p><strong>' + from + '</strong> ' + value + '</p>');

            scrollDiv($('#chatmsgsinternal'));

            if ($('#chatmsgsinternal').is(':hidden')) {
                var internalBadge = $('#show-internal-chat').find('.badge');
                if (internalBadge != null && internalBadge != undefined) {
                    if (!internalBadge.hasClass('badge-warning')) {
                        internalBadge.addClass('badge-warning');
                    }
                    internalBadge.text('new msg(s)');
                }
            }
        } else if (id == getCurrentChatId()) {
            // are we currently viewing that chat session?
            $('#chatmsgs' + id).append('<p><strong>' + from + '</strong> ' + value + '</p>');

            scrollDiv($('#chatmsgs' + id));
        } else {
            var snd = new Audio('assets/sounds/newmsg.mp3');
            snd.play();

            chatMessages[id].push('<strong>' + from + '</strong> ' + value);

            var badge = $('#chat' + id).find('.badge');
            if (badge != null && badge != undefined) {
                if (!badge.hasClass('badge-warning')) {
                    badge.addClass('badge-warning');
                }
                badge.text(chatMessages[id].length);
            }
        }
    };

    myHub.client.leave = function (id) {
        myHub.server.leaveChat(id);
    };
    
    
    // Calls when user successfully logged in
    myHub.client.onConnected = function (id, userName, allUsers) {

        $('#hdId').val(id);
        $('#hdUserName').val(userName);
        $('#spanUser').html(userName);

        // Add All Users
        for (var i = 0; i < allUsers.length; i++) {

            AddUser(myHub, allUsers[i].Id, allUsers[i].Name);
        }

    };
    // On New User Connected
    myHub.client.onNewUserConnected = function(id, name) {

        AddUser(myHub, id, name);
    };
    // On User Disconnected
    myHub.client.onUserDisconnected = function (id, userName) {

        $('#' + id).remove();

        var ctrId = 'private_' + id;
        //$('#' + ctrId).remove();
        //$('#' + ctrId).find('#divMessage').append('<div class="message"><span class="userName">' + userName + '</span>: <span style="color:red">Đã thoát...</span></div>');

        var disc = $('<div class="disconnect">"' + userName + '" logged off.</div>');

        $(disc).hide();
        $('#divusers').prepend(disc);
        $(disc).fadeIn(200).delay(2000).fadeOut(200);
        $(disc).remove();

    };

    myHub.client.sendPrivateMessage = function(windowId, fromUserName, message) {

        var ctrId = 'private_' + windowId;


        if ($('#' + ctrId).length == 0) {

            createPrivateChatWindow(myHub, windowId, ctrId, fromUserName);

        }

        $('#' + ctrId).find('#divMessage').append('<div class="message"><span class="userName">' + fromUserName + '</span>: ' + message + '</div>');

        // set scrollbar
        var height = $('#' + ctrId).find('#divMessage')[0].scrollHeight;
        $('#' + ctrId).find('#divMessage').scrollTop(height);
    };
    
});

function showStatus() {
    $('#change-status').text(agent.name + ' ' + (agent.isOnline ? 'set offline' : 'set online'));
}

function getCurrentChatId() {
    if ($('.chat-session.active').length > 0) {
        return $('.chat-session.active').data('id');
    } else {
        return 'internal';
    }
}

function showChat(windowToShow) {
    $('#real-time-visits').hide();
    $('#all-chatbox').hide();
    $('.chat-msgs').hide();
    $('#chat-controls').hide();

    if (windowToShow == 'rt') {
        $('#real-time-visits').show();
    } else {
        $('#all-chatbox').show();
        $('#chatmsgs' + windowToShow).show();
        $('#chat-controls').show().find('#post-msg').focus();
    }
}

function postMsg() {
    var chatId = getCurrentChatId();
    var msg = $('#post-msg').val();

    $('#post-msg').val('').focus();

    if (msg.substring(0, 1) == '/') {
        commandTriggered(msg);
    } else {
        if (chatId != '' && msg != '') {
            myHub.server.opSend(chatId, msg);
        }
    }
}

function commandTriggered(cmd) {
    cmd = cmd.substring(1);

    var parts = cmd.split(' ');

    var msg = '';
    if (parts.length > 2) {
        for (var p = 2; p < parts.length; p++) {
            msg += parts[p] + ' ';
        }
    }

    // built-in command
    if (parts[0] == 'list') {
        var modalBody = $('#modal-cmd').find('.modal-body');
        if (modalBody != null) {
            var commands = getCommands();
            var body =
                '<p><strong>/list</strong> list all available commands.</p>' +
                '<p><strong>/add command-name text to be sent</strong> add a new command.</p>' +
                '<p><strong>/edit command-name text to be sent</strong> edit an existing command.</p>' +
                '<p><strong>/del command-name</strong> remove an exising command.</p>' +
                '<p><strong>/transfer agent-name</strong> transfer the current chat to another agent.</p>';

            body += '<br />';

            for (var i = 0; i < commands.length; i++) {
                body += '<p><strong>/' + commands[i].trigger + '</strong> ' + commands[i].msg + '</p>';
            }

            modalBody.html(body);

            $('#modal-cmd').modal({ keyboard: true });
        }
    } else if (parts[0] == 'add') {
        var exists = getCommand(parts[1]);
        if (exists == null) {
            exists = { trigger: parts[1], msg: msg };

            var commands = getCommands();
            commands.push(exists);

            setCommands(commands);

            $('#chatmsgs' + getCurrentChatId()).append('<p><strong>system</strong> ' + parts[1] + ' command added.</p>');
        } else {
            $('#chatmsgs' + getCurrentChatId()).append('<p><strong>system</strong> The command <em>' + parts[1] + '</em> already exists, please use /edit instead.</p>');
        }
    } else if (parts[0] == 'edit') {
        var commands = getCommands();
        for (var i = 0; i < commands.length; i++) {
            if(commands[i].trigger == parts[1]) {
                commands[i].msg = msg;
            }
        }
        setCommands(commands);

        $('#chatmsgs' + getCurrentChatId()).append('<p><strong>system</strong> ' + parts[1] + ' has been changed.</p>');
    } else if (parts[0] == 'del') {
        var commands = getCommands();
        var newCommands = [];

        for (var i = 0; i < commands.length; i++) {
            if (commands[i].trigger != parts[1]) {
                newCommands.push(commands[i]);
            }
        }

        setCommands(newCommands);

        $('#chatmsgs' + getCurrentChatId()).append('<p><strong>system</strong> ' + parts[1] + ' has been removed.</p>');
    } else if (parts[0] == 'transfer') {
        var chatId = getCurrentChatId();
        myHub.server.transfer(chatId, parts[1], $('#chatmsgs' + chatId).html());

        setTimeout(function () {
            $("div[data-id='" + chatId + "']").remove();
            $('#chatmsgs' + chatId).remove();
            $('.chat-session').removeClass('active');
            showChat('rt');
        }, 2500);
    } else {
        var command = getCommand(parts[0]);
        var chatId = getCurrentChatId();
        if (command != null && chatId != '') {
            myHub.server.opSend(chatId, command.msg);
        } else {
            $('#chatmsgs' + chatId).append('<p><strong>system</strong> ' + parts[1] + ' is not a recongnized command, use /list to view available commands.</p>');
        }
    }
}

function scrollDiv(div) {
    div.scrollTop(div[0].scrollHeight);
}

function getCommands() {
    var cmdKey = getStoredCommands('lcsk-cmd');
    if (cmdKey != null && cmdKey != '') {
        return JSON.parse(cmdKey);
    }
    else
        return [];
}

function getCommand(trigger) {
    var commands = getCommands();
    if (commands.length > 0) {
        for (var i = 0; i < commands.length; i++) {
            if (commands[i].trigger == trigger) {
                return commands[i];
            }
        }
    }
}

function setCommands(commands) {
    setStoredCommands('lcsk-cmd', JSON.stringify(commands), 365);
}

function hasStorage() {
    return typeof (Storage) !== 'undefined';
}

function setStoredCommands(key, data) {
    if (hasStorage()) {
        localStorage.setItem(key, data);
    }
}

function getStoredCommands(key) {
    if (hasStorage()) {
        return localStorage.getItem(key);
    }
}

//edit by luan

function AddUser(myHub, id, name) {

    var userId = $('#hdId').val();

    var code = "";

    if (userId == id) {

        code = $('<div class="loginUser user">' + name + "</div>");

    }
    else {

        code = $('<a id="' + id + '" class="user" >' + name + '<a>');

        $(code).dblclick(function () {

            var id = $(this).attr('id');

            if (userId != id)
                OpenPrivateChatWindow(myHub, id, name);
        });
    }

    $("#divusers").append(code);

}
function OpenPrivateChatWindow(myHub, id, userName) {

    var ctrId = 'private_' + id;

    if ($('#' + ctrId).length > 0) return;

    createPrivateChatWindow(myHub, id, ctrId, userName);

}
function createPrivateChatWindow(myHub, userId, ctrId, userName) {

    var div = '<div style="opacity: 0.8;z-index: 9999;" id="' + ctrId + '" class="ui-widget-content draggable" rel="0">' +
        '<div class="header">' +
        '<div  style="float:right;">' +
        '<img id="imgDelete"  style="cursor:pointer;" src="../Content/delete.png"/>' +
        '</div>' +

        '<span class="selText" rel="0">' + userName + '</span>' +
        '</div>' +
        '<div id="divMessage" class="messageArea">' +

        '</div>' +
        '<div class="buttonBar">' +
        '<input id="txtPrivateMessage" class="msgText" type="text"/>' +
        '<input id="btnSendMessage" class="submitButton button" type="button" value="Send"/>' +
        '</div>' +
        '</div>';

    var $div = $(div);

    // DELETE BUTTON IMAGE
    $div.find('#imgDelete').click(function () {
        $('#' + ctrId).remove();
    });

    // Send Button event
    $div.find("#btnSendMessage").click(function () {

        $textBox = $div.find("#txtPrivateMessage");
        var msg = $textBox.val();
        if (msg.length > 0) {

            myHub.server.sendPrivateMessage(userId, msg);
            $textBox.val('');
        }
    });

    // Text Box event
    $div.find("#txtPrivateMessage").keypress(function (e) {
        if (e.which == 13) {
            $div.find("#btnSendMessage").click();
        }
    });

    AddDivToContainer($div);

}
function AddDivToContainer($div) {
    $('#chat-content').prepend($div);

    $div.draggable({

        handle: ".header",
        stop: function () {

        }
    });

    ////$div.resizable({
    ////    stop: function () {

    ////    }
    ////});

}

//end edit by luan