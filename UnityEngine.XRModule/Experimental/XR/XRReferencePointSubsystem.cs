using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR
{
	/// <summary>
	///   <para>Provides methods and properties that allow for querying, creating, and removing of reference points. These reference points are cues to the XRSessionSubsystem that indicate areas of interest in the environment which helps assure that tracking of these points remains accurate.</para>
	/// </summary>
	[NativeConditional("ENABLE_XR")]
	[NativeHeader("Modules/XR/Subsystems/ReferencePoints/XRReferencePointSubsystem.h")]
	[UsedByNativeCode]
	[NativeHeader("Modules/XR/XRPrefix.h")]
	public class XRReferencePointSubsystem : Subsystem<XRReferencePointSubsystemDescriptor>
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<ReferencePointUpdatedEventArgs> ReferencePointUpdated;

		/// <summary>
		///   <para>The frame during which the reference points were last updated.</para>
		/// </summary>
		public extern int LastUpdatedFrame
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public bool TryAddReferencePoint(Vector3 position, Quaternion rotation, out TrackableId referencePointId)
		{
			return this.TryAddReferencePoint_Injected(ref position, ref rotation, out referencePointId);
		}

		public bool TryAddReferencePoint(Pose pose, out TrackableId referencePointId)
		{
			return this.TryAddReferencePoint(pose.position, pose.rotation, out referencePointId);
		}

		/// <summary>
		///   <para>Attempt to remove a ReferencePoint getting tracked by the device.</para>
		/// </summary>
		/// <param name="referencePointId">ID of the ReferencePoint you wish to remove so the device no longer tries to track it.</param>
		/// <returns>
		///   <para>If the ReferencePoint was removed successfully, this method returns true. Otherwise, it returns false.</para>
		/// </returns>
		public bool TryRemoveReferencePoint(TrackableId referencePointId)
		{
			return this.TryRemoveReferencePoint_Injected(ref referencePointId);
		}

		public bool TryGetReferencePoint(TrackableId referencePointId, out ReferencePoint referencePoint)
		{
			return this.TryGetReferencePoint_Injected(ref referencePointId, out referencePoint);
		}

		public void GetAllReferencePoints(List<ReferencePoint> referencePointsOut)
		{
			if (referencePointsOut == null)
			{
				throw new ArgumentNullException("referencePointsOut");
			}
			this.Internal_GetAllReferencePointsAsList(referencePointsOut);
		}

		[RequiredByNativeCode]
		private void InvokeReferencePointUpdatedEvent(ReferencePoint updatedReferencePoint, TrackingState previousTrackingState, Pose previousPose)
		{
			if (this.ReferencePointUpdated != null)
			{
				this.ReferencePointUpdated(new ReferencePointUpdatedEventArgs
				{
					ReferencePoint = updatedReferencePoint,
					PreviousTrackingState = previousTrackingState,
					PreviousPose = previousPose
				});
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_GetAllReferencePointsAsList(List<ReferencePoint> referencePointsOut);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern ReferencePoint[] Internal_GetAllReferencePointsAsFixedArray();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool TryAddReferencePoint_Injected(ref Vector3 position, ref Quaternion rotation, out TrackableId referencePointId);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool TryRemoveReferencePoint_Injected(ref TrackableId referencePointId);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool TryGetReferencePoint_Injected(ref TrackableId referencePointId, out ReferencePoint referencePoint);
	}
}
