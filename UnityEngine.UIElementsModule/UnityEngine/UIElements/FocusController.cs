using System;
using System.Collections.Generic;
using UnityEngine.Bindings;
using UnityEngine.Pool;

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

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal TextElement selectedTextElement
		{
			get
			{
				return this.m_SelectedTextElement;
			}
			set
			{
				bool flag = this.m_SelectedTextElement == value;
				if (!flag)
				{
					TextElement selectedTextElement = this.m_SelectedTextElement;
					if (selectedTextElement != null)
					{
						selectedTextElement.selection.SelectNone();
					}
					this.m_SelectedTextElement = value;
				}
			}
		}

		public Focusable focusedElement
		{
			get
			{
				Focusable retargetedFocusedElement = this.GetRetargetedFocusedElement(null);
				return this.IsLocalElement(retargetedFocusedElement) ? retargetedFocusedElement : null;
			}
		}

		public void IgnoreEvent(EventBase evt)
		{
			evt.processedByFocusController = true;
			IMouseEventInternal mouseEventInternal = evt as IMouseEventInternal;
			EventBase eventBase;
			bool flag;
			if (mouseEventInternal != null)
			{
				eventBase = mouseEventInternal.sourcePointerEvent as EventBase;
				flag = eventBase != null;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				eventBase.processedByFocusController = true;
			}
		}

		internal bool IsFocused(Focusable f)
		{
			bool flag = !this.IsLocalElement(f);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				foreach (FocusController.FocusedElement focusedElement in this.m_FocusedElements)
				{
					bool flag3 = focusedElement.m_FocusedElement == f;
					if (flag3)
					{
						return true;
					}
				}
				flag2 = false;
			}
			return flag2;
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

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal Focusable GetLeafFocusedElement()
		{
			bool flag = this.m_FocusedElements.Count > 0;
			Focusable focusable;
			if (flag)
			{
				VisualElement focusedElement = this.m_FocusedElements[0].m_FocusedElement;
				focusable = (this.IsLocalElement(focusedElement) ? focusedElement : null);
			}
			else
			{
				focusable = null;
			}
			return focusable;
		}

		private bool IsLocalElement(Focusable f)
		{
			return ((f != null) ? f.focusController : null) == this;
		}

		internal void ClearPendingFocusEvents()
		{
			this.m_PendingFocusCount = 0;
			this.m_LastPendingFocusedElement = null;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal bool IsPendingFocus(Focusable f)
		{
			for (VisualElement visualElement = this.m_LastPendingFocusedElement as VisualElement; visualElement != null; visualElement = visualElement.hierarchy.parent)
			{
				bool flag = f == visualElement;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		internal void SetFocusToLastFocusedElement()
		{
			bool flag = this.m_LastFocusedElement != null && !(this.m_LastFocusedElement is IMGUIContainer);
			if (flag)
			{
				this.m_LastFocusedElement.Focus();
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void BlurLastFocusedElement()
		{
			this.selectedTextElement = null;
			bool flag = this.m_LastFocusedElement != null && !(this.m_LastFocusedElement is IMGUIContainer);
			if (flag)
			{
				Focusable lastFocusedElement = this.m_LastFocusedElement;
				this.m_LastFocusedElement.Blur();
				this.m_LastFocusedElement = lastFocusedElement;
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void DoFocusChange(Focusable f)
		{
			this.m_FocusedElements.Clear();
			FocusController.GetFocusTargets(f, this.m_FocusedElements);
		}

		internal void ProcessPendingFocusChange(Focusable f)
		{
			this.m_PendingFocusCount--;
			bool flag = this.m_PendingFocusCount == 0;
			if (flag)
			{
				this.m_LastPendingFocusedElement = null;
			}
			foreach (FocusController.FocusedElement focusedElement in this.m_FocusedElements)
			{
				focusedElement.m_FocusedElement.pseudoStates &= ~PseudoStates.Focus;
			}
			this.DoFocusChange(f);
			foreach (FocusController.FocusedElement focusedElement2 in this.m_FocusedElements)
			{
				focusedElement2.m_FocusedElement.pseudoStates |= PseudoStates.Focus;
			}
		}

		private static void GetFocusTargets(Focusable f, List<FocusController.FocusedElement> outTargets)
		{
			VisualElement visualElement = f as VisualElement;
			for (VisualElement visualElement2 = visualElement; visualElement2 != null; visualElement2 = visualElement2.hierarchy.parent)
			{
				bool flag = visualElement2.hierarchy.parent == null || visualElement2.isCompositeRoot;
				if (flag)
				{
					outTargets.Add(new FocusController.FocusedElement
					{
						m_SubTreeRoot = visualElement2,
						m_FocusedElement = visualElement
					});
					visualElement = visualElement2;
				}
			}
		}

		internal Focusable FocusNextInDirection(Focusable currentFocusable, FocusChangeDirection direction)
		{
			Focusable nextFocusable = this.focusRing.GetNextFocusable(currentFocusable, direction);
			direction.ApplyTo(this, nextFocusable);
			return nextFocusable;
		}

		private void AboutToReleaseFocus(Focusable focusable, Focusable willGiveFocusTo, FocusChangeDirection direction, DispatchMode dispatchMode)
		{
			using (FocusOutEvent pooled = FocusEventBase<FocusOutEvent>.GetPooled(focusable, willGiveFocusTo, direction, this, false))
			{
				focusable.SendEvent(pooled, dispatchMode);
			}
		}

		private void ReleaseFocus(Focusable focusable, Focusable willGiveFocusTo, FocusChangeDirection direction, DispatchMode dispatchMode)
		{
			List<FocusController.FocusedElement> list;
			using (CollectionPool<List<FocusController.FocusedElement>, FocusController.FocusedElement>.Get(out list))
			{
				FocusController.GetFocusTargets(focusable, list);
				foreach (FocusController.FocusedElement focusedElement in list)
				{
					using (BlurEvent pooled = FocusEventBase<BlurEvent>.GetPooled(focusedElement.m_FocusedElement, willGiveFocusTo, direction, this, false))
					{
						pooled.target = focusedElement.m_FocusedElement;
						focusable.SendEvent(pooled, dispatchMode);
					}
				}
			}
		}

		private void AboutToGrabFocus(Focusable focusable, Focusable willTakeFocusFrom, FocusChangeDirection direction, DispatchMode dispatchMode)
		{
			using (FocusInEvent pooled = FocusEventBase<FocusInEvent>.GetPooled(focusable, willTakeFocusFrom, direction, this, false))
			{
				focusable.SendEvent(pooled, dispatchMode);
			}
		}

		private void GrabFocus(Focusable focusable, Focusable willTakeFocusFrom, FocusChangeDirection direction, bool bIsFocusDelegated, DispatchMode dispatchMode)
		{
			List<FocusController.FocusedElement> list;
			using (CollectionPool<List<FocusController.FocusedElement>, FocusController.FocusedElement>.Get(out list))
			{
				FocusController.GetFocusTargets(focusable, list);
				foreach (FocusController.FocusedElement focusedElement in list)
				{
					using (FocusEvent pooled = FocusEventBase<FocusEvent>.GetPooled(focusedElement.m_FocusedElement, willTakeFocusFrom, direction, this, bIsFocusDelegated))
					{
						pooled.target = focusedElement.m_FocusedElement;
						focusable.SendEvent(pooled, dispatchMode);
					}
				}
			}
		}

		internal void Blur(Focusable focusable, bool bIsFocusDelegated = false, DispatchMode dispatchMode = DispatchMode.Default)
		{
			bool flag = ((this.m_PendingFocusCount > 0) ? this.IsPendingFocus(focusable) : this.IsFocused(focusable));
			bool flag2 = flag;
			if (flag2)
			{
				this.SwitchFocus(null, bIsFocusDelegated, dispatchMode);
			}
		}

		internal void SwitchFocus(Focusable newFocusedElement, bool bIsFocusDelegated = false, DispatchMode dispatchMode = DispatchMode.Default)
		{
			this.SwitchFocus(newFocusedElement, FocusChangeDirection.unspecified, bIsFocusDelegated, dispatchMode);
		}

		internal void SwitchFocus(Focusable newFocusedElement, FocusChangeDirection direction, bool bIsFocusDelegated = false, DispatchMode dispatchMode = DispatchMode.Default)
		{
			this.m_LastFocusedElement = newFocusedElement;
			Focusable focusable = ((this.m_PendingFocusCount > 0) ? this.m_LastPendingFocusedElement : this.GetLeafFocusedElement());
			bool flag = focusable == newFocusedElement;
			if (!flag)
			{
				VisualElement visualElement = newFocusedElement as VisualElement;
				IPanel panel;
				bool flag2;
				if (visualElement != null && newFocusedElement.canGrabFocus)
				{
					panel = visualElement.panel;
					flag2 = panel != null;
				}
				else
				{
					flag2 = false;
				}
				bool flag3 = flag2;
				if (flag3)
				{
					Focusable focusable2 = ((visualElement != null) ? visualElement.RetargetElement(focusable as VisualElement) : null) ?? newFocusedElement;
					VisualElement visualElement2 = focusable as VisualElement;
					Focusable focusable3 = ((visualElement2 != null) ? visualElement2.RetargetElement(visualElement) : null) ?? focusable;
					this.m_LastPendingFocusedElement = newFocusedElement;
					this.m_PendingFocusCount++;
					using (new EventDispatcherGate(panel.dispatcher))
					{
						bool flag4 = focusable != null;
						if (flag4)
						{
							this.AboutToReleaseFocus(focusable, focusable2, direction, dispatchMode);
						}
						this.AboutToGrabFocus(newFocusedElement, focusable3, direction, dispatchMode);
						bool flag5 = focusable != null;
						if (flag5)
						{
							this.ReleaseFocus(focusable, focusable2, direction, dispatchMode);
						}
						this.GrabFocus(newFocusedElement, focusable3, direction, bIsFocusDelegated, dispatchMode);
					}
				}
				else
				{
					VisualElement visualElement3 = focusable as VisualElement;
					bool flag6 = visualElement3 != null;
					if (flag6)
					{
						BaseVisualElementPanel elementPanel = visualElement3.elementPanel;
						bool flag7 = elementPanel != null;
						if (flag7)
						{
							this.m_LastPendingFocusedElement = null;
							this.m_PendingFocusCount++;
							using (new EventDispatcherGate(elementPanel.dispatcher))
							{
								this.AboutToReleaseFocus(focusable, null, direction, dispatchMode);
								this.ReleaseFocus(focusable, null, direction, dispatchMode);
							}
						}
						else
						{
							this.ProcessPendingFocusChange(null);
						}
					}
				}
			}
		}

		internal void SwitchFocusOnEvent(Focusable currentFocusable, EventBase e)
		{
			bool processedByFocusController = e.processedByFocusController;
			if (!processedByFocusController)
			{
				using (FocusChangeDirection focusChangeDirection = this.focusRing.GetFocusChangeDirection(currentFocusable, e))
				{
					bool flag = focusChangeDirection != FocusChangeDirection.none;
					if (flag)
					{
						this.FocusNextInDirection(currentFocusable, focusChangeDirection);
						e.processedByFocusController = true;
					}
				}
			}
		}

		internal void ReevaluateFocus()
		{
			VisualElement visualElement = this.focusedElement as VisualElement;
			bool flag = visualElement != null;
			if (flag)
			{
				bool flag2 = !visualElement.areAncestorsAndSelfDisplayed || !visualElement.visible;
				if (flag2)
				{
					visualElement.Blur();
				}
			}
		}

		internal bool GetFocusableParentForPointerEvent(Focusable target, out Focusable effectiveTarget)
		{
			bool flag = target == null || !target.focusable;
			bool flag2;
			if (flag)
			{
				effectiveTarget = target;
				flag2 = target != null;
			}
			else
			{
				effectiveTarget = target;
				for (;;)
				{
					VisualElement visualElement = effectiveTarget as VisualElement;
					bool flag3 = visualElement != null && (!visualElement.enabledInHierarchy || !visualElement.focusable || !visualElement.isEligibleToReceiveFocusFromDisabledChild) && visualElement.hierarchy.parent != null;
					if (!flag3)
					{
						break;
					}
					effectiveTarget = visualElement.hierarchy.parent;
				}
				flag2 = !this.IsFocused(effectiveTarget);
			}
			return flag2;
		}

		internal int imguiKeyboardControl { get; set; }

		internal void SyncIMGUIFocus(int imguiKeyboardControlID, Focusable imguiContainerHavingKeyboardControl, bool forceSwitch)
		{
			this.imguiKeyboardControl = imguiKeyboardControlID;
			bool flag = forceSwitch || this.imguiKeyboardControl != 0;
			if (flag)
			{
				this.SwitchFocus(imguiContainerHavingKeyboardControl, FocusChangeDirection.unspecified, false, DispatchMode.Default);
			}
			else
			{
				this.SwitchFocus(null, FocusChangeDirection.unspecified, false, DispatchMode.Default);
			}
		}

		private TextElement m_SelectedTextElement;

		private List<FocusController.FocusedElement> m_FocusedElements = new List<FocusController.FocusedElement>();

		private Focusable m_LastFocusedElement;

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal Focusable m_LastPendingFocusedElement;

		private int m_PendingFocusCount = 0;

		private struct FocusedElement
		{
			public VisualElement m_SubTreeRoot;

			public VisualElement m_FocusedElement;
		}
	}
}
