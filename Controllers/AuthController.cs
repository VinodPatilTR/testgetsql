using Microsoft.AspNetCore.Mvc;

namespace TCToolAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private const string AppPassword = "tctool2024";

    public record LoginRequest(string AppPassword, string DbPassword);

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest req)
    {
        if (req.AppPassword != AppPassword)
            return Ok(new { success = false, error = "Incorrect app password." });

        if (string.IsNullOrWhiteSpace(req.DbPassword))
            return Ok(new { success = false, error = "Database password is required." });

        HttpContext.Session.SetString("logged_in", "true");
        HttpContext.Session.SetString("db_password", req.DbPassword);

        return Ok(new { success = true });
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return Redirect("/login.html");
    }

    [HttpGet("check")]
    public IActionResult Check()
    {
        var loggedIn = HttpContext.Session.GetString("logged_in") == "true";
        return Ok(new { loggedIn });
    }
}
