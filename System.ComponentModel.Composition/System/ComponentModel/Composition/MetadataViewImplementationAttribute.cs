using System;

namespace System.ComponentModel.Composition
{
	[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
	public sealed class MetadataViewImplementationAttribute : Attribute
	{
		public MetadataViewImplementationAttribute(Type implementationType)
		{
			this.ImplementationType = implementationType;
		}

		public Type ImplementationType { get; private set; }
	}
}
