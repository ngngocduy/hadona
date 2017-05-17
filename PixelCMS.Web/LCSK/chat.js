var myHub = $.connection.chatHub;

$(function () {
    LCSKChat.config();
    LCSKChat.init();
});

var LCSKChat = function () {
    var chatKey = 'lcsk-chatId';
    var requestChat = false;
    var chatId = '';
    var chatEditing = false;

    var options = [];

    function setDefaults() {
        options.position = 'fixed';
        options.placement = 'bottom-right';

        options.headerPaddings = '3px 10px 3px 10px';
        options.headerBackgroundColor = '#408F44';
        options.headerTextColor = 'white';
        options.headerBorderColor = '#408F44';
        options.headerGradientStart = '#408F44';
        options.headerGradientEnd = '#408F44';
        options.headerFontSize = '15px';

        options.boxBorderColor = '#408F44';
        options.boxBackgroundColor = '#fff';

        options.width = 250;

        options.offlineTitle = 'Liên hệ với chúng tôi!';
        options.onlineTitle = 'Chat với chúng tôi!';

        options.waitingForOperator = 'Cảm ơn,cho chúng tôi 1 phút để chấp nhận trò chuyện ... ';
        options.emailSent = 'Email của bạn đã được gửi đi,chúng tôi sẽ liên hệ với bạn sớm';
        options.emailFailed = 'Email không thể gửi được vào lúc này.<br /><br />Xin lỗi quý khách.';

    }

    function config(args) {
        setDefaults();

        if (args != null) {
            for (key in options) {
                if (args[key]) {
                    options[key] = args[key];
                }
            }
        }
    }

    function getPlacement() {
        if (options.placement == 'bottom-right') {
            return 'bottom:0px;right:0px;';
        }
        return '';
    }

    function init() {
        $('body').append(
            //'<div id="chat-box-header" style="display: block;position:' + options.position + ';' + getPlacement() + 'width:' + options.width + 'px;padding:' + options.headerPaddings + ';color:' + options.headerTextColor + ';font-size:' + options.headerFontSize + ';cursor:pointer;background:' + options.headerBackgroundColor + ';filter: progid:DXImageTransform.Microsoft.gradient(startColorstr=\'' + options.headerGradientStart + '\', endColorstr=\'' + options.headerGradientEnd + '\');background: -webkit-gradient(linear, left top, left bottom, from(' + options.headerGradientStart + '), to(' + options.headerGradientEnd + '));background: -moz-linear-gradient(top,  ' + options.headerGradientStart + ',  ' + options.headerGradientEnd + ');border:1px solid ' + options.headerBorderColor + ';box-shadow:inset 0 0 7px #408f44;-webkit-box-shadow:inset 0 0 7px #408f44;border-radius: 5px;">' + options.offlineTitle + '</div>' +
            //'<div id="chat-box" style="display:none;position:' + options.position + ';' + getPlacement() + 'width:' + options.width + 'px;height:300px;padding: 10px 10px 10px 10px;border: 1px solid ' + options.boxBorderColor + ';background-color:' + options.boxBackgroundColor + ';font-size:small;"></div>'
            '<div id="chat-box-header" style="z-index: 9999;display: block;position:' + options.position + ';' + getPlacement() + 'width:' + options.width + 'px;padding:' + options.headerPaddings + ';color:' + options.headerTextColor + ';font-size:' + options.headerFontSize + ';cursor:pointer;background:' + options.headerBackgroundColor + ';filter: progid:DXImageTransform.Microsoft.gradient(startColorstr=\'' + options.headerGradientStart + '\', endColorstr=\'' + options.headerGradientEnd + '\');background: -webkit-gradient(linear, left top, left bottom, from(' + options.headerGradientStart + '), to(' + options.headerGradientEnd + '));background: -moz-linear-gradient(top,  ' + options.headerGradientStart + ',  ' + options.headerGradientEnd + ');border:1px solid ' + options.headerBorderColor + ';box-shadow:inset 0 0 7px #408f44;-webkit-box-shadow:inset 0 0 7px #408f44;border-top-left-radius: 5px;border-top-right-radius: 5px;">' + options.offlineTitle + '</div>' +
            '<div id="chat-box" style="z-index: 9999;display:none;position:' + options.position + ';' + getPlacement() + 'width:' + options.width + 'px;padding: 10px 10px 10px 10px;border: 1px solid ' + options.boxBorderColor + ';background-color:' + options.boxBackgroundColor + ';opacity: 0.8;font-size:14px !important;color: black !important;"></div>'
        );

        $.connection.hub.start()
            .done(function () {
                var existingChatId = getExistingChatId(chatKey);
                myHub.server.logVisit(document.location.href, document.referrer, existingChatId);
            })
            .fail(function () { chatRefreshState(false); });

        $('body').on({
            click: function () {
                toggleChatBox();
            }
        }, '#chat-box-header');

        $('#chat-box').on({
            keydown: function (e) {
                var msg = $(this).val();
                if (e.keyCode == 13 && msg != '') {
                    e.preventDefault();
                    e.stopPropagation();

                    if (chatId == null || chatId == '') {
                        myHub.server.requestChat(msg);
                        $('#chat-box-msg').html(options.waitingForOperator);
                    } else {
                        myHub.server.send(msg);
                    }

                    $('#chat-box-textinput').val('');
                }
            }
        }, '#chat-box-textinput');

        $('#chat-box').on({
            keydown: function () {
                chatEditing = true;
            }
        }, '.chat-editing');

        $('#chat-box').on({
            click: function () {
                var namemail = $("#chat-box-email").val();
                if (IsEmail(namemail)) {
                    myHub.server.sendEmail($('#chat-box-email').val(), $('#chat-box-cmt').val());

                    $('#chat-box').html(options.emailSent);
                    chatEditing = false;
                } else {
                    alert("Email ko hợp lệ");
                }
            }
        }, '#chat-box-send');
    }

    function IsEmail(email) {
        var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
        return regex.test(email);
    }

    function chatRefreshState(state) {
        if (state) {
            $('#chat-box-header').text(options.onlineTitle);
            if (!requestChat) {
                $('#chat-box').html(
                    '<div id="chat-box-msg" style="height:237px;overflow:auto;">' +
                    '<p>Nhập nội dung bên dưới và nhấn ENTER.</p></div>' +
                    '<div id="chat-box-input" style="height:35px;"><textarea id="chat-box-textinput" placeholder="Nhập nội dung" style="width:100%;height: 32px;border:1px solid #408f44;border-radius: 3px;" /></div>'
                );
            }
        } else {
            if (!chatEditing) {
                $('#chat-box-header').text(options.offlineTitle);
                $('#chat-box-input').hide();
                $('#chat-box').html(
                    '<p>Email của bạn</p><input type="text" id="chat-box-email" style="border:1px solid #408f44;border-radius: 3px;width: 94%;" class="chat-editing" />' +
                    '<p>Lời nhắn của bạn</p><textarea id="chat-box-cmt" rows="9" class="chat-editing" style="border:1px solid #408f44;border-radius: 3px;width:100%"></textarea>' +
                    '<p><input style="color:#408f44" type="button" id="chat-box-send" value="Gửi" />'
                );
            }
        }
    }
    
    function toggleChatBox() {
        var elm = $('#chat-box-header');
        if ($('#chat-box').hasClass('chat-open')) {
            $('#chat-box').removeClass('chat-open');
            elm.css('bottom', '0px');
        } else {
            //độ cao header
            var y = 294;
            $('#chat-box').addClass('chat-open');
            elm.css('bottom', y);
        }
        $('#chat-box').slideToggle();
    }

    function hasStorage() {
        return typeof(Storage) !== 'undefined';
    }

    function setChatId(chatId) {
        if (hasStorage()) {
            sessionStorage.setItem(chatKey, chatId);
        }
    }

    function getExistingChatId() {
        if (hasStorage()) {
            return sessionStorage.getItem(chatKey);
        }
    }
    
    

    myHub.client.setChat = function (id, agentName, existing) {
        chatId = id;
        requestChat = true;

        setChatId(chatId);

        if (existing) {
            if (!$('#chat-box').hasClass('chat-open')) {
                toggleChatBox();
            }

            $('#chat-box-msg').append('<p>Tiếp tục trò chuyện của bạn với <strong>' + agentName + '</strong></p>');
        } else {
            $('#chat-box-msg').append('<p>Bạn đang chat vơi <strong>' + agentName + '</strong></p>');
        }
    };

    myHub.client.addMessage = function (from, msg) {
        if (chatId != null && chatId != '') {
            if (!requestChat) {
                if (!$('#chat-box').hasClass('chat-open')) {
                    toggleChatBox();
                }
                requestChat = true;
            }

            $('#chat-box-msg').append('<p><strong>' + from + '</strong>: ' + msg + '</p>');
            if (from == '') {
                chatId = '';
                requestChat = false;
            }

            $("#chat-box-msg").scrollTop($("#chat-box-msg")[0].scrollHeight);
        }
    }

    myHub.client.emailResult = function (state) {
        if (!state) {
            $('#chat-box').html(options.emailFailed);
        }
    };

    myHub.client.onlineStatus = function (state) {
        chatRefreshState(state);
    };

    return {
        config: config,
        init: init
    }
}();


