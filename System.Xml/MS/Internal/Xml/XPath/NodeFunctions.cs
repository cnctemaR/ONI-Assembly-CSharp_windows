using System;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace MS.Internal.Xml.XPath
{
	internal sealed class NodeFunctions : ValueQuery
	{
		public NodeFunctions(Function.FunctionType funcType, Query arg)
		{
			this._funcType = funcType;
			this._arg = arg;
		}

		public override void SetXsltContext(XsltContext context)
		{
			this._xsltContext = (context.Whitespace ? context : null);
			if (this._arg != null)
			{
				this._arg.SetXsltContext(context);
			}
		}

		private XPathNavigator EvaluateArg(XPathNodeIterator context)
		{
			if (this._arg == null)
			{
				return context.Current;
			}
			this._arg.Evaluate(context);
			return this._arg.Advance();
		}

		public override object Evaluate(XPathNodeIterator context)
		{
			switch (this._funcType)
			{
			case Function.FunctionType.FuncLast:
				return (double)context.Count;
			case Function.FunctionType.FuncPosition:
				return (double)context.CurrentPosition;
			case Function.FunctionType.FuncCount:
			{
				this._arg.Evaluate(context);
				int num = 0;
				if (this._xsltContext != null)
				{
					XPathNavigator xpathNavigator;
					while ((xpathNavigator = this._arg.Advance()) != null)
					{
						if (xpathNavigator.NodeType != XPathNodeType.Whitespace || this._xsltContext.PreserveWhitespace(xpathNavigator))
						{
							num++;
						}
					}
				}
				else
				{
					while (this._arg.Advance() != null)
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
				return Function.ReturnTypes[(int)this._funcType];
			}
		}

		public override XPathNodeIterator Clone()
		{
			return new NodeFunctions(this._funcType, Query.Clone(this._arg))
			{
				_xsltContext = this._xsltContext
			};
		}

		private Query _arg;

		private Function.FunctionType _funcType;

		private XsltContext _xsltContext;
	}
}
