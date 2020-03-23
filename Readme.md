# Setup Extensions



## Project Description

Extensions for setting up an Orchard Core application.


## Includes

### Logged in user authentication for API requests

A service configuration that will authenticate every API request to be able to run the setup even for the Default tenant (when the application is configured to do so).

This is achieved by calling the `AddOrchardCmsWithAuthorizedApiRequests` extension method on the service collection when setting up the application (startup project => Startup.cs => ConfigureServices method).

When starting the application, the `AuthorizeOrchardApiRequests` setting has to be set to `true` either in the launch settings or when starting the application through the `dotnet` CLI.
Without this setting, API requests will be authenticated as usual, so it's safe to run/deploy the application with this project as long as the setting above is not set to `true`.

Example: `dotnet Lombiq.AwesomeApp.dll --AuthorizeOrchardApiRequests true`


## Contributing

Bug reports, feature requests, comments and code contributions are warmly welcome, **please do so via GitHub**.

This project is developed by [Lombiq Technologies Ltd](https://lombiq.com/). Commercial-grade support is available through Lombiq.