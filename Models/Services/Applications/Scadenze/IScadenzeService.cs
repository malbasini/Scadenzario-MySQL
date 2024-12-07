using Microsoft.AspNetCore.Mvc.Rendering;
using Scadenzario.Models.InputModels.Scadenze;
using Scadenzario.Models.ViewModels;
using Scadenzario.Models.ViewModels.Scadenze;

namespace Scadenzario.Models.Services.Applications.Scadenze;

public interface IScadenzeService
{
    Task<ListViewModel<ScadenzaViewModel>?> GetScadenzeAsync(ScadenzaListInputModel model);
    Task<ScadenzaEditInputModel> GetScadenzaForEditingAsync(int id);
    
    Task<ScadenzaDetailViewModel> GetScadenzaAsync(int id);
    Task<ScadenzaDetailViewModelInfo> CreateScadenzaAsync(ScadenzaCreateInputModel inputModel);
    Task<ScadenzaDetailViewModelInfo> EditScadenzaAsync(ScadenzaEditInputModel inputModel);
    Task DeleteScadenzaAsync(ScadenzaDeleteInputModel inputModel);
    
    List<SelectListItem> GetBeneficiari();
    
    string GetBeneficiarioById(int IdBeneficiario);
    
    int DateDiff(DateTime inizio, DateTime fine);
    bool IsDate(string date);
}