using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SheriffPersonnel.Shared.DTOs;

namespace SheriffPersonnel.Web.Services
{
    public interface IEntraService
    {
        Task<IEnumerable<EntraUserDto>> SearchUsersAsync(string query, CancellationToken cancellationToken = default);
    }
}
