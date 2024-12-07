using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Scadenzario.Models.Entities;
using Scadenzario.Models.ViewModels.Ricevute;
using Scadenze.Controllers;

namespace Scadenzario.Models.InputModels.Scadenze
{

    public class ScadenzaEditInputModel
    {
        public int IdBeneficiario { get; set; }
        public int IdScadenza { get; set; }
        public string IdUser { get; set; }
        [MinLength(3, ErrorMessage = "Il beneficiario dev'essere di almeno {1} caratteri"),
         MaxLength(100, ErrorMessage = "Il beneficiario dev'essere di al massimo {1} caratteri"),
         RegularExpression(@"^[0-9A-z\u00C0-\u00ff\s\.']+$", ErrorMessage = "Beneficiario non valido"), //Questa espressione regolare include anche i caratteri accentati
         Display(Name = "Beneficiario")]
        public string Denominazione { get; set; }
        [Required(ErrorMessage = "La data scadenza è obbligatoria"), 
        Display(Name = "Data Scadenza"),
        DataType(DataType.Date, ErrorMessage="Formato data non valido.")]
        public DateTime DataScadenza { get; set; }
        [Display(Name = "Data Pagamento"), 
         DataType(DataType.Date, ErrorMessage="Formato data non valido.")]
        public DateTime? DataPagamento { get; set; }
        [Display(Name = "Importo"), 
         DataType(DataType.Currency)]
        public decimal Importo { get; set; }
        [Display(Name = "Giorni Ritardo")]
        public int? GiorniRitardo { get; set; }
        [Display(Name = "Sollecito")]
        public bool Sollecito { get; set; }
        public List<SelectListItem> Beneficiari{get;set;}

        public List<RicevutaViewModel> Ricevute { get; set; } = new List<RicevutaViewModel>();
        public static ScadenzaEditInputModel FromEntity(Scadenza scadenza)
        {
         return new ScadenzaEditInputModel {
             IdScadenza = scadenza.IDScadenza,
             IdBeneficiario = scadenza.IDBeneficiario,
             IdUser = scadenza.IDUser,
             Denominazione = scadenza.Denominazione,
             DataScadenza = scadenza.DataScadenza,
             DataPagamento = scadenza.DataPagamento,
             Importo = scadenza.Importo,
             GiorniRitardo = scadenza.GiorniRitardo,
             Sollecito = scadenza.Sollecito,
             Ricevute = scadenza.Ricevute
                 .OrderBy(r=> r.Id)
                 .Select(r=> RicevutaViewModel.FromEntity(r))
                 .ToList()
            };
        }
    }
}