using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnityEngine.UIElements
{
	internal abstract class BaseVisualElementPanel : IPanel, IDisposable
	{
		public abstract EventInterests IMGUIEventInterests { get; set; }

		public abstract ScriptableObject ownerObject { get; protected set; }

		public abstract SavePersistentViewData saveViewData { get; set; }

		public abstract GetViewDataDictionary getViewDataDictionary { get; set; }

		public abstract int IMGUIContainersCount { get; set; }

		public abstract IMGUIContainer rootIMGUIContainer { get; set; }

		public abstract FocusController focusController { get; set; }

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
				}
				this.disposed = true;
			}
		}

		public abstract void Repaint(Event e);

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

		internal PanelClearFlags clearFlags { get; set; } = PanelClearFlags.All;

		internal bool duringLayoutPhase { get; set; }

		internal bool isDirty
		{
			get
			{
				return this.version != this.repaintVersion;
			}
		}

		internal abstract uint version { get; }

		internal abstract uint repaintVersion { get; }

		internal abstract void OnVersionChanged(VisualElement ele, VersionChangeType changeTypeFlag);

		internal abstract void SetUpdater(IVisualTreeUpdater updater, VisualTreeUpdatePhase phase);

		internal virtual RepaintData repaintData { get; set; }

		internal virtual ICursorManager cursorManager { get; set; }

		public ContextualMenuManager contextualMenuManager { get; internal set; }

		internal Matrix4x4 GetProjection()
		{
			Rect layout = this.visualTree.layout;
			return ProjectionUtils.Ortho(layout.xMin, layout.xMax, layout.yMax, layout.yMin, -1f, 1f);
		}

		internal Rect GetViewport()
		{
			return this.visualTree.layout;
		}

		public abstract VisualElement visualTree { get; }

		public abstract EventDispatcher dispatcher { get; protected set; }

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

		public abstract ContextType contextType { get; protected set; }

		public abstract VisualElement Pick(Vector2 point);

		public abstract VisualElement PickAll(Vector2 point, List<VisualElement> picked);

		internal bool disposed { get; private set; }

		internal abstract IVisualTreeUpdater GetUpdater(VisualTreeUpdatePhase phase);

		internal VisualElement GetTopElementUnderPointer(int pointerId)
		{
			return this.m_TopElementUnderPointers.GetTopElementUnderPointer(pointerId);
		}

		private void SetElementUnderPointer(VisualElement newElementUnderPointer, int pointerId, Vector2 pointerPos)
		{
			this.m_TopElementUnderPointers.SetElementUnderPointer(newElementUnderPointer, pointerId, pointerPos);
		}

		internal void SetElementUnderPointer(VisualElement newElementUnderPointer, EventBase triggerEvent)
		{
			this.m_TopElementUnderPointers.SetElementUnderPointer(newElementUnderPointer, triggerEvent);
		}

		internal void ClearCachedElementUnderPointer(EventBase triggerEvent)
		{
			this.m_TopElementUnderPointers.SetTemporaryElementUnderPointer(null, triggerEvent);
		}

		internal void CommitElementUnderPointers()
		{
			this.m_TopElementUnderPointers.CommitElementUnderPointers(this.dispatcher);
		}

		internal abstract Shader standardShader { get; set; }

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal event Action standardShaderChanged;

		protected void InvokeStandardShaderChanged()
		{
			bool flag = this.standardShaderChanged != null;
			if (flag)
			{
				this.standardShaderChanged();
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

		internal void UpdateElementUnderPointers()
		{
			foreach (int num in PointerId.hoveringPointers)
			{
				bool flag = PointerDeviceState.GetPanel(num) != this;
				if (flag)
				{
					this.SetElementUnderPointer(null, num, new Vector2(float.MinValue, float.MinValue));
				}
				else
				{
					Vector2 pointerPosition = PointerDeviceState.GetPointerPosition(num);
					VisualElement visualElement = this.PickAll(pointerPosition, null);
					this.SetElementUnderPointer(visualElement, num, pointerPosition);
				}
			}
			this.CommitElementUnderPointers();
		}

		public void Update()
		{
			this.scheduler.UpdateScheduledEvents();
			this.ValidateLayout();
			this.UpdateBindings();
		}

		private float m_Scale = 1f;

		private float m_PixelsPerPoint = 1f;

		internal ElementUnderPointer m_TopElementUnderPointers = new ElementUnderPointer();
	}
}
