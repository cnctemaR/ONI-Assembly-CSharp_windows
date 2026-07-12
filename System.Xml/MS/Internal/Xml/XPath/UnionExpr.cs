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
			this._advance1 = true;
			this._advance2 = true;
		}

		private UnionExpr(UnionExpr other)
			: base(other)
		{
			this.qy1 = Query.Clone(other.qy1);
			this.qy2 = Query.Clone(other.qy2);
			this._advance1 = other._advance1;
			this._advance2 = other._advance2;
			this._currentNode = Query.Clone(other._currentNode);
			this._nextNode = Query.Clone(other._nextNode);
		}

		public override void Reset()
		{
			this.qy1.Reset();
			this.qy2.Reset();
			this._advance1 = true;
			this._advance2 = true;
			this._nextNode = null;
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
			this._advance1 = true;
			this._advance2 = true;
			this._nextNode = null;
			base.ResetCount();
			return this;
		}

		private XPathNavigator ProcessSamePosition(XPathNavigator result)
		{
			this._currentNode = result;
			this._advance1 = (this._advance2 = true);
			return result;
		}

		private XPathNavigator ProcessBeforePosition(XPathNavigator res1, XPathNavigator res2)
		{
			this._nextNode = res2;
			this._advance2 = false;
			this._advance1 = true;
			this._currentNode = res1;
			return res1;
		}

		private XPathNavigator ProcessAfterPosition(XPathNavigator res1, XPathNavigator res2)
		{
			this._nextNode = res1;
			this._advance1 = false;
			this._advance2 = true;
			this._currentNode = res2;
			return res2;
		}

		public override XPathNavigator Advance()
		{
			XPathNavigator xpathNavigator;
			if (this._advance1)
			{
				xpathNavigator = this.qy1.Advance();
			}
			else
			{
				xpathNavigator = this._nextNode;
			}
			XPathNavigator xpathNavigator2;
			if (this._advance2)
			{
				xpathNavigator2 = this.qy2.Advance();
			}
			else
			{
				xpathNavigator2 = this._nextNode;
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
					this._advance1 = true;
					this._advance2 = false;
					this._currentNode = xpathNavigator;
					this._nextNode = null;
					return xpathNavigator;
				}
				this._advance1 = false;
				this._advance2 = true;
				this._currentNode = xpathNavigator2;
				this._nextNode = null;
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
				return this._currentNode;
			}
		}

		public override int CurrentPosition
		{
			get
			{
				throw new InvalidOperationException();
			}
		}

		internal Query qy1;

		internal Query qy2;

		private bool _advance1;

		private bool _advance2;

		private XPathNavigator _currentNode;

		private XPathNavigator _nextNode;
	}
}
