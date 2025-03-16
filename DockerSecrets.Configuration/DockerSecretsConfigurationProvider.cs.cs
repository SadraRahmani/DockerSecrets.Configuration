using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace DockerSecrets.Configuration
{
    public class DockerSecretsConfigurationProvider : ConfigurationProvider
    {
        private readonly DockerSecretsConfigurationSource _source;

        public DockerSecretsConfigurationProvider(DockerSecretsConfigurationSource source)
        {
            _source = source;
        }

        public override void Load()
        {
            if (!Directory.Exists(_source.SecretsPath))
            {
                return;
            }

            foreach (var file in Directory.GetFiles(_source.SecretsPath))
            {
                try
                {
                    // Get the file name (e.g., "Test$ApplicationSettings__EncryptionKey")
                    var fileName = Path.GetFileName(file);

                    // If an expected namespace is provided, verify and remove it.
                    if (!string.IsNullOrEmpty(_source.ExpectedNamespace))
                    {
                        var nsPrefix = _source.ExpectedNamespace + _source.NamespaceDelimiter;
                        if (!fileName.StartsWith(nsPrefix))
                        {
                            // Skip files that don’t match the expected namespace.
                            continue;
                        }
                        // Remove the namespace prefix.
                        fileName = fileName.Substring(nsPrefix.Length);
                    }

                    // Convert the remaining file name to a configuration key.
                    // Example: "ApplicationSettings__EncryptionKey" becomes "ApplicationSettings:EncryptionKey"
                    var key = fileName.Replace(_source.KeyDelimiter, ":");

                    // Read the secret file's content.
                    var value = File.ReadAllText(file).Trim();

                    // Add or override the configuration key.
                    Data[key] = value;
                }
                catch (Exception)
                {
                    // Optionally log or handle the error as needed.
                }
            }
        }
    }
}
