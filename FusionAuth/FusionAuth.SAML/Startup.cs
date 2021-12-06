using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore.Configuration;
using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace FusionAuth.SAML
{
    public class Startup
    {
        public Startup( IConfiguration configuration )
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices( IServiceCollection services )
        {

            services.AddControllers( );

            services.Configure<Saml2Configuration>( Configuration.GetSection( "Saml2" ) );

            services.Configure<Saml2Configuration>( saml2Configuration =>
                {
                    saml2Configuration.AllowedAudienceUris.Add( saml2Configuration.Issuer );

                    var entityDescriptor = new EntityDescriptor( );
                    entityDescriptor.ReadIdPSsoDescriptorFromUrl( new Uri( Configuration["Saml2:IdPMetadata"] ) );
                    if ( entityDescriptor.IdPSsoDescriptor != null )
                    {
                        saml2Configuration.SingleSignOnDestination = entityDescriptor.IdPSsoDescriptor.SingleSignOnServices.First( ).Location;
                        saml2Configuration.SignatureValidationCertificates.AddRange( entityDescriptor.IdPSsoDescriptor.SigningCertificates );
                    }
                    else
                    {
                        throw new Exception( "IdPSsoDescriptor not loaded from metadata." );
                    }
                } );

            services.AddSaml2( );

            services.AddSwaggerGen( c =>
             {
                 c.SwaggerDoc( "v1", new OpenApiInfo { Title = "FusionAuth.SAML", Version = "v1" } );
             } );

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure( IApplicationBuilder app, IWebHostEnvironment env )
        {
            if ( env.IsDevelopment( ) )
            {
                app.UseDeveloperExceptionPage( );
                app.UseSwagger( );
                app.UseSwaggerUI( c => c.SwaggerEndpoint( "/swagger/v1/swagger.json", "FusionAuth.SAML v1" ) );
            }

            app.UseHttpsRedirection( );

            app.UseRouting( );

            app.UseAuthorization( );

            app.UseEndpoints( endpoints =>
             {
                 endpoints.MapControllers( );
             } );
        }
    }
}