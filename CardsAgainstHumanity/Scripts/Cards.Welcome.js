$(document).ready(function() {
    Welcome.CreateNewLobby();
    Welcome.CreateNewLobbyBack();
    Welcome.JoinExistingLobby();
    Welcome.JoinExistingLobbyBack();

    Welcome.CreateNewLobbyFormSubmit();
    Welcome.JoinLobbyFormSubmit();
});

var Welcome = {

    CreateNewLobby: function() {
        $("#Welcome_NewLobby").on("click", function () {
            $("#NewLobby_Name").val("");

            $("#Page_Welcome").css("display", "none");
            $("#Page_NewLobby").css("display", "block");
        });
    },

    CreateNewLobbyBack: function() {
        $("#NewLobby_Back").on("click", function () {
            $("#Page_NewLobby").css("display", "none");
            $("#Page_Welcome").css("display", "block");
        });
    },

    JoinExistingLobby: function() {
        $("#Welcome_JoinLobby").on("click", function () {
            $("#JoinLobby_Name").val("");
            $("#JoinLobby_Code").val("");
            $("#JoinLobby_Error").css("display", "none");

            $("#Page_Welcome").css("display", "none");
            $("#Page_JoinLobby").css("display", "block");
        });
    },

    JoinExistingLobbyBack: function() {
        $("#JoinLobby_Back").on("click", function () {
            $("#Page_JoinLobby").css("display", "none");
            $("#Page_Welcome").css("display", "block");
        });
    },

    CreateNewLobbyFormSubmit: function() {
        $("#NewLobby_Form").on("submit", function (event) {
            var name = $("#NewLobby_Name").val();

            if (name) {
                $.connection.lobbyHub.server.createNewLobby(name);
            } else {
                $("#NewLobby_Name").focus();
            }

            event.preventDefault();
        });
    },

    JoinLobbyFormSubmit: function() {
        $("#JoinLobby_Form").on("submit", function (event) {

            var name = $("#JoinLobby_Name").val();
            var code = $("#JoinLobby_Code").val();

            if (!name)
                $("#JoinLobby_Name").focus();
            else if (!code)
                $("#JoinLobby_Code").focus();
            else
                $.connection.lobbyHub.server.joinLobby(name, code);

            event.preventDefault();
        });
    },

    // SignalR called functions
    GotoLobby: function(lobbyName) {
        $("#Lobby_LobbyCode").text(lobbyName);
        Game.CurrentGame = lobbyName;

        $("#NewLobby_Name").blur();
        $("#JoinLobby_Name").blur();
        $("#JoinLobby_Code").blur();

        $("#Page_NewLobby").css("display", "none");
        $("#Page_JoinLobby").css("display", "none");
        $("#Page_Answers").css("display", "none");
        $("#Page_Question").css("display", "none");
        $("#Page_Score").css("display", "none");
        $("#Page_Lobby").css("display", "block");
    },


    ErrorJoiningLobby: function(errorMessage) {
        $("#JoinLobby_Error").text(errorMessage);
        $("#JoinLobby_Error").css("display", "block");
    }

};