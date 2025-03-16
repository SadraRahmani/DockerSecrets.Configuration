using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DockerSecrets.Configuration
{
    public class DockerSecretsConfigurationProvider : ConfigurationProvider
    {
        private readonly DockerSecretsConfigurationSource _source;
        private readonly ILogger<DockerSecretsConfigurationProvider> _logger;

        public DockerSecretsConfigurationProvider(DockerSecretsConfigurationSource source, ILogger<DockerSecretsConfigurationProvider> logger)
        {
            _source = source;
            _logger = logger;
        }

        public override void Load()
        {
            _logger.LogInformation("[DockerSecrets] Starting to load secrets from '{SecretsPath}'...", _source.SecretsPath);
            if (!Directory.Exists(_source.SecretsPath))
            {
                _logger.LogWarning("[DockerSecrets] Secrets path '{SecretsPath}' does not exist.", _source.SecretsPath);
                return;
            }

            var files = Directory.GetFiles(_source.SecretsPath);
            _logger.LogInformation("[DockerSecrets] Found {Count} secret file(s).", files.Length);

            foreach (var file in files)
            {
                _logger.LogInformation("[DockerSecrets] Processing file: {File}", file);
                try
                {
                    // Get the file name (e.g., "Test$ApplicationSettings__EncryptionKey")
                    var fileName = Path.GetFileName(file);
                    _logger.LogInformation("[DockerSecrets] Original file name: {FileName}", fileName);

                    // If an expected namespace is provided, verify and remove it.
                    if (!string.IsNullOrEmpty(_source.ExpectedNamespace))
                    {
                        var nsPrefix = _source.ExpectedNamespace + _source.NamespaceDelimiter;
                        _logger.LogInformation("[DockerSecrets] Expected namespace prefix: {NsPrefix}", nsPrefix);
                        if (!fileName.StartsWith(nsPrefix))
                        {
                            _logger.LogInformation("[DockerSecrets] Skipping file '{FileName}' as it does not match the expected namespace.", fileName);
                            continue;
                        }
                        // Remove the namespace prefix.
                        fileName = fileName.Substring(nsPrefix.Length);
                        _logger.LogInformation("[DockerSecrets] File name after removing namespace: {FileName}", fileName);
                    }

                    // Convert the remaining file name to a configuration key.
                    // Example: "ApplicationSettings__EncryptionKey" becomes "ApplicationSettings:EncryptionKey"
                    var key = fileName.Replace(_source.KeyDelimiter, ":");
                    _logger.LogInformation("[DockerSecrets] Generated configuration key: {Key}", key);

                    // Read the secret file's content.
                    var value = File.ReadAllText(file).Trim();

                    // Add or override the configuration key.
                    Data[key] = value;
                    _logger.LogInformation("[DockerSecrets] Loaded secret for key: {Key}", key);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[DockerSecrets] Error processing file '{File}': {Message}", file, ex.Message);
                }
            }
        }
    }
}
