using System;

namespace System.Xml.Xsl.Qil
{
	internal class QilTernary : QilNode
	{
		public QilTernary(QilNodeType nodeType, QilNode left, QilNode center, QilNode right)
			: base(nodeType)
		{
			this.left = left;
			this.center = center;
			this.right = right;
		}

		public override int Count
		{
			get
			{
				return 3;
			}
		}

		public override QilNode this[int index]
		{
			get
			{
				switch (index)
				{
				case 0:
					return this.left;
				case 1:
					return this.center;
				case 2:
					return this.right;
				default:
					throw new IndexOutOfRangeException();
				}
			}
			set
			{
				switch (index)
				{
				case 0:
					this.left = value;
					return;
				case 1:
					this.center = value;
					return;
				case 2:
					this.right = value;
					return;
				default:
					throw new IndexOutOfRangeException();
				}
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

		public QilNode Center
		{
			get
			{
				return this.center;
			}
			set
			{
				this.center = value;
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

		private QilNode center;

		private QilNode right;
	}
}
