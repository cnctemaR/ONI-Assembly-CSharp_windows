using System;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace MS.Internal.Xml.XPath
{
	internal abstract class BaseAxisQuery : Query
	{
		protected BaseAxisQuery(Query qyInput)
		{
			this._name = string.Empty;
			this._prefix = string.Empty;
			this._nsUri = string.Empty;
			this.qyInput = qyInput;
		}

		protected BaseAxisQuery(Query qyInput, string name, string prefix, XPathNodeType typeTest)
		{
			this.qyInput = qyInput;
			this._name = name;
			this._prefix = prefix;
			this._typeTest = typeTest;
			this._nameTest = prefix.Length != 0 || name.Length != 0;
			this._nsUri = string.Empty;
		}

		protected BaseAxisQuery(BaseAxisQuery other)
			: base(other)
		{
			this.qyInput = Query.Clone(other.qyInput);
			this._name = other._name;
			this._prefix = other._prefix;
			this._nsUri = other._nsUri;
			this._typeTest = other._typeTest;
			this._nameTest = other._nameTest;
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
			this._nsUri = context.LookupNamespace(this._prefix);
			this.qyInput.SetXsltContext(context);
		}

		protected string Name
		{
			get
			{
				return this._name;
			}
		}

		protected string Namespace
		{
			get
			{
				return this._nsUri;
			}
		}

		protected bool NameTest
		{
			get
			{
				return this._nameTest;
			}
		}

		protected XPathNodeType TypeTest
		{
			get
			{
				return this._typeTest;
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
				if ((this._name.Equals(e.LocalName) || this._name.Length == 0) && this._nsUri.Equals(e.NamespaceURI))
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
				if (this._name.Length != 0)
				{
					return 0.0;
				}
				if (this._prefix.Length != 0)
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

		internal Query qyInput;

		private bool _nameTest;

		private string _name;

		private string _prefix;

		private string _nsUri;

		private XPathNodeType _typeTest;

		protected XPathNavigator currentNode;

		protected int position;
	}
}
