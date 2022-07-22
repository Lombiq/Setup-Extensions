# Lombiq Setup Extensions for Orchard Core

[![Lombiq.SetupExtensions NuGet](https://img.shields.io/nuget/v/Lombiq.SetupExtensions?label=Lombiq.SetupExtensions)](https://www.nuget.org/packages/Lombiq.SetupExtensions/)

## About

Extensions for setting up an Orchard Core application.

Do you want to quickly try out this project and see it in action? Check it out in our [Open-Source Orchard Core Extensions](https://github.com/Lombiq/Open-Source-Orchard-Core-Extensions) full Orchard Core solution and also see our other useful Orchard Core-related open-source projects!

## Features

### Logged in user authentication for API requests

A service configuration that will authenticate every API request to be able to run the setup even for the Default tenant (when the application is configured to do so). This way you'll be able to set up a fresh Orchard app's first (Default) tenant via an API call.

This is achieved by using one of the extension methods in the applications's `Startup` class:

```csharp
    public class Startup
    {
        private readonly IConfiguration _configuration;


        public Startup(IConfiguration configuration) => _configuration = configuration;


        public void ConfigureServices(IServiceCollection services)
        {
            // Use either one of the below options:

            // Set it up via an AddOrchardCms() argument.
            services.AddOrchardCms(builder => builder.AuthorizeApiRequestsIfEnabled(_configuration));

            // Enable it based on your own logic:
            services.AddOrchardCms(builder =>
            {
                if (...)
                {
                    builder.AuthorizeApiRequests(_configuration); 
                }
            });
        }

        // Rest of the class.
    }
```

When starting the application, the `AuthorizeOrchardApiRequests` setting has to be set to `true` either in the launch settings or when starting the application through the `dotnet` CLI. Without this setting, API requests will be authenticated as usual, so it's safe to run/deploy the application with this project as long as the setting above is not set to `true`.

Example: `dotnet Lombiq.AwesomeApp.dll --AuthorizeOrchardApiRequests true`.

You can use [the Reset-OrchardCoreApp script from the Utility Scripts project](https://github.com/Lombiq/Utility-Scripts) to quickly reset and reinstall a local Orchard Core app configured with this.

## Contribution and support

Bug reports, feature requests, comments, questions, code contributions and love letters are warmly welcome. Please do so via GitHub issues and pull requests. Please adhere to our [open-source guidelines](https://lombiq.com/open-source-guidelines) while doing so.

This project is developed by [Lombiq Technologies](https://lombiq.com/). Commercial-grade support is available through Lombiq.
