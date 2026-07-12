using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	public class PhysicsShapeGroup2D
	{
		internal List<Vector2> groupVertices
		{
			get
			{
				return this.m_GroupState.m_Vertices;
			}
		}

		internal List<PhysicsShape2D> groupShapes
		{
			get
			{
				return this.m_GroupState.m_Shapes;
			}
		}

		public int shapeCount
		{
			get
			{
				return this.m_GroupState.m_Shapes.Count;
			}
		}

		public int vertexCount
		{
			get
			{
				return this.m_GroupState.m_Vertices.Count;
			}
		}

		public Matrix4x4 localToWorldMatrix
		{
			get
			{
				return this.m_GroupState.m_LocalToWorld;
			}
			set
			{
				this.m_GroupState.m_LocalToWorld = value;
			}
		}

		public PhysicsShapeGroup2D([DefaultValue("1")] int shapeCapacity = 1, [DefaultValue("8")] int vertexCapacity = 8)
		{
			this.m_GroupState = new PhysicsShapeGroup2D.GroupState
			{
				m_Shapes = new List<PhysicsShape2D>(shapeCapacity),
				m_Vertices = new List<Vector2>(vertexCapacity),
				m_LocalToWorld = Matrix4x4.identity
			};
		}

		public void Clear()
		{
			this.m_GroupState.ClearGeometry();
			this.m_GroupState.m_LocalToWorld = Matrix4x4.identity;
		}

		public void Add(PhysicsShapeGroup2D physicsShapeGroup)
		{
			bool flag = physicsShapeGroup == null;
			if (flag)
			{
				throw new ArgumentNullException("Cannot merge a NULL PhysicsShapeGroup2D.");
			}
			bool flag2 = physicsShapeGroup == this;
			if (flag2)
			{
				throw new ArgumentException("Cannot merge a PhysicsShapeGroup2D with itself.");
			}
			bool flag3 = physicsShapeGroup.shapeCount == 0;
			if (!flag3)
			{
				int count = this.groupShapes.Count;
				int count2 = this.m_GroupState.m_Vertices.Count;
				this.groupShapes.AddRange(physicsShapeGroup.groupShapes);
				this.groupVertices.AddRange(physicsShapeGroup.groupVertices);
				bool flag4 = count > 0;
				if (flag4)
				{
					for (int i = count; i < this.m_GroupState.m_Shapes.Count; i++)
					{
						PhysicsShape2D physicsShape2D = this.m_GroupState.m_Shapes[i];
						physicsShape2D.vertexStartIndex += count2;
						this.m_GroupState.m_Shapes[i] = physicsShape2D;
					}
				}
			}
		}

		public void GetShapeData(List<PhysicsShape2D> shapes, List<Vector2> vertices)
		{
			shapes.AddRange(this.groupShapes);
			vertices.AddRange(this.groupVertices);
		}

		public void GetShapeData(NativeArray<PhysicsShape2D> shapes, NativeArray<Vector2> vertices)
		{
			bool flag = !shapes.IsCreated || shapes.Length != this.shapeCount;
			if (flag)
			{
				throw new ArgumentException(string.Format("Cannot get shape data as the native shapes array length must be identical to the current custom shape count of {0}.", this.shapeCount), "shapes");
			}
			bool flag2 = !vertices.IsCreated || vertices.Length != this.vertexCount;
			if (flag2)
			{
				throw new ArgumentException(string.Format("Cannot get shape data as the native vertices array length must be identical to the current custom vertex count of {0}.", this.shapeCount), "vertices");
			}
			for (int i = 0; i < this.shapeCount; i++)
			{
				shapes[i] = this.m_GroupState.m_Shapes[i];
			}
			for (int j = 0; j < this.vertexCount; j++)
			{
				vertices[j] = this.m_GroupState.m_Vertices[j];
			}
		}

		public void GetShapeVertices(int shapeIndex, List<Vector2> vertices)
		{
			PhysicsShape2D shape = this.GetShape(shapeIndex);
			int vertexCount = shape.vertexCount;
			vertices.Clear();
			bool flag = vertices.Capacity < vertexCount;
			if (flag)
			{
				vertices.Capacity = vertexCount;
			}
			List<Vector2> groupVertices = this.groupVertices;
			int vertexStartIndex = shape.vertexStartIndex;
			for (int i = 0; i < vertexCount; i++)
			{
				vertices.Add(groupVertices[vertexStartIndex++]);
			}
		}

		public Vector2 GetShapeVertex(int shapeIndex, int vertexIndex)
		{
			int num = this.GetShape(shapeIndex).vertexStartIndex + vertexIndex;
			bool flag = num < 0 || num >= this.groupVertices.Count;
			if (flag)
			{
				throw new ArgumentOutOfRangeException(string.Format("Cannot get shape-vertex at index {0}. There are {1} shape-vertices.", num, this.shapeCount));
			}
			return this.groupVertices[num];
		}

		public void SetShapeVertex(int shapeIndex, int vertexIndex, Vector2 vertex)
		{
			int num = this.GetShape(shapeIndex).vertexStartIndex + vertexIndex;
			bool flag = num < 0 || num >= this.groupVertices.Count;
			if (flag)
			{
				throw new ArgumentOutOfRangeException(string.Format("Cannot set shape-vertex at index {0}. There are {1} shape-vertices.", num, this.shapeCount));
			}
			this.groupVertices[num] = vertex;
		}

		public void SetShapeRadius(int shapeIndex, float radius)
		{
			PhysicsShape2D shape = this.GetShape(shapeIndex);
			switch (shape.shapeType)
			{
			case PhysicsShapeType2D.Circle:
			{
				bool flag = radius <= 0f;
				if (flag)
				{
					throw new ArgumentException(string.Format("Circle radius {0} must be greater than zero.", radius));
				}
				break;
			}
			case PhysicsShapeType2D.Capsule:
			{
				bool flag2 = radius <= 1E-05f;
				if (flag2)
				{
					throw new ArgumentException(string.Format("Capsule radius: {0} is too small.", radius));
				}
				break;
			}
			case PhysicsShapeType2D.Polygon:
			case PhysicsShapeType2D.Edges:
				radius = Mathf.Max(0f, radius);
				break;
			}
			shape.radius = radius;
			this.groupShapes[shapeIndex] = shape;
		}

		public void SetShapeAdjacentVertices(int shapeIndex, bool useAdjacentStart, bool useAdjacentEnd, Vector2 adjacentStart, Vector2 adjacentEnd)
		{
			bool flag = shapeIndex < 0 || shapeIndex >= this.shapeCount;
			if (flag)
			{
				throw new ArgumentOutOfRangeException(string.Format("Cannot set shape adjacent vertices at index {0}. There are {1} shapes(s).", shapeIndex, this.shapeCount));
			}
			PhysicsShape2D physicsShape2D = this.groupShapes[shapeIndex];
			bool flag2 = physicsShape2D.shapeType != PhysicsShapeType2D.Edges;
			if (flag2)
			{
				throw new InvalidOperationException(string.Format("Cannot set shape adjacent vertices at index {0}. The shape must be of type {1} but it is of typee {2}.", shapeIndex, PhysicsShapeType2D.Edges, physicsShape2D.shapeType));
			}
			physicsShape2D.useAdjacentStart = useAdjacentStart;
			physicsShape2D.useAdjacentEnd = useAdjacentEnd;
			physicsShape2D.adjacentStart = adjacentStart;
			physicsShape2D.adjacentEnd = adjacentEnd;
			this.groupShapes[shapeIndex] = physicsShape2D;
		}

		public void DeleteShape(int shapeIndex)
		{
			bool flag = shapeIndex < 0 || shapeIndex >= this.shapeCount;
			if (flag)
			{
				throw new ArgumentOutOfRangeException(string.Format("Cannot delete shape at index {0}. There are {1} shapes(s).", shapeIndex, this.shapeCount));
			}
			PhysicsShape2D physicsShape2D = this.groupShapes[shapeIndex];
			int vertexCount = physicsShape2D.vertexCount;
			this.groupShapes.RemoveAt(shapeIndex);
			this.groupVertices.RemoveRange(physicsShape2D.vertexStartIndex, vertexCount);
			while (shapeIndex < this.groupShapes.Count)
			{
				PhysicsShape2D physicsShape2D2 = this.m_GroupState.m_Shapes[shapeIndex];
				physicsShape2D2.vertexStartIndex -= vertexCount;
				this.m_GroupState.m_Shapes[shapeIndex++] = physicsShape2D2;
			}
		}

		public PhysicsShape2D GetShape(int shapeIndex)
		{
			bool flag = shapeIndex < 0 || shapeIndex >= this.shapeCount;
			if (flag)
			{
				throw new ArgumentOutOfRangeException(string.Format("Cannot get shape at index {0}. There are {1} shapes(s).", shapeIndex, this.shapeCount));
			}
			return this.groupShapes[shapeIndex];
		}

		public int AddCircle(Vector2 center, float radius)
		{
			bool flag = radius <= 0f;
			if (flag)
			{
				throw new ArgumentException(string.Format("radius {0} must be greater than zero.", radius));
			}
			int count = this.groupVertices.Count;
			this.groupVertices.Add(center);
			this.groupShapes.Add(new PhysicsShape2D
			{
				shapeType = PhysicsShapeType2D.Circle,
				radius = radius,
				vertexStartIndex = count,
				vertexCount = 1
			});
			return this.groupShapes.Count - 1;
		}

		public int AddCapsule(Vector2 vertex0, Vector2 vertex1, float radius)
		{
			bool flag = radius <= 1E-05f;
			if (flag)
			{
				throw new ArgumentException(string.Format("radius: {0} is too small.", radius));
			}
			int count = this.groupVertices.Count;
			this.groupVertices.Add(vertex0);
			this.groupVertices.Add(vertex1);
			this.groupShapes.Add(new PhysicsShape2D
			{
				shapeType = PhysicsShapeType2D.Capsule,
				radius = radius,
				vertexStartIndex = count,
				vertexCount = 2
			});
			return this.groupShapes.Count - 1;
		}

		public int AddBox(Vector2 center, Vector2 size, [DefaultValue("0f")] float angle = 0f, [DefaultValue("0f")] float edgeRadius = 0f)
		{
			bool flag = size.x <= 0.0025f || size.y <= 0.0025f;
			if (flag)
			{
				throw new ArgumentException(string.Format("size: {0} is too small. Vertex need to be separated by at least {1}", size, 0.0025f));
			}
			edgeRadius = Mathf.Max(0f, edgeRadius);
			angle *= 0.017453292f;
			float num = Mathf.Cos(angle);
			float num2 = Mathf.Sin(angle);
			Vector2 vector = size * 0.5f;
			Vector2 vector2 = center + PhysicsShapeGroup2D.<AddBox>g__Rotate|28_0(num, num2, -vector);
			Vector2 vector3 = center + PhysicsShapeGroup2D.<AddBox>g__Rotate|28_0(num, num2, new Vector2(vector.x, -vector.y));
			Vector2 vector4 = center + PhysicsShapeGroup2D.<AddBox>g__Rotate|28_0(num, num2, vector);
			Vector2 vector5 = center + PhysicsShapeGroup2D.<AddBox>g__Rotate|28_0(num, num2, new Vector2(-vector.x, vector.y));
			int count = this.groupVertices.Count;
			this.groupVertices.Add(vector2);
			this.groupVertices.Add(vector3);
			this.groupVertices.Add(vector4);
			this.groupVertices.Add(vector5);
			this.groupShapes.Add(new PhysicsShape2D
			{
				shapeType = PhysicsShapeType2D.Polygon,
				radius = edgeRadius,
				vertexStartIndex = count,
				vertexCount = 4
			});
			return this.groupShapes.Count - 1;
		}

		public int AddPolygon(List<Vector2> vertices)
		{
			int count = vertices.Count;
			bool flag = count < 3 || count > 8;
			if (flag)
			{
				throw new ArgumentException(string.Format("Vertex Count {0} must be >= 3 and <= {1}.", count, 8));
			}
			float num = 6.25E-06f;
			for (int i = 1; i < count; i++)
			{
				Vector2 vector = vertices[i - 1];
				Vector2 vector2 = vertices[i];
				bool flag2 = (vector2 - vector).sqrMagnitude <= num;
				if (flag2)
				{
					throw new ArgumentException(string.Format("vertices: {0} and {1} are too close. Vertices need to be separated by at least {2}", vector, vector2, num));
				}
			}
			int count2 = this.groupVertices.Count;
			this.groupVertices.AddRange(vertices);
			this.groupShapes.Add(new PhysicsShape2D
			{
				shapeType = PhysicsShapeType2D.Polygon,
				radius = 0f,
				vertexStartIndex = count2,
				vertexCount = count
			});
			return this.groupShapes.Count - 1;
		}

		public int AddEdges(List<Vector2> vertices, [DefaultValue("0f")] float edgeRadius = 0f)
		{
			return this.AddEdges(vertices, false, false, Vector2.zero, Vector2.zero, edgeRadius);
		}

		public int AddEdges(List<Vector2> vertices, bool useAdjacentStart, bool useAdjacentEnd, Vector2 adjacentStart, Vector2 adjacentEnd, [DefaultValue("0f")] float edgeRadius = 0f)
		{
			int count = vertices.Count;
			bool flag = count < 2;
			if (flag)
			{
				throw new ArgumentOutOfRangeException(string.Format("Vertex Count {0} must be >= 2.", count));
			}
			edgeRadius = Mathf.Max(0f, edgeRadius);
			int count2 = this.groupVertices.Count;
			this.groupVertices.AddRange(vertices);
			this.groupShapes.Add(new PhysicsShape2D
			{
				shapeType = PhysicsShapeType2D.Edges,
				radius = edgeRadius,
				vertexStartIndex = count2,
				vertexCount = count,
				useAdjacentStart = useAdjacentStart,
				useAdjacentEnd = useAdjacentEnd,
				adjacentStart = adjacentStart,
				adjacentEnd = adjacentEnd
			});
			return this.groupShapes.Count - 1;
		}

		[CompilerGenerated]
		internal static Vector2 <AddBox>g__Rotate|28_0(float cos, float sin, Vector2 value)
		{
			return new Vector2(cos * value.x - sin * value.y, sin * value.x + cos * value.y);
		}

		internal PhysicsShapeGroup2D.GroupState m_GroupState;

		private const float MinVertexSeparation = 0.0025f;

		[NativeHeader(Header = "Modules/Physics2D/Public/PhysicsScripting2D.h")]
		internal struct GroupState
		{
			public void ClearGeometry()
			{
				this.m_Shapes.Clear();
				this.m_Vertices.Clear();
			}

			[NativeName("shapesList")]
			public List<PhysicsShape2D> m_Shapes;

			[NativeName("verticesList")]
			public List<Vector2> m_Vertices;

			[NativeName("localToWorld")]
			public Matrix4x4 m_LocalToWorld;
		}
	}
}
