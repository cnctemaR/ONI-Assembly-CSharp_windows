using System;

namespace System.Xml.Xsl.Qil
{
	internal class QilBinary : QilNode
	{
		public QilBinary(QilNodeType nodeType, QilNode left, QilNode right)
			: base(nodeType)
		{
			this._left = left;
			this._right = right;
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
					return this._left;
				}
				if (index != 1)
				{
					throw new IndexOutOfRangeException();
				}
				return this._right;
			}
			set
			{
				if (index == 0)
				{
					this._left = value;
					return;
				}
				if (index != 1)
				{
					throw new IndexOutOfRangeException();
				}
				this._right = value;
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

		private QilNode _right;
	}
}
