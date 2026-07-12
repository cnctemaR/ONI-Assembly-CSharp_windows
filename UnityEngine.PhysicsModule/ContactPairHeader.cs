using System;

namespace UnityEngine
{
	public readonly struct ContactPairHeader
	{
		public int BodyInstanceID
		{
			get
			{
				return this.m_BodyID;
			}
		}

		public int OtherBodyInstanceID
		{
			get
			{
				return this.m_OtherBodyID;
			}
		}

		public Component Body
		{
			get
			{
				return Physics.GetBodyByInstanceID(this.m_BodyID);
			}
		}

		public Component OtherBody
		{
			get
			{
				return Physics.GetBodyByInstanceID(this.m_OtherBodyID);
			}
		}

		public int PairCount
		{
			get
			{
				return (int)this.m_NbPairs;
			}
		}

		internal bool HasRemovedBody
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

		internal readonly int m_BodyID;

		internal readonly int m_OtherBodyID;

		internal readonly IntPtr m_StartPtr;

		internal readonly uint m_NbPairs;

		internal readonly CollisionPairHeaderFlags m_Flags;

		internal readonly Vector3 m_RelativeVelocity;
	}
}
