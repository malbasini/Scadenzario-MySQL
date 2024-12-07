using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Scadenzario.Models.InputModels;
using Scadenzario.Models.InputModels.Beneficiari;
using Scadenzario.Models.Options;
using Scadenzario.Models.ViewModels;
using Scadenzario.Models.ViewModels.Beneficiari;

namespace Scadenzario.Models.Services.Applications.Beneficiari
{
    public class MemoryCacheBeneficiarioService : ICachedBeneficiarioService
    {
        private readonly IBeneficiariService _beneficiarioService;
        private readonly IMemoryCache _memoryCache;
        private readonly IOptionsMonitor<IMemoryCacheOptions> _options;
        
        public MemoryCacheBeneficiarioService(
            IBeneficiariService beneficiarioService, 
            IMemoryCache memoryCache,
            IOptionsMonitor<IMemoryCacheOptions> options)
        {
            _beneficiarioService = beneficiarioService;
            _memoryCache = memoryCache;
            _options = options;
        }
        
        public Task<ListViewModel<BeneficiarioViewModel>?> GetBeneficiariAsync(BeneficiarioListInputModel model)
        {
            //Metto in cache i risultati solo per le prime 5 pagine del catalogo, che reputo essere
            //le più visitate dagli utenti, e che perciò mi permettono di avere il maggior beneficio dalla cache.
            //E inoltre, metto in cache i risultati solo se l'utente non ha cercato nulla.
            //In questo modo riduco drasticamente il consumo di memoria RAM
            bool canCache = model.Page <= 5 && string.IsNullOrEmpty(model.Search);
            
            //Se canCache è true, sfrutto il meccanismo di caching
            if (canCache)
            {
                return _memoryCache.GetOrCreateAsync($"Beneficiari{model.Page}-{model.OrderBy}-{model.Ascending}", cacheEntry => 
                {
                    cacheEntry.SetAbsoluteExpiration(TimeSpan.FromSeconds(_options.CurrentValue.FromSecond));
                    return _beneficiarioService.GetBeneficiariAsync(model);
                });
            }

            //Altrimenti uso il servizio applicativo sottostante, che recupererà sempre i valori dal database
            return _beneficiarioService.GetBeneficiariAsync(model);
        }

        public Task<BeneficiarioEditInputModel> GetBeneficiarioForEditingAsync(int id)
        {
            return _beneficiarioService.GetBeneficiarioForEditingAsync(id);
        }

        public Task<BeneficiarioDetailViewModel?> GetBeneficiarioAsync(int id)
        {
            return _memoryCache.GetOrCreateAsync($"Beneficiario{id}", cacheEntry => 
            {
                cacheEntry.SetAbsoluteExpiration(TimeSpan.FromSeconds(60)); //Esercizio: provate a recuperare il valore 60 usando il servizio di configurazione
                return _beneficiarioService.GetBeneficiarioAsync(id);
            });
        }

        public Task<BeneficiarioDetailViewModel> CreateBeneficiarioAsync(BeneficiarioCreateInputModel inputModel)
        {
            return _beneficiarioService.CreateBeneficiarioAsync(inputModel);
        }

        public async Task<BeneficiarioDetailViewModel> EditBeneficiarioAsync(BeneficiarioEditInputModel inputModel)
        {
            BeneficiarioDetailViewModel viewModel = await _beneficiarioService.EditBeneficiarioAsync(inputModel);
            _memoryCache.Remove($"Beneficiario{inputModel.IdBeneficiario}");
            return viewModel;
        }

        public Task DeleteBeneficiarioAsync(BeneficiarioDeleteInputModel inputModel)
        {
            return _beneficiarioService.DeleteBeneficiarioAsync(inputModel);
        }

        public Task<bool> IsBeneficiarioAvailableAsync(string beneficiario,int id)
        {
            return _beneficiarioService.IsBeneficiarioAvailableAsync(beneficiario,id);
        }
    }
}