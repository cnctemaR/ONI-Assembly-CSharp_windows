using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

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
					this.output = new Type[1];
					this.output[0] = typeof(XmlNodeList);
				}
				return this.output;
			}
		}

		protected override XmlNodeList GetInnerXml()
		{
			if (this.xpath == null)
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml("<XPath xmlns=\"http://www.w3.org/2000/09/xmldsig#\"></XPath>");
				this.xpath = xmlDocument.ChildNodes;
			}
			return this.xpath;
		}

		[MonoTODO("Evaluation of extension function here() results in different from MS.NET (is MS.NET really correct??).")]
		public override object GetOutput()
		{
			if (this.xpath == null || this.doc == null)
			{
				return new XmlDsigNodeList(new ArrayList());
			}
			string text = null;
			for (int i = 0; i < this.xpath.Count; i++)
			{
				switch (this.xpath[i].NodeType)
				{
				case XmlNodeType.Element:
				case XmlNodeType.Text:
				case XmlNodeType.CDATA:
					text += this.xpath[i].InnerText;
					break;
				}
			}
			this.ctx = new XmlDsigXPathTransform.XmlDsigXPathContext(this.doc);
			foreach (object obj in this.xpath)
			{
				XmlNode xmlNode = (XmlNode)obj;
				XPathNavigator xpathNavigator = xmlNode.CreateNavigator();
				XPathNodeIterator xpathNodeIterator = xpathNavigator.Select("namespace::*");
				while (xpathNodeIterator.MoveNext())
				{
					if (xpathNodeIterator.Current.LocalName != "xml")
					{
						this.ctx.AddNamespace(xpathNodeIterator.Current.LocalName, xpathNodeIterator.Current.Value);
					}
				}
			}
			return this.EvaluateMatch(this.doc, text);
		}

		public override object GetOutput(Type type)
		{
			if (type != typeof(XmlNodeList))
			{
				throw new ArgumentException("type");
			}
			return this.GetOutput();
		}

		private XmlDsigNodeList EvaluateMatch(XmlNode n, string xpath)
		{
			ArrayList arrayList = new ArrayList();
			XPathNavigator xpathNavigator = n.CreateNavigator();
			XPathExpression xpathExpression = xpathNavigator.Compile(xpath);
			xpathExpression.SetContext(this.ctx);
			this.EvaluateMatch(n, xpathExpression, arrayList);
			return new XmlDsigNodeList(arrayList);
		}

		private void EvaluateMatch(XmlNode n, XPathExpression exp, ArrayList al)
		{
			if (this.NodeMatches(n, exp))
			{
				al.Add(n);
			}
			if (n.Attributes != null)
			{
				for (int i = 0; i < n.Attributes.Count; i++)
				{
					if (this.NodeMatches(n.Attributes[i], exp))
					{
						al.Add(n.Attributes[i]);
					}
				}
			}
			for (int j = 0; j < n.ChildNodes.Count; j++)
			{
				this.EvaluateMatch(n.ChildNodes[j], exp, al);
			}
		}

		private bool NodeMatches(XmlNode n, XPathExpression exp)
		{
			object obj = n.CreateNavigator().Evaluate(exp);
			if (obj is bool)
			{
				return (bool)obj;
			}
			if (obj is double)
			{
				double num = (double)obj;
				return num != 0.0 && !double.IsNaN(num);
			}
			if (obj is string)
			{
				return ((string)obj).Length > 0;
			}
			if (obj is XPathNodeIterator)
			{
				XPathNodeIterator xpathNodeIterator = (XPathNodeIterator)obj;
				return xpathNodeIterator.Count > 0;
			}
			return false;
		}

		public override void LoadInnerXml(XmlNodeList nodeList)
		{
			if (nodeList == null)
			{
				throw new CryptographicException("nodeList");
			}
			this.xpath = nodeList;
		}

		public override void LoadInput(object obj)
		{
			if (obj is Stream)
			{
				this.doc = new XmlDocument();
				this.doc.PreserveWhitespace = true;
				this.doc.XmlResolver = base.GetResolver();
				this.doc.Load(new XmlSignatureStreamReader(new StreamReader((Stream)obj)));
			}
			else if (obj is XmlDocument)
			{
				this.doc = obj as XmlDocument;
			}
			else if (obj is XmlNodeList)
			{
				this.doc = new XmlDocument();
				this.doc.XmlResolver = base.GetResolver();
				foreach (object obj2 in (obj as XmlNodeList))
				{
					XmlNode xmlNode = (XmlNode)obj2;
					XmlNode xmlNode2 = this.doc.ImportNode(xmlNode, true);
					this.doc.AppendChild(xmlNode2);
				}
			}
		}

		private Type[] input;

		private Type[] output;

		private XmlNodeList xpath;

		private XmlDocument doc;

		private XsltContext ctx;

		internal class XmlDsigXPathContext : XsltContext
		{
			public XmlDsigXPathContext(XmlNode node)
			{
				this.here = new XmlDsigXPathTransform.XmlDsigXPathFunctionHere(node);
			}

			public override IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] argType)
			{
				if (name == "here" && prefix == string.Empty && argType.Length == 0)
				{
					return this.here;
				}
				return null;
			}

			public override bool Whitespace
			{
				get
				{
					return true;
				}
			}

			public override bool PreserveWhitespace(XPathNavigator node)
			{
				return true;
			}

			public override int CompareDocument(string s1, string s2)
			{
				return string.Compare(s1, s2);
			}

			public override IXsltContextVariable ResolveVariable(string prefix, string name)
			{
				throw new InvalidOperationException();
			}

			private XmlDsigXPathTransform.XmlDsigXPathFunctionHere here;
		}

		internal class XmlDsigXPathFunctionHere : IXsltContextFunction
		{
			public XmlDsigXPathFunctionHere(XmlNode node)
			{
				this.xpathNode = node.CreateNavigator().Select(".");
			}

			public XPathResultType[] ArgTypes
			{
				get
				{
					return XmlDsigXPathTransform.XmlDsigXPathFunctionHere.types;
				}
			}

			public int Maxargs
			{
				get
				{
					return 0;
				}
			}

			public int Minargs
			{
				get
				{
					return 0;
				}
			}

			public XPathResultType ReturnType
			{
				get
				{
					return XPathResultType.NodeSet;
				}
			}

			public object Invoke(XsltContext ctx, object[] args, XPathNavigator docContext)
			{
				if (args.Length != 0)
				{
					throw new ArgumentException("Not allowed arguments for function here().", "args");
				}
				return this.xpathNode.Clone();
			}

			private static XPathResultType[] types = new XPathResultType[0];

			private XPathNodeIterator xpathNode;
		}
	}
}
