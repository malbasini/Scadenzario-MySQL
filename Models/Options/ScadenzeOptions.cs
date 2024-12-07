namespace Scdenzario.Models.Options
{
    public class ScadenzeOptions
    {
        public int PerPage { get; set; }
        public ScadenzeOrderOptions Order { get; set; }
    }

    public class ScadenzeOrderOptions
    {
        public string By { get; set; }
        public bool Ascending { get; set; }
        public string[] Allow { get; set; }
    }
}
