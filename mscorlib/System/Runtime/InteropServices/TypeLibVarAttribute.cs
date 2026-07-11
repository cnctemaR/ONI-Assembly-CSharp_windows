using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Field, Inherited = false)]
	public sealed class TypeLibVarAttribute : Attribute
	{
		public TypeLibVarAttribute(short flags)
		{
			this.flags = (TypeLibVarFlags)flags;
		}

		public TypeLibVarAttribute(TypeLibVarFlags flags)
		{
			this.flags = flags;
		}

		public TypeLibVarFlags Value
		{
			get
			{
				return this.flags;
			}
		}

		private TypeLibVarFlags flags;
	}
}
