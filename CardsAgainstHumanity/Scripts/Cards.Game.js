$(document).ready(function () {
    Game.SendAnswerSingle();
    Game.ChooseBestAnswer();
    Game.ReturnToLobby();
});

var Game = {
    CurrentGame: "",

    ShowCurrentCards: function(cards, numberOfAnswers) {
        $("#Page_Lobby").css("display", "none");
        $("#Page_Question").css("display", "none");

        if (numberOfAnswers === 1) {

            $("#Answers_SingleAnswersArea").empty();
            $(cards).each(function (i, element) {

                $("#Answers_SingleAnswersArea").append("<div class=\"radio\"><label><input type=\"radio\" value=\"" + element.Id + "\" name=\"Answers_SingleRadio\" id=\"Answers_SingleRadio-" + element.Id + "\" />" + element.Text + "</label></div>");
            });

            $("#AnswersSingle").css("display", "block");
        }

        $("#Answers_SingleSend").removeClass("disabled");
        $("#Page_Answers").css("display", "block");
        $("#Page_Score").css("display", "block");
    },

    ShowCurrentQuestion: function(questionText) {
        $("#Page_Lobby").css("display", "none");

        $("#Page_Answers").css("display", "none");
        $("#Question_Best").css("display", "none");
        $("#Question_AnswerList").empty();

        $("#Question_CurrentQuestion").text(questionText);
        $("#Page_Question").css("display", "block");
        $("#Page_Score").css("display", "block");
    },

    SendAnswerSingle: function() {
        $("#Answers_SingleSend").on("click", function () {

            if ($("input:radio[name='Answers_SingleRadio']:checked").length === 0)
                return;

            $("#Answers_SingleSend").addClass("disabled");
            $("input:radio[name='Answers_SingleRadio']").attr("disabled", true);

            var card = $("input:radio[name='Answers_SingleRadio']:checked").val();

            $.connection.gameHub.server.sendtAnswer(Game.CurrentGame, card);
        });  
    },

    RecievedAnswer: function() {
        $("#Question_AnswerListClue").append("<li>.....</li>");
    },

    RecievedAllAnswers: function (answers) {

        $("#Question_Best").css("display", "block");

        $("#Question_AnswerListClue").empty();

        $(answers).each(function (i, element) {

            $("#Question_AnswerList").append("<div class=\"radio\"><label><input type=\"radio\" value=\"" + element.Id + "\" name=\"Question_Radio\" id=\"Question_Radio-" + element.Id + "\" />" + element.Text + "</label></div>");

        });
    },

    ChooseBestAnswer: function () {
        $("#Question_Best").on("click", function () {

            if ($("input:radio[name='Question_Radio']:checked").length === 0)
                return;

            var best = $("input:radio[name='Question_Radio']:checked").val();

            $.connection.gameHub.server.choseBestAnswer(Game.CurrentGame, best);
        }); 
    },

    RecievedScores: function(scores) {
        $("#Score_ScoreList").empty();
        $("#Score_ScoreList").append("<tr><th style=\"text-align: center;\">Player</th><th style=\"text-align: center;\">Score</th></tr>");

        scores.forEach(function (element) {
            $("#Score_ScoreList").append("<tr><td>" + element.Name + "</td><td>" + element.NumberOfPoints + "</td></tr>");
        });
    },

    ReturnToLobby: function() {
        $("#Score_ReturnToLobby").on("click", function() {
            $.connection.gameHub.server.returnToLobby(Game.CurrentGame);
        });
    } 
};