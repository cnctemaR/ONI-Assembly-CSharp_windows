using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct AffineTransform : IEquatable<AffineTransform>, IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public AffineTransform(float3 translation, quaternion rotation)
		{
			this.rs = math.float3x3(rotation);
			this.t = translation;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public AffineTransform(float3 translation, quaternion rotation, float3 scale)
		{
			this.rs = math.mulScale(math.float3x3(rotation), scale);
			this.t = translation;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public AffineTransform(float3 translation, float3x3 rotationScale)
		{
			this.rs = rotationScale;
			this.t = translation;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public AffineTransform(float3x3 rotationScale)
		{
			this.rs = rotationScale;
			this.t = float3.zero;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public AffineTransform(RigidTransform rigid)
		{
			this.rs = math.float3x3(rigid.rot);
			this.t = rigid.pos;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public AffineTransform(float3x4 m)
		{
			this.rs = math.float3x3(m.c0, m.c1, m.c2);
			this.t = m.c3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public AffineTransform(float4x4 m)
		{
			this.rs = math.float3x3(m.c0.xyz, m.c1.xyz, m.c2.xyz);
			this.t = m.c3.xyz;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float3x4(AffineTransform m)
		{
			return math.float3x4(m.rs.c0, m.rs.c1, m.rs.c2, m.t);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float4x4(AffineTransform m)
		{
			return math.float4x4(math.float4(m.rs.c0, 0f), math.float4(m.rs.c1, 0f), math.float4(m.rs.c2, 0f), math.float4(m.t, 1f));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(AffineTransform rhs)
		{
			return this.rs.Equals(rhs.rs) && this.t.Equals(rhs.t);
		}

		public override bool Equals(object o)
		{
			if (o is AffineTransform)
			{
				AffineTransform affineTransform = (AffineTransform)o;
				return this.Equals(affineTransform);
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
			return string.Format("AffineTransform(({0}f, {1}f, {2}f,  {3}f, {4}f, {5}f,  {6}f, {7}f, {8}f), ({9}f, {10}f, {11}f))", new object[]
			{
				this.rs.c0.x,
				this.rs.c1.x,
				this.rs.c2.x,
				this.rs.c0.y,
				this.rs.c1.y,
				this.rs.c2.y,
				this.rs.c0.z,
				this.rs.c1.z,
				this.rs.c2.z,
				this.t.x,
				this.t.y,
				this.t.z
			});
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return string.Format("AffineTransform(({0}f, {1}f, {2}f,  {3}f, {4}f, {5}f,  {6}f, {7}f, {8}f), ({9}f, {10}f, {11}f))", new object[]
			{
				this.rs.c0.x.ToString(format, formatProvider),
				this.rs.c1.x.ToString(format, formatProvider),
				this.rs.c2.x.ToString(format, formatProvider),
				this.rs.c0.y.ToString(format, formatProvider),
				this.rs.c1.y.ToString(format, formatProvider),
				this.rs.c2.y.ToString(format, formatProvider),
				this.rs.c0.z.ToString(format, formatProvider),
				this.rs.c1.z.ToString(format, formatProvider),
				this.rs.c2.z.ToString(format, formatProvider),
				this.t.x.ToString(format, formatProvider),
				this.t.y.ToString(format, formatProvider),
				this.t.z.ToString(format, formatProvider)
			});
		}

		public float3x3 rs;

		public float3 t;

		public static readonly AffineTransform identity = new AffineTransform(float3.zero, float3x3.identity);

		public static readonly AffineTransform zero;
	}
}
