using System;

namespace System.Xml.Xsl.Qil
{
	internal class QilParameter : QilIterator
	{
		public QilParameter(QilNodeType nodeType, QilNode defaultValue, QilNode name, XmlQueryType xmlType)
			: base(nodeType, defaultValue)
		{
			this._name = name;
			this.xmlType = xmlType;
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
					return base.Binding;
				}
				if (index != 1)
				{
					throw new IndexOutOfRangeException();
				}
				return this._name;
			}
			set
			{
				if (index == 0)
				{
					base.Binding = value;
					return;
				}
				if (index != 1)
				{
					throw new IndexOutOfRangeException();
				}
				this._name = value;
			}
		}

		public QilNode DefaultValue
		{
			get
			{
				return base.Binding;
			}
			set
			{
				base.Binding = value;
			}
		}

		public QilName Name
		{
			get
			{
				return (QilName)this._name;
			}
			set
			{
				this._name = value;
			}
		}

		private QilNode _name;
	}
}
