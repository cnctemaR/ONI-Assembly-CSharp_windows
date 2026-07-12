using System;
using System.Runtime.CompilerServices;

namespace System
{
	internal ref struct ByReference<T>
	{
		[Intrinsic]
		public ByReference(ref T value)
		{
			throw new NotSupportedException();
		}

		public ref T Value
		{
			[Intrinsic]
			get
			{
				throw new NotSupportedException();
			}
		}

		private IntPtr _value;
	}
}
