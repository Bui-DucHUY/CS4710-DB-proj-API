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
        public async Task<ActionResult<IEnumerable<BilledEvent>>> GetAll() //unused, keep for reference
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            return Ok(await connection.QueryAsync<BilledEvent>("SELECT * FROM Billed_Events ORDER BY DateOfService DESC"));
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string? term = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();

            var offset = (page - 1) * pageSize;

            // This query Joins tables solely for filtering purposes, 
            // but still selects the BilledEvent object structure.
            var sqlData = @"
                SELECT be.* FROM Billed_Events be
                JOIN Providers p ON be.ProviderID = p.ProviderID
                JOIN Clinic_Services s ON be.ServiceID = s.ServiceID
                WHERE (@Term IS NULL 
                       OR (p.FirstName + ' ' + p.LastName) LIKE '%' + @Term + '%'
                       OR s.ServiceName LIKE '%' + @Term + '%'
                       OR CAST(be.EventID AS NVARCHAR) LIKE '%' + @Term + '%')
                ORDER BY be.DateOfService DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var sqlCount = @"
                SELECT COUNT(*) 
                FROM Billed_Events be
                JOIN Providers p ON be.ProviderID = p.ProviderID
                JOIN Clinic_Services s ON be.ServiceID = s.ServiceID
                WHERE (@Term IS NULL 
                       OR (p.FirstName + ' ' + p.LastName) LIKE '%' + @Term + '%'
                       OR s.ServiceName LIKE '%' + @Term + '%'
                       OR CAST(be.EventID AS NVARCHAR) LIKE '%' + @Term + '%')";

            var multi = await connection.QueryMultipleAsync(
                sqlData + ";" + sqlCount,
                new { Term = term, Offset = offset, PageSize = pageSize });

            var items = await multi.ReadAsync<BilledEvent>();
            var total = await multi.ReadFirstAsync<int>();

            return Ok(new
            {
                Data = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            });
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, BilledEvent billedEvent)
        {
            if (id != billedEvent.EventID) return BadRequest();

            using var connection = GetConnection();
            await connection.OpenAsync();
            var sql = @"
                UPDATE Billed_Events 
                SET ServiceID = @ServiceID, ProviderID = @ProviderID, DateOfService = @DateOfService, BilledAmount = @BilledAmount
                WHERE EventID = @EventID";

            await connection.ExecuteAsync(sql, billedEvent);
            return NoContent();
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