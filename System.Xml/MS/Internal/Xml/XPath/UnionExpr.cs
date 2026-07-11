using System;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace MS.Internal.Xml.XPath
{
	internal sealed class UnionExpr : Query
	{
		public UnionExpr(Query query1, Query query2)
		{
			this.qy1 = query1;
			this.qy2 = query2;
			this.advance1 = true;
			this.advance2 = true;
		}

		private UnionExpr(UnionExpr other)
			: base(other)
		{
			this.qy1 = Query.Clone(other.qy1);
			this.qy2 = Query.Clone(other.qy2);
			this.advance1 = other.advance1;
			this.advance2 = other.advance2;
			this.currentNode = Query.Clone(other.currentNode);
			this.nextNode = Query.Clone(other.nextNode);
		}

		public override void Reset()
		{
			this.qy1.Reset();
			this.qy2.Reset();
			this.advance1 = true;
			this.advance2 = true;
			this.nextNode = null;
		}

		public override void SetXsltContext(XsltContext xsltContext)
		{
			this.qy1.SetXsltContext(xsltContext);
			this.qy2.SetXsltContext(xsltContext);
		}

		public override object Evaluate(XPathNodeIterator context)
		{
			this.qy1.Evaluate(context);
			this.qy2.Evaluate(context);
			this.advance1 = true;
			this.advance2 = true;
			this.nextNode = null;
			base.ResetCount();
			return this;
		}

		private XPathNavigator ProcessSamePosition(XPathNavigator result)
		{
			this.currentNode = result;
			this.advance1 = (this.advance2 = true);
			return result;
		}

		private XPathNavigator ProcessBeforePosition(XPathNavigator res1, XPathNavigator res2)
		{
			this.nextNode = res2;
			this.advance2 = false;
			this.advance1 = true;
			this.currentNode = res1;
			return res1;
		}

		private XPathNavigator ProcessAfterPosition(XPathNavigator res1, XPathNavigator res2)
		{
			this.nextNode = res1;
			this.advance1 = false;
			this.advance2 = true;
			this.currentNode = res2;
			return res2;
		}

		public override XPathNavigator Advance()
		{
			XPathNavigator xpathNavigator;
			if (this.advance1)
			{
				xpathNavigator = this.qy1.Advance();
			}
			else
			{
				xpathNavigator = this.nextNode;
			}
			XPathNavigator xpathNavigator2;
			if (this.advance2)
			{
				xpathNavigator2 = this.qy2.Advance();
			}
			else
			{
				xpathNavigator2 = this.nextNode;
			}
			if (xpathNavigator != null && xpathNavigator2 != null)
			{
				XmlNodeOrder xmlNodeOrder = Query.CompareNodes(xpathNavigator, xpathNavigator2);
				if (xmlNodeOrder == XmlNodeOrder.Before)
				{
					return this.ProcessBeforePosition(xpathNavigator, xpathNavigator2);
				}
				if (xmlNodeOrder == XmlNodeOrder.After)
				{
					return this.ProcessAfterPosition(xpathNavigator, xpathNavigator2);
				}
				return this.ProcessSamePosition(xpathNavigator);
			}
			else
			{
				if (xpathNavigator2 == null)
				{
					this.advance1 = true;
					this.advance2 = false;
					this.currentNode = xpathNavigator;
					this.nextNode = null;
					return xpathNavigator;
				}
				this.advance1 = false;
				this.advance2 = true;
				this.currentNode = xpathNavigator2;
				this.nextNode = null;
				return xpathNavigator2;
			}
		}

		public override XPathNavigator MatchNode(XPathNavigator xsltContext)
		{
			if (xsltContext == null)
			{
				return null;
			}
			XPathNavigator xpathNavigator = this.qy1.MatchNode(xsltContext);
			if (xpathNavigator != null)
			{
				return xpathNavigator;
			}
			return this.qy2.MatchNode(xsltContext);
		}

		public override XPathResultType StaticType
		{
			get
			{
				return XPathResultType.NodeSet;
			}
		}

		public override XPathNodeIterator Clone()
		{
			return new UnionExpr(this);
		}

		public override XPathNavigator Current
		{
			get
			{
				return this.currentNode;
			}
		}

		public override int CurrentPosition
		{
			get
			{
				throw new InvalidOperationException();
			}
		}

		public override void PrintQuery(XmlWriter w)
		{
			w.WriteStartElement(base.GetType().Name);
			if (this.qy1 != null)
			{
				this.qy1.PrintQuery(w);
			}
			if (this.qy2 != null)
			{
				this.qy2.PrintQuery(w);
			}
			w.WriteEndElement();
		}

		internal Query qy1;

		internal Query qy2;

		private bool advance1;

		private bool advance2;

		private XPathNavigator currentNode;

		private XPathNavigator nextNode;
	}
}
