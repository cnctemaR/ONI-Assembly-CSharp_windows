using System;
using System.ComponentModel;
using System.Security.Permissions;

namespace System.Data.Common
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	[Serializable]
	public abstract class DBDataPermissionAttribute : CodeAccessSecurityAttribute
	{
		protected DBDataPermissionAttribute(SecurityAction action)
			: base(action)
		{
		}

		public bool AllowBlankPassword
		{
			get
			{
				return this._allowBlankPassword;
			}
			set
			{
				this._allowBlankPassword = value;
			}
		}

		public string ConnectionString
		{
			get
			{
				string connectionString = this._connectionString;
				if (connectionString == null)
				{
					return string.Empty;
				}
				return connectionString;
			}
			set
			{
				this._connectionString = value;
			}
		}

		public KeyRestrictionBehavior KeyRestrictionBehavior
		{
			get
			{
				return this._behavior;
			}
			set
			{
				if (value <= KeyRestrictionBehavior.PreventUsage)
				{
					this._behavior = value;
					return;
				}
				throw ADP.InvalidKeyRestrictionBehavior(value);
			}
		}

		public string KeyRestrictions
		{
			get
			{
				string restrictions = this._restrictions;
				if (restrictions == null)
				{
					return ADP.StrEmpty;
				}
				return restrictions;
			}
			set
			{
				this._restrictions = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShouldSerializeConnectionString()
		{
			return this._connectionString != null;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShouldSerializeKeyRestrictions()
		{
			return this._restrictions != null;
		}

		private bool _allowBlankPassword;

		private string _connectionString;

		private string _restrictions;

		private KeyRestrictionBehavior _behavior;
	}
}
