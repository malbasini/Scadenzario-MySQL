using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Scadenze.Controllers;

namespace Scadenzario.Models.InputModels.Scadenze;

public class ScadenzaCreateInputModel
{
    [Required(ErrorMessage="Il Beneficiario è obbligatorio."),
     Display(Name = "Beneficiario")]
    public int IdBeneficiario { get; set; }
    [Required (ErrorMessage="La Data Scadenza è obbligatoria."),
     DataType(DataType.Date, ErrorMessage="Formato data non valido."),
     Display(Name = "Data Scadenza")]
    public DateTime DataScadenza { get; set; }
    [Required (ErrorMessage="L'importo è obbligatorio.")]
    [Range(1,10000,ErrorMessage="L'importo deve essere compreso tra 1 e 10.000")]
    [DataType(DataType.Currency)]
    [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
    public Decimal Importo { get; set; }
    public List<SelectListItem> Beneficiari{get;set;}
}