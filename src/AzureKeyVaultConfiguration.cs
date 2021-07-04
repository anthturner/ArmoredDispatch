using Azure.Core;
using System.Text.RegularExpressions;

namespace MassTransit.Extensions.ArmoredDispatch
{
    public abstract class AzureKeyVaultConfiguration
    {
        public TokenCredential Credential { get; set; }

        public string KeyIdRegEx { get; set; } = "https\\:\\/\\/.*?.vault.azure.net\\/";

        private Regex _cachedKeyIdRegEx;
        internal Regex KeyIdRegExCompiled
        {
            get
            {
                if (_cachedKeyIdRegEx == null)
                    _cachedKeyIdRegEx = new Regex(KeyIdRegEx, RegexOptions.Compiled);
                return _cachedKeyIdRegEx;
            }
        }
    }
}
