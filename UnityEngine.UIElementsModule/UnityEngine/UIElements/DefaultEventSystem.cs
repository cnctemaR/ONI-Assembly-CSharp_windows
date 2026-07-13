using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IntegerTime;
using UnityEngine.InputForUI;

namespace UnityEngine.UIElements
{
	internal class DefaultEventSystem
	{
		private bool isAppFocused
		{
			get
			{
				return Application.isFocused;
			}
		}

		private bool ShouldIgnoreEventsOnAppNotFocused()
		{
			OperatingSystemFamily operatingSystemFamily = SystemInfo.operatingSystemFamily;
			OperatingSystemFamily operatingSystemFamily2 = operatingSystemFamily;
			return operatingSystemFamily2 - OperatingSystemFamily.MacOSX <= 2;
		}

		public RuntimePanel focusedPanel
		{
			get
			{
				return this.m_FocusedPanel;
			}
			set
			{
				bool flag = this.m_FocusedPanel != value;
				if (flag)
				{
					RuntimePanel focusedPanel = this.m_FocusedPanel;
					if (focusedPanel != null)
					{
						focusedPanel.Blur();
					}
					this.m_FocusedPanel = value;
					RuntimePanel focusedPanel2 = this.m_FocusedPanel;
					if (focusedPanel2 != null)
					{
						focusedPanel2.Focus();
					}
				}
			}
		}

		public void Reset()
		{
			DefaultEventSystem.LegacyInputProcessor legacyInputProcessor = this.m_LegacyInputProcessor;
			if (legacyInputProcessor != null)
			{
				legacyInputProcessor.Reset();
			}
			DefaultEventSystem.InputForUIProcessor inputForUIProcessor = this.m_InputForUIProcessor;
			if (inputForUIProcessor != null)
			{
				inputForUIProcessor.Reset();
			}
			this.m_FocusedPanel = null;
		}

		public void Update(DefaultEventSystem.UpdateMode updateMode = DefaultEventSystem.UpdateMode.Always)
		{
			bool flag = !this.isAppFocused && this.ShouldIgnoreEventsOnAppNotFocused() && updateMode == DefaultEventSystem.UpdateMode.IgnoreIfAppNotFocused;
			if (!flag)
			{
				this.m_UpdateFrameCount++;
				IScreenRaycaster raycaster = this.m_Raycaster;
				if (raycaster != null)
				{
					raycaster.Update();
				}
				bool isInputForUIActive = this.m_IsInputForUIActive;
				if (isInputForUIActive)
				{
					this.inputForUIProcessor.ProcessInputForUIEvents();
				}
				else
				{
					this.legacyInputProcessor.ProcessLegacyInputEvents();
				}
				this.UpdateWorldSpacePointers();
			}
		}

		internal DefaultEventSystem.LegacyInputProcessor legacyInputProcessor
		{
			get
			{
				DefaultEventSystem.LegacyInputProcessor legacyInputProcessor;
				if ((legacyInputProcessor = this.m_LegacyInputProcessor) == null)
				{
					legacyInputProcessor = (this.m_LegacyInputProcessor = new DefaultEventSystem.LegacyInputProcessor(this));
				}
				return legacyInputProcessor;
			}
		}

		private DefaultEventSystem.InputForUIProcessor inputForUIProcessor
		{
			get
			{
				DefaultEventSystem.InputForUIProcessor inputForUIProcessor;
				if ((inputForUIProcessor = this.m_InputForUIProcessor) == null)
				{
					inputForUIProcessor = (this.m_InputForUIProcessor = new DefaultEventSystem.InputForUIProcessor(this));
				}
				return inputForUIProcessor;
			}
		}

		internal bool isInputReady
		{
			get
			{
				return this.m_IsInputReady;
			}
			set
			{
				bool flag = this.m_IsInputReady == value;
				if (!flag)
				{
					this.m_IsInputReady = value;
					bool isInputReady = this.m_IsInputReady;
					if (isInputReady)
					{
						this.InitInputProcessor();
					}
					else
					{
						this.RemoveInputProcessor();
					}
				}
			}
		}

		internal bool useInputForUI
		{
			get
			{
				return this.m_UseInputForUI;
			}
			set
			{
				bool flag = this.m_UseInputForUI == value;
				if (!flag)
				{
					bool isInputReady = this.m_IsInputReady;
					if (isInputReady)
					{
						this.RemoveInputProcessor();
						this.m_UseInputForUI = value;
						this.InitInputProcessor();
					}
					else
					{
						this.m_UseInputForUI = value;
					}
				}
			}
		}

		internal DefaultEventSystem.FocusBasedEventSequenceContext FocusBasedEventSequence()
		{
			return new DefaultEventSystem.FocusBasedEventSequenceContext(this);
		}

		private void RemoveInputProcessor()
		{
			bool isInputForUIActive = this.m_IsInputForUIActive;
			if (isInputForUIActive)
			{
				EventProvider.Unsubscribe(new EventConsumer(this.inputForUIProcessor.OnEvent));
				EventProvider.SetEnabled(false);
				this.m_IsInputForUIActive = false;
			}
		}

		private void InitInputProcessor()
		{
			bool useInputForUI = this.m_UseInputForUI;
			if (useInputForUI)
			{
				this.m_IsInputForUIActive = true;
				EventProvider.SetEnabled(true);
				EventProvider.Subscribe(new EventConsumer(this.inputForUIProcessor.OnEvent), 0, null, Array.Empty<Event.Type>());
				this.m_InputForUIProcessor.Reset();
			}
		}

		internal void OnFocusEvent(RuntimePanel panel, FocusEvent evt)
		{
			this.focusedPanel = panel;
		}

		internal void SendFocusBasedEvent<TArg>(Func<TArg, EventBase> evtFactory, TArg arg)
		{
			bool flag = this.m_PreviousFocusedPanel != null;
			if (flag)
			{
				using (EventBase eventBase = evtFactory(arg))
				{
					eventBase.elementTarget = ((VisualElement)this.m_PreviousFocusedElement) ?? this.m_PreviousFocusedPanel.visualTree;
					this.m_PreviousFocusedPanel.visualTree.SendEvent(eventBase);
					this.UpdateFocusedPanel(this.m_PreviousFocusedPanel);
					return;
				}
			}
			List<BaseRuntimePanel> sortedPlayerPanels = UIElementsRuntimeUtility.GetSortedPlayerPanels();
			for (int i = sortedPlayerPanels.Count - 1; i >= 0; i--)
			{
				BaseRuntimePanel baseRuntimePanel = sortedPlayerPanels[i];
				RuntimePanel runtimePanel = baseRuntimePanel as RuntimePanel;
				bool flag2 = runtimePanel != null && !baseRuntimePanel.drawsInCameras;
				if (flag2)
				{
					using (EventBase eventBase2 = evtFactory(arg))
					{
						eventBase2.elementTarget = runtimePanel.visualTree;
						runtimePanel.visualTree.SendEvent(eventBase2);
						bool flag3 = runtimePanel.focusController.focusedElement != null;
						if (flag3)
						{
							this.focusedPanel = runtimePanel;
							break;
						}
						bool isPropagationStopped = eventBase2.isPropagationStopped;
						if (isPropagationStopped)
						{
							break;
						}
					}
				}
			}
		}

		internal void SendPositionBasedEvent<TArg>(Vector3 mousePosition, Vector3 delta, int pointerId, int? targetDisplay, Func<Vector3, Vector3, TArg, EventBase> evtFactory, TArg arg, bool deselectIfNoTarget = false)
		{
			this.SendPositionBasedEvent<ValueTuple<Func<Vector3, Vector3, TArg, EventBase>, Vector3, TArg>>(mousePosition, delta, pointerId, targetDisplay, delegate(Vector3 p, [TupleElementNames(new string[] { "evtFactory", "delta", "arg" })] ValueTuple<Func<Vector3, Vector3, TArg, EventBase>, Vector3, TArg> t)
			{
				EventBase eventBase = t.Item1(p, t.Item2, t.Item3);
				IPointerOrMouseEvent pointerOrMouseEvent = eventBase as IPointerOrMouseEvent;
				bool flag = pointerOrMouseEvent != null;
				if (flag)
				{
					pointerOrMouseEvent.deltaPosition = t.Item2;
				}
				return eventBase;
			}, new ValueTuple<Func<Vector3, Vector3, TArg, EventBase>, Vector3, TArg>(evtFactory, delta, arg), deselectIfNoTarget);
		}

		internal void SendPositionBasedEvent<TArg>(Vector3 mousePosition, Vector3 delta, int pointerId, int? targetDisplay, Func<Vector3, TArg, EventBase> evtFactory, TArg arg, bool deselectIfNoTarget = false)
		{
			bool flag = this.focusedPanel != null;
			if (flag)
			{
				this.UpdateFocusedPanel(this.focusedPanel);
			}
			VisualElement visualElement;
			RuntimePanel runtimePanel;
			Vector3 vector;
			VisualElement visualElement2;
			Camera camera;
			this.FindTargetAtPosition(mousePosition, delta, pointerId, targetDisplay, out visualElement, out runtimePanel, out vector, out visualElement2, out camera);
			RuntimePanel runtimePanel2 = PointerDeviceState.GetPanel(pointerId, ContextType.Player) as RuntimePanel;
			bool flag2 = runtimePanel2 != runtimePanel;
			if (flag2)
			{
				if (runtimePanel2 != null)
				{
					runtimePanel2.PointerLeavesPanel(pointerId);
				}
				if (runtimePanel != null)
				{
					runtimePanel.PointerEntersPanel(pointerId, vector);
				}
			}
			bool flag3 = runtimePanel != null;
			if (flag3)
			{
				using (EventBase eventBase = evtFactory(vector, arg))
				{
					bool flag4 = !runtimePanel.isFlat;
					if (flag4)
					{
						runtimePanel.SetTopElementUnderPointer(pointerId, visualElement2, eventBase);
					}
					eventBase.elementTarget = visualElement;
					runtimePanel.visualTree.SendEvent(eventBase);
					bool processedByFocusController = eventBase.processedByFocusController;
					if (processedByFocusController)
					{
						this.UpdateFocusedPanel(runtimePanel);
					}
					bool flag5 = eventBase.eventTypeId == EventBase<PointerDownEvent>.TypeId();
					if (flag5)
					{
						PointerDeviceState.SetElementWithSoftPointerCapture(pointerId, visualElement ?? runtimePanel.visualTree, camera);
					}
					else
					{
						bool flag6 = eventBase.eventTypeId == EventBase<PointerUpEvent>.TypeId() && ((PointerUpEvent)eventBase).pressedButtons == 0;
						if (flag6)
						{
							PointerDeviceState.SetElementWithSoftPointerCapture(pointerId, null, null);
						}
					}
				}
			}
			else if (deselectIfNoTarget)
			{
				this.focusedPanel = null;
			}
		}

		internal void SendRayBasedEvent<TArg>(Ray worldRay, float maxDistance, int pointerId, Func<Vector3, TArg, EventBase> evtFactory, TArg arg, bool deselectIfNoTarget = false)
		{
			bool flag = this.focusedPanel != null;
			if (flag)
			{
				this.UpdateFocusedPanel(this.focusedPanel);
			}
			VisualElement visualElement;
			RuntimePanel runtimePanel;
			Vector3 vector;
			VisualElement visualElement2;
			this.FindTargetAtRay(worldRay, maxDistance, pointerId, out visualElement, out runtimePanel, out vector, out visualElement2);
			RuntimePanel runtimePanel2 = PointerDeviceState.GetPanel(pointerId, ContextType.Player) as RuntimePanel;
			bool flag2 = runtimePanel2 != runtimePanel;
			if (flag2)
			{
				if (runtimePanel2 != null)
				{
					runtimePanel2.PointerLeavesPanel(pointerId);
				}
				if (runtimePanel != null)
				{
					runtimePanel.PointerEntersPanel(pointerId, vector);
				}
			}
			bool flag3 = runtimePanel != null;
			if (flag3)
			{
				using (EventBase eventBase = evtFactory(vector, arg))
				{
					bool flag4 = !runtimePanel.isFlat;
					if (flag4)
					{
						runtimePanel.SetTopElementUnderPointer(pointerId, visualElement2, eventBase);
					}
					eventBase.elementTarget = visualElement;
					runtimePanel.visualTree.SendEvent(eventBase);
					bool processedByFocusController = eventBase.processedByFocusController;
					if (processedByFocusController)
					{
						this.UpdateFocusedPanel(runtimePanel);
					}
					bool flag5 = eventBase.eventTypeId == EventBase<PointerDownEvent>.TypeId();
					if (flag5)
					{
						PointerDeviceState.SetElementWithSoftPointerCapture(pointerId, visualElement ?? runtimePanel.visualTree, null);
					}
					else
					{
						bool flag6 = eventBase.eventTypeId == EventBase<PointerUpEvent>.TypeId() && ((PointerUpEvent)eventBase).pressedButtons == 0;
						if (flag6)
						{
							PointerDeviceState.SetElementWithSoftPointerCapture(pointerId, null, null);
						}
					}
				}
			}
			else if (deselectIfNoTarget)
			{
				this.focusedPanel = null;
			}
		}

		public IScreenRaycaster raycaster
		{
			get
			{
				IScreenRaycaster screenRaycaster;
				if ((screenRaycaster = this.m_Raycaster) == null)
				{
					screenRaycaster = (this.m_Raycaster = new MainCameraScreenRaycaster());
				}
				return screenRaycaster;
			}
			set
			{
				this.m_Raycaster = value;
			}
		}

		internal void FindTargetAtPosition(Vector2 mousePosition, Vector2 delta, int pointerId, int? targetDisplay, out VisualElement target, out RuntimePanel targetPanel, out Vector3 targetPanelPosition, out VisualElement elementUnderPointer, out Camera camera)
		{
			PointerDeviceState.ScreenPointerState screenPointerState = PointerDeviceState.GetScreenPointerState(pointerId, true);
			screenPointerState.Reset();
			screenPointerState.mousePosition = mousePosition;
			screenPointerState.targetDisplay = targetDisplay;
			screenPointerState.updateFrameCount = this.m_UpdateFrameCount;
			List<BaseRuntimePanel> sortedScreenOverlayPlayerPanels = UIElementsRuntimeUtility.GetSortedScreenOverlayPlayerPanels();
			VisualElement visualElement;
			for (int i = sortedScreenOverlayPlayerPanels.Count - 1; i >= 0; i--)
			{
				bool flag2;
				bool flag = this.m_ScreenOverlayPicker.TryPick(sortedScreenOverlayPlayerPanels[i], pointerId, mousePosition, delta, targetDisplay, out flag2);
				if (flag)
				{
					elementUnderPointer = (visualElement = null);
					target = visualElement;
					targetPanel = (RuntimePanel)sortedScreenOverlayPlayerPanels[i];
					targetPanel.ScreenToPanel(mousePosition, delta, out targetPanelPosition, true);
					camera = null;
					return;
				}
			}
			foreach (ValueTuple<Ray, Camera, bool> valueTuple in this.raycaster.MakeRay(mousePosition, pointerId, targetDisplay))
			{
				int num = valueTuple.Item2.cullingMask & this.worldSpaceLayers;
				Collider collider;
				UIDocument uidocument;
				float num2;
				bool flag4;
				bool flag3 = this.m_WorldSpacePicker.TryPickWithCapture(pointerId, valueTuple.Item1, this.worldSpaceMaxDistance, num, out collider, out uidocument, out elementUnderPointer, out num2, out flag4) && (valueTuple.Item3 || flag4);
				if (flag3)
				{
					screenPointerState.hit = new PointerDeviceState.RuntimePointerState.RaycastHit
					{
						collider = collider,
						document = uidocument,
						distance = num2,
						element = elementUnderPointer
					};
					bool flag5 = uidocument == null;
					if (flag5)
					{
						break;
					}
					VisualElement visualElement2 = RuntimePanel.s_EventDispatcher.pointerState.GetCapturingElement(pointerId) as VisualElement;
					VisualElement visualElement3;
					if ((visualElement3 = visualElement2) == null)
					{
						visualElement3 = elementUnderPointer ?? uidocument.rootVisualElement;
					}
					target = visualElement3;
					targetPanel = uidocument.containerPanel;
					targetPanelPosition = this.GetPanelPosition(target, uidocument, valueTuple.Item1);
					camera = valueTuple.Item2;
					return;
				}
			}
			elementUnderPointer = (visualElement = null);
			target = visualElement;
			targetPanel = null;
			targetPanelPosition = DefaultEventSystem.s_InvalidPanelCoordinates;
			camera = null;
		}

		internal void FindTargetAtRay(Ray worldRay, float maxDistance, int pointerId, out VisualElement target, out RuntimePanel targetPanel, out Vector3 targetPanelPosition, out VisualElement elementUnderPointer)
		{
			maxDistance = Mathf.Min(maxDistance, this.worldSpaceMaxDistance);
			Collider collider;
			UIDocument uidocument;
			float num;
			bool flag2;
			bool flag = this.m_WorldSpacePicker.TryPickWithCapture(pointerId, worldRay, maxDistance, this.worldSpaceLayers, out collider, out uidocument, out elementUnderPointer, out num, out flag2);
			PointerDeviceState.TrackedPointerState trackedState = PointerDeviceState.GetTrackedState(pointerId, true);
			trackedState.Reset();
			trackedState.worldPosition = worldRay.origin;
			trackedState.worldOrientation = Quaternion.FromToRotation(Vector3.forward, worldRay.direction);
			trackedState.maxDistance = maxDistance;
			trackedState.hit = new PointerDeviceState.RuntimePointerState.RaycastHit
			{
				collider = collider,
				document = uidocument,
				distance = num,
				element = elementUnderPointer
			};
			trackedState.updateFrameCount = this.m_UpdateFrameCount;
			bool flag3 = flag && uidocument != null;
			if (flag3)
			{
				VisualElement visualElement = RuntimePanel.s_EventDispatcher.pointerState.GetCapturingElement(pointerId) as VisualElement;
				VisualElement visualElement2;
				if ((visualElement2 = visualElement) == null)
				{
					visualElement2 = elementUnderPointer ?? uidocument.rootVisualElement;
				}
				target = visualElement2;
				targetPanel = uidocument.containerPanel;
				targetPanelPosition = this.GetPanelPosition(target, uidocument, worldRay);
			}
			else
			{
				VisualElement visualElement3;
				elementUnderPointer = (visualElement3 = null);
				target = visualElement3;
				targetPanel = null;
				targetPanelPosition = DefaultEventSystem.s_InvalidPanelCoordinates;
			}
		}

		private Vector3 GetPanelPosition(VisualElement pickedElement, UIDocument document, Ray worldRay)
		{
			Ray ray = document.transform.worldToLocalMatrix.TransformRay(worldRay);
			float num;
			Vector3 vector;
			pickedElement.IntersectWorldRay(ray, out num, out vector);
			return ray.origin + ray.direction * num;
		}

		private void UpdateFocusedPanel(RuntimePanel runtimePanel)
		{
			bool flag = runtimePanel.focusController.focusedElement != null;
			if (flag)
			{
				this.focusedPanel = runtimePanel;
			}
			else
			{
				bool flag2 = this.focusedPanel == runtimePanel;
				if (flag2)
				{
					this.focusedPanel = null;
				}
			}
		}

		private void UpdateWorldSpacePointers()
		{
			bool flag = UIElementsRuntimeUtility.GetWorldSpacePlayerPanels().Count == 0;
			if (!flag)
			{
				foreach (int num in PointerId.screenHoveringPointers)
				{
					RuntimePanel runtimePanel = PointerDeviceState.GetPanel(num, ContextType.Player) as RuntimePanel;
					bool flag2 = runtimePanel != null && runtimePanel.isFlat;
					if (!flag2)
					{
						PointerDeviceState.ScreenPointerState screenPointerState = PointerDeviceState.GetScreenPointerState(num, false);
						bool flag3 = screenPointerState == null || screenPointerState.updateFrameCount == this.m_UpdateFrameCount;
						if (!flag3)
						{
							VisualElement visualElement;
							RuntimePanel runtimePanel2;
							Vector3 vector;
							VisualElement visualElement2;
							Camera camera;
							this.FindTargetAtPosition(screenPointerState.mousePosition, Vector2.zero, num, screenPointerState.targetDisplay, out visualElement, out runtimePanel2, out vector, out visualElement2, out camera);
							bool flag4 = runtimePanel != runtimePanel2;
							if (flag4)
							{
								if (runtimePanel != null)
								{
									runtimePanel.PointerLeavesPanel(num);
								}
								if (runtimePanel2 != null)
								{
									runtimePanel2.PointerEntersPanel(num, vector);
								}
							}
							bool flag5 = runtimePanel2 != null && !runtimePanel2.isFlat;
							if (flag5)
							{
								runtimePanel2.SetTopElementUnderPointer(num, visualElement2, vector);
								runtimePanel2.CommitElementUnderPointers();
							}
						}
					}
				}
				for (int j = 0; j < PointerId.trackedPointerCount; j++)
				{
					int num2 = PointerId.trackedPointerIdBase + j;
					PointerDeviceState.TrackedPointerState trackedState = PointerDeviceState.GetTrackedState(num2, false);
					bool flag6 = trackedState == null || trackedState.updateFrameCount == this.m_UpdateFrameCount;
					if (!flag6)
					{
						VisualElement visualElement;
						RuntimePanel runtimePanel3;
						Vector3 vector2;
						VisualElement visualElement3;
						this.FindTargetAtRay(trackedState.worldRay, trackedState.maxDistance, num2, out visualElement, out runtimePanel3, out vector2, out visualElement3);
						RuntimePanel runtimePanel4 = PointerDeviceState.GetPanel(num2, ContextType.Player) as RuntimePanel;
						bool flag7 = runtimePanel4 != runtimePanel3;
						if (flag7)
						{
							if (runtimePanel4 != null)
							{
								runtimePanel4.PointerLeavesPanel(num2);
							}
							if (runtimePanel3 != null)
							{
								runtimePanel3.PointerEntersPanel(num2, vector2);
							}
						}
						bool flag8 = runtimePanel3 != null;
						if (flag8)
						{
							runtimePanel3.SetTopElementUnderPointer(num2, visualElement3, vector2);
							runtimePanel3.CommitElementUnderPointers();
						}
					}
				}
			}
		}

		private void Log(object o)
		{
			Debug.Log(o);
			bool flag = this.logToGameScreen;
			if (flag)
			{
				this.LogToGameScreen(((o != null) ? o.ToString() : null) ?? "");
			}
		}

		private void LogWarning(object o)
		{
			Debug.LogWarning(o);
			bool flag = this.logToGameScreen;
			if (flag)
			{
				this.LogToGameScreen("Warning! " + ((o != null) ? o.ToString() : null));
			}
		}

		private void LogToGameScreen(string s)
		{
			bool flag = this.m_LogLabel == null;
			if (flag)
			{
				this.m_LogLabel = new Label
				{
					style = 
					{
						position = Position.Absolute,
						bottom = 0f,
						color = Color.white
					}
				};
				Object.FindFirstObjectByType<UIDocument>().rootVisualElement.Add(this.m_LogLabel);
			}
			this.m_LogLines.Add(s + "\n");
			bool flag2 = this.m_LogLines.Count > 10;
			if (flag2)
			{
				this.m_LogLines.RemoveAt(0);
			}
			this.m_LogLabel.text = string.Concat(this.m_LogLines);
		}

		internal static Func<bool> IsEditorRemoteConnected = () => false;

		private RuntimePanel m_FocusedPanel;

		private RuntimePanel m_PreviousFocusedPanel;

		private Focusable m_PreviousFocusedElement;

		private int m_UpdateFrameCount = 0;

		private DefaultEventSystem.LegacyInputProcessor m_LegacyInputProcessor;

		private DefaultEventSystem.InputForUIProcessor m_InputForUIProcessor;

		private bool m_IsInputReady = false;

		private bool m_UseInputForUI = true;

		private bool m_IsInputForUIActive = false;

		private IScreenRaycaster m_Raycaster;

		private readonly PhysicsDocumentPicker m_WorldSpacePicker = new PhysicsDocumentPicker();

		private readonly ScreenOverlayPanelPicker m_ScreenOverlayPicker = new ScreenOverlayPanelPicker();

		public float worldSpaceMaxDistance = float.PositiveInfinity;

		public int worldSpaceLayers = -5;

		private static readonly Vector3 s_InvalidPanelCoordinates = new Vector3(float.NaN, float.NaN, float.NaN);

		internal bool verbose = false;

		internal bool logToGameScreen = false;

		private Label m_LogLabel;

		private List<string> m_LogLines = new List<string>();

		public enum UpdateMode
		{
			Always,
			IgnoreIfAppNotFocused
		}

		internal struct FocusBasedEventSequenceContext : IDisposable
		{
			public FocusBasedEventSequenceContext(DefaultEventSystem es)
			{
				this.es = es;
				es.m_PreviousFocusedPanel = es.focusedPanel;
				RuntimePanel focusedPanel = es.focusedPanel;
				es.m_PreviousFocusedElement = ((focusedPanel != null) ? focusedPanel.focusController.GetLeafFocusedElement() : null);
			}

			public void Dispose()
			{
				this.es.m_PreviousFocusedPanel = null;
				this.es.m_PreviousFocusedElement = null;
			}

			private DefaultEventSystem es;
		}

		private class InputForUIProcessor
		{
			public InputForUIProcessor(DefaultEventSystem eventSystem)
			{
				this.m_EventSystem = eventSystem;
			}

			public void Reset()
			{
				this.m_LastPointerTimestamp = DiscreteTime.Zero;
				this.m_NextPointerTimestamp = DiscreteTime.Zero;
				this.m_EventList.Clear();
			}

			public bool OnEvent(in Event ev)
			{
				this.m_EventList.Enqueue(ev);
				return true;
			}

			public void ProcessInputForUIEvents()
			{
				bool flag = this.m_EventList.Count == 0;
				if (!flag)
				{
					DefaultEventSystem.FocusBasedEventSequenceContext? focusBasedEventSequenceContext = null;
					while (this.m_EventList.Count > 0)
					{
						Event @event = this.m_EventList.Dequeue();
						switch (@event.type)
						{
						case Event.Type.KeyEvent:
						{
							DefaultEventSystem.FocusBasedEventSequenceContext focusBasedEventSequenceContext2 = focusBasedEventSequenceContext.GetValueOrDefault();
							if (focusBasedEventSequenceContext == null)
							{
								focusBasedEventSequenceContext2 = this.m_EventSystem.FocusBasedEventSequence();
								focusBasedEventSequenceContext = new DefaultEventSystem.FocusBasedEventSequenceContext?(focusBasedEventSequenceContext2);
							}
							this.ProcessKeyEvent(@event.asKeyEvent);
							break;
						}
						case Event.Type.PointerEvent:
							this.ProcessPointerEvent(@event.asPointerEvent);
							break;
						case Event.Type.TextInputEvent:
						{
							DefaultEventSystem.FocusBasedEventSequenceContext focusBasedEventSequenceContext2 = focusBasedEventSequenceContext.GetValueOrDefault();
							if (focusBasedEventSequenceContext == null)
							{
								focusBasedEventSequenceContext2 = this.m_EventSystem.FocusBasedEventSequence();
								focusBasedEventSequenceContext = new DefaultEventSystem.FocusBasedEventSequenceContext?(focusBasedEventSequenceContext2);
							}
							this.ProcessTextInputEvent(@event.asTextInputEvent);
							break;
						}
						case Event.Type.IMECompositionEvent:
						{
							DefaultEventSystem.FocusBasedEventSequenceContext focusBasedEventSequenceContext2 = focusBasedEventSequenceContext.GetValueOrDefault();
							if (focusBasedEventSequenceContext == null)
							{
								focusBasedEventSequenceContext2 = this.m_EventSystem.FocusBasedEventSequence();
								focusBasedEventSequenceContext = new DefaultEventSystem.FocusBasedEventSequenceContext?(focusBasedEventSequenceContext2);
							}
							this.ProcessIMECompositionEvent(@event.asIMECompositionEvent);
							break;
						}
						case Event.Type.CommandEvent:
						{
							DefaultEventSystem.FocusBasedEventSequenceContext focusBasedEventSequenceContext2 = focusBasedEventSequenceContext.GetValueOrDefault();
							if (focusBasedEventSequenceContext == null)
							{
								focusBasedEventSequenceContext2 = this.m_EventSystem.FocusBasedEventSequence();
								focusBasedEventSequenceContext = new DefaultEventSystem.FocusBasedEventSequenceContext?(focusBasedEventSequenceContext2);
							}
							this.ProcessCommandEvent(@event.asCommandEvent);
							break;
						}
						case Event.Type.NavigationEvent:
						{
							DefaultEventSystem.FocusBasedEventSequenceContext focusBasedEventSequenceContext2 = focusBasedEventSequenceContext.GetValueOrDefault();
							if (focusBasedEventSequenceContext == null)
							{
								focusBasedEventSequenceContext2 = this.m_EventSystem.FocusBasedEventSequence();
								focusBasedEventSequenceContext = new DefaultEventSystem.FocusBasedEventSequenceContext?(focusBasedEventSequenceContext2);
							}
							this.ProcessNavigationEvent(@event.asNavigationEvent);
							break;
						}
						default:
						{
							bool verbose = this.m_EventSystem.verbose;
							if (verbose)
							{
								DefaultEventSystem eventSystem = this.m_EventSystem;
								string text = "Unsupported event (";
								string text2 = ((int)@event.type).ToString();
								string text3 = "): ";
								Event event2 = @event;
								eventSystem.Log(text + text2 + text3 + event2.ToString());
							}
							break;
						}
						}
					}
					if (focusBasedEventSequenceContext != null)
					{
						focusBasedEventSequenceContext.GetValueOrDefault().Dispose();
					}
					this.m_LastPointerTimestamp = this.m_NextPointerTimestamp;
				}
			}

			private EventModifiers GetModifiers(EventModifiers eventModifiers)
			{
				EventModifiers eventModifiers2 = EventModifiers.None;
				bool isShiftPressed = eventModifiers.isShiftPressed;
				if (isShiftPressed)
				{
					eventModifiers2 |= EventModifiers.Shift;
				}
				bool isCtrlPressed = eventModifiers.isCtrlPressed;
				if (isCtrlPressed)
				{
					eventModifiers2 |= EventModifiers.Control;
				}
				bool isAltPressed = eventModifiers.isAltPressed;
				if (isAltPressed)
				{
					eventModifiers2 |= EventModifiers.Alt;
				}
				bool isMetaPressed = eventModifiers.isMetaPressed;
				if (isMetaPressed)
				{
					eventModifiers2 |= EventModifiers.Command;
				}
				bool isCapsLockEnabled = eventModifiers.isCapsLockEnabled;
				if (isCapsLockEnabled)
				{
					eventModifiers2 |= EventModifiers.CapsLock;
				}
				bool isNumericPressed = eventModifiers.isNumericPressed;
				if (isNumericPressed)
				{
					eventModifiers2 |= EventModifiers.Numeric;
				}
				bool isFunctionKeyPressed = eventModifiers.isFunctionKeyPressed;
				if (isFunctionKeyPressed)
				{
					eventModifiers2 |= EventModifiers.FunctionKey;
				}
				return eventModifiers2;
			}

			private void ProcessPointerEvent(PointerEvent pointerEvent)
			{
				Vector2 position = pointerEvent.position;
				int displayIndex = pointerEvent.displayIndex;
				Vector2 deltaPosition = pointerEvent.deltaPosition;
				EventSource eventSource = pointerEvent.eventSource;
				if (!true)
				{
				}
				ValueTuple<int, int> valueTuple;
				switch (eventSource)
				{
				case EventSource.Mouse:
					valueTuple = new ValueTuple<int, int>(PointerId.mousePointerId, 1);
					break;
				case EventSource.Pen:
					valueTuple = new ValueTuple<int, int>(PointerId.penPointerIdBase, PointerId.penPointerCount);
					break;
				case EventSource.Touch:
					valueTuple = new ValueTuple<int, int>(PointerId.touchPointerIdBase, PointerId.touchPointerCount);
					break;
				case EventSource.TrackedDevice:
					valueTuple = new ValueTuple<int, int>(PointerId.trackedPointerIdBase, PointerId.trackedPointerCount);
					break;
				default:
					valueTuple = new ValueTuple<int, int>(PointerId.invalidPointerId, 1);
					break;
				}
				if (!true)
				{
				}
				ValueTuple<int, int> valueTuple2 = valueTuple;
				int item = valueTuple2.Item1;
				int item2 = valueTuple2.Item2;
				bool flag = item == PointerId.invalidPointerId;
				if (flag)
				{
					bool verbose = this.m_EventSystem.verbose;
					if (verbose)
					{
						DefaultEventSystem eventSystem = this.m_EventSystem;
						string[] array = new string[5];
						array[0] = "Pointer event source not supported: ";
						int num = 1;
						PointerEvent pointerEvent2 = pointerEvent;
						array[num] = pointerEvent2.ToString();
						array[2] = " (source=";
						array[3] = pointerEvent.eventSource.ToString();
						array[4] = ")";
						eventSystem.Log(string.Concat(array));
					}
				}
				else
				{
					bool flag2 = pointerEvent.pointerIndex < 0 || pointerEvent.pointerIndex >= item2;
					if (flag2)
					{
						bool verbose2 = this.m_EventSystem.verbose;
						if (verbose2)
						{
							DefaultEventSystem eventSystem2 = this.m_EventSystem;
							string[] array2 = new string[7];
							array2[0] = "Pointer index out of range: ";
							int num2 = 1;
							PointerEvent pointerEvent2 = pointerEvent;
							array2[num2] = pointerEvent2.ToString();
							array2[2] = " (index=";
							array2[3] = pointerEvent.pointerIndex.ToString();
							array2[4] = ", should have 0 <= index < ";
							array2[5] = item2.ToString();
							array2[6] = ")";
							eventSystem2.Log(string.Concat(array2));
						}
					}
					else
					{
						int num3 = item + pointerEvent.pointerIndex;
						bool flag3 = num3 < 0 || num3 >= PointerId.maxPointers;
						if (flag3)
						{
							bool verbose3 = this.m_EventSystem.verbose;
							if (verbose3)
							{
								DefaultEventSystem eventSystem3 = this.m_EventSystem;
								string[] array3 = new string[7];
								array3[0] = "Pointer id out of range: ";
								int num4 = 1;
								PointerEvent pointerEvent2 = pointerEvent;
								array3[num4] = pointerEvent2.ToString();
								array3[2] = " (id=";
								array3[3] = num3.ToString();
								array3[4] = ", should have 0 <= id < ";
								array3[5] = PointerId.maxPointers.ToString();
								array3[6] = ")";
								eventSystem3.Log(string.Concat(array3));
							}
						}
						else
						{
							float num5 = ((this.m_LastPointerTimestamp != DiscreteTime.Zero) ? ((float)(pointerEvent.timestamp - this.m_LastPointerTimestamp)) : 0f);
							this.m_NextPointerTimestamp = pointerEvent.timestamp;
							bool flag4 = false;
							bool flag5 = pointerEvent.type == PointerEvent.Type.PointerMoved;
							Func<Vector3, ValueTuple<PointerEvent, int, float>, EventBase> func;
							if (flag5)
							{
								bool flag6 = pointerEvent.eventSource != EventSource.TrackedDevice && Mathf.Approximately(deltaPosition.x, 0f) && Mathf.Approximately(deltaPosition.y, 0f);
								if (flag6)
								{
									return;
								}
								func = (Vector3 panelPosition, [TupleElementNames(new string[] { "pointerEvent", "pointerId", "deltaTime" })] ValueTuple<PointerEvent, int, float> t) => PointerEventBase<PointerMoveEvent>.GetPooled(t.Item1, panelPosition, t.Item2, t.Item3);
							}
							else
							{
								bool flag7 = pointerEvent.type == PointerEvent.Type.ButtonPressed;
								if (flag7)
								{
									func = (Vector3 panelPosition, [TupleElementNames(new string[] { "pointerEvent", "pointerId", "deltaTime" })] ValueTuple<PointerEvent, int, float> t) => PointerEventBase<PointerDownEvent>.GetPooled(t.Item1, panelPosition, t.Item2, t.Item3);
								}
								else
								{
									bool flag8 = pointerEvent.type == PointerEvent.Type.ButtonReleased;
									if (flag8)
									{
										func = (Vector3 panelPosition, [TupleElementNames(new string[] { "pointerEvent", "pointerId", "deltaTime" })] ValueTuple<PointerEvent, int, float> t) => PointerEventBase<PointerUpEvent>.GetPooled(t.Item1, panelPosition, t.Item2, t.Item3);
										flag4 = true;
									}
									else
									{
										bool flag9 = pointerEvent.type == PointerEvent.Type.TouchCanceled || pointerEvent.type == PointerEvent.Type.TouchCanceled;
										if (flag9)
										{
											func = (Vector3 panelPosition, [TupleElementNames(new string[] { "pointerEvent", "pointerId", "deltaTime" })] ValueTuple<PointerEvent, int, float> t) => PointerEventBase<PointerCancelEvent>.GetPooled(t.Item1, panelPosition, t.Item2, t.Item3);
										}
										else
										{
											bool flag10 = pointerEvent.type == PointerEvent.Type.Scroll;
											if (!flag10)
											{
												bool verbose4 = this.m_EventSystem.verbose;
												if (verbose4)
												{
													DefaultEventSystem eventSystem4 = this.m_EventSystem;
													string text = "Unsupported event ";
													PointerEvent pointerEvent2 = pointerEvent;
													eventSystem4.Log(text + pointerEvent2.ToString());
												}
												return;
											}
											func = (Vector3 panelPosition, [TupleElementNames(new string[] { "pointerEvent", "pointerId", "deltaTime" })] ValueTuple<PointerEvent, int, float> t) => WheelEvent.GetPooled(t.Item1.scroll, panelPosition, this.GetModifiers(t.Item1.eventModifiers));
										}
									}
								}
							}
							bool flag11 = pointerEvent.eventSource == EventSource.TrackedDevice;
							if (flag11)
							{
								float num6 = ((pointerEvent.maxDistance > 0f) ? pointerEvent.maxDistance : float.PositiveInfinity);
								this.m_EventSystem.SendRayBasedEvent<ValueTuple<PointerEvent, int, float>>(pointerEvent.worldRay, num6, num3, func, new ValueTuple<PointerEvent, int, float>(pointerEvent, num3, num5), flag4);
							}
							else
							{
								this.m_EventSystem.SendPositionBasedEvent<ValueTuple<PointerEvent, int, float>>(position, deltaPosition, num3, new int?(displayIndex), func, new ValueTuple<PointerEvent, int, float>(pointerEvent, num3, num5), flag4);
							}
						}
					}
				}
			}

			private void ProcessNavigationEvent(NavigationEvent navigationEvent)
			{
				bool verbose = this.m_EventSystem.verbose;
				if (verbose)
				{
					this.m_EventSystem.Log(navigationEvent);
				}
				EventModifiers modifiers = this.GetModifiers(navigationEvent.eventModifiers);
				NavigationDeviceType navigationDeviceType = ((navigationEvent.eventSource == EventSource.Keyboard) ? NavigationDeviceType.Keyboard : ((navigationEvent.eventSource == EventSource.Unspecified) ? NavigationDeviceType.Unknown : NavigationDeviceType.NonKeyboard));
				bool flag = navigationEvent.type == NavigationEvent.Type.Move;
				if (flag)
				{
					Vector2 zero = Vector2.zero;
					bool flag2 = navigationEvent.direction == NavigationEvent.Direction.Left;
					if (flag2)
					{
						zero.x = -1f;
					}
					else
					{
						bool flag3 = navigationEvent.direction == NavigationEvent.Direction.Right;
						if (flag3)
						{
							zero.x = 1f;
						}
						else
						{
							bool flag4 = navigationEvent.direction == NavigationEvent.Direction.Up;
							if (flag4)
							{
								zero.y = 1f;
							}
							else
							{
								bool flag5 = navigationEvent.direction == NavigationEvent.Direction.Down;
								if (flag5)
								{
									zero.y = -1f;
								}
							}
						}
					}
					bool flag6 = zero != Vector2.zero;
					if (flag6)
					{
						this.m_EventSystem.SendFocusBasedEvent<ValueTuple<Vector2, NavigationDeviceType, EventModifiers>>(([TupleElementNames(new string[] { "move", "deviceType", "mod" })] ValueTuple<Vector2, NavigationDeviceType, EventModifiers> t) => NavigationMoveEvent.GetPooled(t.Item1, t.Item2, t.Item3), new ValueTuple<Vector2, NavigationDeviceType, EventModifiers>(zero, navigationDeviceType, modifiers));
					}
					else
					{
						NavigationMoveEvent.Direction direction = ((navigationEvent.direction == NavigationEvent.Direction.Previous) ? NavigationMoveEvent.Direction.Previous : NavigationMoveEvent.Direction.Next);
						this.m_EventSystem.SendFocusBasedEvent<ValueTuple<NavigationMoveEvent.Direction, NavigationDeviceType, EventModifiers>>(([TupleElementNames(new string[] { "direction", "deviceType", "mod" })] ValueTuple<NavigationMoveEvent.Direction, NavigationDeviceType, EventModifiers> t) => NavigationMoveEvent.GetPooled(t.Item1, t.Item2, t.Item3), new ValueTuple<NavigationMoveEvent.Direction, NavigationDeviceType, EventModifiers>(direction, navigationDeviceType, modifiers));
					}
				}
				else
				{
					bool flag7 = navigationEvent.type == NavigationEvent.Type.Submit;
					if (flag7)
					{
						this.m_EventSystem.SendFocusBasedEvent<ValueTuple<NavigationDeviceType, EventModifiers>>(([TupleElementNames(new string[] { "deviceType", "mod" })] ValueTuple<NavigationDeviceType, EventModifiers> t) => NavigationEventBase<NavigationSubmitEvent>.GetPooled(t.Item1, t.Item2), new ValueTuple<NavigationDeviceType, EventModifiers>(navigationDeviceType, modifiers));
					}
					else
					{
						bool flag8 = navigationEvent.type == NavigationEvent.Type.Cancel;
						if (flag8)
						{
							this.m_EventSystem.SendFocusBasedEvent<ValueTuple<NavigationDeviceType, EventModifiers>>(([TupleElementNames(new string[] { "deviceType", "mod" })] ValueTuple<NavigationDeviceType, EventModifiers> t) => NavigationEventBase<NavigationCancelEvent>.GetPooled(t.Item1, t.Item2), new ValueTuple<NavigationDeviceType, EventModifiers>(navigationDeviceType, modifiers));
						}
					}
				}
			}

			private void ProcessKeyEvent(KeyEvent keyEvent)
			{
				bool verbose = this.m_EventSystem.verbose;
				if (verbose)
				{
					this.m_EventSystem.Log(keyEvent);
				}
				bool flag = keyEvent.type == KeyEvent.Type.KeyPressed || keyEvent.type == KeyEvent.Type.KeyRepeated;
				if (flag)
				{
					this.m_EventSystem.SendFocusBasedEvent<ValueTuple<EventModifiers, KeyCode>>(([TupleElementNames(new string[] { "modifiers", "keyCode" })] ValueTuple<EventModifiers, KeyCode> t) => KeyboardEventBase<KeyDownEvent>.GetPooled('\0', t.Item2, t.Item1), new ValueTuple<EventModifiers, KeyCode>(this.GetModifiers(keyEvent.eventModifiers), keyEvent.keyCode));
				}
				else
				{
					bool flag2 = keyEvent.type == KeyEvent.Type.KeyReleased;
					if (flag2)
					{
						this.m_EventSystem.SendFocusBasedEvent<ValueTuple<EventModifiers, KeyCode>>(([TupleElementNames(new string[] { "modifiers", "keyCode" })] ValueTuple<EventModifiers, KeyCode> t) => KeyboardEventBase<KeyUpEvent>.GetPooled('\0', t.Item2, t.Item1), new ValueTuple<EventModifiers, KeyCode>(this.GetModifiers(keyEvent.eventModifiers), keyEvent.keyCode));
					}
				}
			}

			private void ProcessTextInputEvent(TextInputEvent textInputEvent)
			{
				bool verbose = this.m_EventSystem.verbose;
				if (verbose)
				{
					this.m_EventSystem.Log(textInputEvent);
				}
				this.m_EventSystem.SendFocusBasedEvent<ValueTuple<EventModifiers, char>>(([TupleElementNames(new string[] { "modifiers", "character" })] ValueTuple<EventModifiers, char> t) => KeyboardEventBase<KeyDownEvent>.GetPooled(t.Item2, KeyCode.None, t.Item1), new ValueTuple<EventModifiers, char>(this.GetModifiers(textInputEvent.eventModifiers), textInputEvent.character));
			}

			private void ProcessCommandEvent(CommandEvent commandEvent)
			{
				bool verbose = this.m_EventSystem.verbose;
				if (verbose)
				{
					this.m_EventSystem.Log(commandEvent);
				}
			}

			private void ProcessIMECompositionEvent(IMECompositionEvent compositionEvent)
			{
				bool verbose = this.m_EventSystem.verbose;
				if (verbose)
				{
					this.m_EventSystem.Log(compositionEvent);
				}
				this.m_EventSystem.SendFocusBasedEvent<int>((int _) => IMEEvent.GetPooled(compositionEvent.compositionString), 0);
			}

			private readonly DefaultEventSystem m_EventSystem;

			private DiscreteTime m_LastPointerTimestamp = DiscreteTime.Zero;

			private DiscreteTime m_NextPointerTimestamp = DiscreteTime.Zero;

			private readonly Queue<Event> m_EventList = new Queue<Event>();
		}

		internal class LegacyInputProcessor
		{
			private EventModifiers m_CurrentPointerModifiers
			{
				get
				{
					return this.m_CurrentModifiers & (EventModifiers.Shift | EventModifiers.Control | EventModifiers.Alt | EventModifiers.Command);
				}
			}

			public DefaultEventSystem.LegacyInputProcessor.IInput input
			{
				get
				{
					DefaultEventSystem.LegacyInputProcessor.IInput input;
					if ((input = this.m_Input) == null)
					{
						input = (this.m_Input = this.GetDefaultInput());
					}
					return input;
				}
				set
				{
					this.m_Input = value;
				}
			}

			public LegacyInputProcessor(DefaultEventSystem eventSystem)
			{
				this.m_EventSystem = eventSystem;
			}

			public DefaultEventSystem.LegacyInputProcessor.IInput GetDefaultInput()
			{
				DefaultEventSystem.LegacyInputProcessor.IInput input = new DefaultEventSystem.LegacyInputProcessor.Input();
				try
				{
					input.GetAxisRaw("Horizontal");
				}
				catch (InvalidOperationException)
				{
					input = new DefaultEventSystem.LegacyInputProcessor.NoInput();
					this.m_EventSystem.LogWarning("UI Toolkit is currently relying on the legacy Input Manager for its active input source, but the legacy Input Manager is not available using your current Project Settings. Some UI Toolkit functionality might be missing or not working properly as a result. To fix this problem, you can enable \"Input Manager (old)\" or \"Both\" in the Active Input Source setting of the Player section. UI Toolkit is using its internal default event system to process input. Alternatively, you may activate new Input System support with UI Toolkit by adding an EventSystem component to your active scene.");
				}
				return input;
			}

			public void Reset()
			{
				this.m_SendingTouchEvents = false;
				this.m_SendingPenEvent = false;
				this.m_CurrentModifiers = EventModifiers.None;
				this.m_LastMousePressButton = -1;
				this.m_NextMousePressTime = 0f;
				this.m_LastMouseClickCount = 0;
				this.m_LastMousePosition = Vector2.zero;
				this.m_MouseProcessedAtLeastOnce = false;
				this.m_ConsecutiveMoveCount = 0;
				this.m_IsMoveFromKeyboard = false;
				this.m_TouchFingerIdToFingerIndex.Clear();
				this.m_TouchNextFingerIndex = 0;
			}

			public void ProcessLegacyInputEvents()
			{
				this.m_SendingPenEvent = this.ProcessPenEvents();
				bool flag = !this.m_SendingPenEvent;
				if (flag)
				{
					this.m_SendingTouchEvents = this.ProcessTouchEvents();
				}
				bool flag2 = !this.m_SendingPenEvent && !this.m_SendingTouchEvents;
				if (flag2)
				{
					this.ProcessMouseEvents();
				}
				else
				{
					this.m_MouseProcessedAtLeastOnce = false;
				}
				using (this.m_EventSystem.FocusBasedEventSequence())
				{
					this.SendIMGUIEvents();
					this.SendInputEvents();
				}
			}

			private void SendIMGUIEvents()
			{
				bool flag = true;
				while (Event.PopEvent(this.m_Event))
				{
					bool flag2 = this.m_Event.type == EventType.Ignore || this.m_Event.type == EventType.Repaint || this.m_Event.type == EventType.Layout;
					if (!flag2)
					{
						this.m_CurrentModifiers = (flag ? this.m_Event.modifiers : (this.m_CurrentModifiers | this.m_Event.modifiers));
						flag = false;
						bool flag3 = this.m_Event.type == EventType.KeyUp || this.m_Event.type == EventType.KeyDown;
						if (flag3)
						{
							this.m_EventSystem.SendFocusBasedEvent<Event>((Event e) => UIElementsRuntimeUtility.CreateEvent(e), this.m_Event);
							this.ProcessTabEvent(this.m_Event, this.m_CurrentModifiers);
						}
						else
						{
							bool flag4 = this.m_Event.type == EventType.ScrollWheel;
							if (flag4)
							{
								int? num;
								Vector2 vector = UIElementsRuntimeUtility.MultiDisplayBottomLeftToPanelPosition(this.input.mousePosition, out num);
								Vector2 vector2 = vector - this.m_LastMousePosition;
								Vector2 delta = this.m_Event.delta;
								this.m_EventSystem.SendPositionBasedEvent<ValueTuple<EventModifiers, Vector2>>(vector, vector2, PointerId.mousePointerId, num, (Vector3 panelPosition, Vector3 _, [TupleElementNames(new string[] { "modifiers", "scrollDelta" })] ValueTuple<EventModifiers, Vector2> t) => WheelEvent.GetPooled(t.Item2, panelPosition, t.Item1), new ValueTuple<EventModifiers, Vector2>(this.m_CurrentPointerModifiers, delta), false);
							}
							else
							{
								bool flag5 = (!this.m_SendingTouchEvents && !this.m_SendingPenEvent && this.m_Event.pointerType != PointerType.Mouse) || this.m_Event.type == EventType.MouseEnterWindow || this.m_Event.type == EventType.MouseLeaveWindow;
								if (flag5)
								{
									int num2 = ((this.m_Event.pointerType == PointerType.Mouse) ? PointerId.mousePointerId : ((this.m_Event.pointerType == PointerType.Touch) ? PointerId.touchPointerIdBase : PointerId.penPointerIdBase));
									int? num3;
									Vector3 vector3 = UIElementsRuntimeUtility.MultiDisplayToLocalScreenPosition(this.m_Event.mousePosition, out num3);
									Vector2 delta2 = this.m_Event.delta;
									this.m_EventSystem.SendPositionBasedEvent<Event>(vector3, delta2, num2, num3, delegate(Vector3 panelPosition, Vector3 panelDelta, Event evt)
									{
										evt.mousePosition = panelPosition;
										evt.delta = panelDelta;
										return UIElementsRuntimeUtility.CreateEvent(evt);
									}, this.m_Event, this.m_Event.type == EventType.MouseDown || this.m_Event.type == EventType.TouchDown);
								}
							}
						}
					}
				}
			}

			private void ProcessMouseEvents()
			{
				bool flag = !this.input.mousePresent;
				if (!flag)
				{
					int? num;
					Vector2 vector = UIElementsRuntimeUtility.MultiDisplayBottomLeftToPanelPosition(this.input.mousePosition, out num);
					Vector2 vector2 = vector - this.m_LastMousePosition;
					bool flag2 = !this.m_MouseProcessedAtLeastOnce;
					if (flag2)
					{
						vector2 = Vector2.zero;
						this.m_LastMousePosition = vector;
						this.m_MouseProcessedAtLeastOnce = true;
					}
					else
					{
						bool flag3 = !Mathf.Approximately(vector2.x, 0f) || !Mathf.Approximately(vector2.y, 0f);
						if (flag3)
						{
							this.m_LastMousePosition = vector;
							this.m_EventSystem.SendPositionBasedEvent<ValueTuple<EventModifiers, int?>>(vector, vector2, PointerId.mousePointerId, num, (Vector3 panelPosition, Vector3 panelDelta, [TupleElementNames(new string[] { "modifiers", "targetDisplay" })] ValueTuple<EventModifiers, int?> t) => PointerEventBase<PointerMoveEvent>.GetPooled(EventType.MouseMove, panelPosition, panelDelta, -1, 0, t.Item1, t.Item2.GetValueOrDefault()), new ValueTuple<EventModifiers, int?>(this.m_CurrentPointerModifiers, num), false);
						}
					}
					int mouseButtonCount = this.input.mouseButtonCount;
					for (int i = 0; i < mouseButtonCount; i++)
					{
						bool mouseButtonDown = this.input.GetMouseButtonDown(i);
						if (mouseButtonDown)
						{
							bool flag4 = this.m_LastMousePressButton != i || this.input.unscaledTime >= this.m_NextMousePressTime;
							if (flag4)
							{
								this.m_LastMousePressButton = i;
								this.m_LastMouseClickCount = 0;
							}
							int num2 = this.m_LastMouseClickCount + 1;
							this.m_LastMouseClickCount = num2;
							int num3 = num2;
							this.m_NextMousePressTime = this.input.unscaledTime + this.input.doubleClickTime;
							this.m_EventSystem.SendPositionBasedEvent<ValueTuple<int, int, EventModifiers, int?>>(vector, vector2, PointerId.mousePointerId, num, (Vector3 panelPosition, Vector3 panelDelta, [TupleElementNames(new string[] { "button", "clickCount", "modifiers", "targetDisplay" })] ValueTuple<int, int, EventModifiers, int?> t) => PointerEventHelper.GetPooled(EventType.MouseDown, panelPosition, panelDelta, t.Item1, t.Item2, t.Item3, t.Item4.GetValueOrDefault()), new ValueTuple<int, int, EventModifiers, int?>(i, num3, this.m_CurrentPointerModifiers, num), true);
						}
						bool mouseButtonUp = this.input.GetMouseButtonUp(i);
						if (mouseButtonUp)
						{
							int lastMouseClickCount = this.m_LastMouseClickCount;
							this.m_EventSystem.SendPositionBasedEvent<ValueTuple<int, int, EventModifiers, int?>>(vector, vector2, PointerId.mousePointerId, num, (Vector3 panelPosition, Vector3 panelDelta, [TupleElementNames(new string[] { "button", "clickCount", "modifiers", "targetDisplay" })] ValueTuple<int, int, EventModifiers, int?> t) => PointerEventHelper.GetPooled(EventType.MouseUp, panelPosition, panelDelta, t.Item1, t.Item2, t.Item3, t.Item4.GetValueOrDefault()), new ValueTuple<int, int, EventModifiers, int?>(i, lastMouseClickCount, this.m_CurrentPointerModifiers, num), false);
						}
					}
				}
			}

			private void SendInputEvents()
			{
				bool flag = this.ShouldSendMoveFromInput();
				bool flag2 = flag;
				if (flag2)
				{
					this.m_EventSystem.SendFocusBasedEvent<DefaultEventSystem.LegacyInputProcessor>((DefaultEventSystem.LegacyInputProcessor self) => NavigationMoveEvent.GetPooled(self.GetRawMoveVector(), self.m_IsMoveFromKeyboard ? NavigationDeviceType.Keyboard : NavigationDeviceType.NonKeyboard, self.m_CurrentModifiers), this);
				}
				bool buttonDown = this.input.GetButtonDown("Submit");
				if (buttonDown)
				{
					this.m_EventSystem.SendFocusBasedEvent<DefaultEventSystem.LegacyInputProcessor>((DefaultEventSystem.LegacyInputProcessor self) => NavigationEventBase<NavigationSubmitEvent>.GetPooled(self.input.anyKey ? NavigationDeviceType.Keyboard : NavigationDeviceType.NonKeyboard, self.m_CurrentModifiers), this);
				}
				bool buttonDown2 = this.input.GetButtonDown("Cancel");
				if (buttonDown2)
				{
					this.m_EventSystem.SendFocusBasedEvent<DefaultEventSystem.LegacyInputProcessor>((DefaultEventSystem.LegacyInputProcessor self) => NavigationEventBase<NavigationCancelEvent>.GetPooled(self.input.anyKey ? NavigationDeviceType.Keyboard : NavigationDeviceType.NonKeyboard, self.m_CurrentModifiers), this);
				}
			}

			private bool ProcessTouchEvents()
			{
				bool flag = true;
				for (int i = 0; i < this.input.touchCount; i++)
				{
					Touch touch = this.input.GetTouch(i);
					bool flag2 = touch.type == TouchType.Indirect || touch.phase == TouchPhase.Stationary;
					if (!flag2)
					{
						bool flag3 = touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved;
						if (flag3)
						{
							flag = false;
						}
						int? num;
						touch.position = UIElementsRuntimeUtility.MultiDisplayBottomLeftToPanelPosition(touch.position, out num);
						int? num2;
						touch.rawPosition = UIElementsRuntimeUtility.MultiDisplayBottomLeftToPanelPosition(touch.rawPosition, out num2);
						touch.deltaPosition = UIElementsRuntimeUtility.ScreenBottomLeftToPanelDelta(touch.deltaPosition);
						int num3;
						bool flag4 = !this.m_TouchFingerIdToFingerIndex.TryGetValue(touch.fingerId, out num3);
						if (flag4)
						{
							int touchNextFingerIndex = this.m_TouchNextFingerIndex;
							this.m_TouchNextFingerIndex = touchNextFingerIndex + 1;
							num3 = touchNextFingerIndex;
							this.m_TouchFingerIdToFingerIndex.Add(touch.fingerId, num3);
						}
						int num4 = PointerId.touchPointerIdBase + num3;
						this.m_EventSystem.SendPositionBasedEvent<ValueTuple<Touch, int, int?>>(touch.position, touch.deltaPosition, num4, num, delegate(Vector3 panelPosition, Vector3 panelDelta, [TupleElementNames(new string[] { "touch", "pointerId", "targetDisplay" })] ValueTuple<Touch, int, int?> t)
						{
							t.Item1.position = panelPosition;
							t.Item1.deltaPosition = panelDelta;
							return DefaultEventSystem.LegacyInputProcessor.MakeTouchEvent(t.Item1, t.Item2, EventModifiers.None, t.Item3.GetValueOrDefault());
						}, new ValueTuple<Touch, int, int?>(touch, num4, num), false);
					}
				}
				bool flag5 = flag;
				if (flag5)
				{
					this.m_TouchNextFingerIndex = 0;
					this.m_TouchFingerIdToFingerIndex.Clear();
				}
				return this.input.touchCount > 0;
			}

			private bool ProcessPenEvents()
			{
				PenData lastPenContactEvent = this.input.GetLastPenContactEvent();
				bool flag = lastPenContactEvent.contactType == PenEventType.NoContact;
				bool flag2;
				if (flag)
				{
					flag2 = false;
				}
				else
				{
					this.m_EventSystem.SendPositionBasedEvent<PenData>(lastPenContactEvent.position, lastPenContactEvent.deltaPos, PointerId.penPointerIdBase, null, delegate(Vector3 panelPosition, Vector3 panelDelta, PenData _pen)
					{
						_pen.position = panelPosition;
						_pen.deltaPos = panelDelta;
						return DefaultEventSystem.LegacyInputProcessor.MakePenEvent(_pen, EventModifiers.None, 0);
					}, lastPenContactEvent, false);
					this.input.ClearLastPenContactEvent();
					flag2 = true;
				}
				return flag2;
			}

			private Vector2 GetRawMoveVector()
			{
				Vector2 zero = Vector2.zero;
				zero.x = this.input.GetAxisRaw("Horizontal");
				zero.y = this.input.GetAxisRaw("Vertical");
				bool buttonDown = this.input.GetButtonDown("Horizontal");
				if (buttonDown)
				{
					bool flag = zero.x < 0f;
					if (flag)
					{
						zero.x = -1f;
					}
					bool flag2 = zero.x > 0f;
					if (flag2)
					{
						zero.x = 1f;
					}
				}
				bool buttonDown2 = this.input.GetButtonDown("Vertical");
				if (buttonDown2)
				{
					bool flag3 = zero.y < 0f;
					if (flag3)
					{
						zero.y = -1f;
					}
					bool flag4 = zero.y > 0f;
					if (flag4)
					{
						zero.y = 1f;
					}
				}
				return zero;
			}

			private bool ShouldSendMoveFromInput()
			{
				float unscaledTime = this.input.unscaledTime;
				Vector2 rawMoveVector = this.GetRawMoveVector();
				bool flag = Mathf.Approximately(rawMoveVector.x, 0f) && Mathf.Approximately(rawMoveVector.y, 0f);
				bool flag2;
				if (flag)
				{
					this.m_ConsecutiveMoveCount = 0;
					this.m_IsMoveFromKeyboard = false;
					flag2 = false;
				}
				else
				{
					bool flag3 = this.input.GetButtonDown("Horizontal") || this.input.GetButtonDown("Vertical");
					bool flag4 = Vector2.Dot(rawMoveVector, this.m_LastMoveVector) > 0f;
					bool flag5 = !flag3;
					if (flag5)
					{
						bool flag6 = flag4 && this.m_ConsecutiveMoveCount == 1;
						if (flag6)
						{
							flag3 = unscaledTime > this.m_PrevActionTime + 0.5f;
						}
						else
						{
							flag3 = unscaledTime > this.m_PrevActionTime + 0.1f;
						}
					}
					bool flag7 = !flag3;
					if (flag7)
					{
						flag2 = false;
					}
					else
					{
						NavigationMoveEvent.Direction direction = NavigationMoveEvent.DetermineMoveDirection(rawMoveVector.x, rawMoveVector.y, 0.6f);
						bool flag8 = direction > NavigationMoveEvent.Direction.None;
						if (flag8)
						{
							bool flag9 = !flag4;
							if (flag9)
							{
								this.m_ConsecutiveMoveCount = 0;
							}
							this.m_ConsecutiveMoveCount++;
							this.m_PrevActionTime = unscaledTime;
							this.m_LastMoveVector = rawMoveVector;
							this.m_IsMoveFromKeyboard |= this.input.anyKey;
						}
						else
						{
							this.m_ConsecutiveMoveCount = 0;
							this.m_IsMoveFromKeyboard = false;
						}
						flag2 = direction > NavigationMoveEvent.Direction.None;
					}
				}
				return flag2;
			}

			private void ProcessTabEvent(Event e, EventModifiers modifiers)
			{
				bool flag = e.ShouldSendNavigationMoveEventRuntime();
				if (flag)
				{
					NavigationMoveEvent.Direction direction = (e.shift ? NavigationMoveEvent.Direction.Previous : NavigationMoveEvent.Direction.Next);
					this.m_EventSystem.SendFocusBasedEvent<ValueTuple<NavigationMoveEvent.Direction, EventModifiers, DefaultEventSystem.LegacyInputProcessor.IInput>>(([TupleElementNames(new string[] { "direction", "modifiers", "input" })] ValueTuple<NavigationMoveEvent.Direction, EventModifiers, DefaultEventSystem.LegacyInputProcessor.IInput> t) => NavigationMoveEvent.GetPooled(t.Item1, t.Item3.anyKey ? NavigationDeviceType.Keyboard : NavigationDeviceType.NonKeyboard, t.Item2), new ValueTuple<NavigationMoveEvent.Direction, EventModifiers, DefaultEventSystem.LegacyInputProcessor.IInput>(direction, modifiers, this.input));
				}
			}

			private static EventBase MakeTouchEvent(Touch touch, int pointerId, EventModifiers modifiers, int targetDisplay)
			{
				switch (touch.phase)
				{
				case TouchPhase.Began:
					return PointerEventBase<PointerDownEvent>.GetPooled(touch, pointerId, modifiers, targetDisplay);
				case TouchPhase.Moved:
					return PointerEventBase<PointerMoveEvent>.GetPooled(touch, pointerId, modifiers, targetDisplay);
				case TouchPhase.Ended:
					return PointerEventBase<PointerUpEvent>.GetPooled(touch, pointerId, modifiers, targetDisplay);
				case TouchPhase.Canceled:
					return PointerEventBase<PointerCancelEvent>.GetPooled(touch, pointerId, modifiers, targetDisplay);
				}
				return null;
			}

			private static EventBase MakePenEvent(PenData pen, EventModifiers modifiers, int targetDisplay)
			{
				EventBase eventBase;
				switch (pen.contactType)
				{
				case PenEventType.NoContact:
					eventBase = PointerEventBase<PointerMoveEvent>.GetPooled(pen, modifiers, targetDisplay);
					break;
				case PenEventType.PenDown:
					eventBase = PointerEventBase<PointerDownEvent>.GetPooled(pen, modifiers, targetDisplay);
					break;
				case PenEventType.PenUp:
					eventBase = PointerEventBase<PointerUpEvent>.GetPooled(pen, modifiers, targetDisplay);
					break;
				default:
					eventBase = null;
					break;
				}
				return eventBase;
			}

			private const string m_HorizontalAxis = "Horizontal";

			private const string m_VerticalAxis = "Vertical";

			private const string m_SubmitButton = "Submit";

			private const string m_CancelButton = "Cancel";

			private const float m_InputActionsPerSecond = 10f;

			private const float m_RepeatDelay = 0.5f;

			private bool m_SendingTouchEvents;

			private bool m_SendingPenEvent;

			private EventModifiers m_CurrentModifiers;

			private int m_LastMousePressButton = -1;

			private float m_NextMousePressTime = 0f;

			private int m_LastMouseClickCount = 0;

			private Vector2 m_LastMousePosition = Vector2.zero;

			private bool m_MouseProcessedAtLeastOnce;

			private Dictionary<int, int> m_TouchFingerIdToFingerIndex = new Dictionary<int, int>();

			private int m_TouchNextFingerIndex = 0;

			private DefaultEventSystem.LegacyInputProcessor.IInput m_Input;

			private readonly Event m_Event = new Event();

			private readonly DefaultEventSystem m_EventSystem;

			private int m_ConsecutiveMoveCount;

			private Vector2 m_LastMoveVector;

			private float m_PrevActionTime;

			private bool m_IsMoveFromKeyboard;

			internal interface IInput
			{
				bool GetButtonDown(string button);

				float GetAxisRaw(string axis);

				void ResetPenEvents();

				void ClearLastPenContactEvent();

				int penEventCount { get; }

				PenData GetPenEvent(int index);

				PenData GetLastPenContactEvent();

				int touchCount { get; }

				Touch GetTouch(int index);

				bool mousePresent { get; }

				bool GetMouseButtonDown(int button);

				bool GetMouseButtonUp(int button);

				Vector3 mousePosition { get; }

				Vector2 mouseScrollDelta { get; }

				int mouseButtonCount { get; }

				bool anyKey { get; }

				float unscaledTime { get; }

				float doubleClickTime { get; }
			}

			private class Input : DefaultEventSystem.LegacyInputProcessor.IInput
			{
				public bool GetButtonDown(string button)
				{
					return UnityEngine.Input.GetButtonDown(button);
				}

				public float GetAxisRaw(string axis)
				{
					return UnityEngine.Input.GetAxis(axis);
				}

				public void ResetPenEvents()
				{
					UnityEngine.Input.ResetPenEvents();
				}

				public void ClearLastPenContactEvent()
				{
					UnityEngine.Input.ClearLastPenContactEvent();
				}

				public int penEventCount
				{
					get
					{
						return UnityEngine.Input.penEventCount;
					}
				}

				public PenData GetPenEvent(int index)
				{
					return UnityEngine.Input.GetPenEvent(index);
				}

				public PenData GetLastPenContactEvent()
				{
					return UnityEngine.Input.GetLastPenContactEvent();
				}

				public int touchCount
				{
					get
					{
						return UnityEngine.Input.touchCount;
					}
				}

				public Touch GetTouch(int index)
				{
					return UnityEngine.Input.GetTouch(index);
				}

				public bool mousePresent
				{
					get
					{
						return UnityEngine.Input.mousePresent;
					}
				}

				public bool GetMouseButtonDown(int button)
				{
					return UnityEngine.Input.GetMouseButtonDown(button);
				}

				public bool GetMouseButtonUp(int button)
				{
					return UnityEngine.Input.GetMouseButtonUp(button);
				}

				public Vector3 mousePosition
				{
					get
					{
						return UnityEngine.Input.mousePosition;
					}
				}

				public Vector2 mouseScrollDelta
				{
					get
					{
						return UnityEngine.Input.mouseScrollDelta;
					}
				}

				public int mouseButtonCount
				{
					get
					{
						return 3;
					}
				}

				public bool anyKey
				{
					get
					{
						return UnityEngine.Input.anyKey;
					}
				}

				public float unscaledTime
				{
					get
					{
						return Time.unscaledTime;
					}
				}

				public float doubleClickTime
				{
					get
					{
						return (float)Event.GetDoubleClickTime() * 0.001f;
					}
				}
			}

			private class NoInput : DefaultEventSystem.LegacyInputProcessor.IInput
			{
				public bool GetButtonDown(string button)
				{
					return false;
				}

				public float GetAxisRaw(string axis)
				{
					return 0f;
				}

				public int touchCount
				{
					get
					{
						return 0;
					}
				}

				public Touch GetTouch(int index)
				{
					return default(Touch);
				}

				public void ResetPenEvents()
				{
				}

				public void ClearLastPenContactEvent()
				{
				}

				public int penEventCount
				{
					get
					{
						return 0;
					}
				}

				public PenData GetPenEvent(int index)
				{
					return default(PenData);
				}

				public PenData GetLastPenContactEvent()
				{
					return default(PenData);
				}

				public bool mousePresent
				{
					get
					{
						return false;
					}
				}

				public bool GetMouseButtonDown(int button)
				{
					return false;
				}

				public bool GetMouseButtonUp(int button)
				{
					return false;
				}

				public Vector3 mousePosition
				{
					get
					{
						return default(Vector3);
					}
				}

				public Vector2 mouseScrollDelta
				{
					get
					{
						return default(Vector2);
					}
				}

				public int mouseButtonCount
				{
					get
					{
						return 0;
					}
				}

				public bool anyKey
				{
					get
					{
						return false;
					}
				}

				public float unscaledTime
				{
					get
					{
						return 0f;
					}
				}

				public float doubleClickTime
				{
					get
					{
						return float.PositiveInfinity;
					}
				}
			}
		}
	}
}
