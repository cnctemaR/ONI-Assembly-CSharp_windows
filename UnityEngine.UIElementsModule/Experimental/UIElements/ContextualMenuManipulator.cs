using System;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>Manipulator that displays a contextual menu when the user clicks the right mouse button or presses the menu key on the keyboard.</para>
	/// </summary>
	public class ContextualMenuManipulator : MouseManipulator
	{
		public ContextualMenuManipulator(Action<ContextualMenuPopulateEvent> menuBuilder)
		{
			this.m_MenuBuilder = menuBuilder;
			base.activators.Add(new ManipulatorActivationFilter
			{
				button = MouseButton.RightMouse
			});
		}

		/// <summary>
		///   <para>Register the event callbacks on the manipulator target.</para>
		/// </summary>
		protected override void RegisterCallbacksOnTarget()
		{
			base.target.RegisterCallback<MouseUpEvent>(new EventCallback<MouseUpEvent>(this.OnMouseUpEvent), Capture.NoCapture);
			base.target.RegisterCallback<KeyUpEvent>(new EventCallback<KeyUpEvent>(this.OnKeyUpEvent), Capture.NoCapture);
			base.target.RegisterCallback<ContextualMenuPopulateEvent>(new EventCallback<ContextualMenuPopulateEvent>(this.OnContextualMenuEvent), Capture.NoCapture);
		}

		/// <summary>
		///   <para>Unregister the event callbacks from the manipulator target.</para>
		/// </summary>
		protected override void UnregisterCallbacksFromTarget()
		{
			base.target.UnregisterCallback<MouseUpEvent>(new EventCallback<MouseUpEvent>(this.OnMouseUpEvent), Capture.NoCapture);
			base.target.UnregisterCallback<KeyUpEvent>(new EventCallback<KeyUpEvent>(this.OnKeyUpEvent), Capture.NoCapture);
			base.target.UnregisterCallback<ContextualMenuPopulateEvent>(new EventCallback<ContextualMenuPopulateEvent>(this.OnContextualMenuEvent), Capture.NoCapture);
		}

		private void OnMouseUpEvent(MouseUpEvent evt)
		{
			if (base.CanStartManipulation(evt))
			{
				if (base.target.elementPanel != null && base.target.elementPanel.contextualMenuManager != null)
				{
					base.target.elementPanel.contextualMenuManager.DisplayMenu(evt, base.target);
					evt.StopPropagation();
					evt.PreventDefault();
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
