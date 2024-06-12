using HomeBankingV9.DTOs;
using HomeBankingV9.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingV9.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;
        /*private readonly IAccountRepository _accountRepository;*/

        public AccountsController(IAccountService accountService /*, IAccountRepository accountRepository*/)
        {
            _accountService = accountService;
            /*_accountRepository = accountRepository;*/
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult FindAllAccounts()
        {
            try
            {
                IEnumerable<AccountDTO> AccountDTO = _accountService.GetAllAccounts();
                return StatusCode(200, AccountDTO);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Client Only")]
        public IActionResult FindAccountById(long id)
        {
            try
            {
                AccountDTO accountDTO = _accountService.GetAccountById(id);
                return StatusCode(200, accountDTO);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
