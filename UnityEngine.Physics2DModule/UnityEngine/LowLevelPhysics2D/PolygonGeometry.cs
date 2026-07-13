using System;
using Unity.Collections;

namespace UnityEngine.LowLevelPhysics2D
{
	[Serializable]
	public struct PolygonGeometry
	{
		public PolygonGeometry()
		{
			this.vertices = new PhysicsShape.ShapeArray
			{
				vertex0 = new Vector2(-0.5f, -0.5f),
				vertex1 = new Vector2(0.5f, -0.5f),
				vertex2 = new Vector2(0.5f, 0.5f),
				vertex3 = new Vector2(-0.5f, 0.5f)
			};
			this.normals = new PhysicsShape.ShapeArray
			{
				vertex0 = Vector2.down,
				vertex1 = Vector2.right,
				vertex2 = Vector2.right,
				vertex3 = Vector2.left
			};
			this.m_Count = 4;
			this.m_Centroid = Vector2.zero;
			this.m_Radius = 0f;
		}

		public static PolygonGeometry CreateBox(Vector2 size, float radius = 0f, bool inscribe = false)
		{
			return PhysicsLowLevelScripting2D.PolygonGeometry_CreateBox(size, radius, PhysicsTransform.identity, inscribe);
		}

		public static NativeArray<PolygonGeometry> CreatePolygons(ReadOnlySpan<Vector2> vertices, PhysicsTransform transform, Vector2 vertexScale, Allocator allocator = Allocator.Temp)
		{
			return PhysicsLowLevelScripting2D.PolygonGeometry_CreatePolygons(vertices, transform, vertexScale, allocator).ToNativeArray<PolygonGeometry>();
		}

		public static PolygonGeometry CreateBox(Vector2 size, float radius, PhysicsTransform transform, bool inscribe = false)
		{
			return PhysicsLowLevelScripting2D.PolygonGeometry_CreateBox(size, radius, transform, inscribe);
		}

		public static PolygonGeometry Create(ReadOnlySpan<Vector2> vertices, float radius = 0f)
		{
			return PhysicsLowLevelScripting2D.PolygonGeometry_Create_WithPhysicsTransform(vertices, radius, PhysicsTransform.identity);
		}

		public static PolygonGeometry Create(ReadOnlySpan<Vector2> vertices, float radius, PhysicsTransform transform)
		{
			return PhysicsLowLevelScripting2D.PolygonGeometry_Create_WithPhysicsTransform(vertices, radius, transform);
		}

		public static PolygonGeometry Create(ReadOnlySpan<Vector2> vertices, float radius, Matrix4x4 transform)
		{
			return PhysicsLowLevelScripting2D.PolygonGeometry_Create_WithMatrix(vertices, radius, transform);
		}

		public static PolygonGeometry Create(ref PolygonGeometry.ConvexHull convexHull, float radius)
		{
			PolygonGeometry polygonGeometry = new PolygonGeometry();
			polygonGeometry.vertices = convexHull.vertices;
			polygonGeometry.count = convexHull.count;
			polygonGeometry.radius = radius;
			return polygonGeometry.Validate();
		}

		public readonly bool isValid
		{
			get
			{
				return PhysicsLowLevelScripting2D.PolygonGeometry_IsValid(this);
			}
		}

		public unsafe static PolygonGeometry InsertVertex(PolygonGeometry geometry, int index, Vector2 vertex)
		{
			bool flag = geometry.count == 8 || (index < 0 && index >= 8);
			if (flag)
			{
				throw new ArgumentOutOfRangeException("index", "Invalid index.");
			}
			int count = geometry.count;
			geometry.count = count + 1;
			ref PhysicsShape.ShapeArray ptr = ref geometry.vertices;
			for (int i = geometry.count - 1; i > index; i--)
			{
				*ptr[i] = *ptr[i - 1];
			}
			*ptr[index] = vertex;
			return geometry.Validate();
		}

		public unsafe static PolygonGeometry DeleteVertex(PolygonGeometry geometry, int index)
		{
			bool flag = geometry.count == 3 || (index < 0 && index >= 8);
			if (flag)
			{
				throw new ArgumentOutOfRangeException("index", "Invalid index.");
			}
			int num = geometry.count - 1;
			geometry.count = num;
			ref PhysicsShape.ShapeArray ptr = ref geometry.vertices;
			for (int i = index; i < geometry.count; i++)
			{
				*ptr[i] = *ptr[i + 1];
			}
			return geometry.Validate();
		}

		public Vector2 centroid
		{
			readonly get
			{
				return this.m_Centroid;
			}
			set
			{
				this.m_Centroid = value;
			}
		}

		public float radius
		{
			readonly get
			{
				return this.m_Radius;
			}
			set
			{
				this.m_Radius = Mathf.Max(0f, value);
			}
		}

		public int count
		{
			readonly get
			{
				return this.m_Count;
			}
			set
			{
				this.m_Count = Mathf.Clamp(value, 3, 8);
			}
		}

		public unsafe ReadOnlySpan<Vector2> AsReadOnlySpan()
		{
			ref Vector2 ptr = ref this.vertices[0];
			fixed (Vector2* ptr2 = &ptr)
			{
				Vector2* ptr3 = ptr2;
				return new ReadOnlySpan<Vector2>((void*)ptr3, this.m_Count);
			}
		}

		public readonly PolygonGeometry Validate()
		{
			return PhysicsLowLevelScripting2D.PolygonGeometry_Validate(this);
		}

		public readonly PhysicsBody.MassConfiguration CalculateMassConfiguration(float density = 1f)
		{
			return PhysicsLowLevelScripting2D.PolygonGeometry_CalculateMassConfiguration(this, density);
		}

		public readonly PhysicsAABB CalculateAABB(PhysicsTransform transform)
		{
			return PhysicsLowLevelScripting2D.PolygonGeometry_CalculateAABB(this, transform);
		}

		public readonly bool OverlapPoint(Vector2 point)
		{
			return PhysicsLowLevelScripting2D.PolygonGeometry_OverlapPoint(this, point);
		}

		public readonly Vector2 ClosestPoint(Vector2 point)
		{
			return PhysicsLowLevelScripting2D.PolygonGeometry_ClosestPoint(this, point);
		}

		public readonly PhysicsQuery.CastResult CastRay(PhysicsQuery.CastRayInput castRayInput)
		{
			return PhysicsLowLevelScripting2D.PolygonGeometry_CastRay(this, castRayInput);
		}

		public readonly PhysicsQuery.CastResult CastShape(PhysicsQuery.CastShapeInput input)
		{
			return PhysicsLowLevelScripting2D.PolygonGeometry_CastShape(this, input);
		}

		public readonly PhysicsShape.ContactManifold Intersect(PhysicsTransform transform, CircleGeometry otherGeometry, PhysicsTransform otherTransform)
		{
			return PhysicsQuery.PolygonAndCircle(this, transform, otherGeometry, otherTransform);
		}

		public readonly PhysicsShape.ContactManifold Intersect(PhysicsTransform transform, CapsuleGeometry otherGeometry, PhysicsTransform otherTransform)
		{
			return PhysicsQuery.PolygonAndCapsule(this, transform, otherGeometry, otherTransform);
		}

		public readonly PhysicsShape.ContactManifold Intersect(PhysicsTransform transform, PolygonGeometry otherGeometry, PhysicsTransform otherTransform)
		{
			return PhysicsQuery.PolygonAndPolygon(this, transform, otherGeometry, otherTransform);
		}

		public readonly PhysicsShape.ContactManifold Intersect(PhysicsTransform transform, SegmentGeometry otherGeometry, PhysicsTransform otherTransform)
		{
			return PhysicsQuery.SegmentAndPolygon(otherGeometry, otherTransform, this, transform);
		}

		public readonly PolygonGeometry Transform(PhysicsTransform transform)
		{
			return PhysicsLowLevelScripting2D.PolygonGeometry_Transform_WithPhysicsTransform(this, transform);
		}

		public readonly PolygonGeometry InverseTransform(PhysicsTransform transform)
		{
			return PhysicsLowLevelScripting2D.PolygonGeometry_InverseTransform_WithPhysicsTransform(this, transform);
		}

		public readonly PolygonGeometry Transform(Matrix4x4 transform, bool scaleRadius)
		{
			return PhysicsLowLevelScripting2D.PolygonGeometry_Transform_WithMatrix(this, transform, scaleRadius);
		}

		public readonly PolygonGeometry InverseTransform(Matrix4x4 transform, bool scaleRadius)
		{
			return PhysicsLowLevelScripting2D.PolygonGeometry_InverseTransform_WithMatrix(this, transform, scaleRadius);
		}

		public static readonly PolygonGeometry defaultGeometry = PolygonGeometry.CreateBox(Vector2.one, 0f, false);

		public PhysicsShape.ShapeArray vertices;

		public PhysicsShape.ShapeArray normals;

		[SerializeField]
		internal Vector2 m_Centroid;

		[SerializeField]
		[Min(0f)]
		internal float m_Radius;

		[SerializeField]
		[Range(3f, 8f)]
		internal int m_Count;

		[Serializable]
		public struct ConvexHull
		{
			public int count
			{
				readonly get
				{
					return this.m_Count;
				}
				set
				{
					this.m_Count = Mathf.Clamp(value, 3, 8);
				}
			}

			public unsafe ReadOnlySpan<Vector2> AsReadOnlySpan()
			{
				ref Vector2 ptr = ref this.vertices[0];
				fixed (Vector2* ptr2 = &ptr)
				{
					Vector2* ptr3 = ptr2;
					return new ReadOnlySpan<Vector2>((void*)ptr3, this.m_Count);
				}
			}

			public PhysicsShape.ShapeArray vertices;

			[SerializeField]
			[Range(3f, 8f)]
			internal int m_Count;
		}
	}
}
