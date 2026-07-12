using System;
using System.Diagnostics;

namespace System.Collections.Generic
{
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	internal readonly struct Marker
	{
		public Marker(int count, int index)
		{
			this.Count = count;
			this.Index = index;
		}

		public int Count { get; }

		public int Index { get; }

		private string DebuggerDisplay
		{
			get
			{
				return string.Format("{0}: {1}, {2}: {3}", new object[] { "Index", this.Index, "Count", this.Count });
			}
		}
	}
}
