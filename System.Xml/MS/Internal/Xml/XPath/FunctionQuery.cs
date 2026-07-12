using System;
using System.Collections.Generic;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace MS.Internal.Xml.XPath
{
	internal sealed class FunctionQuery : ExtensionQuery
	{
		public FunctionQuery(string prefix, string name, List<Query> args)
			: base(prefix, name)
		{
			this._args = args;
		}

		private FunctionQuery(FunctionQuery other)
			: base(other)
		{
			this._function = other._function;
			Query[] array = new Query[other._args.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = Query.Clone(other._args[i]);
			}
			this._args = array;
			this._args = array;
		}

		public override void SetXsltContext(XsltContext context)
		{
			if (context == null)
			{
				throw XPathException.Create("Namespace Manager or XsltContext needed. This query has a prefix, variable, or user-defined function.");
			}
			if (this.xsltContext != context)
			{
				this.xsltContext = context;
				foreach (Query query in this._args)
				{
					query.SetXsltContext(context);
				}
				XPathResultType[] array = new XPathResultType[this._args.Count];
				for (int i = 0; i < this._args.Count; i++)
				{
					array[i] = this._args[i].StaticType;
				}
				this._function = this.xsltContext.ResolveFunction(this.prefix, this.name, array);
				if (this._function == null)
				{
					throw XPathException.Create("The function '{0}()' is undefined.", base.QName);
				}
			}
		}

		public override object Evaluate(XPathNodeIterator nodeIterator)
		{
			if (this.xsltContext == null)
			{
				throw XPathException.Create("Namespace Manager or XsltContext needed. This query has a prefix, variable, or user-defined function.");
			}
			object[] array = new object[this._args.Count];
			for (int i = 0; i < this._args.Count; i++)
			{
				array[i] = this._args[i].Evaluate(nodeIterator);
				if (array[i] is XPathNodeIterator)
				{
					array[i] = new XPathSelectionIterator(nodeIterator.Current, this._args[i]);
				}
			}
			object obj;
			try
			{
				obj = base.ProcessResult(this._function.Invoke(this.xsltContext, array, nodeIterator.Current));
			}
			catch (Exception ex)
			{
				throw XPathException.Create("Function '{0}()' has failed.", base.QName, ex);
			}
			return obj;
		}

		public override XPathNavigator MatchNode(XPathNavigator navigator)
		{
			if (this.name != "key" && this.prefix.Length != 0)
			{
				throw XPathException.Create("'{0}' is an invalid XSLT pattern.");
			}
			this.Evaluate(new XPathSingletonIterator(navigator, true));
			XPathNavigator xpathNavigator;
			while ((xpathNavigator = this.Advance()) != null)
			{
				if (xpathNavigator.IsSamePosition(navigator))
				{
					return xpathNavigator;
				}
			}
			return xpathNavigator;
		}

		public override XPathResultType StaticType
		{
			get
			{
				XPathResultType xpathResultType = ((this._function != null) ? this._function.ReturnType : XPathResultType.Any);
				if (xpathResultType == XPathResultType.Error)
				{
					xpathResultType = XPathResultType.Any;
				}
				return xpathResultType;
			}
		}

		public override XPathNodeIterator Clone()
		{
			return new FunctionQuery(this);
		}

		private IList<Query> _args;

		private IXsltContextFunction _function;
	}
}
