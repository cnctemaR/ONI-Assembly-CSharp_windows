using System;

namespace UnityEngine.Experimental.XR
{
	public struct SessionTrackingStateChangedEventArgs
	{
		public XRSessionSubsystem SessionSubsystem
		{
			get
			{
				return this.m_Session;
			}
		}

		public TrackingState NewState { get; set; }

		internal XRSessionSubsystem m_Session;
	}
}
