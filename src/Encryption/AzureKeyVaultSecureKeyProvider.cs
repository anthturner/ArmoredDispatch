using Azure.Security.KeyVault.Keys.Cryptography;
using GreenPipes;
using MassTransit.Serialization;
using System;
using System.Security.Cryptography;
using System.Text.Json;

namespace MassTransit.Extensions.ArmoredDispatch.Encryption
{
    internal class AzureKeyVaultSecureKeyProvider : ISecureKeyProvider
    {
        private readonly AzureKeyVaultEncryptionConfiguration _config;
        internal AzureKeyVaultSecureKeyProvider(AzureKeyVaultEncryptionConfiguration config)
        {
            _config = config;
        }

        public byte[] GetKey(ReceiveContext receiveContext)
        {
            var kvKey = receiveContext.TransportHeaders.Get<string>("kv-key");
            if (string.IsNullOrEmpty(kvKey))
                throw new System.Exception("No wrapped key present.");

            var payload = JsonSerializer.Deserialize<WrappedKeyPayload>(kvKey);
            if (payload == null)
                throw new System.Exception("No wrapped key present.");

            if (!_config.KeyIdRegExCompiled.IsMatch(payload.KeyId.ToString()))
                throw new System.Exception("Key ID does not match required prefix!");

            // unwrap embedded key
            var cryptoClient = new CryptographyClient(payload.KeyId, _config.Credential);
            var unwrapped = cryptoClient.UnwrapKey(_config.KeyWrapAlgorithm, payload.Key);

            // return actual (unwrapped) key
            return unwrapped.Key;
        }

        public byte[] GetKey(SendContext sendContext)
        {
            var keyId = _config.GetKeyIdFromDestination(sendContext.DestinationAddress);

            // Use default AES to generate key (MassTransit uses AES-256)
            byte[] generatedKey;
            using (var aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.GenerateKey();
                generatedKey = aes.Key;
            }

            // wrap a copy of the key for embedding
            var cryptoClient = new CryptographyClient(keyId, _config.Credential);
            var wrapped = cryptoClient.WrapKey(_config.KeyWrapAlgorithm, generatedKey);

            var serialized = JsonSerializer.Serialize(new WrappedKeyPayload()
            {
                Key = wrapped.EncryptedKey,
                KeyId = new Uri(wrapped.KeyId)
            });
            sendContext.Headers.Set("kv-key", serialized);            

            // return actual key
            return generatedKey;
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("encryption");
            scope.Add("kv-key", "key");
        }
    }
}
