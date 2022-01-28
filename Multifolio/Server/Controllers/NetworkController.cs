using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Multifolio.Shared;
using System.Threading.Tasks;

namespace Multifolio.Server.Controllers
{
    [ApiController]
    [Route("api/chains")]
    public class NetworkController : ControllerBase
    {
        ILogger Logger { get; }
        MultifolioDbContext DbContext { get; }

        public NetworkController(ILogger<NetworkController> logger, MultifolioDbContext dbContext)
        {
            Logger = logger;
            DbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var chains = await DbContext.Chains.ToListAsync();
            return Ok(chains);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var chains = await DbContext.Chains.FirstOrDefaultAsync(a => a.Id == id);
            return Ok(chains);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Chain chain)
        {
            DbContext.Add(chain);
            await DbContext.SaveChangesAsync();
            return Ok(chain.Id);
        }

        [HttpPut]
        public async Task<IActionResult> Put(Chain chain)
        {
            DbContext.Entry(chain).State = EntityState.Modified;
            await DbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var chain = new Chain { Id = id };
            DbContext.Remove(chain);
            await DbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
