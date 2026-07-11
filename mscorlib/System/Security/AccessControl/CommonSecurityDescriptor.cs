using System;
using System.Security.Principal;

namespace System.Security.AccessControl
{
	public sealed class CommonSecurityDescriptor : GenericSecurityDescriptor
	{
		public CommonSecurityDescriptor(bool isContainer, bool isDS, RawSecurityDescriptor rawSecurityDescriptor)
		{
			this.Init(isContainer, isDS, rawSecurityDescriptor);
		}

		public CommonSecurityDescriptor(bool isContainer, bool isDS, string sddlForm)
		{
			this.Init(isContainer, isDS, new RawSecurityDescriptor(sddlForm));
		}

		public CommonSecurityDescriptor(bool isContainer, bool isDS, byte[] binaryForm, int offset)
		{
			this.Init(isContainer, isDS, new RawSecurityDescriptor(binaryForm, offset));
		}

		public CommonSecurityDescriptor(bool isContainer, bool isDS, ControlFlags flags, SecurityIdentifier owner, SecurityIdentifier group, SystemAcl systemAcl, DiscretionaryAcl discretionaryAcl)
		{
			this.Init(isContainer, isDS, flags, owner, group, systemAcl, discretionaryAcl);
		}

		private void Init(bool isContainer, bool isDS, RawSecurityDescriptor rawSecurityDescriptor)
		{
			if (rawSecurityDescriptor == null)
			{
				throw new ArgumentNullException("rawSecurityDescriptor");
			}
			SystemAcl systemAcl = null;
			if (rawSecurityDescriptor.SystemAcl != null)
			{
				systemAcl = new SystemAcl(isContainer, isDS, rawSecurityDescriptor.SystemAcl);
			}
			DiscretionaryAcl discretionaryAcl = null;
			if (rawSecurityDescriptor.DiscretionaryAcl != null)
			{
				discretionaryAcl = new DiscretionaryAcl(isContainer, isDS, rawSecurityDescriptor.DiscretionaryAcl);
			}
			this.Init(isContainer, isDS, rawSecurityDescriptor.ControlFlags, rawSecurityDescriptor.Owner, rawSecurityDescriptor.Group, systemAcl, discretionaryAcl);
		}

		private void Init(bool isContainer, bool isDS, ControlFlags flags, SecurityIdentifier owner, SecurityIdentifier group, SystemAcl systemAcl, DiscretionaryAcl discretionaryAcl)
		{
			this.flags = flags & ~ControlFlags.SystemAclPresent;
			this.is_container = isContainer;
			this.is_ds = isDS;
			this.Owner = owner;
			this.Group = group;
			this.SystemAcl = systemAcl;
			this.DiscretionaryAcl = discretionaryAcl;
		}

		public override ControlFlags ControlFlags
		{
			get
			{
				ControlFlags controlFlags = this.flags;
				controlFlags |= ControlFlags.DiscretionaryAclPresent;
				controlFlags |= ControlFlags.SelfRelative;
				if (this.SystemAcl != null)
				{
					controlFlags |= ControlFlags.SystemAclPresent;
				}
				return controlFlags;
			}
		}

		public DiscretionaryAcl DiscretionaryAcl
		{
			get
			{
				return this.discretionary_acl;
			}
			set
			{
				if (value == null)
				{
					value = new DiscretionaryAcl(this.IsContainer, this.IsDS, 1);
					value.AddAccess(AccessControlType.Allow, new SecurityIdentifier("WD"), -1, this.IsContainer ? (InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit) : InheritanceFlags.None, PropagationFlags.None);
					value.IsAefa = true;
				}
				this.CheckAclConsistency(value);
				this.discretionary_acl = value;
			}
		}

		internal override GenericAcl InternalDacl
		{
			get
			{
				return this.DiscretionaryAcl;
			}
		}

		public override SecurityIdentifier Group
		{
			get
			{
				return this.group;
			}
			set
			{
				this.group = value;
			}
		}

		public bool IsContainer
		{
			get
			{
				return this.is_container;
			}
		}

		public bool IsDiscretionaryAclCanonical
		{
			get
			{
				return this.DiscretionaryAcl.IsCanonical;
			}
		}

		public bool IsDS
		{
			get
			{
				return this.is_ds;
			}
		}

		public bool IsSystemAclCanonical
		{
			get
			{
				return this.SystemAcl == null || this.SystemAcl.IsCanonical;
			}
		}

		public override SecurityIdentifier Owner
		{
			get
			{
				return this.owner;
			}
			set
			{
				this.owner = value;
			}
		}

		public SystemAcl SystemAcl
		{
			get
			{
				return this.system_acl;
			}
			set
			{
				if (value != null)
				{
					this.CheckAclConsistency(value);
				}
				this.system_acl = value;
			}
		}

		internal override GenericAcl InternalSacl
		{
			get
			{
				return this.SystemAcl;
			}
		}

		public void PurgeAccessControl(SecurityIdentifier sid)
		{
			this.DiscretionaryAcl.Purge(sid);
		}

		public void PurgeAudit(SecurityIdentifier sid)
		{
			if (this.SystemAcl != null)
			{
				this.SystemAcl.Purge(sid);
			}
		}

		public void SetDiscretionaryAclProtection(bool isProtected, bool preserveInheritance)
		{
			this.DiscretionaryAcl.IsAefa = false;
			if (!isProtected)
			{
				this.flags &= ~ControlFlags.DiscretionaryAclProtected;
				return;
			}
			this.flags |= ControlFlags.DiscretionaryAclProtected;
			if (!preserveInheritance)
			{
				this.DiscretionaryAcl.RemoveInheritedAces();
			}
		}

		public void SetSystemAclProtection(bool isProtected, bool preserveInheritance)
		{
			if (!isProtected)
			{
				this.flags &= ~ControlFlags.SystemAclProtected;
				return;
			}
			this.flags |= ControlFlags.SystemAclProtected;
			if (!preserveInheritance && this.SystemAcl != null)
			{
				this.SystemAcl.RemoveInheritedAces();
			}
		}

		public void AddDiscretionaryAcl(byte revision, int trusted)
		{
			this.DiscretionaryAcl = new DiscretionaryAcl(this.IsContainer, this.IsDS, revision, trusted);
			this.flags |= ControlFlags.DiscretionaryAclPresent;
		}

		public void AddSystemAcl(byte revision, int trusted)
		{
			this.SystemAcl = new SystemAcl(this.IsContainer, this.IsDS, revision, trusted);
			this.flags |= ControlFlags.SystemAclPresent;
		}

		private void CheckAclConsistency(CommonAcl acl)
		{
			if (this.IsContainer != acl.IsContainer)
			{
				throw new ArgumentException("IsContainer must match between descriptor and ACL.");
			}
			if (this.IsDS != acl.IsDS)
			{
				throw new ArgumentException("IsDS must match between descriptor and ACL.");
			}
		}

		internal override bool DaclIsUnmodifiedAefa
		{
			get
			{
				return this.DiscretionaryAcl.IsAefa;
			}
		}

		private bool is_container;

		private bool is_ds;

		private ControlFlags flags;

		private SecurityIdentifier owner;

		private SecurityIdentifier group;

		private SystemAcl system_acl;

		private DiscretionaryAcl discretionary_acl;
	}
}
