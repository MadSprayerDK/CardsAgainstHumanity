using System.Collections.Generic;

namespace CAH.Model
{
    public class User
    {
        public string Name { set; get; }
        public string ConnectionId { set; get; }

        public List<Card> CurrentCardsOnHand { set; get; }
        public bool HaveBeenLeader { set; get; }

        public int NumberOfPoints { set; get; }
    }
}