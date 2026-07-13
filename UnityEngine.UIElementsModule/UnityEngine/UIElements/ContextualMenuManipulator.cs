using System;

namespace UnityEngine.UIElements
{
	public class ContextualMenuManipulator : PointerManipulator
	{
		public ContextualMenuManipulator(Action<ContextualMenuPopulateEvent> menuBuilder)
		{
			this.m_MenuBuilder = menuBuilder;
			base.activators.Add(new ManipulatorActivationFilter
			{
				button = MouseButton.RightMouse
			});
			bool flag = this.IsOSXContextualMenuPlatform();
			if (flag)
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
			bool flag = this.IsOSXContextualMenuPlatform();
			if (flag)
			{
				base.target.RegisterCallback<PointerDownEvent>(new EventCallback<PointerDownEvent>(this.OnPointerDownEventOSX), TrickleDown.NoTrickleDown);
				base.target.RegisterCallback<PointerUpEvent>(new EventCallback<PointerUpEvent>(this.OnPointerUpEventOSX), TrickleDown.NoTrickleDown);
				base.target.RegisterCallback<PointerMoveEvent>(new EventCallback<PointerMoveEvent>(this.OnPointerMoveEventOSX), TrickleDown.NoTrickleDown);
			}
			else
			{
				base.target.RegisterCallback<PointerUpEvent>(new EventCallback<PointerUpEvent>(this.OnPointerUpEvent), TrickleDown.NoTrickleDown);
				base.target.RegisterCallback<PointerMoveEvent>(new EventCallback<PointerMoveEvent>(this.OnPointerMoveEvent), TrickleDown.NoTrickleDown);
			}
			base.target.RegisterCallback<KeyUpEvent>(new EventCallback<KeyUpEvent>(this.OnKeyUpEvent), TrickleDown.NoTrickleDown);
			base.target.RegisterCallback<ContextualMenuPopulateEvent>(new EventCallback<ContextualMenuPopulateEvent>(this.OnContextualMenuEvent), TrickleDown.NoTrickleDown);
		}

		protected override void UnregisterCallbacksFromTarget()
		{
			bool flag = this.IsOSXContextualMenuPlatform();
			if (flag)
			{
				base.target.UnregisterCallback<PointerDownEvent>(new EventCallback<PointerDownEvent>(this.OnPointerDownEventOSX), TrickleDown.NoTrickleDown);
				base.target.UnregisterCallback<PointerUpEvent>(new EventCallback<PointerUpEvent>(this.OnPointerUpEventOSX), TrickleDown.NoTrickleDown);
				base.target.UnregisterCallback<PointerMoveEvent>(new EventCallback<PointerMoveEvent>(this.OnPointerMoveEventOSX), TrickleDown.NoTrickleDown);
			}
			else
			{
				base.target.UnregisterCallback<PointerUpEvent>(new EventCallback<PointerUpEvent>(this.OnPointerUpEvent), TrickleDown.NoTrickleDown);
				base.target.UnregisterCallback<PointerMoveEvent>(new EventCallback<PointerMoveEvent>(this.OnPointerMoveEvent), TrickleDown.NoTrickleDown);
			}
			base.target.UnregisterCallback<KeyUpEvent>(new EventCallback<KeyUpEvent>(this.OnKeyUpEvent), TrickleDown.NoTrickleDown);
			base.target.UnregisterCallback<ContextualMenuPopulateEvent>(new EventCallback<ContextualMenuPopulateEvent>(this.OnContextualMenuEvent), TrickleDown.NoTrickleDown);
		}

		protected bool IsOSXContextualMenuPlatform()
		{
			return UIElementsUtility.isOSXContextualMenuPlatform;
		}

		private void OnPointerUpEvent(IPointerEvent evt)
		{
			this.ProcessPointerEvent(evt);
		}

		private void OnPointerDownEventOSX(IPointerEvent evt)
		{
			this.ProcessPointerEvent(evt);
		}

		private void OnPointerUpEventOSX(IPointerEvent evt)
		{
			BaseVisualElementPanel elementPanel = base.target.elementPanel;
			ContextualMenuManager contextualMenuManager = ((elementPanel != null) ? elementPanel.contextualMenuManager : null);
			bool flag = contextualMenuManager != null && contextualMenuManager.displayMenuHandledOSX;
			if (!flag)
			{
				this.ProcessPointerEvent(evt);
			}
		}

		private void OnPointerMoveEvent(PointerMoveEvent evt)
		{
			bool isPointerUp = evt.isPointerUp;
			if (isPointerUp)
			{
				this.OnPointerUpEvent(evt);
			}
		}

		private void OnPointerMoveEventOSX(PointerMoveEvent evt)
		{
			bool isPointerUp = evt.isPointerUp;
			if (isPointerUp)
			{
				this.OnPointerUpEventOSX(evt);
			}
			else
			{
				bool isPointerDown = evt.isPointerDown;
				if (isPointerDown)
				{
					this.OnPointerDownEventOSX(evt);
				}
			}
		}

		private void ProcessPointerEvent(IPointerEvent evt)
		{
			bool flag = base.CanStartManipulation(evt);
			if (flag)
			{
				this.DoDisplayMenu(evt as EventBase);
			}
		}

		private void OnKeyUpEvent(KeyUpEvent evt)
		{
			bool flag = evt.keyCode == KeyCode.Menu;
			if (flag)
			{
				this.DoDisplayMenu(evt);
			}
		}

		private void DoDisplayMenu(EventBase evt)
		{
			BaseVisualElementPanel elementPanel = base.target.elementPanel;
			bool flag = ((elementPanel != null) ? elementPanel.contextualMenuManager : null) != null;
			if (flag)
			{
				base.target.elementPanel.contextualMenuManager.DisplayMenu(evt, base.target);
				evt.StopPropagation();
			}
		}

		private void OnContextualMenuEvent(ContextualMenuPopulateEvent evt)
		{
			Action<ContextualMenuPopulateEvent> menuBuilder = this.m_MenuBuilder;
			if (menuBuilder != null)
			{
				menuBuilder(evt);
			}
		}

		private Action<ContextualMenuPopulateEvent> m_MenuBuilder;
	}
}
