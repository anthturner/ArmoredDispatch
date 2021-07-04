using Azure.Security.KeyVault.Keys.Cryptography;
using System;

namespace MassTransit.Extensions.ArmoredDispatch.Encryption
{
    public class AzureKeyVaultEncryptionConfiguration : AzureKeyVaultConfiguration
    {
        /// <summary>
        /// Key wrapping algorithm to use
        /// </summary>
        public KeyWrapAlgorithm KeyWrapAlgorithm { get; set; } = KeyWrapAlgorithm.Rsa15;

        /// <summary>
        /// Returns the KeyId of the entity receiving the message (who will decrypt with this key)
        /// </summary>
        public Func<Uri, Uri> GetKeyIdFromDestination { get; set; } = new Func<Uri, Uri>(s => s);
    }
}
