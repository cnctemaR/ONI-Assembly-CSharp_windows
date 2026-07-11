using System;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace MS.Internal.Xml.XPath
{
	internal sealed class NodeFunctions : ValueQuery
	{
		public NodeFunctions(Function.FunctionType funcType, Query arg)
		{
			this.funcType = funcType;
			this.arg = arg;
		}

		public override void SetXsltContext(XsltContext context)
		{
			this.xsltContext = (context.Whitespace ? context : null);
			if (this.arg != null)
			{
				this.arg.SetXsltContext(context);
			}
		}

		private XPathNavigator EvaluateArg(XPathNodeIterator context)
		{
			if (this.arg == null)
			{
				return context.Current;
			}
			this.arg.Evaluate(context);
			return this.arg.Advance();
		}

		public override object Evaluate(XPathNodeIterator context)
		{
			switch (this.funcType)
			{
			case Function.FunctionType.FuncLast:
				return (double)context.Count;
			case Function.FunctionType.FuncPosition:
				return (double)context.CurrentPosition;
			case Function.FunctionType.FuncCount:
			{
				this.arg.Evaluate(context);
				int num = 0;
				if (this.xsltContext != null)
				{
					XPathNavigator xpathNavigator;
					while ((xpathNavigator = this.arg.Advance()) != null)
					{
						if (xpathNavigator.NodeType != XPathNodeType.Whitespace || this.xsltContext.PreserveWhitespace(xpathNavigator))
						{
							num++;
						}
					}
				}
				else
				{
					while (this.arg.Advance() != null)
					{
						num++;
					}
				}
				return (double)num;
			}
			case Function.FunctionType.FuncLocalName:
			{
				XPathNavigator xpathNavigator2 = this.EvaluateArg(context);
				if (xpathNavigator2 != null)
				{
					return xpathNavigator2.LocalName;
				}
				break;
			}
			case Function.FunctionType.FuncNameSpaceUri:
			{
				XPathNavigator xpathNavigator2 = this.EvaluateArg(context);
				if (xpathNavigator2 != null)
				{
					return xpathNavigator2.NamespaceURI;
				}
				break;
			}
			case Function.FunctionType.FuncName:
			{
				XPathNavigator xpathNavigator2 = this.EvaluateArg(context);
				if (xpathNavigator2 != null)
				{
					return xpathNavigator2.Name;
				}
				break;
			}
			}
			return string.Empty;
		}

		public override XPathResultType StaticType
		{
			get
			{
				return Function.ReturnTypes[(int)this.funcType];
			}
		}

		public override XPathNodeIterator Clone()
		{
			return new NodeFunctions(this.funcType, Query.Clone(this.arg))
			{
				xsltContext = this.xsltContext
			};
		}

		public override void PrintQuery(XmlWriter w)
		{
			w.WriteStartElement(base.GetType().Name);
			w.WriteAttributeString("name", this.funcType.ToString());
			if (this.arg != null)
			{
				this.arg.PrintQuery(w);
			}
			w.WriteEndElement();
		}

		private Query arg;

		private Function.FunctionType funcType;

		private XsltContext xsltContext;
	}
}
