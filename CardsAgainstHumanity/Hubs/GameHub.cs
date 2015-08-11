// ReSharper disable UnusedMember.Global
using System.Collections.Generic;
using System.Linq;
using CardsAgainstHumanity.Extensions;
using CAH.Database;
using CAH.Model;
using Microsoft.AspNet.SignalR;
using WebGrease.Css.Extensions;

namespace CardsAgainstHumanity.Hubs
{
    public class GameHub : Hub
    {
        public void StartGame(string lobbyCode)
        {
            var lobby = LobbyHub.Lobbies.SingleOrDefault(x => x.Code == lobbyCode.ToUpper());

            if (lobby == null)
                return;

            var db = new CardsReposiitory();

            lobby.ActiveGame = true;
            lobby.QuestionCards = db.GetQuestionCards();
            lobby.AnswerCards = db.GetAnswerCards();

            lobby.CurrentLeader = lobby.Users.First();
            lobby.CurrentQuestion = lobby.QuestionCards.Where(x => x.NumAnswers == 1).PickRandom(); // TODO: ALLOW MULTIPLE ANSWERS
            lobby.CurrentAwnsers = new List<Card>();

            foreach (var user in lobby.Users)
            {
                var cards = lobby.AnswerCards.Where(x => x.CardUsed == CardUsedEnum.Free).PickRandom(10).ToList();
                cards.ForEach(x => x.CardUsed = CardUsedEnum.InUse);

                user.CurrentCardsOnHand = cards;
            }

            foreach (var user in lobby.Users)
            {
                Clients.Client(user.ConnectionId).gameRecievedScores(lobby.Users.Select(x => new { x.Name, x.NumberOfPoints }));
            }

            foreach (var user in lobby.Users)
            {
                if (user == lobby.CurrentLeader)
                    continue;

                Clients.Client(user.ConnectionId).gameShowCurrentCards(user.CurrentCardsOnHand.Select(x => new { x.Id, x.Text }), lobby.CurrentQuestion.NumAnswers);
            }

            Clients.Client(lobby.CurrentLeader.ConnectionId).gameShowCurrentQuestion(lobby.CurrentQuestion.Text);
        }

        public void SendtAnswer(string lobbyCode, int cardId)
        {
            var lobby = LobbyHub.Lobbies.SingleOrDefault(x => x.Code == lobbyCode.ToUpper());

            if (lobby == null)
                return;

            var card = lobby.AnswerCards.SingleOrDefault(x => x.Id == cardId);

            if (card == null)
                return;

            lobby.CurrentAwnsers.Add(card);

            if (lobby.CurrentAwnsers.Count == lobby.Users.Count - 1)
            {
                Clients.Client(lobby.CurrentLeader.ConnectionId).gameRecievedAllAnswers(lobby.CurrentAwnsers.Shuffle().Select(x => new { x.Id, x.Text }));
            }
            else
            {
                Clients.Client(lobby.CurrentLeader.ConnectionId).gameRecievedAnswer();
            }
        }

        public void ChoseBestAnswer(string lobbyCode, int cardId)
        {
            var lobby = LobbyHub.Lobbies.SingleOrDefault(x => x.Code == lobbyCode.ToUpper());

            if (lobby == null)
                return;

            // End old round

            lobby.CurrentLeader.HaveBeenLeader = true;

            foreach (var user in lobby.Users)
            {
                if (user.CurrentCardsOnHand.Any(x => x.Id == cardId))
                    user.NumberOfPoints++;

                user.CurrentCardsOnHand.RemoveAll(x => lobby.CurrentAwnsers.Exists(y => y.Id == x.Id));
            }

            lobby.CurrentQuestion.CardUsed = CardUsedEnum.Used;
            lobby.CurrentAwnsers.ForEach(x => x.CardUsed = CardUsedEnum.Used);

            lobby.CurrentAwnsers.RemoveAll(x => true);

            // Start new round

            if (lobby.Users.All(x => x.HaveBeenLeader))
                lobby.Users.ForEach(x => x.HaveBeenLeader = false);

            lobby.CurrentLeader = lobby.Users.First(x => x.HaveBeenLeader == false);

            if (lobby.QuestionCards.All(x => x.CardUsed == CardUsedEnum.Used))
                lobby.QuestionCards.ForEach(x => x.CardUsed = CardUsedEnum.Free);

            // TODO: ALLOW MULTIPLE ANSWERS
            lobby.CurrentQuestion = lobby.QuestionCards.Where(x => x.NumAnswers == 1 && x.CardUsed == CardUsedEnum.Free).PickRandom();

            foreach (var user in lobby.Users)
            {
                var missingCards = 10 - user.CurrentCardsOnHand.Count;

                if (missingCards == 0)
                    continue;

                if (missingCards > lobby.AnswerCards.Count(x => x.CardUsed == CardUsedEnum.Free))
                    lobby.AnswerCards.Where(x => x.CardUsed == CardUsedEnum.Used).ForEach(x => x.CardUsed = CardUsedEnum.Free);

                var cards = lobby.AnswerCards.Where(x => x.CardUsed == CardUsedEnum.Free).PickRandom(missingCards);

                foreach (var card in cards)
                {
                    card.CardUsed = CardUsedEnum.InUse;
                    user.CurrentCardsOnHand.Add(card);
                }
            }

            // Signal round start

            foreach (var user in lobby.Users)
            {
                Clients.Client(user.ConnectionId).gameRecievedScores(lobby.Users.Select(x => new { x.Name, x.NumberOfPoints }));
            }

            foreach (var user in lobby.Users)
            {
                if (user == lobby.CurrentLeader)
                    continue;

                Clients.Client(user.ConnectionId).gameShowCurrentCards(user.CurrentCardsOnHand.Select(x => new { x.Id, x.Text }), lobby.CurrentQuestion.NumAnswers);
            }

            Clients.Client(lobby.CurrentLeader.ConnectionId).gameShowCurrentQuestion(lobby.CurrentQuestion.Text);
        }

        public void ReturnToLobby(string lobbyCode)
        {
            var lobby = LobbyHub.Lobbies.SingleOrDefault(x => x.Code == lobbyCode);

            if (lobby == null)
                return;

            lobby.Users.ForEach(x => x.NumberOfPoints = 0);
            lobby.Users.ForEach(x => x.CurrentCardsOnHand.Clear());

            lobby.QuestionCards.Where(x => x.CardUsed != CardUsedEnum.Free).ForEach(x => x.CardUsed = CardUsedEnum.Free);
            lobby.AnswerCards.Where(x => x.CardUsed != CardUsedEnum.Free).ForEach(x => x.CardUsed = CardUsedEnum.Free);

            lobby.CurrentAwnsers.Clear();
            lobby.CurrentQuestion = null;

            lobby.CurrentLeader = null;
            lobby.Users.ForEach(x => x.HaveBeenLeader = false);

            lobby.ActiveGame = false;

            foreach (var user in lobby.Users)
            {
                Clients.Client(user.ConnectionId).lobbyGotoLobby(lobbyCode);
            }
        }
    }
}