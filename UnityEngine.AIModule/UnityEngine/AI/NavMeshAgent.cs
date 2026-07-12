using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.AI
{
	[MovedFrom("UnityEngine")]
	[NativeHeader("Modules/AI/Components/NavMeshAgent.bindings.h")]
	[NativeHeader("Modules/AI/NavMesh/NavMesh.bindings.h")]
	public sealed class NavMeshAgent : Behaviour
	{
		public bool SetDestination(Vector3 target)
		{
			return this.SetDestination_Injected(ref target);
		}

		public Vector3 destination
		{
			get
			{
				Vector3 vector;
				this.get_destination_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_destination_Injected(ref value);
			}
		}

		public extern float stoppingDistance
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Vector3 velocity
		{
			get
			{
				Vector3 vector;
				this.get_velocity_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_velocity_Injected(ref value);
			}
		}

		[NativeProperty("Position")]
		public Vector3 nextPosition
		{
			get
			{
				Vector3 vector;
				this.get_nextPosition_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_nextPosition_Injected(ref value);
			}
		}

		public Vector3 steeringTarget
		{
			get
			{
				Vector3 vector;
				this.get_steeringTarget_Injected(out vector);
				return vector;
			}
		}

		public Vector3 desiredVelocity
		{
			get
			{
				Vector3 vector;
				this.get_desiredVelocity_Injected(out vector);
				return vector;
			}
		}

		public extern float remainingDistance
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float baseOffset
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool isOnOffMeshLink
		{
			[NativeName("IsOnOffMeshLink")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ActivateCurrentOffMeshLink(bool activated);

		public OffMeshLinkData currentOffMeshLinkData
		{
			get
			{
				return this.GetCurrentOffMeshLinkDataInternal();
			}
		}

		[FreeFunction("NavMeshAgentScriptBindings::GetCurrentOffMeshLinkDataInternal", HasExplicitThis = true)]
		internal OffMeshLinkData GetCurrentOffMeshLinkDataInternal()
		{
			OffMeshLinkData offMeshLinkData;
			this.GetCurrentOffMeshLinkDataInternal_Injected(out offMeshLinkData);
			return offMeshLinkData;
		}

		public OffMeshLinkData nextOffMeshLinkData
		{
			get
			{
				return this.GetNextOffMeshLinkDataInternal();
			}
		}

		[FreeFunction("NavMeshAgentScriptBindings::GetNextOffMeshLinkDataInternal", HasExplicitThis = true)]
		internal OffMeshLinkData GetNextOffMeshLinkDataInternal()
		{
			OffMeshLinkData offMeshLinkData;
			this.GetNextOffMeshLinkDataInternal_Injected(out offMeshLinkData);
			return offMeshLinkData;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void CompleteOffMeshLink();

		public extern bool autoTraverseOffMeshLink
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool autoBraking
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool autoRepath
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool hasPath
		{
			[NativeName("HasPath")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool pathPending
		{
			[NativeName("PathPending")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isPathStale
		{
			[NativeName("IsPathStale")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern NavMeshPathStatus pathStatus
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[NativeProperty("EndPositionOfCurrentPath")]
		public Vector3 pathEndPosition
		{
			get
			{
				Vector3 vector;
				this.get_pathEndPosition_Injected(out vector);
				return vector;
			}
		}

		public bool Warp(Vector3 newPosition)
		{
			return this.Warp_Injected(ref newPosition);
		}

		public void Move(Vector3 offset)
		{
			this.Move_Injected(ref offset);
		}

		[Obsolete("Set isStopped to true instead.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Stop();

		[Obsolete("Set isStopped to true instead.")]
		public void Stop(bool stopUpdates)
		{
			this.Stop();
		}

		[Obsolete("Set isStopped to false instead.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Resume();

		public extern bool isStopped
		{
			[FreeFunction("NavMeshAgentScriptBindings::GetIsStopped", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[FreeFunction("NavMeshAgentScriptBindings::SetIsStopped", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ResetPath();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool SetPath([NotNull("ArgumentNullException")] NavMeshPath path);

		public NavMeshPath path
		{
			get
			{
				NavMeshPath navMeshPath = new NavMeshPath();
				this.CopyPathTo(navMeshPath);
				return navMeshPath;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new NullReferenceException();
				}
				this.SetPath(value);
			}
		}

		[NativeMethod("CopyPath")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void CopyPathTo([NotNull("ArgumentNullException")] NavMeshPath path);

		[NativeName("DistanceToEdge")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool FindClosestEdge(out NavMeshHit hit);

		public bool Raycast(Vector3 targetPosition, out NavMeshHit hit)
		{
			return this.Raycast_Injected(ref targetPosition, out hit);
		}

		public bool CalculatePath(Vector3 targetPosition, NavMeshPath path)
		{
			path.ClearCorners();
			return this.CalculatePathInternal(targetPosition, path);
		}

		[FreeFunction("NavMeshAgentScriptBindings::CalculatePathInternal", HasExplicitThis = true)]
		private bool CalculatePathInternal(Vector3 targetPosition, [NotNull("ArgumentNullException")] NavMeshPath path)
		{
			return this.CalculatePathInternal_Injected(ref targetPosition, path);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool SamplePathPosition(int areaMask, float maxDistance, out NavMeshHit hit);

		[Obsolete("Use SetAreaCost instead.")]
		[NativeMethod("SetAreaCost")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetLayerCost(int layer, float cost);

		[Obsolete("Use GetAreaCost instead.")]
		[NativeMethod("GetAreaCost")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetLayerCost(int layer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetAreaCost(int areaIndex, float areaCost);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetAreaCost(int areaIndex);

		public Object navMeshOwner
		{
			get
			{
				return this.GetOwnerInternal();
			}
		}

		public extern int agentTypeID
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeName("GetCurrentPolygonOwner")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Object GetOwnerInternal();

		[Obsolete("Use areaMask instead.")]
		public int walkableMask
		{
			get
			{
				return this.areaMask;
			}
			set
			{
				this.areaMask = value;
			}
		}

		public extern int areaMask
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float speed
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float angularSpeed
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float acceleration
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool updatePosition
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool updateRotation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool updateUpAxis
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float radius
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float height
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ObstacleAvoidanceType obstacleAvoidanceType
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int avoidancePriority
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool isOnNavMesh
		{
			[NativeName("InCrowdSystem")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool SetDestination_Injected(ref Vector3 target);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_destination_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_destination_Injected(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_velocity_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_velocity_Injected(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_nextPosition_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_nextPosition_Injected(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_steeringTarget_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_desiredVelocity_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetCurrentOffMeshLinkDataInternal_Injected(out OffMeshLinkData ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetNextOffMeshLinkDataInternal_Injected(out OffMeshLinkData ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_pathEndPosition_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool Warp_Injected(ref Vector3 newPosition);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Move_Injected(ref Vector3 offset);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool Raycast_Injected(ref Vector3 targetPosition, out NavMeshHit hit);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool CalculatePathInternal_Injected(ref Vector3 targetPosition, NavMeshPath path);
	}
}
