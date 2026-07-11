using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class, Inherited = false)]
	public sealed class ClassInterfaceAttribute : Attribute
	{
		public ClassInterfaceAttribute(short classInterfaceType)
		{
			this.ciType = (ClassInterfaceType)classInterfaceType;
		}

		public ClassInterfaceAttribute(ClassInterfaceType classInterfaceType)
		{
			this.ciType = classInterfaceType;
		}

		public ClassInterfaceType Value
		{
			get
			{
				return this.ciType;
			}
		}

		private ClassInterfaceType ciType;
	}
}
