using System;
using System.Linq;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using Azure.Security.KeyVault.Keys.Cryptography;

namespace MassTransit.Extensions.ArmoredDispatch.Signing
{
    internal class AzureKeyVaultSigningProvider
    {
        protected AzureKeyVaultSigningConfiguration Configuration { get; }
        protected AzureKeyVaultSigningProvider(
            AzureKeyVaultSigningConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected CryptographyClient GetKeyVaultClient(Uri keyId = null)
        {
            if (keyId == null) keyId = Configuration.DefaultKeyId;
            return new CryptographyClient(keyId, Configuration.Credential);
        }

        protected static byte[] GetSignableHash<T>(T signable)
            where T : ISignedMessage
        {
            string messageSignedContent = string.Empty;
            foreach (var prop in signable.GetType().GetProperties()
                .Where(p => p.Name != nameof(ISignedMessage.Signature))
                .Where(p => p.Name != nameof(ISignedMessage.SignatureValidation))
                .Where(p => p.GetCustomAttributes(typeof(NotSignedAttribute), true).Length == 0))
                messageSignedContent += JsonConvert.SerializeObject(prop.GetValue(signable));
            messageSignedContent += signable.Signature.Signer;
            messageSignedContent += signable.Signature.Timestamp;
            return SHA256.Create().ComputeHash(
                Encoding.UTF8.GetBytes(messageSignedContent));
        }
    }
}
