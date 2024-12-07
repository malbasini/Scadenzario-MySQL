using AngleSharp.Text;
using Microsoft.AspNetCore.Mvc;
using Scadenzario.Customizations.ModelBinders;
using Scadenzario.Models.Options;
using Scadenze.Customizations.ModelBinders;
using Scdenzario.Models.Options;

namespace Scadenzario.Models.InputModels.Scadenze;

    [ModelBinder(BinderType = typeof(ScadenzaListInputModelBinder))]
    public class ScadenzaListInputModel
    {
        public ScadenzaListInputModel(string search, int page, string? orderby, bool ascending, int limit, ScadenzeOrderOptions orderOptions)
        {
            if (orderOptions.Allow != null && !orderOptions.Allow.Contains(orderby))
            {
                orderby = orderOptions.By;
                ascending = orderOptions.Ascending;
            }

            Search = search ?? "";
            Page = Math.Max(1, page);
            Limit = Math.Max(1, limit);
            OrderBy = orderby;
            Ascending = ascending;

            Offset = (Page - 1) * Limit;
        }
        public string Search { get; }
        public int Page { get; }
        public string? OrderBy { get; }
        public bool Ascending { get; }
        
        public int Limit { get; }
        public int Offset { get; }
    }