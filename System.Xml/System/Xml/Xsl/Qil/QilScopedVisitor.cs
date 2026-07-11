using System;
using System.Collections.Generic;

namespace System.Xml.Xsl.Qil
{
	internal class QilScopedVisitor : QilVisitor
	{
		protected virtual void BeginScope(QilNode node)
		{
		}

		protected virtual void EndScope(QilNode node)
		{
		}

		protected virtual void BeforeVisit(QilNode node)
		{
			QilNodeType nodeType = node.NodeType;
			if (nodeType != QilNodeType.QilExpression)
			{
				if (nodeType - QilNodeType.Loop <= 2)
				{
					goto IL_00EF;
				}
				if (nodeType != QilNodeType.Function)
				{
					return;
				}
			}
			else
			{
				QilExpression qilExpression = (QilExpression)node;
				foreach (QilNode qilNode in qilExpression.GlobalParameterList)
				{
					this.BeginScope(qilNode);
				}
				foreach (QilNode qilNode2 in qilExpression.GlobalVariableList)
				{
					this.BeginScope(qilNode2);
				}
				using (IEnumerator<QilNode> enumerator = qilExpression.FunctionList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						QilNode qilNode3 = enumerator.Current;
						this.BeginScope(qilNode3);
					}
					return;
				}
			}
			using (IEnumerator<QilNode> enumerator = ((QilFunction)node).Arguments.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					QilNode qilNode4 = enumerator.Current;
					this.BeginScope(qilNode4);
				}
				return;
			}
			IL_00EF:
			this.BeginScope(((QilLoop)node).Variable);
		}

		protected virtual void AfterVisit(QilNode node)
		{
			QilNodeType nodeType = node.NodeType;
			if (nodeType != QilNodeType.QilExpression)
			{
				if (nodeType - QilNodeType.Loop <= 2)
				{
					goto IL_00EF;
				}
				if (nodeType != QilNodeType.Function)
				{
					return;
				}
			}
			else
			{
				QilExpression qilExpression = (QilExpression)node;
				foreach (QilNode qilNode in qilExpression.FunctionList)
				{
					this.EndScope(qilNode);
				}
				foreach (QilNode qilNode2 in qilExpression.GlobalVariableList)
				{
					this.EndScope(qilNode2);
				}
				using (IEnumerator<QilNode> enumerator = qilExpression.GlobalParameterList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						QilNode qilNode3 = enumerator.Current;
						this.EndScope(qilNode3);
					}
					return;
				}
			}
			using (IEnumerator<QilNode> enumerator = ((QilFunction)node).Arguments.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					QilNode qilNode4 = enumerator.Current;
					this.EndScope(qilNode4);
				}
				return;
			}
			IL_00EF:
			this.EndScope(((QilLoop)node).Variable);
		}

		protected override QilNode Visit(QilNode n)
		{
			this.BeforeVisit(n);
			QilNode qilNode = base.Visit(n);
			this.AfterVisit(n);
			return qilNode;
		}
	}
}
