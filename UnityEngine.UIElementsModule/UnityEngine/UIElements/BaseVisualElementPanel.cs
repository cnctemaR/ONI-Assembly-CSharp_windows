using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Bindings;
using UnityEngine.UIElements.Layout;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule", "UnityEditor.UIToolkitAuthoringModule", "UnityEngine.VectorGraphicsModule" })]
	internal abstract class BaseVisualElementPanel : IPanel, IDisposable, IGroupBox
	{
		public abstract EventInterests IMGUIEventInterests { get; set; }

		public abstract ScriptableObject ownerObject { get; protected set; }

		public abstract SavePersistentViewData saveViewData { get; set; }

		public abstract GetViewDataDictionary getViewDataDictionary { get; set; }

		public abstract int IMGUIContainersCount { get; set; }

		public abstract FocusController focusController { get; set; }

		public abstract IMGUIContainer rootIMGUIContainer { get; set; }

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal event Action<BaseVisualElementPanel> panelDisposed;

		internal UIElementsBridge uiElementsBridge
		{
			get
			{
				bool flag = this.m_UIElementsBridge != null;
				if (flag)
				{
					return this.m_UIElementsBridge;
				}
				throw new Exception("Panel has no UIElementsBridge.");
			}
			set
			{
				this.m_UIElementsBridge = value;
			}
		}

		protected BaseVisualElementPanel()
		{
			this.layoutConfig = LayoutManager.SharedManager.CreateConfig();
			this.layoutConfig.Measure = new LayoutMeasureFunction(VisualElement.Measure);
			this.m_UIElementsBridge = new RuntimeUIElementsBridge();
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			bool disposed = this.disposed;
			if (!disposed)
			{
				if (disposing)
				{
					bool flag = this.ownerObject != null;
					if (flag)
					{
						UIElementsUtility.RemoveCachedPanel(this.ownerObject.GetInstanceID());
					}
					PointerDeviceState.RemovePanelData(this);
				}
				Action<BaseVisualElementPanel> action = this.panelDisposed;
				if (action != null)
				{
					action(this);
				}
				LayoutManager.SharedManager.DestroyConfig(ref this.layoutConfig);
				this.disposed = true;
			}
		}

		public abstract void Repaint(Event e);

		public abstract void ValidateLayout();

		public abstract void TickSchedulingUpdaters();

		public abstract void UpdateForRepaint();

		public abstract void UpdateAnimations();

		public abstract void UpdateBindings();

		public abstract void UpdateDataBinding();

		public abstract void UpdateAuthoring();

		public abstract void ApplyStyles();

		internal unsafe float scale
		{
			get
			{
				return this.m_Scale;
			}
			set
			{
				bool flag = !Mathf.Approximately(this.m_Scale, value);
				if (flag)
				{
					this.m_Scale = value;
					this.visualTree.IncrementVersion(VersionChangeType.Layout);
					*this.layoutConfig.PointScaleFactor = this.scaledPixelsPerPoint;
					this.visualTree.IncrementVersion(VersionChangeType.StyleSheet);
				}
			}
		}

		internal unsafe float pixelsPerPoint
		{
			get
			{
				return this.m_PixelsPerPoint;
			}
			set
			{
				bool flag = !Mathf.Approximately(this.m_PixelsPerPoint, value);
				if (flag)
				{
					this.m_PixelsPerPoint = value;
					this.visualTree.IncrementVersion(VersionChangeType.Layout);
					*this.layoutConfig.PointScaleFactor = this.scaledPixelsPerPoint;
					this.visualTree.IncrementVersion(VersionChangeType.StyleSheet);
				}
			}
		}

		public float scaledPixelsPerPoint
		{
			get
			{
				return this.m_PixelsPerPoint * this.m_Scale;
			}
		}

		public float referenceSpritePixelsPerUnit { get; set; } = 100f;

		internal PanelClearSettings clearSettings { get; set; } = new PanelClearSettings
		{
			clearDepthStencil = true,
			clearColor = true,
			color = Color.clear
		};

		internal bool duringLayoutPhase { get; set; }

		public bool isDirty
		{
			get
			{
				return this.version != this.repaintVersion;
			}
		}

		internal abstract uint version { get; }

		internal abstract uint repaintVersion { get; }

		internal abstract uint hierarchyVersion { get; }

		internal abstract void OnVersionChanged(VisualElement ele, VersionChangeType changeTypeFlag);

		internal abstract void SetUpdater(IVisualTreeUpdater updater, VisualTreeUpdatePhase phase);

		internal virtual RepaintData repaintData { get; set; }

		internal virtual ICursorManager cursorManager { get; set; }

		public ContextualMenuManager contextualMenuManager { get; internal set; }

		internal virtual DataBindingManager dataBindingManager { get; set; }

		public abstract VisualElement visualTree { get; }

		public abstract EventDispatcher dispatcher { get; set; }

		internal void SendEvent(EventBase e, DispatchMode dispatchMode = DispatchMode.Default)
		{
			using (new IMGUIContainer.UITKScope())
			{
				Debug.Assert(this.dispatcher != null, "dispatcher != null");
				e.AssignTimeStamp(this.TimeSinceStartupMs());
				EventDispatcher dispatcher = this.dispatcher;
				if (dispatcher != null)
				{
					dispatcher.Dispatch(e, this, dispatchMode);
				}
			}
		}

		internal TimerEventScheduler scheduler
		{
			get
			{
				TimerEventScheduler timerEventScheduler;
				if ((timerEventScheduler = this.m_Scheduler) == null)
				{
					timerEventScheduler = (this.m_Scheduler = new TimerEventScheduler(this));
				}
				return timerEventScheduler;
			}
		}

		internal abstract IStylePropertyAnimationSystem styleAnimationSystem
		{
			get; [VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			set;
		}

		public abstract ContextType contextType { get; }

		internal TimeFunction TimeSinceStartupFunc
		{
			get
			{
				return this.m_TimeSinceStartupFunc;
			}
			set
			{
				bool flag = this.m_TimeSinceStartupFunc != value;
				if (flag)
				{
					double num = this.TimeSinceStartupSeconds();
					this.m_TimeSinceStartupFunc = value;
					double num2 = this.TimeSinceStartupSeconds();
					this.ApplyTimeAdjustment(num, num2);
				}
			}
		}

		public long TimeSinceStartupMs()
		{
			return (long)(this.TimeSinceStartupSeconds() * 1000.0);
		}

		public double TimeSinceStartupSeconds()
		{
			bool flag = Panel.TimeSinceStartup != null;
			double num;
			if (flag)
			{
				num = (double)Panel.TimeSinceStartup() / 1000.0;
			}
			else
			{
				TimeFunction timeSinceStartupFunc = this.TimeSinceStartupFunc;
				num = ((timeSinceStartupFunc != null) ? timeSinceStartupFunc() : BaseVisualElementPanel.DefaultTimeSinceStartup());
			}
			return num;
		}

		internal static double DefaultTimeSinceStartup()
		{
			return Time.realtimeSinceStartupAsDouble;
		}

		internal virtual void ApplyTimeAdjustment(double currentTimeBefore, double currentTimeAfter)
		{
			this.scheduler.AdjustCurrentTime(currentTimeBefore, currentTimeAfter);
		}

		public VisualElement Pick(Vector2 point)
		{
			return this.Pick(point, PointerId.mousePointerId);
		}

		public abstract VisualElement Pick(Vector2 point, int pointerId);

		public abstract VisualElement PickAll(Vector2 point, List<VisualElement> picked);

		internal bool disposed { get; private set; }

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal abstract IVisualTreeUpdater GetUpdater(VisualTreeUpdatePhase phase);

		internal VisualElement GetTopElementUnderPointer(int pointerId)
		{
			return this.m_TopElementUnderPointers.GetTopElementUnderPointer(pointerId);
		}

		internal void RemoveElementFromPointerCache(VisualElement e)
		{
			this.m_TopElementUnderPointers.RemoveElementUnderPointer(e);
		}

		internal void SetTopElementUnderPointer(int pointerId, VisualElement element, EventBase triggerEvent)
		{
			this.m_TopElementUnderPointers.SetElementUnderPointer(element, pointerId, triggerEvent);
		}

		internal void SetTopElementUnderPointer(int pointerId, VisualElement element, Vector2 position)
		{
			this.m_TopElementUnderPointers.SetElementUnderPointer(element, pointerId, position);
		}

		internal VisualElement RecomputeTopElementUnderPointer(int pointerId, Vector2 pointerPos, EventBase triggerEvent)
		{
			bool flag = !this.isFlat;
			VisualElement visualElement;
			if (flag)
			{
				visualElement = this.GetTopElementUnderPointer(pointerId);
			}
			else
			{
				VisualElement visualElement2 = null;
				bool flag2 = PointerDeviceState.GetPanel(pointerId, this.contextType) == this && !PointerDeviceState.HasLocationFlag(pointerId, this.contextType, PointerDeviceState.LocationFlag.OutsidePanel);
				if (flag2)
				{
					visualElement2 = this.Pick(pointerPos, pointerId);
				}
				this.m_TopElementUnderPointers.SetElementUnderPointer(visualElement2, pointerId, triggerEvent);
				visualElement = visualElement2;
			}
			return visualElement;
		}

		internal void ClearCachedElementUnderPointer(int pointerId, EventBase triggerEvent)
		{
			this.m_TopElementUnderPointers.SetTemporaryElementUnderPointer(null, pointerId, triggerEvent);
		}

		internal bool CommitElementUnderPointers()
		{
			return this.m_TopElementUnderPointers.CommitElementUnderPointers(this.dispatcher, this.contextType);
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal event Action isFlatChanged;

		public bool isFlat
		{
			get
			{
				return this.m_IsFlat;
			}
			set
			{
				bool flag = this.m_IsFlat == value;
				if (!flag)
				{
					this.m_IsFlat = value;
					this.SetSpecializedHierarchyFlagsUpdater();
					Action action = this.isFlatChanged;
					if (action != null)
					{
						action();
					}
				}
			}
		}

		internal void SetSpecializedHierarchyFlagsUpdater()
		{
			IVisualTreeUpdater updater = this.GetUpdater(VisualTreeUpdatePhase.TransformClip);
			bool flag = updater is VisualTreeWorldSpaceHierarchyFlagsUpdater;
			bool isFlat = this.isFlat;
			if (isFlat)
			{
				bool flag2 = flag;
				if (flag2)
				{
					this.SetUpdater(new VisualTreeHierarchyFlagsUpdater(), VisualTreeUpdatePhase.TransformClip);
				}
			}
			else
			{
				bool flag3 = !flag;
				if (flag3)
				{
					this.SetUpdater(new VisualTreeWorldSpaceHierarchyFlagsUpdater(), VisualTreeUpdatePhase.TransformClip);
				}
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal event Action atlasChanged;

		protected void InvokeAtlasChanged()
		{
			Action action = this.atlasChanged;
			if (action != null)
			{
				action();
			}
		}

		public abstract AtlasBase atlas { get; set; }

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal event HierarchyEvent hierarchyChanged;

		internal void InvokeHierarchyChanged(VisualElement ve, HierarchyChangeType changeType, IReadOnlyList<VisualElement> additionalContext = null)
		{
			bool flag = this.hierarchyChanged != null;
			if (flag)
			{
				this.hierarchyChanged(ve, changeType, additionalContext);
			}
		}

		[Obsolete("This exists only to support GraphView. Do not add new usage of this event.")]
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal event Action<IPanel> beforeUpdate;

		internal void InvokeBeforeUpdate()
		{
			Action<IPanel> action = this.beforeUpdate;
			if (action != null)
			{
				action(this);
			}
		}

		internal bool UpdateElementUnderPointers()
		{
			foreach (int num in PointerId.screenHoveringPointers)
			{
				bool flag = PointerDeviceState.GetPanel(num, this.contextType) != this || PointerDeviceState.HasLocationFlag(num, this.contextType, PointerDeviceState.LocationFlag.OutsidePanel);
				if (flag)
				{
					this.m_TopElementUnderPointers.SetElementUnderPointer(null, num, BaseVisualElementPanel.s_OutsidePanelCoordinates);
				}
				else
				{
					bool isFlat = this.isFlat;
					if (isFlat)
					{
						Vector3 pointerPosition = PointerDeviceState.GetPointerPosition(num, this.contextType);
						VisualElement visualElement = this.PickAll(pointerPosition, null);
						this.m_TopElementUnderPointers.SetElementUnderPointer(visualElement, num, pointerPosition);
					}
				}
			}
			return this.CommitElementUnderPointers();
		}

		void IGroupBox.OnOptionAdded(IGroupBoxOption option)
		{
		}

		void IGroupBox.OnOptionRemoved(IGroupBoxOption option)
		{
		}

		public void RegisterChangeProcessor(IVisualElementChangeProcessor processor)
		{
			VisualTreeAuthoringUpdater visualTreeAuthoringUpdater = this.GetUpdater(VisualTreeUpdatePhase.Authoring) as VisualTreeAuthoringUpdater;
			if (visualTreeAuthoringUpdater != null)
			{
				visualTreeAuthoringUpdater.RegisterProcessor(processor);
			}
		}

		public void UnregisterChangeProcessor(IVisualElementChangeProcessor processor)
		{
			VisualTreeAuthoringUpdater visualTreeAuthoringUpdater = this.GetUpdater(VisualTreeUpdatePhase.Authoring) as VisualTreeAuthoringUpdater;
			if (visualTreeAuthoringUpdater != null)
			{
				visualTreeAuthoringUpdater.UnregisterProcessor(processor);
			}
		}

		public virtual void Render()
		{
			this.panelRenderer.Render();
		}

		internal AbstractGenericMenu CreateMenu()
		{
			return this.CreateMenuFunctor();
		}

		private UIElementsBridge m_UIElementsBridge;

		private float m_Scale = 1f;

		internal LayoutConfig layoutConfig;

		private float m_PixelsPerPoint = 1f;

		internal IPanelRenderer panelRenderer;

		private TimerEventScheduler m_Scheduler;

		private TimeFunction m_TimeSinceStartupFunc;

		internal ElementUnderPointer m_TopElementUnderPointers = new ElementUnderPointer();

		private bool m_IsFlat = true;

		internal static readonly Vector2 s_OutsidePanelCoordinates = new Vector2(-2.1474836E+09f, -2.1474836E+09f);

		public readonly Lazy<HashSet<TextElement>> textElementRegistry = new Lazy<HashSet<TextElement>>(false);

		internal Func<AbstractGenericMenu> CreateMenuFunctor = () => new GenericDropdownMenu();
	}
}
