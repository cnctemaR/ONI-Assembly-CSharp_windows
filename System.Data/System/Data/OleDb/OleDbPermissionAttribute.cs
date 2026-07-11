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

		[Obsolete("Provider property has been deprecated.  Use the Add method.  http://go.microsoft.com/fwlink/?linkid=14202")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string Provider
		{
			get
			{
				string providers = this._providers;
				if (providers == null)
				{
					return ADP.StrEmpty;
				}
				return providers;
			}
			set
			{
				this._providers = value;
			}
		}

		public override IPermission CreatePermission()
		{
			return new OleDbPermission(this);
		}

		private string _providers;
	}
}
