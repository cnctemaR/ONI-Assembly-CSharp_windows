using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct RigidTransform
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RigidTransform(quaternion rotation, float3 translation)
		{
			this.rot = rotation;
			this.pos = translation;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RigidTransform(float3x3 rotation, float3 translation)
		{
			this.rot = new quaternion(rotation);
			this.pos = translation;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RigidTransform(float4x4 transform)
		{
			this.rot = new quaternion(transform);
			this.pos = transform.c3.xyz;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static RigidTransform AxisAngle(float3 axis, float angle)
		{
			return new RigidTransform(quaternion.AxisAngle(axis, angle), float3.zero);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static RigidTransform EulerXYZ(float3 xyz)
		{
			return new RigidTransform(quaternion.EulerXYZ(xyz), float3.zero);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static RigidTransform EulerXZY(float3 xyz)
		{
			return new RigidTransform(quaternion.EulerXZY(xyz), float3.zero);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static RigidTransform EulerYXZ(float3 xyz)
		{
			return new RigidTransform(quaternion.EulerYXZ(xyz), float3.zero);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static RigidTransform EulerYZX(float3 xyz)
		{
			return new RigidTransform(quaternion.EulerYZX(xyz), float3.zero);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static RigidTransform EulerZXY(float3 xyz)
		{
			return new RigidTransform(quaternion.EulerZXY(xyz), float3.zero);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static RigidTransform EulerZYX(float3 xyz)
		{
			return new RigidTransform(quaternion.EulerZYX(xyz), float3.zero);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static RigidTransform EulerXYZ(float x, float y, float z)
		{
			return RigidTransform.EulerXYZ(math.float3(x, y, z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static RigidTransform EulerXZY(float x, float y, float z)
		{
			return RigidTransform.EulerXZY(math.float3(x, y, z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static RigidTransform EulerYXZ(float x, float y, float z)
		{
			return RigidTransform.EulerYXZ(math.float3(x, y, z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static RigidTransform EulerYZX(float x, float y, float z)
		{
			return RigidTransform.EulerYZX(math.float3(x, y, z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static RigidTransform EulerZXY(float x, float y, float z)
		{
			return RigidTransform.EulerZXY(math.float3(x, y, z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static RigidTransform EulerZYX(float x, float y, float z)
		{
			return RigidTransform.EulerZYX(math.float3(x, y, z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static RigidTransform Euler(float3 xyz, math.RotationOrder order = math.RotationOrder.ZXY)
		{
			switch (order)
			{
			case math.RotationOrder.XYZ:
				return RigidTransform.EulerXYZ(xyz);
			case math.RotationOrder.XZY:
				return RigidTransform.EulerXZY(xyz);
			case math.RotationOrder.YXZ:
				return RigidTransform.EulerYXZ(xyz);
			case math.RotationOrder.YZX:
				return RigidTransform.EulerYZX(xyz);
			case math.RotationOrder.ZXY:
				return RigidTransform.EulerZXY(xyz);
			case math.RotationOrder.ZYX:
				return RigidTransform.EulerZYX(xyz);
			default:
				return RigidTransform.identity;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static RigidTransform Euler(float x, float y, float z, math.RotationOrder order = math.RotationOrder.ZXY)
		{
			return RigidTransform.Euler(math.float3(x, y, z), order);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static RigidTransform RotateX(float angle)
		{
			return new RigidTransform(quaternion.RotateX(angle), float3.zero);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static RigidTransform RotateY(float angle)
		{
			return new RigidTransform(quaternion.RotateY(angle), float3.zero);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static RigidTransform RotateZ(float angle)
		{
			return new RigidTransform(quaternion.RotateZ(angle), float3.zero);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static RigidTransform Translate(float3 vector)
		{
			return new RigidTransform(quaternion.identity, vector);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(RigidTransform x)
		{
			return this.rot.Equals(x.rot) && this.pos.Equals(x.pos);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Equals(object x)
		{
			if (x is RigidTransform)
			{
				RigidTransform rigidTransform = (RigidTransform)x;
				return this.Equals(rigidTransform);
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
		{
			return (int)math.hash(this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString()
		{
			return string.Format("RigidTransform(({0}f, {1}f, {2}f, {3}f),  ({4}f, {5}f, {6}f))", new object[]
			{
				this.rot.value.x,
				this.rot.value.y,
				this.rot.value.z,
				this.rot.value.w,
				this.pos.x,
				this.pos.y,
				this.pos.z
			});
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return string.Format("float4x4(({0}f, {1}f, {2}f, {3}f),  ({4}f, {5}f, {6}f))", new object[]
			{
				this.rot.value.x.ToString(format, formatProvider),
				this.rot.value.y.ToString(format, formatProvider),
				this.rot.value.z.ToString(format, formatProvider),
				this.rot.value.w.ToString(format, formatProvider),
				this.pos.x.ToString(format, formatProvider),
				this.pos.y.ToString(format, formatProvider),
				this.pos.z.ToString(format, formatProvider)
			});
		}

		public quaternion rot;

		public float3 pos;

		public static readonly RigidTransform identity = new RigidTransform(new quaternion(0f, 0f, 0f, 1f), new float3(0f, 0f, 0f));
	}
}
