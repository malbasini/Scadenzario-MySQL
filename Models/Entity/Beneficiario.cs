#nullable disable

using Scadenzario.Models.Entity;

namespace Scadenzario.Models.Entities
{
    public sealed class Beneficiario
    {
        public Beneficiario(string denominazione, string descrizione)
        {
            ChangeDenominazione(denominazione);
            ChangeDescrizione(descrizione);
            Scadenze = new HashSet<Scadenza>();
            Email = string.Empty;
            Telefono = string.Empty;
            SitoWeb = string.Empty;

        }
        public int IdBeneficiario { get; set; }
        public string Denominazione { get; private set; }
        public string Descrizione { get; private set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string SitoWeb { get; set; }
        public String IdUser { get; set; }
        public ICollection<Scadenza> Scadenze { get; private set; }
        
        public ApplicationUser ApplicationUser { get; set; }
        public void ChangeDescrizione(string newDescription)
        {
            if (String.IsNullOrEmpty(newDescription))
                throw new ArgumentException("La descrizione deve essere valorizzata.");
            Descrizione = newDescription;
        }
        public void ChangeDenominazione(string newBeneficiario)
        {
            if (String.IsNullOrEmpty(newBeneficiario))
                throw new ArgumentException("Il beneficiario deve essere valorizzato.");
            Denominazione = newBeneficiario;
        }
    }
}
