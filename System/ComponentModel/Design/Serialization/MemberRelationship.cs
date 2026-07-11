using System;

namespace System.ComponentModel.Design.Serialization
{
	public struct MemberRelationship
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
			this._owner = owner;
			this._member = member;
		}

		public bool IsEmpty
		{
			get
			{
				return this._owner == null;
			}
		}

		public MemberDescriptor Member
		{
			get
			{
				return this._member;
			}
		}

		public object Owner
		{
			get
			{
				return this._owner;
			}
		}

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
			if (this._owner == null)
			{
				return base.GetHashCode();
			}
			return this._owner.GetHashCode() ^ this._member.GetHashCode();
		}

		public static bool operator ==(MemberRelationship left, MemberRelationship right)
		{
			return left.Owner == right.Owner && left.Member == right.Member;
		}

		public static bool operator !=(MemberRelationship left, MemberRelationship right)
		{
			return !(left == right);
		}

		private object _owner;

		private MemberDescriptor _member;

		public static readonly MemberRelationship Empty;
	}
}
