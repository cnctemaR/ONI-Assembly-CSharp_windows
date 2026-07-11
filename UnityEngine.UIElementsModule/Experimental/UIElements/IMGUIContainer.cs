using System;
using System.Collections.Generic;
using UnityEngine.Experimental.UIElements.StyleEnums;

namespace UnityEngine.Experimental.UIElements
{
	public class IMGUIContainer : VisualElement
	{
		public IMGUIContainer()
			: this(null)
		{
		}

		public IMGUIContainer(Action onGUIHandler)
		{
			this.m_OnGUIHandler = onGUIHandler;
			this.contextType = ContextType.Editor;
			this.focusIndex = 0;
			base.requireMeasureFunction = true;
			base.style.overflow = Overflow.Hidden;
		}

		internal ObjectGUIState guiState
		{
			get
			{
				Debug.Assert(!this.useOwnerObjectGUIState);
				if (this.m_ObjectGUIState == null)
				{
					this.m_ObjectGUIState = new ObjectGUIState();
				}
				return this.m_ObjectGUIState;
			}
		}

		internal Rect lastWorldClip { get; set; }

		private GUILayoutUtility.LayoutCache cache
		{
			get
			{
				if (this.m_Cache == null)
				{
					this.m_Cache = new GUILayoutUtility.LayoutCache();
				}
				return this.m_Cache;
			}
		}

		public ContextType contextType { get; set; }

		public override bool canGrabFocus
		{
			get
			{
				return base.canGrabFocus && this.hasFocusableControls;
			}
		}

		protected override void DoRepaint(IStylePainter painter)
		{
			this.lastWorldClip = base.elementPanel.repaintData.currentWorldClip;
			IStylePainterInternal stylePainterInternal = (IStylePainterInternal)painter;
			stylePainterInternal.DrawImmediate(new Action(this.HandleIMGUIEvent));
		}

		private void SaveGlobals()
		{
			this.m_GUIGlobals.matrix = GUI.matrix;
			this.m_GUIGlobals.color = GUI.color;
			this.m_GUIGlobals.contentColor = GUI.contentColor;
			this.m_GUIGlobals.backgroundColor = GUI.backgroundColor;
			this.m_GUIGlobals.enabled = GUI.enabled;
			this.m_GUIGlobals.changed = GUI.changed;
			if (Event.current != null)
			{
				this.m_GUIGlobals.displayIndex = Event.current.displayIndex;
			}
		}

		private void RestoreGlobals()
		{
			GUI.matrix = this.m_GUIGlobals.matrix;
			GUI.color = this.m_GUIGlobals.color;
			GUI.contentColor = this.m_GUIGlobals.contentColor;
			GUI.backgroundColor = this.m_GUIGlobals.backgroundColor;
			GUI.enabled = this.m_GUIGlobals.enabled;
			GUI.changed = this.m_GUIGlobals.changed;
			if (Event.current != null)
			{
				Event.current.displayIndex = this.m_GUIGlobals.displayIndex;
			}
		}

		private void DoOnGUI(Event evt, Matrix4x4 worldTransform, Rect clippingRect, bool isComputingLayout = false)
		{
			if (this.m_OnGUIHandler != null && base.panel != null)
			{
				int num = GUIClip.Internal_GetCount();
				this.SaveGlobals();
				UIElementsUtility.BeginContainerGUI(this.cache, evt, this);
				GUI.color = UIElementsUtility.editorPlayModeTintColor;
				if (Event.current.type != EventType.Layout)
				{
					if (this.lostFocus)
					{
						if (this.focusController != null)
						{
							if (this.focusController.focusedElement == null || this.focusController.focusedElement == this || !(this.focusController.focusedElement is IMGUIContainer) || this.useUIElementsFocusStyle)
							{
								GUIUtility.keyboardControl = 0;
								this.focusController.imguiKeyboardControl = 0;
							}
						}
						this.lostFocus = false;
					}
					if (this.receivedFocus)
					{
						if (this.focusChangeDirection != FocusChangeDirection.unspecified && this.focusChangeDirection != FocusChangeDirection.none)
						{
							if (this.focusChangeDirection == VisualElementFocusChangeDirection.left)
							{
								GUIUtility.SetKeyboardControlToLastControlId();
							}
							else if (this.focusChangeDirection == VisualElementFocusChangeDirection.right)
							{
								GUIUtility.SetKeyboardControlToFirstControlId();
							}
						}
						else if (this.useUIElementsFocusStyle)
						{
							if (this.focusController == null || this.focusController.imguiKeyboardControl == 0)
							{
								GUIUtility.SetKeyboardControlToFirstControlId();
							}
							else
							{
								GUIUtility.keyboardControl = this.focusController.imguiKeyboardControl;
							}
						}
						this.receivedFocus = false;
						this.focusChangeDirection = FocusChangeDirection.unspecified;
						if (this.focusController != null)
						{
							this.focusController.imguiKeyboardControl = GUIUtility.keyboardControl;
						}
					}
				}
				EventType type = Event.current.type;
				bool flag = false;
				try
				{
					if (!isComputingLayout)
					{
						using (new GUIClip.ParentClipScope(worldTransform, clippingRect))
						{
							this.m_OnGUIHandler();
						}
					}
					else
					{
						this.m_OnGUIHandler();
					}
				}
				catch (Exception ex)
				{
					if (type != EventType.Layout)
					{
						throw;
					}
					flag = GUIUtility.IsExitGUIException(ex);
					if (!flag)
					{
						Debug.LogException(ex);
					}
				}
				finally
				{
					if (Event.current.type != EventType.Layout)
					{
						int keyboardControl = GUIUtility.keyboardControl;
						int num2 = GUIUtility.CheckForTabEvent(Event.current);
						if (this.focusController != null)
						{
							if (num2 < 0)
							{
								Focusable focusedElement = this.focusController.focusedElement;
								using (KeyDownEvent pooled = KeyboardEventBase<KeyDownEvent>.GetPooled('\t', KeyCode.Tab, (num2 != -1) ? EventModifiers.Shift : EventModifiers.None))
								{
									this.focusController.SwitchFocusOnEvent(pooled);
								}
								if (focusedElement == this)
								{
									if (this.focusController.focusedElement == this)
									{
										if (num2 == -2)
										{
											GUIUtility.SetKeyboardControlToLastControlId();
										}
										else if (num2 == -1)
										{
											GUIUtility.SetKeyboardControlToFirstControlId();
										}
										this.newKeyboardFocusControlID = GUIUtility.keyboardControl;
										this.focusController.imguiKeyboardControl = GUIUtility.keyboardControl;
									}
									else
									{
										GUIUtility.keyboardControl = 0;
										this.focusController.imguiKeyboardControl = 0;
									}
								}
							}
							else if (num2 > 0)
							{
								this.focusController.imguiKeyboardControl = GUIUtility.keyboardControl;
								this.newKeyboardFocusControlID = GUIUtility.keyboardControl;
							}
							else if (num2 == 0)
							{
								if (keyboardControl != GUIUtility.keyboardControl || type == EventType.MouseDown)
								{
									this.focusController.SyncIMGUIFocus(GUIUtility.keyboardControl, this);
								}
								else if (GUIUtility.keyboardControl != this.focusController.imguiKeyboardControl)
								{
									this.newKeyboardFocusControlID = GUIUtility.keyboardControl;
									if (this.focusController.focusedElement == this)
									{
										this.focusController.imguiKeyboardControl = GUIUtility.keyboardControl;
									}
									else
									{
										this.focusController.SyncIMGUIFocus(GUIUtility.keyboardControl, this);
									}
								}
							}
						}
						this.hasFocusableControls = GUIUtility.HasFocusableControls();
					}
				}
				UIElementsUtility.EndContainerGUI(evt);
				this.RestoreGlobals();
				if (!flag)
				{
					if (evt.type != EventType.Ignore && evt.type != EventType.Used)
					{
						int num3 = GUIClip.Internal_GetCount();
						if (num3 > num)
						{
							Debug.LogError("GUI Error: You are pushing more GUIClips than you are popping. Make sure they are balanced.");
						}
						else if (num3 < num)
						{
							Debug.LogError("GUI Error: You are popping more GUIClips than you are pushing. Make sure they are balanced.");
						}
					}
				}
				while (GUIClip.Internal_GetCount() > num)
				{
					GUIClip.Internal_Pop();
				}
				if (evt.type == EventType.Used)
				{
					base.IncrementVersion(VersionChangeType.Repaint);
				}
			}
		}

		public void MarkDirtyLayout()
		{
			base.IncrementVersion(VersionChangeType.Layout);
		}

		public override void HandleEvent(EventBase evt)
		{
			base.HandleEvent(evt);
			if (evt.propagationPhase != PropagationPhase.DefaultAction)
			{
				if (evt.imguiEvent != null)
				{
					if (!evt.isPropagationStopped)
					{
						if (this.HandleIMGUIEvent(evt.imguiEvent))
						{
							evt.StopPropagation();
							evt.PreventDefault();
						}
					}
				}
			}
		}

		internal void HandleIMGUIEvent()
		{
			Matrix4x4 currentOffset = base.elementPanel.repaintData.currentOffset;
			this.HandleIMGUIEvent(base.elementPanel.repaintData.repaintEvent, currentOffset * base.worldTransform, VisualElement.ComputeAAAlignedBound(base.worldClip, currentOffset));
		}

		internal bool HandleIMGUIEvent(Event e)
		{
			Matrix4x4 matrix4x;
			Rect rect;
			IMGUIContainer.GetCurrentTransformAndClip(this, e, out matrix4x, out rect);
			return this.HandleIMGUIEvent(e, matrix4x, rect);
		}

		internal bool HandleIMGUIEvent(Event e, Matrix4x4 worldTransform, Rect clippingRect)
		{
			bool flag;
			if (e == null || this.m_OnGUIHandler == null || base.elementPanel == null || !base.elementPanel.IMGUIEventInterests.WantsEvent(e.type))
			{
				flag = false;
			}
			else
			{
				EventType type = e.type;
				e.type = EventType.Layout;
				this.DoOnGUI(e, worldTransform, clippingRect, false);
				e.type = type;
				this.DoOnGUI(e, worldTransform, clippingRect, false);
				if (this.newKeyboardFocusControlID > 0)
				{
					this.newKeyboardFocusControlID = 0;
					this.HandleIMGUIEvent(new Event
					{
						type = EventType.ExecuteCommand,
						commandName = "NewKeyboardFocus"
					});
				}
				if (e.type == EventType.Used)
				{
					flag = true;
				}
				else
				{
					if (e.type == EventType.MouseUp && this.HasMouseCapture())
					{
						GUIUtility.hotControl = 0;
					}
					if (base.elementPanel == null)
					{
						GUIUtility.ExitGUI();
					}
					flag = false;
				}
			}
			return flag;
		}

		protected internal override void ExecuteDefaultAction(EventBase evt)
		{
			if (evt.GetEventTypeId() == EventBase<BlurEvent>.TypeId())
			{
				this.lostFocus = true;
			}
			else if (evt.GetEventTypeId() == EventBase<FocusEvent>.TypeId())
			{
				FocusEvent focusEvent = evt as FocusEvent;
				this.receivedFocus = true;
				this.focusChangeDirection = focusEvent.direction;
			}
			else if (evt.GetEventTypeId() == EventBase<DetachFromPanelEvent>.TypeId())
			{
				if (base.elementPanel != null)
				{
					base.elementPanel.IMGUIContainersCount--;
				}
			}
			else if (evt.GetEventTypeId() == EventBase<AttachToPanelEvent>.TypeId())
			{
				if (base.elementPanel != null)
				{
					base.elementPanel.IMGUIContainersCount++;
				}
			}
		}

		protected internal override Vector2 DoMeasure(float desiredWidth, VisualElement.MeasureMode widthMode, float desiredHeight, VisualElement.MeasureMode heightMode)
		{
			float num = float.NaN;
			float num2 = float.NaN;
			if (widthMode != VisualElement.MeasureMode.Exactly || heightMode != VisualElement.MeasureMode.Exactly)
			{
				Event @event = new Event
				{
					type = EventType.Layout
				};
				this.DoOnGUI(@event, Matrix4x4.identity, Rect.zero, true);
				num = this.m_Cache.topLevel.minWidth;
				num2 = this.m_Cache.topLevel.minHeight;
			}
			if (widthMode != VisualElement.MeasureMode.Exactly)
			{
				if (widthMode == VisualElement.MeasureMode.AtMost)
				{
					num = Mathf.Min(num, desiredWidth);
				}
			}
			else
			{
				num = desiredWidth;
			}
			if (heightMode != VisualElement.MeasureMode.Exactly)
			{
				if (heightMode == VisualElement.MeasureMode.AtMost)
				{
					num2 = Mathf.Min(num2, desiredHeight);
				}
			}
			else
			{
				num2 = desiredHeight;
			}
			return new Vector2(num, num2);
		}

		private static void GetCurrentTransformAndClip(IMGUIContainer container, Event evt, out Matrix4x4 transform, out Rect clipRect)
		{
			clipRect = container.lastWorldClip;
			if (clipRect.width == 0f || clipRect.height == 0f)
			{
				clipRect = container.worldBound;
			}
			transform = container.worldTransform;
			if (evt.type == EventType.Repaint && container.elementPanel != null)
			{
				transform = container.elementPanel.repaintData.currentOffset * container.worldTransform;
			}
		}

		private readonly Action m_OnGUIHandler;

		private ObjectGUIState m_ObjectGUIState;

		internal bool useOwnerObjectGUIState;

		private GUILayoutUtility.LayoutCache m_Cache = null;

		internal bool useUIElementsFocusStyle;

		private bool lostFocus = false;

		private bool receivedFocus = false;

		private FocusChangeDirection focusChangeDirection = FocusChangeDirection.unspecified;

		private bool hasFocusableControls = false;

		private int newKeyboardFocusControlID = 0;

		private IMGUIContainer.GUIGlobals m_GUIGlobals;

		public new class UxmlFactory : UxmlFactory<IMGUIContainer, IMGUIContainer.UxmlTraits>
		{
		}

		public new class UxmlTraits : VisualElement.UxmlTraits
		{
			public UxmlTraits()
			{
				this.m_FocusIndex.defaultValue = 0;
			}

			public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
			{
				get
				{
					yield break;
				}
			}
		}

		private struct GUIGlobals
		{
			public Matrix4x4 matrix;

			public Color color;

			public Color contentColor;

			public Color backgroundColor;

			public bool enabled;

			public bool changed;

			public int displayIndex;
		}
	}
}
