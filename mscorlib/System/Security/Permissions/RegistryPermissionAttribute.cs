using System;
using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	[Serializable]
	public sealed class RegistryPermissionAttribute : CodeAccessSecurityAttribute
	{
		public RegistryPermissionAttribute(SecurityAction action)
			: base(action)
		{
		}

		[Obsolete("use newer properties")]
		public string All
		{
			get
			{
				throw new NotSupportedException("All");
			}
			set
			{
				this.create = value;
				this.read = value;
				this.write = value;
			}
		}

		public string Create
		{
			get
			{
				return this.create;
			}
			set
			{
				this.create = value;
			}
		}

		public string Read
		{
			get
			{
				return this.read;
			}
			set
			{
				this.read = value;
			}
		}

		public string Write
		{
			get
			{
				return this.write;
			}
			set
			{
				this.write = value;
			}
		}

		public string ChangeAccessControl
		{
			get
			{
				return this.changeAccessControl;
			}
			set
			{
				this.changeAccessControl = value;
			}
		}

		public string ViewAccessControl
		{
			get
			{
				return this.viewAccessControl;
			}
			set
			{
				this.viewAccessControl = value;
			}
		}

		public string ViewAndModify
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				this.create = value;
				this.read = value;
				this.write = value;
			}
		}

		public override IPermission CreatePermission()
		{
			RegistryPermission registryPermission;
			if (base.Unrestricted)
			{
				registryPermission = new RegistryPermission(PermissionState.Unrestricted);
			}
			else
			{
				registryPermission = new RegistryPermission(PermissionState.None);
				if (this.create != null)
				{
					registryPermission.AddPathList(RegistryPermissionAccess.Create, this.create);
				}
				if (this.read != null)
				{
					registryPermission.AddPathList(RegistryPermissionAccess.Read, this.read);
				}
				if (this.write != null)
				{
					registryPermission.AddPathList(RegistryPermissionAccess.Write, this.write);
				}
			}
			return registryPermission;
		}

		private string create;

		private string read;

		private string write;

		private string changeAccessControl;

		private string viewAccessControl;
	}
}
