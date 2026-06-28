using System;
using System.Xml.XPath;
using System.Xml.Xsl;
using Mono.Xml.Xsl;

namespace Mono.Xml.XPath
{
	internal class LocationPathPattern : Pattern
	{
		public LocationPathPattern(NodeTest nodeTest)
		{
			this.nodeTest = nodeTest;
		}

		public LocationPathPattern(ExprFilter filter)
		{
			this.filter = filter;
			while (!(filter.expr is NodeTest))
			{
				filter = (ExprFilter)filter.expr;
			}
			this.nodeTest = (NodeTest)filter.expr;
		}

		internal void SetPreviousPattern(Pattern prev, bool isAncestor)
		{
			LocationPathPattern lastPathPattern = this.LastPathPattern;
			lastPathPattern.patternPrevious = (LocationPathPattern)prev;
			lastPathPattern.isAncestor = isAncestor;
		}

		public override double DefaultPriority
		{
			get
			{
				if (this.patternPrevious != null || this.filter != null)
				{
					return 0.5;
				}
				NodeNameTest nodeNameTest = this.nodeTest as NodeNameTest;
				if (nodeNameTest == null)
				{
					return -0.5;
				}
				if (nodeNameTest.Name.Name == "*" || nodeNameTest.Name.Name.Length == 0)
				{
					return -0.25;
				}
				return 0.0;
			}
		}

		public override XPathNodeType EvaluatedNodeType
		{
			get
			{
				return this.nodeTest.EvaluatedNodeType;
			}
		}

		public override bool Matches(XPathNavigator node, XsltContext ctx)
		{
			if (!this.nodeTest.Match(ctx, node))
			{
				return false;
			}
			if (this.nodeTest is NodeTypeTest && ((NodeTypeTest)this.nodeTest).type == XPathNodeType.All && (node.NodeType == XPathNodeType.Root || node.NodeType == XPathNodeType.Attribute))
			{
				return false;
			}
			if (this.filter == null && this.patternPrevious == null)
			{
				return true;
			}
			XPathNavigator xpathNavigator;
			if (this.patternPrevious != null)
			{
				xpathNavigator = ((XsltCompiledContext)ctx).GetNavCache(this, node);
				if (this.isAncestor)
				{
					while (xpathNavigator.MoveToParent())
					{
						if (this.patternPrevious.Matches(xpathNavigator, ctx))
						{
							goto IL_00D9;
						}
					}
					return false;
				}
				xpathNavigator.MoveToParent();
				if (!this.patternPrevious.Matches(xpathNavigator, ctx))
				{
					return false;
				}
			}
			IL_00D9:
			if (this.filter == null)
			{
				return true;
			}
			if (!this.filter.IsPositional && !(this.filter.expr is ExprFilter))
			{
				return this.filter.pred.EvaluateBoolean(new NullIterator(node, ctx));
			}
			xpathNavigator = ((XsltCompiledContext)ctx).GetNavCache(this, node);
			xpathNavigator.MoveToParent();
			BaseIterator baseIterator = this.filter.EvaluateNodeSet(new NullIterator(xpathNavigator, ctx));
			while (baseIterator.MoveNext())
			{
				if (node.IsSamePosition(baseIterator.Current))
				{
					return true;
				}
			}
			return false;
		}

		public override string ToString()
		{
			string text = string.Empty;
			if (this.patternPrevious != null)
			{
				text = this.patternPrevious.ToString() + ((!this.isAncestor) ? "/" : "//");
			}
			if (this.filter != null)
			{
				text += this.filter.ToString();
			}
			else
			{
				text += this.nodeTest.ToString();
			}
			return text;
		}

		public LocationPathPattern LastPathPattern
		{
			get
			{
				LocationPathPattern locationPathPattern = this;
				while (locationPathPattern.patternPrevious != null)
				{
					locationPathPattern = locationPathPattern.patternPrevious;
				}
				return locationPathPattern;
			}
		}

		private LocationPathPattern patternPrevious;

		private bool isAncestor;

		private NodeTest nodeTest;

		private ExprFilter filter;
	}
}
