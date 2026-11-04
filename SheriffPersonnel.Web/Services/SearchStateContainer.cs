using System.Collections.Generic;
using SheriffPersonnel.Shared.DTOs;

namespace SheriffPersonnel.Web.Services
{
    public class SearchStateContainer
    {
        public EmployeeSearchRequestDto SearchRequest { get; set; } = new();
        public IEnumerable<EmployeeSearchResultDto>? SearchResults { get; set; }
        public bool IsEmployeeFlagChecked { get; set; }
        public bool HasSearched { get; set; }

        public void Clear()
        {
            SearchRequest = new EmployeeSearchRequestDto();
            SearchResults = null;
            IsEmployeeFlagChecked = false;
            HasSearched = false;
        }
    }
}

