using System;

namespace System.Security.Permissions
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	[Serializable]
	public sealed class DataProtectionPermissionAttribute : CodeAccessSecurityAttribute
	{
		public DataProtectionPermissionAttribute(SecurityAction action)
			: base(action)
		{
		}

		public DataProtectionPermissionFlags Flags
		{
			get
			{
				return this._flags;
			}
			set
			{
				if ((value & DataProtectionPermissionFlags.AllFlags) != value)
				{
					string text = string.Format(Locale.GetText("Invalid flags {0}"), value);
					throw new ArgumentException(text, "DataProtectionPermissionFlags");
				}
				this._flags = value;
			}
		}

		public bool ProtectData
		{
			get
			{
				return (this._flags & DataProtectionPermissionFlags.ProtectData) != DataProtectionPermissionFlags.NoFlags;
			}
			set
			{
				if (value)
				{
					this._flags |= DataProtectionPermissionFlags.ProtectData;
				}
				else
				{
					this._flags &= ~DataProtectionPermissionFlags.ProtectData;
				}
			}
		}

		public bool UnprotectData
		{
			get
			{
				return (this._flags & DataProtectionPermissionFlags.UnprotectData) != DataProtectionPermissionFlags.NoFlags;
			}
			set
			{
				if (value)
				{
					this._flags |= DataProtectionPermissionFlags.UnprotectData;
				}
				else
				{
					this._flags &= ~DataProtectionPermissionFlags.UnprotectData;
				}
			}
		}

		public bool ProtectMemory
		{
			get
			{
				return (this._flags & DataProtectionPermissionFlags.ProtectMemory) != DataProtectionPermissionFlags.NoFlags;
			}
			set
			{
				if (value)
				{
					this._flags |= DataProtectionPermissionFlags.ProtectMemory;
				}
				else
				{
					this._flags &= ~DataProtectionPermissionFlags.ProtectMemory;
				}
			}
		}

		public bool UnprotectMemory
		{
			get
			{
				return (this._flags & DataProtectionPermissionFlags.UnprotectMemory) != DataProtectionPermissionFlags.NoFlags;
			}
			set
			{
				if (value)
				{
					this._flags |= DataProtectionPermissionFlags.UnprotectMemory;
				}
				else
				{
					this._flags &= ~DataProtectionPermissionFlags.UnprotectMemory;
				}
			}
		}

		public override IPermission CreatePermission()
		{
			DataProtectionPermission dataProtectionPermission;
			if (base.Unrestricted)
			{
				dataProtectionPermission = new DataProtectionPermission(PermissionState.Unrestricted);
			}
			else
			{
				dataProtectionPermission = new DataProtectionPermission(this._flags);
			}
			return dataProtectionPermission;
		}

		private DataProtectionPermissionFlags _flags;
	}
}
