using System;
using System.Diagnostics;

namespace System.Collections.Generic
{
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	internal readonly struct CopyPosition
	{
		internal CopyPosition(int row, int column)
		{
			this.Row = row;
			this.Column = column;
		}

		public static CopyPosition Start
		{
			get
			{
				return default(CopyPosition);
			}
		}

		internal int Row { get; }

		internal int Column { get; }

		public CopyPosition Normalize(int endColumn)
		{
			if (this.Column != endColumn)
			{
				return this;
			}
			return new CopyPosition(this.Row + 1, 0);
		}

		private string DebuggerDisplay
		{
			get
			{
				return string.Format("[{0}, {1}]", this.Row, this.Column);
			}
		}
	}
}
