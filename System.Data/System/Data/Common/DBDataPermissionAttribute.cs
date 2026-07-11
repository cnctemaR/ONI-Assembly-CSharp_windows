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
			: base((SecurityAction)0)
		{
		}

		public bool AllowBlankPassword
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public string ConnectionString
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public KeyRestrictionBehavior KeyRestrictionBehavior
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public string KeyRestrictions
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShouldSerializeConnectionString()
		{
			throw null;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShouldSerializeKeyRestrictions()
		{
			throw null;
		}
	}
}
