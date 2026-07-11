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
			: base(null)
		{
		}

		public OleDbPermission(PermissionState state)
			: base(null)
		{
		}

		[Obsolete("use OleDbPermission(PermissionState.None)", true)]
		public OleDbPermission(PermissionState state, bool allowBlankPassword)
			: base(null)
		{
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete]
		public string Provider
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public override IPermission Copy()
		{
			throw null;
		}
	}
}
