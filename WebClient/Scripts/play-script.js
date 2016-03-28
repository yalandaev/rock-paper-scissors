$(function () {
    var playersCount = 0;


    $('#status').html(gameStatus);

    setObjectsVisibility = function () {
        switch (gameStatus) {
            case "Waiting":
                $("#timeBlock").hide();
                break;
            case "Game":
                $('#startGame').hide();
                $("#timeBlock").show();
                break;
            case "Finished":
                $("#timeBlock").hide();
                break;
            default:
        };

        if (hintUsed)
            $("#getHint").hide();
    };

    setObjectsVisibility();

    var gameActions = {
        /// Камень
        Stone: 0,
        /// Бумага
        Paper: 1,
        /// Ножницы
        Scissors: 2,
        /// Ящерица
        Lizard: 3,
        /// Спок
        Spock: 4,
        /// Unknown
        Unknown: 5
    };
    var actionsButtons = [
        "#rock-action", "#spock-action", "#paper-action", "#scissors-action", "#lizard-action"
    ];
    var actionsGrayIcons = {
        "#rock-action": "rock2",
        "#spock-action": "spock2",
        "#paper-action": "paper2",
        "#scissors-action": "scissors2",
        "#lizard-action": "lizard2"
    }
    var actionsNormalIcons = {
        "#rock-action": "rock",
        "#spock-action": "spock",
        "#paper-action": "paper",
        "#scissors-action": "scissors",
        "#lizard-action": "lizard"
    };

    var canSelect = true;


    ajaxGetDataUrl = '../../api/game';


    log = function (message) {
        $('#battlelog').prepend('<li>' + message + '</li>');
    };

    var globalTimeInterval;

    startTimer = function (duration, display) {
        var timer = duration, minutes, seconds;
        if (globalTimeInterval)
            clearInterval(globalTimeInterval);
        globalTimeInterval = setInterval(function () {
            minutes = parseInt(timer / 60, 10)
            seconds = parseInt(timer % 60, 10);

            minutes = minutes < 10 ? "0" + minutes : minutes;
            seconds = seconds < 10 ? "0" + seconds : seconds;

            display.text(minutes + ":" + seconds);

            if (--timer < 0) {
                timer = duration;
            }
        }, 1000);
    }

    var playHub = $.connection.playHub;

    playHub.client.onNotification = function (config) {
        log(config.Time + ': ' + config.Message);
    };
    playHub.client.onTurnFinished = function (config) {
        if (!config.isDraw) {
            $('#player1Points').html(config.player1Points);
            $('#player2Points').html(config.player2Points);
            $("#winMessage").text(config.turnWinner + " win current turn!");
        } else {
            $("#winMessage").text("Draw!");
        }

        $('#turn').html(config.nextTurn);


        var otherPlayerAction = PlayerName == config.player1Name ? config.player2Action : config.player1Action;
        if (otherPlayerAction != -1) {
            changeActionClass("#player2Choice", otherPlayerAction);
        }


        display = $('#time');
        if (gameStatus == "Game") {
            startTimer(seconds, display);
        }


        var myInterval = window.setInterval(function (a, b) {
            restoreActionButtons();
            changeActionClass("#player1Choice", 5);
            changeActionClass("#player2Choice", 5);
            $("#winMessage").text("");
        }, 1500);
        window.setTimeout(function (a, b) {
            clearInterval(myInterval);
            canSelect = true;
        }, 1500);
    };
    playHub.client.onGameFinished = function (config) {
        log(config.time + ": Победил " + config.gameWinner);
        $("#winMessage").text(config.gameWinner + " win the game!");
        setObjectsVisibility();
    };
    playHub.client.onPlayerAction = function (config) {
        //Событие при действии игрока (выборе варианта)
    };
    playHub.client.changeGameState = function (config) {
        $('#status').html(config);
    };
    playHub.client.onTurnTimeout = function (config) {
        log(config.Message);
        $('#player1Points').html(config.player1Points);
        $('#player2Points').html(config.player2Points);
    };
    playHub.client.addPlayer2 = function (config) {
        $('#player2Name').html(config.Player2Name);
        $('#player2Points').html("0");
        $('#battlelog').append('<li>' + config.Player2Name + ' connected</li>');
    };
    $.connection.hub.start().done(function () {
        playHub.server.joinGroup(GameName);
    });

    playHub.client.onStateChanged = function (config) {
        gameStatus = config.State;
        $('#status').html(config.State);
        setObjectsVisibility();
    };
    playHub.client.turnLog = function (config) {
         
        var markup = "<div class='turn-container'>";
        markup += "<p class='turn-container-title'>Turn " + config.CurrentTurn + "</p>";
        if (config.Player1Name) {
            if (!config.TurnWinner) {
                config.TurnWinner = "";
            }
            var classes = "turn-container-looser";
            if (config.Player1Name == config.TurnWinner) {
                classes = "turn-container-winner";
            }
            markup += "<p class='" + classes + "'>" + config.Player1Name + ": " + config.Player1Action + "</p>";
            if (config.Player1UsedHint == "True") {
                markup += "<span class='label label-primary'>Hint</span>";
            }
            markup += "<br>";
        }
        if (config.Player2Name) {
            if (!config.TurnWinner) {
                config.TurnWinner = "";
            }
            var classes = "turn-container-looser";
            if (config.Player2Name == config.TurnWinner) {
                classes = "turn-container-winner";
            }
            markup += "<p class='" + classes + "'>" + config.Player2Name + ": " + config.Player2Action + "</p>";
            if (config.Player2UsedHint == "True") {
                markup += "<span class='label label-primary'>Hint</span>";
            }
            markup += "<br></div>";
        }
        markup += "<br></div>";
        $("#tunsTab").append(markup);
    };
    playHub.client.onPlayersCountChanged = function (config) {
        playersCount = config.PlayersCount;
    };
    playHub.client.onPlayerLeaveNotStartedGame = function (config) {
        resetPlayerScore(config.Player);
    };

    playHub.client.onGameStarted = function () {
        $('#startGame').hide();
        display = $('#time');
        startTimer(seconds, display);
    };

    changeActionClass = function (selector, action) {
        $(selector).removeClass();
        var classes = "";
        switch (action) {
            case gameActions.Stone:
                classes += "rock";
                break;
            case gameActions.Paper:
                classes += "paper";
                break;
            case gameActions.Lizard:
                classes += "lizard";
                break;
            case gameActions.Scissors:
                classes += "scissors";
                break;
            case gameActions.Spock:
                classes += "spock";
                break;
            case gameActions.Unknown:
                classes += "unknown";
                break;
            default:

        }
        $(selector).addClass(classes);
        $(selector).addClass("action-image");
    };

    grayOtherActions = function (selectedAction) {
        actionsButtons.forEach(function (item, i, arr) {
            if (item != selectedAction) {
                $(item).removeClass();
                $(item).addClass(actionsGrayIcons[item]);
                $(item).addClass("action-image");
            }
        });
    }
    restoreActionButtons = function () {
        actionsButtons.forEach(function (item, i, arr) {
            $(item).removeClass();
            $(item).addClass(actionsNormalIcons[item]);
            $(item).addClass("action-image");
        });
    };

    $('#spock-action, #lizard-action, #rock-action, #scissors-action, #paper-action').click(function (args) {
        if (canSelect && gameStatus == "Game") {
            canSelect = false;
            var action = args.target.dataset.action;
            changeActionClass("#player1Choice", parseInt(action));
            grayOtherActions("#" + args.target.id);

            $.ajax({
                type: 'POST',
                url: ajaxGetDataUrl,
                data: {
                    "Method": 5,
                    "GameName": GameName,
                    "PlayerName": PlayerName,
                    "Action": action
                },
                dataType: "json",
                success: function (data) {
                }
            });
        }
    });


    $('#startGame').click(function () {
        if (gameStatus == "Waiting" && playersCount == 2) {
            $.ajax({
                type: 'POST',
                url: ajaxGetDataUrl,
                data: {
                    "Method": 6,
                    "GameName": GameName,
                    "PlayerName": PlayerName,
                },
                dataType: "json",
                success: function (data) {

                }
            });
        } else {
            alert("Two players needed!")
        }
    });

    $('#getHint').click(function () {
        $("#getHint").hide();
        $.ajax({
            type: 'POST',
            url: ajaxGetDataUrl,
            data: {
                "Method": 7,
                "GameName": GameName,
                "PlayerName": PlayerName,
            },
            dataType: "json",
            success: function (data) {
                changeActionClass("#player2Choice", data)
            }
        });
    });

    $('#exitGame').click(function () {
        $.ajax({
            type: 'POST',
            url: ajaxGetDataUrl,
            data: {
                "Method": 3,
                "GameName": GameName,
                "PlayerName": PlayerName,
            },
            dataType: "json",
            success: function (data) {
                window.open('', '_self', '');
                window.close();
            }
        });
    });

    resetPlayerScore = function (playerName) {
        var playerSelector = playerName == creatorName ? "#player1" : "#player2";
        $(playerSelector + "Name").html("None");
        $(playerSelector + "Points").html("0");
    };


});
function htmlEncode(value) {
    var encodedValue = $('<div />').text(value).html();
    return encodedValue;
}