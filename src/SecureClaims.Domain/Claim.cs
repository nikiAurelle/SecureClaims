namespace SecureClaims.Domain;

public class Claim
{
    public Guid Id { get; private set; }
    public string NumeroPolice { get; private set; }
    public string Description { get; private set; }
    public decimal Montant { get; private set; }
    public string ClientId { get; private set; }
    public ClaimStatut Statut { get; private set; }
    public DateTime DateSoumission { get; private set; }
    public DateTime? DateDecision { get; private set; }

    public Claim(string numeroPolice, string description, decimal montant, string clientId)
    {
        if (string.IsNullOrWhiteSpace(numeroPolice))
            throw new ArgumentException("Le numéro de police est obligatoire.", nameof(numeroPolice));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("La description est obligatoire.", nameof(description));
        if (montant <= 0)
            throw new ArgumentException("Le montant doit être supérieur à 0.", nameof(montant));
        if (string.IsNullOrWhiteSpace(clientId))
            throw new ArgumentException("Le client est obligatoire.", nameof(clientId));

        Id = Guid.NewGuid();
        NumeroPolice = numeroPolice;
        Description = description;
        Montant = montant;
        ClientId = clientId;
        Statut = ClaimStatut.Soumise;
        DateSoumission = DateTime.UtcNow;
    }

    public void MettreEnAnalyse()
    {
        if (Statut != ClaimStatut.Soumise)
            throw new InvalidOperationException("Seule une réclamation soumise peut être mise en analyse.");
        Statut = ClaimStatut.EnAnalyse;
    }

    public void Approuver()
    {
        if (Statut != ClaimStatut.EnAnalyse)
            throw new InvalidOperationException("Seule une réclamation en analyse peut être approuvée.");
        Statut = ClaimStatut.Approuvee;
        DateDecision = DateTime.UtcNow;
    }

    public void Refuser()
    {
        if (Statut != ClaimStatut.EnAnalyse)
            throw new InvalidOperationException("Seule une réclamation en analyse peut être refusée.");
        Statut = ClaimStatut.Refusee;
        DateDecision = DateTime.UtcNow;
    }
}