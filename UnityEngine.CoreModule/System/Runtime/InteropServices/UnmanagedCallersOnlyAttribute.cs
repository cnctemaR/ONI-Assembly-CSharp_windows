using System;
using System.Runtime.CompilerServices;

namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	internal sealed class UnmanagedCallersOnlyAttribute : Attribute
	{
		[global::System.Runtime.CompilerServices.Nullable(new byte[] { 2, 1 })]
		public Type[] CallConvs;

		[global::System.Runtime.CompilerServices.Nullable(2)]
		public string EntryPoint;
	}
}
