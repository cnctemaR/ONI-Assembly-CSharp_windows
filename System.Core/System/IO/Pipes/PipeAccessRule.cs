using System;
using System.Security.AccessControl;
using System.Security.Principal;

namespace System.IO.Pipes
{
	public sealed class PipeAccessRule : AccessRule
	{
		public PipeAccessRule(string identity, PipeAccessRights rights, AccessControlType type)
			: this(new NTAccount(identity), PipeAccessRule.AccessMaskFromRights(rights, type), false, type)
		{
		}

		public PipeAccessRule(IdentityReference identity, PipeAccessRights rights, AccessControlType type)
			: this(identity, PipeAccessRule.AccessMaskFromRights(rights, type), false, type)
		{
		}

		internal PipeAccessRule(IdentityReference identity, int accessMask, bool isInherited, AccessControlType type)
			: base(identity, accessMask, isInherited, InheritanceFlags.None, PropagationFlags.None, type)
		{
		}

		public PipeAccessRights PipeAccessRights
		{
			get
			{
				return PipeAccessRule.RightsFromAccessMask(base.AccessMask);
			}
		}

		internal static int AccessMaskFromRights(PipeAccessRights rights, AccessControlType controlType)
		{
			if (rights < (PipeAccessRights)0 || rights > (PipeAccessRights.ReadData | PipeAccessRights.WriteData | PipeAccessRights.ReadAttributes | PipeAccessRights.WriteAttributes | PipeAccessRights.ReadExtendedAttributes | PipeAccessRights.WriteExtendedAttributes | PipeAccessRights.CreateNewInstance | PipeAccessRights.Delete | PipeAccessRights.ReadPermissions | PipeAccessRights.ChangePermissions | PipeAccessRights.TakeOwnership | PipeAccessRights.Synchronize | PipeAccessRights.AccessSystemSecurity))
			{
				throw new ArgumentOutOfRangeException("rights", "Invalid PipeAccessRights value.");
			}
			if (controlType == AccessControlType.Allow)
			{
				rights |= PipeAccessRights.Synchronize;
			}
			else if (controlType == AccessControlType.Deny && rights != PipeAccessRights.FullControl)
			{
				rights &= ~PipeAccessRights.Synchronize;
			}
			return (int)rights;
		}

		internal static PipeAccessRights RightsFromAccessMask(int accessMask)
		{
			return (PipeAccessRights)accessMask;
		}
	}
}
