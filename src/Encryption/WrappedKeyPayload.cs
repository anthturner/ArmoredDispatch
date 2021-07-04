using System;

namespace MassTransit.Extensions.ArmoredDispatch.Encryption
{
    internal class WrappedKeyPayload
    {
        // NOTE: Primary security risk is the wholesale replacement of a message
        //       by regenerating a key and salt, wrapping it, and replacing every byte.
        //       That implies access to the vault, however.
        //
        // To avoid this situation, employ a signing mechanism alongside this encryption one.

        public byte[] Key { get; set; }
        public Uri KeyId { get; set; }
    }
}