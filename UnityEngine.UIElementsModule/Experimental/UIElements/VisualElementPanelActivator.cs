using System;

namespace UnityEngine.Experimental.UIElements
{
	internal class VisualElementPanelActivator
	{
		public VisualElementPanelActivator(IVisualElementPanelActivatable activatable)
		{
			this.m_Activatable = activatable;
		}

		public bool isActive { get; private set; }

		public bool isDetaching { get; private set; }

		public void SetActive(bool action)
		{
			if (this.isActive != action)
			{
				this.isActive = action;
				if (this.isActive)
				{
					this.m_Activatable.element.RegisterCallback<AttachToPanelEvent>(new EventCallback<AttachToPanelEvent>(this.OnEnter), TrickleDown.NoTrickleDown);
					this.m_Activatable.element.RegisterCallback<DetachFromPanelEvent>(new EventCallback<DetachFromPanelEvent>(this.OnLeave), TrickleDown.NoTrickleDown);
					this.SendActivation();
				}
				else
				{
					this.m_Activatable.element.UnregisterCallback<AttachToPanelEvent>(new EventCallback<AttachToPanelEvent>(this.OnEnter), TrickleDown.NoTrickleDown);
					this.m_Activatable.element.UnregisterCallback<DetachFromPanelEvent>(new EventCallback<DetachFromPanelEvent>(this.OnLeave), TrickleDown.NoTrickleDown);
					this.SendDeactivation();
				}
			}
		}

		public void SendActivation()
		{
			if (this.m_Activatable.CanBeActivated())
			{
				this.m_Activatable.OnPanelActivate();
			}
		}

		public void SendDeactivation()
		{
			if (this.m_Activatable.CanBeActivated())
			{
				this.m_Activatable.OnPanelDeactivate();
			}
		}

		private void OnEnter(AttachToPanelEvent evt)
		{
			if (this.isActive)
			{
				this.SendActivation();
			}
		}

		private void OnLeave(DetachFromPanelEvent evt)
		{
			if (this.isActive)
			{
				this.isDetaching = true;
				try
				{
					this.SendDeactivation();
				}
				finally
				{
					this.isDetaching = false;
				}
			}
		}

		private IVisualElementPanelActivatable m_Activatable;
	}
}
