using System;
using System.Collections;
using System.Security;
using System.Security.Permissions;

namespace System.Data.Common
{
	[Serializable]
	public abstract class DBDataPermission : CodeAccessPermission, IUnrestrictedPermission
	{
		[Obsolete("use DBDataPermission (PermissionState.None)", true)]
		protected DBDataPermission()
			: this(PermissionState.None)
		{
		}

		protected DBDataPermission(DBDataPermission permission)
		{
			if (permission == null)
			{
				throw new ArgumentNullException("permission");
			}
			this.state = permission.state;
			if (this.state != PermissionState.Unrestricted)
			{
				this.allowBlankPassword = permission.allowBlankPassword;
				this._connections = (Hashtable)permission._connections.Clone();
			}
		}

		protected DBDataPermission(DBDataPermissionAttribute permissionAttribute)
		{
			if (permissionAttribute == null)
			{
				throw new ArgumentNullException("permissionAttribute");
			}
			this._connections = new Hashtable();
			if (permissionAttribute.Unrestricted)
			{
				this.state = PermissionState.Unrestricted;
			}
			else
			{
				this.state = PermissionState.None;
				this.allowBlankPassword = permissionAttribute.AllowBlankPassword;
				if (permissionAttribute.ConnectionString.Length > 0)
				{
					this.Add(permissionAttribute.ConnectionString, permissionAttribute.KeyRestrictions, permissionAttribute.KeyRestrictionBehavior);
				}
			}
		}

		protected DBDataPermission(PermissionState state)
		{
			this.state = PermissionHelper.CheckPermissionState(state, true);
			this._connections = new Hashtable();
		}

		[Obsolete("use DBDataPermission (PermissionState.None)", true)]
		protected DBDataPermission(PermissionState state, bool allowBlankPassword)
			: this(state)
		{
			this.allowBlankPassword = allowBlankPassword;
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

		public virtual void Add(string connectionString, string restrictions, KeyRestrictionBehavior behavior)
		{
			this.state = PermissionState.None;
			this._connections[connectionString] = new object[] { restrictions, behavior };
		}

		protected void Clear()
		{
			this._connections.Clear();
		}

		public override IPermission Copy()
		{
			DBDataPermission dbdataPermission = this.CreateInstance();
			dbdataPermission.allowBlankPassword = this.allowBlankPassword;
			dbdataPermission._connections = (Hashtable)this._connections.Clone();
			return dbdataPermission;
		}

		protected virtual DBDataPermission CreateInstance()
		{
			return (DBDataPermission)Activator.CreateInstance(base.GetType(), new object[] { PermissionState.None });
		}

		public override void FromXml(SecurityElement securityElement)
		{
			PermissionHelper.CheckSecurityElement(securityElement, "securityElement", 1, 1);
			this.state = ((!PermissionHelper.IsUnrestricted(securityElement)) ? PermissionState.None : PermissionState.Unrestricted);
			this.allowBlankPassword = false;
			string text = securityElement.Attribute("AllowBlankPassword");
			if (text != null && !bool.TryParse(text, out this.allowBlankPassword))
			{
				this.allowBlankPassword = false;
			}
			if (securityElement.Children != null)
			{
				foreach (object obj in securityElement.Children)
				{
					SecurityElement securityElement2 = (SecurityElement)obj;
					string text2 = securityElement2.Attribute("ConnectionString");
					string text3 = securityElement2.Attribute("KeyRestrictions");
					KeyRestrictionBehavior keyRestrictionBehavior = (KeyRestrictionBehavior)((int)Enum.Parse(typeof(KeyRestrictionBehavior), securityElement2.Attribute("KeyRestrictionBehavior")));
					if (text2 != null && text2.Length > 0)
					{
						this.Add(text2, text3, keyRestrictionBehavior);
					}
				}
			}
		}

		public override IPermission Intersect(IPermission target)
		{
			DBDataPermission dbdataPermission = this.Cast(target);
			if (dbdataPermission == null)
			{
				return null;
			}
			if (this.IsUnrestricted())
			{
				if (dbdataPermission.IsUnrestricted())
				{
					DBDataPermission dbdataPermission2 = this.CreateInstance();
					dbdataPermission2.state = PermissionState.Unrestricted;
					return dbdataPermission2;
				}
				return dbdataPermission.Copy();
			}
			else
			{
				if (dbdataPermission.IsUnrestricted())
				{
					return this.Copy();
				}
				if (this.IsEmpty() || dbdataPermission.IsEmpty())
				{
					return null;
				}
				DBDataPermission dbdataPermission3 = this.CreateInstance();
				dbdataPermission3.allowBlankPassword = this.allowBlankPassword && dbdataPermission.allowBlankPassword;
				foreach (object obj in this._connections)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
					object obj2 = dbdataPermission._connections[dictionaryEntry.Key];
					if (obj2 != null)
					{
						dbdataPermission3._connections.Add(dictionaryEntry.Key, dictionaryEntry.Value);
					}
				}
				return (dbdataPermission3._connections.Count <= 0) ? null : dbdataPermission3;
			}
		}

		public override bool IsSubsetOf(IPermission target)
		{
			DBDataPermission dbdataPermission = this.Cast(target);
			if (dbdataPermission == null)
			{
				return this.IsEmpty();
			}
			if (dbdataPermission.IsUnrestricted())
			{
				return true;
			}
			if (this.IsUnrestricted())
			{
				return dbdataPermission.IsUnrestricted();
			}
			if (this.allowBlankPassword && !dbdataPermission.allowBlankPassword)
			{
				return false;
			}
			if (this._connections.Count > dbdataPermission._connections.Count)
			{
				return false;
			}
			foreach (object obj in this._connections)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				if (dbdataPermission._connections[dictionaryEntry.Key] == null)
				{
					return false;
				}
			}
			return true;
		}

		public bool IsUnrestricted()
		{
			return this.state == PermissionState.Unrestricted;
		}

		public override SecurityElement ToXml()
		{
			SecurityElement securityElement = PermissionHelper.Element(base.GetType(), 1);
			if (this.IsUnrestricted())
			{
				securityElement.AddAttribute("Unrestricted", "true");
			}
			else
			{
				securityElement.AddAttribute("AllowBlankPassword", this.allowBlankPassword.ToString());
				foreach (object obj in this._connections)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
					SecurityElement securityElement2 = new SecurityElement("add");
					securityElement2.AddAttribute("ConnectionString", (string)dictionaryEntry.Key);
					object[] array = (object[])dictionaryEntry.Value;
					securityElement2.AddAttribute("KeyRestrictions", (string)array[0]);
					KeyRestrictionBehavior keyRestrictionBehavior = (KeyRestrictionBehavior)((int)array[1]);
					securityElement2.AddAttribute("KeyRestrictionBehavior", keyRestrictionBehavior.ToString());
					securityElement.AddChild(securityElement2);
				}
			}
			return securityElement;
		}

		public override IPermission Union(IPermission target)
		{
			DBDataPermission dbdataPermission = this.Cast(target);
			if (dbdataPermission == null)
			{
				return this.Copy();
			}
			if (this.IsEmpty() && dbdataPermission.IsEmpty())
			{
				return this.Copy();
			}
			DBDataPermission dbdataPermission2 = this.CreateInstance();
			if (this.IsUnrestricted() || dbdataPermission.IsUnrestricted())
			{
				dbdataPermission2.state = PermissionState.Unrestricted;
			}
			else
			{
				dbdataPermission2.allowBlankPassword = this.allowBlankPassword || dbdataPermission.allowBlankPassword;
				dbdataPermission2._connections = new Hashtable(this._connections.Count + dbdataPermission._connections.Count);
				foreach (object obj in this._connections)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
					dbdataPermission2._connections.Add(dictionaryEntry.Key, dictionaryEntry.Value);
				}
				foreach (object obj2 in dbdataPermission._connections)
				{
					DictionaryEntry dictionaryEntry2 = (DictionaryEntry)obj2;
					dbdataPermission2._connections[dictionaryEntry2.Key] = dictionaryEntry2.Value;
				}
			}
			return dbdataPermission2;
		}

		private bool IsEmpty()
		{
			return this.state != PermissionState.Unrestricted && this._connections.Count == 0;
		}

		private DBDataPermission Cast(IPermission target)
		{
			if (target == null)
			{
				return null;
			}
			DBDataPermission dbdataPermission = target as DBDataPermission;
			if (dbdataPermission == null)
			{
				PermissionHelper.ThrowInvalidPermission(target, base.GetType());
			}
			return dbdataPermission;
		}

		private const int version = 1;

		private bool allowBlankPassword;

		private PermissionState state;

		private Hashtable _connections;
	}
}
