using System;

namespace UnityEngine.UIElements
{
	public abstract class Focusable : CallbackEventHandler
	{
		protected Focusable()
		{
			this.focusable = true;
			this.tabIndex = 0;
		}

		public abstract FocusController focusController { get; }

		public bool focusable { get; set; }

		public int tabIndex { get; set; }

		public bool delegatesFocus
		{
			get
			{
				return this.m_DelegatesFocus;
			}
			set
			{
				bool flag = !((VisualElement)this).isCompositeRoot;
				if (flag)
				{
					throw new InvalidOperationException("delegatesFocus should only be set on composite roots.");
				}
				this.m_DelegatesFocus = value;
			}
		}

		internal bool excludeFromFocusRing
		{
			get
			{
				return this.m_ExcludeFromFocusRing;
			}
			set
			{
				bool flag = !((VisualElement)this).isCompositeRoot;
				if (flag)
				{
					throw new InvalidOperationException("excludeFromFocusRing should only be set on composite roots.");
				}
				this.m_ExcludeFromFocusRing = value;
			}
		}

		public virtual bool canGrabFocus
		{
			get
			{
				return this.focusable;
			}
		}

		public virtual void Focus()
		{
			bool flag = this.focusController != null;
			if (flag)
			{
				bool canGrabFocus = this.canGrabFocus;
				if (canGrabFocus)
				{
					Focusable focusDelegate = this.GetFocusDelegate();
					this.focusController.SwitchFocus(focusDelegate);
				}
				else
				{
					this.focusController.SwitchFocus(null);
				}
			}
		}

		public virtual void Blur()
		{
			bool flag = this.focusController != null;
			if (flag)
			{
				bool flag2 = this.focusController.IsFocused(this);
				if (flag2)
				{
					this.focusController.SwitchFocus(null);
				}
			}
		}

		private Focusable GetFocusDelegate()
		{
			Focusable focusable = this;
			while (focusable != null && focusable.delegatesFocus)
			{
				focusable = Focusable.GetFirstFocusableChild(focusable as VisualElement);
			}
			return focusable;
		}

		private static Focusable GetFirstFocusableChild(VisualElement ve)
		{
			foreach (VisualElement visualElement in ve.hierarchy.Children())
			{
				bool canGrabFocus = visualElement.canGrabFocus;
				if (canGrabFocus)
				{
					return visualElement;
				}
				bool flag = visualElement.hierarchy.parent != null && visualElement == visualElement.hierarchy.parent.contentContainer;
				bool flag2 = !visualElement.isCompositeRoot && !flag;
				if (flag2)
				{
					Focusable firstFocusableChild = Focusable.GetFirstFocusableChild(visualElement);
					bool flag3 = firstFocusableChild != null;
					if (flag3)
					{
						return firstFocusableChild;
					}
				}
			}
			return null;
		}

		protected override void ExecuteDefaultAction(EventBase evt)
		{
			base.ExecuteDefaultAction(evt);
			bool flag = evt != null && evt.target == evt.leafTarget;
			if (flag)
			{
				bool flag2 = evt.eventTypeId == EventBase<MouseDownEvent>.TypeId();
				if (flag2)
				{
					this.Focus();
					bool canGrabFocus = this.canGrabFocus;
					if (canGrabFocus)
					{
						evt.doNotSendToRootIMGUIContainer = true;
					}
				}
				FocusController focusController = this.focusController;
				if (focusController != null)
				{
					focusController.SwitchFocusOnEvent(evt);
				}
			}
		}

		private bool m_DelegatesFocus;

		private bool m_ExcludeFromFocusRing;

		internal bool isIMGUIContainer = false;
	}
}
