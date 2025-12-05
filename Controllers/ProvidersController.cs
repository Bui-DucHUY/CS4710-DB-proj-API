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
            using var connection = GetConnection();
            await connection.OpenAsync();
            return Ok(await connection.QueryAsync<Provider>("SELECT * FROM Providers"));
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? term = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            var offset = (page - 1) * pageSize;

            var sqlData = @"
                SELECT * FROM Providers
                WHERE (@Term IS NULL 
                       OR FirstName LIKE '%' + @Term + '%' 
                       OR LastName LIKE '%' + @Term + '%'
                       OR Specialty LIKE '%' + @Term + '%')
                ORDER BY LastName, FirstName
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var sqlCount = @"
                SELECT COUNT(*) FROM Providers
                WHERE (@Term IS NULL 
                       OR FirstName LIKE '%' + @Term + '%' 
                       OR LastName LIKE '%' + @Term + '%'
                       OR Specialty LIKE '%' + @Term + '%')";

            var multi = await connection.QueryMultipleAsync(
                sqlData + ";" + sqlCount,
                new { Term = term, Offset = offset, PageSize = pageSize });

            return Ok(new
            {
                Data = await multi.ReadAsync<Provider>(),
                TotalCount = await multi.ReadFirstAsync<int>(),
                Page = page
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Provider>> GetOne(int id)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            var result = await connection.QueryFirstOrDefaultAsync<Provider>("SELECT * FROM Providers WHERE ProviderID = @Id", new { Id = id });
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


        public class ProviderCapability
        {
            public int ProviderID { get; set; }
            public int ServiceID { get; set; }
        }

        [HttpGet("capabilities")]
        public async Task<ActionResult<IEnumerable<ProviderCapability>>> GetAllCapabilities()
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            return Ok(await connection.QueryAsync<ProviderCapability>("SELECT * FROM Provider_Capabilities"));
        }

        [HttpPost("{id}/capabilities")]
        public async Task<IActionResult> UpdateCapabilities(int id, [FromBody] List<int> serviceIds)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                await connection.ExecuteAsync(
                    "DELETE FROM Provider_Capabilities WHERE ProviderID = @Pid",
                    new { Pid = id }, transaction);

                if (serviceIds != null && serviceIds.Any())
                {
                    var sql = "INSERT INTO Provider_Capabilities (ProviderID, ServiceID) VALUES (@Pid, @Sid)";
                    await connection.ExecuteAsync(sql, serviceIds.Select(sid => new { Pid = id, Sid = sid }), transaction);
                }

                transaction.Commit();
                return Ok();
            }
            catch
            {
                transaction.Rollback();
                return StatusCode(500, "Error updating capabilities");
            }
        }
    }
}