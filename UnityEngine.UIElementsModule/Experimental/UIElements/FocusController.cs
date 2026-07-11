using System;

namespace UnityEngine.Experimental.UIElements
{
	public class FocusController
	{
		public FocusController(IFocusRing focusRing)
		{
			this.focusRing = focusRing;
			this.focusedElement = null;
			this.imguiKeyboardControl = 0;
		}

		private IFocusRing focusRing { get; }

		public Focusable focusedElement { get; private set; }

		internal void DoFocusChange(Focusable f)
		{
			this.focusedElement = f;
		}

		private void AboutToReleaseFocus(Focusable focusable, Focusable willGiveFocusTo, FocusChangeDirection direction)
		{
			using (FocusOutEvent pooled = FocusEventBase<FocusOutEvent>.GetPooled(focusable, willGiveFocusTo, direction, this))
			{
				focusable.SendEvent(pooled);
			}
		}

		private void ReleaseFocus(Focusable focusable, Focusable willGiveFocusTo, FocusChangeDirection direction)
		{
			using (BlurEvent pooled = FocusEventBase<BlurEvent>.GetPooled(focusable, willGiveFocusTo, direction, this))
			{
				focusable.SendEvent(pooled);
			}
		}

		private void AboutToGrabFocus(Focusable focusable, Focusable willTakeFocusFrom, FocusChangeDirection direction)
		{
			using (FocusInEvent pooled = FocusEventBase<FocusInEvent>.GetPooled(focusable, willTakeFocusFrom, direction, this))
			{
				focusable.SendEvent(pooled);
			}
		}

		private void GrabFocus(Focusable focusable, Focusable willTakeFocusFrom, FocusChangeDirection direction)
		{
			using (FocusEvent pooled = FocusEventBase<FocusEvent>.GetPooled(focusable, willTakeFocusFrom, direction, this))
			{
				focusable.SendEvent(pooled);
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
						this.AboutToReleaseFocus(focusedElement, newFocusedElement, direction);
						this.ReleaseFocus(focusedElement, newFocusedElement, direction);
					}
				}
				else if (newFocusedElement != focusedElement)
				{
					if (focusedElement != null)
					{
						this.AboutToReleaseFocus(focusedElement, newFocusedElement, direction);
					}
					this.AboutToGrabFocus(newFocusedElement, focusedElement, direction);
					if (focusedElement != null)
					{
						this.ReleaseFocus(focusedElement, newFocusedElement, direction);
					}
					this.GrabFocus(newFocusedElement, focusedElement, direction);
				}
			}
		}

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

		internal void SyncIMGUIFocus(int imguiKeyboardControlID, Focusable imguiContainerHavingKeyboardControl)
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
