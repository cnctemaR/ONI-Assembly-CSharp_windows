using System;
using System.Collections;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace MS.Internal.Xml.XPath
{
	internal class CompiledXpathExpr : XPathExpression
	{
		internal CompiledXpathExpr(Query query, string expression, bool needContext)
		{
			this._query = query;
			this._expr = expression;
			this._needContext = needContext;
		}

		internal Query QueryTree
		{
			get
			{
				if (this._needContext)
				{
					throw XPathException.Create("Namespace Manager or XsltContext needed. This query has a prefix, variable, or user-defined function.");
				}
				return this._query;
			}
		}

		public override string Expression
		{
			get
			{
				return this._expr;
			}
		}

		public virtual void CheckErrors()
		{
		}

		public override void AddSort(object expr, IComparer comparer)
		{
			string text = expr as string;
			Query query;
			if (text != null)
			{
				query = new QueryBuilder().Build(text, out this._needContext);
			}
			else
			{
				CompiledXpathExpr compiledXpathExpr = expr as CompiledXpathExpr;
				if (compiledXpathExpr == null)
				{
					throw XPathException.Create("This is an invalid object. Only objects returned from Compile() can be passed as input.");
				}
				query = compiledXpathExpr.QueryTree;
			}
			SortQuery sortQuery = this._query as SortQuery;
			if (sortQuery == null)
			{
				sortQuery = (this._query = new SortQuery(this._query));
			}
			sortQuery.AddSort(query, comparer);
		}

		public override void AddSort(object expr, XmlSortOrder order, XmlCaseOrder caseOrder, string lang, XmlDataType dataType)
		{
			this.AddSort(expr, new XPathComparerHelper(order, caseOrder, lang, dataType));
		}

		public override XPathExpression Clone()
		{
			return new CompiledXpathExpr(Query.Clone(this._query), this._expr, this._needContext);
		}

		public override void SetContext(XmlNamespaceManager nsManager)
		{
			this.SetContext(nsManager);
		}

		public override void SetContext(IXmlNamespaceResolver nsResolver)
		{
			XsltContext xsltContext = nsResolver as XsltContext;
			if (xsltContext == null)
			{
				if (nsResolver == null)
				{
					nsResolver = new XmlNamespaceManager(new NameTable());
				}
				xsltContext = new CompiledXpathExpr.UndefinedXsltContext(nsResolver);
			}
			this._query.SetXsltContext(xsltContext);
			this._needContext = false;
		}

		public override XPathResultType ReturnType
		{
			get
			{
				return this._query.StaticType;
			}
		}

		private Query _query;

		private string _expr;

		private bool _needContext;

		private class UndefinedXsltContext : XsltContext
		{
			public UndefinedXsltContext(IXmlNamespaceResolver nsResolver)
				: base(false)
			{
				this._nsResolver = nsResolver;
			}

			public override string DefaultNamespace
			{
				get
				{
					return string.Empty;
				}
			}

			public override string LookupNamespace(string prefix)
			{
				if (prefix.Length == 0)
				{
					return string.Empty;
				}
				string text = this._nsResolver.LookupNamespace(prefix);
				if (text == null)
				{
					throw XPathException.Create("Namespace prefix '{0}' is not defined.", prefix);
				}
				return text;
			}

			public override IXsltContextVariable ResolveVariable(string prefix, string name)
			{
				throw XPathException.Create("XsltContext is needed for this query because of an unknown function.");
			}

			public override IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] ArgTypes)
			{
				throw XPathException.Create("XsltContext is needed for this query because of an unknown function.");
			}

			public override bool Whitespace
			{
				get
				{
					return false;
				}
			}

			public override bool PreserveWhitespace(XPathNavigator node)
			{
				return false;
			}

			public override int CompareDocument(string baseUri, string nextbaseUri)
			{
				return string.CompareOrdinal(baseUri, nextbaseUri);
			}

			private IXmlNamespaceResolver _nsResolver;
		}
	}
}
