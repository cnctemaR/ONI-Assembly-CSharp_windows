using System;
using System.Collections;
using System.IO;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public class XmlDecryptionTransform : Transform
	{
		public XmlDecryptionTransform()
		{
			base.Algorithm = "http://www.w3.org/2002/07/decrypt#XML";
			this.encryptedXml = new EncryptedXml();
			this.exceptUris = new ArrayList();
		}

		public EncryptedXml EncryptedXml
		{
			get
			{
				return this.encryptedXml;
			}
			set
			{
				this.encryptedXml = value;
			}
		}

		public override Type[] InputTypes
		{
			get
			{
				if (this.inputTypes == null)
				{
					this.inputTypes = new Type[]
					{
						typeof(Stream),
						typeof(XmlDocument)
					};
				}
				return this.inputTypes;
			}
		}

		public override Type[] OutputTypes
		{
			get
			{
				if (this.outputTypes == null)
				{
					this.outputTypes = new Type[] { typeof(XmlDocument) };
				}
				return this.outputTypes;
			}
		}

		public void AddExceptUri(string uri)
		{
			this.exceptUris.Add(uri);
		}

		private void ClearExceptUris()
		{
			this.exceptUris.Clear();
		}

		[MonoTODO("Verify")]
		protected override XmlNodeList GetInnerXml()
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.AppendChild(xmlDocument.CreateElement("DecryptionTransform"));
			foreach (object obj in this.exceptUris)
			{
				XmlElement xmlElement = xmlDocument.CreateElement("Except", "http://www.w3.org/2002/07/decrypt#");
				xmlElement.Attributes.Append(xmlDocument.CreateAttribute("URI", "http://www.w3.org/2002/07/decrypt#"));
				xmlElement.Attributes["URI", "http://www.w3.org/2002/07/decrypt#"].Value = (string)obj;
				xmlDocument.DocumentElement.AppendChild(xmlElement);
			}
			return xmlDocument.GetElementsByTagName("Except", "http://www.w3.org/2002/07/decrypt#");
		}

		[MonoTODO("Verify processing of ExceptURIs")]
		public override object GetOutput()
		{
			XmlDocument xmlDocument;
			if (this.inputObj is Stream)
			{
				xmlDocument = new XmlDocument();
				xmlDocument.PreserveWhitespace = true;
				xmlDocument.XmlResolver = base.GetResolver();
				xmlDocument.Load(new XmlSignatureStreamReader(new StreamReader(this.inputObj as Stream)));
			}
			else
			{
				if (!(this.inputObj is XmlDocument))
				{
					throw new NullReferenceException();
				}
				xmlDocument = this.inputObj as XmlDocument;
			}
			XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("EncryptedData", "http://www.w3.org/2001/04/xmlenc#");
			foreach (object obj in elementsByTagName)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode == xmlDocument.DocumentElement && this.exceptUris.Contains("#xpointer(/)"))
				{
					break;
				}
				foreach (object obj2 in this.exceptUris)
				{
					string text = (string)obj2;
					if (this.IsTargetElement((XmlElement)xmlNode, text.Substring(1)))
					{
						break;
					}
				}
				EncryptedData encryptedData = new EncryptedData();
				encryptedData.LoadXml((XmlElement)xmlNode);
				SymmetricAlgorithm decryptionKey = this.EncryptedXml.GetDecryptionKey(encryptedData, encryptedData.EncryptionMethod.KeyAlgorithm);
				this.EncryptedXml.ReplaceData((XmlElement)xmlNode, this.EncryptedXml.DecryptData(encryptedData, decryptionKey));
			}
			return xmlDocument;
		}

		public override object GetOutput(Type type)
		{
			if (type == typeof(Stream))
			{
				return this.GetOutput();
			}
			throw new ArgumentException("type");
		}

		[MonoTODO("verify")]
		protected virtual bool IsTargetElement(XmlElement inputElement, string idValue)
		{
			return inputElement != null && idValue != null && inputElement.Attributes["id"].Value == idValue;
		}

		[MonoTODO("This doesn't seem to work in .NET")]
		public override void LoadInnerXml(XmlNodeList nodeList)
		{
			if (nodeList == null)
			{
				throw new NullReferenceException();
			}
			this.ClearExceptUris();
			foreach (object obj in nodeList)
			{
				XmlNode xmlNode = (XmlNode)obj;
				XmlElement xmlElement = xmlNode as XmlElement;
				if (xmlElement.NamespaceURI.Equals("http://www.w3.org/2002/07/decrypt#") && xmlElement.LocalName.Equals("Except"))
				{
					string value = xmlElement.Attributes["URI", "http://www.w3.org/2002/07/decrypt#"].Value;
					if (!value.StartsWith("#"))
					{
						throw new CryptographicException("A Uri attribute is required for a CipherReference element.");
					}
					this.AddExceptUri(value);
				}
			}
		}

		public override void LoadInput(object obj)
		{
			this.inputObj = obj;
		}

		private const string NamespaceUri = "http://www.w3.org/2002/07/decrypt#";

		private EncryptedXml encryptedXml;

		private Type[] inputTypes;

		private Type[] outputTypes;

		private object inputObj;

		private ArrayList exceptUris;
	}
}
