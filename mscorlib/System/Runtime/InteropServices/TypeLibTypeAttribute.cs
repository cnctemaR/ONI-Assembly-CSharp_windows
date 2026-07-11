using System;

namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface, Inherited = false)]
	[ComVisible(true)]
	public sealed class TypeLibTypeAttribute : Attribute
	{
		public TypeLibTypeAttribute(short flags)
		{
			this.flags = (TypeLibTypeFlags)flags;
		}

		public TypeLibTypeAttribute(TypeLibTypeFlags flags)
		{
			this.flags = flags;
		}

		public TypeLibTypeFlags Value
		{
			get
			{
				return this.flags;
			}
		}

		private TypeLibTypeFlags flags;
	}
}
