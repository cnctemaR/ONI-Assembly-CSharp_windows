using System;
using UnityEngine.SpatialTracking;

namespace UnityEngine.Experimental.XR.Interaction
{
	[Serializable]
	public abstract class BaseArmModel : BasePoseProvider
	{
		public TrackedPoseDriver.TrackedPose headPoseSource
		{
			get
			{
				return this.m_HeadPoseSource;
			}
			internal set
			{
				this.m_HeadPoseSource = value;
			}
		}

		public TrackedPoseDriver.TrackedPose poseSource
		{
			get
			{
				return this.m_PoseSource;
			}
			internal set
			{
				this.m_PoseSource = value;
			}
		}

		protected bool TryGetTrackingDataFromSource(TrackedPoseDriver.TrackedPose poseSource, out Pose resultPose)
		{
			return PoseDataSource.TryGetDataFromSource(poseSource, out resultPose);
		}

		[SerializeField]
		[Tooltip("The head pose that will be used by the arm model")]
		private TrackedPoseDriver.TrackedPose m_HeadPoseSource;

		[SerializeField]
		[Tooltip("The arm source pose that will be used by the arm model")]
		private TrackedPoseDriver.TrackedPose m_PoseSource;
	}
}
