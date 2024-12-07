using System.Collections.Generic;
using Scadenzario.Models.InputModels.Beneficiari;
using Scadenzario.Models.ViewModels;

namespace Scadenze.Models.ViewModels
{
    public class BeneficiarioListViewModel:IPaginationInfo
    {
        public ListViewModel<BeneficiarioViewModel> Beneficiari {get;set;}
        public BeneficiarioListInputModel Input {get;set;}

         #region Implementazione IPaginationInfo
         
        int IPaginationInfo.CurrentPage => Input.Page;

        int IPaginationInfo.TotalResults => Beneficiari.TotalCount;

        int IPaginationInfo.ResultsPerPage => Input.Limit;

        string IPaginationInfo.Search => Input.Search;

        string IPaginationInfo.OrderBy => Input.OrderBy;

        bool IPaginationInfo.Ascending => Input.Ascending;
        
        #endregion
    }
}