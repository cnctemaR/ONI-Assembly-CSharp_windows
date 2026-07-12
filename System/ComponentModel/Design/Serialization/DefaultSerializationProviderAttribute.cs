using System;

namespace System.ComponentModel.Design.Serialization
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public sealed class DefaultSerializationProviderAttribute : Attribute
	{
		public DefaultSerializationProviderAttribute(Type providerType)
		{
			if (providerType == null)
			{
				throw new ArgumentNullException("providerType");
			}
			this.ProviderTypeName = providerType.AssemblyQualifiedName;
		}

		public DefaultSerializationProviderAttribute(string providerTypeName)
		{
			if (providerTypeName == null)
			{
				throw new ArgumentNullException("providerTypeName");
			}
			this.ProviderTypeName = providerTypeName;
		}

		public string ProviderTypeName { get; }
	}
}
