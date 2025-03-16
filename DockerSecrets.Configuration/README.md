# DockerSecrets.Configuration

A custom configuration provider for .NET applications that loads Docker secrets with namespace support.

## Features

- **Namespace Support:** Use a custom namespace (with a delimiter) to isolate secrets.  
  For example, a secret file named `Test$ApplicationSettings__EncryptionKey` will load as `ApplicationSettings:EncryptionKey` when the expected namespace is set to `Test`.
- **Custom Delimiters:** Configure both the namespace delimiter and key delimiter.
- **Easy Integration:** Works seamlessly with ASP.NET Core's configuration system.

## Usage

1. **Create a Docker Secret:**

   In your Docker swarm, define a secret with a namespace prefix. For example:
   - `Test$ApplicationSettings__EncryptionKey`

2. **Configure Your Application:**

   Add the provider to your configuration builder. For example:

   ```csharp
   using Microsoft.Extensions.Configuration;
   using DockerSecrets.Configuration;
   
   var builder = new ConfigurationBuilder()
       .SetBasePath(Directory.GetCurrentDirectory())
       .AddJsonFile("appsettings.json", optional: true)
       .AddDockerSecrets(secretsPath: "/run/secrets", expectedNamespace: "Test");
   
   var configuration = builder.Build();
   var encryptionKey = configuration["ApplicationSettings:EncryptionKey"];
