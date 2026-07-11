using System;

namespace System.Data.Common
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	[Serializable]
	public sealed class DbProviderSpecificTypePropertyAttribute : Attribute
	{
		public DbProviderSpecificTypePropertyAttribute(bool isProviderSpecificTypeProperty)
		{
			this.IsProviderSpecificTypeProperty = isProviderSpecificTypeProperty;
		}

		public bool IsProviderSpecificTypeProperty { get; }
	}
}
