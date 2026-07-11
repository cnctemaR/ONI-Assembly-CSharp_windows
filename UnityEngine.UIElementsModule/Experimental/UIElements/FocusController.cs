using System;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>Class in charge of managing the focus inside a Panel.</para>
	/// </summary>
	public class FocusController
	{
		/// <summary>
		///   <para>Constructor.</para>
		/// </summary>
		/// <param name="focusRing"></param>
		public FocusController(IFocusRing focusRing)
		{
			this.focusRing = focusRing;
			this.focusedElement = null;
			this.imguiKeyboardControl = 0;
		}

		private IFocusRing focusRing { get; set; }

		/// <summary>
		///   <para>The currently focused element.</para>
		/// </summary>
		public Focusable focusedElement { get; private set; }

		private static void AboutToReleaseFocus(Focusable focusable, Focusable willGiveFocusTo, FocusChangeDirection direction)
		{
			using (FocusOutEvent pooled = FocusEventBase<FocusOutEvent>.GetPooled(focusable, willGiveFocusTo, direction))
			{
				UIElementsUtility.eventDispatcher.DispatchEvent(pooled, null);
			}
		}

		private static void ReleaseFocus(Focusable focusable, Focusable willGiveFocusTo, FocusChangeDirection direction)
		{
			using (BlurEvent pooled = FocusEventBase<BlurEvent>.GetPooled(focusable, willGiveFocusTo, direction))
			{
				UIElementsUtility.eventDispatcher.DispatchEvent(pooled, null);
			}
		}

		private static void AboutToGrabFocus(Focusable focusable, Focusable willTakeFocusFrom, FocusChangeDirection direction)
		{
			using (FocusInEvent pooled = FocusEventBase<FocusInEvent>.GetPooled(focusable, willTakeFocusFrom, direction))
			{
				UIElementsUtility.eventDispatcher.DispatchEvent(pooled, null);
			}
		}

		private static void GrabFocus(Focusable focusable, Focusable willTakeFocusFrom, FocusChangeDirection direction)
		{
			using (FocusEvent pooled = FocusEventBase<FocusEvent>.GetPooled(focusable, willTakeFocusFrom, direction))
			{
				UIElementsUtility.eventDispatcher.DispatchEvent(pooled, null);
			}
		}

		internal void SwitchFocus(Focusable newFocusedElement)
		{
			this.SwitchFocus(newFocusedElement, FocusChangeDirection.unspecified);
		}

		private void SwitchFocus(Focusable newFocusedElement, FocusChangeDirection direction)
		{
			if (newFocusedElement != this.focusedElement)
			{
				Focusable focusedElement = this.focusedElement;
				if (newFocusedElement == null || !newFocusedElement.canGrabFocus)
				{
					if (focusedElement != null)
					{
						FocusController.AboutToReleaseFocus(focusedElement, newFocusedElement, direction);
						this.focusedElement = null;
						FocusController.ReleaseFocus(focusedElement, newFocusedElement, direction);
					}
				}
				else if (newFocusedElement != focusedElement)
				{
					if (focusedElement != null)
					{
						FocusController.AboutToReleaseFocus(focusedElement, newFocusedElement, direction);
					}
					FocusController.AboutToGrabFocus(newFocusedElement, focusedElement, direction);
					this.focusedElement = newFocusedElement;
					if (focusedElement != null)
					{
						FocusController.ReleaseFocus(focusedElement, newFocusedElement, direction);
					}
					FocusController.GrabFocus(newFocusedElement, focusedElement, direction);
				}
			}
		}

		/// <summary>
		///   <para>Ask the controller to change the focus according to the event. The focus controller will use its focus ring to choose the next element to be focused.</para>
		/// </summary>
		/// <param name="e"></param>
		public void SwitchFocusOnEvent(EventBase e)
		{
			FocusChangeDirection focusChangeDirection = this.focusRing.GetFocusChangeDirection(this.focusedElement, e);
			if (focusChangeDirection != FocusChangeDirection.none)
			{
				Focusable nextFocusable = this.focusRing.GetNextFocusable(this.focusedElement, focusChangeDirection);
				this.SwitchFocus(nextFocusable, focusChangeDirection);
			}
		}

		internal int imguiKeyboardControl { get; set; }

		internal void SyncIMGUIFocus(int imguiKeyboardControlID, IMGUIContainer imguiContainerHavingKeyboardControl)
		{
			this.imguiKeyboardControl = imguiKeyboardControlID;
			if (this.imguiKeyboardControl != 0)
			{
				this.SwitchFocus(imguiContainerHavingKeyboardControl, FocusChangeDirection.unspecified);
			}
			else
			{
				this.SwitchFocus(null, FocusChangeDirection.unspecified);
			}
		}
	}
}
