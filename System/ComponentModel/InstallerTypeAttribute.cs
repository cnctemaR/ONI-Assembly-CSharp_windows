using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Class)]
	public class InstallerTypeAttribute : Attribute
	{
		public InstallerTypeAttribute(string typeName)
		{
			this.installer = Type.GetType(typeName, false);
		}

		public InstallerTypeAttribute(Type installerType)
		{
			this.installer = installerType;
		}

		public virtual Type InstallerType
		{
			get
			{
				return this.installer;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is InstallerTypeAttribute && (obj == this || ((InstallerTypeAttribute)obj).InstallerType == this.installer);
		}

		public override int GetHashCode()
		{
			return this.installer.GetHashCode();
		}

		private Type installer;
	}
}
