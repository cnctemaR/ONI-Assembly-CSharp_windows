using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>A set of options that control how physics operates when using the job system to multithread the physics simulation.</para>
	/// </summary>
	[RequiredByNativeCode(Optional = true, GenerateProxy = true)]
	[NativeHeader("Modules/Physics2D/Public/Physics2DSettings.h")]
	[NativeClass("PhysicsJobOptions2D", "struct PhysicsJobOptions2D;")]
	public struct PhysicsJobOptions2D
	{
		/// <summary>
		///   <para>Should physics simulation use multithreading?</para>
		/// </summary>
		public bool useMultithreading
		{
			get
			{
				return this.m_UseMultithreading;
			}
			set
			{
				this.m_UseMultithreading = value;
			}
		}

		/// <summary>
		///   <para>Should physics simulation sort multi-threaded results to maintain processing order consistency?</para>
		/// </summary>
		public bool useConsistencySorting
		{
			get
			{
				return this.m_UseConsistencySorting;
			}
			set
			{
				this.m_UseConsistencySorting = value;
			}
		}

		/// <summary>
		///   <para>Controls the minimum number of Rigidbody2D being interpolated in each simulation job.</para>
		/// </summary>
		public int interpolationPosesPerJob
		{
			get
			{
				return this.m_InterpolationPosesPerJob;
			}
			set
			{
				this.m_InterpolationPosesPerJob = value;
			}
		}

		/// <summary>
		///   <para>Controls the minimum number of new contacts to find in each simulation job.</para>
		/// </summary>
		public int newContactsPerJob
		{
			get
			{
				return this.m_NewContactsPerJob;
			}
			set
			{
				this.m_NewContactsPerJob = value;
			}
		}

		/// <summary>
		///   <para>Controls the minimum number of contacts to collide in each simulation job.</para>
		/// </summary>
		public int collideContactsPerJob
		{
			get
			{
				return this.m_CollideContactsPerJob;
			}
			set
			{
				this.m_CollideContactsPerJob = value;
			}
		}

		/// <summary>
		///   <para>Controls the minimum number of flags to be cleared in each simulation job.</para>
		/// </summary>
		public int clearFlagsPerJob
		{
			get
			{
				return this.m_ClearFlagsPerJob;
			}
			set
			{
				this.m_ClearFlagsPerJob = value;
			}
		}

		/// <summary>
		///   <para>Controls the minimum number of bodies to be cleared in each simulation job.</para>
		/// </summary>
		public int clearBodyForcesPerJob
		{
			get
			{
				return this.m_ClearBodyForcesPerJob;
			}
			set
			{
				this.m_ClearBodyForcesPerJob = value;
			}
		}

		/// <summary>
		///   <para>Controls the minimum number of fixtures to synchronize in the broadphase during discrete island solving in each simulation job.</para>
		/// </summary>
		public int syncDiscreteFixturesPerJob
		{
			get
			{
				return this.m_SyncDiscreteFixturesPerJob;
			}
			set
			{
				this.m_SyncDiscreteFixturesPerJob = value;
			}
		}

		/// <summary>
		///   <para>Controls the minimum number of fixtures to synchronize in the broadphase during continuous island solving in each simulation job.</para>
		/// </summary>
		public int syncContinuousFixturesPerJob
		{
			get
			{
				return this.m_SyncContinuousFixturesPerJob;
			}
			set
			{
				this.m_SyncContinuousFixturesPerJob = value;
			}
		}

		/// <summary>
		///   <para>Controls the minimum number of nearest contacts to find in each simulation job.</para>
		/// </summary>
		public int findNearestContactsPerJob
		{
			get
			{
				return this.m_FindNearestContactsPerJob;
			}
			set
			{
				this.m_FindNearestContactsPerJob = value;
			}
		}

		/// <summary>
		///   <para>Controls the minimum number of trigger contacts to update in each simulation job.</para>
		/// </summary>
		public int updateTriggerContactsPerJob
		{
			get
			{
				return this.m_UpdateTriggerContactsPerJob;
			}
			set
			{
				this.m_UpdateTriggerContactsPerJob = value;
			}
		}

		/// <summary>
		///   <para>The minimum threshold cost of all bodies, contacts and joints in an island during discrete island solving.</para>
		/// </summary>
		public int islandSolverCostThreshold
		{
			get
			{
				return this.m_IslandSolverCostThreshold;
			}
			set
			{
				this.m_IslandSolverCostThreshold = value;
			}
		}

		/// <summary>
		///   <para>Scales the cost of each body during discrete island solving.</para>
		/// </summary>
		public int islandSolverBodyCostScale
		{
			get
			{
				return this.m_IslandSolverBodyCostScale;
			}
			set
			{
				this.m_IslandSolverBodyCostScale = value;
			}
		}

		/// <summary>
		///   <para>Scales the cost of each contact during discrete island solving.</para>
		/// </summary>
		public int islandSolverContactCostScale
		{
			get
			{
				return this.m_IslandSolverContactCostScale;
			}
			set
			{
				this.m_IslandSolverContactCostScale = value;
			}
		}

		/// <summary>
		///   <para>Scales the cost of each joint during discrete island solving.</para>
		/// </summary>
		public int islandSolverJointCostScale
		{
			get
			{
				return this.m_IslandSolverJointCostScale;
			}
			set
			{
				this.m_IslandSolverJointCostScale = value;
			}
		}

		/// <summary>
		///   <para>Controls the minimum number of bodies to solve in each simulation job when performing island solving.</para>
		/// </summary>
		public int islandSolverBodiesPerJob
		{
			get
			{
				return this.m_IslandSolverBodiesPerJob;
			}
			set
			{
				this.m_IslandSolverBodiesPerJob = value;
			}
		}

		/// <summary>
		///   <para>Controls the minimum number of contacts to solve in each simulation job when performing island solving.</para>
		/// </summary>
		public int islandSolverContactsPerJob
		{
			get
			{
				return this.m_IslandSolverContactsPerJob;
			}
			set
			{
				this.m_IslandSolverContactsPerJob = value;
			}
		}

		private bool m_UseMultithreading;

		private bool m_UseConsistencySorting;

		private int m_InterpolationPosesPerJob;

		private int m_NewContactsPerJob;

		private int m_CollideContactsPerJob;

		private int m_ClearFlagsPerJob;

		private int m_ClearBodyForcesPerJob;

		private int m_SyncDiscreteFixturesPerJob;

		private int m_SyncContinuousFixturesPerJob;

		private int m_FindNearestContactsPerJob;

		private int m_UpdateTriggerContactsPerJob;

		private int m_IslandSolverCostThreshold;

		private int m_IslandSolverBodyCostScale;

		private int m_IslandSolverContactCostScale;

		private int m_IslandSolverJointCostScale;

		private int m_IslandSolverBodiesPerJob;

		private int m_IslandSolverContactsPerJob;
	}
}
