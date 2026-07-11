using System;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>Base class for objects that can get the focus.</para>
	/// </summary>
	public abstract class Focusable : CallbackEventHandler
	{
		protected Focusable()
		{
			this.m_FocusIndex = 0;
		}

		/// <summary>
		///   <para>Return the focus controller for this element.</para>
		/// </summary>
		public abstract FocusController focusController { get; }

		/// <summary>
		///   <para>An integer used to sort focusables in the focus ring. A negative value means that the element can not be focused.</para>
		/// </summary>
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

		/// <summary>
		///   <para>Return true if the element can be focused.</para>
		/// </summary>
		public virtual bool canGrabFocus
		{
			get
			{
				return this.m_FocusIndex >= 0;
			}
		}

		/// <summary>
		///   <para>Attempt to give the focus to this element.</para>
		/// </summary>
		public virtual void Focus()
		{
			if (this.focusController != null)
			{
				this.focusController.SwitchFocus((!this.canGrabFocus) ? null : this);
			}
		}

		/// <summary>
		///   <para>Tell the element to release the focus.</para>
		/// </summary>
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
