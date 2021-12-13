# SAML Integration

To demonstrate how back-end api can integrate with SAML in Fusion auth
## Table of Contents
* [Prerequisites](#prerequisites)
* [Setup FusionAuth](#setup-fusion-auth)
* [Setup project](#setup-project)
* [Try it out](#usage)

## Prerequisites  
* Install .NET 5
* Visual studio
* Basic concept of SAML

## Setup FusionAuth
In this guide we will make all the steps from beginning (FusionAuth UI needs to be available ). If you already have your Tenant, Application, Master Key, User configured, you can skip these steps.
- Add a Tenant  
  Our Tenant contains the basic information like this:    
   ![Add tenant](https://i.ibb.co/BLLmbVV/add-tenant.png)   
- Add a Master key   
  ![Add key master](https://i.ibb.co/NSkw9TC/add-key-master.png)
- Add an Application and configure SAML using the Master Key you added above.
  ![Add application](https://i.ibb.co/gzk1DK9/add-application.png)  
  And in SAML tab:  
  ![saml config](https://i.ibb.co/fY5rKpn/saml-config.png)       

  ![saml config](https://i.ibb.co/wyJ9pWk/saml-config-logout.png)
- Add an User belong to the Tenant and have the Application in Registrations.  
  ![Add user](https://i.ibb.co/LgcGW4F/add-user.png)
  After ```save``` we need to edit application and add the application in registrations list:  
  ![User registrations](hhttps://i.ibb.co/jVhPR1T/user-add-app-registrations.png)
## Setup project
Create a webapp project called ```FusionAuth.SAML```    
Add the required packages in ```NuGet Package Manager```:

 ```
     ITfoxtec.Identity.Saml2 --version 4.0.8  
     ITfoxtec.Identity.Saml2.MvcCore --version 4.0.8
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
The ```IdPMetadta``` value taken from Application ```View``` mode:  
![Metadata](https://i.ibb.co/YbxcNVz/view-application.png)  
The application will now use SAML for authentication.
## Try it out
- Run the project the Home page will be displayed then click on ```Login``` button:  
![Home page](https://i.ibb.co/tCYYr3H/home-page.png)  
- Click ```Login``` and login with ```username``` and ```password``` we created:  
  ![Login](https://i.ibb.co/BKXxs1m/login.png)  
- Once redirected back to your application, you will see that your nave shows that you are logged in.  
  ![Login success](https://i.ibb.co/pQzSXwd/login-success.png)  
 We open claims page to see user Claims:  
  ![Claims](https://i.ibb.co/WtzK1nR/claims.png)  
 
