using System;
using System.Security.Principal;

namespace System.Security.AccessControl
{
	public sealed class CompoundAce : KnownAce
	{
		public CompoundAce(AceFlags flags, int accessMask, CompoundAceType compoundAceType, SecurityIdentifier sid)
			: base(AceType.AccessAllowedCompound, flags)
		{
			this.compound_ace_type = compoundAceType;
			base.AccessMask = accessMask;
			base.SecurityIdentifier = sid;
		}

		[MonoTODO]
		public override int BinaryLength
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public CompoundAceType CompoundAceType
		{
			get
			{
				return this.compound_ace_type;
			}
			set
			{
				this.compound_ace_type = value;
			}
		}

		[MonoTODO]
		public override void GetBinaryForm(byte[] binaryForm, int offset)
		{
			throw new NotImplementedException();
		}

		internal override string GetSddlForm()
		{
			throw new NotImplementedException();
		}

		private CompoundAceType compound_ace_type;
	}
}
