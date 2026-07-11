using System;

namespace UnityEngine.XR.WSA
{
	internal class SimulatedHand
	{
		internal SimulatedHand(GestureHand hand)
		{
			this.m_Hand = hand;
		}

		public Vector3 position
		{
			get
			{
				return HolographicAutomation.GetHandPosition(this.m_Hand);
			}
			set
			{
				HolographicAutomation.SetHandPosition(this.m_Hand, value);
			}
		}

		public bool activated
		{
			get
			{
				return HolographicAutomation.GetHandActivated(this.m_Hand);
			}
			set
			{
				HolographicAutomation.SetHandActivated(this.m_Hand, value);
			}
		}

		public bool visible
		{
			get
			{
				return HolographicAutomation.GetHandVisible(this.m_Hand);
			}
		}

		public void EnsureVisible()
		{
			HolographicAutomation.EnsureHandVisible(this.m_Hand);
		}

		public void PerformGesture(SimulatedGesture gesture)
		{
			HolographicAutomation.PerformGesture(this.m_Hand, gesture);
		}

		public GestureHand m_Hand;
	}
}
