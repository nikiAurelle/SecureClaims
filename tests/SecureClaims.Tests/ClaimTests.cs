using SecureClaims.Domain;

namespace SecureClaims.Tests;

public class ClaimTests
{
    // Petite méthode utilitaire : crée une réclamation valide pour éviter de répéter le code
    private static Claim CreerClaimValide() =>
        new Claim("POL-12345", "Dégât d'eau au sous-sol", 5000m, "marie");

    [Fact]
    public void Nouvelle_reclamation_a_le_statut_Soumise()
    {
        // Arrange + Act
        var claim = CreerClaimValide();

        // Assert
        Assert.Equal(ClaimStatut.Soumise, claim.Statut);
        Assert.Null(claim.DateDecision);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void Montant_nul_ou_negatif_est_refuse(decimal montant)
    {
        Assert.Throws<ArgumentException>(() =>
            new Claim("POL-12345", "Dégât d'eau", montant, "marie"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Numero_de_police_vide_est_refuse(string numeroPolice)
    {
        Assert.Throws<ArgumentException>(() =>
            new Claim(numeroPolice, "Dégât d'eau", 5000m, "marie"));
    }

    [Fact]
    public void Cycle_complet_soumise_analyse_approuvee()
    {
        // Arrange
        var claim = CreerClaimValide();

        // Act
        claim.MettreEnAnalyse();
        claim.Approuver();

        // Assert
        Assert.Equal(ClaimStatut.Approuvee, claim.Statut);
        Assert.NotNull(claim.DateDecision);
    }

    [Fact]
    public void On_ne_peut_pas_approuver_sans_analyse()
    {
        var claim = CreerClaimValide();   // statut : Soumise

        Assert.Throws<InvalidOperationException>(() => claim.Approuver());
    }

    [Fact]
    public void On_ne_peut_pas_approuver_une_reclamation_refusee()
    {
        var claim = CreerClaimValide();
        claim.MettreEnAnalyse();
        claim.Refuser();

        Assert.Throws<InvalidOperationException>(() => claim.Approuver());
    }
}