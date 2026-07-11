using System;
using System.Collections;
using System.Reflection;
using System.Security.Permissions;

namespace System.ComponentModel.Design
{
	[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
	[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
	public class DesigntimeLicenseContext : LicenseContext
	{
		public override LicenseUsageMode UsageMode
		{
			get
			{
				return LicenseUsageMode.Designtime;
			}
		}

		public override string GetSavedLicenseKey(Type type, Assembly resourceAssembly)
		{
			return null;
		}

		public override void SetSavedLicenseKey(Type type, string key)
		{
			this.savedLicenseKeys[type.AssemblyQualifiedName] = key;
		}

		internal Hashtable savedLicenseKeys = new Hashtable();
	}
}
