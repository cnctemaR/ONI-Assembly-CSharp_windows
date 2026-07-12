using System;

namespace System.Xml.Xsl.Qil
{
	internal class QilTernary : QilNode
	{
		public QilTernary(QilNodeType nodeType, QilNode left, QilNode center, QilNode right)
			: base(nodeType)
		{
			this._left = left;
			this._center = center;
			this._right = right;
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
					return this._left;
				case 1:
					return this._center;
				case 2:
					return this._right;
				default:
					throw new IndexOutOfRangeException();
				}
			}
			set
			{
				switch (index)
				{
				case 0:
					this._left = value;
					return;
				case 1:
					this._center = value;
					return;
				case 2:
					this._right = value;
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
				return this._left;
			}
			set
			{
				this._left = value;
			}
		}

		public QilNode Center
		{
			get
			{
				return this._center;
			}
			set
			{
				this._center = value;
			}
		}

		public QilNode Right
		{
			get
			{
				return this._right;
			}
			set
			{
				this._right = value;
			}
		}

		private QilNode _left;

		private QilNode _center;

		private QilNode _right;
	}
}
