![ArmoredDispatch Logo](docs/logo_transparent.png)
**ArmoredDispatch** is an extension for [MassTransit](https://www.masstransit.com) which uses [Azure Key Vault](https://azure.microsoft.com/en-us/services/key-vault/) to enhance MassTransit's message security using keys stored in a hosted Key Vault.

# Using the Extension
An example of adding this extension to MassTransit when used in the dependency injection mode follows:
```csharp
services.AddMassTransit(x =>
{
    x.UsingInMemory((context, cfg) =>
    {
        // To use the signing system, ensure the type given here
        // implements ISignedMessage, and include the following:
        cfg.UseAzureKeyVaultSigning<MyMessage>(
            new AzureKeyVaultSigningConfiguration()
            {
                // DisplayName represents the sender's display name,
                // but can have any content in practice.
                DisplayName = "Roberta Jones",

                // Credential is an instance of TokenCredential. When
                // running inside Azure or on a system with the Azure
                // CLI is installed, you can use DefaultAzureCredential
                // as seen below.
                Credential = new DefaultAzureCredential(),

                // This is the URI to the key of the sender who is signing
                // outgoing objects. This can optionally include a specific
                // key version, if desired. The value is found in the Key
                // Vault section in the Azure Portal. For example:
                DefaultKeyId = new Uri("https://contoso.azure.net/keys/mySigningKey")
            });

        // To use the message encryption system, include the following:
        cfg.UseAzureKeyVaultEncryption(
            new AzureKeyVaultEncryptionConfiguration()
            {
                // See above for the use of Credential
                Credential = credential,

                // In order to encrypt for a specific destination,
                // use the following function to provide the Key ID URI
                // given that destination's endpoint.
                GetKeyIdFromDestination = (destination) => defaultKid
            });
    });
});
```

# Concepts
## Message Sender Identity Verification
In order to guarantee that a message sent to a Consumer is actually from a given Producer, with ArmoredDispatch, a Message can have its integrity signed by a given Key Vault key.

To do this, a Message must implement the `ISignedMessage` interface. This interface adds two properties to all messages implementing it:

### Signature
The `Signature` property is an `EncodedSignature` object, which contains the `Signature` of the message hash, the `Timestamp` when that signature was applied, the `SignatureKid` (Key ID URI) used to perform the signing, and a string which should contain the display name or human-readable identifier of the `Signer` (but which can, in practice, contain anything).

As a part of the signing process, these fields *are* included when computing the message hash.

### SignatureValidation
The `SignatureValidation` property is a `SignatureValidation` object, which contains a boolean `Result` which is populated by the property provider when the message is incoming. If this property is null, the message has not been validated.

## Dynamic Message Encryption
In normal practice with MassTransit, a fixed encryption key is used to cryptographically obscure the message content. However, in a cryptographically optimal situation, the message is signed to only be readable by a single, known, user or service account. This ensures only one identity has the ability to decrypt the message.

---
###### Parts of logo created by [monkik](https://www.flaticon.com/authors/monkik) from [FlatIcon](https://www.flaticon.com/)