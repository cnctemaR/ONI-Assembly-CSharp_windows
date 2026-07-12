using System;
using System.Diagnostics;
using System.Xml.Schema;

namespace System.Xml.Xsl.Qil
{
	internal class QilTypeChecker
	{
		public XmlQueryType Check(QilNode n)
		{
			switch (n.NodeType)
			{
			case QilNodeType.QilExpression:
				return this.CheckQilExpression((QilExpression)n);
			case QilNodeType.FunctionList:
				return this.CheckFunctionList((QilList)n);
			case QilNodeType.GlobalVariableList:
				return this.CheckGlobalVariableList((QilList)n);
			case QilNodeType.GlobalParameterList:
				return this.CheckGlobalParameterList((QilList)n);
			case QilNodeType.ActualParameterList:
				return this.CheckActualParameterList((QilList)n);
			case QilNodeType.FormalParameterList:
				return this.CheckFormalParameterList((QilList)n);
			case QilNodeType.SortKeyList:
				return this.CheckSortKeyList((QilList)n);
			case QilNodeType.BranchList:
				return this.CheckBranchList((QilList)n);
			case QilNodeType.OptimizeBarrier:
				return this.CheckOptimizeBarrier((QilUnary)n);
			case QilNodeType.Unknown:
				return this.CheckUnknown(n);
			case QilNodeType.DataSource:
				return this.CheckDataSource((QilDataSource)n);
			case QilNodeType.Nop:
				return this.CheckNop((QilUnary)n);
			case QilNodeType.Error:
				return this.CheckError((QilUnary)n);
			case QilNodeType.Warning:
				return this.CheckWarning((QilUnary)n);
			case QilNodeType.For:
				return this.CheckFor((QilIterator)n);
			case QilNodeType.Let:
				return this.CheckLet((QilIterator)n);
			case QilNodeType.Parameter:
				return this.CheckParameter((QilParameter)n);
			case QilNodeType.PositionOf:
				return this.CheckPositionOf((QilUnary)n);
			case QilNodeType.True:
				return this.CheckTrue(n);
			case QilNodeType.False:
				return this.CheckFalse(n);
			case QilNodeType.LiteralString:
				return this.CheckLiteralString((QilLiteral)n);
			case QilNodeType.LiteralInt32:
				return this.CheckLiteralInt32((QilLiteral)n);
			case QilNodeType.LiteralInt64:
				return this.CheckLiteralInt64((QilLiteral)n);
			case QilNodeType.LiteralDouble:
				return this.CheckLiteralDouble((QilLiteral)n);
			case QilNodeType.LiteralDecimal:
				return this.CheckLiteralDecimal((QilLiteral)n);
			case QilNodeType.LiteralQName:
				return this.CheckLiteralQName((QilName)n);
			case QilNodeType.LiteralType:
				return this.CheckLiteralType((QilLiteral)n);
			case QilNodeType.LiteralObject:
				return this.CheckLiteralObject((QilLiteral)n);
			case QilNodeType.And:
				return this.CheckAnd((QilBinary)n);
			case QilNodeType.Or:
				return this.CheckOr((QilBinary)n);
			case QilNodeType.Not:
				return this.CheckNot((QilUnary)n);
			case QilNodeType.Conditional:
				return this.CheckConditional((QilTernary)n);
			case QilNodeType.Choice:
				return this.CheckChoice((QilChoice)n);
			case QilNodeType.Length:
				return this.CheckLength((QilUnary)n);
			case QilNodeType.Sequence:
				return this.CheckSequence((QilList)n);
			case QilNodeType.Union:
				return this.CheckUnion((QilBinary)n);
			case QilNodeType.Intersection:
				return this.CheckIntersection((QilBinary)n);
			case QilNodeType.Difference:
				return this.CheckDifference((QilBinary)n);
			case QilNodeType.Average:
				return this.CheckAverage((QilUnary)n);
			case QilNodeType.Sum:
				return this.CheckSum((QilUnary)n);
			case QilNodeType.Minimum:
				return this.CheckMinimum((QilUnary)n);
			case QilNodeType.Maximum:
				return this.CheckMaximum((QilUnary)n);
			case QilNodeType.Negate:
				return this.CheckNegate((QilUnary)n);
			case QilNodeType.Add:
				return this.CheckAdd((QilBinary)n);
			case QilNodeType.Subtract:
				return this.CheckSubtract((QilBinary)n);
			case QilNodeType.Multiply:
				return this.CheckMultiply((QilBinary)n);
			case QilNodeType.Divide:
				return this.CheckDivide((QilBinary)n);
			case QilNodeType.Modulo:
				return this.CheckModulo((QilBinary)n);
			case QilNodeType.StrLength:
				return this.CheckStrLength((QilUnary)n);
			case QilNodeType.StrConcat:
				return this.CheckStrConcat((QilStrConcat)n);
			case QilNodeType.StrParseQName:
				return this.CheckStrParseQName((QilBinary)n);
			case QilNodeType.Ne:
				return this.CheckNe((QilBinary)n);
			case QilNodeType.Eq:
				return this.CheckEq((QilBinary)n);
			case QilNodeType.Gt:
				return this.CheckGt((QilBinary)n);
			case QilNodeType.Ge:
				return this.CheckGe((QilBinary)n);
			case QilNodeType.Lt:
				return this.CheckLt((QilBinary)n);
			case QilNodeType.Le:
				return this.CheckLe((QilBinary)n);
			case QilNodeType.Is:
				return this.CheckIs((QilBinary)n);
			case QilNodeType.After:
				return this.CheckAfter((QilBinary)n);
			case QilNodeType.Before:
				return this.CheckBefore((QilBinary)n);
			case QilNodeType.Loop:
				return this.CheckLoop((QilLoop)n);
			case QilNodeType.Filter:
				return this.CheckFilter((QilLoop)n);
			case QilNodeType.Sort:
				return this.CheckSort((QilLoop)n);
			case QilNodeType.SortKey:
				return this.CheckSortKey((QilSortKey)n);
			case QilNodeType.DocOrderDistinct:
				return this.CheckDocOrderDistinct((QilUnary)n);
			case QilNodeType.Function:
				return this.CheckFunction((QilFunction)n);
			case QilNodeType.Invoke:
				return this.CheckInvoke((QilInvoke)n);
			case QilNodeType.Content:
				return this.CheckContent((QilUnary)n);
			case QilNodeType.Attribute:
				return this.CheckAttribute((QilBinary)n);
			case QilNodeType.Parent:
				return this.CheckParent((QilUnary)n);
			case QilNodeType.Root:
				return this.CheckRoot((QilUnary)n);
			case QilNodeType.XmlContext:
				return this.CheckXmlContext(n);
			case QilNodeType.Descendant:
				return this.CheckDescendant((QilUnary)n);
			case QilNodeType.DescendantOrSelf:
				return this.CheckDescendantOrSelf((QilUnary)n);
			case QilNodeType.Ancestor:
				return this.CheckAncestor((QilUnary)n);
			case QilNodeType.AncestorOrSelf:
				return this.CheckAncestorOrSelf((QilUnary)n);
			case QilNodeType.Preceding:
				return this.CheckPreceding((QilUnary)n);
			case QilNodeType.FollowingSibling:
				return this.CheckFollowingSibling((QilUnary)n);
			case QilNodeType.PrecedingSibling:
				return this.CheckPrecedingSibling((QilUnary)n);
			case QilNodeType.NodeRange:
				return this.CheckNodeRange((QilBinary)n);
			case QilNodeType.Deref:
				return this.CheckDeref((QilBinary)n);
			case QilNodeType.ElementCtor:
				return this.CheckElementCtor((QilBinary)n);
			case QilNodeType.AttributeCtor:
				return this.CheckAttributeCtor((QilBinary)n);
			case QilNodeType.CommentCtor:
				return this.CheckCommentCtor((QilUnary)n);
			case QilNodeType.PICtor:
				return this.CheckPICtor((QilBinary)n);
			case QilNodeType.TextCtor:
				return this.CheckTextCtor((QilUnary)n);
			case QilNodeType.RawTextCtor:
				return this.CheckRawTextCtor((QilUnary)n);
			case QilNodeType.DocumentCtor:
				return this.CheckDocumentCtor((QilUnary)n);
			case QilNodeType.NamespaceDecl:
				return this.CheckNamespaceDecl((QilBinary)n);
			case QilNodeType.RtfCtor:
				return this.CheckRtfCtor((QilBinary)n);
			case QilNodeType.NameOf:
				return this.CheckNameOf((QilUnary)n);
			case QilNodeType.LocalNameOf:
				return this.CheckLocalNameOf((QilUnary)n);
			case QilNodeType.NamespaceUriOf:
				return this.CheckNamespaceUriOf((QilUnary)n);
			case QilNodeType.PrefixOf:
				return this.CheckPrefixOf((QilUnary)n);
			case QilNodeType.TypeAssert:
				return this.CheckTypeAssert((QilTargetType)n);
			case QilNodeType.IsType:
				return this.CheckIsType((QilTargetType)n);
			case QilNodeType.IsEmpty:
				return this.CheckIsEmpty((QilUnary)n);
			case QilNodeType.XPathNodeValue:
				return this.CheckXPathNodeValue((QilUnary)n);
			case QilNodeType.XPathFollowing:
				return this.CheckXPathFollowing((QilUnary)n);
			case QilNodeType.XPathPreceding:
				return this.CheckXPathPreceding((QilUnary)n);
			case QilNodeType.XPathNamespace:
				return this.CheckXPathNamespace((QilUnary)n);
			case QilNodeType.XsltGenerateId:
				return this.CheckXsltGenerateId((QilUnary)n);
			case QilNodeType.XsltInvokeLateBound:
				return this.CheckXsltInvokeLateBound((QilInvokeLateBound)n);
			case QilNodeType.XsltInvokeEarlyBound:
				return this.CheckXsltInvokeEarlyBound((QilInvokeEarlyBound)n);
			case QilNodeType.XsltCopy:
				return this.CheckXsltCopy((QilBinary)n);
			case QilNodeType.XsltCopyOf:
				return this.CheckXsltCopyOf((QilUnary)n);
			case QilNodeType.XsltConvert:
				return this.CheckXsltConvert((QilTargetType)n);
			default:
				return this.CheckUnknown(n);
			}
		}

		public XmlQueryType CheckQilExpression(QilExpression node)
		{
			return XmlQueryTypeFactory.ItemS;
		}

		public XmlQueryType CheckFunctionList(QilList node)
		{
			foreach (QilNode qilNode in node)
			{
			}
			return node.XmlType;
		}

		public XmlQueryType CheckGlobalVariableList(QilList node)
		{
			foreach (QilNode qilNode in node)
			{
			}
			return node.XmlType;
		}

		public XmlQueryType CheckGlobalParameterList(QilList node)
		{
			foreach (QilNode qilNode in node)
			{
			}
			return node.XmlType;
		}

		public XmlQueryType CheckActualParameterList(QilList node)
		{
			return node.XmlType;
		}

		public XmlQueryType CheckFormalParameterList(QilList node)
		{
			foreach (QilNode qilNode in node)
			{
			}
			return node.XmlType;
		}

		public XmlQueryType CheckSortKeyList(QilList node)
		{
			foreach (QilNode qilNode in node)
			{
			}
			return node.XmlType;
		}

		public XmlQueryType CheckBranchList(QilList node)
		{
			return node.XmlType;
		}

		public XmlQueryType CheckOptimizeBarrier(QilUnary node)
		{
			return node.Child.XmlType;
		}

		public XmlQueryType CheckUnknown(QilNode node)
		{
			return node.XmlType;
		}

		public XmlQueryType CheckDataSource(QilDataSource node)
		{
			return XmlQueryTypeFactory.NodeNotRtfQ;
		}

		public XmlQueryType CheckNop(QilUnary node)
		{
			return node.Child.XmlType;
		}

		public XmlQueryType CheckError(QilUnary node)
		{
			return XmlQueryTypeFactory.None;
		}

		public XmlQueryType CheckWarning(QilUnary node)
		{
			return XmlQueryTypeFactory.Empty;
		}

		public XmlQueryType CheckFor(QilIterator node)
		{
			return node.Binding.XmlType.Prime;
		}

		public XmlQueryType CheckLet(QilIterator node)
		{
			return node.Binding.XmlType;
		}

		public XmlQueryType CheckParameter(QilParameter node)
		{
			return node.XmlType;
		}

		public XmlQueryType CheckPositionOf(QilUnary node)
		{
			return XmlQueryTypeFactory.IntX;
		}

		public XmlQueryType CheckTrue(QilNode node)
		{
			return XmlQueryTypeFactory.BooleanX;
		}

		public XmlQueryType CheckFalse(QilNode node)
		{
			return XmlQueryTypeFactory.BooleanX;
		}

		public XmlQueryType CheckLiteralString(QilLiteral node)
		{
			return XmlQueryTypeFactory.StringX;
		}

		public XmlQueryType CheckLiteralInt32(QilLiteral node)
		{
			return XmlQueryTypeFactory.IntX;
		}

		public XmlQueryType CheckLiteralInt64(QilLiteral node)
		{
			return XmlQueryTypeFactory.IntegerX;
		}

		public XmlQueryType CheckLiteralDouble(QilLiteral node)
		{
			return XmlQueryTypeFactory.DoubleX;
		}

		public XmlQueryType CheckLiteralDecimal(QilLiteral node)
		{
			return XmlQueryTypeFactory.DecimalX;
		}

		public XmlQueryType CheckLiteralQName(QilName node)
		{
			return XmlQueryTypeFactory.QNameX;
		}

		public XmlQueryType CheckLiteralType(QilLiteral node)
		{
			return node;
		}

		public XmlQueryType CheckLiteralObject(QilLiteral node)
		{
			return XmlQueryTypeFactory.ItemS;
		}

		public XmlQueryType CheckAnd(QilBinary node)
		{
			return XmlQueryTypeFactory.BooleanX;
		}

		public XmlQueryType CheckOr(QilBinary node)
		{
			return this.CheckAnd(node);
		}

		public XmlQueryType CheckNot(QilUnary node)
		{
			return XmlQueryTypeFactory.BooleanX;
		}

		public XmlQueryType CheckConditional(QilTernary node)
		{
			return XmlQueryTypeFactory.Choice(node.Center.XmlType, node.Right.XmlType);
		}

		public XmlQueryType CheckChoice(QilChoice node)
		{
			return node.Branches.XmlType;
		}

		public XmlQueryType CheckLength(QilUnary node)
		{
			return XmlQueryTypeFactory.IntX;
		}

		public XmlQueryType CheckSequence(QilList node)
		{
			return node.XmlType;
		}

		public XmlQueryType CheckUnion(QilBinary node)
		{
			return this.DistinctType(XmlQueryTypeFactory.Sequence(node.Left.XmlType, node.Right.XmlType));
		}

		public XmlQueryType CheckIntersection(QilBinary node)
		{
			return this.CheckUnion(node);
		}

		public XmlQueryType CheckDifference(QilBinary node)
		{
			return XmlQueryTypeFactory.AtMost(node.Left.XmlType, node.Left.XmlType.Cardinality);
		}

		public XmlQueryType CheckAverage(QilUnary node)
		{
			XmlQueryType xmlType = node.Child.XmlType;
			return XmlQueryTypeFactory.PrimeProduct(xmlType, xmlType.MaybeEmpty ? XmlQueryCardinality.ZeroOrOne : XmlQueryCardinality.One);
		}

		public XmlQueryType CheckSum(QilUnary node)
		{
			return this.CheckAverage(node);
		}

		public XmlQueryType CheckMinimum(QilUnary node)
		{
			return this.CheckAverage(node);
		}

		public XmlQueryType CheckMaximum(QilUnary node)
		{
			return this.CheckAverage(node);
		}

		public XmlQueryType CheckNegate(QilUnary node)
		{
			return node.Child.XmlType;
		}

		public XmlQueryType CheckAdd(QilBinary node)
		{
			if (node.Left.XmlType.TypeCode != XmlTypeCode.None)
			{
				return node.Left.XmlType;
			}
			return node.Right.XmlType;
		}

		public XmlQueryType CheckSubtract(QilBinary node)
		{
			return this.CheckAdd(node);
		}

		public XmlQueryType CheckMultiply(QilBinary node)
		{
			return this.CheckAdd(node);
		}

		public XmlQueryType CheckDivide(QilBinary node)
		{
			return this.CheckAdd(node);
		}

		public XmlQueryType CheckModulo(QilBinary node)
		{
			return this.CheckAdd(node);
		}

		public XmlQueryType CheckStrLength(QilUnary node)
		{
			return XmlQueryTypeFactory.IntX;
		}

		public XmlQueryType CheckStrConcat(QilStrConcat node)
		{
			return XmlQueryTypeFactory.StringX;
		}

		public XmlQueryType CheckStrParseQName(QilBinary node)
		{
			return XmlQueryTypeFactory.QNameX;
		}

		public XmlQueryType CheckNe(QilBinary node)
		{
			return XmlQueryTypeFactory.BooleanX;
		}

		public XmlQueryType CheckEq(QilBinary node)
		{
			return this.CheckNe(node);
		}

		public XmlQueryType CheckGt(QilBinary node)
		{
			return this.CheckNe(node);
		}

		public XmlQueryType CheckGe(QilBinary node)
		{
			return this.CheckNe(node);
		}

		public XmlQueryType CheckLt(QilBinary node)
		{
			return this.CheckNe(node);
		}

		public XmlQueryType CheckLe(QilBinary node)
		{
			return this.CheckNe(node);
		}

		public XmlQueryType CheckIs(QilBinary node)
		{
			return XmlQueryTypeFactory.BooleanX;
		}

		public XmlQueryType CheckAfter(QilBinary node)
		{
			return this.CheckIs(node);
		}

		public XmlQueryType CheckBefore(QilBinary node)
		{
			return this.CheckIs(node);
		}

		public XmlQueryType CheckLoop(QilLoop node)
		{
			XmlQueryType xmlType = node.Body.XmlType;
			XmlQueryCardinality xmlQueryCardinality = ((node.Variable.NodeType == QilNodeType.Let) ? XmlQueryCardinality.One : node.Variable.Binding.XmlType.Cardinality);
			return XmlQueryTypeFactory.PrimeProduct(xmlType, xmlQueryCardinality * xmlType.Cardinality);
		}

		public XmlQueryType CheckFilter(QilLoop node)
		{
			XmlQueryType xmlQueryType = this.FindFilterType(node.Variable, node.Body);
			if (xmlQueryType != null)
			{
				return xmlQueryType;
			}
			return XmlQueryTypeFactory.AtMost(node.Variable.Binding.XmlType, node.Variable.Binding.XmlType.Cardinality);
		}

		public XmlQueryType CheckSort(QilLoop node)
		{
			XmlQueryType xmlType = node.Variable.Binding.XmlType;
			return XmlQueryTypeFactory.PrimeProduct(xmlType, xmlType.Cardinality);
		}

		public XmlQueryType CheckSortKey(QilSortKey node)
		{
			return node.Key.XmlType;
		}

		public XmlQueryType CheckDocOrderDistinct(QilUnary node)
		{
			return this.DistinctType(node.Child.XmlType);
		}

		public XmlQueryType CheckFunction(QilFunction node)
		{
			return node.XmlType;
		}

		public XmlQueryType CheckInvoke(QilInvoke node)
		{
			return node.Function.XmlType;
		}

		public XmlQueryType CheckContent(QilUnary node)
		{
			return XmlQueryTypeFactory.AttributeOrContentS;
		}

		public XmlQueryType CheckAttribute(QilBinary node)
		{
			return XmlQueryTypeFactory.AttributeQ;
		}

		public XmlQueryType CheckParent(QilUnary node)
		{
			return XmlQueryTypeFactory.DocumentOrElementQ;
		}

		public XmlQueryType CheckRoot(QilUnary node)
		{
			return XmlQueryTypeFactory.NodeNotRtf;
		}

		public XmlQueryType CheckXmlContext(QilNode node)
		{
			return XmlQueryTypeFactory.NodeNotRtf;
		}

		public XmlQueryType CheckDescendant(QilUnary node)
		{
			return XmlQueryTypeFactory.ContentS;
		}

		public XmlQueryType CheckDescendantOrSelf(QilUnary node)
		{
			return XmlQueryTypeFactory.Choice(node.Child.XmlType, XmlQueryTypeFactory.ContentS);
		}

		public XmlQueryType CheckAncestor(QilUnary node)
		{
			return XmlQueryTypeFactory.DocumentOrElementS;
		}

		public XmlQueryType CheckAncestorOrSelf(QilUnary node)
		{
			return XmlQueryTypeFactory.Choice(node.Child.XmlType, XmlQueryTypeFactory.DocumentOrElementS);
		}

		public XmlQueryType CheckPreceding(QilUnary node)
		{
			return XmlQueryTypeFactory.DocumentOrContentS;
		}

		public XmlQueryType CheckFollowingSibling(QilUnary node)
		{
			return XmlQueryTypeFactory.ContentS;
		}

		public XmlQueryType CheckPrecedingSibling(QilUnary node)
		{
			return XmlQueryTypeFactory.ContentS;
		}

		public XmlQueryType CheckNodeRange(QilBinary node)
		{
			return XmlQueryTypeFactory.Choice(new XmlQueryType[]
			{
				node.Left.XmlType,
				XmlQueryTypeFactory.ContentS,
				node.Right.XmlType
			});
		}

		public XmlQueryType CheckDeref(QilBinary node)
		{
			return XmlQueryTypeFactory.ElementS;
		}

		public XmlQueryType CheckElementCtor(QilBinary node)
		{
			return XmlQueryTypeFactory.UntypedElement;
		}

		public XmlQueryType CheckAttributeCtor(QilBinary node)
		{
			return XmlQueryTypeFactory.UntypedAttribute;
		}

		public XmlQueryType CheckCommentCtor(QilUnary node)
		{
			return XmlQueryTypeFactory.Comment;
		}

		public XmlQueryType CheckPICtor(QilBinary node)
		{
			return XmlQueryTypeFactory.PI;
		}

		public XmlQueryType CheckTextCtor(QilUnary node)
		{
			return XmlQueryTypeFactory.Text;
		}

		public XmlQueryType CheckRawTextCtor(QilUnary node)
		{
			return XmlQueryTypeFactory.Text;
		}

		public XmlQueryType CheckDocumentCtor(QilUnary node)
		{
			return XmlQueryTypeFactory.UntypedDocument;
		}

		public XmlQueryType CheckNamespaceDecl(QilBinary node)
		{
			return XmlQueryTypeFactory.Namespace;
		}

		public XmlQueryType CheckRtfCtor(QilBinary node)
		{
			return XmlQueryTypeFactory.Node;
		}

		public XmlQueryType CheckNameOf(QilUnary node)
		{
			return XmlQueryTypeFactory.QNameX;
		}

		public XmlQueryType CheckLocalNameOf(QilUnary node)
		{
			return XmlQueryTypeFactory.StringX;
		}

		public XmlQueryType CheckNamespaceUriOf(QilUnary node)
		{
			return XmlQueryTypeFactory.StringX;
		}

		public XmlQueryType CheckPrefixOf(QilUnary node)
		{
			return XmlQueryTypeFactory.StringX;
		}

		public XmlQueryType CheckTypeAssert(QilTargetType node)
		{
			return node.TargetType;
		}

		public XmlQueryType CheckIsType(QilTargetType node)
		{
			return XmlQueryTypeFactory.BooleanX;
		}

		public XmlQueryType CheckIsEmpty(QilUnary node)
		{
			return XmlQueryTypeFactory.BooleanX;
		}

		public XmlQueryType CheckXPathNodeValue(QilUnary node)
		{
			return XmlQueryTypeFactory.StringX;
		}

		public XmlQueryType CheckXPathFollowing(QilUnary node)
		{
			return XmlQueryTypeFactory.ContentS;
		}

		public XmlQueryType CheckXPathPreceding(QilUnary node)
		{
			return XmlQueryTypeFactory.ContentS;
		}

		public XmlQueryType CheckXPathNamespace(QilUnary node)
		{
			return XmlQueryTypeFactory.NamespaceS;
		}

		public XmlQueryType CheckXsltGenerateId(QilUnary node)
		{
			return XmlQueryTypeFactory.StringX;
		}

		public XmlQueryType CheckXsltInvokeLateBound(QilInvokeLateBound node)
		{
			return XmlQueryTypeFactory.ItemS;
		}

		public XmlQueryType CheckXsltInvokeEarlyBound(QilInvokeEarlyBound node)
		{
			return node.XmlType;
		}

		public XmlQueryType CheckXsltCopy(QilBinary node)
		{
			return XmlQueryTypeFactory.Choice(node.Left.XmlType, node.Right.XmlType);
		}

		public XmlQueryType CheckXsltCopyOf(QilUnary node)
		{
			if ((node.Child.XmlType.NodeKinds & XmlNodeKindFlags.Document) != XmlNodeKindFlags.None)
			{
				return XmlQueryTypeFactory.NodeNotRtfS;
			}
			return node.Child.XmlType;
		}

		public XmlQueryType CheckXsltConvert(QilTargetType node)
		{
			return node.TargetType;
		}

		[Conditional("DEBUG")]
		private void Check(bool value, QilNode node, string message)
		{
		}

		[Conditional("DEBUG")]
		private void CheckLiteralValue(QilNode node, Type clrTypeValue)
		{
			((QilLiteral)node).Value.GetType();
		}

		[Conditional("DEBUG")]
		private void CheckClass(QilNode node, Type clrTypeClass)
		{
		}

		[Conditional("DEBUG")]
		private void CheckClassAndNodeType(QilNode node, Type clrTypeClass, QilNodeType nodeType)
		{
		}

		[Conditional("DEBUG")]
		private void CheckXmlType(QilNode node, XmlQueryType xmlType)
		{
		}

		[Conditional("DEBUG")]
		private void CheckNumericX(QilNode node)
		{
		}

		[Conditional("DEBUG")]
		private void CheckNumericXS(QilNode node)
		{
		}

		[Conditional("DEBUG")]
		private void CheckAtomicX(QilNode node)
		{
		}

		[Conditional("DEBUG")]
		private void CheckNotDisjoint(QilBinary node)
		{
		}

		private XmlQueryType DistinctType(XmlQueryType type)
		{
			if (type.Cardinality == XmlQueryCardinality.More)
			{
				return XmlQueryTypeFactory.PrimeProduct(type, XmlQueryCardinality.OneOrMore);
			}
			if (type.Cardinality == XmlQueryCardinality.NotOne)
			{
				return XmlQueryTypeFactory.PrimeProduct(type, XmlQueryCardinality.ZeroOrMore);
			}
			return type;
		}

		private XmlQueryType FindFilterType(QilIterator variable, QilNode body)
		{
			if (body.XmlType.TypeCode == XmlTypeCode.None)
			{
				return XmlQueryTypeFactory.None;
			}
			QilNodeType nodeType = body.NodeType;
			if (nodeType <= QilNodeType.And)
			{
				if (nodeType == QilNodeType.False)
				{
					return XmlQueryTypeFactory.Empty;
				}
				if (nodeType == QilNodeType.And)
				{
					XmlQueryType xmlQueryType = this.FindFilterType(variable, ((QilBinary)body).Left);
					if (xmlQueryType != null)
					{
						return xmlQueryType;
					}
					return this.FindFilterType(variable, ((QilBinary)body).Right);
				}
			}
			else if (nodeType != QilNodeType.Eq)
			{
				if (nodeType == QilNodeType.IsType)
				{
					if (((QilTargetType)body).Source == variable)
					{
						return XmlQueryTypeFactory.AtMost(((QilTargetType)body).TargetType, variable.Binding.XmlType.Cardinality);
					}
				}
			}
			else
			{
				QilBinary qilBinary = (QilBinary)body;
				if (qilBinary.Left.NodeType == QilNodeType.PositionOf && ((QilUnary)qilBinary.Left).Child == variable)
				{
					return XmlQueryTypeFactory.AtMost(variable.Binding.XmlType, XmlQueryCardinality.ZeroOrOne);
				}
			}
			return null;
		}
	}
}
