using System;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Xsl.Qil;
using System.Xml.Xsl.Runtime;

namespace System.Xml.Xsl.XPath
{
	internal class XPathQilFactory : QilPatternFactory
	{
		public XPathQilFactory(QilFactory f, bool debug)
			: base(f, debug)
		{
		}

		public QilNode Error(string res, QilNode args)
		{
			return base.Error(this.InvokeFormatMessage(base.String(res), args));
		}

		public QilNode Error(ISourceLineInfo lineInfo, string res, params string[] args)
		{
			return base.Error(base.String(XslLoadException.CreateMessage(lineInfo, res, args)));
		}

		public QilIterator FirstNode(QilNode n)
		{
			QilIterator qilIterator = base.For(base.DocOrderDistinct(n));
			return base.For(base.Filter(qilIterator, base.Eq(base.PositionOf(qilIterator), base.Int32(1))));
		}

		public bool IsAnyType(QilNode n)
		{
			XmlQueryType xmlType = n.XmlType;
			return !xmlType.IsStrict && !xmlType.IsNode;
		}

		[Conditional("DEBUG")]
		public void CheckAny(QilNode n)
		{
		}

		[Conditional("DEBUG")]
		public void CheckNode(QilNode n)
		{
		}

		[Conditional("DEBUG")]
		public void CheckNodeSet(QilNode n)
		{
		}

		[Conditional("DEBUG")]
		public void CheckNodeNotRtf(QilNode n)
		{
		}

		[Conditional("DEBUG")]
		public void CheckString(QilNode n)
		{
		}

		[Conditional("DEBUG")]
		public void CheckStringS(QilNode n)
		{
		}

		[Conditional("DEBUG")]
		public void CheckDouble(QilNode n)
		{
		}

		[Conditional("DEBUG")]
		public void CheckBool(QilNode n)
		{
		}

		public bool CannotBeNodeSet(QilNode n)
		{
			XmlQueryType xmlType = n.XmlType;
			return xmlType.IsAtomicValue && !xmlType.IsEmpty && !(n is QilIterator);
		}

		public QilNode SafeDocOrderDistinct(QilNode n)
		{
			XmlQueryType xmlType = n.XmlType;
			if (xmlType.MaybeMany)
			{
				if (xmlType.IsNode && xmlType.IsNotRtf)
				{
					return base.DocOrderDistinct(n);
				}
				if (!xmlType.IsAtomicValue)
				{
					QilIterator qilIterator;
					return base.Loop(qilIterator = base.Let(n), base.Conditional(base.Gt(base.Length(qilIterator), base.Int32(1)), base.DocOrderDistinct(base.TypeAssert(qilIterator, XmlQueryTypeFactory.NodeNotRtfS)), qilIterator));
				}
			}
			return n;
		}

		public QilNode InvokeFormatMessage(QilNode res, QilNode args)
		{
			return base.XsltInvokeEarlyBound(base.QName("format-message"), XsltMethods.FormatMessage, XmlQueryTypeFactory.StringX, new QilNode[] { res, args });
		}

		public QilNode InvokeEqualityOperator(QilNodeType op, QilNode left, QilNode right)
		{
			left = base.TypeAssert(left, XmlQueryTypeFactory.ItemS);
			right = base.TypeAssert(right, XmlQueryTypeFactory.ItemS);
			double num;
			if (op == QilNodeType.Eq)
			{
				num = 0.0;
			}
			else
			{
				num = 1.0;
			}
			return base.XsltInvokeEarlyBound(base.QName("EqualityOperator"), XsltMethods.EqualityOperator, XmlQueryTypeFactory.BooleanX, new QilNode[]
			{
				base.Double(num),
				left,
				right
			});
		}

		public QilNode InvokeRelationalOperator(QilNodeType op, QilNode left, QilNode right)
		{
			left = base.TypeAssert(left, XmlQueryTypeFactory.ItemS);
			right = base.TypeAssert(right, XmlQueryTypeFactory.ItemS);
			double num;
			switch (op)
			{
			case QilNodeType.Gt:
				num = 4.0;
				goto IL_0065;
			case QilNodeType.Lt:
				num = 2.0;
				goto IL_0065;
			case QilNodeType.Le:
				num = 3.0;
				goto IL_0065;
			}
			num = 5.0;
			IL_0065:
			return base.XsltInvokeEarlyBound(base.QName("RelationalOperator"), XsltMethods.RelationalOperator, XmlQueryTypeFactory.BooleanX, new QilNode[]
			{
				base.Double(num),
				left,
				right
			});
		}

		[Conditional("DEBUG")]
		private void ExpectAny(QilNode n)
		{
		}

		public QilNode ConvertToType(XmlTypeCode requiredType, QilNode n)
		{
			if (requiredType == XmlTypeCode.Item)
			{
				return n;
			}
			if (requiredType != XmlTypeCode.Node)
			{
				switch (requiredType)
				{
				case XmlTypeCode.String:
					return this.ConvertToString(n);
				case XmlTypeCode.Boolean:
					return this.ConvertToBoolean(n);
				case XmlTypeCode.Double:
					return this.ConvertToNumber(n);
				}
				return null;
			}
			return this.EnsureNodeSet(n);
		}

		public QilNode ConvertToString(QilNode n)
		{
			switch (n.XmlType.TypeCode)
			{
			case XmlTypeCode.String:
				return n;
			case XmlTypeCode.Boolean:
				if (n.NodeType == QilNodeType.True)
				{
					return base.String("true");
				}
				if (n.NodeType != QilNodeType.False)
				{
					return base.Conditional(n, base.String("true"), base.String("false"));
				}
				return base.String("false");
			case XmlTypeCode.Double:
				if (n.NodeType != QilNodeType.LiteralDouble)
				{
					return base.XsltConvert(n, XmlQueryTypeFactory.StringX);
				}
				return base.String(XPathConvert.DoubleToString((QilLiteral)n));
			}
			if (n.XmlType.IsNode)
			{
				return base.XPathNodeValue(this.SafeDocOrderDistinct(n));
			}
			return base.XsltConvert(n, XmlQueryTypeFactory.StringX);
		}

		public QilNode ConvertToBoolean(QilNode n)
		{
			switch (n.XmlType.TypeCode)
			{
			case XmlTypeCode.String:
				if (n.NodeType != QilNodeType.LiteralString)
				{
					return base.Ne(base.StrLength(n), base.Int32(0));
				}
				return base.Boolean(((QilLiteral)n).Length != 0);
			case XmlTypeCode.Boolean:
				return n;
			case XmlTypeCode.Double:
				if (n.NodeType != QilNodeType.LiteralDouble)
				{
					QilIterator qilIterator;
					return base.Loop(qilIterator = base.Let(n), base.Or(base.Lt(qilIterator, base.Double(0.0)), base.Lt(base.Double(0.0), qilIterator)));
				}
				return base.Boolean((QilLiteral)n < 0.0 || 0.0 < (QilLiteral)n);
			}
			if (n.XmlType.IsNode)
			{
				return base.Not(base.IsEmpty(n));
			}
			return base.XsltConvert(n, XmlQueryTypeFactory.BooleanX);
		}

		public QilNode ConvertToNumber(QilNode n)
		{
			switch (n.XmlType.TypeCode)
			{
			case XmlTypeCode.String:
				return base.XsltConvert(n, XmlQueryTypeFactory.DoubleX);
			case XmlTypeCode.Boolean:
				if (n.NodeType == QilNodeType.True)
				{
					return base.Double(1.0);
				}
				if (n.NodeType != QilNodeType.False)
				{
					return base.Conditional(n, base.Double(1.0), base.Double(0.0));
				}
				return base.Double(0.0);
			case XmlTypeCode.Double:
				return n;
			}
			if (n.XmlType.IsNode)
			{
				return base.XsltConvert(base.XPathNodeValue(this.SafeDocOrderDistinct(n)), XmlQueryTypeFactory.DoubleX);
			}
			return base.XsltConvert(n, XmlQueryTypeFactory.DoubleX);
		}

		public QilNode ConvertToNode(QilNode n)
		{
			if (n.XmlType.IsNode && n.XmlType.IsNotRtf && n.XmlType.IsSingleton)
			{
				return n;
			}
			return base.XsltConvert(n, XmlQueryTypeFactory.NodeNotRtf);
		}

		public QilNode ConvertToNodeSet(QilNode n)
		{
			if (n.XmlType.IsNode && n.XmlType.IsNotRtf)
			{
				return n;
			}
			return base.XsltConvert(n, XmlQueryTypeFactory.NodeNotRtfS);
		}

		public QilNode TryEnsureNodeSet(QilNode n)
		{
			if (n.XmlType.IsNode && n.XmlType.IsNotRtf)
			{
				return n;
			}
			if (this.CannotBeNodeSet(n))
			{
				return null;
			}
			return this.InvokeEnsureNodeSet(n);
		}

		public QilNode EnsureNodeSet(QilNode n)
		{
			QilNode qilNode = this.TryEnsureNodeSet(n);
			if (qilNode == null)
			{
				throw new XPathCompileException("Expression must evaluate to a node-set.", Array.Empty<string>());
			}
			return qilNode;
		}

		public QilNode InvokeEnsureNodeSet(QilNode n)
		{
			return base.XsltInvokeEarlyBound(base.QName("ensure-node-set"), XsltMethods.EnsureNodeSet, XmlQueryTypeFactory.NodeSDod, new QilNode[] { n });
		}

		public QilNode Id(QilNode context, QilNode id)
		{
			if (id.XmlType.IsSingleton)
			{
				return base.Deref(context, this.ConvertToString(id));
			}
			QilIterator qilIterator;
			return base.Loop(qilIterator = base.For(id), base.Deref(context, this.ConvertToString(qilIterator)));
		}

		public QilNode InvokeStartsWith(QilNode str1, QilNode str2)
		{
			return base.XsltInvokeEarlyBound(base.QName("starts-with"), XsltMethods.StartsWith, XmlQueryTypeFactory.BooleanX, new QilNode[] { str1, str2 });
		}

		public QilNode InvokeContains(QilNode str1, QilNode str2)
		{
			return base.XsltInvokeEarlyBound(base.QName("contains"), XsltMethods.Contains, XmlQueryTypeFactory.BooleanX, new QilNode[] { str1, str2 });
		}

		public QilNode InvokeSubstringBefore(QilNode str1, QilNode str2)
		{
			return base.XsltInvokeEarlyBound(base.QName("substring-before"), XsltMethods.SubstringBefore, XmlQueryTypeFactory.StringX, new QilNode[] { str1, str2 });
		}

		public QilNode InvokeSubstringAfter(QilNode str1, QilNode str2)
		{
			return base.XsltInvokeEarlyBound(base.QName("substring-after"), XsltMethods.SubstringAfter, XmlQueryTypeFactory.StringX, new QilNode[] { str1, str2 });
		}

		public QilNode InvokeSubstring(QilNode str, QilNode start)
		{
			return base.XsltInvokeEarlyBound(base.QName("substring"), XsltMethods.Substring2, XmlQueryTypeFactory.StringX, new QilNode[] { str, start });
		}

		public QilNode InvokeSubstring(QilNode str, QilNode start, QilNode length)
		{
			return base.XsltInvokeEarlyBound(base.QName("substring"), XsltMethods.Substring3, XmlQueryTypeFactory.StringX, new QilNode[] { str, start, length });
		}

		public QilNode InvokeNormalizeSpace(QilNode str)
		{
			return base.XsltInvokeEarlyBound(base.QName("normalize-space"), XsltMethods.NormalizeSpace, XmlQueryTypeFactory.StringX, new QilNode[] { str });
		}

		public QilNode InvokeTranslate(QilNode str1, QilNode str2, QilNode str3)
		{
			return base.XsltInvokeEarlyBound(base.QName("translate"), XsltMethods.Translate, XmlQueryTypeFactory.StringX, new QilNode[] { str1, str2, str3 });
		}

		public QilNode InvokeLang(QilNode lang, QilNode context)
		{
			return base.XsltInvokeEarlyBound(base.QName("lang"), XsltMethods.Lang, XmlQueryTypeFactory.BooleanX, new QilNode[] { lang, context });
		}

		public QilNode InvokeFloor(QilNode value)
		{
			return base.XsltInvokeEarlyBound(base.QName("floor"), XsltMethods.Floor, XmlQueryTypeFactory.DoubleX, new QilNode[] { value });
		}

		public QilNode InvokeCeiling(QilNode value)
		{
			return base.XsltInvokeEarlyBound(base.QName("ceiling"), XsltMethods.Ceiling, XmlQueryTypeFactory.DoubleX, new QilNode[] { value });
		}

		public QilNode InvokeRound(QilNode value)
		{
			return base.XsltInvokeEarlyBound(base.QName("round"), XsltMethods.Round, XmlQueryTypeFactory.DoubleX, new QilNode[] { value });
		}
	}
}
