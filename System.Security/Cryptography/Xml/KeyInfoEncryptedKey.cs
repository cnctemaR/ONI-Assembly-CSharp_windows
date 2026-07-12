using System;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public class KeyInfoEncryptedKey : KeyInfoClause
	{
		public KeyInfoEncryptedKey()
		{
		}

		public KeyInfoEncryptedKey(EncryptedKey encryptedKey)
		{
			this._encryptedKey = encryptedKey;
		}

		public EncryptedKey EncryptedKey
		{
			get
			{
				return this._encryptedKey;
			}
			set
			{
				this._encryptedKey = value;
			}
		}

		public override XmlElement GetXml()
		{
			if (this._encryptedKey == null)
			{
				throw new CryptographicException("Malformed element {0}.", "KeyInfoEncryptedKey");
			}
			return this._encryptedKey.GetXml();
		}

		internal override XmlElement GetXml(XmlDocument xmlDocument)
		{
			if (this._encryptedKey == null)
			{
				throw new CryptographicException("Malformed element {0}.", "KeyInfoEncryptedKey");
			}
			return this._encryptedKey.GetXml(xmlDocument);
		}

		public override void LoadXml(XmlElement value)
		{
			this._encryptedKey = new EncryptedKey();
			this._encryptedKey.LoadXml(value);
		}

		private EncryptedKey _encryptedKey;
	}
}
