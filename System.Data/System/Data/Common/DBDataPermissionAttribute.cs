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
				return this.allowBlankPassword;
			}
			set
			{
				this.allowBlankPassword = value;
			}
		}

		public string KeyRestrictions
		{
			get
			{
				if (this.keyRestrictions == null)
				{
					return string.Empty;
				}
				return this.keyRestrictions;
			}
			set
			{
				this.keyRestrictions = value;
			}
		}

		public string ConnectionString
		{
			get
			{
				if (this.connectionString == null)
				{
					return string.Empty;
				}
				return this.connectionString;
			}
			set
			{
				this.connectionString = value;
			}
		}

		public KeyRestrictionBehavior KeyRestrictionBehavior
		{
			get
			{
				return this.keyRestrictionBehavior;
			}
			set
			{
				ExceptionHelper.CheckEnumValue(typeof(KeyRestrictionBehavior), value);
				this.keyRestrictionBehavior = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShouldSerializeConnectionString()
		{
			return false;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShouldSerializeKeyRestrictions()
		{
			return false;
		}

		private bool allowBlankPassword;

		private string keyRestrictions;

		private KeyRestrictionBehavior keyRestrictionBehavior;

		private string connectionString;
	}
}
