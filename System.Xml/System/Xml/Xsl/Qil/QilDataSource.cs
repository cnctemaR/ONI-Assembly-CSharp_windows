using System;

namespace System.Xml.Xsl.Qil
{
	internal class QilDataSource : QilBinary
	{
		public QilDataSource(QilNodeType nodeType, QilNode name, QilNode baseUri)
			: base(nodeType, name, baseUri)
		{
		}

		public QilNode Name
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

		public QilNode BaseUri
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
