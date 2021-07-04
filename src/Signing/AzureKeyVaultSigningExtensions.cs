namespace MassTransit.Extensions.ArmoredDispatch.Signing
{
    public static partial class AzureKeyVaultSigningExtensions
    {
        /// <summary>
        /// Use signing mechanisms for Azure Key Vault on send
        /// </summary>
        /// <param name="configurator">Bus Factory configurator</param>
        /// <param name="config">Key Vault Configuration</param>
        public static void UseAzureKeyVaultSigning<TMessage>(this IBusFactoryConfigurator configurator,
            AzureKeyVaultSigningConfiguration config)
            where TMessage : class, ISignedMessage
        {
            // outgoing for publish and send
            configurator.ConfigurePublish(ppc => ppc.UseTransform<TMessage>(t => PerformOutgoingSignatureTransform(t, config)));
            configurator.ConfigureSend(ppc => ppc.UseTransform<TMessage>(t => PerformOutgoingSignatureTransform(t, config)));

            // incoming
            configurator.UseTransform<TMessage>(t => PerformIncomingSignatureTransform(t, config));
        }

        private static void PerformOutgoingSignatureTransform<TMessage>(ITransformConfigurator<TMessage> transformConfig,
            AzureKeyVaultSigningConfiguration config)
            where TMessage : class, ISignedMessage
        {
            transformConfig.Replace = true;
            transformConfig.Set<EncodedSignature>(
                typeof(ISignedMessage).GetProperty(nameof(ISignedMessage.Signature)),
                new OutgoingSigningPropertyProvider(config));
            // null validation result
            transformConfig.Set<SignatureValidation>(s => s.SignatureValidation, null as SignatureValidation);
        }

        private static void PerformIncomingSignatureTransform<TMessage>(ITransformConfigurator<TMessage> transformConfig,
            AzureKeyVaultSigningConfiguration config)
            where TMessage : class, ISignedMessage
        {
            transformConfig.Replace = true;
            transformConfig.Set<SignatureValidation>(
                typeof(ISignedMessage).GetProperty(nameof(ISignedMessage.SignatureValidation)),
                new IncomingSigningPropertyProvider(config));
        }
    }
}
