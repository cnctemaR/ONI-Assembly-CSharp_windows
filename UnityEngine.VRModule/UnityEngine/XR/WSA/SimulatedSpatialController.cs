using System;

namespace UnityEngine.XR.WSA
{
	internal class SimulatedSpatialController
	{
		internal SimulatedSpatialController(Handedness controller)
		{
			this.m_ControllerHandednss = controller;
		}

		public Quaternion orientation
		{
			get
			{
				return HolographicAutomation.GetHandOrientation(this.m_ControllerHandednss);
			}
			set
			{
				HolographicAutomation.TrySetHandOrientation(this.m_ControllerHandednss, value);
			}
		}

		public Vector3 position
		{
			get
			{
				return HolographicAutomation.GetControllerPosition(this.m_ControllerHandednss);
			}
			set
			{
				HolographicAutomation.TrySetControllerPosition(this.m_ControllerHandednss, value);
			}
		}

		public bool activated
		{
			get
			{
				return HolographicAutomation.GetControllerActivated(this.m_ControllerHandednss);
			}
			set
			{
				HolographicAutomation.TrySetControllerActivated(this.m_ControllerHandednss, value);
			}
		}

		public bool visible
		{
			get
			{
				return HolographicAutomation.GetControllerVisible(this.m_ControllerHandednss);
			}
		}

		public void EnsureVisible()
		{
			HolographicAutomation.TryEnsureControllerVisible(this.m_ControllerHandednss);
		}

		public void PerformControllerPress(SimulatedControllerPress button)
		{
			HolographicAutomation.PerformButtonPress(this.m_ControllerHandednss, button);
		}

		public Handedness m_ControllerHandednss;
	}
}
