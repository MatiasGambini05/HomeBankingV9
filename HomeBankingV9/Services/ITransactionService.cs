using HomeBankingV9.DTOs;

namespace HomeBankingV9.Services
{
    public interface ITransactionService
    {
        string NewTransaction(string email, TransfDTO transfDTO);
    }
}
