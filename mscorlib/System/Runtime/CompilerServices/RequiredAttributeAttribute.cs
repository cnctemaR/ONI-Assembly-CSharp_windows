using System;
using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
	[ComVisible(true)]
	[Serializable]
	public sealed class RequiredAttributeAttribute : Attribute
	{
		public RequiredAttributeAttribute(Type requiredContract)
		{
		}

		public Type RequiredContract
		{
			get
			{
				throw new NotSupportedException();
			}
		}
	}
}
