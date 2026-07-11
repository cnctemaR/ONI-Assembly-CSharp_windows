using System;

namespace System.Xml.Xsl.Qil
{
	internal class QilSortKey : QilBinary
	{
		public QilSortKey(QilNodeType nodeType, QilNode key, QilNode collation)
			: base(nodeType, key, collation)
		{
		}

		public QilNode Key
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

		public QilNode Collation
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
