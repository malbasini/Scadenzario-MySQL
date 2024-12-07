namespace Scadenzario.Models.Options
{
    public class BeneficiariOptions
    {
        public int PerPage { get; set; }
        public BeneficiariOrderOptions Order { get; set; }
    }

    public class BeneficiariOrderOptions
    {
        public string By { get; set; }
        public bool Ascending { get; set; }
        public string[] Allow { get; set; }
    }
}