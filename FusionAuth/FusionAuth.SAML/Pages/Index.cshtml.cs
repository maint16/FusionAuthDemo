using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace FusionAuth.SAML.Pages
{
    public class IndexModel : PageModel
    {
        #region Private

        private readonly ILogger<IndexModel> _logger;

        #endregion

        #region Public

        public IndexModel( ILogger<IndexModel> logger )
        {
            this._logger = logger;
        }

        public void OnGet( )
        {
        }

        #endregion
    }
}