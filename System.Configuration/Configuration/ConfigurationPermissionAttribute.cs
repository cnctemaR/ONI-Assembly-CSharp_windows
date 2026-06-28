using System;
using System.Security;
using System.Security.Permissions;

namespace System.Configuration
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
	[Serializable]
	public sealed class ConfigurationPermissionAttribute : CodeAccessSecurityAttribute
	{
		public ConfigurationPermissionAttribute(SecurityAction action)
			: base(action)
		{
		}

		public override IPermission CreatePermission()
		{
			return new ConfigurationPermission((!base.Unrestricted) ? PermissionState.None : PermissionState.Unrestricted);
		}
	}
}
