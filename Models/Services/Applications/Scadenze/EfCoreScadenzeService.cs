using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyCourse.Models.Exceptions.Application;
using Scadenzario.Areas.Identity.Data;
using Scadenzario.Models.Entities;
using Scadenzario.Models.InputModels;
using Scadenzario.Models.InputModels.Beneficiari;
using Scadenzario.Models.InputModels.Scadenze;
using Scadenzario.Models.Services.Applications.Beneficiari;
using Scadenzario.Models.Services.Applications.Scadenze;
using Scadenzario.Models.ViewModels;
using Scadenzario.Models.ViewModels.Beneficiari;
using Scadenzario.Models.ViewModels.Scadenze;
using Scadenze.Models.Exceptions.Application;

namespace Scadenzario.Models.Services.Application.Scadenze
{
    public class EfCoreScadenzeService : IScadenzeService
    {
        private readonly ILogger<EfCoreScadenzeService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ScadenzarioIdentityDbContext _dbContext;
        
        public EfCoreScadenzeService(
            ILogger<EfCoreScadenzeService> logger,
            IHttpContextAccessor httpContextAccessor,
            ScadenzarioIdentityDbContext dbContext)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }
        public async Task<ListViewModel<ScadenzaViewModel>?> GetScadenzeAsync(ScadenzaListInputModel model)
        {
            string IdUser = string.Empty;
            try                                                                                                     
            { 
                IdUser = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;        
            }                                                                                                       
            catch (NullReferenceException)                                                                          
            {                                                                                                       
                throw new UserUnknownException();                                                                   
            }                                                                                                       
            
            IQueryable<Scadenza> baseQuery = _dbContext.Scadenze;

            baseQuery = (model.OrderBy, model.Ascending) switch
            {
                ("Denominazione", true) => baseQuery.OrderBy(s => s.Denominazione),
                ("Denominazione", false) => baseQuery.OrderByDescending(s => s.Denominazione),
                ("DataScadenza", true) => baseQuery.OrderBy(s => s.DataScadenza),
                ("DataScadenza", false) => baseQuery.OrderByDescending(s => s.DataScadenza),
                ("Importo", true) => baseQuery.OrderBy(s => s.Importo),
                ("Importo", false) => baseQuery.OrderByDescending(s => s.Importo),
                _ => baseQuery
            };
            if (IsDate(model.Search))
            {
                DateTime data = Convert.ToDateTime(model.Search);
                IQueryable<ScadenzaViewModel> queryLinq = baseQuery
                    .AsNoTracking()
                    .Include(Scadenza => Scadenza.Ricevute)
                    .Where(Scadenze => Scadenze.IDUser == IdUser)
                    .Where(scadenze => scadenze.DataScadenza == data)
                    .Select(scadenze => ScadenzaViewModel.FromEntity(scadenze)); //Usando metodi statici come FromEntity, la query potrebbe essere inefficiente. Mantenere il mapping nella lambda oppure usare un extension method personalizzato
                List<ScadenzaViewModel> scadenza = await queryLinq
                    .Skip(model.Offset)
                    .Take(model.Limit).ToListAsync();
                int totalCount = await queryLinq.CountAsync();
                ListViewModel<ScadenzaViewModel> results = new ListViewModel<ScadenzaViewModel>
                {
                     Results=scadenza,
                     TotalCount=totalCount
                };
                return results;
            }
            else
            {
               IQueryable<ScadenzaViewModel> queryLinq = baseQuery
                    .AsNoTracking()
                    .Include(Scadenza => Scadenza.Ricevute)
                    .Where(Scadenze => Scadenze.IDUser == IdUser)
                    .Where(scadenze => scadenze.Denominazione.Contains(model.Search))
                    .Select(scadenze => ScadenzaViewModel.FromEntity(scadenze)); //Usando metodi statici come FromEntity, la query potrebbe essere inefficiente. Mantenere il mapping nella lambda oppure usare un extension method personalizzato
                List<ScadenzaViewModel> scadenza = await queryLinq
                    .Skip(model.Offset)
                    .Take(model.Limit).ToListAsync();
                int totalCount = await queryLinq.CountAsync();
                ListViewModel<ScadenzaViewModel> results = new ListViewModel<ScadenzaViewModel>
                {
                     Results=scadenza,
                     TotalCount=totalCount
                };
                return results;
            }
        }

        public async Task<ScadenzaEditInputModel> GetScadenzaForEditingAsync(int id)
        {
            IQueryable<ScadenzaEditInputModel> queryLinq = _dbContext.Scadenze
                .AsNoTracking()
                .Where(s => s.IDScadenza == id)
                .Include(r=>r.Ricevute)
                .Select(s => ScadenzaEditInputModel.FromEntity(s)); //Usando metodi statici come FromEntity, la query potrebbe essere inefficiente. Mantenere il mapping nella lambda oppure usare un extension method personalizzato

            ScadenzaEditInputModel viewModel = await queryLinq.FirstOrDefaultAsync();

            if (viewModel == null)
            {
                _logger.LogWarning("Scadenza {id} not found", id);
                throw new ScadenzaNotFoundException(id);
            }

            return viewModel;
        }

        public async Task<ScadenzaDetailViewModel> GetScadenzaAsync(int id)
        {
            string? IdUser = string.Empty;                                                                          
            try                                                                                                    
            {                                                                                                      
                IdUser = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;       
            }                                                                                                      
            catch (NullReferenceException)                                                                         
            {                                                                                                      
                throw new UserUnknownException();                                                                  
            }                                                                                         
            IQueryable<ScadenzaDetailViewModel> queryLinq = _dbContext.Scadenze
                .AsNoTracking()
                .Where(s => s.IDScadenza == id)
                .Where(z=> z.IDUser == IdUser)
                .Include(scadenza=>scadenza.Ricevute)
                .Select(s => ScadenzaDetailViewModel.FromEntity(s)); //Usando metodi statici come FromEntity, la query potrebbe essere inefficiente. Mantenere il mapping nella lambda oppure usare un extension method personalizzato

            ScadenzaDetailViewModel viewModel = await queryLinq.FirstOrDefaultAsync();
            //.FirstOrDefaultAsync(); //Restituisce null se l'elenco è vuoto e non solleva mai un'eccezione
            //.SingleOrDefaultAsync(); //Tollera il fatto che l'elenco sia vuoto e in quel caso restituisce null, oppure se l'elenco contiene più di 1 elemento, solleva un'eccezione
            //.FirstAsync(); //Restituisce il primo elemento, ma se l'elenco è vuoto solleva un'eccezione
            //.SingleAsync(); //Restituisce il primo elemento, ma se l'elenco è vuoto o contiene più di un elemento, solleva un'eccezione

            if (viewModel == null)
            {
                _logger.LogWarning("Scadenza {id} not found", id);
                throw new ScadenzaNotFoundException(id);
            }

            return viewModel;
        }

        public async Task<ScadenzaDetailViewModelInfo> CreateScadenzaAsync(ScadenzaCreateInputModel inputModel)
        {
            string denominazione = GetBeneficiarioById(inputModel.IdBeneficiario);
            int idBeneficiario = inputModel.IdBeneficiario;
            decimal importo = inputModel.Importo;
            DateTime dataScadenza = inputModel.DataScadenza;
            string? UserId = string.Empty;
            try
            {
                UserId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }
            catch (NullReferenceException)
            {
                throw new UserUnknownException();
            }
            var scadenza = new Scadenza(dataScadenza,importo,denominazione);
            scadenza.IDUser = UserId;
            scadenza.IDBeneficiario = idBeneficiario;
            _dbContext.Add(scadenza);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException exc)
            {
                throw new DbUpdateException(exc.Message);
            }
            return ScadenzaDetailViewModelInfo.FromEntity(scadenza);
        }

        public async Task<ScadenzaDetailViewModelInfo> EditScadenzaAsync(ScadenzaEditInputModel inputModel)
        {
            Scadenza scadenza = await _dbContext.Scadenze.FindAsync(inputModel.IdScadenza);
            
            if (scadenza == null)
            {
                throw new ScadenzaNotFoundException(inputModel.IdScadenza);
            }

            scadenza.Denominazione = inputModel.Denominazione;
            scadenza.IDBeneficiario = inputModel.IdBeneficiario;
            scadenza.IDUser = inputModel.IdUser;
            scadenza.DataScadenza = inputModel.DataScadenza;
            scadenza.Importo = inputModel.Importo;
            scadenza.DataPagamento = inputModel.DataPagamento;
            scadenza.GiorniRitardo = inputModel.GiorniRitardo;
            scadenza.Sollecito = inputModel.Sollecito;
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new OptimisticConcurrencyException();
            }
            catch (DbUpdateException exc)
            {
                throw new DbUpdateException(exc.Message);
            }
            return ScadenzaDetailViewModelInfo.FromEntity(scadenza);
        }
        public async Task DeleteScadenzaAsync(ScadenzaDeleteInputModel inputModel)
        {
            Scadenza? scadenza = await _dbContext.Scadenze.FindAsync(inputModel.IdScadenza);
            if (scadenza == null)
            {
                throw new ScadenzaNotFoundException(inputModel.IdScadenza);
            }
            _dbContext.Remove(scadenza);
            await _dbContext.SaveChangesAsync();
        }

        public List<SelectListItem> GetBeneficiari()
        {
            string IdUser;
            try
            {
                IdUser = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }
            catch (NullReferenceException)
            {
                throw new UserUnknownException();
            }
            List<Beneficiario> beneficiari = new List<Beneficiario>();
            beneficiari = (from b in _dbContext.Beneficiari.Where(z => z.IdUser == IdUser) select b).ToList();
            var beneficiario = beneficiari.Select(b => new SelectListItem
            {
                Text = b.Denominazione,
                Value = b.IdBeneficiario.ToString()
            }).ToList();
            return beneficiario;
        }

        public string GetBeneficiarioById(int id)
        {
            _logger.LogInformation("Ricevuto identificativo beneficiario {id}", id);
            string Beneficiario = _dbContext.Beneficiari
                .Where(t => t.IdBeneficiario == id)
                .Select(t => t.Denominazione).Single();
            return Beneficiario;
        }
        //Calcolo giorni ritardo o giorni mancanti al pagamento
        public int DateDiff(DateTime inizio, DateTime fine)
        {
            int giorni = 0;
            giorni = (inizio.Date - fine.Date).Days;
            return giorni;
        }
        public bool IsDate(string date)
        {
            try
            {
                string[] formats = { "dd/MM/yyyy" };
                DateTime parsedDateTime;
                return DateTime.TryParseExact(date, formats, new CultureInfo("it-IT"), DateTimeStyles.None, out parsedDateTime);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}