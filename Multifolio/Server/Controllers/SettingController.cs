using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Multifolio.Shared;
using System.Threading;
using System.Threading.Tasks;

namespace Multifolio.Server.Controllers
{
    [ApiController]
    [Route("api/settings")]
    public class SettingController : ControllerBase
    {
        ILogger Logger { get; }
        MultifolioDbContext DbContext { get; }

        public SettingController(ILogger<NetworkController> logger, MultifolioDbContext dbContext)
        {
            Logger = logger;
            DbContext = dbContext;
        }


        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var chains = await DbContext.Settings.FirstOrDefaultAsync(cancellationToken);
            return Ok(chains);
        }

        [HttpPost]
        public async Task<IActionResult> Post(BackendSettings settings, CancellationToken cancellationToken)
        {
            DbContext.Add(settings);
            await DbContext.SaveChangesAsync(cancellationToken);
            return Ok(settings.Id);
        }

        [HttpPut]
        public async Task<IActionResult> Put(BackendSettings settings, CancellationToken cancellationToken)
        {
            DbContext.Entry(settings).State = EntityState.Modified;
            await DbContext.SaveChangesAsync(cancellationToken);
            return NoContent();
        }
    }
}
