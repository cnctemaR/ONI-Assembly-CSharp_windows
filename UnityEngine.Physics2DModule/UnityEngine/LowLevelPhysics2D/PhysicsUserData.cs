using System;

namespace UnityEngine.LowLevelPhysics2D
{
	[Serializable]
	public struct PhysicsUserData
	{
		public Object objectValue
		{
			readonly get
			{
				return PhysicsLowLevelScripting2D.PhysicsUserData_GetObject(this.m_EntityId);
			}
			set
			{
				this.m_EntityId = ((value != null) ? value.GetEntityId() : EntityId.None);
			}
		}

		public PhysicsMask physicsMaskValue
		{
			readonly get
			{
				return this.m_PhysicsMask;
			}
			set
			{
				this.m_PhysicsMask = value;
			}
		}

		public float floatValue
		{
			readonly get
			{
				return this.m_Float;
			}
			set
			{
				this.m_Float = value;
			}
		}

		public int intValue
		{
			readonly get
			{
				return this.m_Int;
			}
			set
			{
				this.m_Int = value;
			}
		}

		public ulong int64Value
		{
			readonly get
			{
				return this.m_Int64;
			}
			set
			{
				this.m_Int64 = value;
			}
		}

		public bool boolValue
		{
			readonly get
			{
				return this.m_Bool;
			}
			set
			{
				this.m_Bool = value;
			}
		}

		public override readonly string ToString()
		{
			return string.Format("object={0}, physicsMask={1}, float={2}, int={3}, int64={4}, bool={5}", new object[] { this.objectValue, this.physicsMaskValue, this.floatValue, this.intValue, this.int64Value, this.boolValue });
		}

		[SerializeField]
		internal EntityId m_EntityId;

		[SerializeField]
		internal PhysicsMask m_PhysicsMask;

		[SerializeField]
		internal float m_Float;

		[SerializeField]
		internal int m_Int;

		[SerializeField]
		internal ulong m_Int64;

		[SerializeField]
		internal bool m_Bool;
	}
}
