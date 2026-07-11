using System;

namespace System.Xml.Xsl.Qil
{
	internal class QilLiteral : QilNode
	{
		public QilLiteral(QilNodeType nodeType, object value)
			: base(nodeType)
		{
			this.Value = value;
		}

		public object Value
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = value;
			}
		}

		public static implicit operator string(QilLiteral literal)
		{
			return (string)literal.value;
		}

		public static implicit operator int(QilLiteral literal)
		{
			return (int)literal.value;
		}

		public static implicit operator long(QilLiteral literal)
		{
			return (long)literal.value;
		}

		public static implicit operator double(QilLiteral literal)
		{
			return (double)literal.value;
		}

		public static implicit operator decimal(QilLiteral literal)
		{
			return (decimal)literal.value;
		}

		public static implicit operator XmlQueryType(QilLiteral literal)
		{
			return (XmlQueryType)literal.value;
		}

		private object value;
	}
}
