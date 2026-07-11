using System;
using System.Collections;

namespace System.ComponentModel.Design.Serialization
{
	public abstract class MemberRelationshipService
	{
		protected MemberRelationshipService()
		{
			this._relations = new Hashtable();
		}

		public abstract bool SupportsRelationship(MemberRelationship source, MemberRelationship relationship);

		protected virtual MemberRelationship GetRelationship(MemberRelationship source)
		{
			if (source.IsEmpty)
			{
				throw new ArgumentNullException("source");
			}
			MemberRelationshipService.MemberRelationshipWeakEntry memberRelationshipWeakEntry = this._relations[new MemberRelationshipService.MemberRelationshipWeakEntry(source)] as MemberRelationshipService.MemberRelationshipWeakEntry;
			if (memberRelationshipWeakEntry != null)
			{
				return new MemberRelationship(memberRelationshipWeakEntry.Owner, memberRelationshipWeakEntry.Member);
			}
			return MemberRelationship.Empty;
		}

		protected virtual void SetRelationship(MemberRelationship source, MemberRelationship relationship)
		{
			if (source.IsEmpty)
			{
				throw new ArgumentNullException("source");
			}
			if (!relationship.IsEmpty && !this.SupportsRelationship(source, relationship))
			{
				throw new ArgumentException("Relationship not supported.");
			}
			this._relations[new MemberRelationshipService.MemberRelationshipWeakEntry(source)] = new MemberRelationshipService.MemberRelationshipWeakEntry(relationship);
		}

		public MemberRelationship this[object owner, MemberDescriptor member]
		{
			get
			{
				return this.GetRelationship(new MemberRelationship(owner, member));
			}
			set
			{
				this.SetRelationship(new MemberRelationship(owner, member), value);
			}
		}

		public MemberRelationship this[MemberRelationship source]
		{
			get
			{
				return this.GetRelationship(source);
			}
			set
			{
				this.SetRelationship(source, value);
			}
		}

		private Hashtable _relations;

		private class MemberRelationshipWeakEntry
		{
			public MemberRelationshipWeakEntry(MemberRelationship relation)
			{
				this._ownerWeakRef = new WeakReference(relation.Owner);
				this._member = relation.Member;
			}

			public object Owner
			{
				get
				{
					if (this._ownerWeakRef.IsAlive)
					{
						return this._ownerWeakRef.Target;
					}
					return null;
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
				if (this.Owner != null && this._member != null)
				{
					return this._member.GetHashCode() ^ this._ownerWeakRef.Target.GetHashCode();
				}
				return base.GetHashCode();
			}

			public override bool Equals(object o)
			{
				return o is MemberRelationshipService.MemberRelationshipWeakEntry && (MemberRelationshipService.MemberRelationshipWeakEntry)o == this;
			}

			public static bool operator ==(MemberRelationshipService.MemberRelationshipWeakEntry left, MemberRelationshipService.MemberRelationshipWeakEntry right)
			{
				return left.Owner == right.Owner && left.Member == right.Member;
			}

			public static bool operator !=(MemberRelationshipService.MemberRelationshipWeakEntry left, MemberRelationshipService.MemberRelationshipWeakEntry right)
			{
				return !(left == right);
			}

			private WeakReference _ownerWeakRef;

			private MemberDescriptor _member;
		}
	}
}
