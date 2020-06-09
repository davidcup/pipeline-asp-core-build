# ASP.NET Core 2.2 RESTful Web API Demo

This sample multi-tier application demonstrates common RESTful skeleton of code based on the most popular Microsoft technologies at this moment. ASP.NET Core is a perfect choice and - it Rocks!

Apps developed using this framework can run on Windows, Linux, and MacOS. It ensures a better reach for your app and faster speed to market.

With ASP.NET Core, dependency injection is built-in. No more reliance on third-party frameworks such as AutoFacor and Ninject.

It's worth a mention, that building and maintaining test app infrastructure is painful and nobody wants to spend too much time on. ASP.NET Core offers a package in <a href="https://www.nuget.org/packages/Microsoft.AspNetCore.Mvc.Testing" target="_blank">Microsoft.AspNetCore.Mvc.Testing</a> that allows our projects to quickly realize many of the benefits end to end tests without the typical costs associated with setting them up. **Includes also Sqlite Health Check End-Point.**

This application demonstrate the following functionalities:
- Usage of ASP.NET Core API Controllers,
- JWT (Bearer Token) Based Authentication,
- [API versioning](https://github.com/Microsoft/aspnet-api-versioning)
- exposing [Data Transfer Objects (DTO)](https://docs.microsoft.com/en-us/aspnet/web-api/overview/data/using-web-api-with-entity-framework/part-5) to Client,
- [Swagger API auto documenting tool](https://swagger.io/)
- Usage of [SQLite](https://www.sqlite.org/index.html) as database,
- Logging with [Serilog](https://serilog.net/) sink to file,
- Asynchronous generic repository Pattern for Entity types,
- [In-Memory integration tests](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-2.2) with Entity Framework Core,
- Unit tests ([xUnit](https://xunit.github.io/) & [Moq](https://github.com/Moq/moq4/wiki/Quickstart))

## Prerequisites
- [Visual Studio](https://www.visualstudio.com/vs/community) 2017 15.9 or greater
- [.NET Core SDK 2.2.102](https://dotnet.microsoft.com/download/dotnet-core/2.2)

## Tags & Technologies
- ASP.NET MVC Core 2.2
- Entity Framework Core 2.2
- ASP.NET Identity Core 2.2

Based on this <a href="https://matjazbravc.github.io/aspnetcoreintegrationtests/" target="_blank">blog post</a>.

Enjoy!

## Licence

Licenced under [MIT](http://opensource.org/licenses/mit-license.php).
Contact me on [LinkedIn](https://si.linkedin.com/in/matjazbravc).
