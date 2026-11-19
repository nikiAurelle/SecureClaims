using SecureClaims.Domain;

namespace SecureClaims.Application.Interfaces;

public interface IClaimRepository
{
    Task AjouterAsync(Claim claim);
    Task<Claim?> ObtenirParIdAsync(Guid id);
    Task<List<Claim>> ObtenirParClientAsync(string clientId);
    Task SauvegarderAsync();
}