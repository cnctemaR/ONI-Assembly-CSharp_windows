using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	public readonly struct ContactPair
	{
		public int ColliderInstanceID
		{
			get
			{
				return this.m_ColliderID;
			}
		}

		public int OtherColliderInstanceID
		{
			get
			{
				return this.m_OtherColliderID;
			}
		}

		public Collider Collider
		{
			get
			{
				return (this.m_ColliderID == 0) ? null : Physics.GetColliderByInstanceID(this.m_ColliderID);
			}
		}

		public Collider OtherCollider
		{
			get
			{
				return (this.m_OtherColliderID == 0) ? null : Physics.GetColliderByInstanceID(this.m_OtherColliderID);
			}
		}

		public int ContactCount
		{
			get
			{
				return (int)this.m_NbPoints;
			}
		}

		public Vector3 ImpulseSum
		{
			get
			{
				return this.m_ImpulseSum;
			}
		}

		public bool IsCollisionEnter
		{
			get
			{
				return (this.m_Events & CollisionPairEventFlags.NotifyTouchFound) > (CollisionPairEventFlags)0;
			}
		}

		public bool IsCollisionExit
		{
			get
			{
				return (this.m_Events & CollisionPairEventFlags.NotifyTouchLost) > (CollisionPairEventFlags)0;
			}
		}

		public bool IsCollisionStay
		{
			get
			{
				return (this.m_Events & CollisionPairEventFlags.NotifyTouchPersists) > (CollisionPairEventFlags)0;
			}
		}

		internal bool HasRemovedCollider
		{
			get
			{
				return (this.m_Flags & CollisionPairFlags.RemovedShape) != (CollisionPairFlags)0 || (this.m_Flags & CollisionPairFlags.RemovedOtherShape) > (CollisionPairFlags)0;
			}
		}

		internal int ExtractContacts(List<ContactPoint> managedContainer, bool flipped)
		{
			return ContactPair.ExtractContacts_Injected(ref this, managedContainer, flipped);
		}

		internal int ExtractContactsArray([Unmarshalled] ContactPoint[] managedContainer, bool flipped)
		{
			return ContactPair.ExtractContactsArray_Injected(ref this, managedContainer, flipped);
		}

		public unsafe void CopyToNativeArray(NativeArray<ContactPairPoint> buffer)
		{
			int num = Mathf.Min(buffer.Length, this.ContactCount);
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

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int ExtractContacts_Injected(ref ContactPair _unity_self, List<ContactPoint> managedContainer, bool flipped);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int ExtractContactsArray_Injected(ref ContactPair _unity_self, ContactPoint[] managedContainer, bool flipped);

		private const uint c_InvalidFaceIndex = 4294967295U;

		internal readonly int m_ColliderID;

		internal readonly int m_OtherColliderID;

		internal readonly IntPtr m_StartPtr;

		internal readonly uint m_NbPoints;

		internal readonly CollisionPairFlags m_Flags;

		internal readonly CollisionPairEventFlags m_Events;

		internal readonly Vector3 m_ImpulseSum;
	}
}
