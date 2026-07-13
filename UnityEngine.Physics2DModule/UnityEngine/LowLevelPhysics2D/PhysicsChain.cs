using System;
using System.ComponentModel;
using Unity.Collections;

namespace UnityEngine.LowLevelPhysics2D
{
	public readonly struct PhysicsChain : IEquatable<PhysicsChain>
	{
		public override string ToString()
		{
			return this.isValid ? string.Format("index={0}, world={1}, generation={2}", this.m_Index1, this.m_World0, this.m_Generation) : "<INVALID>";
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public bool Equals(PhysicsChain other)
		{
			return this.m_Index1 == other.m_Index1 && this.m_World0 == other.m_World0 && this.m_Generation == other.m_Generation;
		}

		public static bool operator ==(PhysicsChain lhs, PhysicsChain rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(PhysicsChain lhs, PhysicsChain rhs)
		{
			return !(lhs == rhs);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine<int, ushort, ushort>(this.m_Index1, this.m_World0, this.m_Generation);
		}

		public static PhysicsChain Create(PhysicsBody body, ChainGeometry geometry, PhysicsChainDefinition definition)
		{
			return PhysicsLowLevelScripting2D.PhysicsChain_Create(body, geometry, definition);
		}

		public bool Destroy(int ownerKey = 0)
		{
			return PhysicsLowLevelScripting2D.PhysicsChain_Destroy(this, ownerKey);
		}

		public bool isValid
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsChain_IsValid(this);
			}
		}

		public PhysicsWorld world
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsChain_GetWorld(this);
			}
		}

		public PhysicsBody body
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsChain_GetBody(this);
			}
		}

		public PhysicsAABB aabb
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsChain_CalculateAABB(this);
			}
		}

		public Vector2 ClosestPoint(Vector2 point, out PhysicsShape chainSegmentShape)
		{
			return PhysicsLowLevelScripting2D.PhysicsChain_ClosestPoint(this, point, out chainSegmentShape);
		}

		public PhysicsQuery.CastResult CastRay(PhysicsQuery.CastRayInput castRayInput, out PhysicsShape chainSegmentShape)
		{
			return PhysicsLowLevelScripting2D.PhysicsChain_CastRay(this, castRayInput, out chainSegmentShape);
		}

		public PhysicsQuery.CastResult CastShape(PhysicsQuery.CastShapeInput input, out PhysicsShape chainSegmentShape)
		{
			return PhysicsLowLevelScripting2D.PhysicsChain_CastShape(this, input, out chainSegmentShape);
		}

		public float friction
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsChain_GetFriction(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsChain_SetFriction(this, value);
			}
		}

		public float bounciness
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsChain_GetBounciness(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsChain_SetBounciness(this, value);
			}
		}

		public PhysicsShape.SurfaceMaterial.MixingMode frictionMixing
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsChain_GetFrictionMixing(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsChain_SetFrictionMixing(this, value);
			}
		}

		public PhysicsShape.SurfaceMaterial.MixingMode bouncinessMixing
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsChain_GetBouncinessMixing(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsChain_SetBouncinessMixing(this, value);
			}
		}

		public int segmentCount
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsChain_GetSegmentCount(this);
			}
		}

		public NativeArray<PhysicsShape> GetSegments(Allocator allocator = Allocator.Temp)
		{
			return PhysicsLowLevelScripting2D.PhysicsChain_GetSegments(this, allocator).ToNativeArray<PhysicsShape>();
		}

		public int GetSegmentIndex(PhysicsShape chainSegmentShape)
		{
			bool flag = !chainSegmentShape.isChainSegment || chainSegmentShape.chain != this;
			if (flag)
			{
				throw new ArgumentException("The specified chain segment shape is not a chain segment or does not belong to the current chain shape.", "chainSegmentShape");
			}
			return PhysicsLowLevelScripting2D.PhysicsChain_GetSegmentIndex(this, chainSegmentShape);
		}

		public int SetOwner(Object owner)
		{
			return PhysicsLowLevelScripting2D.PhysicsChain_SetOwner(this, owner);
		}

		public Object GetOwner()
		{
			return PhysicsLowLevelScripting2D.PhysicsChain_GetOwner(this);
		}

		public bool isOwned
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsChain_IsOwned(this);
			}
		}

		public object callbackTarget
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsChain_GetCallbackTarget(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsChain_SetCallbackTarget(this, value);
			}
		}

		public PhysicsUserData userData
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsChain_GetUserData(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsChain_SetUserData(this, value);
			}
		}

		[Obsolete("PhysicsChain.frictionCombine has been deprecated. Please use PhysicsChain.frictionMixing instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public PhysicsMaterialCombine2D frictionCombine
		{
			get
			{
				return (PhysicsMaterialCombine2D)this.frictionMixing;
			}
			set
			{
				this.frictionMixing = (PhysicsShape.SurfaceMaterial.MixingMode)value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("PhysicsChain.bouncinessCombine has been deprecated. Please use PhysicsChain.bouncinessMixing instead.", false)]
		public PhysicsMaterialCombine2D bouncinessCombine
		{
			get
			{
				return (PhysicsMaterialCombine2D)this.bouncinessMixing;
			}
			set
			{
				this.bouncinessMixing = (PhysicsShape.SurfaceMaterial.MixingMode)value;
			}
		}

		private readonly int m_Index1;

		private readonly ushort m_World0;

		private readonly ushort m_Generation;
	}
}
