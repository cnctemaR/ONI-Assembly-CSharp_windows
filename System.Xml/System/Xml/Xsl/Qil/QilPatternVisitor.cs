using System;
using System.Collections;

namespace System.Xml.Xsl.Qil
{
	internal abstract class QilPatternVisitor : QilReplaceVisitor
	{
		public QilPatternVisitor(QilPatternVisitor.QilPatterns patterns, QilFactory f)
			: base(f)
		{
			this.Patterns = patterns;
		}

		public QilPatternVisitor.QilPatterns Patterns
		{
			get
			{
				return this._patterns;
			}
			set
			{
				this._patterns = value;
			}
		}

		public int Threshold
		{
			get
			{
				return this._threshold;
			}
			set
			{
				this._threshold = value;
			}
		}

		public int ReplacementCount
		{
			get
			{
				return this._replacementCnt;
			}
		}

		public int LastReplacement
		{
			get
			{
				return this._lastReplacement;
			}
		}

		public bool Matching
		{
			get
			{
				return this.ReplacementCount < this.Threshold;
			}
		}

		protected virtual bool AllowReplace(int pattern, QilNode original)
		{
			if (this.Matching)
			{
				this._replacementCnt++;
				this._lastReplacement = pattern;
				return true;
			}
			return false;
		}

		protected virtual QilNode Replace(int pattern, QilNode original, QilNode replacement)
		{
			replacement.SourceLine = original.SourceLine;
			return replacement;
		}

		protected virtual QilNode NoReplace(QilNode node)
		{
			return node;
		}

		protected override QilNode Visit(QilNode node)
		{
			if (node == null)
			{
				return this.VisitNull();
			}
			node = this.VisitChildren(node);
			return base.Visit(node);
		}

		protected override QilNode VisitQilExpression(QilExpression n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitFunctionList(QilList n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitGlobalVariableList(QilList n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitGlobalParameterList(QilList n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitActualParameterList(QilList n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitFormalParameterList(QilList n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitSortKeyList(QilList n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitBranchList(QilList n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitOptimizeBarrier(QilUnary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitUnknown(QilNode n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitDataSource(QilDataSource n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitNop(QilUnary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitError(QilUnary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitWarning(QilUnary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitFor(QilIterator n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitForReference(QilIterator n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitLet(QilIterator n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitLetReference(QilIterator n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitParameter(QilParameter n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitParameterReference(QilParameter n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitPositionOf(QilUnary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitTrue(QilNode n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitFalse(QilNode n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitLiteralString(QilLiteral n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitLiteralInt32(QilLiteral n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitLiteralInt64(QilLiteral n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitLiteralDouble(QilLiteral n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitLiteralDecimal(QilLiteral n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitLiteralQName(QilName n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitLiteralType(QilLiteral n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitLiteralObject(QilLiteral n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitAnd(QilBinary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitOr(QilBinary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitNot(QilUnary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitConditional(QilTernary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitChoice(QilChoice n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitLength(QilUnary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitSequence(QilList n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitUnion(QilBinary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitIntersection(QilBinary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitDifference(QilBinary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitAverage(QilUnary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitSum(QilUnary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitMinimum(QilUnary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitMaximum(QilUnary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitNegate(QilUnary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitAdd(QilBinary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitSubtract(QilBinary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitMultiply(QilBinary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitDivide(QilBinary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitModulo(QilBinary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitStrLength(QilUnary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitStrConcat(QilStrConcat n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitStrParseQName(QilBinary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitNe(QilBinary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitEq(QilBinary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitGt(QilBinary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitGe(QilBinary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitLt(QilBinary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitLe(QilBinary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitIs(QilBinary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitAfter(QilBinary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitBefore(QilBinary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitLoop(QilLoop n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitFilter(QilLoop n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitSort(QilLoop n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitSortKey(QilSortKey n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitDocOrderDistinct(QilUnary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitFunction(QilFunction n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitFunctionReference(QilFunction n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitInvoke(QilInvoke n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitContent(QilUnary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitAttribute(QilBinary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitParent(QilUnary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitRoot(QilUnary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitXmlContext(QilNode n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitDescendant(QilUnary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitDescendantOrSelf(QilUnary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitAncestor(QilUnary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitAncestorOrSelf(QilUnary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitPreceding(QilUnary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitFollowingSibling(QilUnary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitPrecedingSibling(QilUnary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitNodeRange(QilBinary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitDeref(QilBinary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitElementCtor(QilBinary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitAttributeCtor(QilBinary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitCommentCtor(QilUnary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitPICtor(QilBinary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitTextCtor(QilUnary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitRawTextCtor(QilUnary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitDocumentCtor(QilUnary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitNamespaceDecl(QilBinary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitRtfCtor(QilBinary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitNameOf(QilUnary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitLocalNameOf(QilUnary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitNamespaceUriOf(QilUnary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitPrefixOf(QilUnary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitTypeAssert(QilTargetType n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitIsType(QilTargetType n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitIsEmpty(QilUnary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitXPathNodeValue(QilUnary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitXPathFollowing(QilUnary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitXPathPreceding(QilUnary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitXPathNamespace(QilUnary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitXsltGenerateId(QilUnary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitXsltInvokeLateBound(QilInvokeLateBound n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitXsltInvokeEarlyBound(QilInvokeEarlyBound n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitXsltCopy(QilBinary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitXsltCopyOf(QilUnary n)
		{
			return this.NoReplace(n);
		}

		protected override QilNode VisitXsltConvert(QilTargetType n)
		{
			return this.NoReplace(n);
		}

		private QilPatternVisitor.QilPatterns _patterns;

		private int _replacementCnt;

		private int _lastReplacement;

		private int _threshold = int.MaxValue;

		internal sealed class QilPatterns
		{
			private QilPatterns(QilPatternVisitor.QilPatterns toCopy)
			{
				this._bits = new BitArray(toCopy._bits);
			}

			public QilPatterns(int szBits, bool allSet)
			{
				this._bits = new BitArray(szBits, allSet);
			}

			public QilPatternVisitor.QilPatterns Clone()
			{
				return new QilPatternVisitor.QilPatterns(this);
			}

			public void ClearAll()
			{
				this._bits.SetAll(false);
			}

			public void Add(int i)
			{
				this._bits.Set(i, true);
			}

			public bool IsSet(int i)
			{
				return this._bits[i];
			}

			private BitArray _bits;
		}
	}
}
