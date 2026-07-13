using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Yoga;

namespace UnityEngine.UIElements
{
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
			this.yogaConfig = new YogaConfig();
			this.yogaConfig.UseWebDefaults = YogaConfig.Default.UseWebDefaults;
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
				this.yogaConfig = null;
				this.disposed = true;
			}
		}

		public abstract void Repaint(Event e);

		public abstract void ValidateFocus();

		public abstract void ValidateLayout();

		public abstract void UpdateAnimations();

		public abstract void UpdateBindings();

		public abstract void ApplyStyles();

		internal float scale
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
					this.yogaConfig.PointScaleFactor = this.scaledPixelsPerPoint;
					this.visualTree.IncrementVersion(VersionChangeType.StyleSheet);
				}
			}
		}

		internal float pixelsPerPoint
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
					this.yogaConfig.PointScaleFactor = this.scaledPixelsPerPoint;
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

		public PanelClearFlags clearFlags
		{
			get
			{
				PanelClearFlags panelClearFlags = PanelClearFlags.None;
				bool clearColor = this.clearSettings.clearColor;
				if (clearColor)
				{
					panelClearFlags |= PanelClearFlags.Color;
				}
				bool clearDepthStencil = this.clearSettings.clearDepthStencil;
				if (clearDepthStencil)
				{
					panelClearFlags |= PanelClearFlags.Depth;
				}
				return panelClearFlags;
			}
			set
			{
				PanelClearSettings clearSettings = this.clearSettings;
				clearSettings.clearColor = (value & PanelClearFlags.Color) == PanelClearFlags.Color;
				clearSettings.clearDepthStencil = (value & PanelClearFlags.Depth) == PanelClearFlags.Depth;
				this.clearSettings = clearSettings;
			}
		}

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

		public abstract VisualElement visualTree { get; }

		public abstract EventDispatcher dispatcher { get; set; }

		internal void SendEvent(EventBase e, DispatchMode dispatchMode = DispatchMode.Default)
		{
			Debug.Assert(this.dispatcher != null);
			EventDispatcher dispatcher = this.dispatcher;
			if (dispatcher != null)
			{
				dispatcher.Dispatch(e, this, dispatchMode);
			}
		}

		internal abstract IScheduler scheduler { get; }

		internal abstract IStylePropertyAnimationSystem styleAnimationSystem { get; set; }

		public abstract ContextType contextType { get; protected set; }

		public abstract VisualElement Pick(Vector2 point);

		public abstract VisualElement PickAll(Vector2 point, List<VisualElement> picked);

		internal bool disposed { get; private set; }

		internal abstract IVisualTreeUpdater GetUpdater(VisualTreeUpdatePhase phase);

		internal VisualElement GetTopElementUnderPointer(int pointerId)
		{
			return this.m_TopElementUnderPointers.GetTopElementUnderPointer(pointerId);
		}

		internal VisualElement RecomputeTopElementUnderPointer(int pointerId, Vector2 pointerPos, EventBase triggerEvent)
		{
			VisualElement visualElement = null;
			bool flag = PointerDeviceState.GetPanel(pointerId, this.contextType) == this && !PointerDeviceState.HasLocationFlag(pointerId, this.contextType, PointerDeviceState.LocationFlag.OutsidePanel);
			if (flag)
			{
				visualElement = this.Pick(pointerPos);
			}
			this.m_TopElementUnderPointers.SetElementUnderPointer(visualElement, pointerId, triggerEvent);
			return visualElement;
		}

		internal void ClearCachedElementUnderPointer(int pointerId, EventBase triggerEvent)
		{
			this.m_TopElementUnderPointers.SetTemporaryElementUnderPointer(null, pointerId, triggerEvent);
		}

		internal void CommitElementUnderPointers()
		{
			this.m_TopElementUnderPointers.CommitElementUnderPointers(this.dispatcher, this.contextType);
		}

		internal abstract Shader standardShader { get; set; }

		internal virtual Shader standardWorldSpaceShader
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal event Action standardShaderChanged;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal event Action standardWorldSpaceShaderChanged;

		protected void InvokeStandardShaderChanged()
		{
			bool flag = this.standardShaderChanged != null;
			if (flag)
			{
				this.standardShaderChanged();
			}
		}

		protected void InvokeStandardWorldSpaceShaderChanged()
		{
			bool flag = this.standardWorldSpaceShaderChanged != null;
			if (flag)
			{
				this.standardWorldSpaceShaderChanged();
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
		internal event Action<Material> updateMaterial;

		internal void InvokeUpdateMaterial(Material mat)
		{
			Action<Material> action = this.updateMaterial;
			if (action != null)
			{
				action(mat);
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal event HierarchyEvent hierarchyChanged;

		internal void InvokeHierarchyChanged(VisualElement ve, HierarchyChangeType changeType)
		{
			bool flag = this.hierarchyChanged != null;
			if (flag)
			{
				this.hierarchyChanged(ve, changeType);
			}
		}

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

		internal void UpdateElementUnderPointers()
		{
			foreach (int num in PointerId.hoveringPointers)
			{
				bool flag = PointerDeviceState.GetPanel(num, this.contextType) != this || PointerDeviceState.HasLocationFlag(num, this.contextType, PointerDeviceState.LocationFlag.OutsidePanel);
				if (flag)
				{
					this.m_TopElementUnderPointers.SetElementUnderPointer(null, num, new Vector2(float.MinValue, float.MinValue));
				}
				else
				{
					Vector2 pointerPosition = PointerDeviceState.GetPointerPosition(num, this.contextType);
					VisualElement visualElement = this.PickAll(pointerPosition, null);
					this.m_TopElementUnderPointers.SetElementUnderPointer(visualElement, num, pointerPosition);
				}
			}
			this.CommitElementUnderPointers();
		}

		void IGroupBox.OnOptionAdded(IGroupBoxOption option)
		{
		}

		void IGroupBox.OnOptionRemoved(IGroupBoxOption option)
		{
		}

		internal virtual IGenericMenu CreateMenu()
		{
			return new GenericDropdownMenu();
		}

		public virtual void Update()
		{
			this.scheduler.UpdateScheduledEvents();
			this.ValidateFocus();
			this.ValidateLayout();
			this.UpdateAnimations();
			this.UpdateBindings();
		}

		private UIElementsBridge m_UIElementsBridge;

		private float m_Scale = 1f;

		internal YogaConfig yogaConfig;

		private float m_PixelsPerPoint = 1f;

		internal ElementUnderPointer m_TopElementUnderPointers = new ElementUnderPointer();
	}
}
