using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	[NativeHeader(Header = "Modules/Physics2D/Public/PhysicsScripting2D.h")]
	public struct PhysicsShape2D
	{
		public PhysicsShapeType2D shapeType
		{
			get
			{
				return this.m_ShapeType;
			}
			set
			{
				this.m_ShapeType = value;
			}
		}

		public float radius
		{
			get
			{
				return this.m_Radius;
			}
			set
			{
				bool flag = value < 0f;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("radius cannot be negative.");
				}
				bool flag2 = float.IsNaN(value) || float.IsInfinity(value);
				if (flag2)
				{
					throw new ArgumentException("radius contains an invalid value.");
				}
				this.m_Radius = value;
			}
		}

		public int vertexStartIndex
		{
			get
			{
				return this.m_VertexStartIndex;
			}
			set
			{
				bool flag = value < 0;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("vertexStartIndex cannot be negative.");
				}
				this.m_VertexStartIndex = value;
			}
		}

		public int vertexCount
		{
			get
			{
				return this.m_VertexCount;
			}
			set
			{
				bool flag = value < 1;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("vertexCount cannot be less than one.");
				}
				this.m_VertexCount = value;
			}
		}

		public bool useAdjacentStart
		{
			get
			{
				return this.m_UseAdjacentStart != 0;
			}
			set
			{
				this.m_UseAdjacentStart = (value ? 1 : 0);
			}
		}

		public bool useAdjacentEnd
		{
			get
			{
				return this.m_UseAdjacentEnd != 0;
			}
			set
			{
				this.m_UseAdjacentEnd = (value ? 1 : 0);
			}
		}

		public Vector2 adjacentStart
		{
			get
			{
				return this.m_AdjacentStart;
			}
			set
			{
				bool flag = float.IsNaN(value.x) || float.IsNaN(value.y) || float.IsInfinity(value.x) || float.IsInfinity(value.y);
				if (flag)
				{
					throw new ArgumentException("adjacentStart contains an invalid value.");
				}
				this.m_AdjacentStart = value;
			}
		}

		public Vector2 adjacentEnd
		{
			get
			{
				return this.m_AdjacentEnd;
			}
			set
			{
				bool flag = float.IsNaN(value.x) || float.IsNaN(value.y) || float.IsInfinity(value.x) || float.IsInfinity(value.y);
				if (flag)
				{
					throw new ArgumentException("adjacentEnd contains an invalid value.");
				}
				this.m_AdjacentEnd = value;
			}
		}

		private PhysicsShapeType2D m_ShapeType;

		private float m_Radius;

		private int m_VertexStartIndex;

		private int m_VertexCount;

		private int m_UseAdjacentStart;

		private int m_UseAdjacentEnd;

		private Vector2 m_AdjacentStart;

		private Vector2 m_AdjacentEnd;
	}
}
