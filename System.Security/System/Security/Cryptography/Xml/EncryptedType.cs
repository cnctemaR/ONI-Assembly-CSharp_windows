using System;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public abstract class EncryptedType
	{
		protected EncryptedType()
		{
			this.cipherData = new CipherData();
			this.encryptionProperties = new EncryptionPropertyCollection();
			this.keyInfo = new KeyInfo();
		}

		public virtual CipherData CipherData
		{
			get
			{
				return this.cipherData;
			}
			set
			{
				this.cipherData = value;
			}
		}

		public virtual string Encoding
		{
			get
			{
				return this.encoding;
			}
			set
			{
				this.encoding = value;
			}
		}

		public virtual EncryptionMethod EncryptionMethod
		{
			get
			{
				return this.encryptionMethod;
			}
			set
			{
				this.encryptionMethod = value;
			}
		}

		public virtual EncryptionPropertyCollection EncryptionProperties
		{
			get
			{
				return this.encryptionProperties;
			}
		}

		public virtual string Id
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		public KeyInfo KeyInfo
		{
			get
			{
				return this.keyInfo;
			}
			set
			{
				this.keyInfo = value;
			}
		}

		public virtual string MimeType
		{
			get
			{
				return this.mimeType;
			}
			set
			{
				this.mimeType = value;
			}
		}

		public virtual string Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		public void AddProperty(EncryptionProperty ep)
		{
			this.EncryptionProperties.Add(ep);
		}

		public abstract XmlElement GetXml();

		public abstract void LoadXml(XmlElement value);

		private CipherData cipherData;

		private string encoding;

		private EncryptionMethod encryptionMethod;

		private EncryptionPropertyCollection encryptionProperties;

		private string id;

		private KeyInfo keyInfo;

		private string mimeType;

		private string type;
	}
}
