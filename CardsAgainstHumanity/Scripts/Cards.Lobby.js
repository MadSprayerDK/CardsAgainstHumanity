$(document).ready(function () {
    Lobby.LobbyBack();
    Lobby.StartGame();
});

var Lobby = {

	UpdateLobbyUserList: function(lobbyList) {
		$("#Lobby_PlayerList").empty();
		$("#Lobby_PlayerList").append("<tr><th style=\"text-align: center;\">Players</th></tr>");

	    lobbyList.forEach(function(element) {
	        $("#Lobby_PlayerList").append("<tr><td>" + element + "</td></tr>");
	    });
	},

	LobbyBack: function() {
	    $("#Lobby_Back").on("click", function () {

		    $.connection.lobbyHub.server.leaveLobby();

		    $("#Page_Lobby").css("display", "none");
		    $("#Page_Welcome").css("display", "block");
	    });
	},

    StartGame: function() {
        $("#Lobby_StartGame").on("click", function () {
            $.connection.gameHub.server.startGame(Game.CurrentGame);
        });
    }
};