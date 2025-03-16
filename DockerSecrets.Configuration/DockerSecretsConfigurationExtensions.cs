using Microsoft.Extensions.Configuration;

namespace DockerSecrets.Configuration
{
    public static class DockerSecretsConfigurationExtensions
    {
        /// <summary>
        /// Adds the Docker secrets configuration provider to the configuration builder.
        /// </summary>
        /// <param name="builder">The configuration builder.</param>
        /// <param name="secretsPath">Path where Docker secrets are mounted (default: /run/secrets).</param>
        /// <param name="expectedNamespace">The namespace to filter secrets (e.g. "Test").</param>
        /// <param name="namespaceDelimiter">Delimiter separating the namespace from the key (default: "$").</param>
        /// <param name="keyDelimiter">Delimiter for the configuration key conversion (default: "__").</param>
        /// <returns>The configuration builder.</returns>
        public static IConfigurationBuilder AddDockerSecrets(this IConfigurationBuilder builder,
            string secretsPath = "/run/secrets",
            string expectedNamespace = "",
            string namespaceDelimiter = "$",
            string keyDelimiter = "__")
        {
            var source = new DockerSecretsConfigurationSource
            {
                SecretsPath = secretsPath,
                ExpectedNamespace = expectedNamespace,
                NamespaceDelimiter = namespaceDelimiter,
                KeyDelimiter = keyDelimiter
            };

            builder.Add(source);
            return builder;
        }
    }
}