using System;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace MS.Internal.Xml.XPath
{
	internal sealed class LogicalExpr : ValueQuery
	{
		public LogicalExpr(Operator.Op op, Query opnd1, Query opnd2)
		{
			this.op = op;
			this.opnd1 = opnd1;
			this.opnd2 = opnd2;
		}

		private LogicalExpr(LogicalExpr other)
			: base(other)
		{
			this.op = other.op;
			this.opnd1 = Query.Clone(other.opnd1);
			this.opnd2 = Query.Clone(other.opnd2);
		}

		public override void SetXsltContext(XsltContext context)
		{
			this.opnd1.SetXsltContext(context);
			this.opnd2.SetXsltContext(context);
		}

		public override object Evaluate(XPathNodeIterator nodeIterator)
		{
			Operator.Op op = this.op;
			object obj = this.opnd1.Evaluate(nodeIterator);
			object obj2 = this.opnd2.Evaluate(nodeIterator);
			int num = (int)base.GetXPathType(obj);
			int num2 = (int)base.GetXPathType(obj2);
			if (num < num2)
			{
				op = Operator.InvertOperator(op);
				object obj3 = obj;
				obj = obj2;
				obj2 = obj3;
				int num3 = num;
				num = num2;
				num2 = num3;
			}
			if (op == Operator.Op.EQ || op == Operator.Op.NE)
			{
				return LogicalExpr.CompXsltE[num][num2](op, obj, obj2);
			}
			return LogicalExpr.CompXsltO[num][num2](op, obj, obj2);
		}

		private static bool cmpQueryQueryE(Operator.Op op, object val1, object val2)
		{
			bool flag = op == Operator.Op.EQ;
			LogicalExpr.NodeSet nodeSet = new LogicalExpr.NodeSet(val1);
			LogicalExpr.NodeSet nodeSet2 = new LogicalExpr.NodeSet(val2);
			IL_0015:
			while (nodeSet.MoveNext())
			{
				if (!nodeSet2.MoveNext())
				{
					return false;
				}
				string value = nodeSet.Value;
				while (value == nodeSet2.Value != flag)
				{
					if (!nodeSet2.MoveNext())
					{
						nodeSet2.Reset();
						goto IL_0015;
					}
				}
				return true;
			}
			return false;
		}

		private static bool cmpQueryQueryO(Operator.Op op, object val1, object val2)
		{
			LogicalExpr.NodeSet nodeSet = new LogicalExpr.NodeSet(val1);
			LogicalExpr.NodeSet nodeSet2 = new LogicalExpr.NodeSet(val2);
			IL_0010:
			while (nodeSet.MoveNext())
			{
				if (!nodeSet2.MoveNext())
				{
					return false;
				}
				double num = NumberFunctions.Number(nodeSet.Value);
				while (!LogicalExpr.cmpNumberNumber(op, num, NumberFunctions.Number(nodeSet2.Value)))
				{
					if (!nodeSet2.MoveNext())
					{
						nodeSet2.Reset();
						goto IL_0010;
					}
				}
				return true;
			}
			return false;
		}

		private static bool cmpQueryNumber(Operator.Op op, object val1, object val2)
		{
			LogicalExpr.NodeSet nodeSet = new LogicalExpr.NodeSet(val1);
			double num = (double)val2;
			while (nodeSet.MoveNext())
			{
				if (LogicalExpr.cmpNumberNumber(op, NumberFunctions.Number(nodeSet.Value), num))
				{
					return true;
				}
			}
			return false;
		}

		private static bool cmpQueryStringE(Operator.Op op, object val1, object val2)
		{
			LogicalExpr.NodeSet nodeSet = new LogicalExpr.NodeSet(val1);
			string text = (string)val2;
			while (nodeSet.MoveNext())
			{
				if (LogicalExpr.cmpStringStringE(op, nodeSet.Value, text))
				{
					return true;
				}
			}
			return false;
		}

		private static bool cmpQueryStringO(Operator.Op op, object val1, object val2)
		{
			LogicalExpr.NodeSet nodeSet = new LogicalExpr.NodeSet(val1);
			double num = NumberFunctions.Number((string)val2);
			while (nodeSet.MoveNext())
			{
				if (LogicalExpr.cmpNumberNumberO(op, NumberFunctions.Number(nodeSet.Value), num))
				{
					return true;
				}
			}
			return false;
		}

		private static bool cmpRtfQueryE(Operator.Op op, object val1, object val2)
		{
			string text = LogicalExpr.Rtf(val1);
			LogicalExpr.NodeSet nodeSet = new LogicalExpr.NodeSet(val2);
			while (nodeSet.MoveNext())
			{
				if (LogicalExpr.cmpStringStringE(op, text, nodeSet.Value))
				{
					return true;
				}
			}
			return false;
		}

		private static bool cmpRtfQueryO(Operator.Op op, object val1, object val2)
		{
			double num = NumberFunctions.Number(LogicalExpr.Rtf(val1));
			LogicalExpr.NodeSet nodeSet = new LogicalExpr.NodeSet(val2);
			while (nodeSet.MoveNext())
			{
				if (LogicalExpr.cmpNumberNumberO(op, num, NumberFunctions.Number(nodeSet.Value)))
				{
					return true;
				}
			}
			return false;
		}

		private static bool cmpQueryBoolE(Operator.Op op, object val1, object val2)
		{
			LogicalExpr.NodeSet nodeSet = new LogicalExpr.NodeSet(val1);
			bool flag = nodeSet.MoveNext();
			bool flag2 = (bool)val2;
			return LogicalExpr.cmpBoolBoolE(op, flag, flag2);
		}

		private static bool cmpQueryBoolO(Operator.Op op, object val1, object val2)
		{
			LogicalExpr.NodeSet nodeSet = new LogicalExpr.NodeSet(val1);
			double num = (nodeSet.MoveNext() ? 1.0 : 0.0);
			double num2 = NumberFunctions.Number((bool)val2);
			return LogicalExpr.cmpNumberNumberO(op, num, num2);
		}

		private static bool cmpBoolBoolE(Operator.Op op, bool n1, bool n2)
		{
			return op == Operator.Op.EQ == (n1 == n2);
		}

		private static bool cmpBoolBoolE(Operator.Op op, object val1, object val2)
		{
			bool flag = (bool)val1;
			bool flag2 = (bool)val2;
			return LogicalExpr.cmpBoolBoolE(op, flag, flag2);
		}

		private static bool cmpBoolBoolO(Operator.Op op, object val1, object val2)
		{
			double num = NumberFunctions.Number((bool)val1);
			double num2 = NumberFunctions.Number((bool)val2);
			return LogicalExpr.cmpNumberNumberO(op, num, num2);
		}

		private static bool cmpBoolNumberE(Operator.Op op, object val1, object val2)
		{
			bool flag = (bool)val1;
			bool flag2 = BooleanFunctions.toBoolean((double)val2);
			return LogicalExpr.cmpBoolBoolE(op, flag, flag2);
		}

		private static bool cmpBoolNumberO(Operator.Op op, object val1, object val2)
		{
			double num = NumberFunctions.Number((bool)val1);
			double num2 = (double)val2;
			return LogicalExpr.cmpNumberNumberO(op, num, num2);
		}

		private static bool cmpBoolStringE(Operator.Op op, object val1, object val2)
		{
			bool flag = (bool)val1;
			bool flag2 = BooleanFunctions.toBoolean((string)val2);
			return LogicalExpr.cmpBoolBoolE(op, flag, flag2);
		}

		private static bool cmpRtfBoolE(Operator.Op op, object val1, object val2)
		{
			bool flag = BooleanFunctions.toBoolean(LogicalExpr.Rtf(val1));
			bool flag2 = (bool)val2;
			return LogicalExpr.cmpBoolBoolE(op, flag, flag2);
		}

		private static bool cmpBoolStringO(Operator.Op op, object val1, object val2)
		{
			return LogicalExpr.cmpNumberNumberO(op, NumberFunctions.Number((bool)val1), NumberFunctions.Number((string)val2));
		}

		private static bool cmpRtfBoolO(Operator.Op op, object val1, object val2)
		{
			return LogicalExpr.cmpNumberNumberO(op, NumberFunctions.Number(LogicalExpr.Rtf(val1)), NumberFunctions.Number((bool)val2));
		}

		private static bool cmpNumberNumber(Operator.Op op, double n1, double n2)
		{
			switch (op)
			{
			case Operator.Op.EQ:
				return n1 == n2;
			case Operator.Op.NE:
				return n1 != n2;
			case Operator.Op.LT:
				return n1 < n2;
			case Operator.Op.LE:
				return n1 <= n2;
			case Operator.Op.GT:
				return n1 > n2;
			case Operator.Op.GE:
				return n1 >= n2;
			default:
				return false;
			}
		}

		private static bool cmpNumberNumberO(Operator.Op op, double n1, double n2)
		{
			switch (op)
			{
			case Operator.Op.LT:
				return n1 < n2;
			case Operator.Op.LE:
				return n1 <= n2;
			case Operator.Op.GT:
				return n1 > n2;
			case Operator.Op.GE:
				return n1 >= n2;
			default:
				return false;
			}
		}

		private static bool cmpNumberNumber(Operator.Op op, object val1, object val2)
		{
			double num = (double)val1;
			double num2 = (double)val2;
			return LogicalExpr.cmpNumberNumber(op, num, num2);
		}

		private static bool cmpStringNumber(Operator.Op op, object val1, object val2)
		{
			double num = (double)val2;
			double num2 = NumberFunctions.Number((string)val1);
			return LogicalExpr.cmpNumberNumber(op, num2, num);
		}

		private static bool cmpRtfNumber(Operator.Op op, object val1, object val2)
		{
			double num = (double)val2;
			double num2 = NumberFunctions.Number(LogicalExpr.Rtf(val1));
			return LogicalExpr.cmpNumberNumber(op, num2, num);
		}

		private static bool cmpStringStringE(Operator.Op op, string n1, string n2)
		{
			return op == Operator.Op.EQ == (n1 == n2);
		}

		private static bool cmpStringStringE(Operator.Op op, object val1, object val2)
		{
			string text = (string)val1;
			string text2 = (string)val2;
			return LogicalExpr.cmpStringStringE(op, text, text2);
		}

		private static bool cmpRtfStringE(Operator.Op op, object val1, object val2)
		{
			string text = LogicalExpr.Rtf(val1);
			string text2 = (string)val2;
			return LogicalExpr.cmpStringStringE(op, text, text2);
		}

		private static bool cmpRtfRtfE(Operator.Op op, object val1, object val2)
		{
			string text = LogicalExpr.Rtf(val1);
			string text2 = LogicalExpr.Rtf(val2);
			return LogicalExpr.cmpStringStringE(op, text, text2);
		}

		private static bool cmpStringStringO(Operator.Op op, object val1, object val2)
		{
			double num = NumberFunctions.Number((string)val1);
			double num2 = NumberFunctions.Number((string)val2);
			return LogicalExpr.cmpNumberNumberO(op, num, num2);
		}

		private static bool cmpRtfStringO(Operator.Op op, object val1, object val2)
		{
			double num = NumberFunctions.Number(LogicalExpr.Rtf(val1));
			double num2 = NumberFunctions.Number((string)val2);
			return LogicalExpr.cmpNumberNumberO(op, num, num2);
		}

		private static bool cmpRtfRtfO(Operator.Op op, object val1, object val2)
		{
			double num = NumberFunctions.Number(LogicalExpr.Rtf(val1));
			double num2 = NumberFunctions.Number(LogicalExpr.Rtf(val2));
			return LogicalExpr.cmpNumberNumberO(op, num, num2);
		}

		public override XPathNodeIterator Clone()
		{
			return new LogicalExpr(this);
		}

		private static string Rtf(object o)
		{
			return ((XPathNavigator)o).Value;
		}

		public override XPathResultType StaticType
		{
			get
			{
				return XPathResultType.Boolean;
			}
		}

		public override void PrintQuery(XmlWriter w)
		{
			w.WriteStartElement(base.GetType().Name);
			w.WriteAttributeString("op", this.op.ToString());
			this.opnd1.PrintQuery(w);
			this.opnd2.PrintQuery(w);
			w.WriteEndElement();
		}

		// Note: this type is marked as 'beforefieldinit'.
		static LogicalExpr()
		{
			LogicalExpr.cmpXslt[][] array = new LogicalExpr.cmpXslt[5][];
			int num = 0;
			LogicalExpr.cmpXslt[] array2 = new LogicalExpr.cmpXslt[5];
			array2[0] = new LogicalExpr.cmpXslt(LogicalExpr.cmpNumberNumber);
			array[num] = array2;
			int num2 = 1;
			LogicalExpr.cmpXslt[] array3 = new LogicalExpr.cmpXslt[5];
			array3[0] = new LogicalExpr.cmpXslt(LogicalExpr.cmpStringNumber);
			array3[1] = new LogicalExpr.cmpXslt(LogicalExpr.cmpStringStringE);
			array[num2] = array3;
			int num3 = 2;
			LogicalExpr.cmpXslt[] array4 = new LogicalExpr.cmpXslt[5];
			array4[0] = new LogicalExpr.cmpXslt(LogicalExpr.cmpBoolNumberE);
			array4[1] = new LogicalExpr.cmpXslt(LogicalExpr.cmpBoolStringE);
			array4[2] = new LogicalExpr.cmpXslt(LogicalExpr.cmpBoolBoolE);
			array[num3] = array4;
			int num4 = 3;
			LogicalExpr.cmpXslt[] array5 = new LogicalExpr.cmpXslt[5];
			array5[0] = new LogicalExpr.cmpXslt(LogicalExpr.cmpQueryNumber);
			array5[1] = new LogicalExpr.cmpXslt(LogicalExpr.cmpQueryStringE);
			array5[2] = new LogicalExpr.cmpXslt(LogicalExpr.cmpQueryBoolE);
			array5[3] = new LogicalExpr.cmpXslt(LogicalExpr.cmpQueryQueryE);
			array[num4] = array5;
			array[4] = new LogicalExpr.cmpXslt[]
			{
				new LogicalExpr.cmpXslt(LogicalExpr.cmpRtfNumber),
				new LogicalExpr.cmpXslt(LogicalExpr.cmpRtfStringE),
				new LogicalExpr.cmpXslt(LogicalExpr.cmpRtfBoolE),
				new LogicalExpr.cmpXslt(LogicalExpr.cmpRtfQueryE),
				new LogicalExpr.cmpXslt(LogicalExpr.cmpRtfRtfE)
			};
			LogicalExpr.CompXsltE = array;
			LogicalExpr.cmpXslt[][] array6 = new LogicalExpr.cmpXslt[5][];
			int num5 = 0;
			LogicalExpr.cmpXslt[] array7 = new LogicalExpr.cmpXslt[5];
			array7[0] = new LogicalExpr.cmpXslt(LogicalExpr.cmpNumberNumber);
			array6[num5] = array7;
			int num6 = 1;
			LogicalExpr.cmpXslt[] array8 = new LogicalExpr.cmpXslt[5];
			array8[0] = new LogicalExpr.cmpXslt(LogicalExpr.cmpStringNumber);
			array8[1] = new LogicalExpr.cmpXslt(LogicalExpr.cmpStringStringO);
			array6[num6] = array8;
			int num7 = 2;
			LogicalExpr.cmpXslt[] array9 = new LogicalExpr.cmpXslt[5];
			array9[0] = new LogicalExpr.cmpXslt(LogicalExpr.cmpBoolNumberO);
			array9[1] = new LogicalExpr.cmpXslt(LogicalExpr.cmpBoolStringO);
			array9[2] = new LogicalExpr.cmpXslt(LogicalExpr.cmpBoolBoolO);
			array6[num7] = array9;
			int num8 = 3;
			LogicalExpr.cmpXslt[] array10 = new LogicalExpr.cmpXslt[5];
			array10[0] = new LogicalExpr.cmpXslt(LogicalExpr.cmpQueryNumber);
			array10[1] = new LogicalExpr.cmpXslt(LogicalExpr.cmpQueryStringO);
			array10[2] = new LogicalExpr.cmpXslt(LogicalExpr.cmpQueryBoolO);
			array10[3] = new LogicalExpr.cmpXslt(LogicalExpr.cmpQueryQueryO);
			array6[num8] = array10;
			array6[4] = new LogicalExpr.cmpXslt[]
			{
				new LogicalExpr.cmpXslt(LogicalExpr.cmpRtfNumber),
				new LogicalExpr.cmpXslt(LogicalExpr.cmpRtfStringO),
				new LogicalExpr.cmpXslt(LogicalExpr.cmpRtfBoolO),
				new LogicalExpr.cmpXslt(LogicalExpr.cmpRtfQueryO),
				new LogicalExpr.cmpXslt(LogicalExpr.cmpRtfRtfO)
			};
			LogicalExpr.CompXsltO = array6;
		}

		private Operator.Op op;

		private Query opnd1;

		private Query opnd2;

		private static readonly LogicalExpr.cmpXslt[][] CompXsltE;

		private static readonly LogicalExpr.cmpXslt[][] CompXsltO;

		private delegate bool cmpXslt(Operator.Op op, object val1, object val2);

		private struct NodeSet
		{
			public NodeSet(object opnd)
			{
				this.opnd = (Query)opnd;
				this.current = null;
			}

			public bool MoveNext()
			{
				this.current = this.opnd.Advance();
				return this.current != null;
			}

			public void Reset()
			{
				this.opnd.Reset();
			}

			public string Value
			{
				get
				{
					return this.current.Value;
				}
			}

			private Query opnd;

			private XPathNavigator current;
		}
	}
}
