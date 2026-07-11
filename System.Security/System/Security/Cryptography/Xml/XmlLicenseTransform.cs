using System;
using System.IO;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public class XmlLicenseTransform : Transform
	{
		public XmlLicenseTransform()
		{
			base.Algorithm = "urn:mpeg:mpeg21:2003:01-REL-R-NS:licenseTransform";
		}

		public override Type[] InputTypes
		{
			get
			{
				return this._inputTypes;
			}
		}

		public override Type[] OutputTypes
		{
			get
			{
				return this._outputTypes;
			}
		}

		public IRelDecryptor Decryptor
		{
			get
			{
				return this._relDecryptor;
			}
			set
			{
				this._relDecryptor = value;
			}
		}

		private void DecryptEncryptedGrants(XmlNodeList encryptedGrantList, IRelDecryptor decryptor)
		{
			int i = 0;
			int count = encryptedGrantList.Count;
			while (i < count)
			{
				XmlElement xmlElement = encryptedGrantList[i].SelectSingleNode("//r:encryptedGrant/enc:EncryptionMethod", this._namespaceManager) as XmlElement;
				XmlElement xmlElement2 = encryptedGrantList[i].SelectSingleNode("//r:encryptedGrant/dsig:KeyInfo", this._namespaceManager) as XmlElement;
				XmlElement xmlElement3 = encryptedGrantList[i].SelectSingleNode("//r:encryptedGrant/enc:CipherData", this._namespaceManager) as XmlElement;
				if (xmlElement != null && xmlElement2 != null && xmlElement3 != null)
				{
					EncryptionMethod encryptionMethod = new EncryptionMethod();
					KeyInfo keyInfo = new KeyInfo();
					CipherData cipherData = new CipherData();
					encryptionMethod.LoadXml(xmlElement);
					keyInfo.LoadXml(xmlElement2);
					cipherData.LoadXml(xmlElement3);
					MemoryStream memoryStream = null;
					Stream stream = null;
					StreamReader streamReader = null;
					try
					{
						memoryStream = new MemoryStream(cipherData.CipherValue);
						stream = this._relDecryptor.Decrypt(encryptionMethod, keyInfo, memoryStream);
						if (stream == null || stream.Length == 0L)
						{
							throw new CryptographicException("Unable to decrypt grant content.");
						}
						streamReader = new StreamReader(stream);
						string text = streamReader.ReadToEnd();
						encryptedGrantList[i].ParentNode.InnerXml = text;
					}
					finally
					{
						if (memoryStream != null)
						{
							memoryStream.Close();
						}
						if (stream != null)
						{
							stream.Close();
						}
						if (streamReader != null)
						{
							streamReader.Close();
						}
					}
				}
				i++;
			}
		}

		protected override XmlNodeList GetInnerXml()
		{
			return null;
		}

		public override object GetOutput()
		{
			return this._license;
		}

		public override object GetOutput(Type type)
		{
			if (type != typeof(XmlDocument) && !type.IsSubclassOf(typeof(XmlDocument)))
			{
				throw new ArgumentException("The input type was invalid for this transform.", "type");
			}
			return this.GetOutput();
		}

		public override void LoadInnerXml(XmlNodeList nodeList)
		{
		}

		public override void LoadInput(object obj)
		{
			if (base.Context == null)
			{
				throw new CryptographicException("Null Context property encountered.");
			}
			this._license = new XmlDocument();
			this._license.PreserveWhitespace = true;
			this._namespaceManager = new XmlNamespaceManager(this._license.NameTable);
			this._namespaceManager.AddNamespace("dsig", "http://www.w3.org/2000/09/xmldsig#");
			this._namespaceManager.AddNamespace("enc", "http://www.w3.org/2001/04/xmlenc#");
			this._namespaceManager.AddNamespace("r", "urn:mpeg:mpeg21:2003:01-REL-R-NS");
			XmlElement xmlElement = base.Context.SelectSingleNode("ancestor-or-self::r:issuer[1]", this._namespaceManager) as XmlElement;
			if (xmlElement == null)
			{
				throw new CryptographicException("Issuer node is required.");
			}
			XmlNode xmlNode = xmlElement.SelectSingleNode("descendant-or-self::dsig:Signature[1]", this._namespaceManager) as XmlElement;
			if (xmlNode != null)
			{
				xmlNode.ParentNode.RemoveChild(xmlNode);
			}
			XmlElement xmlElement2 = xmlElement.SelectSingleNode("ancestor-or-self::r:license[1]", this._namespaceManager) as XmlElement;
			if (xmlElement2 == null)
			{
				throw new CryptographicException("License node is required.");
			}
			XmlNodeList xmlNodeList = xmlElement2.SelectNodes("descendant-or-self::r:license[1]/r:issuer", this._namespaceManager);
			int i = 0;
			int count = xmlNodeList.Count;
			while (i < count)
			{
				if (xmlNodeList[i] != xmlElement && xmlNodeList[i].LocalName == "issuer" && xmlNodeList[i].NamespaceURI == "urn:mpeg:mpeg21:2003:01-REL-R-NS")
				{
					xmlNodeList[i].ParentNode.RemoveChild(xmlNodeList[i]);
				}
				i++;
			}
			XmlNodeList xmlNodeList2 = xmlElement2.SelectNodes("/r:license/r:grant/r:encryptedGrant", this._namespaceManager);
			if (xmlNodeList2.Count > 0)
			{
				if (this._relDecryptor == null)
				{
					throw new CryptographicException("IRelDecryptor is required.");
				}
				this.DecryptEncryptedGrants(xmlNodeList2, this._relDecryptor);
			}
			this._license.InnerXml = xmlElement2.OuterXml;
		}

		private Type[] _inputTypes = new Type[] { typeof(XmlDocument) };

		private Type[] _outputTypes = new Type[] { typeof(XmlDocument) };

		private XmlNamespaceManager _namespaceManager;

		private XmlDocument _license;

		private IRelDecryptor _relDecryptor;

		private const string ElementIssuer = "issuer";

		private const string NamespaceUriCore = "urn:mpeg:mpeg21:2003:01-REL-R-NS";
	}
}
