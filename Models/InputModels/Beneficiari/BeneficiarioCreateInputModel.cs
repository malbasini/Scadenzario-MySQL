using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Scadenzario.Controllers;
using Scadenze.Controllers;

namespace Scadenzario.Models.InputModels.Beneficiari;

public class BeneficiarioCreateInputModel
{
    [Required(ErrorMessage="Il beneficiario è obbligatorio"),
     MinLength(3,ErrorMessage="Il beneficiario dev'essere di almeno {1} caratteri"),
     MaxLength(100, ErrorMessage ="Il beneficiario dev'essere di al massimo {1} caratteri"),
     RegularExpression(@"^[0-9A-z\u00C0-\u00ff\s\.']+$", ErrorMessage = "Beneficiario non valido"), //Questa espressione regolare include anche i caratteri accentati
    Remote(action: nameof(BeneficiariController.IsBeneficiaryAvailable), controller: "Beneficiari", ErrorMessage = "Il beneficiario esiste già")]
    //Beneficiario
    public string Denominazione { get; set; }
    
    
    
    [Required(ErrorMessage="La descrizione è obbligatoria"),
     MinLength(10,ErrorMessage="La descrizione dev'essere di almeno {1} caratteri"),
     MaxLength(200, ErrorMessage ="La descrizione dev'essere di al massimo {1} caratteri"),
     RegularExpression(@"^[0-9A-z\u00C0-\u00ff\s\.']+$", ErrorMessage = "Descrizione non valida"), //Questa espressione regolare include anche i caratteri accentati
    ]
    //Descrizione
    public string? Descrizione { get; set; }
}