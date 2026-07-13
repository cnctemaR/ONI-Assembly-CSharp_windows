using System;

namespace Iced.Intel
{
	internal readonly struct RelocInfo
	{
		public RelocInfo(RelocKind kind, ulong address)
		{
			this.Kind = kind;
			this.Address = address;
		}

		public readonly ulong Address;

		public readonly RelocKind Kind;
	}
}
