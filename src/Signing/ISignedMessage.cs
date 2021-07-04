using System;

namespace MassTransit.Extensions.ArmoredDispatch.Signing
{
    /// <summary>
    /// Marks a property as not included when generating a message's content hash for signing
    /// </summary>
    public class NotSignedAttribute : Attribute { }

    /// <summary>
    /// An interface implementing the properties necessary to sign a given message Type
    /// </summary>
    public interface ISignedMessage
    {
        /// <summary>
        /// Signature embedded by ArmoredDispatch signing process
        /// </summary>
        public EncodedSignature Signature { get; set; }

        /// <summary>
        /// Signature Validation embedded by ArmoredDispatch validation process
        /// </summary>
        public SignatureValidation SignatureValidation { get; set; }
    }

    /// <summary>
    /// Description of if signature is valid or not
    /// </summary>
    public class SignatureValidation
    {
        public bool Result { get; set; }
        public string Notes { get; set; }
    }

    /// <summary>
    /// Signature of message, based on all public properties not marked with [NotSigned] attribute
    /// </summary>
    public class EncodedSignature
    {
        public byte[] Signature { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string Signer { get; set; }
        public Uri SignatureKid { get; internal set; }
    }
}
