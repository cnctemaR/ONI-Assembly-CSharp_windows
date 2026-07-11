using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Xsl.Qil;

namespace System.Xml.Xsl.XPath
{
	internal class XPathBuilder : IXPathBuilder<QilNode>, IXPathEnvironment, IFocus
	{
		QilNode IFocus.GetCurrent()
		{
			return this.GetCurrentNode();
		}

		QilNode IFocus.GetPosition()
		{
			return this.GetCurrentPosition();
		}

		QilNode IFocus.GetLast()
		{
			return this.GetLastPosition();
		}

		XPathQilFactory IXPathEnvironment.Factory
		{
			get
			{
				return this.f;
			}
		}

		QilNode IXPathEnvironment.ResolveVariable(string prefix, string name)
		{
			return this.Variable(prefix, name);
		}

		QilNode IXPathEnvironment.ResolveFunction(string prefix, string name, IList<QilNode> args, IFocus env)
		{
			return null;
		}

		string IXPathEnvironment.ResolvePrefix(string prefix)
		{
			return this.environment.ResolvePrefix(prefix);
		}

		public XPathBuilder(IXPathEnvironment environment)
		{
			this.environment = environment;
			this.f = this.environment.Factory;
			this.fixupCurrent = this.f.Unknown(XmlQueryTypeFactory.NodeNotRtf);
			this.fixupPosition = this.f.Unknown(XmlQueryTypeFactory.DoubleX);
			this.fixupLast = this.f.Unknown(XmlQueryTypeFactory.DoubleX);
			this.fixupVisitor = new XPathBuilder.FixupVisitor(this.f, this.fixupCurrent, this.fixupPosition, this.fixupLast);
		}

		public virtual void StartBuild()
		{
			this.inTheBuild = true;
			this.numFixupCurrent = (this.numFixupPosition = (this.numFixupLast = 0));
		}

		public virtual QilNode EndBuild(QilNode result)
		{
			if (result == null)
			{
				this.inTheBuild = false;
				return result;
			}
			if (result.XmlType.MaybeMany && result.XmlType.IsNode && result.XmlType.IsNotRtf)
			{
				result = this.f.DocOrderDistinct(result);
			}
			result = this.fixupVisitor.Fixup(result, this.environment);
			this.numFixupCurrent -= this.fixupVisitor.numCurrent;
			this.numFixupPosition -= this.fixupVisitor.numPosition;
			this.numFixupLast -= this.fixupVisitor.numLast;
			this.inTheBuild = false;
			return result;
		}

		private QilNode GetCurrentNode()
		{
			this.numFixupCurrent++;
			return this.fixupCurrent;
		}

		private QilNode GetCurrentPosition()
		{
			this.numFixupPosition++;
			return this.fixupPosition;
		}

		private QilNode GetLastPosition()
		{
			this.numFixupLast++;
			return this.fixupLast;
		}

		public virtual QilNode String(string value)
		{
			return this.f.String(value);
		}

		public virtual QilNode Number(double value)
		{
			return this.f.Double(value);
		}

		public virtual QilNode Operator(XPathOperator op, QilNode left, QilNode right)
		{
			switch (XPathBuilder.OperatorGroup[(int)op])
			{
			case XPathBuilder.XPathOperatorGroup.Logical:
				return this.LogicalOperator(op, left, right);
			case XPathBuilder.XPathOperatorGroup.Equality:
				return this.EqualityOperator(op, left, right);
			case XPathBuilder.XPathOperatorGroup.Relational:
				return this.RelationalOperator(op, left, right);
			case XPathBuilder.XPathOperatorGroup.Arithmetic:
				return this.ArithmeticOperator(op, left, right);
			case XPathBuilder.XPathOperatorGroup.Negate:
				return this.NegateOperator(op, left, right);
			case XPathBuilder.XPathOperatorGroup.Union:
				return this.UnionOperator(op, left, right);
			default:
				return null;
			}
		}

		private QilNode LogicalOperator(XPathOperator op, QilNode left, QilNode right)
		{
			left = this.f.ConvertToBoolean(left);
			right = this.f.ConvertToBoolean(right);
			if (op != XPathOperator.Or)
			{
				return this.f.And(left, right);
			}
			return this.f.Or(left, right);
		}

		private QilNode CompareValues(XPathOperator op, QilNode left, QilNode right, XmlTypeCode compType)
		{
			left = this.f.ConvertToType(compType, left);
			right = this.f.ConvertToType(compType, right);
			switch (op)
			{
			case XPathOperator.Eq:
				return this.f.Eq(left, right);
			case XPathOperator.Ne:
				return this.f.Ne(left, right);
			case XPathOperator.Lt:
				return this.f.Lt(left, right);
			case XPathOperator.Le:
				return this.f.Le(left, right);
			case XPathOperator.Gt:
				return this.f.Gt(left, right);
			case XPathOperator.Ge:
				return this.f.Ge(left, right);
			default:
				return null;
			}
		}

		private QilNode CompareNodeSetAndValue(XPathOperator op, QilNode nodeset, QilNode val, XmlTypeCode compType)
		{
			if (compType == XmlTypeCode.Boolean || nodeset.XmlType.IsSingleton)
			{
				return this.CompareValues(op, nodeset, val, compType);
			}
			QilIterator qilIterator = this.f.For(nodeset);
			return this.f.Not(this.f.IsEmpty(this.f.Filter(qilIterator, this.CompareValues(op, this.f.XPathNodeValue(qilIterator), val, compType))));
		}

		private static XPathOperator InvertOp(XPathOperator op)
		{
			if (op == XPathOperator.Lt)
			{
				return XPathOperator.Gt;
			}
			if (op == XPathOperator.Le)
			{
				return XPathOperator.Ge;
			}
			if (op == XPathOperator.Gt)
			{
				return XPathOperator.Lt;
			}
			if (op != XPathOperator.Ge)
			{
				return op;
			}
			return XPathOperator.Le;
		}

		private QilNode CompareNodeSetAndNodeSet(XPathOperator op, QilNode left, QilNode right, XmlTypeCode compType)
		{
			if (right.XmlType.IsSingleton)
			{
				return this.CompareNodeSetAndValue(op, left, right, compType);
			}
			if (left.XmlType.IsSingleton)
			{
				op = XPathBuilder.InvertOp(op);
				return this.CompareNodeSetAndValue(op, right, left, compType);
			}
			QilIterator qilIterator = this.f.For(left);
			QilIterator qilIterator2 = this.f.For(right);
			return this.f.Not(this.f.IsEmpty(this.f.Loop(qilIterator, this.f.Filter(qilIterator2, this.CompareValues(op, this.f.XPathNodeValue(qilIterator), this.f.XPathNodeValue(qilIterator2), compType)))));
		}

		private QilNode EqualityOperator(XPathOperator op, QilNode left, QilNode right)
		{
			XmlQueryType xmlType = left.XmlType;
			XmlQueryType xmlType2 = right.XmlType;
			if (this.f.IsAnyType(left) || this.f.IsAnyType(right))
			{
				return this.f.InvokeEqualityOperator(XPathBuilder.QilOperator[(int)op], left, right);
			}
			if (xmlType.IsNode && xmlType2.IsNode)
			{
				return this.CompareNodeSetAndNodeSet(op, left, right, XmlTypeCode.String);
			}
			if (xmlType.IsNode)
			{
				return this.CompareNodeSetAndValue(op, left, right, xmlType2.TypeCode);
			}
			if (xmlType2.IsNode)
			{
				return this.CompareNodeSetAndValue(op, right, left, xmlType.TypeCode);
			}
			XmlTypeCode xmlTypeCode = ((xmlType.TypeCode == XmlTypeCode.Boolean || xmlType2.TypeCode == XmlTypeCode.Boolean) ? XmlTypeCode.Boolean : ((xmlType.TypeCode == XmlTypeCode.Double || xmlType2.TypeCode == XmlTypeCode.Double) ? XmlTypeCode.Double : XmlTypeCode.String));
			return this.CompareValues(op, left, right, xmlTypeCode);
		}

		private QilNode RelationalOperator(XPathOperator op, QilNode left, QilNode right)
		{
			XmlQueryType xmlType = left.XmlType;
			XmlQueryType xmlType2 = right.XmlType;
			if (this.f.IsAnyType(left) || this.f.IsAnyType(right))
			{
				return this.f.InvokeRelationalOperator(XPathBuilder.QilOperator[(int)op], left, right);
			}
			if (xmlType.IsNode && xmlType2.IsNode)
			{
				return this.CompareNodeSetAndNodeSet(op, left, right, XmlTypeCode.Double);
			}
			if (xmlType.IsNode)
			{
				XmlTypeCode xmlTypeCode = ((xmlType2.TypeCode == XmlTypeCode.Boolean) ? XmlTypeCode.Boolean : XmlTypeCode.Double);
				return this.CompareNodeSetAndValue(op, left, right, xmlTypeCode);
			}
			if (xmlType2.IsNode)
			{
				XmlTypeCode xmlTypeCode2 = ((xmlType.TypeCode == XmlTypeCode.Boolean) ? XmlTypeCode.Boolean : XmlTypeCode.Double);
				op = XPathBuilder.InvertOp(op);
				return this.CompareNodeSetAndValue(op, right, left, xmlTypeCode2);
			}
			return this.CompareValues(op, left, right, XmlTypeCode.Double);
		}

		private QilNode NegateOperator(XPathOperator op, QilNode left, QilNode right)
		{
			return this.f.Negate(this.f.ConvertToNumber(left));
		}

		private QilNode ArithmeticOperator(XPathOperator op, QilNode left, QilNode right)
		{
			left = this.f.ConvertToNumber(left);
			right = this.f.ConvertToNumber(right);
			switch (op)
			{
			case XPathOperator.Plus:
				return this.f.Add(left, right);
			case XPathOperator.Minus:
				return this.f.Subtract(left, right);
			case XPathOperator.Multiply:
				return this.f.Multiply(left, right);
			case XPathOperator.Divide:
				return this.f.Divide(left, right);
			case XPathOperator.Modulo:
				return this.f.Modulo(left, right);
			default:
				return null;
			}
		}

		private QilNode UnionOperator(XPathOperator op, QilNode left, QilNode right)
		{
			if (left == null)
			{
				return this.f.EnsureNodeSet(right);
			}
			left = this.f.EnsureNodeSet(left);
			right = this.f.EnsureNodeSet(right);
			if (left.NodeType == QilNodeType.Sequence)
			{
				((QilList)left).Add(right);
				return left;
			}
			return this.f.Union(left, right);
		}

		public static XmlNodeKindFlags AxisTypeMask(XmlNodeKindFlags inputTypeMask, XPathNodeType nodeType, XPathAxis xpathAxis)
		{
			return inputTypeMask & XPathBuilder.XPathNodeType2QilXmlNodeKind[(int)nodeType] & XPathBuilder.XPathAxisMask[(int)xpathAxis];
		}

		private QilNode BuildAxisFilter(QilNode qilAxis, XPathAxis xpathAxis, XPathNodeType nodeType, string name, string nsUri)
		{
			XmlNodeKindFlags nodeKinds = qilAxis.XmlType.NodeKinds;
			XmlNodeKindFlags xmlNodeKindFlags = XPathBuilder.AxisTypeMask(nodeKinds, nodeType, xpathAxis);
			if (xmlNodeKindFlags == XmlNodeKindFlags.None)
			{
				return this.f.Sequence();
			}
			QilIterator qilIterator;
			if (xmlNodeKindFlags != nodeKinds)
			{
				qilAxis = this.f.Filter(qilIterator = this.f.For(qilAxis), this.f.IsType(qilIterator, XmlQueryTypeFactory.NodeChoice(xmlNodeKindFlags)));
				qilAxis.XmlType = XmlQueryTypeFactory.PrimeProduct(XmlQueryTypeFactory.NodeChoice(xmlNodeKindFlags), qilAxis.XmlType.Cardinality);
				if (qilAxis.NodeType == QilNodeType.Filter)
				{
					QilLoop qilLoop = (QilLoop)qilAxis;
					qilLoop.Body = this.f.And(qilLoop.Body, (name != null && nsUri != null) ? this.f.Eq(this.f.NameOf(qilIterator), this.f.QName(name, nsUri)) : ((nsUri != null) ? this.f.Eq(this.f.NamespaceUriOf(qilIterator), this.f.String(nsUri)) : ((name != null) ? this.f.Eq(this.f.LocalNameOf(qilIterator), this.f.String(name)) : this.f.True())));
					return qilLoop;
				}
			}
			return this.f.Filter(qilIterator = this.f.For(qilAxis), (name != null && nsUri != null) ? this.f.Eq(this.f.NameOf(qilIterator), this.f.QName(name, nsUri)) : ((nsUri != null) ? this.f.Eq(this.f.NamespaceUriOf(qilIterator), this.f.String(nsUri)) : ((name != null) ? this.f.Eq(this.f.LocalNameOf(qilIterator), this.f.String(name)) : this.f.True())));
		}

		private QilNode BuildAxis(XPathAxis xpathAxis, XPathNodeType nodeType, string nsUri, string name)
		{
			QilNode currentNode = this.GetCurrentNode();
			QilNode qilNode;
			switch (xpathAxis)
			{
			case XPathAxis.Ancestor:
				qilNode = this.f.Ancestor(currentNode);
				break;
			case XPathAxis.AncestorOrSelf:
				qilNode = this.f.AncestorOrSelf(currentNode);
				break;
			case XPathAxis.Attribute:
				qilNode = this.f.Content(currentNode);
				break;
			case XPathAxis.Child:
				qilNode = this.f.Content(currentNode);
				break;
			case XPathAxis.Descendant:
				qilNode = this.f.Descendant(currentNode);
				break;
			case XPathAxis.DescendantOrSelf:
				qilNode = this.f.DescendantOrSelf(currentNode);
				break;
			case XPathAxis.Following:
				qilNode = this.f.XPathFollowing(currentNode);
				break;
			case XPathAxis.FollowingSibling:
				qilNode = this.f.FollowingSibling(currentNode);
				break;
			case XPathAxis.Namespace:
				qilNode = this.f.XPathNamespace(currentNode);
				break;
			case XPathAxis.Parent:
				qilNode = this.f.Parent(currentNode);
				break;
			case XPathAxis.Preceding:
				qilNode = this.f.XPathPreceding(currentNode);
				break;
			case XPathAxis.PrecedingSibling:
				qilNode = this.f.PrecedingSibling(currentNode);
				break;
			case XPathAxis.Self:
				qilNode = currentNode;
				break;
			case XPathAxis.Root:
				return this.f.Root(currentNode);
			default:
				qilNode = null;
				break;
			}
			QilNode qilNode2 = this.BuildAxisFilter(qilNode, xpathAxis, nodeType, name, nsUri);
			if (xpathAxis == XPathAxis.Ancestor || xpathAxis == XPathAxis.Preceding || xpathAxis == XPathAxis.AncestorOrSelf || xpathAxis == XPathAxis.PrecedingSibling)
			{
				qilNode2 = this.f.BaseFactory.DocOrderDistinct(qilNode2);
			}
			return qilNode2;
		}

		public virtual QilNode Axis(XPathAxis xpathAxis, XPathNodeType nodeType, string prefix, string name)
		{
			string text = ((prefix == null) ? null : this.environment.ResolvePrefix(prefix));
			return this.BuildAxis(xpathAxis, nodeType, text, name);
		}

		public virtual QilNode JoinStep(QilNode left, QilNode right)
		{
			QilIterator qilIterator = this.f.For(this.f.EnsureNodeSet(left));
			right = this.fixupVisitor.Fixup(right, qilIterator, null);
			this.numFixupCurrent -= this.fixupVisitor.numCurrent;
			this.numFixupPosition -= this.fixupVisitor.numPosition;
			this.numFixupLast -= this.fixupVisitor.numLast;
			return this.f.DocOrderDistinct(this.f.Loop(qilIterator, right));
		}

		public virtual QilNode Predicate(QilNode nodeset, QilNode predicate, bool isReverseStep)
		{
			if (isReverseStep)
			{
				nodeset = ((QilUnary)nodeset).Child;
			}
			predicate = XPathBuilder.PredicateToBoolean(predicate, this.f, this);
			return XPathBuilder.BuildOnePredicate(nodeset, predicate, isReverseStep, this.f, this.fixupVisitor, ref this.numFixupCurrent, ref this.numFixupPosition, ref this.numFixupLast);
		}

		public static QilNode PredicateToBoolean(QilNode predicate, XPathQilFactory f, IXPathEnvironment env)
		{
			if (!f.IsAnyType(predicate))
			{
				if (predicate.XmlType.TypeCode == XmlTypeCode.Double)
				{
					predicate = f.Eq(env.GetPosition(), predicate);
				}
				else
				{
					predicate = f.ConvertToBoolean(predicate);
				}
			}
			else
			{
				QilIterator qilIterator;
				predicate = f.Loop(qilIterator = f.Let(predicate), f.Conditional(f.IsType(qilIterator, XmlQueryTypeFactory.Double), f.Eq(env.GetPosition(), f.TypeAssert(qilIterator, XmlQueryTypeFactory.DoubleX)), f.ConvertToBoolean(qilIterator)));
			}
			return predicate;
		}

		public static QilNode BuildOnePredicate(QilNode nodeset, QilNode predicate, bool isReverseStep, XPathQilFactory f, XPathBuilder.FixupVisitor fixupVisitor, ref int numFixupCurrent, ref int numFixupPosition, ref int numFixupLast)
		{
			nodeset = f.EnsureNodeSet(nodeset);
			QilNode qilNode;
			if (numFixupLast != 0 && fixupVisitor.CountUnfixedLast(predicate) != 0)
			{
				QilIterator qilIterator = f.Let(nodeset);
				QilIterator qilIterator2 = f.Let(f.XsltConvert(f.Length(qilIterator), XmlQueryTypeFactory.DoubleX));
				QilIterator qilIterator3 = f.For(qilIterator);
				predicate = fixupVisitor.Fixup(predicate, qilIterator3, qilIterator2);
				numFixupCurrent -= fixupVisitor.numCurrent;
				numFixupPosition -= fixupVisitor.numPosition;
				numFixupLast -= fixupVisitor.numLast;
				qilNode = f.Loop(qilIterator, f.Loop(qilIterator2, f.Filter(qilIterator3, predicate)));
			}
			else
			{
				QilIterator qilIterator4 = f.For(nodeset);
				predicate = fixupVisitor.Fixup(predicate, qilIterator4, null);
				numFixupCurrent -= fixupVisitor.numCurrent;
				numFixupPosition -= fixupVisitor.numPosition;
				numFixupLast -= fixupVisitor.numLast;
				qilNode = f.Filter(qilIterator4, predicate);
			}
			if (isReverseStep)
			{
				qilNode = f.DocOrderDistinct(qilNode);
			}
			return qilNode;
		}

		public virtual QilNode Variable(string prefix, string name)
		{
			return this.environment.ResolveVariable(prefix, name);
		}

		public virtual QilNode Function(string prefix, string name, IList<QilNode> args)
		{
			XPathBuilder.FunctionInfo<XPathBuilder.FuncId> functionInfo;
			if (prefix.Length != 0 || !XPathBuilder.FunctionTable.TryGetValue(name, out functionInfo))
			{
				return this.environment.ResolveFunction(prefix, name, args, this);
			}
			functionInfo.CastArguments(args, name, this.f);
			switch (functionInfo.id)
			{
			case XPathBuilder.FuncId.Last:
				return this.GetLastPosition();
			case XPathBuilder.FuncId.Position:
				return this.GetCurrentPosition();
			case XPathBuilder.FuncId.Count:
				return this.f.XsltConvert(this.f.Length(this.f.DocOrderDistinct(args[0])), XmlQueryTypeFactory.DoubleX);
			case XPathBuilder.FuncId.LocalName:
				if (args.Count != 0)
				{
					return this.LocalNameOfFirstNode(args[0]);
				}
				return this.f.LocalNameOf(this.GetCurrentNode());
			case XPathBuilder.FuncId.NamespaceUri:
				if (args.Count != 0)
				{
					return this.NamespaceOfFirstNode(args[0]);
				}
				return this.f.NamespaceUriOf(this.GetCurrentNode());
			case XPathBuilder.FuncId.Name:
				if (args.Count != 0)
				{
					return this.NameOfFirstNode(args[0]);
				}
				return this.NameOf(this.GetCurrentNode());
			case XPathBuilder.FuncId.String:
				if (args.Count != 0)
				{
					return this.f.ConvertToString(args[0]);
				}
				return this.f.XPathNodeValue(this.GetCurrentNode());
			case XPathBuilder.FuncId.Number:
				if (args.Count != 0)
				{
					return this.f.ConvertToNumber(args[0]);
				}
				return this.f.XsltConvert(this.f.XPathNodeValue(this.GetCurrentNode()), XmlQueryTypeFactory.DoubleX);
			case XPathBuilder.FuncId.Boolean:
				return this.f.ConvertToBoolean(args[0]);
			case XPathBuilder.FuncId.True:
				return this.f.True();
			case XPathBuilder.FuncId.False:
				return this.f.False();
			case XPathBuilder.FuncId.Not:
				return this.f.Not(args[0]);
			case XPathBuilder.FuncId.Id:
				return this.f.DocOrderDistinct(this.f.Id(this.GetCurrentNode(), args[0]));
			case XPathBuilder.FuncId.Concat:
				return this.f.StrConcat(args);
			case XPathBuilder.FuncId.StartsWith:
				return this.f.InvokeStartsWith(args[0], args[1]);
			case XPathBuilder.FuncId.Contains:
				return this.f.InvokeContains(args[0], args[1]);
			case XPathBuilder.FuncId.SubstringBefore:
				return this.f.InvokeSubstringBefore(args[0], args[1]);
			case XPathBuilder.FuncId.SubstringAfter:
				return this.f.InvokeSubstringAfter(args[0], args[1]);
			case XPathBuilder.FuncId.Substring:
				if (args.Count != 2)
				{
					return this.f.InvokeSubstring(args[0], args[1], args[2]);
				}
				return this.f.InvokeSubstring(args[0], args[1]);
			case XPathBuilder.FuncId.StringLength:
				return this.f.XsltConvert(this.f.StrLength((args.Count == 0) ? this.f.XPathNodeValue(this.GetCurrentNode()) : args[0]), XmlQueryTypeFactory.DoubleX);
			case XPathBuilder.FuncId.Normalize:
				return this.f.InvokeNormalizeSpace((args.Count == 0) ? this.f.XPathNodeValue(this.GetCurrentNode()) : args[0]);
			case XPathBuilder.FuncId.Translate:
				return this.f.InvokeTranslate(args[0], args[1], args[2]);
			case XPathBuilder.FuncId.Lang:
				return this.f.InvokeLang(args[0], this.GetCurrentNode());
			case XPathBuilder.FuncId.Sum:
				return this.Sum(this.f.DocOrderDistinct(args[0]));
			case XPathBuilder.FuncId.Floor:
				return this.f.InvokeFloor(args[0]);
			case XPathBuilder.FuncId.Ceiling:
				return this.f.InvokeCeiling(args[0]);
			case XPathBuilder.FuncId.Round:
				return this.f.InvokeRound(args[0]);
			default:
				return null;
			}
		}

		private QilNode LocalNameOfFirstNode(QilNode arg)
		{
			if (arg.XmlType.IsSingleton)
			{
				return this.f.LocalNameOf(arg);
			}
			QilIterator qilIterator;
			return this.f.StrConcat(this.f.Loop(qilIterator = this.f.FirstNode(arg), this.f.LocalNameOf(qilIterator)));
		}

		private QilNode NamespaceOfFirstNode(QilNode arg)
		{
			if (arg.XmlType.IsSingleton)
			{
				return this.f.NamespaceUriOf(arg);
			}
			QilIterator qilIterator;
			return this.f.StrConcat(this.f.Loop(qilIterator = this.f.FirstNode(arg), this.f.NamespaceUriOf(qilIterator)));
		}

		private QilNode NameOf(QilNode arg)
		{
			if (arg is QilIterator)
			{
				QilIterator qilIterator;
				QilIterator qilIterator2;
				return this.f.Loop(qilIterator = this.f.Let(this.f.PrefixOf(arg)), this.f.Loop(qilIterator2 = this.f.Let(this.f.LocalNameOf(arg)), this.f.Conditional(this.f.Eq(this.f.StrLength(qilIterator), this.f.Int32(0)), qilIterator2, this.f.StrConcat(new QilNode[]
				{
					qilIterator,
					this.f.String(":"),
					qilIterator2
				}))));
			}
			QilIterator qilIterator3 = this.f.Let(arg);
			return this.f.Loop(qilIterator3, this.NameOf(qilIterator3));
		}

		private QilNode NameOfFirstNode(QilNode arg)
		{
			if (arg.XmlType.IsSingleton)
			{
				return this.NameOf(arg);
			}
			QilIterator qilIterator;
			return this.f.StrConcat(this.f.Loop(qilIterator = this.f.FirstNode(arg), this.NameOf(qilIterator)));
		}

		private QilNode Sum(QilNode arg)
		{
			QilIterator qilIterator;
			return this.f.Sum(this.f.Sequence(this.f.Double(0.0), this.f.Loop(qilIterator = this.f.For(arg), this.f.ConvertToNumber(qilIterator))));
		}

		private static Dictionary<string, XPathBuilder.FunctionInfo<XPathBuilder.FuncId>> CreateFunctionTable()
		{
			return new Dictionary<string, XPathBuilder.FunctionInfo<XPathBuilder.FuncId>>(36)
			{
				{
					"last",
					new XPathBuilder.FunctionInfo<XPathBuilder.FuncId>(XPathBuilder.FuncId.Last, 0, 0, null)
				},
				{
					"position",
					new XPathBuilder.FunctionInfo<XPathBuilder.FuncId>(XPathBuilder.FuncId.Position, 0, 0, null)
				},
				{
					"name",
					new XPathBuilder.FunctionInfo<XPathBuilder.FuncId>(XPathBuilder.FuncId.Name, 0, 1, XPathBuilder.argNodeSet)
				},
				{
					"namespace-uri",
					new XPathBuilder.FunctionInfo<XPathBuilder.FuncId>(XPathBuilder.FuncId.NamespaceUri, 0, 1, XPathBuilder.argNodeSet)
				},
				{
					"local-name",
					new XPathBuilder.FunctionInfo<XPathBuilder.FuncId>(XPathBuilder.FuncId.LocalName, 0, 1, XPathBuilder.argNodeSet)
				},
				{
					"count",
					new XPathBuilder.FunctionInfo<XPathBuilder.FuncId>(XPathBuilder.FuncId.Count, 1, 1, XPathBuilder.argNodeSet)
				},
				{
					"id",
					new XPathBuilder.FunctionInfo<XPathBuilder.FuncId>(XPathBuilder.FuncId.Id, 1, 1, XPathBuilder.argAny)
				},
				{
					"string",
					new XPathBuilder.FunctionInfo<XPathBuilder.FuncId>(XPathBuilder.FuncId.String, 0, 1, XPathBuilder.argAny)
				},
				{
					"concat",
					new XPathBuilder.FunctionInfo<XPathBuilder.FuncId>(XPathBuilder.FuncId.Concat, 2, int.MaxValue, null)
				},
				{
					"starts-with",
					new XPathBuilder.FunctionInfo<XPathBuilder.FuncId>(XPathBuilder.FuncId.StartsWith, 2, 2, XPathBuilder.argString2)
				},
				{
					"contains",
					new XPathBuilder.FunctionInfo<XPathBuilder.FuncId>(XPathBuilder.FuncId.Contains, 2, 2, XPathBuilder.argString2)
				},
				{
					"substring-before",
					new XPathBuilder.FunctionInfo<XPathBuilder.FuncId>(XPathBuilder.FuncId.SubstringBefore, 2, 2, XPathBuilder.argString2)
				},
				{
					"substring-after",
					new XPathBuilder.FunctionInfo<XPathBuilder.FuncId>(XPathBuilder.FuncId.SubstringAfter, 2, 2, XPathBuilder.argString2)
				},
				{
					"substring",
					new XPathBuilder.FunctionInfo<XPathBuilder.FuncId>(XPathBuilder.FuncId.Substring, 2, 3, XPathBuilder.argFnSubstr)
				},
				{
					"string-length",
					new XPathBuilder.FunctionInfo<XPathBuilder.FuncId>(XPathBuilder.FuncId.StringLength, 0, 1, XPathBuilder.argString)
				},
				{
					"normalize-space",
					new XPathBuilder.FunctionInfo<XPathBuilder.FuncId>(XPathBuilder.FuncId.Normalize, 0, 1, XPathBuilder.argString)
				},
				{
					"translate",
					new XPathBuilder.FunctionInfo<XPathBuilder.FuncId>(XPathBuilder.FuncId.Translate, 3, 3, XPathBuilder.argString3)
				},
				{
					"boolean",
					new XPathBuilder.FunctionInfo<XPathBuilder.FuncId>(XPathBuilder.FuncId.Boolean, 1, 1, XPathBuilder.argAny)
				},
				{
					"not",
					new XPathBuilder.FunctionInfo<XPathBuilder.FuncId>(XPathBuilder.FuncId.Not, 1, 1, XPathBuilder.argBoolean)
				},
				{
					"true",
					new XPathBuilder.FunctionInfo<XPathBuilder.FuncId>(XPathBuilder.FuncId.True, 0, 0, null)
				},
				{
					"false",
					new XPathBuilder.FunctionInfo<XPathBuilder.FuncId>(XPathBuilder.FuncId.False, 0, 0, null)
				},
				{
					"lang",
					new XPathBuilder.FunctionInfo<XPathBuilder.FuncId>(XPathBuilder.FuncId.Lang, 1, 1, XPathBuilder.argString)
				},
				{
					"number",
					new XPathBuilder.FunctionInfo<XPathBuilder.FuncId>(XPathBuilder.FuncId.Number, 0, 1, XPathBuilder.argAny)
				},
				{
					"sum",
					new XPathBuilder.FunctionInfo<XPathBuilder.FuncId>(XPathBuilder.FuncId.Sum, 1, 1, XPathBuilder.argNodeSet)
				},
				{
					"floor",
					new XPathBuilder.FunctionInfo<XPathBuilder.FuncId>(XPathBuilder.FuncId.Floor, 1, 1, XPathBuilder.argDouble)
				},
				{
					"ceiling",
					new XPathBuilder.FunctionInfo<XPathBuilder.FuncId>(XPathBuilder.FuncId.Ceiling, 1, 1, XPathBuilder.argDouble)
				},
				{
					"round",
					new XPathBuilder.FunctionInfo<XPathBuilder.FuncId>(XPathBuilder.FuncId.Round, 1, 1, XPathBuilder.argDouble)
				}
			};
		}

		public static bool IsFunctionAvailable(string localName, string nsUri)
		{
			return nsUri.Length == 0 && XPathBuilder.FunctionTable.ContainsKey(localName);
		}

		private XPathQilFactory f;

		private IXPathEnvironment environment;

		private bool inTheBuild;

		protected QilNode fixupCurrent;

		protected QilNode fixupPosition;

		protected QilNode fixupLast;

		protected int numFixupCurrent;

		protected int numFixupPosition;

		protected int numFixupLast;

		private XPathBuilder.FixupVisitor fixupVisitor;

		private static XmlNodeKindFlags[] XPathNodeType2QilXmlNodeKind = new XmlNodeKindFlags[]
		{
			XmlNodeKindFlags.Document,
			XmlNodeKindFlags.Element,
			XmlNodeKindFlags.Attribute,
			XmlNodeKindFlags.Namespace,
			XmlNodeKindFlags.Text,
			XmlNodeKindFlags.Text,
			XmlNodeKindFlags.Text,
			XmlNodeKindFlags.PI,
			XmlNodeKindFlags.Comment,
			XmlNodeKindFlags.Any
		};

		private static XPathBuilder.XPathOperatorGroup[] OperatorGroup = new XPathBuilder.XPathOperatorGroup[]
		{
			XPathBuilder.XPathOperatorGroup.Unknown,
			XPathBuilder.XPathOperatorGroup.Logical,
			XPathBuilder.XPathOperatorGroup.Logical,
			XPathBuilder.XPathOperatorGroup.Equality,
			XPathBuilder.XPathOperatorGroup.Equality,
			XPathBuilder.XPathOperatorGroup.Relational,
			XPathBuilder.XPathOperatorGroup.Relational,
			XPathBuilder.XPathOperatorGroup.Relational,
			XPathBuilder.XPathOperatorGroup.Relational,
			XPathBuilder.XPathOperatorGroup.Arithmetic,
			XPathBuilder.XPathOperatorGroup.Arithmetic,
			XPathBuilder.XPathOperatorGroup.Arithmetic,
			XPathBuilder.XPathOperatorGroup.Arithmetic,
			XPathBuilder.XPathOperatorGroup.Arithmetic,
			XPathBuilder.XPathOperatorGroup.Negate,
			XPathBuilder.XPathOperatorGroup.Union
		};

		private static QilNodeType[] QilOperator = new QilNodeType[]
		{
			QilNodeType.Unknown,
			QilNodeType.Or,
			QilNodeType.And,
			QilNodeType.Eq,
			QilNodeType.Ne,
			QilNodeType.Lt,
			QilNodeType.Le,
			QilNodeType.Gt,
			QilNodeType.Ge,
			QilNodeType.Add,
			QilNodeType.Subtract,
			QilNodeType.Multiply,
			QilNodeType.Divide,
			QilNodeType.Modulo,
			QilNodeType.Negate,
			QilNodeType.Sequence
		};

		private static XmlNodeKindFlags[] XPathAxisMask = new XmlNodeKindFlags[]
		{
			XmlNodeKindFlags.None,
			XmlNodeKindFlags.Document | XmlNodeKindFlags.Element,
			XmlNodeKindFlags.Any,
			XmlNodeKindFlags.Attribute,
			XmlNodeKindFlags.Content,
			XmlNodeKindFlags.Content,
			XmlNodeKindFlags.Any,
			XmlNodeKindFlags.Content,
			XmlNodeKindFlags.Content,
			XmlNodeKindFlags.Namespace,
			XmlNodeKindFlags.Document | XmlNodeKindFlags.Element,
			XmlNodeKindFlags.Content,
			XmlNodeKindFlags.Content,
			XmlNodeKindFlags.Any,
			XmlNodeKindFlags.Document
		};

		public static readonly XmlTypeCode[] argAny = new XmlTypeCode[] { XmlTypeCode.Item };

		public static readonly XmlTypeCode[] argNodeSet = new XmlTypeCode[] { XmlTypeCode.Node };

		public static readonly XmlTypeCode[] argBoolean = new XmlTypeCode[] { XmlTypeCode.Boolean };

		public static readonly XmlTypeCode[] argDouble = new XmlTypeCode[] { XmlTypeCode.Double };

		public static readonly XmlTypeCode[] argString = new XmlTypeCode[] { XmlTypeCode.String };

		public static readonly XmlTypeCode[] argString2 = new XmlTypeCode[]
		{
			XmlTypeCode.String,
			XmlTypeCode.String
		};

		public static readonly XmlTypeCode[] argString3 = new XmlTypeCode[]
		{
			XmlTypeCode.String,
			XmlTypeCode.String,
			XmlTypeCode.String
		};

		public static readonly XmlTypeCode[] argFnSubstr = new XmlTypeCode[]
		{
			XmlTypeCode.String,
			XmlTypeCode.Double,
			XmlTypeCode.Double
		};

		public static Dictionary<string, XPathBuilder.FunctionInfo<XPathBuilder.FuncId>> FunctionTable = XPathBuilder.CreateFunctionTable();

		private enum XPathOperatorGroup
		{
			Unknown,
			Logical,
			Equality,
			Relational,
			Arithmetic,
			Negate,
			Union
		}

		internal enum FuncId
		{
			Last,
			Position,
			Count,
			LocalName,
			NamespaceUri,
			Name,
			String,
			Number,
			Boolean,
			True,
			False,
			Not,
			Id,
			Concat,
			StartsWith,
			Contains,
			SubstringBefore,
			SubstringAfter,
			Substring,
			StringLength,
			Normalize,
			Translate,
			Lang,
			Sum,
			Floor,
			Ceiling,
			Round
		}

		internal class FixupVisitor : QilReplaceVisitor
		{
			public FixupVisitor(QilPatternFactory f, QilNode fixupCurrent, QilNode fixupPosition, QilNode fixupLast)
				: base(f.BaseFactory)
			{
				this.f = f;
				this.fixupCurrent = fixupCurrent;
				this.fixupPosition = fixupPosition;
				this.fixupLast = fixupLast;
			}

			public QilNode Fixup(QilNode inExpr, QilIterator current, QilNode last)
			{
				QilDepthChecker.Check(inExpr);
				this.current = current;
				this.last = last;
				this.justCount = false;
				this.environment = null;
				this.numCurrent = (this.numPosition = (this.numLast = 0));
				inExpr = this.VisitAssumeReference(inExpr);
				return inExpr;
			}

			public QilNode Fixup(QilNode inExpr, IXPathEnvironment environment)
			{
				QilDepthChecker.Check(inExpr);
				this.justCount = false;
				this.current = null;
				this.environment = environment;
				this.numCurrent = (this.numPosition = (this.numLast = 0));
				inExpr = this.VisitAssumeReference(inExpr);
				return inExpr;
			}

			public int CountUnfixedLast(QilNode inExpr)
			{
				this.justCount = true;
				this.numCurrent = (this.numPosition = (this.numLast = 0));
				this.VisitAssumeReference(inExpr);
				return this.numLast;
			}

			protected override QilNode VisitUnknown(QilNode unknown)
			{
				if (unknown == this.fixupCurrent)
				{
					this.numCurrent++;
					if (!this.justCount)
					{
						if (this.environment != null)
						{
							unknown = this.environment.GetCurrent();
						}
						else if (this.current != null)
						{
							unknown = this.current;
						}
					}
				}
				else if (unknown == this.fixupPosition)
				{
					this.numPosition++;
					if (!this.justCount)
					{
						if (this.environment != null)
						{
							unknown = this.environment.GetPosition();
						}
						else if (this.current != null)
						{
							unknown = this.f.XsltConvert(this.f.PositionOf(this.current), XmlQueryTypeFactory.DoubleX);
						}
					}
				}
				else if (unknown == this.fixupLast)
				{
					this.numLast++;
					if (!this.justCount)
					{
						if (this.environment != null)
						{
							unknown = this.environment.GetLast();
						}
						else if (this.current != null)
						{
							unknown = this.last;
						}
					}
				}
				return unknown;
			}

			private new QilPatternFactory f;

			private QilNode fixupCurrent;

			private QilNode fixupPosition;

			private QilNode fixupLast;

			private QilIterator current;

			private QilNode last;

			private bool justCount;

			private IXPathEnvironment environment;

			public int numCurrent;

			public int numPosition;

			public int numLast;
		}

		internal class FunctionInfo<T>
		{
			public FunctionInfo(T id, int minArgs, int maxArgs, XmlTypeCode[] argTypes)
			{
				this.id = id;
				this.minArgs = minArgs;
				this.maxArgs = maxArgs;
				this.argTypes = argTypes;
			}

			public static void CheckArity(int minArgs, int maxArgs, string name, int numArgs)
			{
				if (minArgs <= numArgs && numArgs <= maxArgs)
				{
					return;
				}
				string text;
				if (minArgs == maxArgs)
				{
					text = "Function '{0}()' must have {1} argument(s).";
				}
				else if (maxArgs == minArgs + 1)
				{
					text = "Function '{0}()' must have {1} or {2} argument(s).";
				}
				else if (numArgs < minArgs)
				{
					text = "Function '{0}()' must have at least {1} argument(s).";
				}
				else
				{
					text = "Function '{0}()' must have no more than {2} arguments.";
				}
				throw new XPathCompileException(text, new string[]
				{
					name,
					minArgs.ToString(CultureInfo.InvariantCulture),
					maxArgs.ToString(CultureInfo.InvariantCulture)
				});
			}

			public void CastArguments(IList<QilNode> args, string name, XPathQilFactory f)
			{
				XPathBuilder.FunctionInfo<T>.CheckArity(this.minArgs, this.maxArgs, name, args.Count);
				if (this.maxArgs == 2147483647)
				{
					for (int i = 0; i < args.Count; i++)
					{
						args[i] = f.ConvertToType(XmlTypeCode.String, args[i]);
					}
					return;
				}
				for (int j = 0; j < args.Count; j++)
				{
					if (this.argTypes[j] == XmlTypeCode.Node && f.CannotBeNodeSet(args[j]))
					{
						throw new XPathCompileException("Argument {1} of function '{0}()' cannot be converted to a node-set.", new string[]
						{
							name,
							(j + 1).ToString(CultureInfo.InvariantCulture)
						});
					}
					args[j] = f.ConvertToType(this.argTypes[j], args[j]);
				}
			}

			public T id;

			public int minArgs;

			public int maxArgs;

			public XmlTypeCode[] argTypes;

			public const int Infinity = 2147483647;
		}
	}
}
