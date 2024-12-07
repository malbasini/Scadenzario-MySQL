using Microsoft.EntityFrameworkCore;
using Scadenzario.Areas.Identity.Data;
using Scadenzario.Models.Entities;
using Scadenzario.Models.InputModels.Ricevute;
using Scadenzario.Models.ViewModels.Ricevute;
using Scadenze.Models.Exceptions.Application;


namespace Scadenzario.Models.Services.Application
{
    public class EFCoreRicevutaService:IRicevuteService
    {
        private readonly ILogger<EFCoreRicevutaService> logger;
        private readonly ScadenzarioIdentityDbContext dbContext;
        public EFCoreRicevutaService(ILogger<EFCoreRicevutaService> logger, ScadenzarioIdentityDbContext dbContext)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task<RicevutaViewModel> CreateRicevutaAsync(List<RicevutaCreateInputModel> input)
        {
            foreach(var item in input)
            {
                Ricevuta ricevuta = new Ricevuta();
                ricevuta.Beneficiario = item.Beneficiario;
                ricevuta.IdScadenza =  item.IDScadenza;
                ricevuta.FileContent = item.FileContent;
                ricevuta.FileName = item.FileName;
                ricevuta.FileType = item.FileType;
                ricevuta.Path = item.Path;
                await dbContext.AddAsync(ricevuta);
            }
            if(input.Count > 0)
               await dbContext.SaveChangesAsync();
            return null;   
        }

        public async Task DeleteRicevutaAsync(int Id)
        {
            logger.LogInformation("Ricevuto {id}", Id);
            Ricevuta ricevuta = await dbContext.Ricevute.FindAsync(Id);
            if (ricevuta == null)
            {
                throw new RicevutaNotFoundException(Id);
            }
            dbContext.Remove(ricevuta);
            await dbContext.SaveChangesAsync();
        }

        public List<RicevutaViewModel> GetRicevute(int id)
        {
            logger.LogInformation("Ricevuto {id}", id);
            var queryLinq = dbContext.Ricevute
                .AsNoTracking()
                .Where(ricevuta => ricevuta.IdScadenza == id);
            List<RicevutaViewModel> viewModel = new();
            foreach(var item in queryLinq)
            {
                RicevutaViewModel view = RicevutaViewModel.FromEntity(item);
                viewModel.Add(view);
            }
            if (viewModel == null)
            {
                throw new RicevutaNotFoundException(id);
            }
            return viewModel;
        }
        public async Task<RicevutaViewModel> GetRicevutaAsync(int id)
        {
            logger.LogInformation("Ricevuto {id}", id);
            var queryLinq = dbContext.Ricevute
                .AsNoTracking()
                .Where(ricevuta => ricevuta.Id == id)
                .Select(ricevuta=>RicevutaViewModel.FromEntity(ricevuta));
                RicevutaViewModel viewModel = await queryLinq.FirstOrDefaultAsync();
                if (viewModel == null)
                {
                    logger.LogWarning("Ricevuta {id} not found", id);
                    throw new RicevutaNotFoundException(id);
                }
                return viewModel;
        }
    }
}