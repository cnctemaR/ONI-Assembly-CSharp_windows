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
			: base((SecurityAction)0)
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

		public override IPermission CreatePermission()
		{
			throw null;
		}
	}
}
