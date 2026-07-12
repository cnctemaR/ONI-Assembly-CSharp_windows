using System;

namespace System.Xml.Xsl.Qil
{
	internal class QilUnary : QilNode
	{
		public QilUnary(QilNodeType nodeType, QilNode child)
			: base(nodeType)
		{
			this._child = child;
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
				return this._child;
			}
			set
			{
				if (index != 0)
				{
					throw new IndexOutOfRangeException();
				}
				this._child = value;
			}
		}

		public QilNode Child
		{
			get
			{
				return this._child;
			}
			set
			{
				this._child = value;
			}
		}

		private QilNode _child;
	}
}
