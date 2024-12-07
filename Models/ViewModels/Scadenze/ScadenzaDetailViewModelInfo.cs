using Scadenzario.Models.Entities;
using Scadenzario.Models.ViewModels.Ricevute;

namespace Scadenzario.Models.ViewModels.Scadenze
{

    public class ScadenzaDetailViewModelInfo
    {
        public int IDScadenza { get; set; }
        public String IDUser { get; set; }
        public int IDBeneficiario { get; set; }
        public string Denominazione { get; set; }
        public DateTime DataScadenza { get; set; }
        public Decimal Importo { get; set; }
        public bool Sollecito { get; set; }
        public int? GiorniRitardo { get; set; }
        public DateTime? DataPagamento { get; set; }
     

        public static ScadenzaDetailViewModelInfo FromEntity(Scadenza scadenza)
        {
            return new ScadenzaDetailViewModelInfo {
                IDScadenza = scadenza.IDScadenza,
                IDBeneficiario = scadenza.IDBeneficiario,
                IDUser = scadenza.IDUser,
                Denominazione = scadenza.Denominazione,
                DataScadenza = scadenza.DataScadenza,
                DataPagamento = scadenza.DataPagamento,
                Importo = scadenza.Importo,
                GiorniRitardo = scadenza.GiorniRitardo,
                Sollecito = scadenza.Sollecito
            };
        }
    }
}