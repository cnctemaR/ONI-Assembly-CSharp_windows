using System;
using System.Security.Principal;

namespace System.Security.AccessControl
{
	public abstract class AuditRule : AuthorizationRule
	{
		protected AuditRule(IdentityReference identity, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags auditFlags)
			: base(identity, accessMask, isInherited, inheritanceFlags, propagationFlags)
		{
			if (auditFlags != ((AuditFlags.Success | AuditFlags.Failure) & auditFlags))
			{
				throw new ArgumentException("Invalid audit flags.", "auditFlags");
			}
			this.auditFlags = auditFlags;
		}

		public AuditFlags AuditFlags
		{
			get
			{
				return this.auditFlags;
			}
		}

		private AuditFlags auditFlags;
	}
}
