using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using APIs.Models;

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CptCodesController : ControllerBase
    {
        private readonly IConfiguration _config;

        public CptCodesController(IConfiguration config)
        {
            _config = config;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CptCode>>> GetAll()
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            return Ok(await connection.QueryAsync<CptCode>("SELECT * FROM CPT_Codes"));
        }

        //[HttpPost]
        //public async Task<ActionResult> Create(CptCode code)
        //{
        //    using var connection = GetConnection();
        //    await connection.OpenAsync();
        //    var sql = "INSERT INTO CPT_Codes (CPTCode, Descript, Category) VALUES (@CPTCode, @Descript, @Category)";
        //    await connection.ExecuteAsync(sql, code);
        //    return Ok();
        //}
    }
}