$(function () {
    $(document).ready(function () {
        $('#gameList').DataTable({
            "order": [[3, "desc"]],
            "searching": true,
            "paging": true,
            "info": false
        });
    });

    $(document).keypress(function (e) {
        if (e.which == 13) {
            $("#sendmessage").click();
        }
    });

    // Reference the auto-generated proxy for the hub.
    var players = $.connection.playerHub;
    var games = $.connection.gameHub;
    var chat = $.connection.chatHub;


    chat.client.addMessage = function (name, message, time) {

        $('#chat').append('<li class="left clearfix">' +
                    '<div class="chat-body clearfix">' +
                        '<div class="header">' +
                            '<strong class="primary-font" style="padding-right: 15px;">' + name + '</strong>' +
                            '<small class="text-muted"><span class="glyphicon glyphicon-time"></span>'+ time +'</small>' +
                        '</div>' +
                        '<p>' + message + '</p>' +
                    '</div>' +
                '</li>');
        $("#chat").scrollTop($("#chat")[0].scrollHeight); //scroll to down

    };

    players.client.addPlayer = function (config) {
        $('#playerList').append('<li id='+config.id+'><strong>' + config.name + '</li>');
    };

    players.client.removePlayer = function (config) {
        $('#'+config.id).remove();
    };

    games.client.changeGameState = function (config) {
        var a = $('tr[data-id="'+config.GameId+'"]').children()[4]; $(a).html(config.State)
    };

    games.client.updateGamePlayers = function (config) {
        var a = $('tr[data-id="' + config.GameId + '"]').children()[3];
        $(a).html(config.PlayersCount)
        //if (config.PlayersCount == '0/2') {
        //    var table = $('#gameList').DataTable();
        //    table.rows($('tr[data-id="' + config.id + '"]'))[0].remove().draw();
        //}
    };

    games.client.addGame = function (config) {
        var table = $('#gameList').DataTable();

        var i = table.row.add([
            config.name, config.date, config.author, "1/2", config.state
        ]).draw().index();

        var tableRow = table.row(i).node();
        $(tableRow).attr("data-id", config.id);
        $(tableRow).click(function(args){
            connectToGame(args.currentTarget.dataset.id);
        })
    };

    onTrClick = function(args){
        connectToGame(args.dataset.id);
    }

    connectToGame = function(id){
        if (confirm("Connect to game?")) {
            var url = playAction;
            var win = window.open(url + '/' + id, '_blank');
            win.focus();
        }  
    };

    games.client.removeGame = function (config) {
        //datatables.js api
        $('#gameList').DataTable().row($('tr[data-id="' + config.id + '"]')).remove().draw();
    };

    // Set initial focus to message input box.
    $('#messageText').focus();
    // Start the connection.
    $.connection.hub.start().done(function () {
        $('#sendmessage').click(function () {
            if (!$('#messageText').val() == '') {
                // Call the Send method on the hub.
                chat.server.send(playerName, $('#messageText').val());
                // Clear text box and reset focus for next comment.
                $('#messageText').val('').focus();
            }
        });
    });
});
// This optional function html-encodes messages for display in the page.
function htmlEncode(value) {
    var encodedValue = $('<div />').text(value).html();
return encodedValue;
}