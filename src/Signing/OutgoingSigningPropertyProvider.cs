using System.Threading.Tasks;
using System;
using MassTransit.Initializers;
using MassTransit.Extensions.ArmoredDispatch.Signing;

namespace MassTransit.Extensions.ArmoredDispatch.Signing
{
    internal class OutgoingSigningPropertyProvider : AzureKeyVaultSigningProvider, IPropertyProvider<ISignedMessage, EncodedSignature>
    {
        public OutgoingSigningPropertyProvider(AzureKeyVaultSigningConfiguration configuration) : base(configuration)
        {
        }

        public async Task<EncodedSignature> GetProperty<T>(InitializeContext<T, ISignedMessage> context) where T : class
        {
            // --- OUTGOING ---

            // create signature block to sign with data (note byte[0])
            var signatureBlock = new EncodedSignature()
            {
                Signer = Configuration.DisplayName,
                Timestamp = DateTimeOffset.UtcNow,
                Signature = new byte[0]
            };
            context.Input.Signature = signatureBlock;
            var digest = GetSignableHash(context.Input);

            var result = await GetKeyVaultClient(null).SignAsync(Configuration.SignatureAlgorithm, digest);

            return new EncodedSignature()
            {
                Signer = signatureBlock.Signer,
                Timestamp = signatureBlock.Timestamp,
                Signature = result.Signature,
                SignatureKid = new Uri(result.KeyId)
            };
        }
    }
}
