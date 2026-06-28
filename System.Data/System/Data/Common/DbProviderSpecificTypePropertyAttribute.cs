using System;

namespace System.Data.Common
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	[Serializable]
	public sealed class DbProviderSpecificTypePropertyAttribute : Attribute
	{
		public DbProviderSpecificTypePropertyAttribute(bool isProviderSpecificTypeProperty)
		{
			this.isProviderSpecificTypeProperty = isProviderSpecificTypeProperty;
		}

		public bool IsProviderSpecificTypeProperty
		{
			get
			{
				return this.isProviderSpecificTypeProperty;
			}
		}

		private bool isProviderSpecificTypeProperty;
	}
}
