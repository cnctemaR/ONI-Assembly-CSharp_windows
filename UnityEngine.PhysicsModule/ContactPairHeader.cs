using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	public readonly struct ContactPairHeader
	{
		[Obsolete("bodyInstanceID is deprecated, use bodyEntityId instead.", false)]
		public int bodyInstanceID
		{
			get
			{
				return this.m_BodyID;
			}
		}

		[Obsolete("otherBodyInstanceID is deprecated, use otherBodyEntityId instead.", false)]
		public int otherBodyInstanceID
		{
			get
			{
				return this.m_OtherBodyID;
			}
		}

		public EntityId bodyEntityId
		{
			get
			{
				return this.m_BodyID;
			}
		}

		public EntityId otherBodyEntityId
		{
			get
			{
				return this.m_OtherBodyID;
			}
		}

		public Component body
		{
			get
			{
				return Physics.GetBodyByInstanceID(this.m_BodyID);
			}
		}

		public Component otherBody
		{
			get
			{
				return Physics.GetBodyByInstanceID(this.m_OtherBodyID);
			}
		}

		public int pairCount
		{
			get
			{
				return (int)this.m_NbPairs;
			}
		}

		internal bool hasRemovedBody
		{
			get
			{
				return (this.m_Flags & CollisionPairHeaderFlags.RemovedActor) != (CollisionPairHeaderFlags)0 || (this.m_Flags & CollisionPairHeaderFlags.RemovedOtherActor) > (CollisionPairHeaderFlags)0;
			}
		}

		public readonly ref ContactPair GetContactPair(int index)
		{
			return this.GetContactPair_Internal(index);
		}

		internal unsafe ContactPair* GetContactPair_Internal(int index)
		{
			bool flag = (long)index >= (long)((ulong)this.m_NbPairs);
			if (flag)
			{
				throw new IndexOutOfRangeException("Invalid ContactPair index. Index should be greater than 0 and less than ContactPairHeader.PairCount");
			}
			return this.m_StartPtr.ToInt64() / (long)sizeof(ContactPair) + index * sizeof(ContactPair);
		}

		[Obsolete("Please use ContactPairHeader.bodyInstanceID instead. (UnityUpgradable) -> bodyInstanceID", false)]
		public int BodyInstanceID
		{
			get
			{
				return this.bodyInstanceID;
			}
		}

		[Obsolete("Please use ContactPairHeader.otherBodyInstanceID instead. (UnityUpgradable) -> otherBodyInstanceID", false)]
		public int OtherBodyInstanceID
		{
			get
			{
				return this.otherBodyInstanceID;
			}
		}

		[Obsolete("Please use ContactPairHeader.body instead. (UnityUpgradable) -> body", false)]
		public Component Body
		{
			get
			{
				return this.body;
			}
		}

		[Obsolete("Please use ContactPairHeader.otherBody instead. (UnityUpgradable) -> otherBody", false)]
		public Component OtherBody
		{
			get
			{
				return this.otherBody;
			}
		}

		[Obsolete("Please use ContactPairHeader.pairCount instead. (UnityUpgradable) -> pairCount", false)]
		public int PairCount
		{
			get
			{
				return this.pairCount;
			}
		}

		internal readonly EntityId m_BodyID;

		internal readonly EntityId m_OtherBodyID;

		internal readonly IntPtr m_StartPtr;

		internal readonly uint m_NbPairs;

		internal readonly CollisionPairHeaderFlags m_Flags;

		internal readonly Vector3 m_RelativeVelocity;
	}
}
