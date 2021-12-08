using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace FusionAuth.SAML.Pages
{
    [Authorize]
    public class ClaimsModel : PageModel
    {
        #region Private

        private readonly ILogger<ClaimsModel> _logger;

        #endregion

        #region Public

        public ClaimsModel( ILogger<ClaimsModel> logger )
        {
            this._logger = logger;
        }

        public void OnGet( )
        {
        }

        #endregion
    }
}