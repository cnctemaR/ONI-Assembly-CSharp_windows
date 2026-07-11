using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	internal abstract class BaseVisualElementPanel : IPanel, IDisposable
	{
		public abstract EventInterests IMGUIEventInterests { get; set; }

		public abstract ScriptableObject ownerObject { get; protected set; }

		public abstract SavePersistentViewData savePersistentViewData { get; set; }

		public abstract GetViewDataDictionary getViewDataDictionary { get; set; }

		public abstract int IMGUIContainersCount { get; set; }

		public abstract FocusController focusController { get; set; }

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					UIElementsUtility.RemoveCachedPanel(this.ownerObject.GetInstanceID());
				}
				this.disposed = true;
			}
		}

		public abstract void Repaint(Event e);

		public abstract void ValidateLayout();

		public abstract void UpdateBindings();

		public abstract void ApplyStyles();

		public abstract void DirtyStyleSheets();

		internal float currentPixelsPerPoint { get; set; } = 1f;

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

		internal virtual ContextualMenuManager contextualMenuManager { get; set; }

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

		internal abstract IDataWatchService dataWatch { get; }

		public abstract ContextType contextType { get; protected set; }

		public abstract VisualElement Pick(Vector2 point);

		public abstract VisualElement PickAll(Vector2 point, List<VisualElement> picked);

		public abstract VisualElement LoadTemplate(string path, Dictionary<string, VisualElement> slots = null);

		internal bool disposed { get; private set; }

		internal bool allowPixelCaching { get; set; }

		public abstract bool keepPixelCacheOnWorldBoundChange { get; set; }

		internal abstract IVisualTreeUpdater GetUpdater(VisualTreeUpdatePhase phase);

		internal VisualElement topElementUnderMouse { get; set; }
	}
}
