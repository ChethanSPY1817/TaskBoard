using Microsoft.AspNetCore.Identity;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Application.Services.Auth
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
