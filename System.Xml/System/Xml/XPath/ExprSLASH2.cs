using System;

namespace System.Xml.XPath
{
	internal class ExprSLASH2 : NodeSet
	{
		public ExprSLASH2(Expression left, NodeSet right)
		{
			this.left = left;
			this.right = right;
		}

		public override Expression Optimize()
		{
			this.left = this.left.Optimize();
			this.right = (NodeSet)this.right.Optimize();
			NodeTest nodeTest = this.right as NodeTest;
			if (nodeTest != null && nodeTest.Axis.Axis == Axes.Child)
			{
				NodeNameTest nodeNameTest = nodeTest as NodeNameTest;
				if (nodeNameTest != null)
				{
					return new ExprSLASH(this.left, new NodeNameTest(nodeNameTest, Axes.Descendant));
				}
				NodeTypeTest nodeTypeTest = nodeTest as NodeTypeTest;
				if (nodeTypeTest != null)
				{
					return new ExprSLASH(this.left, new NodeTypeTest(nodeTypeTest, Axes.Descendant));
				}
			}
			return this;
		}

		public override string ToString()
		{
			return this.left.ToString() + "//" + this.right.ToString();
		}

		public override object Evaluate(BaseIterator iter)
		{
			BaseIterator baseIterator = this.left.EvaluateNodeSet(iter);
			if (this.left.Peer && !this.left.RequireSorting)
			{
				baseIterator = new SimpleSlashIterator(baseIterator, ExprSLASH2.DescendantOrSelfStar);
			}
			else
			{
				BaseIterator baseIterator2 = new SlashIterator(baseIterator, ExprSLASH2.DescendantOrSelfStar);
				baseIterator = ((!this.left.RequireSorting) ? baseIterator2 : new SortedIterator(baseIterator2));
			}
			SlashIterator slashIterator = new SlashIterator(baseIterator, this.right);
			return new SortedIterator(slashIterator);
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
				return false;
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

		private static NodeTest DescendantOrSelfStar = new NodeTypeTest(Axes.DescendantOrSelf, XPathNodeType.All);
	}
}
