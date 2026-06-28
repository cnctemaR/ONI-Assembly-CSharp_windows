using System;
using System.ComponentModel;
using System.Data.Common;
using System.Security;
using System.Security.Permissions;

namespace System.Data.OleDb
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	[Serializable]
	public sealed class OleDbPermissionAttribute : DBDataPermissionAttribute
	{
		public OleDbPermissionAttribute(SecurityAction action)
			: base(action)
		{
		}

		[Obsolete]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
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

		public override IPermission CreatePermission()
		{
			return new OleDbPermission(this);
		}

		private string _provider;
	}
}
