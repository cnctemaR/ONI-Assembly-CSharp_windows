using System;
using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	[ComVisible(true)]
	[Serializable]
	public sealed class StrongNameIdentityPermissionAttribute : CodeAccessSecurityAttribute
	{
		public StrongNameIdentityPermissionAttribute(SecurityAction action)
			: base(action)
		{
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

		public string PublicKey
		{
			get
			{
				return this.key;
			}
			set
			{
				this.key = value;
			}
		}

		public string Version
		{
			get
			{
				return this.version;
			}
			set
			{
				this.version = value;
			}
		}

		public override IPermission CreatePermission()
		{
			if (base.Unrestricted)
			{
				return new StrongNameIdentityPermission(PermissionState.Unrestricted);
			}
			if (this.name == null && this.key == null && this.version == null)
			{
				return new StrongNameIdentityPermission(PermissionState.None);
			}
			if (this.key == null)
			{
				throw new ArgumentException(Locale.GetText("PublicKey is required"));
			}
			StrongNamePublicKeyBlob strongNamePublicKeyBlob = StrongNamePublicKeyBlob.FromString(this.key);
			Version version = null;
			if (this.version != null)
			{
				version = new Version(this.version);
			}
			return new StrongNameIdentityPermission(strongNamePublicKeyBlob, this.name, version);
		}

		private string name;

		private string key;

		private string version;
	}
}
