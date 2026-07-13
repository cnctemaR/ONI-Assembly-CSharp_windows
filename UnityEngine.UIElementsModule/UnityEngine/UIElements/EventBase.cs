using System;
using JetBrains.Annotations;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	public abstract class EventBase : IDisposable
	{
		protected static long RegisterEventType()
		{
			return EventBase.s_LastTypeId += 1L;
		}

		public virtual long eventTypeId
		{
			get
			{
				return -1L;
			}
		}

		internal int eventCategories { get; }

		public long timestamp { get; private set; }

		internal ulong eventId { get; private set; }

		internal ulong triggerEventId { get; private set; }

		internal void SetTriggerEventId(ulong id)
		{
			this.triggerEventId = id;
		}

		internal EventBase.EventPropagation propagation { get; set; }

		private EventBase.LifeCycleStatus lifeCycleStatus { get; set; }

		[Obsolete("Override PreDispatch(IPanel panel) instead.")]
		protected virtual void PreDispatch()
		{
		}

		protected internal virtual void PreDispatch(IPanel panel)
		{
			this.PreDispatch();
		}

		[Obsolete("Override PostDispatch(IPanel panel) instead.")]
		protected virtual void PostDispatch()
		{
		}

		protected internal virtual void PostDispatch(IPanel panel)
		{
			this.PostDispatch();
			this.processed = true;
		}

		internal virtual void Dispatch([JetBrains.Annotations.NotNull] BaseVisualElementPanel panel)
		{
			EventDispatchUtilities.DefaultDispatch(this, panel);
		}

		public bool bubbles
		{
			get
			{
				return (this.propagation & EventBase.EventPropagation.Bubbles) > EventBase.EventPropagation.None;
			}
			protected set
			{
				if (value)
				{
					this.propagation |= EventBase.EventPropagation.Bubbles;
				}
				else
				{
					this.propagation &= ~EventBase.EventPropagation.Bubbles;
				}
			}
		}

		public bool tricklesDown
		{
			get
			{
				return (this.propagation & EventBase.EventPropagation.TricklesDown) > EventBase.EventPropagation.None;
			}
			protected set
			{
				if (value)
				{
					this.propagation |= EventBase.EventPropagation.TricklesDown;
				}
				else
				{
					this.propagation &= ~EventBase.EventPropagation.TricklesDown;
				}
			}
		}

		internal bool skipDisabledElements
		{
			get
			{
				return (this.propagation & EventBase.EventPropagation.SkipDisabledElements) > EventBase.EventPropagation.None;
			}
			set
			{
				if (value)
				{
					this.propagation |= EventBase.EventPropagation.SkipDisabledElements;
				}
				else
				{
					this.propagation &= ~EventBase.EventPropagation.SkipDisabledElements;
				}
			}
		}

		internal bool bubblesOrTricklesDown
		{
			get
			{
				return (this.propagation & EventBase.EventPropagation.BubblesOrTricklesDown) > EventBase.EventPropagation.None;
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal VisualElement elementTarget { get; set; }

		public IEventHandler target
		{
			get
			{
				return this.elementTarget;
			}
			set
			{
				this.elementTarget = value as VisualElement;
			}
		}

		public bool isPropagationStopped
		{
			get
			{
				return (this.lifeCycleStatus & EventBase.LifeCycleStatus.PropagationStopped) > EventBase.LifeCycleStatus.None;
			}
			private set
			{
				if (value)
				{
					this.lifeCycleStatus |= EventBase.LifeCycleStatus.PropagationStopped;
				}
				else
				{
					this.lifeCycleStatus &= ~EventBase.LifeCycleStatus.PropagationStopped;
				}
			}
		}

		public void StopPropagation()
		{
			this.isPropagationStopped = true;
		}

		public bool isImmediatePropagationStopped
		{
			get
			{
				return (this.lifeCycleStatus & EventBase.LifeCycleStatus.ImmediatePropagationStopped) > EventBase.LifeCycleStatus.None;
			}
			private set
			{
				if (value)
				{
					this.lifeCycleStatus |= EventBase.LifeCycleStatus.ImmediatePropagationStopped;
				}
				else
				{
					this.lifeCycleStatus &= ~EventBase.LifeCycleStatus.ImmediatePropagationStopped;
				}
			}
		}

		public void StopImmediatePropagation()
		{
			this.isPropagationStopped = true;
			this.isImmediatePropagationStopped = true;
		}

		[Obsolete("Use isPropagationStopped. Before proceeding, make sure you understand the latest changes to UIToolkit event propagation rules by visiting Unity's manual page https://docs.unity3d.com/Manual/UIE-Events-Dispatching.html")]
		public bool isDefaultPrevented
		{
			get
			{
				return this.isPropagationStopped;
			}
		}

		[Obsolete("Use StopPropagation and/or FocusController.IgnoreEvent. Before proceeding, make sure you understand the latest changes to UIToolkit event propagation rules by visiting Unity's manual page https://docs.unity3d.com/Manual/UIE-Events-Dispatching.html")]
		public void PreventDefault()
		{
			this.StopPropagation();
			VisualElement elementTarget = this.elementTarget;
			if (elementTarget != null)
			{
				FocusController focusController = elementTarget.focusController;
				if (focusController != null)
				{
					focusController.IgnoreEvent(this);
				}
			}
		}

		public PropagationPhase propagationPhase { get; internal set; }

		public virtual IEventHandler currentTarget
		{
			get
			{
				return this.m_CurrentTarget;
			}
			internal set
			{
				this.m_CurrentTarget = value;
				bool flag = this.imguiEvent != null;
				if (flag)
				{
					VisualElement visualElement = this.currentTarget as VisualElement;
					bool flag2 = visualElement != null;
					if (flag2)
					{
						this.imguiEvent.mousePosition = visualElement.WorldToLocal3D(this.originalMousePosition);
					}
					else
					{
						this.imguiEvent.mousePosition = this.originalMousePosition;
					}
				}
			}
		}

		public bool dispatch
		{
			get
			{
				return (this.lifeCycleStatus & EventBase.LifeCycleStatus.Dispatching) > EventBase.LifeCycleStatus.None;
			}
			internal set
			{
				if (value)
				{
					this.lifeCycleStatus |= EventBase.LifeCycleStatus.Dispatching;
					this.dispatched = true;
				}
				else
				{
					this.lifeCycleStatus &= ~EventBase.LifeCycleStatus.Dispatching;
				}
			}
		}

		internal void MarkReceivedByDispatcher()
		{
			Debug.Assert(!this.dispatched, "Events cannot be dispatched more than once.");
			this.dispatched = true;
		}

		private bool dispatched
		{
			get
			{
				return (this.lifeCycleStatus & EventBase.LifeCycleStatus.Dispatched) > EventBase.LifeCycleStatus.None;
			}
			set
			{
				if (value)
				{
					this.lifeCycleStatus |= EventBase.LifeCycleStatus.Dispatched;
				}
				else
				{
					this.lifeCycleStatus &= ~EventBase.LifeCycleStatus.Dispatched;
				}
			}
		}

		internal bool processed
		{
			get
			{
				return (this.lifeCycleStatus & EventBase.LifeCycleStatus.Processed) > EventBase.LifeCycleStatus.None;
			}
			private set
			{
				if (value)
				{
					this.lifeCycleStatus |= EventBase.LifeCycleStatus.Processed;
				}
				else
				{
					this.lifeCycleStatus &= ~EventBase.LifeCycleStatus.Processed;
				}
			}
		}

		internal bool processedByFocusController
		{
			get
			{
				return (this.lifeCycleStatus & EventBase.LifeCycleStatus.ProcessedByFocusController) > EventBase.LifeCycleStatus.None;
			}
			set
			{
				if (value)
				{
					this.lifeCycleStatus |= EventBase.LifeCycleStatus.ProcessedByFocusController;
				}
				else
				{
					this.lifeCycleStatus &= ~EventBase.LifeCycleStatus.ProcessedByFocusController;
				}
			}
		}

		internal bool propagateToIMGUI
		{
			get
			{
				return (this.lifeCycleStatus & EventBase.LifeCycleStatus.PropagateToIMGUI) > EventBase.LifeCycleStatus.None;
			}
			set
			{
				if (value)
				{
					this.lifeCycleStatus |= EventBase.LifeCycleStatus.PropagateToIMGUI;
				}
				else
				{
					this.lifeCycleStatus &= ~EventBase.LifeCycleStatus.PropagateToIMGUI;
				}
			}
		}

		private bool imguiEventIsValid
		{
			get
			{
				return (this.lifeCycleStatus & EventBase.LifeCycleStatus.IMGUIEventIsValid) > EventBase.LifeCycleStatus.None;
			}
			set
			{
				if (value)
				{
					this.lifeCycleStatus |= EventBase.LifeCycleStatus.IMGUIEventIsValid;
				}
				else
				{
					this.lifeCycleStatus &= ~EventBase.LifeCycleStatus.IMGUIEventIsValid;
				}
			}
		}

		public Event imguiEvent
		{
			get
			{
				return this.imguiEventIsValid ? this.m_ImguiEvent : null;
			}
			protected set
			{
				bool flag = this.m_ImguiEvent == null;
				if (flag)
				{
					this.m_ImguiEvent = new Event();
				}
				bool flag2 = value != null;
				if (flag2)
				{
					this.m_ImguiEvent.CopyFrom(value);
					this.imguiEventIsValid = true;
					this.originalMousePosition = value.mousePosition;
				}
				else
				{
					this.imguiEventIsValid = false;
				}
			}
		}

		public Vector2 originalMousePosition { get; private set; }

		protected virtual void Init()
		{
			this.LocalInit();
		}

		private void LocalInit()
		{
			this.timestamp = 0L;
			this.triggerEventId = 0UL;
			ulong num = EventBase.s_NextEventId;
			EventBase.s_NextEventId = num + 1UL;
			this.eventId = num;
			this.propagation = EventBase.EventPropagation.None;
			this.elementTarget = null;
			this.isPropagationStopped = false;
			this.isImmediatePropagationStopped = false;
			this.propagationPhase = PropagationPhase.None;
			this.originalMousePosition = Vector2.zero;
			this.m_CurrentTarget = null;
			this.dispatch = false;
			this.propagateToIMGUI = true;
			this.dispatched = false;
			this.processed = false;
			this.processedByFocusController = false;
			this.imguiEventIsValid = false;
			this.pooled = false;
		}

		protected EventBase()
			: this(EventCategory.Default)
		{
		}

		internal EventBase(EventCategory category)
		{
			this.eventCategories = 1 << (int)category;
			this.m_ImguiEvent = null;
			this.LocalInit();
		}

		protected bool pooled
		{
			get
			{
				return (this.lifeCycleStatus & EventBase.LifeCycleStatus.Pooled) > EventBase.LifeCycleStatus.None;
			}
			set
			{
				if (value)
				{
					this.lifeCycleStatus |= EventBase.LifeCycleStatus.Pooled;
				}
				else
				{
					this.lifeCycleStatus &= ~EventBase.LifeCycleStatus.Pooled;
				}
			}
		}

		internal abstract void Acquire();

		public abstract void Dispose();

		internal void AssignTimeStamp(long time)
		{
			this.timestamp = time;
		}

		private static long s_LastTypeId;

		private static ulong s_NextEventId;

		private IEventHandler m_CurrentTarget;

		private Event m_ImguiEvent;

		[Flags]
		internal enum EventPropagation
		{
			None = 0,
			Bubbles = 1,
			TricklesDown = 2,
			SkipDisabledElements = 4,
			BubblesOrTricklesDown = 3
		}

		[Flags]
		private enum LifeCycleStatus
		{
			None = 0,
			PropagationStopped = 1,
			ImmediatePropagationStopped = 2,
			Dispatching = 4,
			Pooled = 8,
			IMGUIEventIsValid = 16,
			PropagateToIMGUI = 32,
			Dispatched = 64,
			Processed = 128,
			ProcessedByFocusController = 256
		}
	}
}
