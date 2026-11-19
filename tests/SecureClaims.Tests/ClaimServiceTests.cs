using Moq;
using SecureClaims.Application.Dtos;
using SecureClaims.Application.Interfaces;
using SecureClaims.Application.Services;
using SecureClaims.Domain;

namespace SecureClaims.Tests;

public class ClaimServiceTests
{
    private readonly Mock<IClaimRepository> _repoMock = new();
    private readonly ClaimService _service;

    public ClaimServiceTests()
    {
        // On donne le FAUX repository au vrai service
        _service = new ClaimService(_repoMock.Object);
    }

    [Fact]
    public async Task Soumettre_enregistre_la_reclamation_avec_le_bon_client()
    {
        // Arrange
        var dto = new SoumettreClaimDto("POL-12345", "Dégât d'eau", 5000m);

        // Act
        var resultat = await _service.SoumettreAsync(dto, "marie");

        // Assert
        Assert.Equal("Soumise", resultat.Statut);
        _repoMock.Verify(r => r.AjouterAsync(It.Is<Claim>(c => c.ClientId == "marie")), Times.Once);
        _repoMock.Verify(r => r.SauvegarderAsync(), Times.Once);
    }

    [Fact]
    public async Task Un_client_ne_peut_pas_voir_la_reclamation_d_un_autre_client()
    {
        // Arrange : la réclamation appartient à Marie
        var claimDeMarie = new Claim("POL-12345", "Dégât d'eau", 5000m, "marie");
        _repoMock.Setup(r => r.ObtenirParIdAsync(claimDeMarie.Id)).ReturnsAsync(claimDeMarie);

        // Act : Paul essaie de la lire
        var resultat = await _service.ObtenirParIdAsync(claimDeMarie.Id, "paul", estAgent: false);

        // Assert
        Assert.Null(resultat);
    }

    [Fact]
    public async Task La_proprietaire_peut_voir_sa_reclamation()
    {
        var claimDeMarie = new Claim("POL-12345", "Dégât d'eau", 5000m, "marie");
        _repoMock.Setup(r => r.ObtenirParIdAsync(claimDeMarie.Id)).ReturnsAsync(claimDeMarie);

        var resultat = await _service.ObtenirParIdAsync(claimDeMarie.Id, "marie", estAgent: false);

        Assert.NotNull(resultat);
        Assert.Equal(claimDeMarie.Id, resultat.Id);
    }

    [Fact]
    public async Task Un_agent_peut_voir_toutes_les_reclamations()
    {
        var claimDeMarie = new Claim("POL-12345", "Dégât d'eau", 5000m, "marie");
        _repoMock.Setup(r => r.ObtenirParIdAsync(claimDeMarie.Id)).ReturnsAsync(claimDeMarie);

        var resultat = await _service.ObtenirParIdAsync(claimDeMarie.Id, "agent-julie", estAgent: true);

        Assert.NotNull(resultat);
    }
}