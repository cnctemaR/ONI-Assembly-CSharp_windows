using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Delegate, Inherited = false, AllowMultiple = false)]
	public sealed class UnmanagedFunctionPointerAttribute : Attribute
	{
		public UnmanagedFunctionPointerAttribute(CallingConvention callingConvention)
		{
			this.call_conv = callingConvention;
		}

		public CallingConvention CallingConvention
		{
			get
			{
				return this.call_conv;
			}
		}

		private CallingConvention call_conv;

		public CharSet CharSet;

		public bool SetLastError;

		public bool BestFitMapping;

		public bool ThrowOnUnmappableChar;
	}
}
