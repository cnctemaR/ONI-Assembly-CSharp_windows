using System;
using System.Runtime.InteropServices;

namespace System.IO.Pipes
{
	internal struct SecurityAttributes
	{
		public SecurityAttributes(HandleInheritability inheritability, IntPtr securityDescriptor)
		{
			this.Length = Marshal.SizeOf(typeof(SecurityAttributes));
			this.SecurityDescriptor = securityDescriptor;
			this.Inheritable = inheritability == HandleInheritability.Inheritable;
		}

		public readonly int Length;

		public readonly IntPtr SecurityDescriptor;

		public readonly bool Inheritable;
	}
}
