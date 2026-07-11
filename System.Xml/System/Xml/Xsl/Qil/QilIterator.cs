using System;

namespace System.Xml.Xsl.Qil
{
	internal class QilIterator : QilReference
	{
		public QilIterator(QilNodeType nodeType, QilNode binding)
			: base(nodeType)
		{
			this.Binding = binding;
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
				return this.binding;
			}
			set
			{
				if (index != 0)
				{
					throw new IndexOutOfRangeException();
				}
				this.binding = value;
			}
		}

		public QilNode Binding
		{
			get
			{
				return this.binding;
			}
			set
			{
				this.binding = value;
			}
		}

		private QilNode binding;
	}
}
