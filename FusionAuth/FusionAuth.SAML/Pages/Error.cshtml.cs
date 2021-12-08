using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace FusionAuth.SAML.Pages
{
    [ResponseCache( Duration = 0, Location = ResponseCacheLocation.None, NoStore = true )]
    [IgnoreAntiforgeryToken]
    public class ErrorModel : PageModel
    {
        #region Private

        private readonly ILogger<ErrorModel> _logger;

        #endregion

        #region Public

        public ErrorModel( ILogger<ErrorModel> logger )
        {
            this._logger = logger;
        }

        public void OnGet( )
        {
            this.RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier;
        }
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty( this.RequestId );

        #endregion
    }
}