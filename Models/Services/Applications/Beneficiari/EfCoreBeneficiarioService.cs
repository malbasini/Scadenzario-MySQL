using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyCourse.Models.Exceptions.Application;
using Scadenzario.Areas.Identity.Data;
using Scadenzario.Models.Entities;
using Scadenzario.Models.InputModels;
using Scadenzario.Models.InputModels.Beneficiari;
using Scadenzario.Models.Services.Applications.Beneficiari;
using Scadenzario.Models.ViewModels;
using Scadenzario.Models.ViewModels.Beneficiari;
using Scadenze.Models.Exceptions.Application;

namespace Scadenzario.Models.Services.Application.Beneficiari
{
    public class EfCoreBeneficiarioService : IBeneficiariService
    {
        private readonly ILogger<EfCoreBeneficiarioService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ScadenzarioIdentityDbContext _dbContext;
        private readonly IOptionsMonitor<MemoryCacheBeneficiarioService> _options;
        
        
        
        
        public EfCoreBeneficiarioService(
            ILogger<EfCoreBeneficiarioService> logger,
            IHttpContextAccessor httpContextAccessor,
            ScadenzarioIdentityDbContext dbContext
           )
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }
        
        
        
        public async Task<ListViewModel<BeneficiarioViewModel>?> GetBeneficiariAsync(BeneficiarioListInputModel model)
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
            
            IQueryable<Beneficiario> baseQuery = _dbContext.Beneficiari;

            baseQuery = (model.OrderBy, model.Ascending) switch
            {
                ("Denominazione", true) => baseQuery.OrderBy(b => b.Denominazione),
                ("Denominazione", false) => baseQuery.OrderByDescending(b => b.Denominazione),
                ("Descrizione", true) => baseQuery.OrderBy(b => b.Descrizione),
                ("Descrizione", false) => baseQuery.OrderByDescending(b => b.Descrizione),
                _ => baseQuery
            };

            IQueryable<Beneficiario> queryLinq = baseQuery
                .Where(b => b.Denominazione.Contains(model.Search))
                .Where(b=> b.IdUser == IdUser)
                .AsNoTracking();

            List<BeneficiarioViewModel> beneficiari = await queryLinq
                .Skip(model.Offset)
                .Take(model.Limit)
                .Select(b => BeneficiarioViewModel.FromEntity(b)) //Usando metodi statici come FromEntity, la query potrebbe essere inefficiente. Mantenere il mapping nella lambda oppure usare un extension method personalizzato
                .ToListAsync(); //La query al database viene inviata qui, quando manifestiamo l'intenzione di voler leggere i risultati

            int totalCount = await queryLinq.CountAsync();

            ListViewModel<BeneficiarioViewModel> result = new ListViewModel<BeneficiarioViewModel>
            {
                Results = beneficiari,
                TotalCount = totalCount
            };

            return result;
        }

        public async Task<BeneficiarioEditInputModel> GetBeneficiarioForEditingAsync(int id)
        {
            IQueryable<BeneficiarioEditInputModel> queryLinq = _dbContext.Beneficiari
                .AsNoTracking()
                .Where(b => b.IdBeneficiario == id)
                .Select(b => BeneficiarioEditInputModel.FromEntity(b)); //Usando metodi statici come FromEntity, la query potrebbe essere inefficiente. Mantenere il mapping nella lambda oppure usare un extension method personalizzato

            BeneficiarioEditInputModel viewModel = await queryLinq.FirstOrDefaultAsync();

            if (viewModel == null)
            {
                _logger.LogWarning("Beneficiario {id} not found", id);
                throw new BeneficiarioNotFoundException(id);
            }

            return viewModel;
        }

        public async Task<BeneficiarioDetailViewModel> GetBeneficiarioAsync(int id)
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
            IQueryable<BeneficiarioDetailViewModel> queryLinq = _dbContext.Beneficiari
                .AsNoTracking()
                .Where(b => b.IdBeneficiario == id)
                .Where(z=> z.IdUser == IdUser)
                .Select(b => BeneficiarioDetailViewModel.FromEntity(b)); //Usando metodi statici come FromEntity, la query potrebbe essere inefficiente. Mantenere il mapping nella lambda oppure usare un extension method personalizzato

            BeneficiarioDetailViewModel viewModel = await queryLinq.FirstOrDefaultAsync();
            //.FirstOrDefaultAsync(); //Restituisce null se l'elenco è vuoto e non solleva mai un'eccezione
            //.SingleOrDefaultAsync(); //Tollera il fatto che l'elenco sia vuoto e in quel caso restituisce null, oppure se l'elenco contiene più di 1 elemento, solleva un'eccezione
            //.FirstAsync(); //Restituisce il primo elemento, ma se l'elenco è vuoto solleva un'eccezione
            //.SingleAsync(); //Restituisce il primo elemento, ma se l'elenco è vuoto o contiene più di un elemento, solleva un'eccezione

            if (viewModel == null)
            {
                _logger.LogWarning("Beneficiario {id} not found", id);
                throw new BeneficiarioNotFoundException(id);
            }

            return viewModel;
        }

        public async Task<BeneficiarioDetailViewModel> CreateBeneficiarioAsync(BeneficiarioCreateInputModel inputModel)
        {
            string? denominazione = inputModel.Denominazione;
            string? descrizione = inputModel.Descrizione;
            string? UserId = string.Empty;
            try
            {
                UserId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }
            catch (NullReferenceException)
            {
                throw new UserUnknownException();
            }
            var beneficiario = new Beneficiario(denominazione,descrizione);
            beneficiario.IdUser = UserId;
            _dbContext.Add(beneficiario);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException exc)
            {
                throw new DbUpdateException(exc.Message);
            }

            return BeneficiarioDetailViewModel.FromEntity(beneficiario);
        }

        public async Task<BeneficiarioDetailViewModel> EditBeneficiarioAsync(BeneficiarioEditInputModel inputModel)
        {
            Beneficiario beneficiario = await _dbContext.Beneficiari.FindAsync(inputModel.IdBeneficiario);
            
            if (beneficiario == null)
            {
                throw new BeneficiarioNotFoundException(inputModel.IdBeneficiario);
            }

            beneficiario.ChangeDenominazione(inputModel.Denominazione);
            beneficiario.ChangeDescrizione(inputModel.Descrizione);
            beneficiario.IdUser = inputModel.IdUser;
            beneficiario.Email = inputModel.Email;
            beneficiario.Telefono = inputModel.Telefono;
            beneficiario.SitoWeb = inputModel.SitoWeb;
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
            return BeneficiarioDetailViewModel.FromEntity(beneficiario);
        }
        public async Task DeleteBeneficiarioAsync(BeneficiarioDeleteInputModel inputModel)
        {
            Beneficiario? beneficiario = await _dbContext.Beneficiari.FindAsync(inputModel.IdBeneficiario);
            if (beneficiario == null)
            {
                throw new BeneficiarioNotFoundException(inputModel.IdBeneficiario);
            }
            _dbContext.Remove(beneficiario);
            await _dbContext.SaveChangesAsync();
        }
        
        
        public async Task<bool> IsBeneficiarioAvailableAsync(string beneficiario, int id)
        {
            string? UserId = string.Empty;
            try
            {
                UserId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }
            catch (NullReferenceException)
            {
                throw new UserUnknownException();
            }
            //await dbContext.Courses.AnyAsync(course => course.Title == title);
            bool titleExists = await _dbContext.Beneficiari.AnyAsync(b => EF.Functions.Like(b.Denominazione, beneficiario) && b.IdBeneficiario != id  && b.IdUser == UserId);
            return !titleExists;
        }
    }
}