using Microsoft.Extensions.Configuration;

namespace DockerSecrets.Configuration
{
    /// <summary>
    /// Configuration source for Docker secrets.
    /// </summary>
    public class DockerSecretsConfigurationSource : IConfigurationSource
    {
        /// <summary>
        /// The directory where Docker secrets are mounted.
        /// </summary>
        public string SecretsPath { get; set; } = "/run/secrets";

        /// <summary>
        /// The expected namespace for this application (e.g. "Test", "PaymentWallet", etc.).
        /// Only secrets with this namespace will be loaded.
        /// </summary>
        public string ExpectedNamespace { get; set; } = "";

        /// <summary>
        /// The delimiter separating the namespace from the key in the file name.
        /// For example, if set to "$", a file named "Test$ApplicationSettings__EncryptionKey" is expected.
        /// </summary>
        public string NamespaceDelimiter { get; set; } = ".";

        /// <summary>
        /// The delimiter used within the remainder of the file name to generate configuration keys.
        /// For example, "ApplicationSettings__EncryptionKey" becomes "ApplicationSettings:EncryptionKey".
        /// </summary>
        public string KeyDelimiter { get; set; } = "__";

        /// <summary>
        /// Builds the Docker secrets configuration provider.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new DockerSecretsConfigurationProvider(this);
        }
    }
}