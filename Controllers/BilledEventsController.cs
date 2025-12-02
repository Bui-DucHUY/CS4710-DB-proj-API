using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using APIs.Models;

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BilledEventsController : ControllerBase
    {
        private readonly IConfiguration _config;

        public BilledEventsController(IConfiguration config)
        {
            _config = config;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BilledEvent>>> GetAll()
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            return Ok(await connection.QueryAsync<BilledEvent>("SELECT * FROM Billed_Events"));
        }

        [HttpPost]
        public async Task<ActionResult<BilledEvent>> Create(BilledEvent billedEvent)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            var sql = @"
                INSERT INTO Billed_Events (ServiceID, ProviderID, DateOfService, BilledAmount)
                VALUES (@ServiceID, @ProviderID, @DateOfService, @BilledAmount);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            var id = await connection.ExecuteScalarAsync<int>(sql, billedEvent);
            billedEvent.EventID = id;
            return Ok(billedEvent);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            await connection.ExecuteAsync("DELETE FROM Billed_Events WHERE EventID = @Id", new { Id = id });
            return NoContent();
        }
    }
}