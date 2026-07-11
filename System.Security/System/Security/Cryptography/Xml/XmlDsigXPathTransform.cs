using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace System.Security.Cryptography.Xml
{
	public class XmlDsigXPathTransform : Transform
	{
		public XmlDsigXPathTransform()
		{
			base.Algorithm = "http://www.w3.org/TR/1999/REC-xpath-19991116";
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

		public override void LoadInnerXml(XmlNodeList nodeList)
		{
			if (nodeList == null)
			{
				throw new CryptographicException("Unknown transform has been encountered.");
			}
			foreach (object obj in nodeList)
			{
				XmlElement xmlElement = ((XmlNode)obj) as XmlElement;
				if (xmlElement != null && xmlElement.LocalName == "XPath")
				{
					this._xpathexpr = xmlElement.InnerXml.Trim(null);
					XmlNameTable nameTable = new XmlNodeReader(xmlElement).NameTable;
					this._nsm = new XmlNamespaceManager(nameTable);
					using (IEnumerator enumerator2 = xmlElement.Attributes.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							object obj2 = enumerator2.Current;
							XmlAttribute xmlAttribute = (XmlAttribute)obj2;
							if (xmlAttribute.Prefix == "xmlns")
							{
								string text = xmlAttribute.LocalName;
								string text2 = xmlAttribute.Value;
								if (text == null)
								{
									text = xmlElement.Prefix;
									text2 = xmlElement.NamespaceURI;
								}
								this._nsm.AddNamespace(text, text2);
							}
						}
						break;
					}
				}
			}
			if (this._xpathexpr == null)
			{
				throw new CryptographicException("Unknown transform has been encountered.");
			}
		}

		protected override XmlNodeList GetInnerXml()
		{
			XmlDocument xmlDocument = new XmlDocument();
			XmlElement xmlElement = xmlDocument.CreateElement(null, "XPath", "http://www.w3.org/2000/09/xmldsig#");
			if (this._nsm != null)
			{
				foreach (object obj in this._nsm)
				{
					string text = (string)obj;
					if (!(text == "xml") && !(text == "xmlns") && text != null && text.Length > 0)
					{
						xmlElement.SetAttribute("xmlns:" + text, this._nsm.LookupNamespace(text));
					}
				}
			}
			xmlElement.InnerXml = this._xpathexpr;
			xmlDocument.AppendChild(xmlElement);
			return xmlDocument.ChildNodes;
		}

		public override void LoadInput(object obj)
		{
			if (obj is Stream)
			{
				this.LoadStreamInput((Stream)obj);
				return;
			}
			if (obj is XmlNodeList)
			{
				this.LoadXmlNodeListInput((XmlNodeList)obj);
				return;
			}
			if (obj is XmlDocument)
			{
				this.LoadXmlDocumentInput((XmlDocument)obj);
			}
		}

		private void LoadStreamInput(Stream stream)
		{
			XmlResolver xmlResolver = (base.ResolverSet ? this._xmlResolver : new XmlSecureResolver(new XmlUrlResolver(), base.BaseURI));
			XmlReader xmlReader = Utils.PreProcessStreamInput(stream, xmlResolver, base.BaseURI);
			this._document = new XmlDocument();
			this._document.PreserveWhitespace = true;
			this._document.Load(xmlReader);
		}

		private void LoadXmlNodeListInput(XmlNodeList nodeList)
		{
			XmlResolver xmlResolver = (base.ResolverSet ? this._xmlResolver : new XmlSecureResolver(new XmlUrlResolver(), base.BaseURI));
			using (MemoryStream memoryStream = new MemoryStream(new CanonicalXml(nodeList, xmlResolver, true).GetBytes()))
			{
				this.LoadStreamInput(memoryStream);
			}
		}

		private void LoadXmlDocumentInput(XmlDocument doc)
		{
			this._document = doc;
		}

		public override object GetOutput()
		{
			CanonicalXmlNodeList canonicalXmlNodeList = new CanonicalXmlNodeList();
			if (!string.IsNullOrEmpty(this._xpathexpr))
			{
				XPathNavigator xpathNavigator = this._document.CreateNavigator();
				XPathNodeIterator xpathNodeIterator = xpathNavigator.Select("//. | //@*");
				XPathExpression xpathExpression = xpathNavigator.Compile("boolean(" + this._xpathexpr + ")");
				xpathExpression.SetContext(this._nsm);
				while (xpathNodeIterator.MoveNext())
				{
					XPathNavigator xpathNavigator2 = xpathNodeIterator.Current;
					XmlNode node = ((IHasXmlNode)xpathNavigator2).GetNode();
					if ((bool)xpathNodeIterator.Current.Evaluate(xpathExpression))
					{
						canonicalXmlNodeList.Add(node);
					}
				}
				xpathNodeIterator = xpathNavigator.Select("//namespace::*");
				while (xpathNodeIterator.MoveNext())
				{
					XPathNavigator xpathNavigator3 = xpathNodeIterator.Current;
					XmlNode node2 = ((IHasXmlNode)xpathNavigator3).GetNode();
					canonicalXmlNodeList.Add(node2);
				}
			}
			return canonicalXmlNodeList;
		}

		public override object GetOutput(Type type)
		{
			if (type != typeof(XmlNodeList) && !type.IsSubclassOf(typeof(XmlNodeList)))
			{
				throw new ArgumentException("The input type was invalid for this transform.", "type");
			}
			return (XmlNodeList)this.GetOutput();
		}

		private Type[] _inputTypes = new Type[]
		{
			typeof(Stream),
			typeof(XmlNodeList),
			typeof(XmlDocument)
		};

		private Type[] _outputTypes = new Type[] { typeof(XmlNodeList) };

		private string _xpathexpr;

		private XmlDocument _document;

		private XmlNamespaceManager _nsm;
	}
}
