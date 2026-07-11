using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	public class IMGUIContainer : VisualElement, IDisposable
	{
		public Action onGUIHandler
		{
			get
			{
				return this.m_OnGUIHandler;
			}
			set
			{
				bool flag = this.m_OnGUIHandler != value;
				if (flag)
				{
					this.m_OnGUIHandler = value;
					base.IncrementVersion(VersionChangeType.Layout);
					base.IncrementVersion(VersionChangeType.Repaint);
				}
			}
		}

		internal ObjectGUIState guiState
		{
			get
			{
				Debug.Assert(!this.useOwnerObjectGUIState);
				bool flag = this.m_ObjectGUIState == null;
				if (flag)
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
				bool flag = this.m_Cache == null;
				if (flag)
				{
					this.m_Cache = new GUILayoutUtility.LayoutCache();
				}
				return this.m_Cache;
			}
		}

		private float layoutMeasuredWidth
		{
			get
			{
				return Mathf.Ceil(this.cache.topLevel.maxWidth);
			}
		}

		private float layoutMeasuredHeight
		{
			get
			{
				return Mathf.Ceil(this.cache.topLevel.maxHeight);
			}
		}

		public ContextType contextType { get; set; }

		internal bool focusOnlyIfHasFocusableControls { get; set; } = true;

		public override bool canGrabFocus
		{
			get
			{
				return this.focusOnlyIfHasFocusableControls ? (this.hasFocusableControls && base.canGrabFocus) : base.canGrabFocus;
			}
		}

		public IMGUIContainer()
			: this(null)
		{
		}

		public IMGUIContainer(Action onGUIHandler)
		{
			this.isIMGUIContainer = true;
			base.AddToClassList(IMGUIContainer.ussClassName);
			this.onGUIHandler = onGUIHandler;
			this.contextType = ContextType.Editor;
			base.focusable = true;
			base.requireMeasureFunction = true;
			base.generateVisualContent = (Action<MeshGenerationContext>)Delegate.Combine(base.generateVisualContent, new Action<MeshGenerationContext>(this.OnGenerateVisualContent));
		}

		private void OnGenerateVisualContent(MeshGenerationContext mgc)
		{
			this.lastWorldClip = base.elementPanel.repaintData.currentWorldClip;
			mgc.painter.DrawImmediate(new Action(this.DoIMGUIRepaint));
		}

		private void SaveGlobals()
		{
			this.m_GUIGlobals.matrix = GUI.matrix;
			this.m_GUIGlobals.color = GUI.color;
			this.m_GUIGlobals.contentColor = GUI.contentColor;
			this.m_GUIGlobals.backgroundColor = GUI.backgroundColor;
			this.m_GUIGlobals.enabled = GUI.enabled;
			this.m_GUIGlobals.changed = GUI.changed;
			bool flag = Event.current != null;
			if (flag)
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
			bool flag = Event.current != null;
			if (flag)
			{
				Event.current.displayIndex = this.m_GUIGlobals.displayIndex;
			}
		}

		private void DoOnGUI(Event evt, Matrix4x4 parentTransform, Rect clippingRect, bool isComputingLayout, Rect layoutSize, Action onGUIHandler, bool canAffectFocus = true)
		{
			bool flag = onGUIHandler == null || base.panel == null;
			if (!flag)
			{
				int num = GUIClip.Internal_GetCount();
				this.SaveGlobals();
				float layoutMeasuredWidth = this.layoutMeasuredWidth;
				float layoutMeasuredHeight = this.layoutMeasuredHeight;
				UIElementsUtility.BeginContainerGUI(this.cache, evt, this);
				GUI.color = UIElementsUtility.editorPlayModeTintColor;
				bool flag2 = Event.current.type != EventType.Layout;
				if (flag2)
				{
					bool flag3 = this.lostFocus;
					if (flag3)
					{
						bool flag4 = this.focusController != null;
						if (flag4)
						{
							bool flag5 = GUIUtility.OwnsId(GUIUtility.keyboardControl);
							if (flag5)
							{
								GUIUtility.keyboardControl = 0;
								this.focusController.imguiKeyboardControl = 0;
							}
						}
						this.lostFocus = false;
					}
					bool flag6 = this.receivedFocus;
					if (flag6)
					{
						bool flag7 = this.hasFocusableControls;
						if (flag7)
						{
							bool flag8 = this.focusChangeDirection != FocusChangeDirection.unspecified && this.focusChangeDirection != FocusChangeDirection.none;
							if (flag8)
							{
								bool flag9 = this.focusChangeDirection == VisualElementFocusChangeDirection.left;
								if (flag9)
								{
									GUIUtility.SetKeyboardControlToLastControlId();
								}
								else
								{
									bool flag10 = this.focusChangeDirection == VisualElementFocusChangeDirection.right;
									if (flag10)
									{
										GUIUtility.SetKeyboardControlToFirstControlId();
									}
								}
							}
							else
							{
								bool flag11 = GUIUtility.keyboardControl == 0;
								if (flag11)
								{
									GUIUtility.SetKeyboardControlToFirstControlId();
								}
							}
						}
						this.receivedFocus = false;
						this.focusChangeDirection = FocusChangeDirection.unspecified;
						bool flag12 = this.focusController != null;
						if (flag12)
						{
							this.focusController.imguiKeyboardControl = GUIUtility.keyboardControl;
						}
					}
				}
				EventType type = Event.current.type;
				bool flag13 = false;
				try
				{
					using (new GUIClip.ParentClipScope(parentTransform, clippingRect))
					{
						onGUIHandler();
					}
				}
				catch (Exception ex)
				{
					bool flag14 = type == EventType.Layout;
					if (!flag14)
					{
						throw;
					}
					flag13 = GUIUtility.IsExitGUIException(ex);
					bool flag15 = !flag13;
					if (flag15)
					{
						Debug.LogException(ex);
					}
				}
				finally
				{
					bool flag16 = Event.current.type != EventType.Layout && canAffectFocus;
					if (flag16)
					{
						int keyboardControl = GUIUtility.keyboardControl;
						int num2 = GUIUtility.CheckForTabEvent(Event.current);
						bool flag17 = this.focusController != null;
						if (flag17)
						{
							bool flag18 = num2 < 0;
							if (flag18)
							{
								Focusable leafFocusedElement = this.focusController.GetLeafFocusedElement();
								Focusable focusable = null;
								using (KeyDownEvent pooled = KeyboardEventBase<KeyDownEvent>.GetPooled('\t', KeyCode.Tab, (num2 == -1) ? EventModifiers.None : EventModifiers.Shift))
								{
									focusable = this.focusController.SwitchFocusOnEvent(pooled);
								}
								bool flag19 = leafFocusedElement == this;
								if (flag19)
								{
									bool flag20 = focusable == this;
									if (flag20)
									{
										bool flag21 = num2 == -2;
										if (flag21)
										{
											GUIUtility.SetKeyboardControlToLastControlId();
										}
										else
										{
											bool flag22 = num2 == -1;
											if (flag22)
											{
												GUIUtility.SetKeyboardControlToFirstControlId();
											}
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
							else
							{
								bool flag23 = num2 > 0;
								if (flag23)
								{
									this.focusController.imguiKeyboardControl = GUIUtility.keyboardControl;
									this.newKeyboardFocusControlID = GUIUtility.keyboardControl;
								}
								else
								{
									bool flag24 = num2 == 0;
									if (flag24)
									{
										bool flag25 = type == EventType.MouseDown && !this.focusOnlyIfHasFocusableControls;
										if (flag25)
										{
											this.focusController.SyncIMGUIFocus(GUIUtility.keyboardControl, this, true);
										}
										else
										{
											bool flag26 = keyboardControl != GUIUtility.keyboardControl || type == EventType.MouseDown;
											if (flag26)
											{
												this.focusController.SyncIMGUIFocus(GUIUtility.keyboardControl, this, false);
											}
											else
											{
												bool flag27 = GUIUtility.keyboardControl != this.focusController.imguiKeyboardControl;
												if (flag27)
												{
													this.newKeyboardFocusControlID = GUIUtility.keyboardControl;
													bool flag28 = this.focusController.GetLeafFocusedElement() == this;
													if (flag28)
													{
														this.focusController.imguiKeyboardControl = GUIUtility.keyboardControl;
													}
													else
													{
														this.focusController.SyncIMGUIFocus(GUIUtility.keyboardControl, this, false);
													}
												}
											}
										}
									}
								}
							}
						}
						this.hasFocusableControls = GUIUtility.HasFocusableControls();
					}
				}
				UIElementsUtility.EndContainerGUI(evt, layoutSize);
				this.RestoreGlobals();
				bool flag29 = !isComputingLayout && evt.type == EventType.Layout && (!Mathf.Approximately(layoutMeasuredWidth, this.layoutMeasuredWidth) || !Mathf.Approximately(layoutMeasuredHeight, this.layoutMeasuredHeight));
				if (flag29)
				{
					base.IncrementVersion(VersionChangeType.Layout);
				}
				bool flag30 = !flag13;
				if (flag30)
				{
					bool flag31 = evt.type != EventType.Ignore && evt.type != EventType.Used;
					if (flag31)
					{
						int num3 = GUIClip.Internal_GetCount();
						bool flag32 = num3 > num;
						if (flag32)
						{
							Debug.LogError("GUI Error: You are pushing more GUIClips than you are popping. Make sure they are balanced.");
						}
						else
						{
							bool flag33 = num3 < num;
							if (flag33)
							{
								Debug.LogError("GUI Error: You are popping more GUIClips than you are pushing. Make sure they are balanced.");
							}
						}
					}
				}
				while (GUIClip.Internal_GetCount() > num)
				{
					GUIClip.Internal_Pop();
				}
				bool flag34 = evt.type == EventType.Used;
				if (flag34)
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
			bool flag = evt == null;
			if (!flag)
			{
				bool flag2 = evt.propagationPhase != PropagationPhase.TrickleDown && evt.propagationPhase != PropagationPhase.AtTarget && evt.propagationPhase != PropagationPhase.BubbleUp;
				if (!flag2)
				{
					bool flag3 = evt.imguiEvent == null;
					if (!flag3)
					{
						bool isPropagationStopped = evt.isPropagationStopped;
						if (!isPropagationStopped)
						{
							bool flag4 = this.SendEventToIMGUI(evt, true);
							if (flag4)
							{
								evt.StopPropagation();
								evt.PreventDefault();
							}
						}
					}
				}
			}
		}

		private void DoIMGUIRepaint()
		{
			Matrix4x4 currentOffset = base.elementPanel.repaintData.currentOffset;
			this.m_CachedClippingRect = VisualElement.ComputeAAAlignedBound(base.worldClip, currentOffset);
			this.m_CachedTransform = currentOffset * base.worldTransform;
			this.HandleIMGUIEvent(base.elementPanel.repaintData.repaintEvent, this.m_CachedTransform, this.m_CachedClippingRect, this.onGUIHandler, true);
		}

		internal bool SendEventToIMGUI(EventBase evt, bool canAffectFocus = true)
		{
			bool flag = evt is IPointerEvent;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3;
				using (new EventDebuggerLogIMGUICall(evt))
				{
					flag3 = this.HandleIMGUIEvent(evt.imguiEvent, canAffectFocus);
				}
				flag2 = flag3;
			}
			return flag2;
		}

		internal bool HandleIMGUIEvent(Event e, bool canAffectFocus)
		{
			return this.HandleIMGUIEvent(e, this.onGUIHandler, canAffectFocus);
		}

		internal bool HandleIMGUIEvent(Event e, Action onGUIHandler, bool canAffectFocus)
		{
			IMGUIContainer.GetCurrentTransformAndClip(this, e, out this.m_CachedTransform, out this.m_CachedClippingRect);
			return this.HandleIMGUIEvent(e, this.m_CachedTransform, this.m_CachedClippingRect, onGUIHandler, canAffectFocus);
		}

		private bool HandleIMGUIEvent(Event e, Matrix4x4 worldTransform, Rect clippingRect, Action onGUIHandler, bool canAffectFocus)
		{
			bool flag = e == null || onGUIHandler == null || base.elementPanel == null || !base.elementPanel.IMGUIEventInterests.WantsEvent(e.rawType);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				EventType rawType = e.rawType;
				e.type = EventType.Layout;
				this.DoOnGUI(e, worldTransform, clippingRect, false, base.layout, onGUIHandler, canAffectFocus);
				e.type = rawType;
				this.DoOnGUI(e, worldTransform, clippingRect, false, base.layout, onGUIHandler, canAffectFocus);
				bool flag3 = this.newKeyboardFocusControlID > 0;
				if (flag3)
				{
					this.newKeyboardFocusControlID = 0;
					Event @event = new Event
					{
						type = EventType.ExecuteCommand,
						commandName = "NewKeyboardFocus"
					};
					this.HandleIMGUIEvent(@event, true);
				}
				bool flag4 = e.rawType == EventType.Used;
				if (flag4)
				{
					flag2 = true;
				}
				else
				{
					bool flag5 = e.rawType == EventType.MouseUp && this.HasMouseCapture();
					if (flag5)
					{
						GUIUtility.hotControl = 0;
					}
					bool flag6 = base.elementPanel == null;
					if (flag6)
					{
						GUIUtility.ExitGUI();
					}
					flag2 = false;
				}
			}
			return flag2;
		}

		protected override void ExecuteDefaultAction(EventBase evt)
		{
			bool flag = evt == null;
			if (!flag)
			{
				bool flag2 = evt.eventTypeId == EventBase<BlurEvent>.TypeId();
				if (flag2)
				{
					this.lostFocus = true;
					base.IncrementVersion(VersionChangeType.Repaint);
				}
				else
				{
					bool flag3 = evt.eventTypeId == EventBase<FocusEvent>.TypeId();
					if (flag3)
					{
						FocusEvent focusEvent = evt as FocusEvent;
						this.receivedFocus = true;
						this.focusChangeDirection = focusEvent.direction;
					}
					else
					{
						bool flag4 = evt.eventTypeId == EventBase<DetachFromPanelEvent>.TypeId();
						if (flag4)
						{
							bool flag5 = base.elementPanel != null;
							if (flag5)
							{
								BaseVisualElementPanel elementPanel = base.elementPanel;
								int num = elementPanel.IMGUIContainersCount;
								elementPanel.IMGUIContainersCount = num - 1;
							}
						}
						else
						{
							bool flag6 = evt.eventTypeId == EventBase<AttachToPanelEvent>.TypeId();
							if (flag6)
							{
								bool flag7 = base.elementPanel != null;
								if (flag7)
								{
									BaseVisualElementPanel elementPanel2 = base.elementPanel;
									int num = elementPanel2.IMGUIContainersCount;
									elementPanel2.IMGUIContainersCount = num + 1;
								}
							}
						}
					}
				}
			}
		}

		protected internal override Vector2 DoMeasure(float desiredWidth, VisualElement.MeasureMode widthMode, float desiredHeight, VisualElement.MeasureMode heightMode)
		{
			float num = float.NaN;
			float num2 = float.NaN;
			bool flag = widthMode != VisualElement.MeasureMode.Exactly || heightMode != VisualElement.MeasureMode.Exactly;
			if (flag)
			{
				Event @event = new Event
				{
					type = EventType.Layout
				};
				Rect layout = base.layout;
				if (widthMode == VisualElement.MeasureMode.Exactly)
				{
					layout.width = desiredWidth;
				}
				if (heightMode == VisualElement.MeasureMode.Exactly)
				{
					layout.height = desiredHeight;
				}
				this.DoOnGUI(@event, this.m_CachedTransform, this.m_CachedClippingRect, true, layout, this.onGUIHandler, true);
				num = this.layoutMeasuredWidth;
				num2 = this.layoutMeasuredHeight;
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
			bool flag = clipRect.width == 0f || clipRect.height == 0f;
			if (flag)
			{
				clipRect = container.worldBound;
			}
			transform = container.worldTransform;
			bool flag2 = evt.rawType == EventType.Repaint && container.elementPanel != null;
			if (flag2)
			{
				transform = container.elementPanel.repaintData.currentOffset * container.worldTransform;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposeManaged)
		{
			if (disposeManaged)
			{
				ObjectGUIState objectGUIState = this.m_ObjectGUIState;
				if (objectGUIState != null)
				{
					objectGUIState.Dispose();
				}
			}
		}

		private Action m_OnGUIHandler;

		private ObjectGUIState m_ObjectGUIState;

		internal bool useOwnerObjectGUIState;

		private GUILayoutUtility.LayoutCache m_Cache = null;

		private Rect m_CachedClippingRect = Rect.zero;

		private Matrix4x4 m_CachedTransform = Matrix4x4.identity;

		private bool lostFocus = false;

		private bool receivedFocus = false;

		private FocusChangeDirection focusChangeDirection = FocusChangeDirection.unspecified;

		private bool hasFocusableControls = false;

		private int newKeyboardFocusControlID = 0;

		public static readonly string ussClassName = "unity-imgui-container";

		private IMGUIContainer.GUIGlobals m_GUIGlobals;

		public new class UxmlFactory : UxmlFactory<IMGUIContainer, IMGUIContainer.UxmlTraits>
		{
		}

		public new class UxmlTraits : VisualElement.UxmlTraits
		{
			public UxmlTraits()
			{
				base.focusIndex.defaultValue = 0;
				base.focusable.defaultValue = true;
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
