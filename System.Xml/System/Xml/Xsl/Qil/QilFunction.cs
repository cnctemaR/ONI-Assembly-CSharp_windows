using System;

namespace System.Xml.Xsl.Qil
{
	internal class QilFunction : QilReference
	{
		public QilFunction(QilNodeType nodeType, QilNode arguments, QilNode definition, QilNode sideEffects, XmlQueryType resultType)
			: base(nodeType)
		{
			this.arguments = arguments;
			this.definition = definition;
			this.sideEffects = sideEffects;
			this.xmlType = resultType;
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
					return this.arguments;
				case 1:
					return this.definition;
				case 2:
					return this.sideEffects;
				default:
					throw new IndexOutOfRangeException();
				}
			}
			set
			{
				switch (index)
				{
				case 0:
					this.arguments = value;
					return;
				case 1:
					this.definition = value;
					return;
				case 2:
					this.sideEffects = value;
					return;
				default:
					throw new IndexOutOfRangeException();
				}
			}
		}

		public QilList Arguments
		{
			get
			{
				return (QilList)this.arguments;
			}
			set
			{
				this.arguments = value;
			}
		}

		public QilNode Definition
		{
			get
			{
				return this.definition;
			}
			set
			{
				this.definition = value;
			}
		}

		public bool MaybeSideEffects
		{
			get
			{
				return this.sideEffects.NodeType == QilNodeType.True;
			}
			set
			{
				this.sideEffects.NodeType = (value ? QilNodeType.True : QilNodeType.False);
			}
		}

		private QilNode arguments;

		private QilNode definition;

		private QilNode sideEffects;
	}
}
