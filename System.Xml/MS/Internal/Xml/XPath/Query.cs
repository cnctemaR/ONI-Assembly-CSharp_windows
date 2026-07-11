using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace MS.Internal.Xml.XPath
{
	[DebuggerDisplay("{ToString()}")]
	internal abstract class Query : ResetableIterator
	{
		public Query()
		{
		}

		protected Query(Query other)
			: base(other)
		{
		}

		public override bool MoveNext()
		{
			return this.Advance() != null;
		}

		public override int Count
		{
			get
			{
				if (this.count == -1)
				{
					Query query = (Query)this.Clone();
					query.Reset();
					this.count = 0;
					while (query.MoveNext())
					{
						this.count++;
					}
				}
				return this.count;
			}
		}

		public virtual void SetXsltContext(XsltContext context)
		{
		}

		public abstract object Evaluate(XPathNodeIterator nodeIterator);

		public abstract XPathNavigator Advance();

		public virtual XPathNavigator MatchNode(XPathNavigator current)
		{
			throw XPathException.Create("'{0}' is an invalid XSLT pattern.");
		}

		public virtual double XsltDefaultPriority
		{
			get
			{
				return 0.5;
			}
		}

		public abstract XPathResultType StaticType { get; }

		public virtual QueryProps Properties
		{
			get
			{
				return QueryProps.Merge;
			}
		}

		public static Query Clone(Query input)
		{
			if (input != null)
			{
				return (Query)input.Clone();
			}
			return null;
		}

		protected static XPathNodeIterator Clone(XPathNodeIterator input)
		{
			if (input != null)
			{
				return input.Clone();
			}
			return null;
		}

		protected static XPathNavigator Clone(XPathNavigator input)
		{
			if (input != null)
			{
				return input.Clone();
			}
			return null;
		}

		public bool Insert(List<XPathNavigator> buffer, XPathNavigator nav)
		{
			int i = 0;
			int num = buffer.Count;
			if (num != 0)
			{
				XmlNodeOrder xmlNodeOrder = Query.CompareNodes(buffer[num - 1], nav);
				if (xmlNodeOrder == XmlNodeOrder.Before)
				{
					buffer.Add(nav.Clone());
					return true;
				}
				if (xmlNodeOrder == XmlNodeOrder.Same)
				{
					return false;
				}
				num--;
			}
			while (i < num)
			{
				int median = Query.GetMedian(i, num);
				XmlNodeOrder xmlNodeOrder = Query.CompareNodes(buffer[median], nav);
				if (xmlNodeOrder != XmlNodeOrder.Before)
				{
					if (xmlNodeOrder == XmlNodeOrder.Same)
					{
						return false;
					}
					num = median;
				}
				else
				{
					i = median + 1;
				}
			}
			buffer.Insert(i, nav.Clone());
			return true;
		}

		private static int GetMedian(int l, int r)
		{
			return (int)((uint)(l + r) >> 1);
		}

		public static XmlNodeOrder CompareNodes(XPathNavigator l, XPathNavigator r)
		{
			XmlNodeOrder xmlNodeOrder = l.ComparePosition(r);
			if (xmlNodeOrder == XmlNodeOrder.Unknown)
			{
				XPathNavigator xpathNavigator = l.Clone();
				xpathNavigator.MoveToRoot();
				string baseURI = xpathNavigator.BaseURI;
				if (!xpathNavigator.MoveTo(r))
				{
					xpathNavigator = r.Clone();
				}
				xpathNavigator.MoveToRoot();
				string baseURI2 = xpathNavigator.BaseURI;
				int num = string.CompareOrdinal(baseURI, baseURI2);
				xmlNodeOrder = ((num < 0) ? XmlNodeOrder.Before : ((num > 0) ? XmlNodeOrder.After : XmlNodeOrder.Unknown));
			}
			return xmlNodeOrder;
		}

		[Conditional("DEBUG")]
		private void AssertDOD(List<XPathNavigator> buffer, XPathNavigator nav, int pos)
		{
			if (nav.GetType().ToString() == "Microsoft.VisualStudio.Modeling.StoreNavigator")
			{
				return;
			}
			if (nav.GetType().ToString() == "System.Xml.DataDocumentXPathNavigator")
			{
				return;
			}
			if (0 < pos)
			{
				Query.CompareNodes(buffer[pos - 1], nav);
			}
			if (pos < buffer.Count)
			{
				Query.CompareNodes(nav, buffer[pos]);
			}
		}

		[Conditional("DEBUG")]
		public static void AssertQuery(Query query)
		{
			if (query is FunctionQuery)
			{
				return;
			}
			query = Query.Clone(query);
			XPathNavigator xpathNavigator = null;
			int count = query.Clone().Count;
			int num = 0;
			XPathNavigator xpathNavigator2;
			while ((xpathNavigator2 = query.Advance()) != null)
			{
				if (xpathNavigator2.GetType().ToString() == "Microsoft.VisualStudio.Modeling.StoreNavigator")
				{
					return;
				}
				if (xpathNavigator2.GetType().ToString() == "System.Xml.DataDocumentXPathNavigator")
				{
					return;
				}
				if (xpathNavigator != null && (xpathNavigator.NodeType != XPathNodeType.Namespace || xpathNavigator2.NodeType != XPathNodeType.Namespace))
				{
					Query.CompareNodes(xpathNavigator, xpathNavigator2);
				}
				xpathNavigator = xpathNavigator2.Clone();
				num++;
			}
		}

		protected XPathResultType GetXPathType(object value)
		{
			if (value is XPathNodeIterator)
			{
				return XPathResultType.NodeSet;
			}
			if (value is string)
			{
				return XPathResultType.String;
			}
			if (value is double)
			{
				return XPathResultType.Number;
			}
			if (value is bool)
			{
				return XPathResultType.Boolean;
			}
			return (XPathResultType)4;
		}

		public virtual void PrintQuery(XmlWriter w)
		{
			w.WriteElementString(base.GetType().Name, string.Empty);
		}

		public const XPathResultType XPathResultType_Navigator = (XPathResultType)4;
	}
}
