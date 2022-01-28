using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Multifolio.Server.Services;
using Multifolio.Shared;
using System.Threading;
using System.Threading.Tasks;

namespace Multifolio.Server.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountController : ControllerBase
    {
        ILogger Logger { get; }
        MultifolioDbContext DbContext { get; }
        BalanceService BalanceService { get; }

        public AccountController(ILogger<AccountController> logger, MultifolioDbContext dbContext, BalanceService balanceService)
        {
            Logger = logger;
            DbContext = dbContext;
            BalanceService = balanceService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var accounts = await DbContext.Accounts.ToListAsync(cancellationToken);
            return Ok(accounts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            var account = await DbContext.Accounts.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
            return Ok(account);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Account account, CancellationToken cancellationToken)
        {
            DbContext.Add(account);
            await DbContext.SaveChangesAsync(cancellationToken);
            return Ok(account.Id);
        }

        [HttpPut]
        public async Task<IActionResult> Put(Account account, CancellationToken cancellationToken)
        {
            DbContext.Entry(account).State = EntityState.Modified;
            await DbContext.SaveChangesAsync(cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var account = new Account { Id = id };
            DbContext.Remove(account);
            await DbContext.SaveChangesAsync(cancellationToken);
            return NoContent();
        }


        [HttpGet("{id}/balance")]
        public async Task<IActionResult> GetBalance(int id, CancellationToken cancellationToken)
        {
            var account = await DbContext.Accounts.FirstOrDefaultAsync(a => a.Id == id, cancellationToken: cancellationToken);
            var balance = await BalanceService.GetAccountBalanceFromDBAsync(account, DbContext, cancellationToken);
            return Ok(balance);
        }

        [HttpGet("balances")]
        public async Task<IActionResult> GetBalances(CancellationToken cancellationToken)
        {
            var balances = await BalanceService.GetAccountBalancesFromDBAsync(DbContext, cancellationToken);
            return Ok(balances);
        }

        // ============== TOKEN =========================================

        [HttpGet("tokens")]
        public async Task<IActionResult> GetTokens(CancellationToken cancellationToken)
        {
            var tokens = await DbContext.AccountTokens.ToListAsync(cancellationToken);
            return Ok(tokens);
        }

        [HttpGet("tokens/{id}")]
        public async Task<IActionResult> GetToken(int id, CancellationToken cancellationToken)
        {
            var token = await DbContext.AccountTokens.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
            return Ok(token);
        }

        [HttpPost("tokens")]
        public async Task<IActionResult> PostToken(AccountToken token, CancellationToken cancellationToken)
        {
            DbContext.Add(token);
            await DbContext.SaveChangesAsync(cancellationToken);
            return Ok(token.Id);
        }

        [HttpPut("tokens")]
        public async Task<IActionResult> PutToken(AccountToken token, CancellationToken cancellationToken)
        {
            DbContext.Entry(token).State = EntityState.Modified;
            await DbContext.SaveChangesAsync(cancellationToken);
            return NoContent();
        }

        [HttpDelete("tokens/{id}")]
        public async Task<IActionResult> DeleteToken(int id)
        {
            var token = new AccountToken { Id = id };
            DbContext.Remove(token);
            await DbContext.SaveChangesAsync();
            return NoContent();
        }


        [HttpGet("{aid}/tokens/{tid}/balance")]
        public async Task<IActionResult> GetTokenBalances(int aid, int tid, CancellationToken cancellationToken)
        {
            var account = await DbContext.Accounts.FirstOrDefaultAsync(x => x.Id == aid, cancellationToken: cancellationToken);
            var token = await DbContext.AccountTokens.FirstOrDefaultAsync(x => x.Id == tid, cancellationToken: cancellationToken);
            var balance = await BalanceService.GetAccountTokenBalanceFromDBAsync(account, token, DbContext, cancellationToken);
            return Ok(balance);
        }

        [HttpGet("tokens/balances")]
        public async Task<IActionResult> GetTokenBalances(CancellationToken cancellationToken)
        {
            var balances = await BalanceService.GetTokenBalancesFromDBAsync(DbContext, cancellationToken);
            return Ok(balances);
        }
    }
}
