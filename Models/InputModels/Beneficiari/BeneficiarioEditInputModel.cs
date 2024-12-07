using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Scadenzario.Models.Entities;
using Scadenze.Controllers;

namespace Scadenzario.Models.InputModels.Beneficiari
{

    public class BeneficiarioEditInputModel
    {
        [Required] public int IdBeneficiario { get; set; }

        [Required(ErrorMessage = "Il beneficiario è obbligatorio"),
         MinLength(3, ErrorMessage = "Il beneficiario dev'essere di almeno {1} caratteri"),
         MaxLength(100, ErrorMessage = "Il beneficiario dev'essere di al massimo {1} caratteri"),
         RegularExpression(@"^[0-9A-z\u00C0-\u00ff\s\.']+$", ErrorMessage = "Beneficiario non valido"), //Questa espressione regolare include anche i caratteri accentati
         Display(Name = "Beneficiario")]
        public string Denominazione { get; set; }

        [MinLength(10, ErrorMessage = "La descrizione dev'essere di almeno {1} caratteri"),
         MaxLength(100, ErrorMessage = "La descrizione dev'essere di massimo {1} caratteri"),
         Display(Name = "Descrizione")]
        public string Descrizione { get; set; }

        [Required(AllowEmptyStrings = true),
         Display(Name = "Email"),
         DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = true),
          Display(Name = "Telefono"),
          DataType(DataType.PhoneNumber)]
        public string Telefono { get; set; }

        [Required(AllowEmptyStrings = true),
         Display(Name = "Sito web"),
         DataType(DataType.Url)]
        public string SitoWeb { get; set; }

        public string IdUser { get; set; }
        
        public static BeneficiarioEditInputModel FromEntity(Beneficiario beneficiario)
        {
         return new BeneficiarioEditInputModel {
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