using System;

namespace UnityEngine.Experimental.XR
{
	/// <summary>
	///   <para>Structure defining Tracking State Changed event arguments passed when tracking state changes.</para>
	/// </summary>
	public struct SessionTrackingStateChangedEventArgs
	{
		/// <summary>
		///   <para>Reference to the XRSessionSubsystem object associated with the event.</para>
		/// </summary>
		public XRSessionSubsystem SessionSubsystem
		{
			get
			{
				return this.m_Session;
			}
		}

		/// <summary>
		///   <para>New Tracking State.</para>
		/// </summary>
		public TrackingState NewState { get; set; }

		internal XRSessionSubsystem m_Session;
	}
}
