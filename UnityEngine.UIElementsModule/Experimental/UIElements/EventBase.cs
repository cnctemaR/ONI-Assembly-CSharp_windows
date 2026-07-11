using System;

namespace UnityEngine.Experimental.UIElements
{
	public abstract class EventBase : IDisposable
	{
		protected EventBase()
		{
			this.m_ImguiEvent = null;
			this.Init();
		}

		protected static long RegisterEventType()
		{
			return EventBase.s_LastTypeId += 1L;
		}

		public abstract long GetEventTypeId();

		public long timestamp { get; private set; }

		protected EventBase.EventFlags flags { get; set; }

		private EventBase.LifeCycleFlags lifeCycleFlags { get; set; }

		protected internal virtual void PreDispatch()
		{
		}

		protected internal virtual void PostDispatch()
		{
		}

		public bool bubbles
		{
			get
			{
				return (this.flags & EventBase.EventFlags.Bubbles) != EventBase.EventFlags.None;
			}
		}

		[Obsolete("Use tricklesDown instead of capturable.")]
		public bool capturable
		{
			get
			{
				return this.tricklesDown;
			}
		}

		public bool tricklesDown
		{
			get
			{
				return (this.flags & EventBase.EventFlags.TricklesDown) != EventBase.EventFlags.None;
			}
		}

		public IEventHandler target { get; set; }

		internal IEventHandler skipElement { get; set; }

		public bool isPropagationStopped
		{
			get
			{
				return (this.lifeCycleFlags & EventBase.LifeCycleFlags.PropagationStopped) != EventBase.LifeCycleFlags.None;
			}
			private set
			{
				if (value)
				{
					this.lifeCycleFlags |= EventBase.LifeCycleFlags.PropagationStopped;
				}
				else
				{
					this.lifeCycleFlags &= ~EventBase.LifeCycleFlags.PropagationStopped;
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
				return (this.lifeCycleFlags & EventBase.LifeCycleFlags.ImmediatePropagationStopped) != EventBase.LifeCycleFlags.None;
			}
			private set
			{
				if (value)
				{
					this.lifeCycleFlags |= EventBase.LifeCycleFlags.ImmediatePropagationStopped;
				}
				else
				{
					this.lifeCycleFlags &= ~EventBase.LifeCycleFlags.ImmediatePropagationStopped;
				}
			}
		}

		public void StopImmediatePropagation()
		{
			this.isPropagationStopped = true;
			this.isImmediatePropagationStopped = true;
		}

		public bool isDefaultPrevented
		{
			get
			{
				return (this.lifeCycleFlags & EventBase.LifeCycleFlags.DefaultPrevented) != EventBase.LifeCycleFlags.None;
			}
			private set
			{
				if (value)
				{
					this.lifeCycleFlags |= EventBase.LifeCycleFlags.DefaultPrevented;
				}
				else
				{
					this.lifeCycleFlags &= ~EventBase.LifeCycleFlags.DefaultPrevented;
				}
			}
		}

		public void PreventDefault()
		{
			if ((this.flags & EventBase.EventFlags.Cancellable) == EventBase.EventFlags.Cancellable)
			{
				this.isDefaultPrevented = true;
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
				if (this.imguiEvent != null)
				{
					VisualElement visualElement = this.currentTarget as VisualElement;
					if (visualElement != null)
					{
						this.imguiEvent.mousePosition = visualElement.WorldToLocal(this.originalMousePosition);
					}
				}
			}
		}

		public bool dispatch
		{
			get
			{
				return (this.lifeCycleFlags & EventBase.LifeCycleFlags.Dispatching) != EventBase.LifeCycleFlags.None;
			}
			internal set
			{
				if (value)
				{
					this.lifeCycleFlags |= EventBase.LifeCycleFlags.Dispatching;
					this.dispatched = true;
				}
				else
				{
					this.lifeCycleFlags &= ~EventBase.LifeCycleFlags.Dispatching;
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
				return (this.lifeCycleFlags & EventBase.LifeCycleFlags.Dispatched) != EventBase.LifeCycleFlags.None;
			}
			set
			{
				if (value)
				{
					this.lifeCycleFlags |= EventBase.LifeCycleFlags.Dispatched;
				}
				else
				{
					this.lifeCycleFlags &= ~EventBase.LifeCycleFlags.Dispatched;
				}
			}
		}

		private bool imguiEventIsValid
		{
			get
			{
				return (this.lifeCycleFlags & EventBase.LifeCycleFlags.IMGUIEventIsValid) != EventBase.LifeCycleFlags.None;
			}
			set
			{
				if (value)
				{
					this.lifeCycleFlags |= EventBase.LifeCycleFlags.IMGUIEventIsValid;
				}
				else
				{
					this.lifeCycleFlags &= ~EventBase.LifeCycleFlags.IMGUIEventIsValid;
				}
			}
		}

		public Event imguiEvent
		{
			get
			{
				return (!this.imguiEventIsValid) ? null : this.m_ImguiEvent;
			}
			protected set
			{
				if (this.m_ImguiEvent == null)
				{
					this.m_ImguiEvent = new Event();
				}
				if (value != null)
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
			this.timestamp = (long)(Time.realtimeSinceStartup * 1000f);
			this.flags = EventBase.EventFlags.None;
			this.target = null;
			this.skipElement = null;
			this.isPropagationStopped = false;
			this.isImmediatePropagationStopped = false;
			this.isDefaultPrevented = false;
			this.propagationPhase = PropagationPhase.None;
			this.originalMousePosition = Vector2.zero;
			this.m_CurrentTarget = null;
			this.dispatch = false;
			this.dispatched = false;
			this.imguiEventIsValid = false;
			this.pooled = false;
		}

		protected bool pooled
		{
			get
			{
				return (this.lifeCycleFlags & EventBase.LifeCycleFlags.Pooled) != EventBase.LifeCycleFlags.None;
			}
			set
			{
				if (value)
				{
					this.lifeCycleFlags |= EventBase.LifeCycleFlags.Pooled;
				}
				else
				{
					this.lifeCycleFlags &= ~EventBase.LifeCycleFlags.Pooled;
				}
			}
		}

		internal abstract void Acquire();

		public abstract void Dispose();

		private static long s_LastTypeId = 0L;

		protected IEventHandler m_CurrentTarget;

		private Event m_ImguiEvent;

		[Flags]
		protected internal enum EventFlags
		{
			None = 0,
			Bubbles = 1,
			TricklesDown = 2,
			[Obsolete("Use TrickesDown instead of Capturable")]
			Capturable = 2,
			Cancellable = 4
		}

		[Flags]
		private enum LifeCycleFlags
		{
			None = 0,
			PropagationStopped = 1,
			ImmediatePropagationStopped = 2,
			DefaultPrevented = 4,
			Dispatching = 8,
			Pooled = 16,
			IMGUIEventIsValid = 32,
			Dispatched = 512
		}
	}
}
