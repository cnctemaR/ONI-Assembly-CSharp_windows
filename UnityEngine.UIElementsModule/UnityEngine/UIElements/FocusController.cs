using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	public class FocusController
	{
		public FocusController(IFocusRing focusRing)
		{
			this.focusRing = focusRing;
			this.imguiKeyboardControl = 0;
		}

		private IFocusRing focusRing { get; }

		public Focusable focusedElement
		{
			get
			{
				return this.GetRetargetedFocusedElement(null);
			}
		}

		internal bool IsFocused(Focusable f)
		{
			foreach (FocusController.FocusedElement focusedElement in this.m_FocusedElements)
			{
				bool flag = focusedElement.m_FocusedElement == f;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		internal Focusable GetRetargetedFocusedElement(VisualElement retargetAgainst)
		{
			VisualElement visualElement = ((retargetAgainst != null) ? retargetAgainst.hierarchy.parent : null);
			bool flag = visualElement == null;
			if (flag)
			{
				bool flag2 = this.m_FocusedElements.Count > 0;
				if (flag2)
				{
					return this.m_FocusedElements[this.m_FocusedElements.Count - 1].m_FocusedElement;
				}
			}
			else
			{
				while (!visualElement.isCompositeRoot && visualElement.hierarchy.parent != null)
				{
					visualElement = visualElement.hierarchy.parent;
				}
				foreach (FocusController.FocusedElement focusedElement in this.m_FocusedElements)
				{
					bool flag3 = focusedElement.m_SubTreeRoot == visualElement;
					if (flag3)
					{
						return focusedElement.m_FocusedElement;
					}
				}
			}
			return null;
		}

		internal Focusable GetLeafFocusedElement()
		{
			bool flag = this.m_FocusedElements.Count > 0;
			Focusable focusable;
			if (flag)
			{
				focusable = this.m_FocusedElements[0].m_FocusedElement;
			}
			else
			{
				focusable = null;
			}
			return focusable;
		}

		internal void SetFocusToLastFocusedElement()
		{
			bool flag = this.m_LastFocusedElement != null && !(this.m_LastFocusedElement is IMGUIContainer);
			if (flag)
			{
				this.m_LastFocusedElement.Focus();
			}
			this.m_LastFocusedElement = null;
		}

		internal void BlurLastFocusedElement()
		{
			bool flag = this.m_LastFocusedElement != null && !(this.m_LastFocusedElement is IMGUIContainer);
			if (flag)
			{
				Focusable lastFocusedElement = this.m_LastFocusedElement;
				this.m_LastFocusedElement.Blur();
				this.m_LastFocusedElement = lastFocusedElement;
			}
		}

		internal void DoFocusChange(Focusable f)
		{
			this.m_FocusedElements.Clear();
			VisualElement visualElement = f as VisualElement;
			bool flag = !(f is IMGUIContainer);
			if (flag)
			{
				this.m_LastFocusedElement = f;
			}
			while (visualElement != null)
			{
				bool flag2 = visualElement.hierarchy.parent == null || visualElement.isCompositeRoot;
				if (flag2)
				{
					this.m_FocusedElements.Add(new FocusController.FocusedElement
					{
						m_SubTreeRoot = visualElement,
						m_FocusedElement = f
					});
					f = visualElement;
				}
				visualElement = visualElement.hierarchy.parent;
			}
		}

		private void AboutToReleaseFocus(Focusable focusable, Focusable willGiveFocusTo, FocusChangeDirection direction)
		{
			using (FocusOutEvent pooled = FocusEventBase<FocusOutEvent>.GetPooled(focusable, willGiveFocusTo, direction, this, false))
			{
				focusable.SendEvent(pooled);
			}
		}

		private void ReleaseFocus(Focusable focusable, Focusable willGiveFocusTo, FocusChangeDirection direction)
		{
			using (BlurEvent pooled = FocusEventBase<BlurEvent>.GetPooled(focusable, willGiveFocusTo, direction, this, false))
			{
				focusable.SendEvent(pooled);
			}
		}

		private void AboutToGrabFocus(Focusable focusable, Focusable willTakeFocusFrom, FocusChangeDirection direction)
		{
			using (FocusInEvent pooled = FocusEventBase<FocusInEvent>.GetPooled(focusable, willTakeFocusFrom, direction, this, false))
			{
				focusable.SendEvent(pooled);
			}
		}

		private void GrabFocus(Focusable focusable, Focusable willTakeFocusFrom, FocusChangeDirection direction, bool bIsFocusDelegated = false)
		{
			using (FocusEvent pooled = FocusEventBase<FocusEvent>.GetPooled(focusable, willTakeFocusFrom, direction, this, bIsFocusDelegated))
			{
				focusable.SendEvent(pooled);
			}
		}

		internal void SwitchFocus(Focusable newFocusedElement, bool bIsFocusDelegated = false)
		{
			this.SwitchFocus(newFocusedElement, FocusChangeDirection.unspecified, bIsFocusDelegated);
		}

		internal void SwitchFocus(Focusable newFocusedElement, FocusChangeDirection direction, bool bIsFocusDelegated = false)
		{
			bool flag = this.GetLeafFocusedElement() == newFocusedElement;
			if (!flag)
			{
				Focusable leafFocusedElement = this.GetLeafFocusedElement();
				bool flag2 = newFocusedElement == null || !newFocusedElement.canGrabFocus;
				if (flag2)
				{
					bool flag3 = leafFocusedElement != null;
					if (flag3)
					{
						this.AboutToReleaseFocus(leafFocusedElement, null, direction);
						this.ReleaseFocus(leafFocusedElement, null, direction);
					}
				}
				else
				{
					bool flag4 = newFocusedElement != leafFocusedElement;
					if (flag4)
					{
						VisualElement visualElement = newFocusedElement as VisualElement;
						VisualElement visualElement2 = ((visualElement != null) ? visualElement.RetargetElement(leafFocusedElement as VisualElement) : null);
						VisualElement visualElement3 = leafFocusedElement as VisualElement;
						VisualElement visualElement4 = ((visualElement3 != null) ? visualElement3.RetargetElement(newFocusedElement as VisualElement) : null);
						bool flag5 = leafFocusedElement != null;
						if (flag5)
						{
							this.AboutToReleaseFocus(leafFocusedElement, visualElement2, direction);
						}
						this.AboutToGrabFocus(newFocusedElement, visualElement4, direction);
						bool flag6 = leafFocusedElement != null;
						if (flag6)
						{
							this.ReleaseFocus(leafFocusedElement, visualElement2, direction);
						}
						this.GrabFocus(newFocusedElement, visualElement4, direction, bIsFocusDelegated);
					}
				}
			}
		}

		internal Focusable SwitchFocusOnEvent(EventBase e)
		{
			bool processedByFocusController = e.processedByFocusController;
			Focusable focusable;
			if (processedByFocusController)
			{
				focusable = this.GetLeafFocusedElement();
			}
			else
			{
				using (FocusChangeDirection focusChangeDirection = this.focusRing.GetFocusChangeDirection(this.GetLeafFocusedElement(), e))
				{
					bool flag = focusChangeDirection != FocusChangeDirection.none;
					if (flag)
					{
						Focusable nextFocusable = this.focusRing.GetNextFocusable(this.GetLeafFocusedElement(), focusChangeDirection);
						focusChangeDirection.ApplyTo(this, nextFocusable);
						e.processedByFocusController = true;
						return nextFocusable;
					}
				}
				focusable = this.GetLeafFocusedElement();
			}
			return focusable;
		}

		internal int imguiKeyboardControl { get; set; }

		internal void SyncIMGUIFocus(int imguiKeyboardControlID, Focusable imguiContainerHavingKeyboardControl, bool forceSwitch)
		{
			this.imguiKeyboardControl = imguiKeyboardControlID;
			bool flag = forceSwitch || this.imguiKeyboardControl != 0;
			if (flag)
			{
				this.SwitchFocus(imguiContainerHavingKeyboardControl, FocusChangeDirection.unspecified, false);
			}
			else
			{
				this.SwitchFocus(null, FocusChangeDirection.unspecified, false);
			}
		}

		private List<FocusController.FocusedElement> m_FocusedElements = new List<FocusController.FocusedElement>();

		private Focusable m_LastFocusedElement;

		private struct FocusedElement
		{
			public VisualElement m_SubTreeRoot;

			public Focusable m_FocusedElement;
		}
	}
}
