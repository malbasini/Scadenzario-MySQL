using Scadenzario.Models.InputModels.Ricevute;
using Scadenzario.Models.ViewModels.Ricevute;

public interface IRicevuteService
{
       
    Task<RicevutaViewModel> CreateRicevutaAsync(List<RicevutaCreateInputModel> input);
    List<RicevutaViewModel> GetRicevute(int id);
    Task DeleteRicevutaAsync(int Id);
    Task<RicevutaViewModel> GetRicevutaAsync(int id);
   
        
}