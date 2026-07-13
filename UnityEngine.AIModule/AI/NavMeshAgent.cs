using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.AI
{
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.ai.navigation@2.0/manual/NavMeshAgent.html")]
	[NativeHeader("Modules/AI/NavMesh/NavMesh.bindings.h")]
	[NativeHeader("Modules/AI/Components/NavMeshAgent.bindings.h")]
	[MovedFrom("UnityEngine")]
	public sealed class NavMeshAgent : Behaviour
	{
		public bool SetDestination(Vector3 target)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return NavMeshAgent.SetDestination_Injected(intPtr, ref target);
		}

		public Vector3 destination
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				NavMeshAgent.get_destination_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				NavMeshAgent.set_destination_Injected(intPtr, ref value);
			}
		}

		public float stoppingDistance
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return NavMeshAgent.get_stoppingDistance_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				NavMeshAgent.set_stoppingDistance_Injected(intPtr, value);
			}
		}

		public Vector3 velocity
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				NavMeshAgent.get_velocity_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				NavMeshAgent.set_velocity_Injected(intPtr, ref value);
			}
		}

		[NativeProperty("Position")]
		public Vector3 nextPosition
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				NavMeshAgent.get_nextPosition_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				NavMeshAgent.set_nextPosition_Injected(intPtr, ref value);
			}
		}

		public Vector3 steeringTarget
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				NavMeshAgent.get_steeringTarget_Injected(intPtr, out vector);
				return vector;
			}
		}

		public Vector3 desiredVelocity
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				NavMeshAgent.get_desiredVelocity_Injected(intPtr, out vector);
				return vector;
			}
		}

		public float remainingDistance
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return NavMeshAgent.get_remainingDistance_Injected(intPtr);
			}
		}

		public float baseOffset
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return NavMeshAgent.get_baseOffset_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				NavMeshAgent.set_baseOffset_Injected(intPtr, value);
			}
		}

		public bool isOnOffMeshLink
		{
			[NativeName("IsOnOffMeshLink")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return NavMeshAgent.get_isOnOffMeshLink_Injected(intPtr);
			}
		}

		public void ActivateCurrentOffMeshLink(bool activated)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			NavMeshAgent.ActivateCurrentOffMeshLink_Injected(intPtr, activated);
		}

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
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			OffMeshLinkData offMeshLinkData;
			NavMeshAgent.GetCurrentOffMeshLinkDataInternal_Injected(intPtr, out offMeshLinkData);
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
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			OffMeshLinkData offMeshLinkData;
			NavMeshAgent.GetNextOffMeshLinkDataInternal_Injected(intPtr, out offMeshLinkData);
			return offMeshLinkData;
		}

		public void CompleteOffMeshLink()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			NavMeshAgent.CompleteOffMeshLink_Injected(intPtr);
		}

		public bool autoTraverseOffMeshLink
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return NavMeshAgent.get_autoTraverseOffMeshLink_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				NavMeshAgent.set_autoTraverseOffMeshLink_Injected(intPtr, value);
			}
		}

		public bool autoBraking
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return NavMeshAgent.get_autoBraking_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				NavMeshAgent.set_autoBraking_Injected(intPtr, value);
			}
		}

		public bool autoRepath
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return NavMeshAgent.get_autoRepath_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				NavMeshAgent.set_autoRepath_Injected(intPtr, value);
			}
		}

		public bool hasPath
		{
			[NativeName("HasPath")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return NavMeshAgent.get_hasPath_Injected(intPtr);
			}
		}

		public bool pathPending
		{
			[NativeName("PathPending")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return NavMeshAgent.get_pathPending_Injected(intPtr);
			}
		}

		public bool isPathStale
		{
			[NativeName("IsPathStale")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return NavMeshAgent.get_isPathStale_Injected(intPtr);
			}
		}

		public NavMeshPathStatus pathStatus
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return NavMeshAgent.get_pathStatus_Injected(intPtr);
			}
		}

		[NativeProperty("EndPositionOfCurrentPath")]
		public Vector3 pathEndPosition
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				NavMeshAgent.get_pathEndPosition_Injected(intPtr, out vector);
				return vector;
			}
		}

		public bool Warp(Vector3 newPosition)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return NavMeshAgent.Warp_Injected(intPtr, ref newPosition);
		}

		public void Move(Vector3 offset)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			NavMeshAgent.Move_Injected(intPtr, ref offset);
		}

		[Obsolete("Set isStopped to true instead.")]
		public void Stop()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			NavMeshAgent.Stop_Injected(intPtr);
		}

		[Obsolete("Set isStopped to true instead.")]
		public void Stop(bool stopUpdates)
		{
			this.Stop();
		}

		[Obsolete("Set isStopped to false instead.")]
		public void Resume()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			NavMeshAgent.Resume_Injected(intPtr);
		}

		public bool isStopped
		{
			[FreeFunction("NavMeshAgentScriptBindings::GetIsStopped", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return NavMeshAgent.get_isStopped_Injected(intPtr);
			}
			[FreeFunction("NavMeshAgentScriptBindings::SetIsStopped", HasExplicitThis = true)]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				NavMeshAgent.set_isStopped_Injected(intPtr, value);
			}
		}

		public void ResetPath()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			NavMeshAgent.ResetPath_Injected(intPtr);
		}

		public bool SetPath([NotNull] NavMeshPath path)
		{
			if (path == null)
			{
				ThrowHelper.ThrowArgumentNullException(path, "path");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = NavMeshPath.BindingsMarshaller.ConvertToNative(path);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(path, "path");
			}
			return NavMeshAgent.SetPath_Injected(intPtr, intPtr2);
		}

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
		internal void CopyPathTo([NotNull] NavMeshPath path)
		{
			if (path == null)
			{
				ThrowHelper.ThrowArgumentNullException(path, "path");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = NavMeshPath.BindingsMarshaller.ConvertToNative(path);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(path, "path");
			}
			NavMeshAgent.CopyPathTo_Injected(intPtr, intPtr2);
		}

		[NativeName("DistanceToEdge")]
		public bool FindClosestEdge(out NavMeshHit hit)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return NavMeshAgent.FindClosestEdge_Injected(intPtr, out hit);
		}

		public bool Raycast(Vector3 targetPosition, out NavMeshHit hit)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return NavMeshAgent.Raycast_Injected(intPtr, ref targetPosition, out hit);
		}

		public bool CalculatePath(Vector3 targetPosition, NavMeshPath path)
		{
			path.ClearCorners();
			return this.CalculatePathInternal(targetPosition, path);
		}

		[FreeFunction("NavMeshAgentScriptBindings::CalculatePathInternal", HasExplicitThis = true)]
		private bool CalculatePathInternal(Vector3 targetPosition, [NotNull] NavMeshPath path)
		{
			if (path == null)
			{
				ThrowHelper.ThrowArgumentNullException(path, "path");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = NavMeshPath.BindingsMarshaller.ConvertToNative(path);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(path, "path");
			}
			return NavMeshAgent.CalculatePathInternal_Injected(intPtr, ref targetPosition, intPtr2);
		}

		public bool SamplePathPosition(int areaMask, float maxDistance, out NavMeshHit hit)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return NavMeshAgent.SamplePathPosition_Injected(intPtr, areaMask, maxDistance, out hit);
		}

		[Obsolete("Use SetAreaCost instead.")]
		[NativeMethod("SetAreaCost")]
		public void SetLayerCost(int layer, float cost)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			NavMeshAgent.SetLayerCost_Injected(intPtr, layer, cost);
		}

		[NativeMethod("GetAreaCost")]
		[Obsolete("Use GetAreaCost instead.")]
		public float GetLayerCost(int layer)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return NavMeshAgent.GetLayerCost_Injected(intPtr, layer);
		}

		public void SetAreaCost(int areaIndex, float areaCost)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			NavMeshAgent.SetAreaCost_Injected(intPtr, areaIndex, areaCost);
		}

		public float GetAreaCost(int areaIndex)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return NavMeshAgent.GetAreaCost_Injected(intPtr, areaIndex);
		}

		public Object navMeshOwner
		{
			get
			{
				return this.GetOwnerInternal();
			}
		}

		public int agentTypeID
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return NavMeshAgent.get_agentTypeID_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				NavMeshAgent.set_agentTypeID_Injected(intPtr, value);
			}
		}

		[NativeName("GetCurrentPolygonOwner")]
		private Object GetOwnerInternal()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Object>(NavMeshAgent.GetOwnerInternal_Injected(intPtr));
		}

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

		public int areaMask
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return NavMeshAgent.get_areaMask_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				NavMeshAgent.set_areaMask_Injected(intPtr, value);
			}
		}

		public float speed
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return NavMeshAgent.get_speed_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				NavMeshAgent.set_speed_Injected(intPtr, value);
			}
		}

		public float angularSpeed
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return NavMeshAgent.get_angularSpeed_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				NavMeshAgent.set_angularSpeed_Injected(intPtr, value);
			}
		}

		public float acceleration
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return NavMeshAgent.get_acceleration_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				NavMeshAgent.set_acceleration_Injected(intPtr, value);
			}
		}

		public bool updatePosition
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return NavMeshAgent.get_updatePosition_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				NavMeshAgent.set_updatePosition_Injected(intPtr, value);
			}
		}

		public bool updateRotation
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return NavMeshAgent.get_updateRotation_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				NavMeshAgent.set_updateRotation_Injected(intPtr, value);
			}
		}

		public bool updateUpAxis
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return NavMeshAgent.get_updateUpAxis_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				NavMeshAgent.set_updateUpAxis_Injected(intPtr, value);
			}
		}

		public float radius
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return NavMeshAgent.get_radius_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				NavMeshAgent.set_radius_Injected(intPtr, value);
			}
		}

		public float height
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return NavMeshAgent.get_height_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				NavMeshAgent.set_height_Injected(intPtr, value);
			}
		}

		public ObstacleAvoidanceType obstacleAvoidanceType
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return NavMeshAgent.get_obstacleAvoidanceType_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				NavMeshAgent.set_obstacleAvoidanceType_Injected(intPtr, value);
			}
		}

		public int avoidancePriority
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return NavMeshAgent.get_avoidancePriority_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				NavMeshAgent.set_avoidancePriority_Injected(intPtr, value);
			}
		}

		public bool isOnNavMesh
		{
			[NativeName("InCrowdSystem")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshAgent>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return NavMeshAgent.get_isOnNavMesh_Injected(intPtr);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetDestination_Injected(IntPtr _unity_self, [In] ref Vector3 target);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_destination_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_destination_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_stoppingDistance_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_stoppingDistance_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_velocity_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_velocity_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_nextPosition_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_nextPosition_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_steeringTarget_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_desiredVelocity_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_remainingDistance_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_baseOffset_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_baseOffset_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isOnOffMeshLink_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ActivateCurrentOffMeshLink_Injected(IntPtr _unity_self, bool activated);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetCurrentOffMeshLinkDataInternal_Injected(IntPtr _unity_self, out OffMeshLinkData ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetNextOffMeshLinkDataInternal_Injected(IntPtr _unity_self, out OffMeshLinkData ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CompleteOffMeshLink_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_autoTraverseOffMeshLink_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_autoTraverseOffMeshLink_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_autoBraking_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_autoBraking_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_autoRepath_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_autoRepath_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_hasPath_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_pathPending_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isPathStale_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern NavMeshPathStatus get_pathStatus_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_pathEndPosition_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Warp_Injected(IntPtr _unity_self, [In] ref Vector3 newPosition);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Move_Injected(IntPtr _unity_self, [In] ref Vector3 offset);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Stop_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Resume_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isStopped_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_isStopped_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResetPath_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetPath_Injected(IntPtr _unity_self, IntPtr path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyPathTo_Injected(IntPtr _unity_self, IntPtr path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool FindClosestEdge_Injected(IntPtr _unity_self, out NavMeshHit hit);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Raycast_Injected(IntPtr _unity_self, [In] ref Vector3 targetPosition, out NavMeshHit hit);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CalculatePathInternal_Injected(IntPtr _unity_self, [In] ref Vector3 targetPosition, IntPtr path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SamplePathPosition_Injected(IntPtr _unity_self, int areaMask, float maxDistance, out NavMeshHit hit);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLayerCost_Injected(IntPtr _unity_self, int layer, float cost);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetLayerCost_Injected(IntPtr _unity_self, int layer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetAreaCost_Injected(IntPtr _unity_self, int areaIndex, float areaCost);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetAreaCost_Injected(IntPtr _unity_self, int areaIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_agentTypeID_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_agentTypeID_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetOwnerInternal_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_areaMask_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_areaMask_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_speed_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_speed_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_angularSpeed_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_angularSpeed_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_acceleration_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_acceleration_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_updatePosition_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_updatePosition_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_updateRotation_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_updateRotation_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_updateUpAxis_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_updateUpAxis_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_radius_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_radius_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_height_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_height_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ObstacleAvoidanceType get_obstacleAvoidanceType_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_obstacleAvoidanceType_Injected(IntPtr _unity_self, ObstacleAvoidanceType value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_avoidancePriority_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_avoidancePriority_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isOnNavMesh_Injected(IntPtr _unity_self);
	}
}
