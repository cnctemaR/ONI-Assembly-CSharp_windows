using System;
using System.Security.AccessControl;
using System.Security.Principal;

namespace System.IO.Pipes
{
	public sealed class PipeAuditRule : AuditRule
	{
		public PipeAuditRule(IdentityReference identity, PipeAccessRights rights, AuditFlags flags)
			: this(identity, PipeAuditRule.AccessMaskFromRights(rights), false, flags)
		{
		}

		public PipeAuditRule(string identity, PipeAccessRights rights, AuditFlags flags)
			: this(new NTAccount(identity), PipeAuditRule.AccessMaskFromRights(rights), false, flags)
		{
		}

		internal PipeAuditRule(IdentityReference identity, int accessMask, bool isInherited, AuditFlags flags)
			: base(identity, accessMask, isInherited, InheritanceFlags.None, PropagationFlags.None, flags)
		{
		}

		private static int AccessMaskFromRights(PipeAccessRights rights)
		{
			if (rights < (PipeAccessRights)0 || rights > (PipeAccessRights.ReadData | PipeAccessRights.WriteData | PipeAccessRights.ReadAttributes | PipeAccessRights.WriteAttributes | PipeAccessRights.ReadExtendedAttributes | PipeAccessRights.WriteExtendedAttributes | PipeAccessRights.CreateNewInstance | PipeAccessRights.Delete | PipeAccessRights.ReadPermissions | PipeAccessRights.ChangePermissions | PipeAccessRights.TakeOwnership | PipeAccessRights.Synchronize | PipeAccessRights.AccessSystemSecurity))
			{
				throw new ArgumentOutOfRangeException("rights", "Invalid PipeAccessRights value.");
			}
			return (int)rights;
		}

		public PipeAccessRights PipeAccessRights
		{
			get
			{
				return PipeAccessRule.RightsFromAccessMask(base.AccessMask);
			}
		}
	}
}
