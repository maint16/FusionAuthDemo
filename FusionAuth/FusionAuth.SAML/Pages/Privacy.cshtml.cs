using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace FusionAuth.SAML.Pages
{
    public class PrivacyModel : PageModel
    {
        #region Private

        private readonly ILogger<PrivacyModel> _logger;

        #endregion

        #region Public

        public PrivacyModel( ILogger<PrivacyModel> logger )
        {
            this._logger = logger;
        }

        public void OnGet( )
        {
        }

        #endregion
    }
}