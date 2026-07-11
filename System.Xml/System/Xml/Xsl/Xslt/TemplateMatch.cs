using System;
using System.Collections.Generic;
using System.Xml.Xsl.Qil;

namespace System.Xml.Xsl.Xslt
{
	internal class TemplateMatch
	{
		public XmlNodeKindFlags NodeKind
		{
			get
			{
				return this.nodeKind;
			}
		}

		public QilName QName
		{
			get
			{
				return this.qname;
			}
		}

		public QilIterator Iterator
		{
			get
			{
				return this.iterator;
			}
		}

		public QilNode Condition
		{
			get
			{
				return this.condition;
			}
		}

		public QilFunction TemplateFunction
		{
			get
			{
				return this.template.Function;
			}
		}

		public TemplateMatch(Template template, QilLoop filter)
		{
			this.template = template;
			this.priority = (double.IsNaN(template.Priority) ? XPathPatternBuilder.GetPriority(filter) : template.Priority);
			this.iterator = filter.Variable;
			this.condition = filter.Body;
			XPathPatternBuilder.CleanAnnotation(filter);
			this.NipOffTypeNameCheck();
		}

		private void NipOffTypeNameCheck()
		{
			QilBinary[] array = new QilBinary[4];
			int num = -1;
			QilNode left = this.condition;
			this.nodeKind = XmlNodeKindFlags.None;
			this.qname = null;
			while (left.NodeType == QilNodeType.And)
			{
				left = (array[++num & 3] = (QilBinary)left).Left;
			}
			if (left.NodeType != QilNodeType.IsType)
			{
				return;
			}
			QilBinary qilBinary = (QilBinary)left;
			if (qilBinary.Left != this.iterator || qilBinary.Right.NodeType != QilNodeType.LiteralType)
			{
				return;
			}
			XmlNodeKindFlags nodeKinds = qilBinary.Right.XmlType.NodeKinds;
			if (!Bits.ExactlyOne((uint)nodeKinds))
			{
				return;
			}
			this.nodeKind = nodeKinds;
			QilBinary qilBinary2 = array[num & 3];
			if (qilBinary2 != null && qilBinary2.Right.NodeType == QilNodeType.Eq)
			{
				QilBinary qilBinary3 = (QilBinary)qilBinary2.Right;
				if (qilBinary3.Left.NodeType == QilNodeType.NameOf && ((QilUnary)qilBinary3.Left).Child == this.iterator && qilBinary3.Right.NodeType == QilNodeType.LiteralQName)
				{
					this.qname = (QilName)((QilLiteral)qilBinary3.Right).Value;
					num--;
				}
			}
			QilBinary qilBinary4 = array[num & 3];
			QilBinary qilBinary5 = array[(num - 1) & 3];
			if (qilBinary5 != null)
			{
				qilBinary5.Left = qilBinary4.Right;
				return;
			}
			if (qilBinary4 != null)
			{
				this.condition = qilBinary4.Right;
				return;
			}
			this.condition = null;
		}

		public static readonly TemplateMatch.TemplateMatchComparer Comparer = new TemplateMatch.TemplateMatchComparer();

		private Template template;

		private double priority;

		private XmlNodeKindFlags nodeKind;

		private QilName qname;

		private QilIterator iterator;

		private QilNode condition;

		internal class TemplateMatchComparer : IComparer<TemplateMatch>
		{
			public int Compare(TemplateMatch x, TemplateMatch y)
			{
				if (x.priority > y.priority)
				{
					return 1;
				}
				if (x.priority >= y.priority)
				{
					return x.template.OrderNumber - y.template.OrderNumber;
				}
				return -1;
			}
		}
	}
}
