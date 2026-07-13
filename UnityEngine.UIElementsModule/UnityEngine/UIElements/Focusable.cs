using System;
using Unity.Properties;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	public abstract class Focusable : CallbackEventHandler
	{
		protected Focusable()
		{
			UIElementsRuntimeUtilityNative.VisualElementCreation();
			this.focusable = true;
			this.tabIndex = 0;
		}

		public abstract FocusController focusController { get; }

		[CreateProperty]
		public virtual bool focusable
		{
			get
			{
				return this.m_Focusable;
			}
			set
			{
				bool flag = this.m_Focusable == value;
				if (!flag)
				{
					this.m_Focusable = value;
					base.NotifyPropertyChanged(in Focusable.focusableProperty);
				}
			}
		}

		[CreateProperty]
		public int tabIndex
		{
			get
			{
				return this.m_TabIndex;
			}
			set
			{
				bool flag = this.m_TabIndex == value;
				if (!flag)
				{
					this.m_TabIndex = value;
					base.NotifyPropertyChanged(in Focusable.tabIndexProperty);
				}
			}
		}

		[CreateProperty]
		public bool delegatesFocus
		{
			get
			{
				return this.m_DelegatesFocus;
			}
			set
			{
				bool flag = this.m_DelegatesFocus == value;
				if (!flag)
				{
					this.m_DelegatesFocus = value;
					base.NotifyPropertyChanged(in Focusable.delegatesFocusProperty);
				}
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

		internal bool isEligibleToReceiveFocusFromDisabledChild { get; set; } = true;

		[CreateProperty(ReadOnly = true)]
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
					this.focusController.SwitchFocus(focusDelegate, this != focusDelegate, DispatchMode.Default);
				}
				else
				{
					this.focusController.SwitchFocus(null, false, DispatchMode.Default);
				}
			}
		}

		public virtual void Blur()
		{
			FocusController focusController = this.focusController;
			if (focusController != null)
			{
				focusController.Blur(this, false, DispatchMode.Default);
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void BlurImmediately()
		{
			FocusController focusController = this.focusController;
			if (focusController != null)
			{
				focusController.Blur(this, false, DispatchMode.Immediate);
			}
		}

		internal Focusable GetFocusDelegate()
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
			int childCount = ve.hierarchy.childCount;
			int i = 0;
			while (i < childCount)
			{
				VisualElement visualElement = ve.hierarchy[i];
				bool flag = visualElement.canGrabFocus && visualElement.tabIndex >= 0;
				if (!flag)
				{
					bool flag2 = visualElement.hierarchy.parent != null && visualElement == visualElement.hierarchy.parent.contentContainer;
					bool flag3 = !visualElement.isCompositeRoot && !flag2;
					if (flag3)
					{
						Focusable firstFocusableChild = Focusable.GetFirstFocusableChild(visualElement);
						bool flag4 = firstFocusableChild != null;
						if (flag4)
						{
							return firstFocusableChild;
						}
					}
					i++;
					continue;
				}
				return visualElement;
			}
			return null;
		}

		internal static readonly BindingId focusableProperty = "focusable";

		internal static readonly BindingId tabIndexProperty = "tabIndex";

		internal static readonly BindingId delegatesFocusProperty = "delegatesFocus";

		internal static readonly BindingId canGrabFocusProperty = "canGrabFocus";

		private bool m_Focusable;

		private int m_TabIndex;

		private bool m_DelegatesFocus;

		private bool m_ExcludeFromFocusRing;
	}
}
