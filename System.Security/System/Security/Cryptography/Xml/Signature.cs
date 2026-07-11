using System;
using System.Collections;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public class Signature
	{
		static Signature()
		{
			Signature.dsigNsmgr.AddNamespace("xd", "http://www.w3.org/2000/09/xmldsig#");
		}

		public Signature()
		{
			this.list = new ArrayList();
		}

		public string Id
		{
			get
			{
				return this.id;
			}
			set
			{
				this.element = null;
				this.id = value;
			}
		}

		public KeyInfo KeyInfo
		{
			get
			{
				return this.key;
			}
			set
			{
				this.element = null;
				this.key = value;
			}
		}

		public IList ObjectList
		{
			get
			{
				return this.list;
			}
			set
			{
				this.list = ArrayList.Adapter(value);
			}
		}

		public byte[] SignatureValue
		{
			get
			{
				return this.signature;
			}
			set
			{
				this.element = null;
				this.signature = value;
			}
		}

		public SignedInfo SignedInfo
		{
			get
			{
				return this.info;
			}
			set
			{
				this.element = null;
				this.info = value;
			}
		}

		public void AddObject(DataObject dataObject)
		{
			this.list.Add(dataObject);
		}

		public XmlElement GetXml()
		{
			return this.GetXml(null);
		}

		internal XmlElement GetXml(XmlDocument document)
		{
			if (this.element != null)
			{
				return this.element;
			}
			if (this.info == null)
			{
				throw new CryptographicException("SignedInfo");
			}
			if (this.signature == null)
			{
				throw new CryptographicException("SignatureValue");
			}
			if (document == null)
			{
				document = new XmlDocument();
			}
			XmlElement xmlElement = document.CreateElement("Signature", "http://www.w3.org/2000/09/xmldsig#");
			if (this.id != null)
			{
				xmlElement.SetAttribute("Id", this.id);
			}
			XmlNode xmlNode = this.info.GetXml();
			XmlNode xmlNode2 = document.ImportNode(xmlNode, true);
			xmlElement.AppendChild(xmlNode2);
			if (this.signature != null)
			{
				XmlElement xmlElement2 = document.CreateElement("SignatureValue", "http://www.w3.org/2000/09/xmldsig#");
				xmlElement2.InnerText = Convert.ToBase64String(this.signature);
				xmlElement.AppendChild(xmlElement2);
			}
			if (this.key != null)
			{
				xmlNode = this.key.GetXml();
				xmlNode2 = document.ImportNode(xmlNode, true);
				xmlElement.AppendChild(xmlNode2);
			}
			if (this.list.Count > 0)
			{
				foreach (object obj in this.list)
				{
					xmlNode = ((DataObject)obj).GetXml();
					xmlNode2 = document.ImportNode(xmlNode, true);
					xmlElement.AppendChild(xmlNode2);
				}
			}
			return xmlElement;
		}

		private string GetAttribute(XmlElement xel, string attribute)
		{
			XmlAttribute xmlAttribute = xel.Attributes[attribute];
			if (xmlAttribute == null)
			{
				return null;
			}
			return xmlAttribute.InnerText;
		}

		public void LoadXml(XmlElement value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.LocalName == "Signature" && value.NamespaceURI == "http://www.w3.org/2000/09/xmldsig#")
			{
				this.id = this.GetAttribute(value, "Id");
				int num = this.NextElementPos(value.ChildNodes, 0, "SignedInfo", "http://www.w3.org/2000/09/xmldsig#", true);
				XmlElement xmlElement = (XmlElement)value.ChildNodes[num];
				this.info = new SignedInfo();
				this.info.LoadXml(xmlElement);
				num = this.NextElementPos(value.ChildNodes, num + 1, "SignatureValue", "http://www.w3.org/2000/09/xmldsig#", true);
				XmlElement xmlElement2 = (XmlElement)value.ChildNodes[num];
				this.signature = Convert.FromBase64String(xmlElement2.InnerText);
				num = this.NextElementPos(value.ChildNodes, num + 1, "KeyInfo", "http://www.w3.org/2000/09/xmldsig#", false);
				if (num > 0)
				{
					XmlElement xmlElement3 = (XmlElement)value.ChildNodes[num];
					this.key = new KeyInfo();
					this.key.LoadXml(xmlElement3);
				}
				using (IEnumerator enumerator = value.SelectNodes("xd:Object", Signature.dsigNsmgr).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						XmlElement xmlElement4 = (XmlElement)obj;
						DataObject dataObject = new DataObject();
						dataObject.LoadXml(xmlElement4);
						this.AddObject(dataObject);
					}
					goto IL_0180;
				}
				goto IL_0175;
				IL_0180:
				if (this.info == null)
				{
					throw new CryptographicException("SignedInfo");
				}
				if (this.signature == null)
				{
					throw new CryptographicException("SignatureValue");
				}
				return;
			}
			IL_0175:
			throw new CryptographicException("Malformed element: Signature.");
		}

		private int NextElementPos(XmlNodeList nl, int pos, string name, string ns, bool required)
		{
			while (pos < nl.Count)
			{
				if (nl[pos].NodeType == XmlNodeType.Element)
				{
					if (!(nl[pos].LocalName != name) && !(nl[pos].NamespaceURI != ns))
					{
						return pos;
					}
					if (required)
					{
						throw new CryptographicException("Malformed element " + name);
					}
					return -2;
				}
				else
				{
					pos++;
				}
			}
			if (required)
			{
				throw new CryptographicException("Malformed element " + name);
			}
			return -1;
		}

		private static XmlNamespaceManager dsigNsmgr = new XmlNamespaceManager(new NameTable());

		private ArrayList list;

		private SignedInfo info;

		private KeyInfo key;

		private string id;

		private byte[] signature;

		private XmlElement element;
	}
}
