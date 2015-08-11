using System.Dynamic;

namespace CAH.Model
{
    public class Card
    {
        public int Id { set; get; }
        public string CardType { set; get; }
        public string Text { set; get; }
        public int NumAnswers { set; get; }
        public string Expansion { set; get; }

        public CardUsedEnum CardUsed { set; get; }
    }
}