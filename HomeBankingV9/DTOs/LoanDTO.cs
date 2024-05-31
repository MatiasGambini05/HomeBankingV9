using HomeBankingV9.Models;

namespace HomeBankingV9.DTOs
{
    public class LoanDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public double MaxAmount { get; set; }
        public string Payments { get; set; }

        public LoanDTO(Loan Loan)
        {
            Id = Loan.Id;
            Name = Loan.Name;
            MaxAmount = Loan.MaxAmount;
            Payments = Loan.Payments;
        }
    }
}
