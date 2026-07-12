using System;
using System.Collections;
using System.Globalization;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public class SignedInfo : ICollection, IEnumerable
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

		public SignedInfo()
		{
			this._references = new ArrayList();
		}

		public IEnumerator GetEnumerator()
		{
			throw new NotSupportedException();
		}

		public void CopyTo(Array array, int index)
		{
			throw new NotSupportedException();
		}

		public int Count
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public bool IsReadOnly
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public bool IsSynchronized
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public object SyncRoot
		{
			get
			{
				throw new NotSupportedException();
			}
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
				this._cachedXml = null;
			}
		}

		public string CanonicalizationMethod
		{
			get
			{
				if (this._canonicalizationMethod == null)
				{
					return "http://www.w3.org/TR/2001/REC-xml-c14n-20010315";
				}
				return this._canonicalizationMethod;
			}
			set
			{
				this._canonicalizationMethod = value;
				this._cachedXml = null;
			}
		}

		public Transform CanonicalizationMethodObject
		{
			get
			{
				if (this._canonicalizationMethodTransform == null)
				{
					this._canonicalizationMethodTransform = CryptoHelpers.CreateFromName<Transform>(this.CanonicalizationMethod);
					if (this._canonicalizationMethodTransform == null)
					{
						throw new CryptographicException(string.Format(CultureInfo.CurrentCulture, "Could not create the XML transformation identified by the URI {0}.", this.CanonicalizationMethod));
					}
					this._canonicalizationMethodTransform.SignedXml = this.SignedXml;
					this._canonicalizationMethodTransform.Reference = null;
				}
				return this._canonicalizationMethodTransform;
			}
		}

		public string SignatureMethod
		{
			get
			{
				return this._signatureMethod;
			}
			set
			{
				this._signatureMethod = value;
				this._cachedXml = null;
			}
		}

		public string SignatureLength
		{
			get
			{
				return this._signatureLength;
			}
			set
			{
				this._signatureLength = value;
				this._cachedXml = null;
			}
		}

		public ArrayList References
		{
			get
			{
				return this._references;
			}
		}

		internal bool CacheValid
		{
			get
			{
				if (this._cachedXml == null)
				{
					return false;
				}
				using (IEnumerator enumerator = this.References.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (!((Reference)enumerator.Current).CacheValid)
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		public XmlElement GetXml()
		{
			if (this.CacheValid)
			{
				return this._cachedXml;
			}
			return this.GetXml(new XmlDocument
			{
				PreserveWhitespace = true
			});
		}

		internal XmlElement GetXml(XmlDocument document)
		{
			XmlElement xmlElement = document.CreateElement("SignedInfo", "http://www.w3.org/2000/09/xmldsig#");
			if (!string.IsNullOrEmpty(this._id))
			{
				xmlElement.SetAttribute("Id", this._id);
			}
			XmlElement xml = this.CanonicalizationMethodObject.GetXml(document, "CanonicalizationMethod");
			xmlElement.AppendChild(xml);
			if (string.IsNullOrEmpty(this._signatureMethod))
			{
				throw new CryptographicException("A signature method is required.");
			}
			XmlElement xmlElement2 = document.CreateElement("SignatureMethod", "http://www.w3.org/2000/09/xmldsig#");
			xmlElement2.SetAttribute("Algorithm", this._signatureMethod);
			if (this._signatureLength != null)
			{
				XmlElement xmlElement3 = document.CreateElement(null, "HMACOutputLength", "http://www.w3.org/2000/09/xmldsig#");
				XmlText xmlText = document.CreateTextNode(this._signatureLength);
				xmlElement3.AppendChild(xmlText);
				xmlElement2.AppendChild(xmlElement3);
			}
			xmlElement.AppendChild(xmlElement2);
			if (this._references.Count == 0)
			{
				throw new CryptographicException("At least one Reference element is required.");
			}
			for (int i = 0; i < this._references.Count; i++)
			{
				Reference reference = (Reference)this._references[i];
				xmlElement.AppendChild(reference.GetXml(document));
			}
			return xmlElement;
		}

		public void LoadXml(XmlElement value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (!value.LocalName.Equals("SignedInfo"))
			{
				throw new CryptographicException("Malformed element {0}.", "SignedInfo");
			}
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(value.OwnerDocument.NameTable);
			xmlNamespaceManager.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
			int num = 0;
			this._id = Utils.GetAttribute(value, "Id", "http://www.w3.org/2000/09/xmldsig#");
			if (!Utils.VerifyAttributes(value, "Id"))
			{
				throw new CryptographicException("Malformed element {0}.", "SignedInfo");
			}
			XmlNodeList xmlNodeList = value.SelectNodes("ds:CanonicalizationMethod", xmlNamespaceManager);
			if (xmlNodeList == null || xmlNodeList.Count == 0 || xmlNodeList.Count > 1)
			{
				throw new CryptographicException("Malformed element {0}.", "SignedInfo/CanonicalizationMethod");
			}
			XmlElement xmlElement = xmlNodeList.Item(0) as XmlElement;
			num += xmlNodeList.Count;
			this._canonicalizationMethod = Utils.GetAttribute(xmlElement, "Algorithm", "http://www.w3.org/2000/09/xmldsig#");
			if (this._canonicalizationMethod == null || !Utils.VerifyAttributes(xmlElement, "Algorithm"))
			{
				throw new CryptographicException("Malformed element {0}.", "SignedInfo/CanonicalizationMethod");
			}
			this._canonicalizationMethodTransform = null;
			if (xmlElement.ChildNodes.Count > 0)
			{
				this.CanonicalizationMethodObject.LoadInnerXml(xmlElement.ChildNodes);
			}
			XmlNodeList xmlNodeList2 = value.SelectNodes("ds:SignatureMethod", xmlNamespaceManager);
			if (xmlNodeList2 == null || xmlNodeList2.Count == 0 || xmlNodeList2.Count > 1)
			{
				throw new CryptographicException("Malformed element {0}.", "SignedInfo/SignatureMethod");
			}
			XmlElement xmlElement2 = xmlNodeList2.Item(0) as XmlElement;
			num += xmlNodeList2.Count;
			this._signatureMethod = Utils.GetAttribute(xmlElement2, "Algorithm", "http://www.w3.org/2000/09/xmldsig#");
			if (this._signatureMethod == null || !Utils.VerifyAttributes(xmlElement2, "Algorithm"))
			{
				throw new CryptographicException("Malformed element {0}.", "SignedInfo/SignatureMethod");
			}
			XmlElement xmlElement3 = xmlElement2.SelectSingleNode("ds:HMACOutputLength", xmlNamespaceManager) as XmlElement;
			if (xmlElement3 != null)
			{
				this._signatureLength = xmlElement3.InnerXml;
			}
			this._references.Clear();
			XmlNodeList xmlNodeList3 = value.SelectNodes("ds:Reference", xmlNamespaceManager);
			if (xmlNodeList3 != null)
			{
				if (xmlNodeList3.Count > 100)
				{
					throw new CryptographicException("Malformed element {0}.", "SignedInfo/Reference");
				}
				foreach (object obj in xmlNodeList3)
				{
					XmlElement xmlElement4 = ((XmlNode)obj) as XmlElement;
					Reference reference = new Reference();
					this.AddReference(reference);
					reference.LoadXml(xmlElement4);
				}
				num += xmlNodeList3.Count;
				if (value.SelectNodes("*").Count != num)
				{
					throw new CryptographicException("Malformed element {0}.", "SignedInfo");
				}
			}
			this._cachedXml = value;
		}

		public void AddReference(Reference reference)
		{
			if (reference == null)
			{
				throw new ArgumentNullException("reference");
			}
			reference.SignedXml = this.SignedXml;
			this._references.Add(reference);
		}

		private string _id;

		private string _canonicalizationMethod;

		private string _signatureMethod;

		private string _signatureLength;

		private ArrayList _references;

		private XmlElement _cachedXml;

		private SignedXml _signedXml;

		private Transform _canonicalizationMethodTransform;
	}
}
