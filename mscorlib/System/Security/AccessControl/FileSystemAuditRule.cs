using System;
using System.Security.Principal;

namespace System.Security.AccessControl
{
	public sealed class FileSystemAuditRule : AuditRule
	{
		public FileSystemAuditRule(IdentityReference identity, FileSystemRights fileSystemRights, AuditFlags flags)
			: this(identity, fileSystemRights, InheritanceFlags.None, PropagationFlags.None, flags)
		{
		}

		public FileSystemAuditRule(string identity, FileSystemRights fileSystemRights, AuditFlags flags)
			: this(new NTAccount(identity), fileSystemRights, flags)
		{
		}

		public FileSystemAuditRule(IdentityReference identity, FileSystemRights fileSystemRights, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags flags)
			: this(identity, fileSystemRights, false, inheritanceFlags, propagationFlags, flags)
		{
		}

		internal FileSystemAuditRule(IdentityReference identity, FileSystemRights fileSystemRights, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags flags)
			: base(identity, (int)fileSystemRights, isInherited, inheritanceFlags, propagationFlags, flags)
		{
		}

		public FileSystemAuditRule(string identity, FileSystemRights fileSystemRights, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags flags)
			: this(new NTAccount(identity), fileSystemRights, inheritanceFlags, propagationFlags, flags)
		{
		}

		public FileSystemRights FileSystemRights
		{
			get
			{
				return (FileSystemRights)base.AccessMask;
			}
		}
	}
}
