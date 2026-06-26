using System.Collections.Generic;
using System.Threading.Tasks;
using CodeShield.Models;

namespace CodeShield.Services
{
    public interface IOsvService
    {
        Task<(bool Success, string? ErrorMessage)> CheckVulnerabilitiesAsync(List<DependencyPackage> packages);
    }
}
