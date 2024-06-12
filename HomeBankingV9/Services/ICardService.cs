using HomeBankingV9.DTOs;

namespace HomeBankingV9.Services
{
    public interface ICardService
    {
        string NewCard(string email, TypeColorDTO typeColorDTO);
    }
}
