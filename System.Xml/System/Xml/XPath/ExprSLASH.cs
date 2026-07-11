using System;

namespace System.Xml.XPath
{
	internal class ExprSLASH : NodeSet
	{
		public ExprSLASH(Expression left, NodeSet right)
		{
			this.left = left;
			this.right = right;
		}

		public override Expression Optimize()
		{
			this.left = this.left.Optimize();
			this.right = (NodeSet)this.right.Optimize();
			return this;
		}

		public override string ToString()
		{
			return this.left.ToString() + "/" + this.right.ToString();
		}

		public override object Evaluate(BaseIterator iter)
		{
			BaseIterator baseIterator = this.left.EvaluateNodeSet(iter);
			if (this.left.Peer && this.right.Subtree)
			{
				return new SimpleSlashIterator(baseIterator, this.right);
			}
			BaseIterator baseIterator2 = new SlashIterator(baseIterator, this.right);
			return new SortedIterator(baseIterator2);
		}

		public override bool RequireSorting
		{
			get
			{
				return this.left.RequireSorting || this.right.RequireSorting;
			}
		}

		internal override XPathNodeType EvaluatedNodeType
		{
			get
			{
				return this.right.EvaluatedNodeType;
			}
		}

		internal override bool IsPositional
		{
			get
			{
				return this.left.IsPositional || this.right.IsPositional;
			}
		}

		internal override bool Peer
		{
			get
			{
				return this.left.Peer && this.right.Peer;
			}
		}

		internal override bool Subtree
		{
			get
			{
				NodeSet nodeSet = this.left as NodeSet;
				return nodeSet != null && nodeSet.Subtree && this.right.Subtree;
			}
		}

		public Expression left;

		public NodeSet right;
	}
}
