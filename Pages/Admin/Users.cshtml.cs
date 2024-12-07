using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Scadenzario.Models.Entity;
using Scadenzario.Models.Enums;
using Scadenzario.Models.InputModels.Users;

namespace Scadenzario.Pages.Admin
{
    [Authorize(Roles = nameof(Role.Administrator))]
    public class UsersModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;

        public UsersModel(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        [BindProperty]
        public UserRoleInputModel Input { get; set; }
        public IList<ApplicationUser> Users { get; private set; }
        /*
           L'attributo `[BindProperty(SupportsGet = true)]` nel codice C# indica che la proprietà a cui è collegato può essere 
           associata ai dati provenienti sia dalle richieste HTTP POST che da quelle HTTP GET. Questo attributo appartiene al 
           framework ASP.NET Core MVC.
           
           Nel codice fornito, viene applicato alla proprietà "InRole" della classe "UsersModel":
           
           [BindProperty(SupportsGet = true)]
            public Role InRole { get; set; }
           
           Ciò significa che la proprietà "InRole" riceverà il suo valore dai dati della richiesta in entrata, sia dalle richieste 
           POST che GET. Ad esempio, se il tuo URL è "https://example.com?InRole=Administrator", il valore "Administrator" 
           dalla stringa di query verrà copiato nella proprietà "InRole" quando viene effettuata una richiesta GET. 
           La parte `SupportsGet=true` lo rende possibile. Senza "SupportsGet=true", l'attributo "BindProperty" funzionerebbe solo 
           per le richieste HTTP POST e qualsiasi richiesta GET non sarebbe in grado di popolare la proprietà con i dati.
           Tieni presente che questo attributo può essere utile e potenzialmente pericoloso, poiché può esporre i 
           dati in posizioni in cui potresti non volere che siano (come la stringa di query nell'URL). 
           Assicurati di sapere cosa stai facendo quando usi "BindProperty", soprattutto in combinazione con "SupportsGet".
         */
        [BindProperty(SupportsGet = true)]
        public Role InRole { get; set; }
        /*
         * "OnGetAsync" è un nome di metodo che fa parte della convenzione ASP.NET Core Razor Pages per la gestione delle
         * richieste HTTP Get in modo asincrono. Questo metodo viene in genere utilizzato per ottenere e visualizzare dati
         * da un database back-end o da un altro servizio.
         *
         * Quando viene effettuata una richiesta GET alla pagina Razor associata, viene chiamato il metodo "OnGetAsync".
         * Questo metodo esegue le seguenti azioni:
           
           1. Imposta il valore `ViewData["Title"]`. Viene utilizzato per impostare il titolo della scheda del browser.
           
           2. Crea un nuovo oggetto "Claim", dove il tipo di attestazione è il ruolo e il valore dell'attestazione viene 
           recuperato dalla proprietà "InRole" (`InRole.ToString()`).
           
           3. Utilizza la funzione "userManager.GetUsersForClaimAsync(claim)" per ottenere in modo asincrono tutti gli utenti 
           nel sistema che dispongono dell'attestazione fornita. L'oggetto "userManager" è un'istanza di "UserManager<ApplicationUser>" 
           fornita da Dependency Injection alla classe "UsersModel".
           
           4. Infine, il metodo restituisce la pagina Razor con l'aiuto del metodo `Page()`. Gli utenti recuperati nel passaggio 
           3 e il titolo del passaggio 1 saranno disponibili per l'uso nella pagina Razor per la visualizzazione o altre operazioni.
         */
        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["Title"] = "Gestione utenti";
            Claim claim = new (ClaimTypes.Role, InRole.ToString());
            Users = await userManager.GetUsersForClaimAsync(claim);
            return Page();
        }

        public async Task<IActionResult> OnPostAssignAsync()
        {
            if (!ModelState.IsValid)
            {
                return await OnGetAsync();
            }
            //Cerco l'utente con la e-mail indicata
            ApplicationUser user = await userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                //Se l'utente ha indicato una e-mail sintatticamente valida ma che
                //non esiste nel database notifico l'errore
                ModelState.AddModelError(nameof(Input.Email), $"L'indirizzo email {Input.Email} non corrisponde ad alcun utente");
                return await OnGetAsync();
            }
            //Ho trovato l'utente, cerco tutti i Claim associati
            IList<Claim> claims = await userManager.GetClaimsAsync(user);
            Claim roleClaim = new (ClaimTypes.Role, Input.Role.ToString());
            //Se l'utente ha già il ruolo assegnato notifico l'errore
            if (claims.Any(claim => claim.Type == roleClaim.Type && claim.Value == roleClaim.Value))
            {
                ModelState.AddModelError(nameof(Input.Role), $"Il ruolo {Input.Role} è già assegnato all'utente {Input.Email}");
                return await OnGetAsync();
            }
            //Aggiungo il ruolo all'utente
            IdentityResult result = await userManager.AddClaimAsync(user, roleClaim);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, $"L'operazione è fallita: {result.Errors.FirstOrDefault()?.Description}");
                return await OnGetAsync();
            }

            TempData["ConfirmationMessage"] = $"Il ruolo {Input.Role} è stato assegnato all'utente {Input.Email}";
            return RedirectToPage(new { inrole = (int) InRole });
        }

        public async Task<IActionResult> OnPostRevokeAsync()
        {
            if (!ModelState.IsValid)
            {
                return await OnGetAsync();
            }

            ApplicationUser user = await userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                ModelState.AddModelError(nameof(Input.Email), $"L'indirizzo email {Input.Email} non corrisponde ad alcun utente");
                return await OnGetAsync();
            }

            IList<Claim> claims = await userManager.GetClaimsAsync(user);

            Claim roleClaim = new (ClaimTypes.Role, Input.Role.ToString());
            //Se il ruolo non era assegnato all'utente non lo posso revocare.
            //Gli altri step li ho già spiegati.
            if (!claims.Any(claim => claim.Type == roleClaim.Type && claim.Value == roleClaim.Value))
            {
                ModelState.AddModelError(nameof(Input.Role), $"Il ruolo {Input.Role} non era assegnato all'utente {Input.Email}");
                return await OnGetAsync();
            }
            //Rimuovo il ruolo
            IdentityResult result = await userManager.RemoveClaimAsync(user, roleClaim);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, $"L'operazione è fallita: {result.Errors.FirstOrDefault()?.Description}");
                return await OnGetAsync();
            }

            TempData["ConfirmationMessage"] = $"Il ruolo {Input.Role} è stato revocato all'utente {Input.Email}";
            return RedirectToPage(new { inrole = (int) InRole });
        }
    }

   
}