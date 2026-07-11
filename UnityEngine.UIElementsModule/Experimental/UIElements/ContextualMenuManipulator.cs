using System;

namespace UnityEngine.Experimental.UIElements
{
	public class ContextualMenuManipulator : MouseManipulator
	{
		public ContextualMenuManipulator(Action<ContextualMenuPopulateEvent> menuBuilder)
		{
			this.m_MenuBuilder = menuBuilder;
			base.activators.Add(new ManipulatorActivationFilter
			{
				button = MouseButton.RightMouse
			});
			if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
			{
				base.activators.Add(new ManipulatorActivationFilter
				{
					button = MouseButton.LeftMouse,
					modifiers = EventModifiers.Control
				});
			}
		}

		protected override void RegisterCallbacksOnTarget()
		{
			if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
			{
				base.target.RegisterCallback<MouseDownEvent>(new EventCallback<MouseDownEvent>(this.OnMouseUpDownEvent), TrickleDown.NoTrickleDown);
			}
			else
			{
				base.target.RegisterCallback<MouseUpEvent>(new EventCallback<MouseUpEvent>(this.OnMouseUpDownEvent), TrickleDown.NoTrickleDown);
			}
			base.target.RegisterCallback<KeyUpEvent>(new EventCallback<KeyUpEvent>(this.OnKeyUpEvent), TrickleDown.NoTrickleDown);
			base.target.RegisterCallback<ContextualMenuPopulateEvent>(new EventCallback<ContextualMenuPopulateEvent>(this.OnContextualMenuEvent), TrickleDown.NoTrickleDown);
		}

		protected override void UnregisterCallbacksFromTarget()
		{
			if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
			{
				base.target.UnregisterCallback<MouseDownEvent>(new EventCallback<MouseDownEvent>(this.OnMouseUpDownEvent), TrickleDown.NoTrickleDown);
			}
			else
			{
				base.target.UnregisterCallback<MouseUpEvent>(new EventCallback<MouseUpEvent>(this.OnMouseUpDownEvent), TrickleDown.NoTrickleDown);
			}
			base.target.UnregisterCallback<KeyUpEvent>(new EventCallback<KeyUpEvent>(this.OnKeyUpEvent), TrickleDown.NoTrickleDown);
			base.target.UnregisterCallback<ContextualMenuPopulateEvent>(new EventCallback<ContextualMenuPopulateEvent>(this.OnContextualMenuEvent), TrickleDown.NoTrickleDown);
		}

		private void OnMouseUpDownEvent(IMouseEvent evt)
		{
			if (base.CanStartManipulation(evt))
			{
				if (base.target.elementPanel != null && base.target.elementPanel.contextualMenuManager != null)
				{
					EventBase eventBase = evt as EventBase;
					base.target.elementPanel.contextualMenuManager.DisplayMenu(eventBase, base.target);
					eventBase.StopPropagation();
					eventBase.PreventDefault();
				}
			}
		}

		private void OnKeyUpEvent(KeyUpEvent evt)
		{
			if (evt.keyCode == KeyCode.Menu)
			{
				if (base.target.elementPanel != null && base.target.elementPanel.contextualMenuManager != null)
				{
					base.target.elementPanel.contextualMenuManager.DisplayMenu(evt, base.target);
					evt.StopPropagation();
					evt.PreventDefault();
				}
			}
		}

		private void OnContextualMenuEvent(ContextualMenuPopulateEvent evt)
		{
			if (this.m_MenuBuilder != null)
			{
				this.m_MenuBuilder(evt);
			}
		}

		private Action<ContextualMenuPopulateEvent> m_MenuBuilder;
	}
}
