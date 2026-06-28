using System;
using System.Collections;
using System.IO;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public class XmlDsigEnvelopedSignatureTransform : Transform
	{
		public XmlDsigEnvelopedSignatureTransform()
			: this(false)
		{
		}

		public XmlDsigEnvelopedSignatureTransform(bool includeComments)
		{
			base.Algorithm = "http://www.w3.org/2000/09/xmldsig#enveloped-signature";
			this.comments = includeComments;
		}

		public override Type[] InputTypes
		{
			get
			{
				if (this.input == null)
				{
					this.input = new Type[3];
					this.input[0] = typeof(Stream);
					this.input[1] = typeof(XmlDocument);
					this.input[2] = typeof(XmlNodeList);
				}
				return this.input;
			}
		}

		public override Type[] OutputTypes
		{
			get
			{
				if (this.output == null)
				{
					this.output = new Type[2];
					this.output[0] = typeof(XmlDocument);
					this.output[1] = typeof(XmlNodeList);
				}
				return this.output;
			}
		}

		protected override XmlNodeList GetInnerXml()
		{
			return null;
		}

		public override object GetOutput()
		{
			if (this.inputObj is Stream)
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.PreserveWhitespace = true;
				xmlDocument.XmlResolver = base.GetResolver();
				xmlDocument.Load(new XmlSignatureStreamReader(new StreamReader(this.inputObj as Stream)));
				return this.GetOutputFromNode(xmlDocument, this.GetNamespaceManager(xmlDocument), true);
			}
			if (this.inputObj is XmlDocument)
			{
				XmlDocument xmlDocument = this.inputObj as XmlDocument;
				return this.GetOutputFromNode(xmlDocument, this.GetNamespaceManager(xmlDocument), true);
			}
			if (this.inputObj is XmlNodeList)
			{
				ArrayList arrayList = new ArrayList();
				XmlNodeList xmlNodeList = (XmlNodeList)this.inputObj;
				if (xmlNodeList.Count > 0)
				{
					XmlNamespaceManager namespaceManager = this.GetNamespaceManager(xmlNodeList.Item(0));
					ArrayList arrayList2 = new ArrayList();
					foreach (object obj in xmlNodeList)
					{
						XmlNode xmlNode = (XmlNode)obj;
						arrayList2.Add(xmlNode);
					}
					foreach (object obj2 in arrayList2)
					{
						XmlNode xmlNode2 = (XmlNode)obj2;
						if (xmlNode2.SelectNodes("ancestor-or-self::dsig:Signature", namespaceManager).Count == 0)
						{
							arrayList.Add(this.GetOutputFromNode(xmlNode2, namespaceManager, false));
						}
					}
				}
				return new XmlDsigNodeList(arrayList);
			}
			if (this.inputObj is XmlElement)
			{
				XmlElement xmlElement = this.inputObj as XmlElement;
				XmlNamespaceManager namespaceManager2 = this.GetNamespaceManager(xmlElement);
				if (xmlElement.SelectNodes("ancestor-or-self::dsig:Signature", namespaceManager2).Count == 0)
				{
					return this.GetOutputFromNode(xmlElement, namespaceManager2, true);
				}
			}
			throw new NullReferenceException();
		}

		private XmlNamespaceManager GetNamespaceManager(XmlNode n)
		{
			XmlDocument xmlDocument = ((!(n is XmlDocument)) ? n.OwnerDocument : (n as XmlDocument));
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
			xmlNamespaceManager.AddNamespace("dsig", "http://www.w3.org/2000/09/xmldsig#");
			return xmlNamespaceManager;
		}

		private XmlNode GetOutputFromNode(XmlNode input, XmlNamespaceManager nsmgr, bool remove)
		{
			if (remove)
			{
				XmlNodeList xmlNodeList = input.SelectNodes("descendant-or-self::dsig:Signature", nsmgr);
				ArrayList arrayList = new ArrayList();
				foreach (object obj in xmlNodeList)
				{
					XmlNode xmlNode = (XmlNode)obj;
					arrayList.Add(xmlNode);
				}
				foreach (object obj2 in arrayList)
				{
					XmlNode xmlNode2 = (XmlNode)obj2;
					xmlNode2.ParentNode.RemoveChild(xmlNode2);
				}
			}
			return input;
		}

		public override object GetOutput(Type type)
		{
			if (type == typeof(Stream))
			{
				return this.GetOutput();
			}
			throw new ArgumentException("type");
		}

		public override void LoadInnerXml(XmlNodeList nodeList)
		{
		}

		public override void LoadInput(object obj)
		{
			this.inputObj = obj;
		}

		private Type[] input;

		private Type[] output;

		private bool comments;

		private object inputObj;
	}
}
