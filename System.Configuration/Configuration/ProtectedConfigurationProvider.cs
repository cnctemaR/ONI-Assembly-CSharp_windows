using System;
using System.Configuration.Provider;
using System.Xml;

namespace System.Configuration
{
	public abstract class ProtectedConfigurationProvider : ProviderBase
	{
		public abstract XmlNode Decrypt(XmlNode encrypted_node);

		public abstract XmlNode Encrypt(XmlNode node);
	}
}
