using Xunit;
using Microsoft.Extensions.Configuration;
using AssignmentSystem.API.DTOs.Requests;
using AssignmentSystem.API.Models;
using AssignmentSystem.API.Repositories.Interfaces;
using AssignmentSystem.API.Services;
using AssignmentSystem.Tests.Helpers;
using FluentAssertions;
using Moq;
namespace AssignmentSystem.Tests.Services;
public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _ur = new();
    private readonly Mock<IConfiguration> _cfg = new();
    private readonly AuthService _s;
    public AuthServiceTests()
    {
        _cfg.Setup(x => x["Jwt:Secret"]).Returns("test-secret-key-that-is-at-least-32-chars!!");
        _cfg.Setup(x => x["Jwt:Issuer"]).Returns("t");
        _cfg.Setup(x => x["Jwt:Audience"]).Returns("t");
        _s = new AuthService(_ur.Object, _cfg.Object);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ThrowsUnauthorized()
    {
        _ur.Setup(x => x.GetByEmailAsync("w@t.com")).ReturnsAsync((User?)null);
        var act = () => _s.LoginAsync(new LoginRequest("w@t.com", "p"));
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Login_InactiveAccount_ThrowsUnauthorized()
    {
        var u = TestDataBuilder.BuildStudent(); u.IsActive = false; u.PasswordHash = BCrypt.Net.BCrypt.HashPassword("p");
        _ur.Setup(x => x.GetByEmailAsync(u.Email)).ReturnsAsync(u);
        var act = () => _s.LoginAsync(new LoginRequest(u.Email, "p"));
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}
