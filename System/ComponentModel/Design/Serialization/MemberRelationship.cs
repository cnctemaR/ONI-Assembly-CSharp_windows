using System;

namespace System.ComponentModel.Design.Serialization
{
	public struct MemberRelationship
	{
		public MemberRelationship(object owner, MemberDescriptor member)
		{
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

		public object Owner
		{
			get
			{
				return this._owner;
			}
		}

		public MemberDescriptor Member
		{
			get
			{
				return this._member;
			}
		}

		public override int GetHashCode()
		{
			if (this._owner != null && this._member != null)
			{
				return this._member.GetHashCode() ^ this._owner.GetHashCode();
			}
			return base.GetHashCode();
		}

		public override bool Equals(object o)
		{
			return o is MemberRelationship && (MemberRelationship)o == this;
		}

		public static bool operator ==(MemberRelationship left, MemberRelationship right)
		{
			return left.Owner == right.Owner && left.Member == right.Member;
		}

		public static bool operator !=(MemberRelationship left, MemberRelationship right)
		{
			return !(left == right);
		}

		public static readonly MemberRelationship Empty = default(MemberRelationship);

		private object _owner;

		private MemberDescriptor _member;
	}
}
