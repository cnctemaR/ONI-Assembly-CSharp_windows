using System;

namespace UnityEngine.Experimental.UIElements
{
	public abstract class Focusable : CallbackEventHandler
	{
		protected Focusable()
		{
			this.m_FocusIndex = 0;
		}

		public abstract FocusController focusController { get; }

		public virtual int focusIndex
		{
			get
			{
				return this.m_FocusIndex;
			}
			set
			{
				this.m_FocusIndex = value;
			}
		}

		public virtual bool canGrabFocus
		{
			get
			{
				return this.m_FocusIndex >= 0;
			}
		}

		public virtual void Focus()
		{
			if (this.focusController != null)
			{
				this.focusController.SwitchFocus((!this.canGrabFocus) ? null : this);
			}
		}

		public virtual void Blur()
		{
			if (this.focusController != null && this.focusController.focusedElement == this)
			{
				this.focusController.SwitchFocus(null);
			}
		}

		protected internal override void ExecuteDefaultAction(EventBase evt)
		{
			base.ExecuteDefaultAction(evt);
			if (evt.GetEventTypeId() == EventBase<MouseDownEvent>.TypeId())
			{
				this.Focus();
			}
			if (this.focusController != null)
			{
				this.focusController.SwitchFocusOnEvent(evt);
			}
		}

		private int m_FocusIndex;
	}
}
