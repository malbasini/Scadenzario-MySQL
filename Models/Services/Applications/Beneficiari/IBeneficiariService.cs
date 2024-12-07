
using Scadenzario.Models.InputModels;
using Scadenzario.Models.InputModels.Beneficiari;
using Scadenzario.Models.ViewModels;
using Scadenzario.Models.ViewModels.Beneficiari;

namespace Scadenzario.Models.Services.Applications.Beneficiari;

public interface IBeneficiariService
{
    Task<ListViewModel<BeneficiarioViewModel>?> GetBeneficiariAsync(BeneficiarioListInputModel model);
    Task<BeneficiarioEditInputModel> GetBeneficiarioForEditingAsync(int id);
    
    Task<BeneficiarioDetailViewModel> GetBeneficiarioAsync(int id);
    Task<BeneficiarioDetailViewModel> CreateBeneficiarioAsync(BeneficiarioCreateInputModel inputModel);
    Task<BeneficiarioDetailViewModel> EditBeneficiarioAsync(BeneficiarioEditInputModel inputModel);
    Task DeleteBeneficiarioAsync(BeneficiarioDeleteInputModel inputModel);
    
    Task<bool> IsBeneficiarioAvailableAsync(string beneficiario,int id);
    
}