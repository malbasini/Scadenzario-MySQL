using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Scadenzario.Models.InputModels.Scadenze;
using Scadenzario.Models.Options;
using Scadenzario.Models.ViewModels;
using Scadenzario.Models.ViewModels.Scadenze;

namespace Scadenzario.Models.Services.Applications.Scadenze
{
    public class MemoryCacheScadenzeService : ICachedScadenzeService
    {
        private readonly IScadenzeService _scadenzeService;
        private readonly IMemoryCache _memoryCache;
        private readonly IOptionsMonitor<IMemoryCacheOptions> _options;
        
        public MemoryCacheScadenzeService(IScadenzeService scadenzeService, 
            IMemoryCache memoryCache,
            IOptionsMonitor<IMemoryCacheOptions> options)
        {
            _scadenzeService = scadenzeService;
            _memoryCache = memoryCache;
            _options = options;
        }
        
        public Task<ListViewModel<ScadenzaViewModel>?> GetScadenzeAsync(ScadenzaListInputModel model)
        {
            //Metto in cache i risultati solo per le prime 5 pagine del catalogo, che reputo essere
            //le più visitate dagli utenti, e che perciò mi permettono di avere il maggior beneficio dalla cache.
            //E inoltre, metto in cache i risultati solo se l'utente non ha cercato nulla.
            //In questo modo riduco drasticamente il consumo di memoria RAM
            bool canCache = model.Page <= 5 && string.IsNullOrEmpty(model.Search);
            
            //Se canCache è true, sfrutto il meccanismo di caching
            if (canCache)
            {
                return _memoryCache.GetOrCreateAsync($"Scadenze{model.Page}-{model.OrderBy}-{model.Ascending}", cacheEntry => 
                {
                    cacheEntry.SetAbsoluteExpiration(TimeSpan.FromSeconds(_options.CurrentValue.FromSecond));
                    return _scadenzeService.GetScadenzeAsync(model);
                });
            }

            //Altrimenti uso il servizio applicativo sottostante, che recupererà sempre i valori dal database
            return _scadenzeService.GetScadenzeAsync(model);
        }

        public Task<ScadenzaEditInputModel> GetScadenzaForEditingAsync(int id)
        {
            return _scadenzeService.GetScadenzaForEditingAsync(id);
        }

        public Task<ScadenzaDetailViewModel?> GetScadenzaAsync(int id)
        {
            return _memoryCache.GetOrCreateAsync($"Scadenza{id}", cacheEntry => 
            {
                cacheEntry.SetAbsoluteExpiration(TimeSpan.FromSeconds(60)); //Esercizio: provate a recuperare il valore 60 usando il servizio di configurazione
                return _scadenzeService.GetScadenzaAsync(id);
            });
        }

        public Task<ScadenzaDetailViewModelInfo> CreateScadenzaAsync(ScadenzaCreateInputModel inputModel)
        {
            return _scadenzeService.CreateScadenzaAsync(inputModel);
        }

        public async Task<ScadenzaDetailViewModelInfo> EditScadenzaAsync(ScadenzaEditInputModel inputModel)
        {
            ScadenzaDetailViewModelInfo viewModel = await _scadenzeService.EditScadenzaAsync(inputModel);
            _memoryCache.Remove($"Scadenza{inputModel.IdScadenza}");
            return viewModel;
        }

        public Task DeleteScadenzaAsync(ScadenzaDeleteInputModel inputModel)
        {
            return _scadenzeService.DeleteScadenzaAsync(inputModel);
        }
        public List<SelectListItem> GetBeneficiari()
        {
            return _scadenzeService.GetBeneficiari();
        }

        public string GetBeneficiarioById(int IdBeneficiario)
        {
            return _scadenzeService.GetBeneficiarioById(IdBeneficiario);
        }

        public int DateDiff(DateTime inizio, DateTime fine)
        {
            return _scadenzeService.DateDiff(inizio, fine);
        }

        public bool IsDate(string date)
        {
            return _scadenzeService.IsDate(date);
        }
    }
}