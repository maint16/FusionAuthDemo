# SAML Integration

To demonstrate how back-end api can integrate with SAML in Fusion auth
## Table of Contents
* [Setup FusionAuth](#setup-fusion-auth)
* [Setup project](#setup-project)
* [Try it out](#usage)


## Setup FusionAuth
In this guide we will make all the steps from beginning (FusionAuth UI needs to be available ). If you already have your Tenant, Application, Master Key, User configured, you can skip these steps.
- Add a Tenant  
  Our Tenant contains the basic information like this:  
   !(/Images/add_tenant.png)
- Add a Master key   
  !(/Images/add_key_master.png)
- Add an Application and configure SAML using the Master Key you added above.
  !(/Images/add_application.png)  
  And in SAML tab:  
  !(/Images/saml_config.png)  
  !(/images/saml_config_logout.png)
- Add an User belong to the Tenant and have the Application in Registrations.  
  !(/Images/add_user.png)
  After ```save``` we need to edit application and add the application in registrations list:  
  !(/Images/user_add_app_registrations.png)
## Setup project
  Add the required packages by running the following commands:  
  
 ```
     dotnet add package ITfoxtec.Identity.Saml2 --version 4.0.8  
     dotnet add package ITfoxtec.Identity.Saml2.MvcCore --version 4.0.8
```
In ```Startup.cs``` file, start by adding SAML config in ```ConfigureServices()```:  
```
public void ConfigureServices(IServiceCollection services)
{
    services.Configure<Saml2Configuration>(Configuration.GetSection("Saml2"));

    services.Configure<Saml2Configuration>(saml2Configuration =>
    {
        saml2Configuration.AllowedAudienceUris.Add(saml2Configuration.Issuer);

        var entityDescriptor = new EntityDescriptor();
        entityDescriptor.ReadIdPSsoDescriptorFromUrl(new Uri(Configuration["Saml2:IdPMetadata"]));
        if (entityDescriptor.IdPSsoDescriptor != null)
        {
            saml2Configuration.SingleSignOnDestination = entityDescriptor.IdPSsoDescriptor.SingleSignOnServices.First().Location;
            saml2Configuration.SignatureValidationCertificates.AddRange(entityDescriptor.IdPSsoDescriptor.SigningCertificates);
        }
        else
        {
            throw new Exception("IdPSsoDescriptor not loaded from metadata.");
        }
    });

    services.AddSaml2();  
}
```  
And in ```Configure``` add the following after ```app.UseRoting```: 
```
app.UseSaml2();
```  
Lastly, add the configuation settings to ```appsettings.json```:  
```
"Saml2": {
        "IdPMetadata": "http://localhost:9011/samlv2/metadata/d3b296eb-93fb-4307-b55a-6eec7c0ea96a",
        "Issuer": "nstep.com.au",
        "SignatureAlgorithm": "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256",
        "CertificateValidationMode": "ChainTrust",
        "RevocationMode": "NoCheck"
    }
```
The application will now use SAML for authentication.
## Try it out
- Run the project Home page then click on ```Login``` button:  
!(/Images/home_page.png)  
- Click ```Login``` and login with ```username``` and ```password``` we created:  
  !(/Images/login.png)  
- Once redirected back to your application, you will see that your nave shows that you are logged in.  
  !(/Images/login_success.png) 

