$(document).ready(function () {
    
	$.connection.hub.start().done(function () {
	    $("#Welcome_Connection").html("Connection Established");
	});

	$.connection.lobbyHub.client.lobbyGotoLobby = Welcome.GotoLobby;
	$.connection.lobbyHub.client.lobbyErrorJoiningLobby = Welcome.ErrorJoiningLobby;
	$.connection.lobbyHub.client.lobbyUpdateLobbyUserList = Lobby.UpdateLobbyUserList;

    $.connection.gameHub.client.gameShowCurrentCards = Game.ShowCurrentCards;
    $.connection.gameHub.client.gameShowCurrentQuestion = Game.ShowCurrentQuestion;
    $.connection.gameHub.client.gameRecievedAnswer = Game.RecievedAnswer;
    $.connection.gameHub.client.gameRecievedAllAnswers = Game.RecievedAllAnswers;
    $.connection.gameHub.client.gameRecievedScores = Game.RecievedScores;
    $.connection.gameHub.client.lobbyGotoLobby = Welcome.GotoLobby;
});