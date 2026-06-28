using System;
using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, Inherited = false)]
	[ComVisible(true)]
	[Serializable]
	public sealed class MethodImplAttribute : Attribute
	{
		public MethodImplAttribute()
		{
		}

		public MethodImplAttribute(short value)
		{
			this._val = (MethodImplOptions)value;
		}

		public MethodImplAttribute(MethodImplOptions methodImplOptions)
		{
			this._val = methodImplOptions;
		}

		public MethodImplOptions Value
		{
			get
			{
				return this._val;
			}
		}

		private MethodImplOptions _val;

		public MethodCodeType MethodCodeType;
	}
}
