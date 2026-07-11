using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	public sealed class TypeLibFuncAttribute : Attribute
	{
		public TypeLibFuncAttribute(TypeLibFuncFlags flags)
		{
			this._val = flags;
		}

		public TypeLibFuncAttribute(short flags)
		{
			this._val = (TypeLibFuncFlags)flags;
		}

		public TypeLibFuncFlags Value
		{
			get
			{
				return this._val;
			}
		}

		internal TypeLibFuncFlags _val;
	}
}
