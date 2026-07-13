using System;
using Unity.Collections;

namespace UnityEngine.LowLevelPhysics2D
{
	public readonly struct PhysicsComposer : IEquatable<PhysicsComposer>
	{
		public override string ToString()
		{
			return this.isValid ? string.Format("index={0}, generation={1}", this.m_Index1, this.m_Generation) : "<INVALID>";
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public bool Equals(PhysicsComposer other)
		{
			return this.m_Index1 == other.m_Index1 && this.m_Generation == other.m_Generation;
		}

		public static bool operator ==(PhysicsComposer lhs, PhysicsComposer rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(PhysicsComposer lhs, PhysicsComposer rhs)
		{
			return !(lhs == rhs);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine<int, ushort>(this.m_Index1, this.m_Generation);
		}

		public static PhysicsComposer Create(Allocator allocator = Allocator.Temp)
		{
			return PhysicsComposerScripting2D.PhysicsComposer_Create(allocator);
		}

		public bool Destroy()
		{
			return PhysicsComposerScripting2D.PhysicsComposer_Destroy(this);
		}

		public bool isValid
		{
			get
			{
				return PhysicsComposerScripting2D.Composer_IsValid(this);
			}
		}

		public unsafe PhysicsComposer.LayerHandle AddLayer(CircleGeometry geometry, PhysicsTransform transform, PhysicsComposer.Operation operation = PhysicsComposer.Operation.OR, int order = 0, float curveStride = 0.06f, bool reverseWinding = false)
		{
			return this.AddLayer(new ReadOnlySpan<CircleGeometry>((void*)(&geometry), 1), transform, operation, order, curveStride, reverseWinding);
		}

		public PhysicsComposer.LayerHandle AddLayer(ReadOnlySpan<CircleGeometry> geometry, PhysicsTransform transform, PhysicsComposer.Operation operation = PhysicsComposer.Operation.OR, int order = 0, float curveStride = 0.06f, bool reverseWinding = false)
		{
			return PhysicsComposerScripting2D.PhysicsComposer_AddLayer(this, new PhysicsComposer.Layer(geometry, transform, operation, order, curveStride, reverseWinding));
		}

		public unsafe PhysicsComposer.LayerHandle AddLayer(CapsuleGeometry geometry, PhysicsTransform transform, PhysicsComposer.Operation operation = PhysicsComposer.Operation.OR, int order = 0, float curveStride = 0.06f, bool reverseWinding = false)
		{
			return this.AddLayer(new ReadOnlySpan<CapsuleGeometry>((void*)(&geometry), 1), transform, operation, order, curveStride, reverseWinding);
		}

		public PhysicsComposer.LayerHandle AddLayer(ReadOnlySpan<CapsuleGeometry> geometry, PhysicsTransform transform, PhysicsComposer.Operation operation = PhysicsComposer.Operation.OR, int order = 0, float curveStride = 0.06f, bool reverseWinding = false)
		{
			return PhysicsComposerScripting2D.PhysicsComposer_AddLayer(this, new PhysicsComposer.Layer(geometry, transform, operation, order, curveStride, reverseWinding));
		}

		public unsafe PhysicsComposer.LayerHandle AddLayer(PolygonGeometry geometry, PhysicsTransform transform, PhysicsComposer.Operation operation = PhysicsComposer.Operation.OR, int order = 0, float curveStride = 0.06f, bool reverseWinding = false)
		{
			return this.AddLayer(new ReadOnlySpan<PolygonGeometry>((void*)(&geometry), 1), transform, operation, order, curveStride, reverseWinding);
		}

		public PhysicsComposer.LayerHandle AddLayer(ReadOnlySpan<PolygonGeometry> geometry, PhysicsTransform transform, PhysicsComposer.Operation operation = PhysicsComposer.Operation.OR, int order = 0, float curveStride = 0.06f, bool reverseWinding = false)
		{
			return PhysicsComposerScripting2D.PhysicsComposer_AddLayer(this, new PhysicsComposer.Layer(geometry, transform, operation, order, curveStride, reverseWinding));
		}

		public unsafe PhysicsComposer.LayerHandle AddLayer(PhysicsShape shape, PhysicsTransform transform, PhysicsComposer.Operation operation = PhysicsComposer.Operation.OR, int order = 0, float curveStride = 0.06f, bool reverseWinding = false)
		{
			return this.AddLayer(new ReadOnlySpan<PhysicsShape>((void*)(&shape), 1), transform, operation, order, curveStride, reverseWinding);
		}

		public PhysicsComposer.LayerHandle AddLayer(ReadOnlySpan<PhysicsShape> shapes, PhysicsTransform transform, PhysicsComposer.Operation operation = PhysicsComposer.Operation.OR, int order = 0, float curveStride = 0.06f, bool reverseWinding = false)
		{
			return PhysicsComposerScripting2D.PhysicsComposer_AddLayer(this, new PhysicsComposer.Layer(shapes, transform, operation, order, curveStride, reverseWinding));
		}

		public PhysicsComposer.LayerHandle AddLayer(ReadOnlySpan<Vector2> vertices, PhysicsTransform transform, PhysicsComposer.Operation operation = PhysicsComposer.Operation.OR, int order = 0, bool reverseWinding = false)
		{
			PhysicsComposer.Layer layer = new PhysicsComposer.Layer(PhysicsLowLevelScripting2D.PhysicsBuffer.FromSpan<Vector2>(vertices), transform, operation, order, reverseWinding);
			return PhysicsComposerScripting2D.PhysicsComposer_AddLayer(this, layer);
		}

		public void RemoveLayer(PhysicsComposer.LayerHandle layerHandle)
		{
			PhysicsComposerScripting2D.PhysicsComposer_RemoveLayer(this, layerHandle);
		}

		public bool useDelaunay
		{
			get
			{
				return PhysicsComposerScripting2D.PhysicsComposer_GetDelaunay(this);
			}
			set
			{
				PhysicsComposerScripting2D.PhysicsComposer_SetDelaunay(this, value);
			}
		}

		public int maxPolygonVertices
		{
			get
			{
				return PhysicsComposerScripting2D.PhysicsComposer_GetMaxPolygonVertices(this);
			}
			set
			{
				PhysicsComposerScripting2D.PhysicsComposer_SetMaxPolygonVertices(this, value);
			}
		}

		public int layerCount
		{
			get
			{
				return PhysicsComposerScripting2D.PhysicsComposer_GetLayerCount(this);
			}
		}

		public NativeArray<PhysicsComposer.LayerHandle> layerHandles
		{
			get
			{
				return PhysicsComposerScripting2D.PhysicsComposer_GetLayerHandles(this).ToNativeArray<PhysicsComposer.LayerHandle>();
			}
		}

		public int rejectedGeometryCount
		{
			get
			{
				return PhysicsComposerScripting2D.PhysicsComposer_GetRejectedGeometryCount(this);
			}
		}

		public NativeArray<RangeInt> GetGeometryIslands(Allocator allocator)
		{
			return PhysicsComposerScripting2D.PhysicsComposer_GetGeometryIslands(this, allocator).ToNativeArray<RangeInt>();
		}

		public NativeArray<PolygonGeometry> CreatePolygonGeometry(Vector2 vertexScale, Allocator allocator)
		{
			return PhysicsComposerScripting2D.PhysicsComposer_CreatePolygonGeometry(this, vertexScale, allocator).ToNativeArray<PolygonGeometry>();
		}

		public NativeArray<PolygonGeometry.ConvexHull> CreateConvexHulls(Vector2 vertexScale, Allocator allocator)
		{
			return PhysicsComposerScripting2D.PhysicsComposer_CreateConvexHulls(this, vertexScale, allocator).ToNativeArray<PolygonGeometry.ConvexHull>();
		}

		public NativeArray<ChainGeometry> CreateChainGeometry(out NativeArray<Vector2> vertices, Vector2 vertexScale, Allocator allocator)
		{
			PhysicsLowLevelScripting2D.PhysicsBufferPair physicsBufferPair = PhysicsComposerScripting2D.PhysicsComposer_CreateChainGeometry(this, vertexScale, allocator);
			vertices = physicsBufferPair.buffer1.ToNativeArray<Vector2>();
			NativeArray<ChainGeometry> nativeArray2;
			using (NativeArray<PhysicsLowLevelScripting2D.PhysicsBuffer> nativeArray = physicsBufferPair.buffer2.ToNativeArray<PhysicsLowLevelScripting2D.PhysicsBuffer>())
			{
				bool flag = vertices.Length == 0 || nativeArray.Length == 0;
				if (flag)
				{
					bool flag2 = vertices.Length > 0;
					if (flag2)
					{
						vertices.Dispose();
					}
					nativeArray2 = default(NativeArray<ChainGeometry>);
				}
				else
				{
					NativeArray<ChainGeometry> nativeArray3 = new NativeArray<ChainGeometry>(nativeArray.Length, physicsBufferPair.buffer2.allocator, NativeArrayOptions.UninitializedMemory);
					for (int i = 0; i < nativeArray.Length; i++)
					{
						nativeArray3[i] = new ChainGeometry(nativeArray[i].ToSpan<Vector2>());
					}
					nativeArray2 = nativeArray3;
				}
			}
			return nativeArray2;
		}

		private readonly int m_Index1;

		private readonly ushort m_Generation;

		public const float DefaultCurveStride = 0.06f;

		public const float MinCurveStride = 0.01f;

		[Serializable]
		internal readonly struct Layer
		{
			internal Layer(ReadOnlySpan<CircleGeometry> geometry, PhysicsTransform transform, PhysicsComposer.Operation operation, int order, float curveStride, bool reverseWinding)
			{
				bool flag = geometry.Length < 1;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("geometry", "At least a single geometry must be specified.");
				}
				bool flag2 = curveStride < 0.01f || curveStride > 1f;
				if (flag2)
				{
					throw new ArgumentOutOfRangeException("curveStride", string.Format("Curve Stride must be in the range [{0}, 1.0]", 0.01f));
				}
				this.m_LayerType = PhysicsComposer.Layer.LayerType.Geometry;
				this.m_GeometryType = PhysicsShape.ShapeType.Circle;
				this.m_DataBuffer = PhysicsLowLevelScripting2D.PhysicsBuffer.FromSpan<CircleGeometry>(geometry);
				this.m_Transform = transform;
				this.m_Operation = operation;
				this.m_Order = order;
				this.m_CurveStride = curveStride;
				this.m_ReverseWinding = reverseWinding;
			}

			internal Layer(ReadOnlySpan<CapsuleGeometry> geometry, PhysicsTransform transform, PhysicsComposer.Operation operation, int order, float curveStride, bool reverseWinding)
			{
				bool flag = geometry.Length < 1;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("geometry", "At least a single geometry must be specified.");
				}
				bool flag2 = curveStride < 0.01f || curveStride > 1f;
				if (flag2)
				{
					throw new ArgumentOutOfRangeException("curveStride", string.Format("Curve Stride must be in the range [{0}, 1.0]", 0.01f));
				}
				this.m_LayerType = PhysicsComposer.Layer.LayerType.Geometry;
				this.m_GeometryType = PhysicsShape.ShapeType.Capsule;
				this.m_DataBuffer = PhysicsLowLevelScripting2D.PhysicsBuffer.FromSpan<CapsuleGeometry>(geometry);
				this.m_Transform = transform;
				this.m_Operation = operation;
				this.m_Order = order;
				this.m_CurveStride = curveStride;
				this.m_ReverseWinding = reverseWinding;
			}

			internal Layer(ReadOnlySpan<PolygonGeometry> geometry, PhysicsTransform transform, PhysicsComposer.Operation operation, int order, float curveStride, bool reverseWinding)
			{
				bool flag = geometry.Length < 1;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("geometry", "At least a single geometry must be specified.");
				}
				bool flag2 = curveStride < 0.01f || curveStride > 1f;
				if (flag2)
				{
					throw new ArgumentOutOfRangeException("curveStride", string.Format("Curve Stride must be in the range [{0}, 1.0]", 0.01f));
				}
				this.m_LayerType = PhysicsComposer.Layer.LayerType.Geometry;
				this.m_GeometryType = PhysicsShape.ShapeType.Polygon;
				this.m_DataBuffer = PhysicsLowLevelScripting2D.PhysicsBuffer.FromSpan<PolygonGeometry>(geometry);
				this.m_Transform = transform;
				this.m_Operation = operation;
				this.m_Order = order;
				this.m_CurveStride = curveStride;
				this.m_ReverseWinding = reverseWinding;
			}

			internal unsafe Layer(ReadOnlySpan<PhysicsShape> shapes, PhysicsTransform transform, PhysicsComposer.Operation operation, int order, float curveStride, bool reverseWinding)
			{
				bool flag = shapes.Length < 1;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("shapes", "At least a single PhysicsShape must be specified.");
				}
				ReadOnlySpan<PhysicsShape> readOnlySpan = shapes;
				for (int i = 0; i < readOnlySpan.Length; i++)
				{
					PhysicsShape physicsShape = *readOnlySpan[i];
					bool flag2 = !physicsShape.isValid;
					if (flag2)
					{
						throw new ArgumentException("shapes", "At least one of the shapes was invalid.");
					}
				}
				bool flag3 = curveStride < 0.01f || curveStride > 1f;
				if (flag3)
				{
					throw new ArgumentOutOfRangeException("curveStride", string.Format("Curve Stride must be in the range [{0}, 1.0]", 0.01f));
				}
				this.m_LayerType = PhysicsComposer.Layer.LayerType.Shape;
				this.m_DataBuffer = PhysicsLowLevelScripting2D.PhysicsBuffer.FromSpan<PhysicsShape>(shapes);
				this.m_Transform = transform;
				this.m_Operation = operation;
				this.m_Order = order;
				this.m_CurveStride = curveStride;
				this.m_ReverseWinding = reverseWinding;
				this.m_GeometryType = PhysicsShape.ShapeType.Circle;
			}

			internal Layer(PhysicsLowLevelScripting2D.PhysicsBuffer vertices, PhysicsTransform transform, PhysicsComposer.Operation operation, int order, bool reverseWinding)
			{
				bool flag = vertices.size < 3;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("vertices", "A minimum of 3 vertices must be specified.");
				}
				this.m_LayerType = PhysicsComposer.Layer.LayerType.Vertex;
				this.m_DataBuffer = vertices;
				this.m_Transform = transform;
				this.m_Operation = operation;
				this.m_Order = order;
				this.m_CurveStride = 1f;
				this.m_ReverseWinding = reverseWinding;
				this.m_GeometryType = PhysicsShape.ShapeType.Circle;
			}

			public PhysicsComposer.Layer.LayerType layerType
			{
				get
				{
					return this.m_LayerType;
				}
			}

			public PhysicsShape.ShapeType geometryType
			{
				get
				{
					return this.m_GeometryType;
				}
			}

			public PhysicsLowLevelScripting2D.PhysicsBuffer dataBuffer
			{
				get
				{
					return this.m_DataBuffer;
				}
			}

			public PhysicsTransform transform
			{
				get
				{
					return this.m_Transform;
				}
			}

			public PhysicsComposer.Operation operation
			{
				get
				{
					return this.m_Operation;
				}
			}

			public int order
			{
				get
				{
					return this.m_Order;
				}
			}

			public float curveStride
			{
				get
				{
					return this.m_CurveStride;
				}
			}

			public bool reverseWinding
			{
				get
				{
					return this.m_ReverseWinding;
				}
			}

			private readonly PhysicsComposer.Layer.LayerType m_LayerType;

			private readonly PhysicsShape.ShapeType m_GeometryType;

			private readonly PhysicsLowLevelScripting2D.PhysicsBuffer m_DataBuffer;

			private readonly PhysicsTransform m_Transform;

			private readonly PhysicsComposer.Operation m_Operation;

			private readonly int m_Order;

			private readonly float m_CurveStride;

			private readonly bool m_ReverseWinding;

			public enum LayerType
			{
				Geometry,
				Shape,
				Vertex
			}
		}

		public readonly struct LayerHandle
		{
			public override string ToString()
			{
				return string.Format("index={0}, composer={1}, generation={2}", this.m_IndexId, this.m_Composer, this.m_Revision);
			}

			private readonly int m_IndexId;

			private readonly int m_Composer;

			private readonly ushort m_Revision;
		}

		public enum Operation
		{
			OR,
			AND,
			NOT,
			XOR
		}
	}
}
