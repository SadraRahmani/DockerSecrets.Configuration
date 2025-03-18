# DockerSecrets.Configuration

[![NuGet](https://img.shields.io/nuget/v/DockerSecrets.Configuration.svg)](https://www.nuget.org/packages/DockerSecrets.Configuration)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

## Overview

**DockerSecrets.Configuration** is a lightweight NuGet package that seamlessly integrates Docker secrets into your .NET configuration system. This provider reads secrets from a mounted Docker secrets directory (defaulting to `/run/secrets`), processes secret file names based on configurable namespaces and custom delimiters, and loads them as key-value pairs into your application configuration.

The package supports filtering secrets by multiple namespaces and can optionally include secrets without a namespace, giving you complete control over how secrets are imported into your application.

## Table of Contents

- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
  - [Basic Example](#basic-example)
  - [Using Multiple Namespaces](#using-multiple-namespaces)
- [API Documentation](#api-documentation)
  - [AddDockerSecrets Extension Method](#adddockersecrets-extension-method)
  - [DockerSecretsConfigurationSource](#dockersecretsconfigurationsource)
  - [DockerSecretsConfigurationProvider](#dockersecretsconfigurationprovider)
- [Contributing](#contributing)
- [License](#license)
- [Additional Resources](#additional-resources)

## Features

- **Docker Secrets Integration:** Load secrets directly from a mounted directory into your configuration.
- **Namespace Filtering:** Filter secrets using one or multiple namespaces.
- **Customizable Delimiters:** Configure namespace and key delimiters for flexible file naming conventions.
- **Empty Namespace Inclusion:** Optionally include secrets that do not have a namespace.

## Installation

Install the package via the .NET CLI:

```bash
dotnet add package DockerSecrets.Configuration
```

Or via the Package Manager Console:

```powershell
Install-Package DockerSecrets.Configuration
```

For more details, visit the [NuGet package page](https://www.nuget.org/packages/DockerSecrets.Configuration).

## Usage

### Basic Example

Add the Docker secrets configuration provider to your configuration builder. For instance, in your `Program.cs` or `Startup.cs`:

```csharp
using DockerSecrets.Configuration;
using Microsoft.Extensions.Configuration;
using System.IO;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddDockerSecrets(); // Uses default parameters: secretsPath = "/run/secrets"

// Build the configuration
var configuration = builder.Build();

// Access a secret value (assuming a file named "ApplicationSettings__EncryptionKey")
var encryptionKey = configuration["ApplicationSettings:EncryptionKey"];
```

### Using Multiple Namespaces

You can filter secrets by specific namespaces and control whether to include secrets without a namespace:

```csharp
using DockerSecrets.Configuration;
using Microsoft.Extensions.Configuration;
using System.IO;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddDockerSecrets(
        secretsPath: "/run/secrets",
        expectedNamespaces: new[] { "Test", "Production" },
        namespaceDelimiter: ".",
        keyDelimiter: "__",
        includeEmptyNamespace: false);

var configuration = builder.Build();
```

In this example, only secrets with file names beginning with `Test.` or `Production.` will be loaded, and secrets with no namespace will be excluded.

## API Documentation

### AddDockerSecrets Extension Method

```csharp
public static IConfigurationBuilder AddDockerSecrets(
    this IConfigurationBuilder builder,
    string secretsPath = "/run/secrets",
    IEnumerable<string> expectedNamespaces = null,
    string namespaceDelimiter = ".",
    string keyDelimiter = "__",
    bool includeEmptyNamespace = false)
```

**Parameters:**

- **builder:** The configuration builder to which the provider is added.
- **secretsPath:** The directory path where Docker secrets are mounted.
- **expectedNamespaces:** A collection of namespaces to filter secrets. If `null` or empty, secrets without a namespace are automatically included.
- **namespaceDelimiter:** The delimiter that separates the namespace from the key in a secret file name.
- **keyDelimiter:** The delimiter used to transform the secret file name into a configuration key.
- **includeEmptyNamespace:** Indicates whether to include secrets without a namespace. Defaults to `false` unless no namespaces are provided.

### DockerSecretsConfigurationSource

This class represents the configuration source for Docker secrets.

**Properties:**

- **SecretsPath:** The directory where Docker secrets are mounted (default: `/run/secrets`).
- **ExpectedNamespaces:** A collection of namespaces used to filter which secrets to load.
- **NamespaceDelimiter:** The delimiter that separates the namespace from the key in the secret file name.
- **KeyDelimiter:** The delimiter used in the secret file name to construct the configuration key.
- **IncludeEmptyNamespace:** Indicates whether secrets without a namespace should be included.

**Method:**

- **Build(IConfigurationBuilder builder):** Builds the `DockerSecretsConfigurationProvider` instance.

### DockerSecretsConfigurationProvider

This provider reads Docker secrets from the specified directory, converts secret file names into configuration keys using the provided delimiters, and loads the secrets into the configuration system.

**Key Responsibilities:**

- Reads all files from the configured secrets directory.
- Parses file names to extract the namespace and key parts.
- Replaces custom key delimiters with the standard configuration key delimiter (`:`).
- Loads secrets based on the filtering rules defined in the configuration source.

## Contributing

Contributions are welcome! To contribute:

1. **Fork** the repository.
2. **Create a new branch** for your feature or bugfix.
3. **Write tests** for your changes.
4. **Submit a pull request** with detailed information about your changes.

If you have any issues, suggestions, or improvements, please open an issue or submit a pull request on GitHub.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Additional Resources

- [NuGet Package](https://www.nuget.org/packages/DockerSecrets.Configuration)

---

Happy coding! Enjoy secure and manageable configuration with **DockerSecrets.Configuration**.
