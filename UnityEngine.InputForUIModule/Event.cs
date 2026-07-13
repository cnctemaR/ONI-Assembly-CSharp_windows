using System;
using System.Runtime.InteropServices;
using Unity.IntegerTime;
using UnityEngine.Bindings;

namespace UnityEngine.InputForUI
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	[StructLayout(LayoutKind.Explicit)]
	internal struct Event : IEventProperties
	{
		internal static int CompareType(Event a, Event b)
		{
			bool flag = a.type == Event.Type.PointerEvent && b.type == Event.Type.PointerEvent;
			int num;
			if (flag)
			{
				int eventSource = (int)a.eventSource;
				num = ((int)b.eventSource).CompareTo(eventSource);
			}
			else
			{
				int type = (int)a.type;
				int type2 = (int)b.type;
				num = type.CompareTo(type2);
			}
			return num;
		}

		public Event.Type type
		{
			get
			{
				return this._type;
			}
		}

		private IEventProperties asObject
		{
			get
			{
				return this.Map<IEventProperties, Event.MapAsObject>();
			}
		}

		public DiscreteTime timestamp
		{
			get
			{
				return this.Map<DiscreteTime, Event.MapAsTimestamp>();
			}
		}

		public EventSource eventSource
		{
			get
			{
				return this.Map<EventSource, Event.MapAsEventSource>();
			}
		}

		public uint playerId
		{
			get
			{
				return this.Map<uint, Event.MapAsPlayerId>();
			}
		}

		public EventModifiers eventModifiers
		{
			get
			{
				return this.Map<EventModifiers, Event.MapAsEventModifiers>();
			}
		}

		private void Ensure(Event.Type t)
		{
			Debug.Assert(this.type == t);
		}

		public override string ToString()
		{
			string text = this.eventModifiers.ToString();
			bool flag = !string.IsNullOrEmpty(text);
			if (flag)
			{
				text = " ev:" + text;
			}
			return (this.type == Event.Type.Invalid) ? "Invalid" : string.Format("{0}{1} src:{2}", this.asObject, text, this.eventSource.ToString());
		}

		public static Event From(KeyEvent keyEvent)
		{
			return new Event
			{
				_type = Event.Type.KeyEvent,
				_keyEvent = keyEvent
			};
		}

		public KeyEvent asKeyEvent
		{
			get
			{
				this.Ensure(Event.Type.KeyEvent);
				return this._keyEvent;
			}
		}

		public static Event From(PointerEvent pointerEvent)
		{
			return new Event
			{
				_type = Event.Type.PointerEvent,
				_pointerEvent = pointerEvent
			};
		}

		public PointerEvent asPointerEvent
		{
			get
			{
				this.Ensure(Event.Type.PointerEvent);
				return this._pointerEvent;
			}
		}

		public static Event From(TextInputEvent textInputEvent)
		{
			return new Event
			{
				_type = Event.Type.TextInputEvent,
				_textInputEvent = textInputEvent
			};
		}

		public TextInputEvent asTextInputEvent
		{
			get
			{
				this.Ensure(Event.Type.TextInputEvent);
				return this._textInputEvent;
			}
		}

		public static Event From(IMECompositionEvent imeCompositionEvent)
		{
			return new Event
			{
				_type = Event.Type.IMECompositionEvent,
				_managedEvent = imeCompositionEvent
			};
		}

		public IMECompositionEvent asIMECompositionEvent
		{
			get
			{
				this.Ensure(Event.Type.IMECompositionEvent);
				return (IMECompositionEvent)this._managedEvent;
			}
		}

		public static Event From(CommandEvent commandEvent)
		{
			return new Event
			{
				_type = Event.Type.CommandEvent,
				_commandEvent = commandEvent
			};
		}

		public CommandEvent asCommandEvent
		{
			get
			{
				this.Ensure(Event.Type.CommandEvent);
				return this._commandEvent;
			}
		}

		public static Event From(NavigationEvent navigationEvent)
		{
			return new Event
			{
				_type = Event.Type.NavigationEvent,
				_navigationEvent = navigationEvent
			};
		}

		public NavigationEvent asNavigationEvent
		{
			get
			{
				this.Ensure(Event.Type.NavigationEvent);
				return this._navigationEvent;
			}
		}

		private TOutputType Map<TOutputType, TMapType>(TMapType fn) where TMapType : Event.IMapFn<TOutputType>
		{
			TOutputType toutputType;
			switch (this.type)
			{
			case Event.Type.Invalid:
				toutputType = default(TOutputType);
				break;
			case Event.Type.KeyEvent:
				toutputType = fn.Map<KeyEvent>(ref this._keyEvent);
				break;
			case Event.Type.PointerEvent:
				toutputType = fn.Map<PointerEvent>(ref this._pointerEvent);
				break;
			case Event.Type.TextInputEvent:
				toutputType = fn.Map<TextInputEvent>(ref this._textInputEvent);
				break;
			case Event.Type.IMECompositionEvent:
			{
				IMECompositionEvent imecompositionEvent = (IMECompositionEvent)this._managedEvent;
				toutputType = fn.Map<IMECompositionEvent>(ref imecompositionEvent);
				break;
			}
			case Event.Type.CommandEvent:
				toutputType = fn.Map<CommandEvent>(ref this._commandEvent);
				break;
			case Event.Type.NavigationEvent:
				toutputType = fn.Map<NavigationEvent>(ref this._navigationEvent);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			return toutputType;
		}

		private TOutputType Map<TOutputType, TMapType>() where TMapType : Event.IMapFn<TOutputType>, new()
		{
			return this.Map<TOutputType, TMapType>(new TMapType());
		}

		public static Event.Type[] TypesWithState = new Event.Type[]
		{
			Event.Type.KeyEvent,
			Event.Type.PointerEvent,
			Event.Type.IMECompositionEvent
		};

		private const int kManagedOffset = 8;

		private const int kUnmanagedOffset = 16;

		[FieldOffset(0)]
		private Event.Type _type;

		[FieldOffset(8)]
		private object _managedEvent;

		[FieldOffset(16)]
		private KeyEvent _keyEvent;

		[FieldOffset(16)]
		private PointerEvent _pointerEvent;

		[FieldOffset(16)]
		private TextInputEvent _textInputEvent;

		[FieldOffset(16)]
		private CommandEvent _commandEvent;

		[FieldOffset(16)]
		private NavigationEvent _navigationEvent;

		public enum Type
		{
			Invalid,
			KeyEvent,
			PointerEvent,
			TextInputEvent,
			IMECompositionEvent,
			CommandEvent,
			NavigationEvent
		}

		private interface IMapFn<TOutputType>
		{
			TOutputType Map<TEventType>(ref TEventType ev) where TEventType : IEventProperties;
		}

		private struct MapAsObject : Event.IMapFn<IEventProperties>
		{
			public IEventProperties Map<TEventType>(ref TEventType ev) where TEventType : IEventProperties
			{
				return ev;
			}
		}

		private struct MapAsTimestamp : Event.IMapFn<DiscreteTime>
		{
			public DiscreteTime Map<TEventType>(ref TEventType ev) where TEventType : IEventProperties
			{
				return ev.timestamp;
			}
		}

		private struct MapAsEventSource : Event.IMapFn<EventSource>
		{
			public EventSource Map<TEventType>(ref TEventType ev) where TEventType : IEventProperties
			{
				return ev.eventSource;
			}
		}

		private struct MapAsPlayerId : Event.IMapFn<uint>
		{
			public uint Map<TEventType>(ref TEventType ev) where TEventType : IEventProperties
			{
				return ev.playerId;
			}
		}

		private struct MapAsEventModifiers : Event.IMapFn<EventModifiers>
		{
			public EventModifiers Map<TEventType>(ref TEventType ev) where TEventType : IEventProperties
			{
				return ev.eventModifiers;
			}
		}
	}
}
