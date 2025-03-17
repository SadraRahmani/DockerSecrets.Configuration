using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace DockerSecrets.Configuration
{
    /// <summary>
    /// Represents the configuration source for Docker secrets.
    /// </summary>
    public class DockerSecretsConfigurationSource : IConfigurationSource
    {
        /// <summary>
        /// Gets or sets the directory path where Docker secrets are mounted.
        /// </summary>
        public string SecretsPath { get; set; } = "/run/secrets";

        /// <summary>
        /// Gets or sets a collection of namespaces to filter which secrets should be loaded.
        /// Only secrets whose file names begin with one of these namespaces (separated by the <see cref="NamespaceDelimiter"/>) are loaded.
        /// </summary>
        public IList<string> ExpectedNamespaces { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the delimiter that separates the namespace from the key in a secret file name.
        /// For example, with a delimiter of ".", a secret named "Test.ApplicationSettings__EncryptionKey" uses "Test" as the namespace.
        /// </summary>
        public string NamespaceDelimiter { get; set; } = ".";

        /// <summary>
        /// Gets or sets the delimiter used within the remainder of the secret file name to construct the configuration key.
        /// For example, "ApplicationSettings__EncryptionKey" is transformed into "ApplicationSettings:EncryptionKey".
        /// </summary>
        public string KeyDelimiter { get; set; } = "__";

        /// <summary>
        /// Gets or sets a value indicating whether secrets without a namespace should be included.
        /// Defaults to <c>false</c>, unless no expected namespaces are provided (then it is set to <c>true</c>).
        /// </summary>
        public bool IncludeEmptyNamespace { get; set; } = false;

        /// <summary>
        /// Builds the <see cref="DockerSecretsConfigurationProvider"/> based on this source.
        /// </summary>
        /// <param name="builder">The configuration builder.</param>
        /// <returns>An instance of <see cref="DockerSecretsConfigurationProvider"/>.</returns>
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new DockerSecretsConfigurationProvider(this);
        }
    }
}
