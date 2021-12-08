using System.Collections.Generic;
using System.Security.Authentication;
using System.Threading.Tasks;
using FusionAuth.SAML.Models;
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore;
using ITfoxtec.Identity.Saml2.Schemas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FusionAuth.SAML.Controllers
{
    [AllowAnonymous]
    [Route( "Authentication" )]
    public class AuthenticationController : Controller
    {
        #region Static

        #region - Private

        const string relayStateReturnUrl = "ReturnUrl";

        #endregion

        #endregion

        #region Private

        private readonly Saml2Configuration config;

        #endregion

        #region Public

        public AuthenticationController( IOptions<Saml2Configuration> configAccessor )
        {
            this.config = configAccessor.Value;
        }

        [Route( "AssertionConsumerService" )]
        public async Task<IActionResult> AssertionConsumerService( )
        {
            var binding = new Saml2PostBinding( );
            var saml2AuthnResponse = new Saml2AuthnResponse( this.config );

            binding.ReadSamlResponse( this.Request.ToGenericHttpRequest( ), saml2AuthnResponse );
            if ( saml2AuthnResponse.Status != Saml2StatusCodes.Success )
            {
                throw new AuthenticationException( $"SAML Response status: {saml2AuthnResponse.Status}" );
            }

            binding.Unbind( this.Request.ToGenericHttpRequest( ), saml2AuthnResponse );
            await saml2AuthnResponse.CreateSession( this.HttpContext, claimsTransform: claimsPrincipal => ClaimsTransform.Transform( claimsPrincipal ) );

            var relayStateQuery = binding.GetRelayStateQuery( );
            var returnUrl = relayStateQuery.ContainsKey( relayStateReturnUrl ) ? relayStateQuery[relayStateReturnUrl] : this.Url.Content( "~/" );
            return this.Redirect( returnUrl );
        }

        [Route( "Login" )]
        public IActionResult Login( string returnUrl = null )
        {
            var binding = new Saml2RedirectBinding( );
            binding.SetRelayStateQuery( new Dictionary<string, string> { { relayStateReturnUrl, returnUrl ?? this.Url.Content( "~/" ) } } );

            return binding.Bind( new Saml2AuthnRequest( this.config ) ).ToActionResult( );
        }

        [HttpPost( "Logout" )]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout( )
        {
            if ( !this.User.Identity.IsAuthenticated )
            {
                return this.Redirect( this.Url.Content( "~/" ) );
            }

            var binding = new Saml2PostBinding( );
            var saml2LogoutRequest = await new Saml2LogoutRequest( this.config, this.User ).DeleteSession( this.HttpContext );
            return this.Redirect( "~/" );
        }

        #endregion
    }
}