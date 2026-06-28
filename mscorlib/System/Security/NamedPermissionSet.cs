using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Security
{
	[ComVisible(true)]
	[Serializable]
	public sealed class NamedPermissionSet : PermissionSet
	{
		internal NamedPermissionSet()
		{
		}

		public NamedPermissionSet(string name, PermissionSet permSet)
			: base(permSet)
		{
			this.Name = name;
		}

		public NamedPermissionSet(string name, PermissionState state)
			: base(state)
		{
			this.Name = name;
		}

		public NamedPermissionSet(NamedPermissionSet permSet)
			: base(permSet)
		{
			this.name = permSet.name;
			this.description = permSet.description;
		}

		public NamedPermissionSet(string name)
			: this(name, PermissionState.Unrestricted)
		{
		}

		public string Description
		{
			get
			{
				return this.description;
			}
			set
			{
				this.description = value;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				if (value == null || value == string.Empty)
				{
					throw new ArgumentException(Locale.GetText("invalid name"));
				}
				this.name = value;
			}
		}

		public override PermissionSet Copy()
		{
			return new NamedPermissionSet(this);
		}

		public NamedPermissionSet Copy(string name)
		{
			return new NamedPermissionSet(this)
			{
				Name = name
			};
		}

		public override void FromXml(SecurityElement et)
		{
			base.FromXml(et);
			this.name = et.Attribute("Name");
			this.description = et.Attribute("Description");
			if (this.description == null)
			{
				this.description = string.Empty;
			}
		}

		public override SecurityElement ToXml()
		{
			SecurityElement securityElement = base.ToXml();
			if (this.name != null)
			{
				securityElement.AddAttribute("Name", this.name);
			}
			if (this.description != null)
			{
				securityElement.AddAttribute("Description", this.description);
			}
			return securityElement;
		}

		[ComVisible(false)]
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			NamedPermissionSet namedPermissionSet = obj as NamedPermissionSet;
			return namedPermissionSet != null && this.name == namedPermissionSet.Name && base.Equals(obj);
		}

		[ComVisible(false)]
		public override int GetHashCode()
		{
			int num = base.GetHashCode();
			if (this.name != null)
			{
				num ^= this.name.GetHashCode();
			}
			return num;
		}

		private string name;

		private string description;
	}
}
