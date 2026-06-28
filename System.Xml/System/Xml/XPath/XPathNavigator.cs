using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Schema;
using System.Xml.Xsl;
using Mono.Xml.XPath;

namespace System.Xml.XPath
{
	public abstract class XPathNavigator : XPathItem, ICloneable, IXmlNamespaceResolver, IXPathNavigable
	{
		object ICloneable.Clone()
		{
			return this.Clone();
		}

		public static IEqualityComparer NavigatorComparer
		{
			get
			{
				return XPathNavigatorComparer.Instance;
			}
		}

		public abstract string BaseURI { get; }

		public virtual bool CanEdit
		{
			get
			{
				return false;
			}
		}

		public virtual bool HasAttributes
		{
			get
			{
				if (!this.MoveToFirstAttribute())
				{
					return false;
				}
				this.MoveToParent();
				return true;
			}
		}

		public virtual bool HasChildren
		{
			get
			{
				if (!this.MoveToFirstChild())
				{
					return false;
				}
				this.MoveToParent();
				return true;
			}
		}

		public abstract bool IsEmptyElement { get; }

		public abstract string LocalName { get; }

		public abstract string Name { get; }

		public abstract string NamespaceURI { get; }

		public abstract XmlNameTable NameTable { get; }

		public abstract XPathNodeType NodeType { get; }

		public abstract string Prefix { get; }

		public virtual string XmlLang
		{
			get
			{
				XPathNavigator xpathNavigator = this.Clone();
				XPathNodeType nodeType = xpathNavigator.NodeType;
				if (nodeType == XPathNodeType.Attribute || nodeType == XPathNodeType.Namespace)
				{
					xpathNavigator.MoveToParent();
				}
				while (!xpathNavigator.MoveToAttribute("lang", "http://www.w3.org/XML/1998/namespace"))
				{
					if (!xpathNavigator.MoveToParent())
					{
						return string.Empty;
					}
				}
				return xpathNavigator.Value;
			}
		}

		public abstract XPathNavigator Clone();

		public virtual XmlNodeOrder ComparePosition(XPathNavigator nav)
		{
			if (this.IsSamePosition(nav))
			{
				return XmlNodeOrder.Same;
			}
			if (this.IsDescendant(nav))
			{
				return XmlNodeOrder.Before;
			}
			if (nav.IsDescendant(this))
			{
				return XmlNodeOrder.After;
			}
			XPathNavigator xpathNavigator = this.Clone();
			XPathNavigator xpathNavigator2 = nav.Clone();
			xpathNavigator.MoveToRoot();
			xpathNavigator2.MoveToRoot();
			if (!xpathNavigator.IsSamePosition(xpathNavigator2))
			{
				return XmlNodeOrder.Unknown;
			}
			xpathNavigator.MoveTo(this);
			xpathNavigator2.MoveTo(nav);
			int num = 0;
			while (xpathNavigator.MoveToParent())
			{
				num++;
			}
			xpathNavigator.MoveTo(this);
			int num2 = 0;
			while (xpathNavigator2.MoveToParent())
			{
				num2++;
			}
			xpathNavigator2.MoveTo(nav);
			int i;
			for (i = num; i > num2; i--)
			{
				xpathNavigator.MoveToParent();
			}
			for (int j = num2; j > i; j--)
			{
				xpathNavigator2.MoveToParent();
			}
			while (!xpathNavigator.IsSamePosition(xpathNavigator2))
			{
				xpathNavigator.MoveToParent();
				xpathNavigator2.MoveToParent();
				i--;
			}
			xpathNavigator.MoveTo(this);
			for (int k = num; k > i + 1; k--)
			{
				xpathNavigator.MoveToParent();
			}
			xpathNavigator2.MoveTo(nav);
			for (int l = num2; l > i + 1; l--)
			{
				xpathNavigator2.MoveToParent();
			}
			if (xpathNavigator.NodeType == XPathNodeType.Namespace)
			{
				if (xpathNavigator2.NodeType != XPathNodeType.Namespace)
				{
					return XmlNodeOrder.Before;
				}
				while (xpathNavigator.MoveToNextNamespace())
				{
					if (xpathNavigator.IsSamePosition(xpathNavigator2))
					{
						return XmlNodeOrder.Before;
					}
				}
				return XmlNodeOrder.After;
			}
			else
			{
				if (xpathNavigator2.NodeType == XPathNodeType.Namespace)
				{
					return XmlNodeOrder.After;
				}
				if (xpathNavigator.NodeType != XPathNodeType.Attribute)
				{
					while (xpathNavigator.MoveToNext())
					{
						if (xpathNavigator.IsSamePosition(xpathNavigator2))
						{
							return XmlNodeOrder.Before;
						}
					}
					return XmlNodeOrder.After;
				}
				if (xpathNavigator2.NodeType != XPathNodeType.Attribute)
				{
					return XmlNodeOrder.Before;
				}
				while (xpathNavigator.MoveToNextAttribute())
				{
					if (xpathNavigator.IsSamePosition(xpathNavigator2))
					{
						return XmlNodeOrder.Before;
					}
				}
				return XmlNodeOrder.After;
			}
		}

		public virtual XPathExpression Compile(string xpath)
		{
			return XPathExpression.Compile(xpath);
		}

		internal virtual XPathExpression Compile(string xpath, IStaticXsltContext ctx)
		{
			return XPathExpression.Compile(xpath, null, ctx);
		}

		public virtual object Evaluate(string xpath)
		{
			return this.Evaluate(this.Compile(xpath));
		}

		public virtual object Evaluate(XPathExpression expr)
		{
			return this.Evaluate(expr, null);
		}

		public virtual object Evaluate(XPathExpression expr, XPathNodeIterator context)
		{
			return this.Evaluate(expr, context, null);
		}

		private BaseIterator ToBaseIterator(XPathNodeIterator iter, IXmlNamespaceResolver ctx)
		{
			BaseIterator baseIterator = iter as BaseIterator;
			if (baseIterator == null)
			{
				baseIterator = new WrapperIterator(iter, ctx);
			}
			return baseIterator;
		}

		private object Evaluate(XPathExpression expr, XPathNodeIterator context, IXmlNamespaceResolver ctx)
		{
			CompiledExpression compiledExpression = (CompiledExpression)expr;
			if (ctx == null)
			{
				ctx = compiledExpression.NamespaceManager;
			}
			if (context == null)
			{
				context = new NullIterator(this, ctx);
			}
			BaseIterator baseIterator = this.ToBaseIterator(context, ctx);
			baseIterator.NamespaceManager = ctx;
			return compiledExpression.Evaluate(baseIterator);
		}

		internal XPathNodeIterator EvaluateNodeSet(XPathExpression expr, XPathNodeIterator context, IXmlNamespaceResolver ctx)
		{
			CompiledExpression compiledExpression = (CompiledExpression)expr;
			if (ctx == null)
			{
				ctx = compiledExpression.NamespaceManager;
			}
			if (context == null)
			{
				context = new NullIterator(this, compiledExpression.NamespaceManager);
			}
			BaseIterator baseIterator = this.ToBaseIterator(context, ctx);
			baseIterator.NamespaceManager = ctx;
			return compiledExpression.EvaluateNodeSet(baseIterator);
		}

		internal string EvaluateString(XPathExpression expr, XPathNodeIterator context, IXmlNamespaceResolver ctx)
		{
			CompiledExpression compiledExpression = (CompiledExpression)expr;
			if (ctx == null)
			{
				ctx = compiledExpression.NamespaceManager;
			}
			if (context == null)
			{
				context = new NullIterator(this, compiledExpression.NamespaceManager);
			}
			BaseIterator baseIterator = this.ToBaseIterator(context, ctx);
			return compiledExpression.EvaluateString(baseIterator);
		}

		internal double EvaluateNumber(XPathExpression expr, XPathNodeIterator context, IXmlNamespaceResolver ctx)
		{
			CompiledExpression compiledExpression = (CompiledExpression)expr;
			if (ctx == null)
			{
				ctx = compiledExpression.NamespaceManager;
			}
			if (context == null)
			{
				context = new NullIterator(this, compiledExpression.NamespaceManager);
			}
			BaseIterator baseIterator = this.ToBaseIterator(context, ctx);
			baseIterator.NamespaceManager = ctx;
			return compiledExpression.EvaluateNumber(baseIterator);
		}

		internal bool EvaluateBoolean(XPathExpression expr, XPathNodeIterator context, IXmlNamespaceResolver ctx)
		{
			CompiledExpression compiledExpression = (CompiledExpression)expr;
			if (ctx == null)
			{
				ctx = compiledExpression.NamespaceManager;
			}
			if (context == null)
			{
				context = new NullIterator(this, compiledExpression.NamespaceManager);
			}
			BaseIterator baseIterator = this.ToBaseIterator(context, ctx);
			baseIterator.NamespaceManager = ctx;
			return compiledExpression.EvaluateBoolean(baseIterator);
		}

		public virtual string GetAttribute(string localName, string namespaceURI)
		{
			if (!this.MoveToAttribute(localName, namespaceURI))
			{
				return string.Empty;
			}
			string value = this.Value;
			this.MoveToParent();
			return value;
		}

		public virtual string GetNamespace(string name)
		{
			if (!this.MoveToNamespace(name))
			{
				return string.Empty;
			}
			string value = this.Value;
			this.MoveToParent();
			return value;
		}

		public virtual bool IsDescendant(XPathNavigator nav)
		{
			if (nav != null)
			{
				nav = nav.Clone();
				while (nav.MoveToParent())
				{
					if (this.IsSamePosition(nav))
					{
						return true;
					}
				}
			}
			return false;
		}

		public abstract bool IsSamePosition(XPathNavigator other);

		public virtual bool Matches(string xpath)
		{
			return this.Matches(this.Compile(xpath));
		}

		public virtual bool Matches(XPathExpression expr)
		{
			Expression expression = ((CompiledExpression)expr).ExpressionNode;
			if (expression is ExprRoot)
			{
				return this.NodeType == XPathNodeType.Root;
			}
			NodeTest nodeTest = expression as NodeTest;
			if (nodeTest == null)
			{
				if (expression is ExprFilter)
				{
					do
					{
						expression = ((ExprFilter)expression).LeftHandSide;
					}
					while (expression is ExprFilter);
					if (expression is NodeTest && !((NodeTest)expression).Match(((CompiledExpression)expr).NamespaceManager, this))
					{
						return false;
					}
				}
				switch (expression.ReturnType)
				{
				case XPathResultType.NodeSet:
				case XPathResultType.Any:
				{
					XPathNodeType evaluatedNodeType = expression.EvaluatedNodeType;
					if (evaluatedNodeType == XPathNodeType.Attribute || evaluatedNodeType == XPathNodeType.Namespace)
					{
						if (this.NodeType != expression.EvaluatedNodeType)
						{
							return false;
						}
					}
					XPathNodeIterator xpathNodeIterator = this.Select(expr);
					while (xpathNodeIterator.MoveNext())
					{
						if (this.IsSamePosition(xpathNodeIterator.Current))
						{
							return true;
						}
					}
					XPathNavigator xpathNavigator = this.Clone();
					while (xpathNavigator.MoveToParent())
					{
						xpathNodeIterator = xpathNavigator.Select(expr);
						while (xpathNodeIterator.MoveNext())
						{
							if (this.IsSamePosition(xpathNodeIterator.Current))
							{
								return true;
							}
						}
					}
					return false;
				}
				}
				return false;
			}
			Axes axis = nodeTest.Axis.Axis;
			if (axis != Axes.Attribute && axis != Axes.Child)
			{
				throw new XPathException("Only child and attribute pattern are allowed for a pattern.");
			}
			return nodeTest.Match(((CompiledExpression)expr).NamespaceManager, this);
		}

		public abstract bool MoveTo(XPathNavigator other);

		public virtual bool MoveToAttribute(string localName, string namespaceURI)
		{
			if (this.MoveToFirstAttribute())
			{
				while (!(this.LocalName == localName) || !(this.NamespaceURI == namespaceURI))
				{
					if (!this.MoveToNextAttribute())
					{
						this.MoveToParent();
						return false;
					}
				}
				return true;
			}
			return false;
		}

		public virtual bool MoveToNamespace(string name)
		{
			if (this.MoveToFirstNamespace())
			{
				while (!(this.LocalName == name))
				{
					if (!this.MoveToNextNamespace())
					{
						this.MoveToParent();
						return false;
					}
				}
				return true;
			}
			return false;
		}

		public virtual bool MoveToFirst()
		{
			return this.MoveToFirstImpl();
		}

		public virtual void MoveToRoot()
		{
			while (this.MoveToParent())
			{
			}
		}

		internal bool MoveToFirstImpl()
		{
			XPathNodeType nodeType = this.NodeType;
			if (nodeType == XPathNodeType.Attribute || nodeType == XPathNodeType.Namespace)
			{
				return false;
			}
			if (!this.MoveToParent())
			{
				return false;
			}
			this.MoveToFirstChild();
			return true;
		}

		public abstract bool MoveToFirstAttribute();

		public abstract bool MoveToFirstChild();

		public bool MoveToFirstNamespace()
		{
			return this.MoveToFirstNamespace(XPathNamespaceScope.All);
		}

		public abstract bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope);

		public abstract bool MoveToId(string id);

		public abstract bool MoveToNext();

		public abstract bool MoveToNextAttribute();

		public bool MoveToNextNamespace()
		{
			return this.MoveToNextNamespace(XPathNamespaceScope.All);
		}

		public abstract bool MoveToNextNamespace(XPathNamespaceScope namespaceScope);

		public abstract bool MoveToParent();

		public abstract bool MoveToPrevious();

		public virtual XPathNodeIterator Select(string xpath)
		{
			return this.Select(this.Compile(xpath));
		}

		public virtual XPathNodeIterator Select(XPathExpression expr)
		{
			return this.Select(expr, null);
		}

		internal XPathNodeIterator Select(XPathExpression expr, IXmlNamespaceResolver ctx)
		{
			CompiledExpression compiledExpression = (CompiledExpression)expr;
			if (ctx == null)
			{
				ctx = compiledExpression.NamespaceManager;
			}
			BaseIterator baseIterator = new NullIterator(this, ctx);
			return compiledExpression.EvaluateNodeSet(baseIterator);
		}

		public virtual XPathNodeIterator SelectAncestors(XPathNodeType type, bool matchSelf)
		{
			Axes axes = ((!matchSelf) ? Axes.Ancestor : Axes.AncestorOrSelf);
			return this.SelectTest(new NodeTypeTest(axes, type));
		}

		public virtual XPathNodeIterator SelectAncestors(string name, string namespaceURI, bool matchSelf)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (namespaceURI == null)
			{
				throw new ArgumentNullException("namespaceURI");
			}
			Axes axes = ((!matchSelf) ? Axes.Ancestor : Axes.AncestorOrSelf);
			XmlQualifiedName xmlQualifiedName = new XmlQualifiedName(name, namespaceURI);
			return this.SelectTest(new NodeNameTest(axes, xmlQualifiedName, true));
		}

		private static IEnumerable EnumerateChildren(XPathNavigator n, XPathNodeType type)
		{
			if (!n.MoveToFirstChild())
			{
				yield break;
			}
			n.MoveToParent();
			XPathNavigator nav = n.Clone();
			nav.MoveToFirstChild();
			XPathNavigator nav2 = null;
			do
			{
				if (type == XPathNodeType.All || nav.NodeType == type)
				{
					if (nav2 == null)
					{
						nav2 = nav.Clone();
					}
					else
					{
						nav2.MoveTo(nav);
					}
					yield return nav2;
				}
			}
			while (nav.MoveToNext());
			yield break;
		}

		public virtual XPathNodeIterator SelectChildren(XPathNodeType type)
		{
			return new WrapperIterator(new XPathNavigator.EnumerableIterator(XPathNavigator.EnumerateChildren(this, type), 0), null);
		}

		private static IEnumerable EnumerateChildren(XPathNavigator n, string name, string ns)
		{
			if (!n.MoveToFirstChild())
			{
				yield break;
			}
			n.MoveToParent();
			XPathNavigator nav = n.Clone();
			nav.MoveToFirstChild();
			XPathNavigator nav2 = nav.Clone();
			do
			{
				if ((name == string.Empty || nav.LocalName == name) && (ns == string.Empty || nav.NamespaceURI == ns))
				{
					nav2.MoveTo(nav);
					yield return nav2;
				}
			}
			while (nav.MoveToNext());
			yield break;
		}

		public virtual XPathNodeIterator SelectChildren(string name, string namespaceURI)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (namespaceURI == null)
			{
				throw new ArgumentNullException("namespaceURI");
			}
			return new WrapperIterator(new XPathNavigator.EnumerableIterator(XPathNavigator.EnumerateChildren(this, name, namespaceURI), 0), null);
		}

		public virtual XPathNodeIterator SelectDescendants(XPathNodeType type, bool matchSelf)
		{
			Axes axes = ((!matchSelf) ? Axes.Descendant : Axes.DescendantOrSelf);
			return this.SelectTest(new NodeTypeTest(axes, type));
		}

		public virtual XPathNodeIterator SelectDescendants(string name, string namespaceURI, bool matchSelf)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (namespaceURI == null)
			{
				throw new ArgumentNullException("namespaceURI");
			}
			Axes axes = ((!matchSelf) ? Axes.Descendant : Axes.DescendantOrSelf);
			XmlQualifiedName xmlQualifiedName = new XmlQualifiedName(name, namespaceURI);
			return this.SelectTest(new NodeNameTest(axes, xmlQualifiedName, true));
		}

		internal XPathNodeIterator SelectTest(NodeTest test)
		{
			return test.EvaluateNodeSet(new NullIterator(this));
		}

		public override string ToString()
		{
			return this.Value;
		}

		public virtual bool CheckValidity(XmlSchemaSet schemas, ValidationEventHandler handler)
		{
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.NameTable = this.NameTable;
			xmlReaderSettings.SetSchemas(schemas);
			xmlReaderSettings.ValidationEventHandler += handler;
			xmlReaderSettings.ValidationType = ValidationType.Schema;
			try
			{
				XmlReader xmlReader = XmlReader.Create(this.ReadSubtree(), xmlReaderSettings);
				while (!xmlReader.EOF)
				{
					xmlReader.Read();
				}
			}
			catch (XmlSchemaValidationException)
			{
				return false;
			}
			return true;
		}

		public virtual XPathNavigator CreateNavigator()
		{
			return this.Clone();
		}

		public virtual object Evaluate(string xpath, IXmlNamespaceResolver nsResolver)
		{
			return this.Evaluate(this.Compile(xpath), null, nsResolver);
		}

		public virtual IDictionary<string, string> GetNamespacesInScope(XmlNamespaceScope scope)
		{
			IDictionary<string, string> dictionary = new Dictionary<string, string>();
			XPathNamespaceScope xpathNamespaceScope = ((scope != XmlNamespaceScope.Local) ? ((scope != XmlNamespaceScope.ExcludeXml) ? XPathNamespaceScope.All : XPathNamespaceScope.ExcludeXml) : XPathNamespaceScope.Local);
			XPathNavigator xpathNavigator = this.Clone();
			if (xpathNavigator.NodeType != XPathNodeType.Element)
			{
				xpathNavigator.MoveToParent();
			}
			if (!xpathNavigator.MoveToFirstNamespace(xpathNamespaceScope))
			{
				return dictionary;
			}
			do
			{
				dictionary.Add(xpathNavigator.Name, xpathNavigator.Value);
			}
			while (xpathNavigator.MoveToNextNamespace(xpathNamespaceScope));
			return dictionary;
		}

		public virtual string LookupNamespace(string prefix)
		{
			XPathNavigator xpathNavigator = this.Clone();
			if (xpathNavigator.NodeType != XPathNodeType.Element)
			{
				xpathNavigator.MoveToParent();
			}
			if (xpathNavigator.MoveToNamespace(prefix))
			{
				return xpathNavigator.Value;
			}
			return null;
		}

		public virtual string LookupPrefix(string namespaceUri)
		{
			XPathNavigator xpathNavigator = this.Clone();
			if (xpathNavigator.NodeType != XPathNodeType.Element)
			{
				xpathNavigator.MoveToParent();
			}
			if (!xpathNavigator.MoveToFirstNamespace())
			{
				return null;
			}
			while (!(xpathNavigator.Value == namespaceUri))
			{
				if (!xpathNavigator.MoveToNextNamespace())
				{
					return null;
				}
			}
			return xpathNavigator.Name;
		}

		private bool MoveTo(XPathNodeIterator iter)
		{
			if (iter.MoveNext())
			{
				this.MoveTo(iter.Current);
				return true;
			}
			return false;
		}

		public virtual bool MoveToChild(XPathNodeType type)
		{
			return this.MoveTo(this.SelectChildren(type));
		}

		public virtual bool MoveToChild(string localName, string namespaceURI)
		{
			return this.MoveTo(this.SelectChildren(localName, namespaceURI));
		}

		public virtual bool MoveToNext(string localName, string namespaceURI)
		{
			XPathNavigator xpathNavigator = this.Clone();
			while (xpathNavigator.MoveToNext())
			{
				if (xpathNavigator.LocalName == localName && xpathNavigator.NamespaceURI == namespaceURI)
				{
					this.MoveTo(xpathNavigator);
					return true;
				}
			}
			return false;
		}

		public virtual bool MoveToNext(XPathNodeType type)
		{
			XPathNavigator xpathNavigator = this.Clone();
			while (xpathNavigator.MoveToNext())
			{
				if (type == XPathNodeType.All || xpathNavigator.NodeType == type)
				{
					this.MoveTo(xpathNavigator);
					return true;
				}
			}
			return false;
		}

		public virtual bool MoveToFollowing(string localName, string namespaceURI)
		{
			return this.MoveToFollowing(localName, namespaceURI, null);
		}

		public virtual bool MoveToFollowing(string localName, string namespaceURI, XPathNavigator end)
		{
			if (localName == null)
			{
				throw new ArgumentNullException("localName");
			}
			if (namespaceURI == null)
			{
				throw new ArgumentNullException("namespaceURI");
			}
			localName = this.NameTable.Get(localName);
			if (localName == null)
			{
				return false;
			}
			namespaceURI = this.NameTable.Get(namespaceURI);
			if (namespaceURI == null)
			{
				return false;
			}
			XPathNavigator xpathNavigator = this.Clone();
			XPathNodeType nodeType = xpathNavigator.NodeType;
			if (nodeType == XPathNodeType.Attribute || nodeType == XPathNodeType.Namespace)
			{
				xpathNavigator.MoveToParent();
			}
			for (;;)
			{
				if (!xpathNavigator.MoveToFirstChild())
				{
					while (!xpathNavigator.MoveToNext())
					{
						if (!xpathNavigator.MoveToParent())
						{
							return false;
						}
					}
				}
				if (end != null && end.IsSamePosition(xpathNavigator))
				{
					return false;
				}
				if (object.ReferenceEquals(localName, xpathNavigator.LocalName) && object.ReferenceEquals(namespaceURI, xpathNavigator.NamespaceURI))
				{
					goto Block_12;
				}
			}
			return false;
			Block_12:
			this.MoveTo(xpathNavigator);
			return true;
		}

		public virtual bool MoveToFollowing(XPathNodeType type)
		{
			return this.MoveToFollowing(type, null);
		}

		public virtual bool MoveToFollowing(XPathNodeType type, XPathNavigator end)
		{
			if (type == XPathNodeType.Root)
			{
				return false;
			}
			XPathNavigator xpathNavigator = this.Clone();
			XPathNodeType nodeType = xpathNavigator.NodeType;
			if (nodeType == XPathNodeType.Attribute || nodeType == XPathNodeType.Namespace)
			{
				xpathNavigator.MoveToParent();
			}
			for (;;)
			{
				if (!xpathNavigator.MoveToFirstChild())
				{
					while (!xpathNavigator.MoveToNext())
					{
						if (!xpathNavigator.MoveToParent())
						{
							return false;
						}
					}
				}
				if (end != null && end.IsSamePosition(xpathNavigator))
				{
					return false;
				}
				if (type == XPathNodeType.All || xpathNavigator.NodeType == type)
				{
					goto IL_008F;
				}
			}
			return false;
			IL_008F:
			this.MoveTo(xpathNavigator);
			return true;
		}

		public virtual XmlReader ReadSubtree()
		{
			XPathNodeType nodeType = this.NodeType;
			if (nodeType != XPathNodeType.Root && nodeType != XPathNodeType.Element)
			{
				throw new InvalidOperationException(string.Format("NodeType {0} is not supported to read as a subtree of an XPathNavigator.", this.NodeType));
			}
			return new XPathNavigatorReader(this);
		}

		public virtual XPathNodeIterator Select(string xpath, IXmlNamespaceResolver nsResolver)
		{
			return this.Select(this.Compile(xpath), nsResolver);
		}

		public virtual XPathNavigator SelectSingleNode(string xpath)
		{
			return this.SelectSingleNode(xpath, null);
		}

		public virtual XPathNavigator SelectSingleNode(string xpath, IXmlNamespaceResolver nsResolver)
		{
			XPathExpression xpathExpression = this.Compile(xpath);
			xpathExpression.SetContext(nsResolver);
			return this.SelectSingleNode(xpathExpression);
		}

		public virtual XPathNavigator SelectSingleNode(XPathExpression expression)
		{
			XPathNodeIterator xpathNodeIterator = this.Select(expression);
			if (xpathNodeIterator.MoveNext())
			{
				return xpathNodeIterator.Current;
			}
			return null;
		}

		public override object ValueAs(Type type, IXmlNamespaceResolver nsResolver)
		{
			return new XmlAtomicValue(this.Value, XmlSchemaSimpleType.XsString).ValueAs(type, nsResolver);
		}

		public virtual void WriteSubtree(XmlWriter writer)
		{
			writer.WriteNode(this, false);
		}

		private static string EscapeString(string value, bool attr)
		{
			char[] array = ((!attr) ? XPathNavigator.escape_text_chars : XPathNavigator.escape_attr_chars);
			if (value.IndexOfAny(array) < 0)
			{
				return value;
			}
			StringBuilder stringBuilder = new StringBuilder(value, value.Length + 10);
			if (attr)
			{
				stringBuilder.Replace("\"", "&quot;");
			}
			stringBuilder.Replace("<", "&lt;");
			stringBuilder.Replace(">", "&gt;");
			if (attr)
			{
				stringBuilder.Replace("\r\n", "&#10;");
				stringBuilder.Replace("\r", "&#10;");
				stringBuilder.Replace("\n", "&#10;");
			}
			return stringBuilder.ToString();
		}

		public virtual string InnerXml
		{
			get
			{
				switch (this.NodeType)
				{
				case XPathNodeType.Attribute:
				case XPathNodeType.Namespace:
					return XPathNavigator.EscapeString(this.Value, true);
				case XPathNodeType.Text:
				case XPathNodeType.SignificantWhitespace:
				case XPathNodeType.Whitespace:
					return string.Empty;
				case XPathNodeType.ProcessingInstruction:
				case XPathNodeType.Comment:
					return this.Value;
				}
				XmlReader xmlReader = this.ReadSubtree();
				xmlReader.Read();
				int num = xmlReader.Depth;
				if (this.NodeType != XPathNodeType.Root)
				{
					xmlReader.Read();
				}
				else
				{
					num = -1;
				}
				StringWriter stringWriter = new StringWriter();
				XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
				{
					Indent = true,
					ConformanceLevel = ConformanceLevel.Fragment,
					OmitXmlDeclaration = true
				});
				while (!xmlReader.EOF && xmlReader.Depth > num)
				{
					xmlWriter.WriteNode(xmlReader, false);
				}
				return stringWriter.ToString();
			}
			set
			{
				this.DeleteChildren();
				if (this.NodeType == XPathNodeType.Attribute)
				{
					this.SetValue(value);
					return;
				}
				this.AppendChild(value);
			}
		}

		public sealed override bool IsNode
		{
			get
			{
				return true;
			}
		}

		public virtual string OuterXml
		{
			get
			{
				switch (this.NodeType)
				{
				case XPathNodeType.Attribute:
					return string.Concat(new string[]
					{
						this.Prefix,
						(this.Prefix.Length <= 0) ? string.Empty : ":",
						this.LocalName,
						"=\"",
						XPathNavigator.EscapeString(this.Value, true),
						"\""
					});
				case XPathNodeType.Namespace:
					return string.Concat(new string[]
					{
						"xmlns",
						(this.LocalName.Length <= 0) ? string.Empty : ":",
						this.LocalName,
						"=\"",
						XPathNavigator.EscapeString(this.Value, true),
						"\""
					});
				case XPathNodeType.Text:
					return XPathNavigator.EscapeString(this.Value, false);
				case XPathNodeType.SignificantWhitespace:
				case XPathNodeType.Whitespace:
					return this.Value;
				default:
				{
					XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
					xmlWriterSettings.Indent = true;
					xmlWriterSettings.OmitXmlDeclaration = true;
					xmlWriterSettings.ConformanceLevel = ConformanceLevel.Fragment;
					StringBuilder stringBuilder = new StringBuilder();
					using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, xmlWriterSettings))
					{
						this.WriteSubtree(xmlWriter);
					}
					return stringBuilder.ToString();
				}
				}
			}
			set
			{
				switch (this.NodeType)
				{
				case XPathNodeType.Root:
				case XPathNodeType.Attribute:
				case XPathNodeType.Namespace:
					throw new XmlException("Setting OuterXml Root, Attribute and Namespace is not supported.");
				}
				this.DeleteSelf();
				this.AppendChild(value);
				this.MoveToFirstChild();
			}
		}

		public virtual IXmlSchemaInfo SchemaInfo
		{
			get
			{
				return null;
			}
		}

		public override object TypedValue
		{
			get
			{
				XPathNodeType nodeType = this.NodeType;
				if (nodeType == XPathNodeType.Element || nodeType == XPathNodeType.Attribute)
				{
					if (this.XmlType != null)
					{
						XmlSchemaDatatype datatype = this.XmlType.Datatype;
						if (datatype != null)
						{
							return datatype.ParseValue(this.Value, this.NameTable, this);
						}
					}
				}
				return this.Value;
			}
		}

		public virtual object UnderlyingObject
		{
			get
			{
				return null;
			}
		}

		public override bool ValueAsBoolean
		{
			get
			{
				return XQueryConvert.StringToBoolean(this.Value);
			}
		}

		public override DateTime ValueAsDateTime
		{
			get
			{
				return XmlConvert.ToDateTime(this.Value);
			}
		}

		public override double ValueAsDouble
		{
			get
			{
				return XQueryConvert.StringToDouble(this.Value);
			}
		}

		public override int ValueAsInt
		{
			get
			{
				return XQueryConvert.StringToInt(this.Value);
			}
		}

		public override long ValueAsLong
		{
			get
			{
				return XQueryConvert.StringToInteger(this.Value);
			}
		}

		public override Type ValueType
		{
			get
			{
				return (this.SchemaInfo == null || this.SchemaInfo.SchemaType == null || this.SchemaInfo.SchemaType.Datatype == null) ? null : this.SchemaInfo.SchemaType.Datatype.ValueType;
			}
		}

		public override XmlSchemaType XmlType
		{
			get
			{
				if (this.SchemaInfo != null)
				{
					return this.SchemaInfo.SchemaType;
				}
				return null;
			}
		}

		private XmlReader CreateFragmentReader(string fragment)
		{
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(this.NameTable);
			foreach (KeyValuePair<string, string> keyValuePair in this.GetNamespacesInScope(XmlNamespaceScope.All))
			{
				xmlNamespaceManager.AddNamespace(keyValuePair.Key, keyValuePair.Value);
			}
			return XmlReader.Create(new StringReader(fragment), xmlReaderSettings, new XmlParserContext(this.NameTable, xmlNamespaceManager, null, XmlSpace.None));
		}

		public virtual XmlWriter AppendChild()
		{
			throw new NotSupportedException();
		}

		public virtual void AppendChild(string xmlFragments)
		{
			this.AppendChild(this.CreateFragmentReader(xmlFragments));
		}

		public virtual void AppendChild(XmlReader reader)
		{
			XmlWriter xmlWriter = this.AppendChild();
			while (!reader.EOF)
			{
				xmlWriter.WriteNode(reader, false);
			}
			xmlWriter.Close();
		}

		public virtual void AppendChild(XPathNavigator nav)
		{
			this.AppendChild(new XPathNavigatorReader(nav));
		}

		public virtual void AppendChildElement(string prefix, string name, string ns, string value)
		{
			XmlWriter xmlWriter = this.AppendChild();
			xmlWriter.WriteStartElement(prefix, name, ns);
			xmlWriter.WriteString(value);
			xmlWriter.WriteEndElement();
			xmlWriter.Close();
		}

		public virtual void CreateAttribute(string prefix, string localName, string namespaceURI, string value)
		{
			using (XmlWriter xmlWriter = this.CreateAttributes())
			{
				xmlWriter.WriteAttributeString(prefix, localName, namespaceURI, value);
			}
		}

		public virtual XmlWriter CreateAttributes()
		{
			throw new NotSupportedException();
		}

		public virtual void DeleteSelf()
		{
			throw new NotSupportedException();
		}

		public virtual void DeleteRange(XPathNavigator nav)
		{
			throw new NotSupportedException();
		}

		public virtual XmlWriter ReplaceRange(XPathNavigator nav)
		{
			throw new NotSupportedException();
		}

		public virtual XmlWriter InsertAfter()
		{
			switch (this.NodeType)
			{
			case XPathNodeType.Root:
			case XPathNodeType.Attribute:
			case XPathNodeType.Namespace:
				throw new InvalidOperationException(string.Format("Insertion after {0} is not allowed.", this.NodeType));
			}
			XPathNavigator xpathNavigator = this.Clone();
			if (xpathNavigator.MoveToNext())
			{
				return xpathNavigator.InsertBefore();
			}
			if (xpathNavigator.MoveToParent())
			{
				return xpathNavigator.AppendChild();
			}
			throw new InvalidOperationException("Could not move to parent to insert sibling node");
		}

		public virtual void InsertAfter(string xmlFragments)
		{
			this.InsertAfter(this.CreateFragmentReader(xmlFragments));
		}

		public virtual void InsertAfter(XmlReader reader)
		{
			using (XmlWriter xmlWriter = this.InsertAfter())
			{
				xmlWriter.WriteNode(reader, false);
			}
		}

		public virtual void InsertAfter(XPathNavigator nav)
		{
			this.InsertAfter(new XPathNavigatorReader(nav));
		}

		public virtual XmlWriter InsertBefore()
		{
			throw new NotSupportedException();
		}

		public virtual void InsertBefore(string xmlFragments)
		{
			this.InsertBefore(this.CreateFragmentReader(xmlFragments));
		}

		public virtual void InsertBefore(XmlReader reader)
		{
			using (XmlWriter xmlWriter = this.InsertBefore())
			{
				xmlWriter.WriteNode(reader, false);
			}
		}

		public virtual void InsertBefore(XPathNavigator nav)
		{
			this.InsertBefore(new XPathNavigatorReader(nav));
		}

		public virtual void InsertElementAfter(string prefix, string localName, string namespaceURI, string value)
		{
			using (XmlWriter xmlWriter = this.InsertAfter())
			{
				xmlWriter.WriteElementString(prefix, localName, namespaceURI, value);
			}
		}

		public virtual void InsertElementBefore(string prefix, string localName, string namespaceURI, string value)
		{
			using (XmlWriter xmlWriter = this.InsertBefore())
			{
				xmlWriter.WriteElementString(prefix, localName, namespaceURI, value);
			}
		}

		public virtual XmlWriter PrependChild()
		{
			XPathNavigator xpathNavigator = this.Clone();
			if (xpathNavigator.MoveToFirstChild())
			{
				return xpathNavigator.InsertBefore();
			}
			return this.AppendChild();
		}

		public virtual void PrependChild(string xmlFragments)
		{
			this.PrependChild(this.CreateFragmentReader(xmlFragments));
		}

		public virtual void PrependChild(XmlReader reader)
		{
			using (XmlWriter xmlWriter = this.PrependChild())
			{
				xmlWriter.WriteNode(reader, false);
			}
		}

		public virtual void PrependChild(XPathNavigator nav)
		{
			this.PrependChild(new XPathNavigatorReader(nav));
		}

		public virtual void PrependChildElement(string prefix, string localName, string namespaceURI, string value)
		{
			using (XmlWriter xmlWriter = this.PrependChild())
			{
				xmlWriter.WriteElementString(prefix, localName, namespaceURI, value);
			}
		}

		public virtual void ReplaceSelf(string xmlFragment)
		{
			this.ReplaceSelf(this.CreateFragmentReader(xmlFragment));
		}

		public virtual void ReplaceSelf(XmlReader reader)
		{
			throw new NotSupportedException();
		}

		public virtual void ReplaceSelf(XPathNavigator navigator)
		{
			this.ReplaceSelf(new XPathNavigatorReader(navigator));
		}

		[MonoTODO]
		public virtual void SetTypedValue(object value)
		{
			throw new NotSupportedException();
		}

		public virtual void SetValue(string value)
		{
			throw new NotSupportedException();
		}

		private void DeleteChildren()
		{
			switch (this.NodeType)
			{
			case XPathNodeType.Attribute:
				return;
			case XPathNodeType.Namespace:
				throw new InvalidOperationException("Removing namespace node content is not supported.");
			case XPathNodeType.Text:
			case XPathNodeType.SignificantWhitespace:
			case XPathNodeType.Whitespace:
			case XPathNodeType.ProcessingInstruction:
			case XPathNodeType.Comment:
				this.DeleteSelf();
				return;
			default:
			{
				if (!this.HasChildren)
				{
					return;
				}
				XPathNavigator xpathNavigator = this.Clone();
				xpathNavigator.MoveToFirstChild();
				while (!xpathNavigator.IsSamePosition(this))
				{
					xpathNavigator.DeleteSelf();
				}
				return;
			}
			}
		}

		private static readonly char[] escape_text_chars = new char[] { '&', '<', '>' };

		private static readonly char[] escape_attr_chars = new char[] { '"', '&', '<', '>', '\r', '\n' };

		private class EnumerableIterator : XPathNodeIterator
		{
			public EnumerableIterator(IEnumerable source, int pos)
			{
				this.source = source;
				for (int i = 0; i < pos; i++)
				{
					this.MoveNext();
				}
			}

			public override XPathNodeIterator Clone()
			{
				return new XPathNavigator.EnumerableIterator(this.source, this.pos);
			}

			public override bool MoveNext()
			{
				if (this.e == null)
				{
					this.e = this.source.GetEnumerator();
				}
				if (!this.e.MoveNext())
				{
					return false;
				}
				this.pos++;
				return true;
			}

			public override int CurrentPosition
			{
				get
				{
					return this.pos;
				}
			}

			public override XPathNavigator Current
			{
				get
				{
					return (this.pos != 0) ? ((XPathNavigator)this.e.Current) : null;
				}
			}

			private IEnumerable source;

			private IEnumerator e;

			private int pos;
		}
	}
}
