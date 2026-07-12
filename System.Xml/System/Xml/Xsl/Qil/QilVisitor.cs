using System;

namespace System.Xml.Xsl.Qil
{
	internal abstract class QilVisitor
	{
		protected virtual QilNode VisitAssumeReference(QilNode expr)
		{
			if (expr is QilReference)
			{
				return this.VisitReference(expr);
			}
			return this.Visit(expr);
		}

		protected virtual QilNode VisitChildren(QilNode parent)
		{
			for (int i = 0; i < parent.Count; i++)
			{
				if (this.IsReference(parent, i))
				{
					this.VisitReference(parent[i]);
				}
				else
				{
					this.Visit(parent[i]);
				}
			}
			return parent;
		}

		protected virtual bool IsReference(QilNode parent, int childNum)
		{
			QilNode qilNode = parent[childNum];
			if (qilNode != null)
			{
				QilNodeType nodeType = qilNode.NodeType;
				if (nodeType - QilNodeType.For <= 2)
				{
					QilNodeType nodeType2 = parent.NodeType;
					return nodeType2 - QilNodeType.GlobalVariableList > 1 && nodeType2 != QilNodeType.FormalParameterList && (nodeType2 - QilNodeType.Loop > 2 || childNum == 1);
				}
				if (nodeType == QilNodeType.Function)
				{
					return parent.NodeType == QilNodeType.Invoke;
				}
			}
			return false;
		}

		protected virtual QilNode Visit(QilNode n)
		{
			if (n == null)
			{
				return this.VisitNull();
			}
			switch (n.NodeType)
			{
			case QilNodeType.QilExpression:
				return this.VisitQilExpression((QilExpression)n);
			case QilNodeType.FunctionList:
				return this.VisitFunctionList((QilList)n);
			case QilNodeType.GlobalVariableList:
				return this.VisitGlobalVariableList((QilList)n);
			case QilNodeType.GlobalParameterList:
				return this.VisitGlobalParameterList((QilList)n);
			case QilNodeType.ActualParameterList:
				return this.VisitActualParameterList((QilList)n);
			case QilNodeType.FormalParameterList:
				return this.VisitFormalParameterList((QilList)n);
			case QilNodeType.SortKeyList:
				return this.VisitSortKeyList((QilList)n);
			case QilNodeType.BranchList:
				return this.VisitBranchList((QilList)n);
			case QilNodeType.OptimizeBarrier:
				return this.VisitOptimizeBarrier((QilUnary)n);
			case QilNodeType.Unknown:
				return this.VisitUnknown(n);
			case QilNodeType.DataSource:
				return this.VisitDataSource((QilDataSource)n);
			case QilNodeType.Nop:
				return this.VisitNop((QilUnary)n);
			case QilNodeType.Error:
				return this.VisitError((QilUnary)n);
			case QilNodeType.Warning:
				return this.VisitWarning((QilUnary)n);
			case QilNodeType.For:
				return this.VisitFor((QilIterator)n);
			case QilNodeType.Let:
				return this.VisitLet((QilIterator)n);
			case QilNodeType.Parameter:
				return this.VisitParameter((QilParameter)n);
			case QilNodeType.PositionOf:
				return this.VisitPositionOf((QilUnary)n);
			case QilNodeType.True:
				return this.VisitTrue(n);
			case QilNodeType.False:
				return this.VisitFalse(n);
			case QilNodeType.LiteralString:
				return this.VisitLiteralString((QilLiteral)n);
			case QilNodeType.LiteralInt32:
				return this.VisitLiteralInt32((QilLiteral)n);
			case QilNodeType.LiteralInt64:
				return this.VisitLiteralInt64((QilLiteral)n);
			case QilNodeType.LiteralDouble:
				return this.VisitLiteralDouble((QilLiteral)n);
			case QilNodeType.LiteralDecimal:
				return this.VisitLiteralDecimal((QilLiteral)n);
			case QilNodeType.LiteralQName:
				return this.VisitLiteralQName((QilName)n);
			case QilNodeType.LiteralType:
				return this.VisitLiteralType((QilLiteral)n);
			case QilNodeType.LiteralObject:
				return this.VisitLiteralObject((QilLiteral)n);
			case QilNodeType.And:
				return this.VisitAnd((QilBinary)n);
			case QilNodeType.Or:
				return this.VisitOr((QilBinary)n);
			case QilNodeType.Not:
				return this.VisitNot((QilUnary)n);
			case QilNodeType.Conditional:
				return this.VisitConditional((QilTernary)n);
			case QilNodeType.Choice:
				return this.VisitChoice((QilChoice)n);
			case QilNodeType.Length:
				return this.VisitLength((QilUnary)n);
			case QilNodeType.Sequence:
				return this.VisitSequence((QilList)n);
			case QilNodeType.Union:
				return this.VisitUnion((QilBinary)n);
			case QilNodeType.Intersection:
				return this.VisitIntersection((QilBinary)n);
			case QilNodeType.Difference:
				return this.VisitDifference((QilBinary)n);
			case QilNodeType.Average:
				return this.VisitAverage((QilUnary)n);
			case QilNodeType.Sum:
				return this.VisitSum((QilUnary)n);
			case QilNodeType.Minimum:
				return this.VisitMinimum((QilUnary)n);
			case QilNodeType.Maximum:
				return this.VisitMaximum((QilUnary)n);
			case QilNodeType.Negate:
				return this.VisitNegate((QilUnary)n);
			case QilNodeType.Add:
				return this.VisitAdd((QilBinary)n);
			case QilNodeType.Subtract:
				return this.VisitSubtract((QilBinary)n);
			case QilNodeType.Multiply:
				return this.VisitMultiply((QilBinary)n);
			case QilNodeType.Divide:
				return this.VisitDivide((QilBinary)n);
			case QilNodeType.Modulo:
				return this.VisitModulo((QilBinary)n);
			case QilNodeType.StrLength:
				return this.VisitStrLength((QilUnary)n);
			case QilNodeType.StrConcat:
				return this.VisitStrConcat((QilStrConcat)n);
			case QilNodeType.StrParseQName:
				return this.VisitStrParseQName((QilBinary)n);
			case QilNodeType.Ne:
				return this.VisitNe((QilBinary)n);
			case QilNodeType.Eq:
				return this.VisitEq((QilBinary)n);
			case QilNodeType.Gt:
				return this.VisitGt((QilBinary)n);
			case QilNodeType.Ge:
				return this.VisitGe((QilBinary)n);
			case QilNodeType.Lt:
				return this.VisitLt((QilBinary)n);
			case QilNodeType.Le:
				return this.VisitLe((QilBinary)n);
			case QilNodeType.Is:
				return this.VisitIs((QilBinary)n);
			case QilNodeType.After:
				return this.VisitAfter((QilBinary)n);
			case QilNodeType.Before:
				return this.VisitBefore((QilBinary)n);
			case QilNodeType.Loop:
				return this.VisitLoop((QilLoop)n);
			case QilNodeType.Filter:
				return this.VisitFilter((QilLoop)n);
			case QilNodeType.Sort:
				return this.VisitSort((QilLoop)n);
			case QilNodeType.SortKey:
				return this.VisitSortKey((QilSortKey)n);
			case QilNodeType.DocOrderDistinct:
				return this.VisitDocOrderDistinct((QilUnary)n);
			case QilNodeType.Function:
				return this.VisitFunction((QilFunction)n);
			case QilNodeType.Invoke:
				return this.VisitInvoke((QilInvoke)n);
			case QilNodeType.Content:
				return this.VisitContent((QilUnary)n);
			case QilNodeType.Attribute:
				return this.VisitAttribute((QilBinary)n);
			case QilNodeType.Parent:
				return this.VisitParent((QilUnary)n);
			case QilNodeType.Root:
				return this.VisitRoot((QilUnary)n);
			case QilNodeType.XmlContext:
				return this.VisitXmlContext(n);
			case QilNodeType.Descendant:
				return this.VisitDescendant((QilUnary)n);
			case QilNodeType.DescendantOrSelf:
				return this.VisitDescendantOrSelf((QilUnary)n);
			case QilNodeType.Ancestor:
				return this.VisitAncestor((QilUnary)n);
			case QilNodeType.AncestorOrSelf:
				return this.VisitAncestorOrSelf((QilUnary)n);
			case QilNodeType.Preceding:
				return this.VisitPreceding((QilUnary)n);
			case QilNodeType.FollowingSibling:
				return this.VisitFollowingSibling((QilUnary)n);
			case QilNodeType.PrecedingSibling:
				return this.VisitPrecedingSibling((QilUnary)n);
			case QilNodeType.NodeRange:
				return this.VisitNodeRange((QilBinary)n);
			case QilNodeType.Deref:
				return this.VisitDeref((QilBinary)n);
			case QilNodeType.ElementCtor:
				return this.VisitElementCtor((QilBinary)n);
			case QilNodeType.AttributeCtor:
				return this.VisitAttributeCtor((QilBinary)n);
			case QilNodeType.CommentCtor:
				return this.VisitCommentCtor((QilUnary)n);
			case QilNodeType.PICtor:
				return this.VisitPICtor((QilBinary)n);
			case QilNodeType.TextCtor:
				return this.VisitTextCtor((QilUnary)n);
			case QilNodeType.RawTextCtor:
				return this.VisitRawTextCtor((QilUnary)n);
			case QilNodeType.DocumentCtor:
				return this.VisitDocumentCtor((QilUnary)n);
			case QilNodeType.NamespaceDecl:
				return this.VisitNamespaceDecl((QilBinary)n);
			case QilNodeType.RtfCtor:
				return this.VisitRtfCtor((QilBinary)n);
			case QilNodeType.NameOf:
				return this.VisitNameOf((QilUnary)n);
			case QilNodeType.LocalNameOf:
				return this.VisitLocalNameOf((QilUnary)n);
			case QilNodeType.NamespaceUriOf:
				return this.VisitNamespaceUriOf((QilUnary)n);
			case QilNodeType.PrefixOf:
				return this.VisitPrefixOf((QilUnary)n);
			case QilNodeType.TypeAssert:
				return this.VisitTypeAssert((QilTargetType)n);
			case QilNodeType.IsType:
				return this.VisitIsType((QilTargetType)n);
			case QilNodeType.IsEmpty:
				return this.VisitIsEmpty((QilUnary)n);
			case QilNodeType.XPathNodeValue:
				return this.VisitXPathNodeValue((QilUnary)n);
			case QilNodeType.XPathFollowing:
				return this.VisitXPathFollowing((QilUnary)n);
			case QilNodeType.XPathPreceding:
				return this.VisitXPathPreceding((QilUnary)n);
			case QilNodeType.XPathNamespace:
				return this.VisitXPathNamespace((QilUnary)n);
			case QilNodeType.XsltGenerateId:
				return this.VisitXsltGenerateId((QilUnary)n);
			case QilNodeType.XsltInvokeLateBound:
				return this.VisitXsltInvokeLateBound((QilInvokeLateBound)n);
			case QilNodeType.XsltInvokeEarlyBound:
				return this.VisitXsltInvokeEarlyBound((QilInvokeEarlyBound)n);
			case QilNodeType.XsltCopy:
				return this.VisitXsltCopy((QilBinary)n);
			case QilNodeType.XsltCopyOf:
				return this.VisitXsltCopyOf((QilUnary)n);
			case QilNodeType.XsltConvert:
				return this.VisitXsltConvert((QilTargetType)n);
			default:
				return this.VisitUnknown(n);
			}
		}

		protected virtual QilNode VisitReference(QilNode n)
		{
			if (n == null)
			{
				return this.VisitNull();
			}
			QilNodeType nodeType = n.NodeType;
			switch (nodeType)
			{
			case QilNodeType.For:
				return this.VisitForReference((QilIterator)n);
			case QilNodeType.Let:
				return this.VisitLetReference((QilIterator)n);
			case QilNodeType.Parameter:
				return this.VisitParameterReference((QilParameter)n);
			default:
				if (nodeType != QilNodeType.Function)
				{
					return this.VisitUnknown(n);
				}
				return this.VisitFunctionReference((QilFunction)n);
			}
		}

		protected virtual QilNode VisitNull()
		{
			return null;
		}

		protected virtual QilNode VisitQilExpression(QilExpression n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitFunctionList(QilList n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitGlobalVariableList(QilList n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitGlobalParameterList(QilList n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitActualParameterList(QilList n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitFormalParameterList(QilList n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitSortKeyList(QilList n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitBranchList(QilList n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitOptimizeBarrier(QilUnary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitUnknown(QilNode n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitDataSource(QilDataSource n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitNop(QilUnary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitError(QilUnary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitWarning(QilUnary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitFor(QilIterator n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitForReference(QilIterator n)
		{
			return n;
		}

		protected virtual QilNode VisitLet(QilIterator n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitLetReference(QilIterator n)
		{
			return n;
		}

		protected virtual QilNode VisitParameter(QilParameter n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitParameterReference(QilParameter n)
		{
			return n;
		}

		protected virtual QilNode VisitPositionOf(QilUnary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitTrue(QilNode n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitFalse(QilNode n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitLiteralString(QilLiteral n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitLiteralInt32(QilLiteral n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitLiteralInt64(QilLiteral n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitLiteralDouble(QilLiteral n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitLiteralDecimal(QilLiteral n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitLiteralQName(QilName n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitLiteralType(QilLiteral n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitLiteralObject(QilLiteral n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitAnd(QilBinary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitOr(QilBinary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitNot(QilUnary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitConditional(QilTernary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitChoice(QilChoice n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitLength(QilUnary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitSequence(QilList n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitUnion(QilBinary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitIntersection(QilBinary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitDifference(QilBinary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitAverage(QilUnary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitSum(QilUnary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitMinimum(QilUnary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitMaximum(QilUnary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitNegate(QilUnary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitAdd(QilBinary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitSubtract(QilBinary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitMultiply(QilBinary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitDivide(QilBinary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitModulo(QilBinary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitStrLength(QilUnary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitStrConcat(QilStrConcat n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitStrParseQName(QilBinary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitNe(QilBinary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitEq(QilBinary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitGt(QilBinary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitGe(QilBinary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitLt(QilBinary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitLe(QilBinary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitIs(QilBinary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitAfter(QilBinary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitBefore(QilBinary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitLoop(QilLoop n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitFilter(QilLoop n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitSort(QilLoop n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitSortKey(QilSortKey n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitDocOrderDistinct(QilUnary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitFunction(QilFunction n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitFunctionReference(QilFunction n)
		{
			return n;
		}

		protected virtual QilNode VisitInvoke(QilInvoke n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitContent(QilUnary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitAttribute(QilBinary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitParent(QilUnary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitRoot(QilUnary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitXmlContext(QilNode n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitDescendant(QilUnary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitDescendantOrSelf(QilUnary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitAncestor(QilUnary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitAncestorOrSelf(QilUnary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitPreceding(QilUnary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitFollowingSibling(QilUnary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitPrecedingSibling(QilUnary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitNodeRange(QilBinary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitDeref(QilBinary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitElementCtor(QilBinary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitAttributeCtor(QilBinary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitCommentCtor(QilUnary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitPICtor(QilBinary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitTextCtor(QilUnary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitRawTextCtor(QilUnary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitDocumentCtor(QilUnary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitNamespaceDecl(QilBinary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitRtfCtor(QilBinary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitNameOf(QilUnary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitLocalNameOf(QilUnary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitNamespaceUriOf(QilUnary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitPrefixOf(QilUnary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitTypeAssert(QilTargetType n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitIsType(QilTargetType n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitIsEmpty(QilUnary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitXPathNodeValue(QilUnary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitXPathFollowing(QilUnary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitXPathPreceding(QilUnary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitXPathNamespace(QilUnary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitXsltGenerateId(QilUnary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitXsltInvokeLateBound(QilInvokeLateBound n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitXsltInvokeEarlyBound(QilInvokeEarlyBound n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitXsltCopy(QilBinary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitXsltCopyOf(QilUnary n)
		{
			return this.VisitChildren(n);
		}

		protected virtual QilNode VisitXsltConvert(QilTargetType n)
		{
			return this.VisitChildren(n);
		}
	}
}
