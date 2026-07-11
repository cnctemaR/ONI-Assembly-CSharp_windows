using System;

namespace System.Xml.Xsl.Qil
{
	internal abstract class QilReplaceVisitor : QilVisitor
	{
		public QilReplaceVisitor(QilFactory f)
		{
			this.f = f;
		}

		protected override QilNode VisitChildren(QilNode parent)
		{
			XmlQueryType xmlType = parent.XmlType;
			bool flag = false;
			for (int i = 0; i < parent.Count; i++)
			{
				QilNode qilNode = parent[i];
				XmlQueryType xmlQueryType = ((qilNode != null) ? qilNode.XmlType : null);
				QilNode qilNode2;
				if (this.IsReference(parent, i))
				{
					qilNode2 = this.VisitReference(qilNode);
				}
				else
				{
					qilNode2 = this.Visit(qilNode);
				}
				if (qilNode != qilNode2 || (qilNode2 != null && xmlQueryType != qilNode2.XmlType))
				{
					flag = true;
					parent[i] = qilNode2;
				}
			}
			if (flag)
			{
				this.RecalculateType(parent, xmlType);
			}
			return parent;
		}

		protected virtual void RecalculateType(QilNode node, XmlQueryType oldType)
		{
			XmlQueryType xmlQueryType = this.f.TypeChecker.Check(node);
			node.XmlType = xmlQueryType;
		}

		protected QilFactory f;
	}
}
