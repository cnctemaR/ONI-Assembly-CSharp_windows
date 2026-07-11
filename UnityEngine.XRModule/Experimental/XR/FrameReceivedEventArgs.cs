using System;

namespace UnityEngine.Experimental.XR
{
	public struct FrameReceivedEventArgs
	{
		public XRCameraSubsystem CameraSubsystem
		{
			get
			{
				return this.m_CameraSubsystem;
			}
		}

		internal XRCameraSubsystem m_CameraSubsystem;
	}
}
