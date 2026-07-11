using System;

namespace Mono.Unix.Native
{
	[Map("struct linger")]
	[CLSCompliant(false)]
	public struct Linger
	{
		public override string ToString()
		{
			return string.Format("{0}, {1}", this.l_onoff, this.l_linger);
		}

		public int l_onoff;

		public int l_linger;
	}
}
