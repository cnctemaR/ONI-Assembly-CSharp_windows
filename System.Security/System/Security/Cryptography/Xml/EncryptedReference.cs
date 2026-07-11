using System;
using System.Collections.Generic;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public abstract class EncryptedReference
	{
		protected EncryptedReference()
		{
			this.TransformChain = new TransformChain();
		}

		protected EncryptedReference(string uri)
		{
			this.Uri = uri;
			this.TransformChain = new TransformChain();
		}

		protected EncryptedReference(string uri, TransformChain tc)
			: this()
		{
			this.Uri = uri;
			this.TransformChain = tc;
		}

		[MonoTODO]
		protected internal bool CacheValid
		{
			get
			{
				return this.cacheValid;
			}
		}

		protected string ReferenceType
		{
			get
			{
				return this.referenceType;
			}
			set
			{
				this.referenceType = value;
			}
		}

		public TransformChain TransformChain
		{
			get
			{
				return this.tc;
			}
			set
			{
				this.tc = value;
			}
		}

		public string Uri
		{
			get
			{
				return this.uri;
			}
			set
			{
				this.uri = value;
			}
		}

		public void AddTransform(Transform transform)
		{
			this.TransformChain.Add(transform);
		}

		public virtual XmlElement GetXml()
		{
			return this.GetXml(new XmlDocument());
		}

		internal virtual XmlElement GetXml(XmlDocument document)
		{
			XmlElement xmlElement = document.CreateElement(this.ReferenceType, "http://www.w3.org/2001/04/xmlenc#");
			xmlElement.SetAttribute("URI", this.Uri);
			if (this.TransformChain != null && this.TransformChain.Count > 0)
			{
				XmlElement xmlElement2 = document.CreateElement("Transforms", "http://www.w3.org/2001/04/xmlenc#");
				foreach (object obj in this.TransformChain)
				{
					Transform transform = (Transform)obj;
					xmlElement2.AppendChild(document.ImportNode(transform.GetXml(), true));
				}
				xmlElement.AppendChild(xmlElement2);
			}
			return xmlElement;
		}

		[MonoTODO("Make compliant.")]
		public virtual void LoadXml(XmlElement value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.Uri = null;
			this.TransformChain = new TransformChain();
			foreach (object obj in value.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (!(xmlNode is XmlWhitespace))
				{
					string localName = xmlNode.LocalName;
					if (localName != null)
					{
						if (EncryptedReference.<>f__switch$map3 == null)
						{
							EncryptedReference.<>f__switch$map3 = new Dictionary<string, int>(1) { { "Transforms", 0 } };
						}
						int num;
						if (EncryptedReference.<>f__switch$map3.TryGetValue(localName, out num))
						{
							if (num == 0)
							{
								foreach (object obj2 in ((XmlElement)xmlNode).GetElementsByTagName("Transform", "http://www.w3.org/2000/09/xmldsig#"))
								{
									XmlNode xmlNode2 = (XmlNode)obj2;
									string value2 = ((XmlElement)xmlNode2).Attributes["Algorithm"].Value;
									if (value2 != null)
									{
										if (EncryptedReference.<>f__switch$map2 == null)
										{
											EncryptedReference.<>f__switch$map2 = new Dictionary<string, int>(9)
											{
												{ "http://www.w3.org/2000/09/xmldsig#base64", 0 },
												{ "http://www.w3.org/TR/2001/REC-xml-c14n-20010315", 1 },
												{ "http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithComments", 2 },
												{ "http://www.w3.org/2000/09/xmldsig#enveloped-signature", 3 },
												{ "http://www.w3.org/TR/1999/REC-xpath-19991116", 4 },
												{ "http://www.w3.org/TR/1999/REC-xslt-19991116", 5 },
												{ "http://www.w3.org/2001/10/xml-exc-c14n#", 6 },
												{ "http://www.w3.org/2001/10/xml-exc-c14n#WithComments", 7 },
												{ "http://www.w3.org/2002/07/decrypt#XML", 8 }
											};
										}
										int num2;
										if (EncryptedReference.<>f__switch$map2.TryGetValue(value2, out num2))
										{
											Transform transform;
											switch (num2)
											{
											case 0:
												transform = new XmlDsigBase64Transform();
												break;
											case 1:
												transform = new XmlDsigC14NTransform();
												break;
											case 2:
												transform = new XmlDsigC14NWithCommentsTransform();
												break;
											case 3:
												transform = new XmlDsigEnvelopedSignatureTransform();
												break;
											case 4:
												transform = new XmlDsigXPathTransform();
												break;
											case 5:
												transform = new XmlDsigXsltTransform();
												break;
											case 6:
												transform = new XmlDsigExcC14NTransform();
												break;
											case 7:
												transform = new XmlDsigExcC14NWithCommentsTransform();
												break;
											case 8:
												transform = new XmlDecryptionTransform();
												break;
											default:
												continue;
											}
											transform.LoadInnerXml(((XmlElement)xmlNode2).ChildNodes);
											this.TransformChain.Add(transform);
										}
									}
								}
							}
						}
					}
				}
			}
			if (value.HasAttribute("URI"))
			{
				this.Uri = value.Attributes["URI"].Value;
			}
		}

		private bool cacheValid;

		private string referenceType;

		private string uri;

		private TransformChain tc;
	}
}
