using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	public sealed class DllImportAttribute : Attribute
	{
		public DllImportAttribute(string dllName)
		{
			this.Dll = dllName;
		}

		public string Value
		{
			get
			{
				return this.Dll;
			}
		}

		public CallingConvention CallingConvention;

		public CharSet CharSet;

		private string Dll;

		public string EntryPoint;

		public bool ExactSpelling;

		public bool PreserveSig;

		public bool SetLastError;

		public bool BestFitMapping;

		public bool ThrowOnUnmappableChar;
	}
}
