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