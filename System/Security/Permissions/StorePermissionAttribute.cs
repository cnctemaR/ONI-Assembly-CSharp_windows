using System;

namespace System.Security.Permissions
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	[Serializable]
	public sealed class StorePermissionAttribute : CodeAccessSecurityAttribute
	{
		public StorePermissionAttribute(SecurityAction action)
			: base(action)
		{
			this._flags = StorePermissionFlags.NoFlags;
		}

		public StorePermissionFlags Flags
		{
			get
			{
				return this._flags;
			}
			set
			{
				if ((value & StorePermissionFlags.AllFlags) != value)
				{
					string text = string.Format(global::Locale.GetText("Invalid flags {0}"), value);
					throw new ArgumentException(text, "StorePermissionFlags");
				}
				this._flags = value;
			}
		}

		public bool AddToStore
		{
			get
			{
				return (this._flags & StorePermissionFlags.AddToStore) != StorePermissionFlags.NoFlags;
			}
			set
			{
				if (value)
				{
					this._flags |= StorePermissionFlags.AddToStore;
				}
				else
				{
					this._flags &= ~StorePermissionFlags.AddToStore;
				}
			}
		}

		public bool CreateStore
		{
			get
			{
				return (this._flags & StorePermissionFlags.CreateStore) != StorePermissionFlags.NoFlags;
			}
			set
			{
				if (value)
				{
					this._flags |= StorePermissionFlags.CreateStore;
				}
				else
				{
					this._flags &= ~StorePermissionFlags.CreateStore;
				}
			}
		}

		public bool DeleteStore
		{
			get
			{
				return (this._flags & StorePermissionFlags.DeleteStore) != StorePermissionFlags.NoFlags;
			}
			set
			{
				if (value)
				{
					this._flags |= StorePermissionFlags.DeleteStore;
				}
				else
				{
					this._flags &= ~StorePermissionFlags.DeleteStore;
				}
			}
		}

		public bool EnumerateCertificates
		{
			get
			{
				return (this._flags & StorePermissionFlags.EnumerateCertificates) != StorePermissionFlags.NoFlags;
			}
			set
			{
				if (value)
				{
					this._flags |= StorePermissionFlags.EnumerateCertificates;
				}
				else
				{
					this._flags &= ~StorePermissionFlags.EnumerateCertificates;
				}
			}
		}

		public bool EnumerateStores
		{
			get
			{
				return (this._flags & StorePermissionFlags.EnumerateStores) != StorePermissionFlags.NoFlags;
			}
			set
			{
				if (value)
				{
					this._flags |= StorePermissionFlags.EnumerateStores;
				}
				else
				{
					this._flags &= ~StorePermissionFlags.EnumerateStores;
				}
			}
		}

		public bool OpenStore
		{
			get
			{
				return (this._flags & StorePermissionFlags.OpenStore) != StorePermissionFlags.NoFlags;
			}
			set
			{
				if (value)
				{
					this._flags |= StorePermissionFlags.OpenStore;
				}
				else
				{
					this._flags &= ~StorePermissionFlags.OpenStore;
				}
			}
		}

		public bool RemoveFromStore
		{
			get
			{
				return (this._flags & StorePermissionFlags.RemoveFromStore) != StorePermissionFlags.NoFlags;
			}
			set
			{
				if (value)
				{
					this._flags |= StorePermissionFlags.RemoveFromStore;
				}
				else
				{
					this._flags &= ~StorePermissionFlags.RemoveFromStore;
				}
			}
		}

		public override IPermission CreatePermission()
		{
			StorePermission storePermission;
			if (base.Unrestricted)
			{
				storePermission = new StorePermission(PermissionState.Unrestricted);
			}
			else
			{
				storePermission = new StorePermission(this._flags);
			}
			return storePermission;
		}

		private StorePermissionFlags _flags;
	}
}
