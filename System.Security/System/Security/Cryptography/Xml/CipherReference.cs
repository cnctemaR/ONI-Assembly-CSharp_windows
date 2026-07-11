using System;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public sealed class CipherReference : EncryptedReference
	{
		public CipherReference()
		{
		}

		public CipherReference(string uri)
			: base(uri)
		{
		}

		public CipherReference(string uri, TransformChain tc)
			: base(uri, tc)
		{
		}

		public override XmlElement GetXml()
		{
			return this.GetXml(new XmlDocument());
		}

		internal override XmlElement GetXml(XmlDocument document)
		{
			XmlElement xmlElement = document.CreateElement("CipherReference", "http://www.w3.org/2001/04/xmlenc#");
			xmlElement.SetAttribute("URI", base.Uri);
			if (base.TransformChain != null && base.TransformChain.Count > 0)
			{
				XmlElement xmlElement2 = document.CreateElement("Transforms", "http://www.w3.org/2001/04/xmlenc#");
				foreach (object obj in base.TransformChain)
				{
					Transform transform = (Transform)obj;
					xmlElement2.AppendChild(document.ImportNode(transform.GetXml(), true));
				}
				xmlElement.AppendChild(xmlElement2);
			}
			return xmlElement;
		}

		public override void LoadXml(XmlElement value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.LocalName != "CipherReference" || value.NamespaceURI != "http://www.w3.org/2001/04/xmlenc#")
			{
				throw new CryptographicException("Malformed CipherReference element.");
			}
			base.LoadXml(value);
		}
	}
}
