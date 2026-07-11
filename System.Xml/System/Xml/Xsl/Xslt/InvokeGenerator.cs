using System;
using System.Collections.Generic;
using System.Xml.Xsl.Qil;

namespace System.Xml.Xsl.Xslt
{
	internal class InvokeGenerator : QilCloneVisitor
	{
		public InvokeGenerator(XsltQilFactory f, bool debug)
			: base(f.BaseFactory)
		{
			this.debug = debug;
			this.fac = f;
			this.iterStack = new Stack<QilIterator>();
		}

		public QilNode GenerateInvoke(QilFunction func, IList<XslNode> actualArgs)
		{
			this.iterStack.Clear();
			this.formalArgs = func.Arguments;
			this.invokeArgs = this.fac.ActualParameterList();
			this.curArg = 0;
			while (this.curArg < this.formalArgs.Count)
			{
				QilParameter qilParameter = (QilParameter)this.formalArgs[this.curArg];
				QilNode qilNode = this.FindActualArg(qilParameter, actualArgs);
				if (qilNode == null)
				{
					if (this.debug)
					{
						if (qilParameter.Name.NamespaceUri == "urn:schemas-microsoft-com:xslt-debug")
						{
							qilNode = base.Clone(qilParameter.DefaultValue);
						}
						else
						{
							qilNode = this.fac.DefaultValueMarker();
						}
					}
					else
					{
						qilNode = base.Clone(qilParameter.DefaultValue);
					}
				}
				XmlQueryType xmlType = qilParameter.XmlType;
				if (!qilNode.XmlType.IsSubtypeOf(xmlType))
				{
					qilNode = this.fac.TypeAssert(qilNode, xmlType);
				}
				this.invokeArgs.Add(qilNode);
				this.curArg++;
			}
			QilNode qilNode2 = this.fac.Invoke(func, this.invokeArgs);
			while (this.iterStack.Count != 0)
			{
				qilNode2 = this.fac.Loop(this.iterStack.Pop(), qilNode2);
			}
			return qilNode2;
		}

		private QilNode FindActualArg(QilParameter formalArg, IList<XslNode> actualArgs)
		{
			QilName name = formalArg.Name;
			foreach (XslNode xslNode in actualArgs)
			{
				if (xslNode.Name.Equals(name))
				{
					return ((VarPar)xslNode).Value;
				}
			}
			return null;
		}

		protected override QilNode VisitReference(QilNode n)
		{
			QilNode qilNode = base.FindClonedReference(n);
			if (qilNode != null)
			{
				return qilNode;
			}
			int i = 0;
			while (i < this.curArg)
			{
				if (n == this.formalArgs[i])
				{
					if (this.invokeArgs[i] is QilLiteral)
					{
						return this.invokeArgs[i].ShallowClone(this.fac.BaseFactory);
					}
					if (!(this.invokeArgs[i] is QilIterator))
					{
						QilIterator qilIterator = this.fac.BaseFactory.Let(this.invokeArgs[i]);
						this.iterStack.Push(qilIterator);
						this.invokeArgs[i] = qilIterator;
					}
					return this.invokeArgs[i];
				}
				else
				{
					i++;
				}
			}
			return n;
		}

		protected override QilNode VisitFunction(QilFunction n)
		{
			return n;
		}

		private bool debug;

		private Stack<QilIterator> iterStack;

		private QilList formalArgs;

		private QilList invokeArgs;

		private int curArg;

		private XsltQilFactory fac;
	}
}
