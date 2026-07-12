using System;

namespace System.ComponentModel.Design.Serialization
{
	public readonly struct MemberRelationship
	{
		public MemberRelationship(object owner, MemberDescriptor member)
		{
			if (owner == null)
			{
				throw new ArgumentNullException("owner");
			}
			if (member == null)
			{
				throw new ArgumentNullException("member");
			}
			this.Owner = owner;
			this.Member = member;
		}

		public bool IsEmpty
		{
			get
			{
				return this.Owner == null;
			}
		}

		public MemberDescriptor Member { get; }

		public object Owner { get; }

		public override bool Equals(object obj)
		{
			if (!(obj is MemberRelationship))
			{
				return false;
			}
			MemberRelationship memberRelationship = (MemberRelationship)obj;
			return memberRelationship.Owner == this.Owner && memberRelationship.Member == this.Member;
		}

		public override int GetHashCode()
		{
			if (this.Owner == null)
			{
				return base.GetHashCode();
			}
			return this.Owner.GetHashCode() ^ this.Member.GetHashCode();
		}

		public static bool operator ==(MemberRelationship left, MemberRelationship right)
		{
			return left.Owner == right.Owner && left.Member == right.Member;
		}

		public static bool operator !=(MemberRelationship left, MemberRelationship right)
		{
			return !(left == right);
		}

		public static readonly MemberRelationship Empty;
	}
}
