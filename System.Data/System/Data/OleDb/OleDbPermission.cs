using System;
using System.ComponentModel;
using System.Data.Common;
using System.Security;
using System.Security.Permissions;

namespace System.Data.OleDb
{
	[Serializable]
	public sealed class OleDbPermission : DBDataPermission
	{
		[Obsolete("use OleDbPermission(PermissionState.None)", true)]
		public OleDbPermission()
			: base(PermissionState.None)
		{
		}

		public OleDbPermission(PermissionState state)
			: base(state)
		{
		}

		[Obsolete("use OleDbPermission(PermissionState.None)", true)]
		public OleDbPermission(PermissionState state, bool allowBlankPassword)
			: base(state)
		{
			base.AllowBlankPassword = allowBlankPassword;
		}

		internal OleDbPermission(DBDataPermission permission)
			: base(permission)
		{
		}

		internal OleDbPermission(DBDataPermissionAttribute attribute)
			: base(attribute)
		{
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[Obsolete]
		public string Provider
		{
			get
			{
				if (this._provider == null)
				{
					return string.Empty;
				}
				return this._provider;
			}
			set
			{
				this._provider = value;
			}
		}

		public override IPermission Copy()
		{
			return new OleDbPermission(this);
		}

		private string _provider;
	}
}
