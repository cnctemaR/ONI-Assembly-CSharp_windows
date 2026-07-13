using System;

namespace UnityEngine.LowLevelPhysics2D
{
	[Serializable]
	public struct PhysicsPlane
	{
		public readonly bool isValid
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsPlane_IsValid(this);
			}
		}

		public readonly float GetSeparation(Vector2 point)
		{
			return PhysicsLowLevelScripting2D.PhysicsPlane_GetSeparation(this, point);
		}

		public override readonly string ToString()
		{
			return string.Format("normal={0}, offset={1}", this.normal, this.offset);
		}

		public Vector2 normal;

		public float offset;
	}
}
