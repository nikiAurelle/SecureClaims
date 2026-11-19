using SecureClaims.Application.Dtos;
using SecureClaims.Application.Interfaces;
using SecureClaims.Domain;

namespace SecureClaims.Application.Services;

public class ClaimService
{
    private readonly IClaimRepository _repository;

    // Injection de dépendances : on REÇOIT le repository, on ne le crée pas
    public ClaimService(IClaimRepository repository)
    {
        _repository = repository;
    }

    public async Task<ClaimDto> SoumettreAsync(SoumettreClaimDto dto, string clientId)
    {
        var claim = new Claim(dto.NumeroPolice, dto.Description, dto.Montant, clientId);
        await _repository.AjouterAsync(claim);
        await _repository.SauvegarderAsync();
        return ClaimDto.DepuisEntite(claim);
    }

    public async Task<List<ClaimDto>> ObtenirMesClaimsAsync(string clientId)
    {
        var claims = await _repository.ObtenirParClientAsync(clientId);
        return claims.Select(ClaimDto.DepuisEntite).ToList();
    }

    public async Task<ClaimDto?> ObtenirParIdAsync(Guid id, string utilisateurId, bool estAgent)
    {
        var claim = await _repository.ObtenirParIdAsync(id);
        if (claim is null)
            return null;

        // Contrôle d'accès : un client ne voit QUE ses réclamations
        if (!estAgent && claim.ClientId != utilisateurId)
            return null;   // on répond "introuvable", pas "interdit" (voir explication)

        return ClaimDto.DepuisEntite(claim);
    }
}