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
				return this._value;
			}
			set
			{
				this._value = value;
			}
		}

		public static implicit operator string(QilLiteral literal)
		{
			return (string)literal._value;
		}

		public static implicit operator int(QilLiteral literal)
		{
			return (int)literal._value;
		}

		public static implicit operator long(QilLiteral literal)
		{
			return (long)literal._value;
		}

		public static implicit operator double(QilLiteral literal)
		{
			return (double)literal._value;
		}

		public static implicit operator decimal(QilLiteral literal)
		{
			return (decimal)literal._value;
		}

		public static implicit operator XmlQueryType(QilLiteral literal)
		{
			return (XmlQueryType)literal._value;
		}

		private object _value;
	}
}
