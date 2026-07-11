using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Xml.XPath;
using System.Xml.Xsl.Qil;
using System.Xml.Xsl.XPath;

namespace System.Xml.Xsl.Xslt
{
	internal class XPathPatternBuilder : XPathPatternParser.IPatternBuilder, IXPathBuilder<QilNode>
	{
		public XPathPatternBuilder(IXPathEnvironment environment)
		{
			this.environment = environment;
			this.f = environment.Factory;
			this.predicateEnvironment = new XPathPatternBuilder.XPathPredicateEnvironment(environment);
			this.predicateBuilder = new XPathBuilder(this.predicateEnvironment);
			this.fixupNode = this.f.Unknown(XmlQueryTypeFactory.NodeNotRtfS);
		}

		public QilNode FixupNode
		{
			get
			{
				return this.fixupNode;
			}
		}

		public virtual void StartBuild()
		{
			this.inTheBuild = true;
		}

		[Conditional("DEBUG")]
		public void AssertFilter(QilLoop filter)
		{
		}

		private void FixupFilterBinding(QilLoop filter, QilNode newBinding)
		{
			filter.Variable.Binding = newBinding;
		}

		public virtual QilNode EndBuild(QilNode result)
		{
			this.inTheBuild = false;
			return result;
		}

		public QilNode Operator(XPathOperator op, QilNode left, QilNode right)
		{
			if (left.NodeType == QilNodeType.Sequence)
			{
				((QilList)left).Add(right);
				return left;
			}
			return this.f.Sequence(left, right);
		}

		private static QilLoop BuildAxisFilter(QilPatternFactory f, QilIterator itr, XPathAxis xpathAxis, XPathNodeType nodeType, string name, string nsUri)
		{
			QilNode qilNode = ((name != null && nsUri != null) ? f.Eq(f.NameOf(itr), f.QName(name, nsUri)) : ((nsUri != null) ? f.Eq(f.NamespaceUriOf(itr), f.String(nsUri)) : ((name != null) ? f.Eq(f.LocalNameOf(itr), f.String(name)) : f.True())));
			XmlNodeKindFlags xmlNodeKindFlags = XPathBuilder.AxisTypeMask(itr.XmlType.NodeKinds, nodeType, xpathAxis);
			QilNode qilNode2 = ((xmlNodeKindFlags == XmlNodeKindFlags.None) ? f.False() : ((xmlNodeKindFlags == itr.XmlType.NodeKinds) ? f.True() : f.IsType(itr, XmlQueryTypeFactory.NodeChoice(xmlNodeKindFlags))));
			QilLoop qilLoop = f.BaseFactory.Filter(itr, f.And(qilNode2, qilNode));
			qilLoop.XmlType = XmlQueryTypeFactory.PrimeProduct(XmlQueryTypeFactory.NodeChoice(xmlNodeKindFlags), qilLoop.XmlType.Cardinality);
			return qilLoop;
		}

		public QilNode Axis(XPathAxis xpathAxis, XPathNodeType nodeType, string prefix, string name)
		{
			if (xpathAxis != XPathAxis.DescendantOrSelf)
			{
				QilLoop qilLoop;
				double num;
				if (xpathAxis != XPathAxis.Root)
				{
					string text = ((prefix == null) ? null : this.environment.ResolvePrefix(prefix));
					qilLoop = XPathPatternBuilder.BuildAxisFilter(this.f, this.f.For(this.fixupNode), xpathAxis, nodeType, name, text);
					if (nodeType - XPathNodeType.Element > 1)
					{
						if (nodeType != XPathNodeType.ProcessingInstruction)
						{
							num = -0.5;
						}
						else
						{
							num = ((name != null) ? 0.0 : (-0.5));
						}
					}
					else if (name != null)
					{
						num = 0.0;
					}
					else if (prefix != null)
					{
						num = -0.25;
					}
					else
					{
						num = -0.5;
					}
				}
				else
				{
					QilIterator qilIterator;
					qilLoop = this.f.BaseFactory.Filter(qilIterator = this.f.For(this.fixupNode), this.f.IsType(qilIterator, XmlQueryTypeFactory.Document));
					num = 0.5;
				}
				XPathPatternBuilder.SetPriority(qilLoop, num);
				XPathPatternBuilder.SetLastParent(qilLoop, qilLoop);
				return qilLoop;
			}
			return this.f.Nop(this.fixupNode);
		}

		public QilNode JoinStep(QilNode left, QilNode right)
		{
			if (left.NodeType == QilNodeType.Nop)
			{
				QilUnary qilUnary = (QilUnary)left;
				qilUnary.Child = right;
				return qilUnary;
			}
			XPathPatternBuilder.CleanAnnotation(left);
			QilLoop qilLoop = (QilLoop)left;
			bool flag = false;
			if (right.NodeType == QilNodeType.Nop)
			{
				flag = true;
				right = ((QilUnary)right).Child;
			}
			QilLoop lastParent = XPathPatternBuilder.GetLastParent(right);
			this.FixupFilterBinding(qilLoop, flag ? this.f.Ancestor(lastParent.Variable) : this.f.Parent(lastParent.Variable));
			lastParent.Body = this.f.And(lastParent.Body, this.f.Not(this.f.IsEmpty(qilLoop)));
			XPathPatternBuilder.SetPriority(right, 0.5);
			XPathPatternBuilder.SetLastParent(right, qilLoop);
			return right;
		}

		QilNode IXPathBuilder<QilNode>.Predicate(QilNode node, QilNode condition, bool isReverseStep)
		{
			return null;
		}

		public QilNode BuildPredicates(QilNode nodeset, List<QilNode> predicates)
		{
			List<QilNode> list = new List<QilNode>(predicates.Count);
			foreach (QilNode qilNode in predicates)
			{
				list.Add(XPathBuilder.PredicateToBoolean(qilNode, this.f, this.predicateEnvironment));
			}
			QilLoop qilLoop = (QilLoop)nodeset;
			QilIterator variable = qilLoop.Variable;
			if (this.predicateEnvironment.numFixupLast == 0 && this.predicateEnvironment.numFixupPosition == 0)
			{
				foreach (QilNode qilNode2 in list)
				{
					qilLoop.Body = this.f.And(qilLoop.Body, qilNode2);
				}
				qilLoop.Body = this.predicateEnvironment.fixupVisitor.Fixup(qilLoop.Body, variable, null);
			}
			else
			{
				QilIterator qilIterator = this.f.For(this.f.Parent(variable));
				QilNode qilNode3 = this.f.Content(qilIterator);
				QilLoop qilLoop2 = (QilLoop)nodeset.DeepClone(this.f.BaseFactory);
				qilLoop2.Variable.Binding = qilNode3;
				qilLoop2 = (QilLoop)this.f.Loop(qilIterator, qilLoop2);
				QilNode qilNode4 = qilLoop2;
				foreach (QilNode qilNode5 in list)
				{
					qilNode4 = XPathBuilder.BuildOnePredicate(qilNode4, qilNode5, false, this.f, this.predicateEnvironment.fixupVisitor, ref this.predicateEnvironment.numFixupCurrent, ref this.predicateEnvironment.numFixupPosition, ref this.predicateEnvironment.numFixupLast);
				}
				QilIterator qilIterator2 = this.f.For(qilNode4);
				QilNode qilNode6 = this.f.Filter(qilIterator2, this.f.Is(qilIterator2, variable));
				qilLoop.Body = this.f.Not(this.f.IsEmpty(qilNode6));
				qilLoop.Body = this.f.And(this.f.IsType(variable, qilLoop.XmlType), qilLoop.Body);
			}
			XPathPatternBuilder.SetPriority(nodeset, 0.5);
			return nodeset;
		}

		public QilNode Function(string prefix, string name, IList<QilNode> args)
		{
			QilIterator qilIterator = this.f.For(this.fixupNode);
			QilNode qilNode;
			if (name == "id")
			{
				qilNode = this.f.Id(qilIterator, args[0]);
			}
			else
			{
				qilNode = this.environment.ResolveFunction(prefix, name, args, new XPathPatternBuilder.XsltFunctionFocus(qilIterator));
			}
			QilIterator qilIterator2;
			QilLoop qilLoop = this.f.BaseFactory.Filter(qilIterator, this.f.Not(this.f.IsEmpty(this.f.Filter(qilIterator2 = this.f.For(qilNode), this.f.Is(qilIterator2, qilIterator)))));
			XPathPatternBuilder.SetPriority(qilLoop, 0.5);
			XPathPatternBuilder.SetLastParent(qilLoop, qilLoop);
			return qilLoop;
		}

		public QilNode String(string value)
		{
			return this.f.String(value);
		}

		public QilNode Number(double value)
		{
			return this.UnexpectedToken("Literal number");
		}

		public QilNode Variable(string prefix, string name)
		{
			return this.UnexpectedToken("Variable");
		}

		private QilNode UnexpectedToken(string tokenName)
		{
			throw new Exception(string.Format(CultureInfo.InvariantCulture, "Internal Error: {0} is not allowed in XSLT pattern outside of predicate.", tokenName));
		}

		public static void SetPriority(QilNode node, double priority)
		{
			XPathPatternBuilder.Annotation annotation = ((XPathPatternBuilder.Annotation)node.Annotation) ?? new XPathPatternBuilder.Annotation();
			annotation.Priority = priority;
			node.Annotation = annotation;
		}

		public static double GetPriority(QilNode node)
		{
			return ((XPathPatternBuilder.Annotation)node.Annotation).Priority;
		}

		private static void SetLastParent(QilNode node, QilLoop parent)
		{
			XPathPatternBuilder.Annotation annotation = ((XPathPatternBuilder.Annotation)node.Annotation) ?? new XPathPatternBuilder.Annotation();
			annotation.Parent = parent;
			node.Annotation = annotation;
		}

		private static QilLoop GetLastParent(QilNode node)
		{
			return ((XPathPatternBuilder.Annotation)node.Annotation).Parent;
		}

		public static void CleanAnnotation(QilNode node)
		{
			node.Annotation = null;
		}

		public IXPathBuilder<QilNode> GetPredicateBuilder(QilNode ctx)
		{
			QilLoop qilLoop = (QilLoop)ctx;
			return this.predicateBuilder;
		}

		private XPathPatternBuilder.XPathPredicateEnvironment predicateEnvironment;

		private XPathBuilder predicateBuilder;

		private bool inTheBuild;

		private XPathQilFactory f;

		private QilNode fixupNode;

		private IXPathEnvironment environment;

		private class Annotation
		{
			public double Priority;

			public QilLoop Parent;
		}

		private class XPathPredicateEnvironment : IXPathEnvironment, IFocus
		{
			public XPathPredicateEnvironment(IXPathEnvironment baseEnvironment)
			{
				this.baseEnvironment = baseEnvironment;
				this.f = baseEnvironment.Factory;
				this.fixupCurrent = this.f.Unknown(XmlQueryTypeFactory.NodeNotRtf);
				this.fixupPosition = this.f.Unknown(XmlQueryTypeFactory.DoubleX);
				this.fixupLast = this.f.Unknown(XmlQueryTypeFactory.DoubleX);
				this.fixupVisitor = new XPathBuilder.FixupVisitor(this.f, this.fixupCurrent, this.fixupPosition, this.fixupLast);
			}

			public XPathQilFactory Factory
			{
				get
				{
					return this.f;
				}
			}

			public QilNode ResolveVariable(string prefix, string name)
			{
				return this.baseEnvironment.ResolveVariable(prefix, name);
			}

			public QilNode ResolveFunction(string prefix, string name, IList<QilNode> args, IFocus env)
			{
				return this.baseEnvironment.ResolveFunction(prefix, name, args, env);
			}

			public string ResolvePrefix(string prefix)
			{
				return this.baseEnvironment.ResolvePrefix(prefix);
			}

			public QilNode GetCurrent()
			{
				this.numFixupCurrent++;
				return this.fixupCurrent;
			}

			public QilNode GetPosition()
			{
				this.numFixupPosition++;
				return this.fixupPosition;
			}

			public QilNode GetLast()
			{
				this.numFixupLast++;
				return this.fixupLast;
			}

			private readonly IXPathEnvironment baseEnvironment;

			private readonly XPathQilFactory f;

			public readonly XPathBuilder.FixupVisitor fixupVisitor;

			private readonly QilNode fixupCurrent;

			private readonly QilNode fixupPosition;

			private readonly QilNode fixupLast;

			public int numFixupCurrent;

			public int numFixupPosition;

			public int numFixupLast;
		}

		private class XsltFunctionFocus : IFocus
		{
			public XsltFunctionFocus(QilIterator current)
			{
				this.current = current;
			}

			public QilNode GetCurrent()
			{
				return this.current;
			}

			public QilNode GetPosition()
			{
				return null;
			}

			public QilNode GetLast()
			{
				return null;
			}

			private QilIterator current;
		}
	}
}
