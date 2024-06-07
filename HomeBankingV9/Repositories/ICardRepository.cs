using HomeBankingV9.Models;

namespace HomeBankingV9.Repositories
{
    public interface ICardRepository
    {
        Card FindCardByNumber(string number);
        void Save(Card card);
    }
}
