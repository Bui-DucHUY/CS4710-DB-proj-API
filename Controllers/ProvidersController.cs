using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using APIs.Models;

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProvidersController : ControllerBase
    {
        private readonly IConfiguration _config;
        public ProvidersController(IConfiguration config)
        {
            _config = config;
        }
        private SqlConnection CreateConnection()
        {
            var connectionString = _config.GetConnectionString("DefaultConnection");
            return new SqlConnection(connectionString);
        }
        [HttpGet] //api/providers
        public async Task<ActionResult<IEnumerable<Provider>>> GetProviders()
        {
            string query = "SELECT * FROM Providers";
            using (var connection = CreateConnection())
            {
                var providers = await connection.QueryAsync<Provider>(query);
                return Ok(providers);
            }
        }
        [HttpGet("{id}")] //api/providers/1
        public async Task<ActionResult<Provider>> GetProvider(int id)
        {
            string query = "SELECT * FROM Providers WHERE ProviderId = @Id";
            using (var connection = CreateConnection())
            {
                var provider = await connection.QuerySingleOrDefaultAsync<Provider>(query, new { Id = id });
                if (provider == null)
                {
                    return NotFound();
                }
                return Ok(provider);
            }
        }


        /*
        public IActionResult Index()
        {
            return View();
        }*/
    }
}
