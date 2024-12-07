using Scadenzario.Models.Entity;

#nullable disable

namespace Scadenzario.Models.Entities
{
    public sealed class Scadenza
    {
        public Scadenza(DateTime dataScadenza, decimal importo, string denominazione)
        {
            Denominazione = denominazione;
            DataScadenza = dataScadenza;
            Importo = importo;
            Sollecito = false;
            GiorniRitardo = null;
            DataPagamento = null;
            Ricevute = new HashSet<Ricevuta>();
        }
        public int IDScadenza { get; set; }
        public String IDUser { get; set; }
        public int IDBeneficiario { get; set; }
        public string Denominazione { get; set; }
        public DateTime DataScadenza { get; set; }
        public Decimal Importo { get; set; }
        public bool Sollecito { get; set; }
        public int? GiorniRitardo { get; set; }
        public DateTime? DataPagamento { get; set; }
        public Beneficiario Beneficiario { get; set; }
        public ICollection<Ricevuta> Ricevute { get; private set; }
    }
    
}
