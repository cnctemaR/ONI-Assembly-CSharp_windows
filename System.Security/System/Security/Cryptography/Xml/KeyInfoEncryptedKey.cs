using System;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public class KeyInfoEncryptedKey : KeyInfoClause
	{
		public KeyInfoEncryptedKey()
		{
		}

		public KeyInfoEncryptedKey(EncryptedKey ek)
		{
			this.EncryptedKey = ek;
		}

		public EncryptedKey EncryptedKey
		{
			get
			{
				return this.encryptedKey;
			}
			set
			{
				this.encryptedKey = value;
			}
		}

		public override XmlElement GetXml()
		{
			return this.GetXml(new XmlDocument());
		}

		internal XmlElement GetXml(XmlDocument document)
		{
			if (this.encryptedKey != null)
			{
				return this.encryptedKey.GetXml(document);
			}
			return null;
		}

		public override void LoadXml(XmlElement value)
		{
			this.EncryptedKey = new EncryptedKey();
			this.EncryptedKey.LoadXml(value);
		}

		private EncryptedKey encryptedKey;
	}
}
