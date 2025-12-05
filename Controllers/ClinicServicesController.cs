using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using APIs.Models;

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClinicServicesController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ClinicServicesController(IConfiguration config)
        {
            _config = config;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClinicService>>> GetAll()
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            return Ok(await connection.QueryAsync<ClinicService>("SELECT * FROM Clinic_Services"));
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? term = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            var offset = (page - 1) * pageSize;

            var sqlData = @"
                SELECT * FROM Clinic_Services
                WHERE (@Term IS NULL 
                       OR ServiceName LIKE '%' + @Term + '%' 
                       OR CPTCode LIKE '%' + @Term + '%')
                ORDER BY ServiceName
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var sqlCount = @"
                SELECT COUNT(*) FROM Clinic_Services
                WHERE (@Term IS NULL 
                       OR ServiceName LIKE '%' + @Term + '%' 
                       OR CPTCode LIKE '%' + @Term + '%')";

            var multi = await connection.QueryMultipleAsync(
                sqlData + ";" + sqlCount,
                new { Term = term, Offset = offset, PageSize = pageSize });

            return Ok(new
            {
                Data = await multi.ReadAsync<ClinicService>(),
                TotalCount = await multi.ReadFirstAsync<int>(),
                Page = page
            });
        }

        [HttpPost]
        public async Task<ActionResult<ClinicService>> Create(ClinicService service)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            var sql = @"
                INSERT INTO Clinic_Services (ServiceName, Fee, CPTCode)
                VALUES (@ServiceName, @Fee, @CPTCode);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            var id = await connection.ExecuteScalarAsync<int>(sql, service);
            service.ServiceID = id;
            return Ok(service);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ClinicService service)
        {
            if (id != service.ServiceID) return BadRequest();

            using var connection = GetConnection();
            await connection.OpenAsync();
            var sql = @"
                UPDATE Clinic_Services 
                SET ServiceName = @ServiceName, Fee = @Fee, CPTCode = @CPTCode
                WHERE ServiceID = @ServiceID";

            await connection.ExecuteAsync(sql, service);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            await connection.ExecuteAsync("DELETE FROM Clinic_Services WHERE ServiceID = @Id", new { Id = id });
            return NoContent();
        }
    }
}