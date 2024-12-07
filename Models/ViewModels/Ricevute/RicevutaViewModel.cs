using Scadenzario.Models.Entities;

namespace Scadenzario.Models.ViewModels.Ricevute;

public class RicevutaViewModel
{
    public int Id { get; set; }
    public int IdScadenza { get; set; }
    public string FileName { get; set; }
    public string FileType { get; set; }
    public string Path {get;set;}
    public byte[] FileContent { get; set; }
    public string Beneficiario { get; set; }


    public static RicevutaViewModel FromEntity(Ricevuta ricevuta)
    {
        return new RicevutaViewModel {
            Id = ricevuta.Id,
            IdScadenza = ricevuta.IdScadenza,
            FileName = ricevuta.FileName,
            FileType = ricevuta.FileType,
            Path = ricevuta.Path,
            FileContent = ricevuta.FileContent,
            Beneficiario = ricevuta.Beneficiario
        };
    }
}