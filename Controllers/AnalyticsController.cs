using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using APIs.Models;

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AnalyticsController(IConfiguration config)
        {
            _config = config;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        }

        [HttpGet("trends")]
        public async Task<IActionResult> GetTrends([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();

            var sql = @"
                SELECT 
                    cs.ServiceName, 
                    COUNT(be.EventID) as UsageCount, 
                    SUM(be.BilledAmount) as TotalRevenue
                FROM Billed_Events be
                JOIN Clinic_Services cs ON be.ServiceID = cs.ServiceID
                WHERE be.DateOfService BETWEEN @StartDate AND @EndDate
                GROUP BY cs.ServiceName
                ORDER BY TotalRevenue DESC";

            var results = await connection.QueryAsync(sql, new { StartDate = startDate, EndDate = endDate });
            return Ok(results);
        }

        [HttpGet("suggestions")]
        public async Task<ActionResult<IEnumerable<ServiceSuggestion>>> GetSuggestions()
        {
            using var connection = GetConnection();
            await connection.OpenAsync();

            var sql = @"
                SELECT TOP 10
                    s1.ServiceName AS ServiceA,
                    s2.ServiceName AS ServiceB,
                    COUNT(*) AS Frequency
                FROM Billed_Events b1
                JOIN Billed_Events b2 
                    ON b1.DateOfService = b2.DateOfService 
                    AND b1.ProviderID = b2.ProviderID
                    AND b1.ServiceID < b2.ServiceID 
                JOIN Clinic_Services s1 ON b1.ServiceID = s1.ServiceID
                JOIN Clinic_Services s2 ON b2.ServiceID = s2.ServiceID
                GROUP BY s1.ServiceName, s2.ServiceName
                ORDER BY Frequency DESC";

            var suggestions = await connection.QueryAsync<ServiceSuggestion>(sql);
            return Ok(suggestions);
        }
    }
}