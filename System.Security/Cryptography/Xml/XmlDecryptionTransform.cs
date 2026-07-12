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
		}

		private ArrayList ExceptUris
		{
			get
			{
				if (this._arrayListUri == null)
				{
					this._arrayListUri = new ArrayList();
				}
				return this._arrayListUri;
			}
		}

		protected virtual bool IsTargetElement(XmlElement inputElement, string idValue)
		{
			return inputElement != null && (inputElement.GetAttribute("Id") == idValue || inputElement.GetAttribute("id") == idValue || inputElement.GetAttribute("ID") == idValue);
		}

		public EncryptedXml EncryptedXml
		{
			get
			{
				if (this._exml != null)
				{
					return this._exml;
				}
				Reference reference = base.Reference;
				SignedXml signedXml = ((reference == null) ? base.SignedXml : reference.SignedXml);
				if (signedXml == null || signedXml.EncryptedXml == null)
				{
					this._exml = new EncryptedXml(this._containingDocument);
				}
				else
				{
					this._exml = signedXml.EncryptedXml;
				}
				return this._exml;
			}
			set
			{
				this._exml = value;
			}
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

		public void AddExceptUri(string uri)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			this.ExceptUris.Add(uri);
		}

		public override void LoadInnerXml(XmlNodeList nodeList)
		{
			if (nodeList == null)
			{
				throw new CryptographicException("Unknown transform has been encountered.");
			}
			this.ExceptUris.Clear();
			foreach (object obj in nodeList)
			{
				XmlElement xmlElement = ((XmlNode)obj) as XmlElement;
				if (xmlElement != null)
				{
					if (!(xmlElement.LocalName == "Except") || !(xmlElement.NamespaceURI == "http://www.w3.org/2002/07/decrypt#"))
					{
						throw new CryptographicException("Unknown transform has been encountered.");
					}
					string attribute = Utils.GetAttribute(xmlElement, "URI", "http://www.w3.org/2002/07/decrypt#");
					if (attribute == null || attribute.Length == 0 || attribute[0] != '#')
					{
						throw new CryptographicException("A Uri attribute is required for a CipherReference element.");
					}
					if (!Utils.VerifyAttributes(xmlElement, "URI"))
					{
						throw new CryptographicException("Unknown transform has been encountered.");
					}
					string text = Utils.ExtractIdFromLocalUri(attribute);
					this.ExceptUris.Add(text);
				}
			}
		}

		protected override XmlNodeList GetInnerXml()
		{
			if (this.ExceptUris.Count == 0)
			{
				return null;
			}
			XmlDocument xmlDocument = new XmlDocument();
			XmlElement xmlElement = xmlDocument.CreateElement("Transform", "http://www.w3.org/2000/09/xmldsig#");
			if (!string.IsNullOrEmpty(base.Algorithm))
			{
				xmlElement.SetAttribute("Algorithm", base.Algorithm);
			}
			foreach (object obj in this.ExceptUris)
			{
				string text = (string)obj;
				XmlElement xmlElement2 = xmlDocument.CreateElement("Except", "http://www.w3.org/2002/07/decrypt#");
				xmlElement2.SetAttribute("URI", text);
				xmlElement.AppendChild(xmlElement2);
			}
			return xmlElement.ChildNodes;
		}

		public override void LoadInput(object obj)
		{
			if (obj is Stream)
			{
				this.LoadStreamInput((Stream)obj);
				return;
			}
			if (obj is XmlDocument)
			{
				this.LoadXmlDocumentInput((XmlDocument)obj);
			}
		}

		private void LoadStreamInput(Stream stream)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.PreserveWhitespace = true;
			XmlResolver xmlResolver = (base.ResolverSet ? this._xmlResolver : new XmlSecureResolver(new XmlUrlResolver(), base.BaseURI));
			XmlReader xmlReader = Utils.PreProcessStreamInput(stream, xmlResolver, base.BaseURI);
			xmlDocument.Load(xmlReader);
			this._containingDocument = xmlDocument;
			this._nsm = new XmlNamespaceManager(this._containingDocument.NameTable);
			this._nsm.AddNamespace("enc", "http://www.w3.org/2001/04/xmlenc#");
			this._encryptedDataList = xmlDocument.SelectNodes("//enc:EncryptedData", this._nsm);
		}

		private void LoadXmlDocumentInput(XmlDocument document)
		{
			if (document == null)
			{
				throw new ArgumentNullException("document");
			}
			this._containingDocument = document;
			this._nsm = new XmlNamespaceManager(document.NameTable);
			this._nsm.AddNamespace("enc", "http://www.w3.org/2001/04/xmlenc#");
			this._encryptedDataList = document.SelectNodes("//enc:EncryptedData", this._nsm);
		}

		private void ReplaceEncryptedData(XmlElement encryptedDataElement, byte[] decrypted)
		{
			XmlNode parentNode = encryptedDataElement.ParentNode;
			if (parentNode.NodeType == XmlNodeType.Document)
			{
				parentNode.InnerXml = this.EncryptedXml.Encoding.GetString(decrypted);
				return;
			}
			this.EncryptedXml.ReplaceData(encryptedDataElement, decrypted);
		}

		private bool ProcessEncryptedDataItem(XmlElement encryptedDataElement)
		{
			if (this.ExceptUris.Count > 0)
			{
				for (int i = 0; i < this.ExceptUris.Count; i++)
				{
					if (this.IsTargetElement(encryptedDataElement, (string)this.ExceptUris[i]))
					{
						return false;
					}
				}
			}
			EncryptedData encryptedData = new EncryptedData();
			encryptedData.LoadXml(encryptedDataElement);
			SymmetricAlgorithm decryptionKey = this.EncryptedXml.GetDecryptionKey(encryptedData, null);
			if (decryptionKey == null)
			{
				throw new CryptographicException("Unable to retrieve the decryption key.");
			}
			byte[] array = this.EncryptedXml.DecryptData(encryptedData, decryptionKey);
			this.ReplaceEncryptedData(encryptedDataElement, array);
			return true;
		}

		private void ProcessElementRecursively(XmlNodeList encryptedDatas)
		{
			if (encryptedDatas == null || encryptedDatas.Count == 0)
			{
				return;
			}
			Queue queue = new Queue();
			foreach (object obj in encryptedDatas)
			{
				XmlNode xmlNode = (XmlNode)obj;
				queue.Enqueue(xmlNode);
			}
			for (XmlNode xmlNode2 = queue.Dequeue() as XmlNode; xmlNode2 != null; xmlNode2 = queue.Dequeue() as XmlNode)
			{
				XmlElement xmlElement = xmlNode2 as XmlElement;
				if (xmlElement != null && xmlElement.LocalName == "EncryptedData" && xmlElement.NamespaceURI == "http://www.w3.org/2001/04/xmlenc#")
				{
					XmlNode nextSibling = xmlElement.NextSibling;
					XmlNode parentNode = xmlElement.ParentNode;
					if (this.ProcessEncryptedDataItem(xmlElement))
					{
						XmlNode xmlNode3 = parentNode.FirstChild;
						while (xmlNode3 != null && xmlNode3.NextSibling != nextSibling)
						{
							xmlNode3 = xmlNode3.NextSibling;
						}
						if (xmlNode3 != null)
						{
							XmlNodeList xmlNodeList = xmlNode3.SelectNodes("//enc:EncryptedData", this._nsm);
							if (xmlNodeList.Count > 0)
							{
								foreach (object obj2 in xmlNodeList)
								{
									XmlNode xmlNode4 = (XmlNode)obj2;
									queue.Enqueue(xmlNode4);
								}
							}
						}
					}
				}
				if (queue.Count == 0)
				{
					break;
				}
			}
		}

		public override object GetOutput()
		{
			if (this._encryptedDataList != null)
			{
				this.ProcessElementRecursively(this._encryptedDataList);
			}
			Utils.AddNamespaces(this._containingDocument.DocumentElement, base.PropagatedNamespaces);
			return this._containingDocument;
		}

		public override object GetOutput(Type type)
		{
			if (type == typeof(XmlDocument))
			{
				return (XmlDocument)this.GetOutput();
			}
			throw new ArgumentException("The input type was invalid for this transform.", "type");
		}

		private Type[] _inputTypes = new Type[]
		{
			typeof(Stream),
			typeof(XmlDocument)
		};

		private Type[] _outputTypes = new Type[] { typeof(XmlDocument) };

		private XmlNodeList _encryptedDataList;

		private ArrayList _arrayListUri;

		private EncryptedXml _exml;

		private XmlDocument _containingDocument;

		private XmlNamespaceManager _nsm;

		private const string XmlDecryptionTransformNamespaceUrl = "http://www.w3.org/2002/07/decrypt#";
	}
}
