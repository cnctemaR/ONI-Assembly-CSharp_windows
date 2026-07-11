using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class LicenseProviderAttribute : Attribute
	{
		public LicenseProviderAttribute()
		{
			this.Provider = null;
		}

		public LicenseProviderAttribute(string typeName)
		{
			this.Provider = Type.GetType(typeName, false);
		}

		public LicenseProviderAttribute(Type type)
		{
			this.Provider = type;
		}

		public Type LicenseProvider
		{
			get
			{
				return this.Provider;
			}
		}

		public override object TypeId
		{
			get
			{
				return base.ToString() + ((this.Provider == null) ? null : this.Provider.ToString());
			}
		}

		public override bool Equals(object obj)
		{
			return obj is LicenseProviderAttribute && (obj == this || ((LicenseProviderAttribute)obj).LicenseProvider.Equals(this.Provider));
		}

		public override int GetHashCode()
		{
			return this.Provider.GetHashCode();
		}

		private Type Provider;

		public static readonly LicenseProviderAttribute Default = new LicenseProviderAttribute();
	}
}
