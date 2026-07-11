using System;

namespace System.Xml.Xsl.Qil
{
	internal class QilChoice : QilBinary
	{
		public QilChoice(QilNodeType nodeType, QilNode expression, QilNode branches)
			: base(nodeType, expression, branches)
		{
		}

		public QilNode Expression
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

		public QilList Branches
		{
			get
			{
				return (QilList)base.Right;
			}
			set
			{
				base.Right = value;
			}
		}
	}
}
