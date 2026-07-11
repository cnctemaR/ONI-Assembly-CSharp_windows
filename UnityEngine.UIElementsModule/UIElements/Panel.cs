using System;
using System.Collections.Generic;
using Unity.Profiling;

namespace UnityEngine.UIElements
{
	internal class Panel : BaseVisualElementPanel
	{
		public override VisualElement visualTree
		{
			get
			{
				return this.m_RootContainer;
			}
		}

		public override EventDispatcher dispatcher { get; protected set; }

		public TimerEventScheduler timerEventScheduler
		{
			get
			{
				TimerEventScheduler timerEventScheduler;
				if ((timerEventScheduler = this.m_Scheduler) == null)
				{
					timerEventScheduler = (this.m_Scheduler = new TimerEventScheduler());
				}
				return timerEventScheduler;
			}
		}

		internal override IScheduler scheduler
		{
			get
			{
				return this.timerEventScheduler;
			}
		}

		public override ScriptableObject ownerObject { get; protected set; }

		public override ContextType contextType { get; protected set; }

		public override SavePersistentViewData saveViewData { get; set; }

		public override GetViewDataDictionary getViewDataDictionary { get; set; }

		public override FocusController focusController { get; set; }

		public override EventInterests IMGUIEventInterests { get; set; }

		internal static LoadResourceFunction loadResourceFunc { private get; set; }

		internal static Object LoadResource(string pathName, Type type, float dpiScaling)
		{
			bool flag = Panel.loadResourceFunc != null;
			Object @object;
			if (flag)
			{
				@object = Panel.loadResourceFunc(pathName, type, dpiScaling);
			}
			else
			{
				@object = Resources.Load(pathName, type);
			}
			return @object;
		}

		internal void Focus()
		{
			bool flag = this.m_SavedFocusedElement != null && !(this.m_SavedFocusedElement is IMGUIContainer);
			if (flag)
			{
				this.m_SavedFocusedElement.Focus();
			}
			this.m_SavedFocusedElement = null;
		}

		internal void Blur()
		{
			FocusController focusController = this.focusController;
			this.m_SavedFocusedElement = ((focusController != null) ? focusController.GetLeafFocusedElement() : null);
			bool flag = this.m_SavedFocusedElement != null && !(this.m_SavedFocusedElement is IMGUIContainer);
			if (flag)
			{
				this.m_SavedFocusedElement.Blur();
			}
		}

		internal string name
		{
			get
			{
				return this.m_PanelName;
			}
			set
			{
				this.m_PanelName = value;
				this.CreateMarkers();
			}
		}

		private void CreateMarkers()
		{
			bool flag = !string.IsNullOrEmpty(this.m_PanelName);
			if (flag)
			{
				this.m_MarkerUpdate = new ProfilerMarker("Panel.Update." + this.m_PanelName);
				this.m_MarkerLayout = new ProfilerMarker("Panel.Layout." + this.m_PanelName);
				this.m_MarkerBindings = new ProfilerMarker("Panel.Bindings." + this.m_PanelName);
				this.m_MarkerAnimations = new ProfilerMarker("Panel.Animations." + this.m_PanelName);
			}
			else
			{
				this.m_MarkerUpdate = new ProfilerMarker("Panel.Update");
				this.m_MarkerLayout = new ProfilerMarker("Panel.Layout");
				this.m_MarkerBindings = new ProfilerMarker("Panel.Bindings");
				this.m_MarkerAnimations = new ProfilerMarker("Panel.Animations");
			}
		}

		internal static TimeMsFunction TimeSinceStartup
		{
			get
			{
				return Panel.s_TimeSinceStartup;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					value = new TimeMsFunction(Panel.DefaultTimeSinceStartupMs);
				}
				Panel.s_TimeSinceStartup = value;
			}
		}

		public override int IMGUIContainersCount { get; set; }

		public override IMGUIContainer rootIMGUIContainer { get; set; }

		internal override uint version
		{
			get
			{
				return this.m_Version;
			}
		}

		internal override uint repaintVersion
		{
			get
			{
				return this.m_RepaintVersion;
			}
		}

		internal override Shader standardShader
		{
			get
			{
				return this.m_StandardShader;
			}
			set
			{
				bool flag = this.m_StandardShader != value;
				if (flag)
				{
					this.m_StandardShader = value;
					base.InvokeStandardShaderChanged();
				}
			}
		}

		internal static Panel CreateEditorPanel(ScriptableObject ownerObject)
		{
			return new Panel(ownerObject, ContextType.Editor, new EventDispatcher());
		}

		public Panel(ScriptableObject ownerObject, ContextType contextType, EventDispatcher dispatcher)
		{
			this.m_VisualTreeUpdater = new VisualTreeUpdater(this);
			this.ownerObject = ownerObject;
			this.contextType = contextType;
			this.dispatcher = dispatcher;
			this.repaintData = new RepaintData();
			this.cursorManager = new CursorManager();
			base.contextualMenuManager = null;
			this.m_RootContainer = new VisualElement
			{
				name = VisualElementUtils.GetUniqueName("unity-panel-container"),
				viewDataKey = "PanelContainer"
			};
			this.visualTree.SetPanel(this);
			this.focusController = new FocusController(new VisualElementFocusRing(this.visualTree, VisualElementFocusRing.DefaultFocusOrder.ChildOrder));
			this.CreateMarkers();
			base.InvokeHierarchyChanged(this.visualTree, HierarchyChangeType.Add);
		}

		protected override void Dispose(bool disposing)
		{
			bool disposed = base.disposed;
			if (!disposed)
			{
				if (disposing)
				{
					this.m_VisualTreeUpdater.Dispose();
				}
				base.Dispose(disposing);
			}
		}

		public static long TimeSinceStartupMs()
		{
			return (Panel.s_TimeSinceStartup == null) ? Panel.DefaultTimeSinceStartupMs() : Panel.s_TimeSinceStartup();
		}

		internal static long DefaultTimeSinceStartupMs()
		{
			return (long)(Time.realtimeSinceStartup * 1000f);
		}

		internal static VisualElement PickAllWithoutValidatingLayout(VisualElement root, Vector2 point)
		{
			return Panel.PickAll(root, point, null);
		}

		private static VisualElement PickAll(VisualElement root, Vector2 point, List<VisualElement> picked = null)
		{
			return Panel.PerformPick(root, point, picked);
		}

		private static VisualElement PerformPick(VisualElement root, Vector2 point, List<VisualElement> picked = null)
		{
			bool flag = root.resolvedStyle.display == DisplayStyle.None;
			VisualElement visualElement;
			if (flag)
			{
				visualElement = null;
			}
			else
			{
				bool flag2 = root.pickingMode == PickingMode.Ignore && root.hierarchy.childCount == 0;
				if (flag2)
				{
					visualElement = null;
				}
				else
				{
					bool flag3 = !root.worldBoundingBox.Contains(point);
					if (flag3)
					{
						visualElement = null;
					}
					else
					{
						Vector2 vector = root.WorldToLocal(point);
						bool flag4 = root.ContainsPoint(vector);
						bool flag5 = !flag4 && root.ShouldClip();
						if (flag5)
						{
							visualElement = null;
						}
						else
						{
							VisualElement visualElement2 = null;
							int childCount = root.hierarchy.childCount;
							for (int i = childCount - 1; i >= 0; i--)
							{
								VisualElement visualElement3 = root.hierarchy[i];
								VisualElement visualElement4 = Panel.PerformPick(visualElement3, point, picked);
								bool flag6 = visualElement2 == null && visualElement4 != null && visualElement4.visible;
								if (flag6)
								{
									visualElement2 = visualElement4;
								}
							}
							bool flag7 = picked != null && root.enabledInHierarchy && root.visible && root.pickingMode == PickingMode.Position && flag4;
							if (flag7)
							{
								picked.Add(root);
							}
							bool flag8 = visualElement2 != null;
							if (flag8)
							{
								visualElement = visualElement2;
							}
							else
							{
								PickingMode pickingMode = root.pickingMode;
								if (pickingMode != PickingMode.Position)
								{
									if (pickingMode != PickingMode.Ignore)
									{
									}
								}
								else
								{
									bool flag9 = flag4 && root.enabledInHierarchy && root.visible;
									if (flag9)
									{
										return root;
									}
								}
								visualElement = null;
							}
						}
					}
				}
			}
			return visualElement;
		}

		public override VisualElement PickAll(Vector2 point, List<VisualElement> picked)
		{
			this.ValidateLayout();
			bool flag = picked != null;
			if (flag)
			{
				picked.Clear();
			}
			return Panel.PickAll(this.visualTree, point, picked);
		}

		public override VisualElement Pick(Vector2 point)
		{
			this.ValidateLayout();
			Vector2 vector;
			bool flag;
			VisualElement topElementUnderPointer = this.m_TopElementUnderPointers.GetTopElementUnderPointer(PointerId.mousePointerId, out vector, out flag);
			bool flag2 = !flag && (vector - point).sqrMagnitude < 0.25f;
			VisualElement visualElement;
			if (flag2)
			{
				visualElement = topElementUnderPointer;
			}
			else
			{
				visualElement = Panel.PickAll(this.visualTree, point, null);
			}
			return visualElement;
		}

		public override void ValidateLayout()
		{
			bool flag = !this.m_ValidatingLayout;
			if (flag)
			{
				this.m_ValidatingLayout = true;
				this.m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.Styles);
				this.m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.Layout);
				this.m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.TransformClip);
				this.m_ValidatingLayout = false;
			}
		}

		public override void UpdateAnimations()
		{
			this.m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.Animation);
		}

		public override void UpdateBindings()
		{
			this.m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.Bindings);
		}

		public override void ApplyStyles()
		{
			this.m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.Styles);
		}

		private void UpdateForRepaint()
		{
			this.m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.ViewData);
			this.m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.Styles);
			this.m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.Layout);
			this.m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.TransformClip);
			this.m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.Repaint);
		}

		public override void Repaint(Event e)
		{
			bool flag = this.contextType == ContextType.Editor;
			if (flag)
			{
				Debug.Assert(GUIClip.Internal_GetCount() == 0, "UIElement is not compatible with IMGUI GUIClips, only GUIClip.ParentClipScope");
			}
			this.m_RepaintVersion = this.version;
			bool flag2 = this.contextType == ContextType.Editor;
			if (flag2)
			{
				base.pixelsPerPoint = GUIUtility.pixelsPerPoint;
			}
			this.repaintData.repaintEvent = e;
			using (this.m_MarkerUpdate.Auto())
			{
				this.UpdateForRepaint();
			}
		}

		internal override void OnVersionChanged(VisualElement ve, VersionChangeType versionChangeType)
		{
			this.m_Version += 1U;
			this.m_VisualTreeUpdater.OnVersionChanged(ve, versionChangeType);
		}

		internal override void SetUpdater(IVisualTreeUpdater updater, VisualTreeUpdatePhase phase)
		{
			this.m_VisualTreeUpdater.SetUpdater(updater, phase);
		}

		internal override IVisualTreeUpdater GetUpdater(VisualTreeUpdatePhase phase)
		{
			return this.m_VisualTreeUpdater.GetUpdater(phase);
		}

		private VisualElement m_RootContainer;

		private VisualTreeUpdater m_VisualTreeUpdater;

		private string m_PanelName;

		private uint m_Version = 0U;

		private uint m_RepaintVersion = 0U;

		internal static Action BeforeUpdaterChange;

		internal static Action AfterUpdaterChange;

		private ProfilerMarker m_MarkerUpdate;

		private ProfilerMarker m_MarkerLayout;

		private ProfilerMarker m_MarkerBindings;

		private ProfilerMarker m_MarkerAnimations;

		private static ProfilerMarker s_MarkerPickAll = new ProfilerMarker("Panel.PickAll");

		private TimerEventScheduler m_Scheduler;

		private Focusable m_SavedFocusedElement;

		private static TimeMsFunction s_TimeSinceStartup;

		private Shader m_StandardShader;

		private bool m_ValidatingLayout = false;
	}
}
