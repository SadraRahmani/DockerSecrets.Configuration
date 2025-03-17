# DockerSecrets.Configuration

DockerSecrets.Configuration is a lightweight configuration provider that integrates Docker secrets seamlessly into your .NET applications. It leverages the [Microsoft.Extensions.Configuration](https://docs.microsoft.com/en-us/dotnet/core/extensions/configuration) framework to load secrets from a specified directory (by default `/run/secrets`) and transform them into hierarchical configuration keys. This is especially useful when running your application in a Dockerized environment.

[![NuGet](https://img.shields.io/nuget/v/DockerSecrets.Configuration)](https://www.nuget.org/packages/DockerSecrets.Configuration) 

## Table of Contents

- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
  - [ASP.NET Core Web Application](#aspnet-core-web-application)
  - [Generic Host / Console Application](#generic-host--console-application)
- [Configuration Options](#configuration-options)
- [How It Works](#how-it-works)
- [Contributing](#contributing)
- [License](#license)

## Features

- **Docker Secrets Integration**: Load Docker secrets from a specified directory directly into your application configuration.
- **Namespace Filtering**: Optionally filter secrets by a specified namespace to load only relevant secrets.
- **Customizable Delimiters**: Easily define custom delimiters for namespace separation and key conversion.
- **Simple Integration**: One-line extension method to add the secrets provider to your configuration pipeline.
- **Verbose Logging**: Built-in console logging helps trace secret loading for easier debugging and monitoring.

## Installation

Install the package via the .NET CLI:

```bash
dotnet add package DockerSecrets.Configuration
```

Or via the NuGet Package Manager:

```powershell
Install-Package DockerSecrets.Configuration
```

For more details, check out the [NuGet page](https://www.nuget.org/packages/DockerSecrets.Configuration).

## Usage

### ASP.NET Core Web Application

To integrate Docker secrets into an ASP.NET Core web application, modify your `Program.cs` as follows:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add Docker secrets with an optional expected namespace filter.
builder.Configuration.AddDockerSecrets(
    expectedNamespace: "Namespace" // Replace "Namespace" with your desired namespace, or omit for no filtering.
);

var app = builder.Build();
app.Run();
```

### Generic Host / Console Application

For generic host or console applications, add the secrets provider during configuration setup:

```csharp
using Microsoft.Extensions.Hosting;
using DockerSecrets.Configuration;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        // Add Docker secrets with an optional expected namespace filter.
        config.AddDockerSecrets(
            expectedNamespace: "Namespace" // Replace with your desired namespace.
        );
    })
    .Build();

host.Run();
```

## Configuration Options

When using the `AddDockerSecrets` extension method, you can customize the following parameters:

- **secretsPath**:  
  The directory where Docker secrets are mounted.  
  _Default_: `/run/secrets`

- **expectedNamespace**:  
  If provided, only secrets that begin with this namespace (plus the delimiter) will be loaded.  
  _Example_: `"Test"`

- **namespaceDelimiter**:  
  The character used to separate the namespace from the remainder of the file name.  
  _Default_: `"."`  
  _Example_: With an expected namespace of `"Test"` and a delimiter `"."`, a valid secret file name would be `Test.ApplicationSettings__EncryptionKey`.

- **keyDelimiter**:  
  The delimiter within the file name that will be replaced with a colon (`:`) to form hierarchical configuration keys.  
  _Default_: `"__"`  
  _Example_: A file named `ApplicationSettings__EncryptionKey` will be loaded as `ApplicationSettings:EncryptionKey` in the configuration.

## How It Works

1. The `DockerSecretsConfigurationProvider` reads all files from the configured `secretsPath` directory.
2. If an `expectedNamespace` is set, only secrets prefixed with that namespace will be loaded.
3. The namespace (if present) is stripped, and the file name is transformed into a hierarchical configuration key using the `keyDelimiter`.
4. The contents of the file become the value of the configuration key.

For example, assuming the following files exist in `/run/secrets`:

```
/run/secrets/Test.Database__Password
/run/secrets/Test.Api__Key
```

With `expectedNamespace` set to `"Test"` and `keyDelimiter` set to `"__"`, the resulting configuration keys will be:

```json
{
  "Database:Password": "mysecretpassword",
  "Api:Key": "myapikey"
}
```

## Contributing

Contributions are welcome! Feel free to open issues or submit pull requests.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
