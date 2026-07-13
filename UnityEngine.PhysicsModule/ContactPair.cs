using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	public readonly struct ContactPair
	{
		[Obsolete("colliderInstanceID is deprecated, use colliderEntityId instead.", false)]
		public int colliderInstanceID
		{
			get
			{
				return this.m_ColliderID;
			}
		}

		[Obsolete("otherColliderInstanceID is deprecated, use otherColliderEntityId instead.", false)]
		public int otherColliderInstanceID
		{
			get
			{
				return this.m_OtherColliderID;
			}
		}

		public EntityId colliderEntityId
		{
			get
			{
				return this.m_ColliderID;
			}
		}

		public EntityId otherColliderEntityId
		{
			get
			{
				return this.m_OtherColliderID;
			}
		}

		public Collider collider
		{
			get
			{
				return (this.m_ColliderID == 0) ? null : Physics.GetColliderByInstanceID(this.m_ColliderID);
			}
		}

		public Collider otherCollider
		{
			get
			{
				return (this.m_OtherColliderID == 0) ? null : Physics.GetColliderByInstanceID(this.m_OtherColliderID);
			}
		}

		public int contactCount
		{
			get
			{
				return (int)this.m_NbPoints;
			}
		}

		public Vector3 impulseSum
		{
			get
			{
				return this.m_ImpulseSum;
			}
		}

		public bool isCollisionEnter
		{
			get
			{
				return (this.m_Events & CollisionPairEventFlags.NotifyTouchFound) > (CollisionPairEventFlags)0;
			}
		}

		public bool isCollisionExit
		{
			get
			{
				return (this.m_Events & CollisionPairEventFlags.NotifyTouchLost) > (CollisionPairEventFlags)0;
			}
		}

		public bool isCollisionStay
		{
			get
			{
				return (this.m_Events & CollisionPairEventFlags.NotifyTouchPersists) > (CollisionPairEventFlags)0;
			}
		}

		internal bool hasRemovedCollider
		{
			get
			{
				return (this.m_Flags & CollisionPairFlags.RemovedShape) != (CollisionPairFlags)0 || (this.m_Flags & CollisionPairFlags.RemovedOtherShape) > (CollisionPairFlags)0;
			}
		}

		internal int ExtractContacts(List<ContactPoint> managedContainer, bool flipped)
		{
			int num = (int)Math.Min((long)managedContainer.Capacity, (long)((ulong)this.m_NbPoints));
			managedContainer.Clear();
			for (int i = 0; i < num; i++)
			{
				readonly ref ContactPairPoint contactPoint = ref this.GetContactPoint(i);
				ContactPoint contactPoint2 = new ContactPoint
				{
					m_Point = contactPoint.position,
					m_Impulse = contactPoint.impulse,
					m_Separation = contactPoint.separation
				};
				if (flipped)
				{
					contactPoint2.m_Normal = -contactPoint.normal;
					contactPoint2.m_ThisColliderEntityId = this.m_OtherColliderID;
					contactPoint2.m_OtherColliderEntityId = this.m_ColliderID;
				}
				else
				{
					contactPoint2.m_Normal = contactPoint.normal;
					contactPoint2.m_ThisColliderEntityId = this.m_ColliderID;
					contactPoint2.m_OtherColliderEntityId = this.m_OtherColliderID;
				}
				managedContainer.Add(contactPoint2);
			}
			return num;
		}

		internal int ExtractContactsArray(ContactPoint[] managedContainer, bool flipped)
		{
			int num = (int)Math.Min((long)managedContainer.Length, (long)((ulong)this.m_NbPoints));
			for (int i = 0; i < num; i++)
			{
				readonly ref ContactPairPoint contactPoint = ref this.GetContactPoint(i);
				ContactPoint contactPoint2 = new ContactPoint
				{
					m_Point = contactPoint.position,
					m_Impulse = contactPoint.impulse,
					m_Separation = contactPoint.separation
				};
				if (flipped)
				{
					contactPoint2.m_Normal = -contactPoint.normal;
					contactPoint2.m_ThisColliderEntityId = this.m_OtherColliderID;
					contactPoint2.m_OtherColliderEntityId = this.m_ColliderID;
				}
				else
				{
					contactPoint2.m_Normal = contactPoint.normal;
					contactPoint2.m_ThisColliderEntityId = this.m_ColliderID;
					contactPoint2.m_OtherColliderEntityId = this.m_OtherColliderID;
				}
				managedContainer[i] = contactPoint2;
			}
			return num;
		}

		public unsafe void CopyToNativeArray(NativeArray<ContactPairPoint> buffer)
		{
			int num = Mathf.Min(buffer.Length, this.contactCount);
			for (int i = 0; i < num; i++)
			{
				buffer[i] = *this.GetContactPoint(i);
			}
		}

		public readonly ref ContactPairPoint GetContactPoint(int index)
		{
			return this.GetContactPoint_Internal(index);
		}

		public unsafe uint GetContactPointFaceIndex(int contactIndex)
		{
			uint internalFaceIndex = this.GetContactPoint_Internal(contactIndex)->m_InternalFaceIndex0;
			uint internalFaceIndex2 = this.GetContactPoint_Internal(contactIndex)->m_InternalFaceIndex1;
			bool flag = internalFaceIndex != uint.MaxValue;
			uint num;
			if (flag)
			{
				num = Physics.TranslateTriangleIndexFromID(this.m_ColliderID, internalFaceIndex);
			}
			else
			{
				bool flag2 = internalFaceIndex2 != uint.MaxValue;
				if (flag2)
				{
					num = Physics.TranslateTriangleIndexFromID(this.m_OtherColliderID, internalFaceIndex2);
				}
				else
				{
					num = uint.MaxValue;
				}
			}
			return num;
		}

		internal unsafe ContactPairPoint* GetContactPoint_Internal(int index)
		{
			bool flag = (long)index >= (long)((ulong)this.m_NbPoints);
			if (flag)
			{
				throw new IndexOutOfRangeException("Invalid ContactPairPoint index. Index should be greater than 0 and less than ContactPair.ContactCount");
			}
			return this.m_StartPtr.ToInt64() / (long)sizeof(ContactPairPoint) + index * sizeof(ContactPairPoint);
		}

		[Obsolete("Please use ContactPair.colliderInstanceID instead. (UnityUpgradable) -> colliderInstanceID", false)]
		public int ColliderInstanceID
		{
			get
			{
				return this.colliderInstanceID;
			}
		}

		[Obsolete("Please use ContactPair.otherColliderInstanceID instead. (UnityUpgradable) -> otherColliderInstanceID", false)]
		public int OtherColliderInstanceID
		{
			get
			{
				return this.otherColliderInstanceID;
			}
		}

		[Obsolete("Please use ContactPair.collider instead. (UnityUpgradable) -> collider", false)]
		public Collider Collider
		{
			get
			{
				return this.collider;
			}
		}

		[Obsolete("Please use ContactPair.otherCollider instead. (UnityUpgradable) -> otherCollider", false)]
		public Collider OtherCollider
		{
			get
			{
				return this.otherCollider;
			}
		}

		[Obsolete("Please use ContactPair.contactCount instead. (UnityUpgradable) -> contactCount", false)]
		public int ContactCount
		{
			get
			{
				return this.contactCount;
			}
		}

		[Obsolete("Please use ContactPair.impulseSum instead. (UnityUpgradable) -> impulseSum", false)]
		public Vector3 ImpulseSum
		{
			get
			{
				return this.impulseSum;
			}
		}

		[Obsolete("Please use ContactPair.isCollisionEnter instead. (UnityUpgradable) -> isCollisionEnter", false)]
		public bool IsCollisionEnter
		{
			get
			{
				return this.isCollisionEnter;
			}
		}

		[Obsolete("Please use ContactPair.isCollisionExit instead. (UnityUpgradable) -> isCollisionExit", false)]
		public bool IsCollisionExit
		{
			get
			{
				return this.isCollisionExit;
			}
		}

		[Obsolete("Please use ContactPair.isCollisionStay instead. (UnityUpgradable) -> isCollisionStay", false)]
		public bool IsCollisionStay
		{
			get
			{
				return this.isCollisionStay;
			}
		}

		private const uint c_InvalidFaceIndex = 4294967295U;

		internal readonly EntityId m_ColliderID;

		internal readonly EntityId m_OtherColliderID;

		internal readonly IntPtr m_StartPtr;

		internal readonly uint m_NbPoints;

		internal readonly CollisionPairFlags m_Flags;

		internal readonly CollisionPairEventFlags m_Events;

		internal readonly Vector3 m_ImpulseSum;
	}
}
