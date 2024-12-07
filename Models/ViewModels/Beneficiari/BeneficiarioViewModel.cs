using Scadenzario.Models.Entities;
using Scadenzario.Models.ViewModels.Beneficiari;

namespace Scadenzario.Models.ViewModels;

public class BeneficiarioViewModel
{
    public int IdBeneficiario { get; set; }
    public string Denominazione { get; set; }
    public string Descrizione { get; set; }
    public string Email { get; set; }
    public string Telefono { get; set; }
    public string SitoWeb { get; set; }
    public String IdUser { get; set; }

    public static BeneficiarioViewModel FromEntity(Beneficiario beneficiario)
    {
        return new BeneficiarioViewModel {
            IdBeneficiario = beneficiario.IdBeneficiario,
            Denominazione = beneficiario.Denominazione,
            Descrizione = beneficiario.Descrizione,
            Email = beneficiario.Email,
            Telefono = beneficiario.Telefono,
            SitoWeb = beneficiario.SitoWeb,
            IdUser = beneficiario.IdUser
        };
    }
}