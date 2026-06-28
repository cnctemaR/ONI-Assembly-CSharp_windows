using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Interface, Inherited = false)]
	public sealed class InterfaceTypeAttribute : Attribute
	{
		public InterfaceTypeAttribute(ComInterfaceType interfaceType)
		{
			this.intType = interfaceType;
		}

		public InterfaceTypeAttribute(short interfaceType)
		{
			this.intType = (ComInterfaceType)interfaceType;
		}

		public ComInterfaceType Value
		{
			get
			{
				return this.intType;
			}
		}

		private ComInterfaceType intType;
	}
}
