using System.Threading.Tasks;
using System;
using MassTransit.Initializers;

namespace MassTransit.Extensions.ArmoredDispatch.Signing
{
    internal class IncomingSigningPropertyProvider : AzureKeyVaultSigningProvider, IPropertyProvider<ISignedMessage, SignatureValidation>
    {
        public IncomingSigningPropertyProvider(AzureKeyVaultSigningConfiguration configuration) : base(configuration)
        {
        }

        public async Task<SignatureValidation> GetProperty<T>(InitializeContext<T, ISignedMessage> context) where T : class
        {
            // --- INCOMING ---

            if (context.Input.Signature == null)
                throw new Exception("No signature applied.");
            if (!Configuration.KeyIdRegExCompiled.IsMatch(context.Input.Signature.SignatureKid.ToString()))
                throw new Exception("Signature key does not match configured regular expression");

            // get signature
            var duplicatedSignature = new byte[context.Input.Signature.Signature.Length];
            Array.Copy(context.Input.Signature.Signature, duplicatedSignature, duplicatedSignature.Length);

            // remove signature bytes from input
            context.Input.Signature.Signature = new byte[0];

            // get digest and check signature
            var digest = GetSignableHash(context.Input);

            var result = await GetKeyVaultClient(context.Input.Signature.SignatureKid).VerifyAsync(
                Configuration.SignatureAlgorithm,
                digest,
                duplicatedSignature);

            // put signature bytes back
            context.Input.Signature.Signature = duplicatedSignature;

            return new SignatureValidation()
            {
                Result = result.IsValid,
                Notes = "OK"
            };
        }
    }
}
