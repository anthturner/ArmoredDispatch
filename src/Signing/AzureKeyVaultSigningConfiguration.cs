using Azure.Security.KeyVault.Keys.Cryptography;
using System;

namespace MassTransit.Extensions.ArmoredDispatch.Signing
{
    public class AzureKeyVaultSigningConfiguration : AzureKeyVaultConfiguration
    {
        public string DisplayName { get; set; }
        public Uri DefaultKeyId { get; set; }
        public SignatureAlgorithm SignatureAlgorithm { get; set; } = SignatureAlgorithm.RS256;
    }
}
