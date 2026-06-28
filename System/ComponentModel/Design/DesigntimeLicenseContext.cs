using System;
using System.Collections;
using System.Reflection;

namespace System.ComponentModel.Design
{
	public class DesigntimeLicenseContext : LicenseContext
	{
		public override string GetSavedLicenseKey(Type type, Assembly resourceAssembly)
		{
			return (string)this.keys[type];
		}

		public override void SetSavedLicenseKey(Type type, string key)
		{
			this.keys[type] = key;
		}

		public override LicenseUsageMode UsageMode
		{
			get
			{
				return LicenseUsageMode.Designtime;
			}
		}

		internal Hashtable keys = new Hashtable();
	}
}
