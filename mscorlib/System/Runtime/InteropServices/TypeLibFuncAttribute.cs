using System;

namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	[ComVisible(true)]
	public sealed class TypeLibFuncAttribute : Attribute
	{
		public TypeLibFuncAttribute(short flags)
		{
			this.flags = (TypeLibFuncFlags)flags;
		}

		public TypeLibFuncAttribute(TypeLibFuncFlags flags)
		{
			this.flags = flags;
		}

		public TypeLibFuncFlags Value
		{
			get
			{
				return this.flags;
			}
		}

		private TypeLibFuncFlags flags;
	}
}
