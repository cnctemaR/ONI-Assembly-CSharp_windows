using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.AI
{
	/// <summary>
	///   <para>The NavMeshBuildSettings struct allows you to specify a collection of settings which describe the dimensions and limitations of a particular agent type.</para>
	/// </summary>
	[NativeHeader("Modules/AI/Public/NavMeshBuildSettings.h")]
	public struct NavMeshBuildSettings
	{
		/// <summary>
		///   <para>The agent type ID the NavMesh will be baked for.</para>
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

		/// <summary>
		///   <para>The radius of the agent for baking in world units.</para>
		/// </summary>
		public float agentRadius
		{
			get
			{
				return this.m_AgentRadius;
			}
			set
			{
				this.m_AgentRadius = value;
			}
		}

		/// <summary>
		///   <para>The height of the agent for baking in world units.</para>
		/// </summary>
		public float agentHeight
		{
			get
			{
				return this.m_AgentHeight;
			}
			set
			{
				this.m_AgentHeight = value;
			}
		}

		/// <summary>
		///   <para>The maximum slope angle which is walkable (angle in degrees).</para>
		/// </summary>
		public float agentSlope
		{
			get
			{
				return this.m_AgentSlope;
			}
			set
			{
				this.m_AgentSlope = value;
			}
		}

		/// <summary>
		///   <para>The maximum vertical step size an agent can take.</para>
		/// </summary>
		public float agentClimb
		{
			get
			{
				return this.m_AgentClimb;
			}
			set
			{
				this.m_AgentClimb = value;
			}
		}

		/// <summary>
		///   <para>The approximate minimum area of individual NavMesh regions.</para>
		/// </summary>
		public float minRegionArea
		{
			get
			{
				return this.m_MinRegionArea;
			}
			set
			{
				this.m_MinRegionArea = value;
			}
		}

		/// <summary>
		///   <para>Enables overriding the default voxel size. See Also: voxelSize.</para>
		/// </summary>
		public bool overrideVoxelSize
		{
			get
			{
				return this.m_OverrideVoxelSize != 0;
			}
			set
			{
				this.m_OverrideVoxelSize = ((!value) ? 0 : 1);
			}
		}

		/// <summary>
		///   <para>Sets the voxel size in world length units.</para>
		/// </summary>
		public float voxelSize
		{
			get
			{
				return this.m_VoxelSize;
			}
			set
			{
				this.m_VoxelSize = value;
			}
		}

		/// <summary>
		///   <para>Enables overriding the default tile size. See Also: tileSize.</para>
		/// </summary>
		public bool overrideTileSize
		{
			get
			{
				return this.m_OverrideTileSize != 0;
			}
			set
			{
				this.m_OverrideTileSize = ((!value) ? 0 : 1);
			}
		}

		/// <summary>
		///   <para>Sets the tile size in voxel units.</para>
		/// </summary>
		public int tileSize
		{
			get
			{
				return this.m_TileSize;
			}
			set
			{
				this.m_TileSize = value;
			}
		}

		/// <summary>
		///   <para>Options for collecting debug data during the build process.</para>
		/// </summary>
		public NavMeshBuildDebugSettings debug
		{
			get
			{
				return this.m_Debug;
			}
			set
			{
				this.m_Debug = value;
			}
		}

		/// <summary>
		///   <para>Validates the properties of NavMeshBuildSettings.</para>
		/// </summary>
		/// <param name="buildBounds">Describes the volume to build NavMesh for.</param>
		/// <returns>
		///   <para>The list of violated constraints.</para>
		/// </returns>
		public string[] ValidationReport(Bounds buildBounds)
		{
			return NavMeshBuildSettings.InternalValidationReport(this, buildBounds);
		}

		[FreeFunction]
		[NativeHeader("Modules/AI/Public/NavMeshBuildSettings.h")]
		private static string[] InternalValidationReport(NavMeshBuildSettings buildSettings, Bounds buildBounds)
		{
			return NavMeshBuildSettings.InternalValidationReport_Injected(ref buildSettings, ref buildBounds);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string[] InternalValidationReport_Injected(ref NavMeshBuildSettings buildSettings, ref Bounds buildBounds);

		private int m_AgentTypeID;

		private float m_AgentRadius;

		private float m_AgentHeight;

		private float m_AgentSlope;

		private float m_AgentClimb;

		private float m_LedgeDropHeight;

		private float m_MaxJumpAcrossDistance;

		private float m_MinRegionArea;

		private int m_OverrideVoxelSize;

		private float m_VoxelSize;

		private int m_OverrideTileSize;

		private int m_TileSize;

		private int m_AccuratePlacement;

		private NavMeshBuildDebugSettings m_Debug;
	}
}
