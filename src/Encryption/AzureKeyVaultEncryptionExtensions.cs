namespace MassTransit.Extensions.ArmoredDispatch.Encryption
{
    public static partial class AzureKeyVaultEncryptionExtensions
    {
        /// <summary>
        /// Use encryption mechanisms via Azure Key Vault on send
        /// </summary>
        /// <param name="configurator">Bus Factory configurator</param>
        /// <param name="config">Key Vault Configuration</param>
        public static void UseAzureKeyVaultEncryption(this IBusFactoryConfigurator configurator,
            AzureKeyVaultEncryptionConfiguration config)
        {
            configurator.UseEncryption(new AzureKeyVaultSecureKeyProvider(config));
        }
    }
}
