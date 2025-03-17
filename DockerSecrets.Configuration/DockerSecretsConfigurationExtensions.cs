using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace DockerSecrets.Configuration
{
    /// <summary>
    /// Provides extension methods for adding the Docker secrets configuration provider to an <see cref="IConfigurationBuilder"/>.
    /// </summary>
    public static class DockerSecretsConfigurationExtensions
    {
        /// <summary>
        /// Adds the Docker secrets configuration provider to the configuration builder with support for filtering by multiple namespaces.
        /// </summary>
        /// <param name="builder">The configuration builder to which the Docker secrets provider is added.</param>
        /// <param name="secretsPath">The path where Docker secrets are mounted (default: "/run/secrets").</param>
        /// <param name="expectedNamespaces">
        /// A collection of namespaces to filter secrets. Only secrets whose file names start with one of these namespaces (followed by the <paramref name="namespaceDelimiter"/>)
        /// will be loaded. If <c>null</c> or an empty collection is provided, secrets that do not have a namespace will be included.
        /// </param>
        /// <param name="namespaceDelimiter">
        /// The delimiter separating the namespace from the key in the secret file name (default: "."). 
        /// For example, with a delimiter of ".", a secret named "Test.ApplicationSettings__EncryptionKey" has a namespace of "Test".
        /// </param>
        /// <param name="keyDelimiter">
        /// The delimiter used in the remainder of the file name to generate configuration keys (default: "__"). 
        /// For instance, "ApplicationSettings__EncryptionKey" is converted into "ApplicationSettings:EncryptionKey".
        /// </param>
        /// <param name="includeEmptyNamespace">
        /// A flag that determines whether to load secrets that do not have a namespace.
        /// This flag is <c>false</c> by default unless no expected namespaces are provided, in which case it is overridden to <c>true</c>.
        /// </param>
        /// <returns>The configuration builder with the Docker secrets provider added.</returns>
        public static IConfigurationBuilder AddDockerSecrets(this IConfigurationBuilder builder,
            string secretsPath = "/run/secrets",
            IEnumerable<string> expectedNamespaces = null,
            string namespaceDelimiter = ".",
            string keyDelimiter = "__",
            bool includeEmptyNamespace = false)
        {
            // If no namespaces are provided, automatically include secrets with no namespace.
            if (expectedNamespaces == null || !expectedNamespaces.Any())
            {
                includeEmptyNamespace = true;
            }

            var source = new DockerSecretsConfigurationSource
            {
                SecretsPath = secretsPath,
                ExpectedNamespaces = expectedNamespaces != null ? new List<string>(expectedNamespaces) : new List<string>(),
                NamespaceDelimiter = namespaceDelimiter,
                KeyDelimiter = keyDelimiter,
                IncludeEmptyNamespace = includeEmptyNamespace
            };

            builder.Add(source);
            return builder;
        }
    }


}
