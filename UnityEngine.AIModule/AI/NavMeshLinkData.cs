using System;

namespace UnityEngine.AI
{
	/// <summary>
	///   <para>Used for runtime manipulation of links connecting polygons of the NavMesh.</para>
	/// </summary>
	public struct NavMeshLinkData
	{
		/// <summary>
		///   <para>Start position of the link.</para>
		/// </summary>
		public Vector3 startPosition
		{
			get
			{
				return this.m_StartPosition;
			}
			set
			{
				this.m_StartPosition = value;
			}
		}

		/// <summary>
		///   <para>End position of the link.</para>
		/// </summary>
		public Vector3 endPosition
		{
			get
			{
				return this.m_EndPosition;
			}
			set
			{
				this.m_EndPosition = value;
			}
		}

		/// <summary>
		///   <para>If positive, overrides the pathfinder cost to traverse the link.</para>
		/// </summary>
		public float costModifier
		{
			get
			{
				return this.m_CostModifier;
			}
			set
			{
				this.m_CostModifier = value;
			}
		}

		/// <summary>
		///   <para>If true, the link can be traversed in both directions, otherwise only from start to end position.</para>
		/// </summary>
		public bool bidirectional
		{
			get
			{
				return this.m_Bidirectional != 0;
			}
			set
			{
				this.m_Bidirectional = ((!value) ? 0 : 1);
			}
		}

		/// <summary>
		///   <para>If positive, the link will be rectangle aligned along the line from start to end.</para>
		/// </summary>
		public float width
		{
			get
			{
				return this.m_Width;
			}
			set
			{
				this.m_Width = value;
			}
		}

		/// <summary>
		///   <para>Area type of the link.</para>
		/// </summary>
		public int area
		{
			get
			{
				return this.m_Area;
			}
			set
			{
				this.m_Area = value;
			}
		}

		/// <summary>
		///   <para>Specifies which agent type this link is available for.</para>
		/// </summary>
		public int agentTypeID
		{
			get
			{
				return this.m_AgentTypeID;
			}
			set
			{
				this.m_AgentTypeID = value;
			}
		}

		private Vector3 m_StartPosition;

		private Vector3 m_EndPosition;

		private float m_CostModifier;

		private int m_Bidirectional;

		private float m_Width;

		private int m_Area;

		private int m_AgentTypeID;
	}
}
