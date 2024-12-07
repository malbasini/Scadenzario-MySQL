using Microsoft.AspNetCore.Identity;
using Scadenzario.Models.Entities;

namespace Scadenzario.Models.Entity;

public class ApplicationUser:IdentityUser
{
    public string FullName { get; set; }
    
    public ICollection<Beneficiario> Beneficiario { get; set; }
}