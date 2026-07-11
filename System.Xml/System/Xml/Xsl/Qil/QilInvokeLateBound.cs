using System;

namespace System.Xml.Xsl.Qil
{
	internal class QilInvokeLateBound : QilBinary
	{
		public QilInvokeLateBound(QilNodeType nodeType, QilNode name, QilNode arguments)
			: base(nodeType, name, arguments)
		{
		}

		public QilName Name
		{
			get
			{
				return (QilName)base.Left;
			}
			set
			{
				base.Left = value;
			}
		}

		public QilList Arguments
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
