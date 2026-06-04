using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace TCToolAPI.Controllers;

[ApiController]
[Route("api/tctoolconfig")]
public class TCToolConfigController : ControllerBase
{
    private const string DbUser   = "TCtoolUser";
    private const string DbServer = "eu2-dev-taxcaddy-sqlsrv.database.windows.net";
    private const string DbName   = "eu2-dev-Log-sql-db";
    private const string DbPort   = "1433";

    private bool IsLoggedIn() =>
        HttpContext.Session.GetString("logged_in") == "true";

    private SqlConnection GetConnection()
    {
        var pwd = HttpContext.Session.GetString("db_password")
                  ?? throw new InvalidOperationException("Not authenticated.");

        var connStr =
            $"Server={DbServer},{DbPort};" +
            $"Database={DbName};" +
            $"User Id={DbUser};" +
            $"Password={pwd};" +
            "Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        return new SqlConnection(connStr);
    }

    // GET /api/tctoolconfig
    [HttpGet]
    public IActionResult GetAll()
    {
        if (!IsLoggedIn())
            return Unauthorized(new { success = false, error = "Not logged in." });

        try
        {
            using var conn = GetConnection();
            conn.Open();

            using var cmd = new SqlCommand("SELECT * FROM TCToolConfig", conn);
            using var reader = cmd.ExecuteReader();

            var columns = Enumerable.Range(0, reader.FieldCount)
                                    .Select(reader.GetName)
                                    .ToList();

            var rows = new List<Dictionary<string, object?>>();
            while (reader.Read())
            {
                var row = new Dictionary<string, object?>();
                foreach (var col in columns)
                {
                    var ordinal = reader.GetOrdinal(col);
                    row[col] = reader.IsDBNull(ordinal) ? null : reader.GetValue(ordinal);
                }
                rows.Add(row);
            }

            return Ok(new { success = true, count = rows.Count, data = rows });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = ex.Message });
        }
    }

    // GET /api/tctoolconfig/count
    [HttpGet("count")]
    public IActionResult GetCount()
    {
        if (!IsLoggedIn())
            return Unauthorized(new { success = false, error = "Not logged in." });

        try
        {
            using var conn = GetConnection();
            conn.Open();

            using var cmd = new SqlCommand("SELECT COUNT(*) FROM TCToolConfig", conn);
            var count = (int)cmd.ExecuteScalar()!;

            return Ok(new { success = true, count });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = ex.Message });
        }
    }
}
