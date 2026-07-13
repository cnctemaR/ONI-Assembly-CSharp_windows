using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.LowLevelPhysics2D
{
	[Serializable]
	public struct PhysicsTransform
	{
		public PhysicsTransform()
		{
			this = PhysicsTransform.identityTransform;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public PhysicsTransform(Vector2 position)
		{
			this.position = position;
			this.rotation = PhysicsRotate.identity;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public PhysicsTransform(Vector2 position, PhysicsRotate rotation)
		{
			this.position = position;
			this.rotation = rotation;
		}

		public readonly bool isValid
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsTransform_IsValid(this);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly void GetPositionAndRotation(out Vector2 position, out PhysicsRotate rotation)
		{
			position = this.position;
			rotation = this.rotation;
		}

		public readonly Vector2 TransformPoint(Vector2 point)
		{
			return PhysicsLowLevelScripting2D.PhysicsTransform_TransformPoint(this, point);
		}

		public readonly Vector2 InverseTransformPoint(Vector2 point)
		{
			return PhysicsLowLevelScripting2D.PhysicsTransform_InverseTransformPoint(this, point);
		}

		public readonly PhysicsTransform MultiplyTransform(PhysicsTransform transform)
		{
			return PhysicsLowLevelScripting2D.PhysicsTransform_MultiplyTransform(this, transform);
		}

		public readonly PhysicsTransform InverseMultiplyTransform(PhysicsTransform transform)
		{
			return PhysicsLowLevelScripting2D.PhysicsTransform_InverseMultiplyTransform(this, transform);
		}

		public static PhysicsTransform identity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return PhysicsTransform.identityTransform;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator PhysicsTransform(Vector2 position)
		{
			return new PhysicsTransform
			{
				position = position,
				rotation = PhysicsRotate.identity
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator PhysicsTransform(PhysicsRotate rotation)
		{
			return new PhysicsTransform
			{
				position = Vector2.zero,
				rotation = rotation
			};
		}

		public override readonly string ToString()
		{
			return string.Format("position={0}, rotation={1}", this.position, this.rotation);
		}

		public Vector2 position;

		public PhysicsRotate rotation;

		private static readonly PhysicsTransform identityTransform = new PhysicsTransform
		{
			position = Vector2.zero,
			rotation = PhysicsRotate.identity
		};
	}
}
