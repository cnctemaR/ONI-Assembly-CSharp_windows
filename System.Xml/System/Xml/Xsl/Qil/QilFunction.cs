using System;

namespace System.Xml.Xsl.Qil
{
	internal class QilFunction : QilReference
	{
		public QilFunction(QilNodeType nodeType, QilNode arguments, QilNode definition, QilNode sideEffects, XmlQueryType resultType)
			: base(nodeType)
		{
			this._arguments = arguments;
			this._definition = definition;
			this._sideEffects = sideEffects;
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
					return this._arguments;
				case 1:
					return this._definition;
				case 2:
					return this._sideEffects;
				default:
					throw new IndexOutOfRangeException();
				}
			}
			set
			{
				switch (index)
				{
				case 0:
					this._arguments = value;
					return;
				case 1:
					this._definition = value;
					return;
				case 2:
					this._sideEffects = value;
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
				return (QilList)this._arguments;
			}
			set
			{
				this._arguments = value;
			}
		}

		public QilNode Definition
		{
			get
			{
				return this._definition;
			}
			set
			{
				this._definition = value;
			}
		}

		public bool MaybeSideEffects
		{
			get
			{
				return this._sideEffects.NodeType == QilNodeType.True;
			}
			set
			{
				this._sideEffects.NodeType = (value ? QilNodeType.True : QilNodeType.False);
			}
		}

		private QilNode _arguments;

		private QilNode _definition;

		private QilNode _sideEffects;
	}
}
