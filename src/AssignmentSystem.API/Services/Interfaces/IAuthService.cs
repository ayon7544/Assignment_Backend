using AssignmentSystem.API.DTOs.Requests;
using AssignmentSystem.API.DTOs.Responses;

namespace AssignmentSystem.API.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
}