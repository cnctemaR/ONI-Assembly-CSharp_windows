using System;
using System.Diagnostics;

namespace System.Xml.Xsl
{
	[DebuggerDisplay("({Line},{Pos})")]
	internal struct Location
	{
		public int Line
		{
			get
			{
				return (int)(this.value >> 32);
			}
		}

		public int Pos
		{
			get
			{
				return (int)this.value;
			}
		}

		public Location(int line, int pos)
		{
			this.value = (ulong)(((long)line << 32) | (long)((ulong)pos));
		}

		public Location(Location that)
		{
			this.value = that.value;
		}

		public bool LessOrEqual(Location that)
		{
			return this.value <= that.value;
		}

		private ulong value;
	}
}
