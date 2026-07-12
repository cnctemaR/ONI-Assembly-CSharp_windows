using System;

namespace System.Xml.Xsl.Qil
{
	internal class QilCloneVisitor : QilScopedVisitor
	{
		public QilCloneVisitor(QilFactory fac)
			: this(fac, new SubstitutionList())
		{
		}

		public QilCloneVisitor(QilFactory fac, SubstitutionList subs)
		{
			this._fac = fac;
			this._subs = subs;
		}

		public QilNode Clone(QilNode node)
		{
			QilDepthChecker.Check(node);
			return this.VisitAssumeReference(node);
		}

		protected override QilNode Visit(QilNode oldNode)
		{
			QilNode qilNode = null;
			if (oldNode == null)
			{
				return null;
			}
			if (oldNode is QilReference)
			{
				qilNode = this.FindClonedReference(oldNode);
			}
			if (qilNode == null)
			{
				qilNode = oldNode.ShallowClone(this._fac);
			}
			return base.Visit(qilNode);
		}

		protected override QilNode VisitChildren(QilNode parent)
		{
			for (int i = 0; i < parent.Count; i++)
			{
				QilNode qilNode = parent[i];
				if (this.IsReference(parent, i))
				{
					parent[i] = this.VisitReference(qilNode);
					if (parent[i] == null)
					{
						parent[i] = qilNode;
					}
				}
				else
				{
					parent[i] = this.Visit(qilNode);
				}
			}
			return parent;
		}

		protected override QilNode VisitReference(QilNode oldNode)
		{
			QilNode qilNode = this.FindClonedReference(oldNode);
			return base.VisitReference((qilNode == null) ? oldNode : qilNode);
		}

		protected override void BeginScope(QilNode node)
		{
			this._subs.AddSubstitutionPair(node, node.ShallowClone(this._fac));
		}

		protected override void EndScope(QilNode node)
		{
			this._subs.RemoveLastSubstitutionPair();
		}

		protected QilNode FindClonedReference(QilNode node)
		{
			return this._subs.FindReplacement(node);
		}

		private QilFactory _fac;

		private SubstitutionList _subs;
	}
}
