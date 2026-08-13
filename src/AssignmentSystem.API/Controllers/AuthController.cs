using AssignmentSystem.API.DTOs.Requests;
using AssignmentSystem.API.DTOs.Responses;
using AssignmentSystem.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentSystem.API.Controllers;

[ApiController]
[Route("api/auth")]
[Tags("Auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _auth.LoginAsync(request);
        return Ok(ApiResponse<LoginResponse>.Ok(result, "Login successful"));
    }
}