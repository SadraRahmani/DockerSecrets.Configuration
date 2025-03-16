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
            Console.WriteLine($"[DockerSecrets] Starting to load secrets from '{_source.SecretsPath}'...");
            if (!Directory.Exists(_source.SecretsPath))
            {
                Console.WriteLine($"[DockerSecrets] Warning: Secrets path '{_source.SecretsPath}' does not exist.");
                return;
            }

            var files = Directory.GetFiles(_source.SecretsPath);
            Console.WriteLine($"[DockerSecrets] Found {files.Length} secret file(s).");

            foreach (var file in files)
            {
                Console.WriteLine($"[DockerSecrets] Processing file: {file}");
                try
                {
                    // Get the file name (e.g., "Test.ApplicationSettings__EncryptionKey")
                    var fileName = Path.GetFileName(file);
                    Console.WriteLine($"[DockerSecrets] Original file name: {fileName}");

                    // If an expected namespace is provided, verify and remove it.
                    if (!string.IsNullOrEmpty(_source.ExpectedNamespace))
                    {
                        var nsPrefix = _source.ExpectedNamespace + _source.NamespaceDelimiter;
                        Console.WriteLine($"[DockerSecrets] Expected namespace prefix: {nsPrefix}");
                        if (!fileName.StartsWith(nsPrefix))
                        {
                            Console.WriteLine($"[DockerSecrets] Skipping file '{fileName}' as it does not match the expected namespace.");
                            continue;
                        }
                        // Remove the namespace prefix.
                        fileName = fileName.Substring(nsPrefix.Length);
                        Console.WriteLine($"[DockerSecrets] File name after removing namespace: {fileName}");
                    }

                    // Convert the remaining file name to a configuration key.
                    // Example: "ApplicationSettings__EncryptionKey" becomes "ApplicationSettings:EncryptionKey"
                    var key = fileName.Replace(_source.KeyDelimiter, ":");
                    Console.WriteLine($"[DockerSecrets] Generated configuration key: {key}");

                    // Read the secret file's content.
                    var value = File.ReadAllText(file).Trim();

                    // Add or override the configuration key.
                    Data[key] = value;
                    Console.WriteLine($"[DockerSecrets] Loaded secret for key: {key}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[DockerSecrets] Error processing file '{file}': {ex.Message}");
                }
            }
        }
    }
}
