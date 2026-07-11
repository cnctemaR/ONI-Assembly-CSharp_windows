using System;

namespace Mono.Unity
{
	internal struct size_t
	{
		public size_t(uint i)
		{
			this.value = new IntPtr((long)((ulong)i));
		}

		public static implicit operator size_t(int d)
		{
			return new size_t((uint)d);
		}

		public static implicit operator int(size_t s)
		{
			return s.value.ToInt32();
		}

		public IntPtr value;
	}
}
