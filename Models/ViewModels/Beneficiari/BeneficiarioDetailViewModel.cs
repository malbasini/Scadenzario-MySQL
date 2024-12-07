using Scadenzario.Models.Entities;

namespace Scadenzario.Models.ViewModels.Beneficiari
{

    public class BeneficiarioDetailViewModel
    {
        public int IdBeneficiario { get; set; }
        public string Denominazione { get; set; }
        public string Descrizione { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string SitoWeb { get; set; }
        public String IdUser { get; set; }

        public static BeneficiarioDetailViewModel FromEntity(Beneficiario beneficiario)
        {
            return new BeneficiarioDetailViewModel {
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
}