using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>A UnityGUI event.</para>
	/// </summary>
	[NativeHeader("Modules/IMGUI/Event.bindings.h")]
	[StaticAccessor("GUIEvent", StaticAccessorType.DoubleColon)]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class Event
	{
		public Event()
		{
			this.m_Ptr = Event.Internal_Create(0);
		}

		public Event(int displayIndex)
		{
			this.m_Ptr = Event.Internal_Create(displayIndex);
		}

		public Event(Event other)
		{
			if (other == null)
			{
				throw new ArgumentException("Event to copy from is null.");
			}
			this.m_Ptr = Event.Internal_Copy(other.m_Ptr);
		}

		[NativeProperty("type", false, TargetType.Field)]
		public extern EventType rawType
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The mouse position.</para>
		/// </summary>
		[NativeProperty("mousePosition", false, TargetType.Field)]
		public Vector2 mousePosition
		{
			get
			{
				Vector2 vector;
				this.get_mousePosition_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_mousePosition_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>The relative movement of the mouse compared to last event.</para>
		/// </summary>
		[NativeProperty("delta", false, TargetType.Field)]
		public Vector2 delta
		{
			get
			{
				Vector2 vector;
				this.get_delta_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_delta_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Which mouse button was pressed.</para>
		/// </summary>
		[NativeProperty("button", false, TargetType.Field)]
		public extern int button
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Which modifier keys are held down.</para>
		/// </summary>
		[NativeProperty("modifiers", false, TargetType.Field)]
		public extern EventModifiers modifiers
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("pressure", false, TargetType.Field)]
		public extern float pressure
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>How many consecutive mouse clicks have we received.</para>
		/// </summary>
		[NativeProperty("clickCount", false, TargetType.Field)]
		public extern int clickCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The character typed.</para>
		/// </summary>
		[NativeProperty("character", false, TargetType.Field)]
		public extern char character
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The raw key code for keyboard events.</para>
		/// </summary>
		[NativeProperty("keycode", false, TargetType.Field)]
		public extern KeyCode keyCode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Index of display that the event belongs to.</para>
		/// </summary>
		[NativeProperty("displayIndex", false, TargetType.Field)]
		public extern int displayIndex
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The type of event.</para>
		/// </summary>
		public extern EventType type
		{
			[FreeFunction("GUIEvent::GetType", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[FreeFunction("GUIEvent::SetType", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The name of an ExecuteCommand or ValidateCommand Event.</para>
		/// </summary>
		public extern string commandName
		{
			[FreeFunction("GUIEvent::GetCommandName", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[FreeFunction("GUIEvent::SetCommandName", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeMethod("Use")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_Use();

		[FreeFunction("GUIEvent::Internal_Create", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Internal_Create(int displayIndex);

		[FreeFunction("GUIEvent::Internal_Destroy", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Destroy(IntPtr ptr);

		[FreeFunction("GUIEvent::Internal_Copy", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Internal_Copy(IntPtr otherPtr);

		/// <summary>
		///   <para>Get a filtered event type for a given control ID.</para>
		/// </summary>
		/// <param name="controlID">The ID of the control you are querying from.</param>
		[FreeFunction("GUIEvent::GetTypeForControl", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern EventType GetTypeForControl(int controlID);

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		[FreeFunction("GUIEvent::CopyFromPtr", IsThreadSafe = true, HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void CopyFromPtr(IntPtr ptr);

		/// <summary>
		///   <para>Get the next queued [Event] from the event system.</para>
		/// </summary>
		/// <param name="outEvent">Next Event.</param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool PopEvent(Event outEvent);

		/// <summary>
		///   <para>Returns the current number of events that are stored in the event queue.</para>
		/// </summary>
		/// <returns>
		///   <para>Current number of events currently in the event queue.</para>
		/// </returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetEventCount();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetNativeEvent(IntPtr ptr);

		[RequiredByNativeCode]
		internal static void Internal_MakeMasterEventCurrent(int displayIndex)
		{
			if (Event.s_MasterEvent == null)
			{
				Event.s_MasterEvent = new Event(displayIndex);
			}
			Event.s_MasterEvent.displayIndex = displayIndex;
			Event.s_Current = Event.s_MasterEvent;
			Event.Internal_SetNativeEvent(Event.s_MasterEvent.m_Ptr);
		}

		~Event()
		{
			if (this.m_Ptr != IntPtr.Zero)
			{
				Event.Internal_Destroy(this.m_Ptr);
				this.m_Ptr = IntPtr.Zero;
			}
		}

		internal static void CleanupRoots()
		{
			Event.s_Current = null;
			Event.s_MasterEvent = null;
		}

		[Obsolete("Use HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);", true)]
		public Ray mouseRay
		{
			get
			{
				return new Ray(Vector3.up, Vector3.up);
			}
			set
			{
			}
		}

		/// <summary>
		///   <para>Is Shift held down? (Read Only)</para>
		/// </summary>
		public bool shift
		{
			get
			{
				return (this.modifiers & EventModifiers.Shift) != EventModifiers.None;
			}
			set
			{
				if (!value)
				{
					this.modifiers &= ~EventModifiers.Shift;
				}
				else
				{
					this.modifiers |= EventModifiers.Shift;
				}
			}
		}

		/// <summary>
		///   <para>Is Control key held down? (Read Only)</para>
		/// </summary>
		public bool control
		{
			get
			{
				return (this.modifiers & EventModifiers.Control) != EventModifiers.None;
			}
			set
			{
				if (!value)
				{
					this.modifiers &= ~EventModifiers.Control;
				}
				else
				{
					this.modifiers |= EventModifiers.Control;
				}
			}
		}

		/// <summary>
		///   <para>Is Alt/Option key held down? (Read Only)</para>
		/// </summary>
		public bool alt
		{
			get
			{
				return (this.modifiers & EventModifiers.Alt) != EventModifiers.None;
			}
			set
			{
				if (!value)
				{
					this.modifiers &= ~EventModifiers.Alt;
				}
				else
				{
					this.modifiers |= EventModifiers.Alt;
				}
			}
		}

		/// <summary>
		///   <para>Is Command/Windows key held down? (Read Only)</para>
		/// </summary>
		public bool command
		{
			get
			{
				return (this.modifiers & EventModifiers.Command) != EventModifiers.None;
			}
			set
			{
				if (!value)
				{
					this.modifiers &= ~EventModifiers.Command;
				}
				else
				{
					this.modifiers |= EventModifiers.Command;
				}
			}
		}

		/// <summary>
		///   <para>Is Caps Lock on? (Read Only)</para>
		/// </summary>
		public bool capsLock
		{
			get
			{
				return (this.modifiers & EventModifiers.CapsLock) != EventModifiers.None;
			}
			set
			{
				if (!value)
				{
					this.modifiers &= ~EventModifiers.CapsLock;
				}
				else
				{
					this.modifiers |= EventModifiers.CapsLock;
				}
			}
		}

		/// <summary>
		///   <para>Is the current keypress on the numeric keyboard? (Read Only)</para>
		/// </summary>
		public bool numeric
		{
			get
			{
				return (this.modifiers & EventModifiers.Numeric) != EventModifiers.None;
			}
			set
			{
				if (!value)
				{
					this.modifiers &= ~EventModifiers.Numeric;
				}
				else
				{
					this.modifiers |= EventModifiers.Numeric;
				}
			}
		}

		/// <summary>
		///   <para>Is the current keypress a function key? (Read Only)</para>
		/// </summary>
		public bool functionKey
		{
			[CompilerGenerated]
			get
			{
				return (this.modifiers & EventModifiers.FunctionKey) != EventModifiers.None;
			}
		}

		/// <summary>
		///   <para>The current event that's being processed right now.</para>
		/// </summary>
		public static Event current
		{
			get
			{
				return Event.s_Current;
			}
			set
			{
				Event.s_Current = value ?? Event.s_MasterEvent;
				Event.Internal_SetNativeEvent(Event.s_Current.m_Ptr);
			}
		}

		/// <summary>
		///   <para>Is this event a keyboard event? (Read Only)</para>
		/// </summary>
		public bool isKey
		{
			get
			{
				EventType type = this.type;
				return type == EventType.KeyDown || type == EventType.KeyUp;
			}
		}

		/// <summary>
		///   <para>Is this event a mouse event? (Read Only)</para>
		/// </summary>
		public bool isMouse
		{
			get
			{
				EventType type = this.type;
				return type == EventType.MouseMove || type == EventType.MouseDown || type == EventType.MouseUp || type == EventType.MouseDrag || type == EventType.ContextClick || type == EventType.MouseEnterWindow || type == EventType.MouseLeaveWindow;
			}
		}

		public bool isScrollWheel
		{
			get
			{
				EventType type = this.type;
				return type == EventType.ScrollWheel;
			}
		}

		/// <summary>
		///   <para>Create a keyboard event.</para>
		/// </summary>
		/// <param name="key"></param>
		public static Event KeyboardEvent(string key)
		{
			Event @event = new Event(0)
			{
				type = EventType.KeyDown
			};
			Event event2;
			if (string.IsNullOrEmpty(key))
			{
				event2 = @event;
			}
			else
			{
				int num = 0;
				bool flag;
				do
				{
					flag = true;
					if (num >= key.Length)
					{
						break;
					}
					char c = key[num];
					switch (c)
					{
					case '#':
						@event.modifiers |= EventModifiers.Shift;
						num++;
						break;
					default:
						if (c != '^')
						{
							flag = false;
						}
						else
						{
							@event.modifiers |= EventModifiers.Control;
							num++;
						}
						break;
					case '%':
						@event.modifiers |= EventModifiers.Command;
						num++;
						break;
					case '&':
						@event.modifiers |= EventModifiers.Alt;
						num++;
						break;
					}
				}
				while (flag);
				string text = key.Substring(num, key.Length - num).ToLower();
				switch (text)
				{
				case "[0]":
					@event.character = '0';
					@event.keyCode = KeyCode.Keypad0;
					goto IL_0A8A;
				case "[1]":
					@event.character = '1';
					@event.keyCode = KeyCode.Keypad1;
					goto IL_0A8A;
				case "[2]":
					@event.character = '2';
					@event.keyCode = KeyCode.Keypad2;
					goto IL_0A8A;
				case "[3]":
					@event.character = '3';
					@event.keyCode = KeyCode.Keypad3;
					goto IL_0A8A;
				case "[4]":
					@event.character = '4';
					@event.keyCode = KeyCode.Keypad4;
					goto IL_0A8A;
				case "[5]":
					@event.character = '5';
					@event.keyCode = KeyCode.Keypad5;
					goto IL_0A8A;
				case "[6]":
					@event.character = '6';
					@event.keyCode = KeyCode.Keypad6;
					goto IL_0A8A;
				case "[7]":
					@event.character = '7';
					@event.keyCode = KeyCode.Keypad7;
					goto IL_0A8A;
				case "[8]":
					@event.character = '8';
					@event.keyCode = KeyCode.Keypad8;
					goto IL_0A8A;
				case "[9]":
					@event.character = '9';
					@event.keyCode = KeyCode.Keypad9;
					goto IL_0A8A;
				case "[.]":
					@event.character = '.';
					@event.keyCode = KeyCode.KeypadPeriod;
					goto IL_0A8A;
				case "[/]":
					@event.character = '/';
					@event.keyCode = KeyCode.KeypadDivide;
					goto IL_0A8A;
				case "[-]":
					@event.character = '-';
					@event.keyCode = KeyCode.KeypadMinus;
					goto IL_0A8A;
				case "[+]":
					@event.character = '+';
					@event.keyCode = KeyCode.KeypadPlus;
					goto IL_0A8A;
				case "[=]":
					@event.character = '=';
					@event.keyCode = KeyCode.KeypadEquals;
					goto IL_0A8A;
				case "[equals]":
					@event.character = '=';
					@event.keyCode = KeyCode.KeypadEquals;
					goto IL_0A8A;
				case "[enter]":
					@event.character = '\n';
					@event.keyCode = KeyCode.KeypadEnter;
					goto IL_0A8A;
				case "up":
					@event.keyCode = KeyCode.UpArrow;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_0A8A;
				case "down":
					@event.keyCode = KeyCode.DownArrow;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_0A8A;
				case "left":
					@event.keyCode = KeyCode.LeftArrow;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_0A8A;
				case "right":
					@event.keyCode = KeyCode.RightArrow;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_0A8A;
				case "insert":
					@event.keyCode = KeyCode.Insert;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_0A8A;
				case "home":
					@event.keyCode = KeyCode.Home;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_0A8A;
				case "end":
					@event.keyCode = KeyCode.End;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_0A8A;
				case "pgup":
					@event.keyCode = KeyCode.PageDown;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_0A8A;
				case "page up":
					@event.keyCode = KeyCode.PageUp;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_0A8A;
				case "pgdown":
					@event.keyCode = KeyCode.PageUp;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_0A8A;
				case "page down":
					@event.keyCode = KeyCode.PageDown;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_0A8A;
				case "backspace":
					@event.keyCode = KeyCode.Backspace;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_0A8A;
				case "delete":
					@event.keyCode = KeyCode.Delete;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_0A8A;
				case "tab":
					@event.keyCode = KeyCode.Tab;
					goto IL_0A8A;
				case "f1":
					@event.keyCode = KeyCode.F1;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_0A8A;
				case "f2":
					@event.keyCode = KeyCode.F2;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_0A8A;
				case "f3":
					@event.keyCode = KeyCode.F3;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_0A8A;
				case "f4":
					@event.keyCode = KeyCode.F4;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_0A8A;
				case "f5":
					@event.keyCode = KeyCode.F5;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_0A8A;
				case "f6":
					@event.keyCode = KeyCode.F6;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_0A8A;
				case "f7":
					@event.keyCode = KeyCode.F7;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_0A8A;
				case "f8":
					@event.keyCode = KeyCode.F8;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_0A8A;
				case "f9":
					@event.keyCode = KeyCode.F9;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_0A8A;
				case "f10":
					@event.keyCode = KeyCode.F10;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_0A8A;
				case "f11":
					@event.keyCode = KeyCode.F11;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_0A8A;
				case "f12":
					@event.keyCode = KeyCode.F12;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_0A8A;
				case "f13":
					@event.keyCode = KeyCode.F13;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_0A8A;
				case "f14":
					@event.keyCode = KeyCode.F14;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_0A8A;
				case "f15":
					@event.keyCode = KeyCode.F15;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_0A8A;
				case "[esc]":
					@event.keyCode = KeyCode.Escape;
					goto IL_0A8A;
				case "return":
					@event.character = '\n';
					@event.keyCode = KeyCode.Return;
					@event.modifiers &= ~EventModifiers.FunctionKey;
					goto IL_0A8A;
				case "space":
					@event.keyCode = KeyCode.Space;
					@event.character = ' ';
					@event.modifiers &= ~EventModifiers.FunctionKey;
					goto IL_0A8A;
				}
				if (text.Length != 1)
				{
					try
					{
						@event.keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), text, true);
					}
					catch (ArgumentException)
					{
						Debug.LogError(UnityString.Format("Unable to find key name that matches '{0}'", new object[] { text }));
					}
				}
				else
				{
					@event.character = text.ToLower()[0];
					@event.keyCode = (KeyCode)@event.character;
					if (@event.modifiers != EventModifiers.None)
					{
						@event.character = '\0';
					}
				}
				IL_0A8A:
				event2 = @event;
			}
			return event2;
		}

		public override int GetHashCode()
		{
			int num = 1;
			if (this.isKey)
			{
				num = (int)((ushort)this.keyCode);
			}
			if (this.isMouse)
			{
				num = this.mousePosition.GetHashCode();
			}
			return (num * 37) | (int)this.modifiers;
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj == null)
			{
				flag = false;
			}
			else if (object.ReferenceEquals(this, obj))
			{
				flag = true;
			}
			else if (obj.GetType() != base.GetType())
			{
				flag = false;
			}
			else
			{
				Event @event = (Event)obj;
				if (this.type != @event.type || (this.modifiers & ~EventModifiers.CapsLock) != (@event.modifiers & ~EventModifiers.CapsLock))
				{
					flag = false;
				}
				else if (this.isKey)
				{
					flag = this.keyCode == @event.keyCode;
				}
				else
				{
					flag = this.isMouse && this.mousePosition == @event.mousePosition;
				}
			}
			return flag;
		}

		public override string ToString()
		{
			string text;
			if (this.isKey)
			{
				if (this.character == '\0')
				{
					text = UnityString.Format("Event:{0}   Character:\\0   Modifiers:{1}   KeyCode:{2}", new object[] { this.type, this.modifiers, this.keyCode });
				}
				else
				{
					text = string.Concat(new object[]
					{
						"Event:",
						this.type,
						"   Character:",
						(int)this.character,
						"   Modifiers:",
						this.modifiers,
						"   KeyCode:",
						this.keyCode
					});
				}
			}
			else if (this.isMouse)
			{
				text = UnityString.Format("Event: {0}   Position: {1} Modifiers: {2}", new object[] { this.type, this.mousePosition, this.modifiers });
			}
			else if (this.type == EventType.ExecuteCommand || this.type == EventType.ValidateCommand)
			{
				text = UnityString.Format("Event: {0}  \"{1}\"", new object[] { this.type, this.commandName });
			}
			else
			{
				text = "" + this.type;
			}
			return text;
		}

		/// <summary>
		///   <para>Use this event.</para>
		/// </summary>
		public void Use()
		{
			if (this.type == EventType.Repaint || this.type == EventType.Layout)
			{
				Debug.LogWarning(UnityString.Format("Event.Use() should not be called for events of type {0}", new object[] { this.type }));
			}
			this.Internal_Use();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_mousePosition_Injected(out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_mousePosition_Injected(ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_delta_Injected(out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_delta_Injected(ref Vector2 value);

		[NonSerialized]
		internal IntPtr m_Ptr;

		private static Event s_Current;

		private static Event s_MasterEvent;
	}
}
