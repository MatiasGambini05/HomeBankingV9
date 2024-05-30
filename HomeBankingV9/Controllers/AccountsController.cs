using HomeBankingV9.DTOs;
using HomeBankingV9.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HomeBankingV9.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;

        public AccountsController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [HttpGet]
        public IActionResult FindAllAccounts()
        {
            try
            {
                var accounts = _accountRepository.FindAllAccounts();
                var accountsDTO = accounts.Select(a => new AccountDTO(a)).ToList();
                return StatusCode(StatusCodes.Status200OK, accountsDTO);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult FindAccountById(long id)
        {
            try
            {
                var account = _accountRepository.FindAccountById(id);
                var accountDTO = new AccountDTO(account);
                return StatusCode(StatusCodes.Status200OK, accountDTO);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
    }
}
