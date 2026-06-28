using System;

namespace System.IO.Pipes
{
	internal struct SecurityAttributesHack
	{
		public SecurityAttributesHack(bool inheritable)
		{
			this.Length = 0;
			this.SecurityDescriptor = IntPtr.Zero;
			this.Inheritable = inheritable;
		}

		public readonly int Length;

		public readonly IntPtr SecurityDescriptor;

		public readonly bool Inheritable;
	}
}
