using Microsoft.AspNetCore.Mvc;
using Scadenzario.Customizations.ModelBinders;
using Scadenzario.Models.Options;

namespace Scadenzario.Models.InputModels.Beneficiari;

    [ModelBinder(BinderType = typeof(BeneficiarioListInputModelBinder))]
    public class BeneficiarioListInputModel
    {
        public BeneficiarioListInputModel(string search, int page, string? orderby, bool ascending, int limit, BeneficiariOrderOptions orderOptions)
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