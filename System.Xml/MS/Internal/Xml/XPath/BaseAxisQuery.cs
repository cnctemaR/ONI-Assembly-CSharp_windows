using System;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace MS.Internal.Xml.XPath
{
	internal abstract class BaseAxisQuery : Query
	{
		protected BaseAxisQuery(Query qyInput)
		{
			this.name = string.Empty;
			this.prefix = string.Empty;
			this.nsUri = string.Empty;
			this.qyInput = qyInput;
		}

		protected BaseAxisQuery(Query qyInput, string name, string prefix, XPathNodeType typeTest)
		{
			this.qyInput = qyInput;
			this.name = name;
			this.prefix = prefix;
			this.typeTest = typeTest;
			this.nameTest = prefix.Length != 0 || name.Length != 0;
			this.nsUri = string.Empty;
		}

		protected BaseAxisQuery(BaseAxisQuery other)
			: base(other)
		{
			this.qyInput = Query.Clone(other.qyInput);
			this.name = other.name;
			this.prefix = other.prefix;
			this.nsUri = other.nsUri;
			this.typeTest = other.typeTest;
			this.nameTest = other.nameTest;
			this.position = other.position;
			this.currentNode = other.currentNode;
		}

		public override void Reset()
		{
			this.position = 0;
			this.currentNode = null;
			this.qyInput.Reset();
		}

		public override void SetXsltContext(XsltContext context)
		{
			this.nsUri = context.LookupNamespace(this.prefix);
			this.qyInput.SetXsltContext(context);
		}

		protected string Name
		{
			get
			{
				return this.name;
			}
		}

		protected string Prefix
		{
			get
			{
				return this.prefix;
			}
		}

		protected string Namespace
		{
			get
			{
				return this.nsUri;
			}
		}

		protected bool NameTest
		{
			get
			{
				return this.nameTest;
			}
		}

		protected XPathNodeType TypeTest
		{
			get
			{
				return this.typeTest;
			}
		}

		public override int CurrentPosition
		{
			get
			{
				return this.position;
			}
		}

		public override XPathNavigator Current
		{
			get
			{
				return this.currentNode;
			}
		}

		public virtual bool matches(XPathNavigator e)
		{
			if (this.TypeTest == e.NodeType || this.TypeTest == XPathNodeType.All || (this.TypeTest == XPathNodeType.Text && (e.NodeType == XPathNodeType.Whitespace || e.NodeType == XPathNodeType.SignificantWhitespace)))
			{
				if (!this.NameTest)
				{
					return true;
				}
				if ((this.name.Equals(e.LocalName) || this.name.Length == 0) && this.nsUri.Equals(e.NamespaceURI))
				{
					return true;
				}
			}
			return false;
		}

		public override object Evaluate(XPathNodeIterator nodeIterator)
		{
			base.ResetCount();
			this.Reset();
			this.qyInput.Evaluate(nodeIterator);
			return this;
		}

		public override double XsltDefaultPriority
		{
			get
			{
				if (this.qyInput.GetType() != typeof(ContextQuery))
				{
					return 0.5;
				}
				if (this.name.Length != 0)
				{
					return 0.0;
				}
				if (this.prefix.Length != 0)
				{
					return -0.25;
				}
				return -0.5;
			}
		}

		public override XPathResultType StaticType
		{
			get
			{
				return XPathResultType.NodeSet;
			}
		}

		public override void PrintQuery(XmlWriter w)
		{
			w.WriteStartElement(base.GetType().Name);
			if (this.NameTest)
			{
				w.WriteAttributeString("name", (this.Prefix.Length != 0) ? (this.Prefix + ":" + this.Name) : this.Name);
			}
			if (this.TypeTest != XPathNodeType.Element)
			{
				w.WriteAttributeString("nodeType", this.TypeTest.ToString());
			}
			this.qyInput.PrintQuery(w);
			w.WriteEndElement();
		}

		internal Query qyInput;

		private bool nameTest;

		private string name;

		private string prefix;

		private string nsUri;

		private XPathNodeType typeTest;

		protected XPathNavigator currentNode;

		protected int position;
	}
}
