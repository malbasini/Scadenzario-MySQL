using System.ComponentModel.DataAnnotations;
using Scadenzario.Models.Entities;
using Scadenzario.Models.ViewModels.Ricevute;

namespace Scadenzario.Models.ViewModels.Scadenze
{

    public class ScadenzaDetailViewModel
    {
        public int IDScadenza { get; set; }
        public String IDUser { get; set; }
        public int IDBeneficiario { get; set; }
        public string Denominazione { get; set; }
        [DataType(DataType.Date)]
        public DateTime DataScadenza { get; set; }
        public Decimal Importo { get; set; }
        public bool Sollecito { get; set; }
        public int? GiorniRitardo { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DataPagamento { get; set; }
        public List<RicevutaViewModel> Ricevute { get; set; } = new();

        public static ScadenzaDetailViewModel FromEntity(Scadenza scadenza)
        {
            return new ScadenzaDetailViewModel {
                IDScadenza = scadenza.IDScadenza,
                IDBeneficiario = scadenza.IDBeneficiario,
                IDUser = scadenza.IDUser,
                Denominazione = scadenza.Denominazione,
                DataScadenza = scadenza.DataScadenza,
                DataPagamento = scadenza.DataPagamento,
                Importo = scadenza.Importo,
                GiorniRitardo = scadenza.GiorniRitardo,
                Sollecito = scadenza.Sollecito,
                Ricevute = scadenza.Ricevute
                    .OrderBy(r=> r.Id)
                    .Select(r=> RicevutaViewModel.FromEntity(r))
                    .ToList()
            };
        }
    }
}