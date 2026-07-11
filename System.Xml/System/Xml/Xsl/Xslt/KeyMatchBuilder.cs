using System;
using System.Xml.Xsl.Qil;
using System.Xml.Xsl.XPath;

namespace System.Xml.Xsl.Xslt
{
	internal class KeyMatchBuilder : XPathBuilder, XPathPatternParser.IPatternBuilder, IXPathBuilder<QilNode>
	{
		public KeyMatchBuilder(IXPathEnvironment env)
			: base(env)
		{
			this.convertor = new KeyMatchBuilder.PathConvertor(env.Factory);
		}

		public override void StartBuild()
		{
			if (this.depth == 0)
			{
				base.StartBuild();
			}
			this.depth++;
		}

		public override QilNode EndBuild(QilNode result)
		{
			this.depth--;
			if (result == null)
			{
				return base.EndBuild(result);
			}
			if (this.depth == 0)
			{
				result = this.convertor.ConvertReletive2Absolute(result, this.fixupCurrent);
				result = base.EndBuild(result);
			}
			return result;
		}

		public virtual IXPathBuilder<QilNode> GetPredicateBuilder(QilNode ctx)
		{
			return this;
		}

		private int depth;

		private KeyMatchBuilder.PathConvertor convertor;

		internal class PathConvertor : QilReplaceVisitor
		{
			public PathConvertor(XPathQilFactory f)
				: base(f.BaseFactory)
			{
				this.f = f;
			}

			public QilNode ConvertReletive2Absolute(QilNode node, QilNode fixup)
			{
				QilDepthChecker.Check(node);
				this.fixup = fixup;
				return this.Visit(node);
			}

			protected override QilNode Visit(QilNode n)
			{
				if (n.NodeType == QilNodeType.Union || n.NodeType == QilNodeType.DocOrderDistinct || n.NodeType == QilNodeType.Filter || n.NodeType == QilNodeType.Loop)
				{
					return base.Visit(n);
				}
				return n;
			}

			protected override QilNode VisitLoop(QilLoop n)
			{
				if (n.Variable.Binding.NodeType == QilNodeType.Root || n.Variable.Binding.NodeType == QilNodeType.Deref)
				{
					return n;
				}
				if (n.Variable.Binding.NodeType == QilNodeType.Content)
				{
					QilUnary qilUnary = (QilUnary)n.Variable.Binding;
					QilIterator qilIterator = this.f.For(this.f.DescendantOrSelf(this.f.Root(this.fixup)));
					qilUnary.Child = qilIterator;
					n.Variable.Binding = this.f.Loop(qilIterator, qilUnary);
					return n;
				}
				n.Variable.Binding = this.Visit(n.Variable.Binding);
				return n;
			}

			protected override QilNode VisitFilter(QilLoop n)
			{
				return this.VisitLoop(n);
			}

			private new XPathQilFactory f;

			private QilNode fixup;
		}
	}
}
