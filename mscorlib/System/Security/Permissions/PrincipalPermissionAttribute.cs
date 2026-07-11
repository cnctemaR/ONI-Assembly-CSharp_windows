using System;
using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	[ComVisible(true)]
	[Serializable]
	public sealed class PrincipalPermissionAttribute : CodeAccessSecurityAttribute
	{
		public PrincipalPermissionAttribute(SecurityAction action)
			: base(action)
		{
			this.authenticated = true;
		}

		public bool Authenticated
		{
			get
			{
				return this.authenticated;
			}
			set
			{
				this.authenticated = value;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		public string Role
		{
			get
			{
				return this.role;
			}
			set
			{
				this.role = value;
			}
		}

		public override IPermission CreatePermission()
		{
			PrincipalPermission principalPermission;
			if (base.Unrestricted)
			{
				principalPermission = new PrincipalPermission(PermissionState.Unrestricted);
			}
			else
			{
				principalPermission = new PrincipalPermission(this.name, this.role, this.authenticated);
			}
			return principalPermission;
		}

		private bool authenticated;

		private string name;

		private string role;
	}
}
