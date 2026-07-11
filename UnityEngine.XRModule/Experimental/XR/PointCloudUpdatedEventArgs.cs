using System;

namespace UnityEngine.Experimental.XR
{
	public struct PointCloudUpdatedEventArgs
	{
		public XRDepthSubsystem DepthSubsystem
		{
			get
			{
				return this.m_DepthSubsystem;
			}
		}

		internal XRDepthSubsystem m_DepthSubsystem;
	}
}
