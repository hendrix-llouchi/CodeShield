using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CodeShield.Models;
using CodeShield.Services;

namespace CodeShield.Controllers
{
    [Authorize]
    public class ScanController : Controller
    {
        private readonly IGitHubService _gitHubService;
        private readonly IOsvService _osvService;

        public ScanController(IGitHubService gitHubService, IOsvService osvService)
        {
            _gitHubService = gitHubService;
            _osvService = osvService;
        }

        [HttpGet]
        [Route("Scan")]
        public IActionResult Index()
        {
            return View(new ScanViewModel());
        }

        [HttpPost]
        [Route("Scan")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ScanViewModel model)
        {
            model.IsSubmitted = true;

            if (string.IsNullOrWhiteSpace(model.RepositoryUrl))
            {
                model.ErrorMessage = "Please enter a valid GitHub repository URL.";
                return View(model);
            }

            var (files, packages, detectedEcosystems, errorMessage) = await _gitHubService.GetRepositoryFilesAsync(model.RepositoryUrl);

            if (errorMessage != null)
            {
                model.ErrorMessage = errorMessage;
            }
            else
            {
                model.Files = files;
                model.Packages = packages;
                model.DetectedEcosystems = detectedEcosystems;

                if (packages != null && packages.Count > 0)
                {
                    var (osvSuccess, osvErrorMessage) = await _osvService.CheckVulnerabilitiesAsync(packages);
                    if (!osvSuccess || osvErrorMessage != null)
                    {
                        model.OsvWarningMessage = osvErrorMessage ?? "Some packages could not be checked due to a temporary issue — try rescanning for complete results.";
                    }
                }
            }

            return View(model);
        }
    }
}
