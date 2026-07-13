using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.LowLevelPhysics2D
{
	public readonly struct PhysicsMath
	{
		public static float PI
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsMath_PI();
			}
		}

		public static float TAU
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsMath_TAU();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float ToDegrees(float radians)
		{
			return PhysicsLowLevelScripting2D.PhysicsMath_ToDegrees(radians);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float ToRadians(float degrees)
		{
			return PhysicsLowLevelScripting2D.PhysicsMath_ToRadians(degrees);
		}

		public static float Atan2(float y, float x)
		{
			return PhysicsLowLevelScripting2D.PhysicsMath_Atan2(y, x);
		}

		public static void CosSin(float angle, out float cosine, out float sine)
		{
			PhysicsLowLevelScripting2D.PhysicsMath_CosSin(angle, out cosine, out sine);
		}

		public static Vector2 CosSin(float angle)
		{
			float num;
			float num2;
			PhysicsMath.CosSin(angle, out num, out num2);
			return new Vector2(num, num2);
		}

		public static float SpringDamper(float frequency, float damping, float translation, float speed, float deltaTime)
		{
			return PhysicsLowLevelScripting2D.PhysicsMath_SpringDamper(frequency, damping, translation, speed, deltaTime);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float MinAbsComponent(Vector2 vector)
		{
			return Math.Min(Math.Abs(vector.x), Math.Abs(vector.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float MinAbsComponent(Vector3 vector)
		{
			return Math.Min(Math.Min(Math.Abs(vector.x), Math.Abs(vector.y)), Math.Abs(vector.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float MaxAbsComponent(Vector2 vector)
		{
			return Math.Max(Math.Abs(vector.x), Math.Abs(vector.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float MaxAbsComponent(Vector3 vector)
		{
			return Math.Max(Math.Max(Math.Abs(vector.x), Math.Abs(vector.y)), Math.Abs(vector.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 GetTranslationAxes(PhysicsWorld.TransformPlane transformPlane = PhysicsWorld.TransformPlane.XY)
		{
			if (!true)
			{
			}
			Vector3 vector;
			switch (transformPlane)
			{
			case PhysicsWorld.TransformPlane.XY:
				vector = Vector3.right + Vector3.up;
				break;
			case PhysicsWorld.TransformPlane.XZ:
				vector = Vector3.right + Vector3.forward;
				break;
			case PhysicsWorld.TransformPlane.ZY:
				vector = Vector3.up + Vector3.forward;
				break;
			default:
				throw new InvalidOperationException("Invalid Transform Plane.");
			}
			if (!true)
			{
			}
			return vector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 GetTranslationIgnoredAxes(PhysicsWorld.TransformPlane transformPlane = PhysicsWorld.TransformPlane.XY)
		{
			if (!true)
			{
			}
			Vector3 vector;
			switch (transformPlane)
			{
			case PhysicsWorld.TransformPlane.XY:
				vector = Vector3.forward;
				break;
			case PhysicsWorld.TransformPlane.XZ:
				vector = Vector3.up;
				break;
			case PhysicsWorld.TransformPlane.ZY:
				vector = Vector3.right;
				break;
			default:
				throw new InvalidOperationException("Invalid Transform Plane.");
			}
			if (!true)
			{
			}
			return vector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float GetTranslationIgnoredAxis(Vector3 position, PhysicsWorld.TransformPlane transformPlane = PhysicsWorld.TransformPlane.XY)
		{
			if (!true)
			{
			}
			float num;
			switch (transformPlane)
			{
			case PhysicsWorld.TransformPlane.XY:
				num = position.z;
				break;
			case PhysicsWorld.TransformPlane.XZ:
				num = position.y;
				break;
			case PhysicsWorld.TransformPlane.ZY:
				num = position.x;
				break;
			default:
				throw new InvalidOperationException("Invalid Transform Plane.");
			}
			if (!true)
			{
			}
			return num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 GetRotationAxes(PhysicsWorld.TransformPlane transformPlane = PhysicsWorld.TransformPlane.XY)
		{
			if (!true)
			{
			}
			Vector3 vector;
			switch (transformPlane)
			{
			case PhysicsWorld.TransformPlane.XY:
				vector = Vector3.forward;
				break;
			case PhysicsWorld.TransformPlane.XZ:
				vector = Vector3.up;
				break;
			case PhysicsWorld.TransformPlane.ZY:
				vector = Vector3.right;
				break;
			default:
				throw new InvalidOperationException("Invalid Transform Plane.");
			}
			if (!true)
			{
			}
			return vector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 GetRotationIgnoredAxes(PhysicsWorld.TransformPlane transformPlane = PhysicsWorld.TransformPlane.XY)
		{
			if (!true)
			{
			}
			Vector3 vector;
			switch (transformPlane)
			{
			case PhysicsWorld.TransformPlane.XY:
				vector = Vector3.right + Vector3.up;
				break;
			case PhysicsWorld.TransformPlane.XZ:
				vector = Vector3.right + Vector3.forward;
				break;
			case PhysicsWorld.TransformPlane.ZY:
				vector = Vector3.up + Vector3.forward;
				break;
			default:
				throw new InvalidOperationException("Invalid Transform Plane.");
			}
			if (!true)
			{
			}
			return vector;
		}

		public static Matrix4x4 GetRelativeMatrix(Transform transformFrom, Transform transformTo, PhysicsWorld.TransformPlane transformPlane = PhysicsWorld.TransformPlane.XY, bool useScale = true)
		{
			bool flag = transformFrom == transformTo;
			Matrix4x4 matrix4x;
			if (flag)
			{
				if (useScale)
				{
					matrix4x = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, transformTo.lossyScale);
				}
				else
				{
					matrix4x = Matrix4x4.identity;
				}
			}
			else
			{
				Quaternion quaternion = Quaternion.Inverse(PhysicsMath.ToRotationFast3D(PhysicsMath.ToRotation2D(transformFrom.rotation, transformPlane), transformPlane));
				Vector3 vector = quaternion * -PhysicsMath.Swizzle(transformFrom.position, transformPlane);
				Matrix4x4 matrix4x2 = Matrix4x4.TRS(vector, quaternion, Vector3.one);
				if (useScale)
				{
					matrix4x = matrix4x2 * PhysicsMath.Swizzle(transformTo.localToWorldMatrix, transformPlane);
				}
				else
				{
					matrix4x = matrix4x2 * PhysicsMath.Swizzle(transformTo.localToWorldMatrix * Matrix4x4.Scale(transformTo.localScale).inverse, transformPlane);
				}
			}
			return matrix4x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 Swizzle(Vector3 position, PhysicsWorld.TransformPlane transformPlane = PhysicsWorld.TransformPlane.XY)
		{
			if (!true)
			{
			}
			Vector3 vector;
			switch (transformPlane)
			{
			case PhysicsWorld.TransformPlane.XY:
				vector = position;
				break;
			case PhysicsWorld.TransformPlane.XZ:
				vector = new Vector3(position.x, position.z, position.y);
				break;
			case PhysicsWorld.TransformPlane.ZY:
				vector = new Vector3(position.z, position.y, position.x);
				break;
			default:
				throw new InvalidOperationException("Invalid Transform Plane.");
			}
			if (!true)
			{
			}
			return vector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 Swizzle(Vector4 position, PhysicsWorld.TransformPlane transformPlane = PhysicsWorld.TransformPlane.XY)
		{
			Vector3 vector = PhysicsMath.Swizzle(new Vector3(position.x, position.y, position.z), transformPlane);
			return new Vector4(vector.x, vector.y, vector.z, position.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Matrix4x4 Swizzle(Matrix4x4 matrix, PhysicsWorld.TransformPlane transformPlane = PhysicsWorld.TransformPlane.XY)
		{
			matrix.SetColumn(3, PhysicsMath.Swizzle(matrix.GetColumn(3), transformPlane));
			return matrix;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 ToPosition3D(Vector2 position, Vector3 reference, PhysicsWorld.TransformPlane transformPlane = PhysicsWorld.TransformPlane.XY)
		{
			if (!true)
			{
			}
			Vector3 vector;
			switch (transformPlane)
			{
			case PhysicsWorld.TransformPlane.XY:
				vector = new Vector3(position.x, position.y, reference.z);
				break;
			case PhysicsWorld.TransformPlane.XZ:
				vector = new Vector3(position.x, reference.y, position.y);
				break;
			case PhysicsWorld.TransformPlane.ZY:
				vector = new Vector3(reference.x, position.y, position.x);
				break;
			default:
				throw new InvalidOperationException("Invalid Transform Plane.");
			}
			if (!true)
			{
			}
			return vector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 ToPosition2D(Vector3 position, PhysicsWorld.TransformPlane transformPlane = PhysicsWorld.TransformPlane.XY)
		{
			if (!true)
			{
			}
			Vector2 vector;
			switch (transformPlane)
			{
			case PhysicsWorld.TransformPlane.XY:
				vector = position;
				break;
			case PhysicsWorld.TransformPlane.XZ:
				vector = new Vector2(position.x, position.z);
				break;
			case PhysicsWorld.TransformPlane.ZY:
				vector = new Vector2(position.z, position.y);
				break;
			default:
				throw new InvalidOperationException("Invalid Transform Plane.");
			}
			if (!true)
			{
			}
			return vector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float ToRotation2D(Quaternion quaternion, PhysicsWorld.TransformPlane transformPlane = PhysicsWorld.TransformPlane.XY)
		{
			bool flag = quaternion.w < 0f;
			if (flag)
			{
				quaternion = new Quaternion(-quaternion.x, -quaternion.y, -quaternion.z, -quaternion.w);
			}
			if (!true)
			{
			}
			float num;
			switch (transformPlane)
			{
			case PhysicsWorld.TransformPlane.XY:
				num = 2f * PhysicsMath.Atan2(quaternion.z, quaternion.w);
				break;
			case PhysicsWorld.TransformPlane.XZ:
				num = -2f * PhysicsMath.Atan2(quaternion.y, quaternion.w);
				break;
			case PhysicsWorld.TransformPlane.ZY:
				num = -2f * PhysicsMath.Atan2(quaternion.x, quaternion.w);
				break;
			default:
				throw new InvalidOperationException("Invalid Transform Plane.");
			}
			if (!true)
			{
			}
			return num;
		}

		public static PhysicsTransform ToPhysicsTransform(Transform transform, PhysicsWorld.TransformPlane transformPlane = PhysicsWorld.TransformPlane.XY)
		{
			return new PhysicsTransform
			{
				position = PhysicsMath.ToPosition2D(transform.position, transformPlane),
				rotation = new PhysicsRotate(PhysicsMath.ToRotation2D(transform.rotation, transformPlane))
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion AngularVelocityToQuaternion(float angularVelocity, float deltaTime, PhysicsWorld.TransformPlane transformPlane)
		{
			float num = Mathf.Abs(angularVelocity);
			bool flag = num < 1E-05f;
			Quaternion quaternion;
			if (flag)
			{
				quaternion = Quaternion.identity;
			}
			else
			{
				PhysicsRotate physicsRotate = new PhysicsRotate(num * deltaTime * 0.5f);
				Vector3 vector = PhysicsMath.Swizzle(new Vector3(0f, 0f, angularVelocity * (physicsRotate.sin / num)), transformPlane);
				quaternion = new Quaternion(vector.x, vector.y, vector.z, physicsRotate.cos).normalized;
			}
			return quaternion;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion ToRotationFast3D(float angle, PhysicsWorld.TransformPlane transformPlane = PhysicsWorld.TransformPlane.XY)
		{
			Quaternion quaternion;
			switch (transformPlane)
			{
			case PhysicsWorld.TransformPlane.XY:
			{
				PhysicsRotate physicsRotate = new PhysicsRotate(angle * 0.5f);
				quaternion = new Quaternion(0f, 0f, physicsRotate.sin, physicsRotate.cos);
				break;
			}
			case PhysicsWorld.TransformPlane.XZ:
			{
				PhysicsRotate physicsRotate2 = new PhysicsRotate(angle * -0.5f);
				quaternion = new Quaternion(0f, physicsRotate2.sin, 0f, physicsRotate2.cos);
				break;
			}
			case PhysicsWorld.TransformPlane.ZY:
			{
				PhysicsRotate physicsRotate3 = new PhysicsRotate(angle * -0.5f);
				quaternion = new Quaternion(physicsRotate3.sin, 0f, 0f, physicsRotate3.cos);
				break;
			}
			default:
				throw new InvalidOperationException("Invalid Transform Plane.");
			}
			return quaternion;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion ToRotationSlow3D(float angle, Quaternion reference, PhysicsWorld.TransformPlane transformPlane = PhysicsWorld.TransformPlane.XY)
		{
			bool flag = reference.w < 0f;
			if (flag)
			{
				reference = new Quaternion(-reference.x, -reference.y, -reference.z, -reference.w);
			}
			Quaternion quaternion = PhysicsMath.ToRotationFast3D(angle, transformPlane);
			Quaternion quaternion2 = Quaternion.Inverse(PhysicsMath.ToRotationFast3D(PhysicsMath.ToRotation2D(reference, transformPlane), transformPlane));
			return quaternion * quaternion2 * reference;
		}
	}
}
