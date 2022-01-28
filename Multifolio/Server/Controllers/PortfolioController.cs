using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Multifolio.Server.Services;
using Multifolio.Shared;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Multifolio.Server.Controllers
{
    [ApiController]
    [Route("api/portfolio")]
    public class PortfolioController : ControllerBase
    {
        ILogger Logger { get; }
        MultifolioDbContext DbContext { get; }
        BalanceService BalanceService { get; }
        PortfolioService PortfolioService { get; }

        public PortfolioController(PortfolioService portfolioService, BalanceService balanceService, MultifolioDbContext dbContext, ILogger<PortfolioController> logger)
        {
            Logger = logger;
            DbContext = dbContext;
            BalanceService = balanceService;
            PortfolioService = portfolioService;            
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var portfolio = await PortfolioService.GetPortfolio(cancellationToken);
            return Ok(portfolio);
        }

        [HttpGet("{range}/{grouped}/{inclEmpty}")]
        public async Task<IActionResult> GetRange(int range, int grouped, int inclEmpty, CancellationToken cancellationToken)
        {
            var portfolio = await PortfolioService.GetPortfolio((DateRange)range, Convert.ToBoolean(grouped), Convert.ToBoolean(inclEmpty), cancellationToken);
            return Ok(portfolio);
        }

        [HttpGet("refresh")]
        public async Task<IActionResult> Refresh(int range, CancellationToken cancellationToken)
        {
            var result = await BalanceService.GetCoinmarketcapData(DbContext, cancellationToken);
            await BalanceService.UpdateDatabaseAsync(result, DbContext, cancellationToken);
            return Ok();
        }
    }
}
