using System;
using System.Collections;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public class Signature
	{
		internal SignedXml SignedXml
		{
			get
			{
				return this._signedXml;
			}
			set
			{
				this._signedXml = value;
			}
		}

		public Signature()
		{
			this._embeddedObjects = new ArrayList();
			this._referencedItems = new CanonicalXmlNodeList();
		}

		public string Id
		{
			get
			{
				return this._id;
			}
			set
			{
				this._id = value;
			}
		}

		public SignedInfo SignedInfo
		{
			get
			{
				return this._signedInfo;
			}
			set
			{
				this._signedInfo = value;
				if (this.SignedXml != null && this._signedInfo != null)
				{
					this._signedInfo.SignedXml = this.SignedXml;
				}
			}
		}

		public byte[] SignatureValue
		{
			get
			{
				return this._signatureValue;
			}
			set
			{
				this._signatureValue = value;
			}
		}

		public KeyInfo KeyInfo
		{
			get
			{
				if (this._keyInfo == null)
				{
					this._keyInfo = new KeyInfo();
				}
				return this._keyInfo;
			}
			set
			{
				this._keyInfo = value;
			}
		}

		public IList ObjectList
		{
			get
			{
				return this._embeddedObjects;
			}
			set
			{
				this._embeddedObjects = value;
			}
		}

		internal CanonicalXmlNodeList ReferencedItems
		{
			get
			{
				return this._referencedItems;
			}
		}

		public XmlElement GetXml()
		{
			return this.GetXml(new XmlDocument
			{
				PreserveWhitespace = true
			});
		}

		internal XmlElement GetXml(XmlDocument document)
		{
			XmlElement xmlElement = document.CreateElement("Signature", "http://www.w3.org/2000/09/xmldsig#");
			if (!string.IsNullOrEmpty(this._id))
			{
				xmlElement.SetAttribute("Id", this._id);
			}
			if (this._signedInfo == null)
			{
				throw new CryptographicException("Signature requires a SignedInfo.");
			}
			xmlElement.AppendChild(this._signedInfo.GetXml(document));
			if (this._signatureValue == null)
			{
				throw new CryptographicException("Signature requires a SignatureValue.");
			}
			XmlElement xmlElement2 = document.CreateElement("SignatureValue", "http://www.w3.org/2000/09/xmldsig#");
			xmlElement2.AppendChild(document.CreateTextNode(Convert.ToBase64String(this._signatureValue)));
			if (!string.IsNullOrEmpty(this._signatureValueId))
			{
				xmlElement2.SetAttribute("Id", this._signatureValueId);
			}
			xmlElement.AppendChild(xmlElement2);
			if (this.KeyInfo.Count > 0)
			{
				xmlElement.AppendChild(this.KeyInfo.GetXml(document));
			}
			foreach (object obj in this._embeddedObjects)
			{
				DataObject dataObject = obj as DataObject;
				if (dataObject != null)
				{
					xmlElement.AppendChild(dataObject.GetXml(document));
				}
			}
			return xmlElement;
		}

		public void LoadXml(XmlElement value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (!value.LocalName.Equals("Signature"))
			{
				throw new CryptographicException("Malformed element {0}.", "Signature");
			}
			this._id = Utils.GetAttribute(value, "Id", "http://www.w3.org/2000/09/xmldsig#");
			if (!Utils.VerifyAttributes(value, "Id"))
			{
				throw new CryptographicException("Malformed element {0}.", "Signature");
			}
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(value.OwnerDocument.NameTable);
			xmlNamespaceManager.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
			int num = 0;
			XmlNodeList xmlNodeList = value.SelectNodes("ds:SignedInfo", xmlNamespaceManager);
			if (xmlNodeList == null || xmlNodeList.Count == 0 || xmlNodeList.Count > 1)
			{
				throw new CryptographicException("Malformed element {0}.", "SignedInfo");
			}
			XmlElement xmlElement = xmlNodeList[0] as XmlElement;
			num += xmlNodeList.Count;
			this.SignedInfo = new SignedInfo();
			this.SignedInfo.LoadXml(xmlElement);
			XmlNodeList xmlNodeList2 = value.SelectNodes("ds:SignatureValue", xmlNamespaceManager);
			if (xmlNodeList2 == null || xmlNodeList2.Count == 0 || xmlNodeList2.Count > 1)
			{
				throw new CryptographicException("Malformed element {0}.", "SignatureValue");
			}
			XmlElement xmlElement2 = xmlNodeList2[0] as XmlElement;
			num += xmlNodeList2.Count;
			this._signatureValue = Convert.FromBase64String(Utils.DiscardWhiteSpaces(xmlElement2.InnerText));
			this._signatureValueId = Utils.GetAttribute(xmlElement2, "Id", "http://www.w3.org/2000/09/xmldsig#");
			if (!Utils.VerifyAttributes(xmlElement2, "Id"))
			{
				throw new CryptographicException("Malformed element {0}.", "SignatureValue");
			}
			XmlNodeList xmlNodeList3 = value.SelectNodes("ds:KeyInfo", xmlNamespaceManager);
			this._keyInfo = new KeyInfo();
			if (xmlNodeList3 != null)
			{
				if (xmlNodeList3.Count > 1)
				{
					throw new CryptographicException("Malformed element {0}.", "KeyInfo");
				}
				foreach (object obj in xmlNodeList3)
				{
					XmlElement xmlElement3 = ((XmlNode)obj) as XmlElement;
					if (xmlElement3 != null)
					{
						this._keyInfo.LoadXml(xmlElement3);
					}
				}
				num += xmlNodeList3.Count;
			}
			XmlNodeList xmlNodeList4 = value.SelectNodes("ds:Object", xmlNamespaceManager);
			this._embeddedObjects.Clear();
			if (xmlNodeList4 != null)
			{
				foreach (object obj2 in xmlNodeList4)
				{
					XmlElement xmlElement4 = ((XmlNode)obj2) as XmlElement;
					if (xmlElement4 != null)
					{
						DataObject dataObject = new DataObject();
						dataObject.LoadXml(xmlElement4);
						this._embeddedObjects.Add(dataObject);
					}
				}
				num += xmlNodeList4.Count;
			}
			XmlNodeList xmlNodeList5 = value.SelectNodes("//*[@Id]", xmlNamespaceManager);
			if (xmlNodeList5 != null)
			{
				foreach (object obj3 in xmlNodeList5)
				{
					XmlNode xmlNode = (XmlNode)obj3;
					this._referencedItems.Add(xmlNode);
				}
			}
			if (value.SelectNodes("*").Count != num)
			{
				throw new CryptographicException("Malformed element {0}.", "Signature");
			}
		}

		public void AddObject(DataObject dataObject)
		{
			this._embeddedObjects.Add(dataObject);
		}

		private string _id;

		private SignedInfo _signedInfo;

		private byte[] _signatureValue;

		private string _signatureValueId;

		private KeyInfo _keyInfo;

		private IList _embeddedObjects;

		private CanonicalXmlNodeList _referencedItems;

		private SignedXml _signedXml;
	}
}
