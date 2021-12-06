﻿using System.Collections.Generic;
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
    [ApiController]
    [AllowAnonymous]
    [Route( "[controller]" )]
    public class AuthenticationController : Controller
    {
        const string relayStateReturnUrl = "ReturnUrl";
        private readonly Saml2Configuration config;

        public AuthenticationController( IOptions<Saml2Configuration> configAccessor )
        {
            config = configAccessor.Value;
        }

        [HttpGet( "Login" )]
        public IActionResult Login( string returnUrl = null )
        {
            var binding = new Saml2RedirectBinding( );
            binding.SetRelayStateQuery( new Dictionary<string, string> { { relayStateReturnUrl, returnUrl ?? Url.Content( "~/" ) } } );
            return binding.Bind( new Saml2AuthnRequest( config ) ).ToActionResult(  );
        }

        [HttpGet( "AssertionConsumerService" )]
        public async Task<IActionResult> AssertionConsumerService( )
        {
            var binding = new Saml2PostBinding( );
            var saml2AuthnResponse = new Saml2AuthnResponse( config );

            binding.ReadSamlResponse( Request.ToGenericHttpRequest( ), saml2AuthnResponse );
            if ( saml2AuthnResponse.Status != Saml2StatusCodes.Success )
            {
                throw new AuthenticationException( $"SAML Response status: {saml2AuthnResponse.Status}" );
            }
            binding.Unbind( Request.ToGenericHttpRequest( ), saml2AuthnResponse );
            await saml2AuthnResponse.CreateSession( HttpContext, claimsTransform: ( claimsPrincipal ) => ClaimsTransform.Transform( claimsPrincipal ) );

            var relayStateQuery = binding.GetRelayStateQuery( );
            var returnUrl = relayStateQuery.ContainsKey( relayStateReturnUrl ) ? relayStateQuery[relayStateReturnUrl] : Url.Content( "~/" );
            return Redirect( returnUrl );
        }

        [HttpPost( "Logout" )]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout( )
        {
            if ( !User.Identity.IsAuthenticated )
            {
                return Redirect( Url.Content( "~/" ) );
            }

            var binding = new Saml2PostBinding( );
            var saml2LogoutRequest = await new Saml2LogoutRequest( config, User ).DeleteSession( HttpContext );
            return Redirect( "~/" );
        }
    }
}