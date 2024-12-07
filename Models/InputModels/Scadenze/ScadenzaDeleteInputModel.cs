using System.ComponentModel.DataAnnotations;

namespace Scadenzario.Models.InputModels.Scadenze;

public class ScadenzaDeleteInputModel
{
    [Required]
    public int IdScadenza { get; set; }
}