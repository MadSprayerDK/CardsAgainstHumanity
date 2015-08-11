using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAH.Model;

namespace CAH.Database
{
    public class CardsReposiitory
    {
        private CardsDbContext Context { set; get; }

        public CardsReposiitory()
        {
            Context = new CardsDbContext();
        }

        public IQueryable<Card> Queryable()
        {
            return Context.Cards;
        }

        public List<Card> GetQuestionCards()
        {
            return Context.Cards.Where(x => x.CardType == "Q").ToList();
        }

        public List<Card> GetAnswerCards()
        {
            return Context.Cards.Where(x => x.CardType == "A").ToList();
        }
    }
}
