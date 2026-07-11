using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface, Inherited = false)]
	public sealed class TypeLibTypeAttribute : Attribute
	{
		public TypeLibTypeAttribute(TypeLibTypeFlags flags)
		{
			this._val = flags;
		}

		public TypeLibTypeAttribute(short flags)
		{
			this._val = (TypeLibTypeFlags)flags;
		}

		public TypeLibTypeFlags Value
		{
			get
			{
				return this._val;
			}
		}

		internal TypeLibTypeFlags _val;
	}
}
