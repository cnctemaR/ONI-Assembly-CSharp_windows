using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public class KeyInfo : IEnumerable
	{
		public KeyInfo()
		{
			this.Info = new ArrayList();
		}

		public int Count
		{
			get
			{
				return this.Info.Count;
			}
		}

		public string Id
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

		public void AddClause(KeyInfoClause clause)
		{
			this.Info.Add(clause);
		}

		public IEnumerator GetEnumerator()
		{
			return this.Info.GetEnumerator();
		}

		public IEnumerator GetEnumerator(Type requestedObjectType)
		{
			ArrayList arrayList = new ArrayList();
			IEnumerator enumerator = this.Info.GetEnumerator();
			do
			{
				if (enumerator.Current.GetType().Equals(requestedObjectType))
				{
					arrayList.Add(enumerator.Current);
				}
			}
			while (enumerator.MoveNext());
			return arrayList.GetEnumerator();
		}

		public XmlElement GetXml()
		{
			XmlDocument xmlDocument = new XmlDocument();
			XmlElement xmlElement = xmlDocument.CreateElement("KeyInfo", "http://www.w3.org/2000/09/xmldsig#");
			foreach (object obj in this.Info)
			{
				KeyInfoClause keyInfoClause = (KeyInfoClause)obj;
				XmlNode xml = keyInfoClause.GetXml();
				XmlNode xmlNode = xmlDocument.ImportNode(xml, true);
				xmlElement.AppendChild(xmlNode);
			}
			return xmlElement;
		}

		public void LoadXml(XmlElement value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.Id = ((value.Attributes["Id"] == null) ? null : value.GetAttribute("Id"));
			if (value.LocalName == "KeyInfo" && value.NamespaceURI == "http://www.w3.org/2000/09/xmldsig#")
			{
				foreach (object obj in value.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					if (xmlNode.NodeType == XmlNodeType.Element)
					{
						KeyInfoClause keyInfoClause = null;
						string localName = xmlNode.LocalName;
						if (localName == null)
						{
							goto IL_0255;
						}
						if (KeyInfo.<>f__switch$mapC == null)
						{
							KeyInfo.<>f__switch$mapC = new Dictionary<string, int>(6)
							{
								{ "KeyValue", 0 },
								{ "KeyName", 1 },
								{ "RetrievalMethod", 2 },
								{ "X509Data", 3 },
								{ "RSAKeyValue", 4 },
								{ "EncryptedKey", 5 }
							};
						}
						int num;
						if (!KeyInfo.<>f__switch$mapC.TryGetValue(localName, out num))
						{
							goto IL_0255;
						}
						switch (num)
						{
						case 0:
						{
							XmlNodeList childNodes = xmlNode.ChildNodes;
							if (childNodes.Count > 0)
							{
								foreach (object obj2 in childNodes)
								{
									XmlNode xmlNode2 = (XmlNode)obj2;
									string localName2 = xmlNode2.LocalName;
									if (localName2 != null)
									{
										if (KeyInfo.<>f__switch$mapB == null)
										{
											KeyInfo.<>f__switch$mapB = new Dictionary<string, int>(2)
											{
												{ "DSAKeyValue", 0 },
												{ "RSAKeyValue", 1 }
											};
										}
										int num2;
										if (KeyInfo.<>f__switch$mapB.TryGetValue(localName2, out num2))
										{
											if (num2 != 0)
											{
												if (num2 == 1)
												{
													keyInfoClause = new RSAKeyValue();
												}
											}
											else
											{
												keyInfoClause = new DSAKeyValue();
											}
										}
									}
								}
							}
							break;
						}
						case 1:
							keyInfoClause = new KeyInfoName();
							break;
						case 2:
							keyInfoClause = new KeyInfoRetrievalMethod();
							break;
						case 3:
							keyInfoClause = new KeyInfoX509Data();
							break;
						case 4:
							keyInfoClause = new RSAKeyValue();
							break;
						case 5:
							keyInfoClause = new KeyInfoEncryptedKey();
							break;
						default:
							goto IL_0255;
						}
						IL_0260:
						if (keyInfoClause != null)
						{
							keyInfoClause.LoadXml((XmlElement)xmlNode);
							this.AddClause(keyInfoClause);
							continue;
						}
						continue;
						IL_0255:
						keyInfoClause = new KeyInfoNode();
						goto IL_0260;
					}
				}
			}
		}

		private ArrayList Info;

		private string id;
	}
}
