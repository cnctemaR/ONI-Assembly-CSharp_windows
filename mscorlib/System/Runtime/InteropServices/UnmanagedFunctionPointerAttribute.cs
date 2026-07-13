using System;

namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
	[ComVisible(true)]
	public sealed class UnmanagedFunctionPointerAttribute : Attribute
	{
		public UnmanagedFunctionPointerAttribute(CallingConvention callingConvention)
		{
			this.m_callingConvention = callingConvention;
		}

		public CallingConvention CallingConvention
		{
			get
			{
				return this.m_callingConvention;
			}
		}

		private CallingConvention m_callingConvention;

		public CharSet CharSet;

		public bool BestFitMapping;

		public bool ThrowOnUnmappableChar;

		public bool SetLastError;
	}
}
