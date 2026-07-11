using System;
using System.Collections.Generic;
using UnityEngine.XR;
using UnityEngine.XR.Tango;

namespace UnityEngine.SpatialTracking
{
	public static class PoseDataSource
	{
		internal static bool TryGetNodePoseData(XRNode node, out Pose resultPose)
		{
			InputTracking.GetNodeStates(PoseDataSource.nodeStates);
			foreach (XRNodeState xrnodeState in PoseDataSource.nodeStates)
			{
				if (xrnodeState.nodeType == node)
				{
					if (xrnodeState.TryGetPosition(out resultPose.position))
					{
						if (xrnodeState.TryGetRotation(out resultPose.rotation))
						{
							return true;
						}
					}
					resultPose = Pose.identity;
					return false;
				}
			}
			resultPose = Pose.identity;
			return false;
		}

		public static bool TryGetDataFromSource(TrackedPoseDriver.TrackedPose poseSource, out Pose resultPose)
		{
			bool flag;
			switch (poseSource)
			{
			case TrackedPoseDriver.TrackedPose.LeftEye:
				flag = PoseDataSource.TryGetNodePoseData(XRNode.LeftEye, out resultPose);
				break;
			case TrackedPoseDriver.TrackedPose.RightEye:
				flag = PoseDataSource.TryGetNodePoseData(XRNode.RightEye, out resultPose);
				break;
			case TrackedPoseDriver.TrackedPose.Center:
				flag = PoseDataSource.TryGetNodePoseData(XRNode.CenterEye, out resultPose);
				break;
			case TrackedPoseDriver.TrackedPose.Head:
				flag = PoseDataSource.TryGetNodePoseData(XRNode.Head, out resultPose);
				break;
			case TrackedPoseDriver.TrackedPose.LeftPose:
				flag = PoseDataSource.TryGetNodePoseData(XRNode.LeftHand, out resultPose);
				break;
			case TrackedPoseDriver.TrackedPose.RightPose:
				flag = PoseDataSource.TryGetNodePoseData(XRNode.RightHand, out resultPose);
				break;
			case TrackedPoseDriver.TrackedPose.ColorCamera:
				flag = PoseDataSource.TryGetTangoPose(CoordinateFrame.CameraColor, out resultPose) || PoseDataSource.TryGetNodePoseData(XRNode.CenterEye, out resultPose);
				break;
			case TrackedPoseDriver.TrackedPose.DepthCamera:
				flag = PoseDataSource.TryGetTangoPose(CoordinateFrame.CameraDepth, out resultPose);
				break;
			case TrackedPoseDriver.TrackedPose.FisheyeCamera:
				flag = PoseDataSource.TryGetTangoPose(CoordinateFrame.CameraFisheye, out resultPose);
				break;
			case TrackedPoseDriver.TrackedPose.Device:
				flag = PoseDataSource.TryGetTangoPose(CoordinateFrame.Device, out resultPose);
				break;
			case TrackedPoseDriver.TrackedPose.RemotePose:
				flag = PoseDataSource.TryGetNodePoseData(XRNode.RightHand, out resultPose);
				break;
			default:
				resultPose = Pose.identity;
				flag = false;
				break;
			}
			return flag;
		}

		private static bool TryGetTangoPose(CoordinateFrame frame, out Pose pose)
		{
			PoseData poseData;
			bool flag;
			if (TangoInputTracking.TryGetPoseAtTime(out poseData, TangoDevice.baseCoordinateFrame, frame, 0.0) && poseData.statusCode == PoseStatus.Valid)
			{
				pose.position = poseData.position;
				pose.rotation = poseData.rotation;
				flag = true;
			}
			else
			{
				pose = Pose.identity;
				flag = false;
			}
			return flag;
		}

		private static List<XRNodeState> nodeStates = new List<XRNodeState>();
	}
}
