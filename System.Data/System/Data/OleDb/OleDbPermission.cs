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
		[Obsolete("OleDbPermission() has been deprecated.  Use the OleDbPermission(PermissionState.None) constructor.  http://go.microsoft.com/fwlink/?linkid=14202", true)]
		public OleDbPermission()
			: this(PermissionState.None)
		{
		}

		public OleDbPermission(PermissionState state)
			: base(state)
		{
		}

		[Obsolete("OleDbPermission(PermissionState state, Boolean allowBlankPassword) has been deprecated.  Use the OleDbPermission(PermissionState.None) constructor.  http://go.microsoft.com/fwlink/?linkid=14202", true)]
		public OleDbPermission(PermissionState state, bool allowBlankPassword)
			: this(state)
		{
			base.AllowBlankPassword = allowBlankPassword;
		}

		private OleDbPermission(OleDbPermission permission)
			: base(permission)
		{
		}

		internal OleDbPermission(OleDbPermissionAttribute permissionAttribute)
			: base(permissionAttribute)
		{
		}

		internal OleDbPermission(OleDbConnectionString constr)
			: base(constr)
		{
			if (constr == null || constr.IsEmpty)
			{
				base.Add(ADP.StrEmpty, ADP.StrEmpty, KeyRestrictionBehavior.AllowOnly);
			}
		}

		[Obsolete("Provider property has been deprecated.  Use the Add method.  http://go.microsoft.com/fwlink/?linkid=14202")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public string Provider
		{
			get
			{
				string text = this._providers;
				if (text == null)
				{
					string[] providerRestriction = this._providerRestriction;
					if (providerRestriction != null && providerRestriction.Length != 0)
					{
						text = providerRestriction[0];
						for (int i = 1; i < providerRestriction.Length; i++)
						{
							text = text + ";" + providerRestriction[i];
						}
					}
				}
				if (text == null)
				{
					return ADP.StrEmpty;
				}
				return text;
			}
			set
			{
				string[] array = null;
				if (!ADP.IsEmpty(value))
				{
					array = value.Split(new char[] { ';' });
					array = DBConnectionString.RemoveDuplicates(array);
				}
				this._providerRestriction = array;
				this._providers = value;
			}
		}

		public override IPermission Copy()
		{
			return new OleDbPermission(this);
		}

		private string[] _providerRestriction;

		private string _providers;
	}
}
