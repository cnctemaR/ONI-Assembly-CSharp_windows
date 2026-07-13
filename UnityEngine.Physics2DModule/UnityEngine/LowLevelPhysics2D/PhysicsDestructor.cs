using System;
using Unity.Collections;

namespace UnityEngine.LowLevelPhysics2D
{
	public readonly struct PhysicsDestructor
	{
		public static PhysicsDestructor.FragmentResult Fragment(PhysicsDestructor.FragmentGeometry target, ReadOnlySpan<Vector2> fragmentPoints, Allocator allocator)
		{
			return PhysicsDestructorScripting2D.PhysicsDestructor_Fragment(target, fragmentPoints, allocator);
		}

		public static PhysicsDestructor.FragmentResult Fragment(PhysicsDestructor.FragmentGeometry target, PhysicsDestructor.FragmentGeometry mask, ReadOnlySpan<Vector2> fragmentPoints, Allocator allocator)
		{
			return PhysicsDestructorScripting2D.PhysicsDestructor_FragmentMasked(target, mask, fragmentPoints, allocator);
		}

		public static PhysicsDestructor.SliceResult Slice(PhysicsDestructor.FragmentGeometry target, Vector2 origin, Vector2 translation, Allocator allocator)
		{
			return PhysicsDestructorScripting2D.PhysicsDestructor_Slice(target, origin, translation, allocator);
		}

		public readonly struct FragmentGeometry
		{
			public FragmentGeometry(PhysicsTransform transform, ReadOnlySpan<PolygonGeometry> geometry)
			{
				this.m_Transform = transform;
				this.m_Geometry = PhysicsLowLevelScripting2D.PhysicsBuffer.FromSpan<PolygonGeometry>(geometry);
			}

			private readonly PhysicsTransform m_Transform;

			private readonly PhysicsLowLevelScripting2D.PhysicsBuffer m_Geometry;
		}

		public readonly struct FragmentResult : IDisposable
		{
			public PhysicsTransform transform
			{
				get
				{
					return this.m_Transform;
				}
			}

			public NativeArray<RangeInt> unbrokenGeometryIslands
			{
				get
				{
					return this.m_UnbrokenGeometryIslands.ToNativeArray<RangeInt>();
				}
			}

			public NativeArray<PolygonGeometry> unbrokenGeometry
			{
				get
				{
					return this.m_UnbrokenGeometry.ToNativeArray<PolygonGeometry>();
				}
			}

			public NativeArray<PolygonGeometry> brokenGeometry
			{
				get
				{
					return this.m_BrokenGeometry.ToNativeArray<PolygonGeometry>();
				}
			}

			public void Dispose()
			{
				this.unbrokenGeometryIslands.Dispose();
				this.unbrokenGeometry.Dispose();
				this.brokenGeometry.Dispose();
			}

			private readonly PhysicsTransform m_Transform;

			private readonly PhysicsLowLevelScripting2D.PhysicsBuffer m_UnbrokenGeometryIslands;

			private readonly PhysicsLowLevelScripting2D.PhysicsBuffer m_UnbrokenGeometry;

			private readonly PhysicsLowLevelScripting2D.PhysicsBuffer m_BrokenGeometry;
		}

		public readonly struct SliceResult : IDisposable
		{
			public PhysicsTransform transform
			{
				get
				{
					return this.m_Transform;
				}
			}

			public NativeArray<PolygonGeometry> leftGeometry
			{
				get
				{
					return this.m_LeftGeometry.ToNativeArray<PolygonGeometry>();
				}
			}

			public NativeArray<PolygonGeometry> rightGeometry
			{
				get
				{
					return this.m_RightGeometry.ToNativeArray<PolygonGeometry>();
				}
			}

			public void Dispose()
			{
				this.leftGeometry.Dispose();
				this.rightGeometry.Dispose();
			}

			private readonly PhysicsTransform m_Transform;

			private readonly PhysicsLowLevelScripting2D.PhysicsBuffer m_LeftGeometry;

			private readonly PhysicsLowLevelScripting2D.PhysicsBuffer m_RightGeometry;
		}
	}
}
