using System.ComponentModel.DataAnnotations;

namespace Scadenzario.Models.InputModels;

public class BeneficiarioDeleteInputModel
{
    [Required]
    public int IdBeneficiario { get; set; }
}