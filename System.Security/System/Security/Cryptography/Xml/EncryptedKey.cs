using System;
using System.Collections.Generic;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public sealed class EncryptedKey : EncryptedType
	{
		public EncryptedKey()
		{
			this.referenceList = new ReferenceList();
		}

		public string CarriedKeyName
		{
			get
			{
				return this.carriedKeyName;
			}
			set
			{
				this.carriedKeyName = value;
			}
		}

		public string Recipient
		{
			get
			{
				return this.recipient;
			}
			set
			{
				this.recipient = value;
			}
		}

		public ReferenceList ReferenceList
		{
			get
			{
				return this.referenceList;
			}
		}

		public void AddReference(DataReference dataReference)
		{
			this.ReferenceList.Add(dataReference);
		}

		public void AddReference(KeyReference keyReference)
		{
			this.ReferenceList.Add(keyReference);
		}

		public override XmlElement GetXml()
		{
			return this.GetXml(new XmlDocument());
		}

		internal XmlElement GetXml(XmlDocument document)
		{
			if (this.CipherData == null)
			{
				throw new CryptographicException("Cipher data is not specified.");
			}
			XmlElement xmlElement = document.CreateElement("EncryptedKey", "http://www.w3.org/2001/04/xmlenc#");
			if (this.EncryptionMethod != null)
			{
				xmlElement.AppendChild(this.EncryptionMethod.GetXml(document));
			}
			if (base.KeyInfo != null)
			{
				xmlElement.AppendChild(document.ImportNode(base.KeyInfo.GetXml(), true));
			}
			if (this.CipherData != null)
			{
				xmlElement.AppendChild(this.CipherData.GetXml(document));
			}
			if (this.EncryptionProperties.Count > 0)
			{
				XmlElement xmlElement2 = document.CreateElement("EncryptionProperties", "http://www.w3.org/2001/04/xmlenc#");
				foreach (object obj in this.EncryptionProperties)
				{
					EncryptionProperty encryptionProperty = (EncryptionProperty)obj;
					xmlElement2.AppendChild(encryptionProperty.GetXml(document));
				}
				xmlElement.AppendChild(xmlElement2);
			}
			if (this.ReferenceList.Count > 0)
			{
				XmlElement xmlElement3 = document.CreateElement("ReferenceList", "http://www.w3.org/2001/04/xmlenc#");
				foreach (object obj2 in this.ReferenceList)
				{
					EncryptedReference encryptedReference = (EncryptedReference)obj2;
					xmlElement3.AppendChild(encryptedReference.GetXml(document));
				}
				xmlElement.AppendChild(xmlElement3);
			}
			if (this.CarriedKeyName != null)
			{
				XmlElement xmlElement4 = document.CreateElement("CarriedKeyName", "http://www.w3.org/2001/04/xmlenc#");
				xmlElement4.InnerText = this.CarriedKeyName;
				xmlElement.AppendChild(xmlElement4);
			}
			if (this.Id != null)
			{
				xmlElement.SetAttribute("Id", this.Id);
			}
			if (this.Type != null)
			{
				xmlElement.SetAttribute("Type", this.Type);
			}
			if (this.MimeType != null)
			{
				xmlElement.SetAttribute("MimeType", this.MimeType);
			}
			if (this.Encoding != null)
			{
				xmlElement.SetAttribute("Encoding", this.Encoding);
			}
			if (this.Recipient != null)
			{
				xmlElement.SetAttribute("Recipient", this.Recipient);
			}
			return xmlElement;
		}

		public override void LoadXml(XmlElement value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.LocalName != "EncryptedKey" || value.NamespaceURI != "http://www.w3.org/2001/04/xmlenc#")
			{
				throw new CryptographicException("Malformed EncryptedKey element.");
			}
			this.EncryptionMethod = null;
			this.EncryptionMethod = null;
			this.EncryptionProperties.Clear();
			this.ReferenceList.Clear();
			this.CarriedKeyName = null;
			this.Id = null;
			this.Type = null;
			this.MimeType = null;
			this.Encoding = null;
			this.Recipient = null;
			foreach (object obj in value.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (!(xmlNode is XmlWhitespace))
				{
					string localName = xmlNode.LocalName;
					switch (localName)
					{
					case "EncryptionMethod":
						this.EncryptionMethod = new EncryptionMethod();
						this.EncryptionMethod.LoadXml((XmlElement)xmlNode);
						break;
					case "KeyInfo":
						base.KeyInfo = new KeyInfo();
						base.KeyInfo.LoadXml((XmlElement)xmlNode);
						break;
					case "CipherData":
						this.CipherData = new CipherData();
						this.CipherData.LoadXml((XmlElement)xmlNode);
						break;
					case "EncryptionProperties":
						foreach (object obj2 in ((XmlElement)xmlNode).GetElementsByTagName("EncryptionProperty", "http://www.w3.org/2001/04/xmlenc#"))
						{
							XmlElement xmlElement = (XmlElement)obj2;
							this.EncryptionProperties.Add(new EncryptionProperty(xmlElement));
						}
						break;
					case "ReferenceList":
						foreach (object obj3 in ((XmlElement)xmlNode).ChildNodes)
						{
							XmlNode xmlNode2 = (XmlNode)obj3;
							if (!(xmlNode2 is XmlWhitespace))
							{
								string localName2 = xmlNode2.LocalName;
								if (localName2 != null)
								{
									if (EncryptedKey.<>f__switch$map6 == null)
									{
										EncryptedKey.<>f__switch$map6 = new Dictionary<string, int>(2)
										{
											{ "DataReference", 0 },
											{ "KeyReference", 1 }
										};
									}
									int num2;
									if (EncryptedKey.<>f__switch$map6.TryGetValue(localName2, out num2))
									{
										if (num2 != 0)
										{
											if (num2 == 1)
											{
												KeyReference keyReference = new KeyReference();
												keyReference.LoadXml((XmlElement)xmlNode2);
												this.AddReference(keyReference);
											}
										}
										else
										{
											DataReference dataReference = new DataReference();
											dataReference.LoadXml((XmlElement)xmlNode2);
											this.AddReference(dataReference);
										}
									}
								}
							}
						}
						break;
					case "CarriedKeyName":
						this.CarriedKeyName = ((XmlElement)xmlNode).InnerText;
						break;
					}
				}
			}
			if (value.HasAttribute("Id"))
			{
				this.Id = value.Attributes["Id"].Value;
			}
			if (value.HasAttribute("Type"))
			{
				this.Type = value.Attributes["Type"].Value;
			}
			if (value.HasAttribute("MimeType"))
			{
				this.MimeType = value.Attributes["MimeType"].Value;
			}
			if (value.HasAttribute("Encoding"))
			{
				this.Encoding = value.Attributes["Encoding"].Value;
			}
			if (value.HasAttribute("Recipient"))
			{
				this.Encoding = value.Attributes["Recipient"].Value;
			}
		}

		private string carriedKeyName;

		private string recipient;

		private ReferenceList referenceList;
	}
}
