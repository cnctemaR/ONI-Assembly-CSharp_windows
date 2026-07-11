using System;

namespace System.Xml.Xsl.Qil
{
	internal class QilBinary : QilNode
	{
		public QilBinary(QilNodeType nodeType, QilNode left, QilNode right)
			: base(nodeType)
		{
			this.left = left;
			this.right = right;
		}

		public override int Count
		{
			get
			{
				return 2;
			}
		}

		public override QilNode this[int index]
		{
			get
			{
				if (index == 0)
				{
					return this.left;
				}
				if (index != 1)
				{
					throw new IndexOutOfRangeException();
				}
				return this.right;
			}
			set
			{
				if (index == 0)
				{
					this.left = value;
					return;
				}
				if (index != 1)
				{
					throw new IndexOutOfRangeException();
				}
				this.right = value;
			}
		}

		public QilNode Left
		{
			get
			{
				return this.left;
			}
			set
			{
				this.left = value;
			}
		}

		public QilNode Right
		{
			get
			{
				return this.right;
			}
			set
			{
				this.right = value;
			}
		}

		private QilNode left;

		private QilNode right;
	}
}
