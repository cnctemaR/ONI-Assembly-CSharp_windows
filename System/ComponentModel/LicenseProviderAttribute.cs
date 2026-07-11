using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class LicenseProviderAttribute : Attribute
	{
		public LicenseProviderAttribute()
			: this(null)
		{
		}

		public LicenseProviderAttribute(string typeName)
		{
			this.licenseProviderName = typeName;
		}

		public LicenseProviderAttribute(Type type)
		{
			this.licenseProviderType = type;
		}

		public Type LicenseProvider
		{
			get
			{
				if (this.licenseProviderType == null && this.licenseProviderName != null)
				{
					this.licenseProviderType = Type.GetType(this.licenseProviderName);
				}
				return this.licenseProviderType;
			}
		}

		public override object TypeId
		{
			get
			{
				string fullName = this.licenseProviderName;
				if (fullName == null && this.licenseProviderType != null)
				{
					fullName = this.licenseProviderType.FullName;
				}
				return base.GetType().FullName + fullName;
			}
		}

		public override bool Equals(object value)
		{
			if (value is LicenseProviderAttribute && value != null)
			{
				Type licenseProvider = ((LicenseProviderAttribute)value).LicenseProvider;
				if (licenseProvider == this.LicenseProvider)
				{
					return true;
				}
				if (licenseProvider != null && licenseProvider.Equals(this.LicenseProvider))
				{
					return true;
				}
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static readonly LicenseProviderAttribute Default = new LicenseProviderAttribute();

		private Type licenseProviderType;

		private string licenseProviderName;
	}
}
