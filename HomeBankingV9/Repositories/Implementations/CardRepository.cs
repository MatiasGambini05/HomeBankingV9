using HomeBankingV9.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeBankingV9.Repositories.Implementations
{
    public class CardRepository : RepositoryBase<Card>, ICardRepository
    {
        public CardRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }

        public Card FindCardByNumber(string number)
        {
            return FindByCondition(a => a.Number == number)
            .FirstOrDefault();
        }

        public void Save(Card card)
        {
            Create(card);
            Savechanges();
        }
    }
}
