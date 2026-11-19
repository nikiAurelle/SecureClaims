using SecureClaims.Domain;

namespace SecureClaims.Application.Dtos;

// Ce que le client ENVOIE pour soumettre une réclamation
public record SoumettreClaimDto(string NumeroPolice, string Description, decimal Montant);

// Ce que l'API RENVOIE
public record ClaimDto(
    Guid Id,
    string NumeroPolice,
    string Description,
    decimal Montant,
    string Statut,
    DateTime DateSoumission,
    DateTime? DateDecision)
{
    public static ClaimDto DepuisEntite(Claim c) => new(
        c.Id, c.NumeroPolice, c.Description, c.Montant,
        c.Statut.ToString(), c.DateSoumission, c.DateDecision);
}