using System;
using System.Collections.Specialized;
using System.Xml;

namespace System.Configuration
{
	public sealed class DpapiProtectedConfigurationProvider : ProtectedConfigurationProvider
	{
		[MonoNotSupported("DpapiProtectedConfigurationProvider depends on the Microsoft Data\nProtection API, and is unimplemented in Mono.  For portability's sake,\nit is suggested that you use the RsaProtectedConfigurationProvider.")]
		public override XmlNode Decrypt(XmlNode encryptedNode)
		{
			throw new NotSupportedException("DpapiProtectedConfigurationProvider depends on the Microsoft Data\nProtection API, and is unimplemented in Mono.  For portability's sake,\nit is suggested that you use the RsaProtectedConfigurationProvider.");
		}

		[MonoNotSupported("DpapiProtectedConfigurationProvider depends on the Microsoft Data\nProtection API, and is unimplemented in Mono.  For portability's sake,\nit is suggested that you use the RsaProtectedConfigurationProvider.")]
		public override XmlNode Encrypt(XmlNode node)
		{
			throw new NotSupportedException("DpapiProtectedConfigurationProvider depends on the Microsoft Data\nProtection API, and is unimplemented in Mono.  For portability's sake,\nit is suggested that you use the RsaProtectedConfigurationProvider.");
		}

		[MonoTODO]
		public override void Initialize(string name, NameValueCollection configurationValues)
		{
			base.Initialize(name, configurationValues);
			string text = configurationValues["useMachineProtection"];
			if (text != null && text.ToLowerInvariant() == "true")
			{
				this.useMachineProtection = true;
			}
		}

		public bool UseMachineProtection
		{
			get
			{
				return this.useMachineProtection;
			}
		}

		private bool useMachineProtection;

		private const string NotSupportedReason = "DpapiProtectedConfigurationProvider depends on the Microsoft Data\nProtection API, and is unimplemented in Mono.  For portability's sake,\nit is suggested that you use the RsaProtectedConfigurationProvider.";
	}
}
