using System;

namespace UnityEngine.UIElements
{
	internal class EditorPanelRootElement : PanelRootElement
	{
		public EditorPanelRootElement()
		{
			base.pickingMode = PickingMode.Position;
			base.RegisterCallback<ExecuteCommandEvent>(new EventCallback<ExecuteCommandEvent>(this.OnEventCompletedAtAnyTarget), TrickleDown.NoTrickleDown);
			base.RegisterCallback<ValidateCommandEvent>(new EventCallback<ValidateCommandEvent>(this.OnEventCompletedAtAnyTarget), TrickleDown.NoTrickleDown);
			base.RegisterCallback<MouseEnterWindowEvent>(new EventCallback<MouseEnterWindowEvent>(this.OnEventCompletedAtAnyTarget), TrickleDown.NoTrickleDown);
			base.RegisterCallback<MouseLeaveWindowEvent>(new EventCallback<MouseLeaveWindowEvent>(this.OnEventCompletedAtAnyTarget), TrickleDown.NoTrickleDown);
			base.RegisterCallback<IMGUIEvent>(new EventCallback<IMGUIEvent>(this.OnEventCompletedAtAnyTarget), TrickleDown.NoTrickleDown);
		}

		private void OnEventCompletedAtAnyTarget(EventBase evt)
		{
			bool propagateToIMGUI = evt.propagateToIMGUI;
			if (propagateToIMGUI)
			{
				EventDispatchUtilities.PropagateToRemainingIMGUIContainers(evt, this);
				evt.propagateToIMGUI = false;
			}
		}
	}
}
