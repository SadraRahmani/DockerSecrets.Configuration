using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace DockerSecrets.Configuration
{
    /// <summary>
    /// A configuration provider that reads Docker secrets from a mounted directory,
    /// processes the file names according to namespace and key delimiters,
    /// and loads them as key/value pairs into the configuration system.
    /// </summary>
    public class DockerSecretsConfigurationProvider : ConfigurationProvider
    {
        private readonly DockerSecretsConfigurationSource _source;

        /// <summary>
        /// Initializes a new instance of the <see cref="DockerSecretsConfigurationProvider"/> class with the specified source.
        /// </summary>
        /// <param name="source">The <see cref="DockerSecretsConfigurationSource"/> containing the settings for the provider.</param>
        /// <exception cref="ArgumentNullException">Thrown if the provided <paramref name="source"/> is <c>null</c>.</exception>
        public DockerSecretsConfigurationProvider(DockerSecretsConfigurationSource source)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
        }

        /// <summary>
        /// Loads Docker secrets from the configured secrets path and converts them into configuration key/value pairs.
        /// </summary>
        /// <remarks>
        /// This method reads each file in the secrets directory. It determines the namespace (if any) by splitting the file name using
        /// the <see cref="DockerSecretsConfigurationSource.NamespaceDelimiter"/>. The remainder of the file name is then converted into a configuration key
        /// using the <see cref="DockerSecretsConfigurationSource.KeyDelimiter"/> (which is replaced by the standard configuration key delimiter ':').
        /// Secrets are loaded if they either match one of the expected namespaces or, if they have no namespace, if <see cref="DockerSecretsConfigurationSource.IncludeEmptyNamespace"/> is <c>true</c>.
        /// </remarks>
        public override void Load()
        {
            var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (!Directory.Exists(_source.SecretsPath))
            {
                // Secrets path does not exist. No secrets will be loaded.
                Data = data;
                return;
            }

            var secretFiles = Directory.GetFiles(_source.SecretsPath);

            foreach (var file in secretFiles)
            {
                // Retrieve the file name, which encodes the secret's namespace and key.
                var fileName = Path.GetFileName(file);

                string secretNamespace = string.Empty;
                string keyPart = fileName;

                // Check if the file name contains the namespace delimiter.
                if (fileName.Contains(_source.NamespaceDelimiter))
                {
                    var parts = fileName.Split(new[] { _source.NamespaceDelimiter }, 2, StringSplitOptions.None);
                    secretNamespace = parts[0];
                    keyPart = parts[1];
                }

                // Decide whether to load this secret based on the namespace.
                bool shouldLoad = false;

                if (string.IsNullOrEmpty(secretNamespace))
                {
                    // Load if empty namespace is allowed.
                    shouldLoad = _source.IncludeEmptyNamespace;
                }
                else if (_source.ExpectedNamespaces.Any(ns =>
                             string.Equals(ns, secretNamespace, StringComparison.OrdinalIgnoreCase)))
                {
                    shouldLoad = true;
                }

                if (!shouldLoad)
                {
                    continue;
                }

                // Convert the key part by replacing the key delimiter with the configuration system's key delimiter.
                var configKey = keyPart.Replace(_source.KeyDelimiter, ConfigurationPath.KeyDelimiter);

                // Read the file content.
                string value = File.ReadAllText(file);

                data[configKey] = value;
            }

            Data = data;
        }
    }
}
