using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace MS.Internal.Xml.XPath
{
	internal sealed class FunctionQuery : ExtensionQuery
	{
		public FunctionQuery(string prefix, string name, List<Query> args)
			: base(prefix, name)
		{
			this.args = args;
		}

		private FunctionQuery(FunctionQuery other)
			: base(other)
		{
			this.function = other.function;
			Query[] array = new Query[other.args.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = Query.Clone(other.args[i]);
			}
			this.args = array;
			this.args = array;
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
				foreach (Query query in this.args)
				{
					query.SetXsltContext(context);
				}
				XPathResultType[] array = new XPathResultType[this.args.Count];
				for (int i = 0; i < this.args.Count; i++)
				{
					array[i] = this.args[i].StaticType;
				}
				this.function = this.xsltContext.ResolveFunction(this.prefix, this.name, array);
				if (this.function == null)
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
			object[] array = new object[this.args.Count];
			for (int i = 0; i < this.args.Count; i++)
			{
				array[i] = this.args[i].Evaluate(nodeIterator);
				if (array[i] is XPathNodeIterator)
				{
					array[i] = new XPathSelectionIterator(nodeIterator.Current, this.args[i]);
				}
			}
			object obj;
			try
			{
				obj = base.ProcessResult(this.function.Invoke(this.xsltContext, array, nodeIterator.Current));
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
				XPathResultType xpathResultType = ((this.function != null) ? this.function.ReturnType : XPathResultType.Any);
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

		public override void PrintQuery(XmlWriter w)
		{
			w.WriteStartElement(base.GetType().Name);
			w.WriteAttributeString("name", (this.prefix.Length != 0) ? (this.prefix + ":" + this.name) : this.name);
			foreach (Query query in this.args)
			{
				query.PrintQuery(w);
			}
			w.WriteEndElement();
		}

		private IList<Query> args;

		private IXsltContextFunction function;
	}
}
