using System;

namespace System.Xml.Xsl.Qil
{
	internal class QilStrConcat : QilBinary
	{
		public QilStrConcat(QilNodeType nodeType, QilNode delimiter, QilNode values)
			: base(nodeType, delimiter, values)
		{
		}

		public QilNode Delimiter
		{
			get
			{
				return base.Left;
			}
			set
			{
				base.Left = value;
			}
		}

		public QilNode Values
		{
			get
			{
				return base.Right;
			}
			set
			{
				base.Right = value;
			}
		}
	}
}
