using Project.Domain.Entities;

namespace Project.Application.Interfaces.IServices
{
    public interface IJwtTokenService
    {
        Task<string> GenerateJwtTokenAsync(User user, CancellationToken cancellation = default);
        string GenerateRefreshToken();
    }
}
