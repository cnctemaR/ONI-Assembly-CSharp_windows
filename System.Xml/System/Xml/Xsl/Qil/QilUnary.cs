using System;

namespace System.Xml.Xsl.Qil
{
	internal class QilUnary : QilNode
	{
		public QilUnary(QilNodeType nodeType, QilNode child)
			: base(nodeType)
		{
			this.child = child;
		}

		public override int Count
		{
			get
			{
				return 1;
			}
		}

		public override QilNode this[int index]
		{
			get
			{
				if (index != 0)
				{
					throw new IndexOutOfRangeException();
				}
				return this.child;
			}
			set
			{
				if (index != 0)
				{
					throw new IndexOutOfRangeException();
				}
				this.child = value;
			}
		}

		public QilNode Child
		{
			get
			{
				return this.child;
			}
			set
			{
				this.child = value;
			}
		}

		private QilNode child;
	}
}
