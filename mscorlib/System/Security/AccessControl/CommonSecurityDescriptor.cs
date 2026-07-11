using System;
using System.Security.Principal;

namespace System.Security.AccessControl
{
	public sealed class CommonSecurityDescriptor : GenericSecurityDescriptor
	{
		public CommonSecurityDescriptor(bool isContainer, bool isDS, RawSecurityDescriptor rawSecurityDescriptor)
		{
			throw new NotImplementedException();
		}

		public CommonSecurityDescriptor(bool isContainer, bool isDS, string sddlForm)
		{
			throw new NotImplementedException();
		}

		public CommonSecurityDescriptor(bool isContainer, bool isDS, byte[] binaryForm, int offset)
		{
			throw new NotImplementedException();
		}

		public CommonSecurityDescriptor(bool isContainer, bool isDS, ControlFlags flags, SecurityIdentifier owner, SecurityIdentifier group, SystemAcl systemAcl, DiscretionaryAcl discretionaryAcl)
		{
			this.isContainer = isContainer;
			this.isDS = isDS;
			this.flags = flags;
			this.owner = owner;
			this.group = group;
			this.systemAcl = systemAcl;
			this.discretionaryAcl = discretionaryAcl;
			throw new NotImplementedException();
		}

		public override ControlFlags ControlFlags
		{
			get
			{
				return this.flags;
			}
		}

		public DiscretionaryAcl DiscretionaryAcl
		{
			get
			{
				return this.discretionaryAcl;
			}
			set
			{
				if (value == null)
				{
				}
				this.discretionaryAcl = value;
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
				return this.isContainer;
			}
		}

		public bool IsDiscretionaryAclCanonical
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public bool IsDS
		{
			get
			{
				return this.isDS;
			}
		}

		public bool IsSystemAclCanonical
		{
			get
			{
				throw new NotImplementedException();
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
				return this.systemAcl;
			}
			set
			{
				this.systemAcl = value;
			}
		}

		public void PurgeAccessControl(SecurityIdentifier sid)
		{
			throw new NotImplementedException();
		}

		public void PurgeAudit(SecurityIdentifier sid)
		{
			throw new NotImplementedException();
		}

		public void SetDiscretionaryAclProtection(bool isProtected, bool preserveInheritance)
		{
			throw new NotImplementedException();
		}

		public void SetSystemAclProtection(bool isProtected, bool preserveInheritance)
		{
			throw new NotImplementedException();
		}

		private bool isContainer;

		private bool isDS;

		private ControlFlags flags;

		private SecurityIdentifier owner;

		private SecurityIdentifier group;

		private SystemAcl systemAcl;

		private DiscretionaryAcl discretionaryAcl;
	}
}
