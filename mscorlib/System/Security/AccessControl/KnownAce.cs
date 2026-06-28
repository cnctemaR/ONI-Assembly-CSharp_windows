using System;
using System.Security.Principal;

namespace System.Security.AccessControl
{
	public abstract class KnownAce : GenericAce
	{
		internal KnownAce(InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags)
			: base(inheritanceFlags, propagationFlags)
		{
		}

		public int AccessMask
		{
			get
			{
				return this.access_mask;
			}
			set
			{
				this.access_mask = value;
			}
		}

		public SecurityIdentifier SecurityIdentifier
		{
			get
			{
				return this.identifier;
			}
			set
			{
				this.identifier = value;
			}
		}

		private int access_mask;

		private SecurityIdentifier identifier;
	}
}
