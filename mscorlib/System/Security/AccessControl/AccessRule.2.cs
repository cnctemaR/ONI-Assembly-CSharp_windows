using System;
using System.Security.Principal;

namespace System.Security.AccessControl
{
	public class AccessRule<T> : AccessRule where T : struct
	{
		public AccessRule(string identity, T rights, AccessControlType type)
			: this(new NTAccount(identity), rights, type)
		{
		}

		public AccessRule(IdentityReference identity, T rights, AccessControlType type)
			: this(identity, rights, InheritanceFlags.None, PropagationFlags.None, type)
		{
		}

		public AccessRule(string identity, T rights, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AccessControlType type)
			: this(new NTAccount(identity), rights, inheritanceFlags, propagationFlags, type)
		{
		}

		public AccessRule(IdentityReference identity, T rights, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AccessControlType type)
			: this(identity, (int)((object)rights), false, inheritanceFlags, propagationFlags, type)
		{
		}

		internal AccessRule(IdentityReference identity, int rights, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AccessControlType type)
			: base(identity, rights, isInherited, inheritanceFlags, propagationFlags, type)
		{
		}

		public T Rights
		{
			get
			{
				return (T)((object)base.AccessMask);
			}
		}
	}
}
