using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR
{
	/// <summary>
	///   <para>Provides access to depth data of the physical environment, such as a point cloud.</para>
	/// </summary>
	[NativeHeader("Modules/XR/XRPrefix.h")]
	[UsedByNativeCode]
	[NativeConditional("ENABLE_XR")]
	[NativeHeader("Modules/XR/Subsystems/Depth/XRDepthSubsystem.h")]
	public class XRDepthSubsystem : Subsystem<XRDepthSubsystemDescriptor>
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<PointCloudUpdatedEventArgs> PointCloudUpdated;

		/// <summary>
		///   <para>The frame during which the point cloud was last updated.</para>
		/// </summary>
		public extern int LastUpdatedFrame
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public void GetPoints(List<Vector3> pointsOut)
		{
			if (pointsOut == null)
			{
				throw new ArgumentNullException("pointsOut");
			}
			this.Internal_GetPointCloudPointsAsList(pointsOut);
		}

		public void GetConfidence(List<float> confidenceOut)
		{
			if (confidenceOut == null)
			{
				throw new ArgumentNullException("confidenceOut");
			}
			this.Internal_GetPointCloudConfidenceAsList(confidenceOut);
		}

		[RequiredByNativeCode]
		private void InvokePointCloudUpdatedEvent()
		{
			if (this.PointCloudUpdated != null)
			{
				this.PointCloudUpdated(new PointCloudUpdatedEventArgs
				{
					m_DepthSubsystem = this
				});
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_GetPointCloudPointsAsList(List<Vector3> pointsOut);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_GetPointCloudConfidenceAsList(List<float> confidenceOut);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Vector3[] Internal_GetPointCloudPointsAsFixedArray();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float[] Internal_GetPointCloudConfidenceAsFixedArray();
	}
}
