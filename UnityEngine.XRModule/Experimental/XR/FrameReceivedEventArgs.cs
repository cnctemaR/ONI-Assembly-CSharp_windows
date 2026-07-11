using System;

namespace UnityEngine.Experimental.XR
{
	/// <summary>
	///   <para>Structure containing data passed during Frame Received Event.</para>
	/// </summary>
	public struct FrameReceivedEventArgs
	{
		/// <summary>
		///   <para>Reference to the XRCameraSubsystem associated with this event.</para>
		/// </summary>
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
