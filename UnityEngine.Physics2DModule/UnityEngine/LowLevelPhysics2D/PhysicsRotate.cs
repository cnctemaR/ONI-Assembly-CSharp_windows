using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.LowLevelPhysics2D
{
	[Serializable]
	public struct PhysicsRotate : ISerializationCallbackReceiver
	{
		public readonly float cos
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.direction.x;
			}
		}

		public readonly float sin
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.direction.y;
			}
		}

		public PhysicsRotate()
		{
			this.direction = Vector2.right;
		}

		public PhysicsRotate(Vector2 direction)
		{
			this = PhysicsLowLevelScripting2D.PhysicsRotate_CreateDirection(in direction);
		}

		public PhysicsRotate(float angle)
		{
			this = PhysicsLowLevelScripting2D.PhysicsRotate_CreateAngle(angle);
		}

		public PhysicsRotate(Quaternion rotation, PhysicsWorld.TransformPlane transformPlane)
		{
			this = new PhysicsRotate(PhysicsMath.ToRotation2D(rotation, transformPlane));
		}

		public readonly float GetRelativeAngle(PhysicsRotate rotation)
		{
			return PhysicsLowLevelScripting2D.PhysicsRotate_GetRelativeAngle(this, rotation);
		}

		public static float UnwindAngle(float angle)
		{
			return PhysicsLowLevelScripting2D.PhysicsRotate_UnwindAngle(angle);
		}

		public readonly PhysicsRotate IntegrateRotation(float deltaAngle)
		{
			return PhysicsLowLevelScripting2D.PhysicsRotate_IntegrateRotation(this, deltaAngle);
		}

		public readonly PhysicsRotate LerpRotation(PhysicsRotate rotation, float interval)
		{
			return PhysicsLowLevelScripting2D.PhysicsRotate_LerpRotation(this, rotation, interval);
		}

		public static PhysicsRotate LerpRotation(PhysicsRotate rotationA, PhysicsRotate rotationB, float interval)
		{
			return PhysicsLowLevelScripting2D.PhysicsRotate_LerpRotation(rotationA, rotationB, interval);
		}

		public readonly float AngularVelocity(PhysicsRotate rotation, float deltaTime)
		{
			return PhysicsLowLevelScripting2D.PhysicsRotate_AngularVelocity(this, rotation, deltaTime);
		}

		public static float AngularVelocity(PhysicsRotate rotationA, PhysicsRotate rotationB, float deltaTime)
		{
			return PhysicsLowLevelScripting2D.PhysicsRotate_AngularVelocity(rotationA, rotationB, deltaTime);
		}

		public readonly PhysicsRotate MultiplyRotation(PhysicsRotate rotation)
		{
			return PhysicsLowLevelScripting2D.PhysicsRotate_MultiplyRotation(this, rotation);
		}

		public readonly PhysicsRotate InverseMultiplyRotation(PhysicsRotate rotation)
		{
			return PhysicsLowLevelScripting2D.PhysicsRotate_InverseMultiplyRotation(this, rotation);
		}

		public readonly Vector2 RotateVector(Vector2 vector)
		{
			return PhysicsLowLevelScripting2D.PhysicsRotate_RotateVector(this, vector);
		}

		public readonly Vector2 InverseRotateVector(Vector2 vector)
		{
			return PhysicsLowLevelScripting2D.PhysicsRotate_InverseRotateVector(this, vector);
		}

		public readonly PhysicsRotate Rotate(float deltaAngle)
		{
			return PhysicsLowLevelScripting2D.PhysicsRotate_Rotate(this, deltaAngle);
		}

		public readonly Matrix4x4 GetMatrix(PhysicsWorld.TransformPlane transformPlane)
		{
			return Matrix4x4.Rotate(PhysicsMath.ToRotationFast3D(this.angle, transformPlane));
		}

		public readonly PhysicsRotate Normalized()
		{
			return PhysicsLowLevelScripting2D.PhysicsRotate_CreateDirection(in this.direction);
		}

		public readonly bool isNormalized
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsRotate_IsNormalized(this);
			}
		}

		public readonly bool isValid
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsRotate_IsValid(this);
			}
		}

		public readonly float angle
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsRotate_GetAngle(this);
			}
		}

		public static PhysicsRotate identity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return PhysicsRotate.identityRotation;
			}
		}

		public static PhysicsRotate right
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return PhysicsRotate.identityRotation;
			}
		}

		public static PhysicsRotate left
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return PhysicsRotate.leftRotation;
			}
		}

		public static PhysicsRotate up
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return PhysicsRotate.upRotation;
			}
		}

		public static PhysicsRotate down
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return PhysicsRotate.downRotation;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator PhysicsRotate(Vector2 direction)
		{
			return new PhysicsRotate(direction);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector2(PhysicsRotate rotate2)
		{
			return rotate2.direction;
		}

		public void OnBeforeSerialize()
		{
			bool flag = !this.isValid;
			if (flag)
			{
				this = PhysicsRotate.identity;
			}
		}

		public void OnAfterDeserialize()
		{
			bool flag = !this.isValid;
			if (flag)
			{
				this = PhysicsRotate.identity;
			}
		}

		public override readonly string ToString()
		{
			return string.Format("angle={0} (rad), cos={1}, sin={2}", this.angle, this.cos, this.sin);
		}

		public Vector2 direction;

		private static readonly PhysicsRotate identityRotation = new PhysicsRotate(Vector2.right);

		private static readonly PhysicsRotate leftRotation = new PhysicsRotate(Vector2.left);

		private static readonly PhysicsRotate upRotation = new PhysicsRotate(Vector2.up);

		private static readonly PhysicsRotate downRotation = new PhysicsRotate(Vector2.down);
	}
}
