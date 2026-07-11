using System;
using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
	[Serializable]
	public sealed class RequiredAttributeAttribute : Attribute
	{
		public RequiredAttributeAttribute(Type requiredContract)
		{
			this.requiredContract = requiredContract;
		}

		public Type RequiredContract
		{
			get
			{
				return this.requiredContract;
			}
		}

		private Type requiredContract;
	}
}
