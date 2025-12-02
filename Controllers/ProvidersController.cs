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

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Provider>>> GetAll()
        {
            try
            {
                using var connection = GetConnection();
                await connection.OpenAsync();
                var providers = await connection.QueryAsync<Provider>("SELECT * FROM Providers");
                return Ok(providers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "DB Error", Details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Provider>> GetOne(int id)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            var result = await connection.QueryFirstOrDefaultAsync<Provider>(
                "SELECT * FROM Providers WHERE ProviderID = @Id", new { Id = id });

            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<Provider>> Create(Provider provider)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            var sql = @"
                INSERT INTO Providers (FirstName, LastName, Addrss, Specialty)
                VALUES (@FirstName, @LastName, @Addrss, @Specialty);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            var id = await connection.ExecuteScalarAsync<int>(sql, provider);
            provider.ProviderID = id;
            return CreatedAtAction(nameof(GetOne), new { id = id }, provider);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Provider provider)
        {
            if (id != provider.ProviderID) return BadRequest();

            using var connection = GetConnection();
            await connection.OpenAsync();
            var sql = @"UPDATE Providers SET FirstName=@FirstName, LastName=@LastName, Addrss=@Addrss, Specialty=@Specialty WHERE ProviderID=@ProviderID";
            await connection.ExecuteAsync(sql, provider);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            await connection.ExecuteAsync("DELETE FROM Providers WHERE ProviderID = @Id", new { Id = id });
            return NoContent();
        }
    }
}