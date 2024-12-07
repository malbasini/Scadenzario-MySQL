using System.ComponentModel.DataAnnotations;

namespace Scadenzario.Models.Enums
{
    public enum Role
    {
        [Display(Name = "Amministratore")]
        Administrator,
        [Display(Name = "Editore")]
        Editor
    }
}