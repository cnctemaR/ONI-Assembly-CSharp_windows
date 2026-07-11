using System;

namespace UnityEngine.Experimental.XR
{
	/// <summary>
	///   <para>Contains data supplied to a XRDepth.PointCloudUpdated event.</para>
	/// </summary>
	public struct PointCloudUpdatedEventArgs
	{
		/// <summary>
		///   <para>A reference to the XRDepthSubsystem object from which the event originated.</para>
		/// </summary>
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
