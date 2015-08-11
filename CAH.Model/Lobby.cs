using System.Collections.Generic;

namespace CAH.Model
{
    public class Lobby
    {
        public string Code { set; get; }

        public readonly List<User> Users = new List<User>();
        public bool ActiveGame { set; get; }

        public List<Card> QuestionCards { set; get; }
        public List<Card> AnswerCards { set; get; }

        public Card CurrentQuestion { set; get; }
        public User CurrentLeader { set; get; }

        public List<Card> CurrentAwnsers { set; get; }
    }
}