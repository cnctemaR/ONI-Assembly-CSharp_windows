using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[StaticAccessor("GUIEvent", StaticAccessorType.DoubleColon)]
	[NativeHeader("Modules/IMGUI/Event.bindings.h")]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class Event
	{
		[NativeProperty("type", false, TargetType.Field)]
		public extern EventType rawType
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

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

		[NativeProperty("pointerType", false, TargetType.Field)]
		public extern PointerType pointerType
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("button", false, TargetType.Field)]
		public extern int button
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

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

		[NativeProperty("clickCount", false, TargetType.Field)]
		public extern int clickCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("character", false, TargetType.Field)]
		public extern char character
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("keycode", false, TargetType.Field)]
		public extern KeyCode keyCode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("displayIndex", false, TargetType.Field)]
		public extern int displayIndex
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern EventType type
		{
			[FreeFunction("GUIEvent::GetType", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[FreeFunction("GUIEvent::SetType", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

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

		[FreeFunction("GUIEvent::GetTypeForControl", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern EventType GetTypeForControl(int controlID);

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		[FreeFunction("GUIEvent::CopyFromPtr", IsThreadSafe = true, HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void CopyFromPtr(IntPtr ptr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool PopEvent([NotNull("ArgumentNullException")] Event outEvent);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetEventCount();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetNativeEvent(IntPtr ptr);

		[RequiredByNativeCode]
		internal static void Internal_MakeMasterEventCurrent(int displayIndex)
		{
			bool flag = Event.s_MasterEvent == null;
			if (flag)
			{
				Event.s_MasterEvent = new Event(displayIndex);
			}
			Event.s_MasterEvent.displayIndex = displayIndex;
			Event.s_Current = Event.s_MasterEvent;
			Event.Internal_SetNativeEvent(Event.s_MasterEvent.m_Ptr);
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetDoubleClickTime();

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
			bool flag = other == null;
			if (flag)
			{
				throw new ArgumentException("Event to copy from is null.");
			}
			this.m_Ptr = Event.Internal_Copy(other.m_Ptr);
		}

		protected override void Finalize()
		{
			try
			{
				bool flag = this.m_Ptr != IntPtr.Zero;
				if (flag)
				{
					Event.Internal_Destroy(this.m_Ptr);
					this.m_Ptr = IntPtr.Zero;
				}
			}
			finally
			{
				base.Finalize();
			}
		}

		internal static void CleanupRoots()
		{
			Event.s_Current = null;
			Event.s_MasterEvent = null;
		}

		internal void CopyFrom(Event e)
		{
			bool flag = e.m_Ptr != this.m_Ptr;
			if (flag)
			{
				this.CopyFromPtr(e.m_Ptr);
			}
		}

		[Obsolete("Use HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
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

		public bool shift
		{
			get
			{
				return (this.modifiers & EventModifiers.Shift) > EventModifiers.None;
			}
			set
			{
				bool flag = !value;
				if (flag)
				{
					this.modifiers &= ~EventModifiers.Shift;
				}
				else
				{
					this.modifiers |= EventModifiers.Shift;
				}
			}
		}

		public bool control
		{
			get
			{
				return (this.modifiers & EventModifiers.Control) > EventModifiers.None;
			}
			set
			{
				bool flag = !value;
				if (flag)
				{
					this.modifiers &= ~EventModifiers.Control;
				}
				else
				{
					this.modifiers |= EventModifiers.Control;
				}
			}
		}

		public bool alt
		{
			get
			{
				return (this.modifiers & EventModifiers.Alt) > EventModifiers.None;
			}
			set
			{
				bool flag = !value;
				if (flag)
				{
					this.modifiers &= ~EventModifiers.Alt;
				}
				else
				{
					this.modifiers |= EventModifiers.Alt;
				}
			}
		}

		public bool command
		{
			get
			{
				return (this.modifiers & EventModifiers.Command) > EventModifiers.None;
			}
			set
			{
				bool flag = !value;
				if (flag)
				{
					this.modifiers &= ~EventModifiers.Command;
				}
				else
				{
					this.modifiers |= EventModifiers.Command;
				}
			}
		}

		public bool capsLock
		{
			get
			{
				return (this.modifiers & EventModifiers.CapsLock) > EventModifiers.None;
			}
			set
			{
				bool flag = !value;
				if (flag)
				{
					this.modifiers &= ~EventModifiers.CapsLock;
				}
				else
				{
					this.modifiers |= EventModifiers.CapsLock;
				}
			}
		}

		public bool numeric
		{
			get
			{
				return (this.modifiers & EventModifiers.Numeric) > EventModifiers.None;
			}
			set
			{
				bool flag = !value;
				if (flag)
				{
					this.modifiers &= ~EventModifiers.Numeric;
				}
				else
				{
					this.modifiers |= EventModifiers.Numeric;
				}
			}
		}

		public bool functionKey
		{
			get
			{
				return (this.modifiers & EventModifiers.FunctionKey) > EventModifiers.None;
			}
		}

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

		public bool isKey
		{
			get
			{
				EventType type = this.type;
				return type == EventType.KeyDown || type == EventType.KeyUp;
			}
		}

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

		internal bool isDirectManipulationDevice
		{
			get
			{
				return this.pointerType == PointerType.Pen || this.pointerType == PointerType.Touch;
			}
		}

		public static Event KeyboardEvent(string key)
		{
			Event @event = new Event(0)
			{
				type = EventType.KeyDown
			};
			bool flag = string.IsNullOrEmpty(key);
			Event event2;
			if (flag)
			{
				event2 = @event;
			}
			else
			{
				int num = 0;
				for (;;)
				{
					bool flag2 = true;
					bool flag3 = num >= key.Length;
					if (flag3)
					{
						break;
					}
					char c = key[num];
					char c2 = c;
					switch (c2)
					{
					case '#':
						@event.modifiers |= EventModifiers.Shift;
						num++;
						break;
					case '$':
						goto IL_00CA;
					case '%':
						@event.modifiers |= EventModifiers.Command;
						num++;
						break;
					case '&':
						@event.modifiers |= EventModifiers.Alt;
						num++;
						break;
					default:
						if (c2 != '^')
						{
							goto IL_00CA;
						}
						@event.modifiers |= EventModifiers.Control;
						num++;
						break;
					}
					IL_00CE:
					if (!flag2)
					{
						break;
					}
					continue;
					IL_00CA:
					flag2 = false;
					goto IL_00CE;
				}
				string text = key.Substring(num, key.Length - num).ToLowerInvariant();
				string text2 = text;
				string text3 = text2;
				if (text3 != null)
				{
					uint num2 = <PrivateImplementationDetails>.ComputeStringHash(text3);
					if (num2 <= 2049299002U)
					{
						if (num2 <= 1035581717U)
						{
							if (num2 <= 388133425U)
							{
								if (num2 <= 306900080U)
								{
									if (num2 != 203579616U)
									{
										if (num2 != 220357235U)
										{
											if (num2 == 306900080U)
											{
												if (text3 == "left")
												{
													@event.keyCode = KeyCode.LeftArrow;
													@event.modifiers |= EventModifiers.FunctionKey;
													goto IL_0EFF;
												}
											}
										}
										else if (text3 == "f8")
										{
											@event.keyCode = KeyCode.F8;
											@event.modifiers |= EventModifiers.FunctionKey;
											goto IL_0EFF;
										}
									}
									else if (text3 == "f9")
									{
										@event.keyCode = KeyCode.F9;
										@event.modifiers |= EventModifiers.FunctionKey;
										goto IL_0EFF;
									}
								}
								else if (num2 != 337800568U)
								{
									if (num2 != 371355806U)
									{
										if (num2 == 388133425U)
										{
											if (text3 == "f2")
											{
												@event.keyCode = KeyCode.F2;
												@event.modifiers |= EventModifiers.FunctionKey;
												goto IL_0EFF;
											}
										}
									}
									else if (text3 == "f3")
									{
										@event.keyCode = KeyCode.F3;
										@event.modifiers |= EventModifiers.FunctionKey;
										goto IL_0EFF;
									}
								}
								else if (text3 == "f1")
								{
									@event.keyCode = KeyCode.F1;
									@event.modifiers |= EventModifiers.FunctionKey;
									goto IL_0EFF;
								}
							}
							else if (num2 <= 438466282U)
							{
								if (num2 != 404911044U)
								{
									if (num2 != 421688663U)
									{
										if (num2 == 438466282U)
										{
											if (text3 == "f7")
											{
												@event.keyCode = KeyCode.F7;
												@event.modifiers |= EventModifiers.FunctionKey;
												goto IL_0EFF;
											}
										}
									}
									else if (text3 == "f4")
									{
										@event.keyCode = KeyCode.F4;
										@event.modifiers |= EventModifiers.FunctionKey;
										goto IL_0EFF;
									}
								}
								else if (text3 == "f5")
								{
									@event.keyCode = KeyCode.F5;
									@event.modifiers |= EventModifiers.FunctionKey;
									goto IL_0EFF;
								}
							}
							else if (num2 != 455243901U)
							{
								if (num2 != 894689925U)
								{
									if (num2 == 1035581717U)
									{
										if (text3 == "down")
										{
											@event.keyCode = KeyCode.DownArrow;
											@event.modifiers |= EventModifiers.FunctionKey;
											goto IL_0EFF;
										}
									}
								}
								else if (text3 == "space")
								{
									@event.keyCode = KeyCode.Space;
									@event.character = ' ';
									@event.modifiers &= ~EventModifiers.FunctionKey;
									goto IL_0EFF;
								}
							}
							else if (text3 == "f6")
							{
								@event.keyCode = KeyCode.F6;
								@event.modifiers |= EventModifiers.FunctionKey;
								goto IL_0EFF;
							}
						}
						else if (num2 <= 1980614408U)
						{
							if (num2 <= 1193063839U)
							{
								if (num2 != 1113118030U)
								{
									if (num2 != 1128467232U)
									{
										if (num2 == 1193063839U)
										{
											if (text3 == "page up")
											{
												@event.keyCode = KeyCode.PageUp;
												@event.modifiers |= EventModifiers.FunctionKey;
												goto IL_0EFF;
											}
										}
									}
									else if (text3 == "up")
									{
										@event.keyCode = KeyCode.UpArrow;
										@event.modifiers |= EventModifiers.FunctionKey;
										goto IL_0EFF;
									}
								}
								else if (text3 == "[equals]")
								{
									@event.character = '=';
									@event.keyCode = KeyCode.KeypadEquals;
									goto IL_0EFF;
								}
							}
							else if (num2 != 1740784714U)
							{
								if (num2 != 1787721130U)
								{
									if (num2 == 1980614408U)
									{
										if (text3 == "[=]")
										{
											@event.character = '=';
											@event.keyCode = KeyCode.KeypadEquals;
											goto IL_0EFF;
										}
									}
								}
								else if (text3 == "end")
								{
									@event.keyCode = KeyCode.End;
									@event.modifiers |= EventModifiers.FunctionKey;
									goto IL_0EFF;
								}
							}
							else if (text3 == "delete")
							{
								@event.keyCode = KeyCode.Delete;
								@event.modifiers |= EventModifiers.FunctionKey;
								goto IL_0EFF;
							}
						}
						else if (num2 <= 1981894336U)
						{
							if (num2 != 1980761503U)
							{
								if (num2 != 1981202788U)
								{
									if (num2 == 1981894336U)
									{
										if (text3 == "[5]")
										{
											@event.character = '5';
											@event.keyCode = KeyCode.Keypad5;
											goto IL_0EFF;
										}
									}
								}
								else if (text3 == "[1]")
								{
									@event.character = '1';
									@event.keyCode = KeyCode.Keypad1;
									goto IL_0EFF;
								}
							}
							else if (text3 == "[2]")
							{
								@event.character = '2';
								@event.keyCode = KeyCode.Keypad2;
								goto IL_0EFF;
							}
						}
						else if (num2 != 2028154341U)
						{
							if (num2 != 2048857717U)
							{
								if (num2 == 2049299002U)
								{
									if (text3 == "[+]")
									{
										@event.character = '+';
										@event.keyCode = KeyCode.KeypadPlus;
										goto IL_0EFF;
									}
								}
							}
							else if (text3 == "[4]")
							{
								@event.character = '4';
								@event.keyCode = KeyCode.Keypad4;
								goto IL_0EFF;
							}
						}
						else if (text3 == "right")
						{
							@event.keyCode = KeyCode.RightArrow;
							@event.modifiers |= EventModifiers.FunctionKey;
							goto IL_0EFF;
						}
					}
					else if (num2 <= 3121933785U)
					{
						if (num2 <= 3053690476U)
						{
							if (num2 <= 2235328556U)
							{
								if (num2 != 2049990550U)
								{
									if (num2 != 2130866490U)
									{
										if (num2 == 2235328556U)
										{
											if (text3 == "backspace")
											{
												@event.keyCode = KeyCode.Backspace;
												@event.modifiers |= EventModifiers.FunctionKey;
												goto IL_0EFF;
											}
										}
									}
									else if (text3 == "page down")
									{
										@event.keyCode = KeyCode.PageDown;
										@event.modifiers |= EventModifiers.FunctionKey;
										goto IL_0EFF;
									}
								}
								else if (text3 == "[/]")
								{
									@event.character = '/';
									@event.keyCode = KeyCode.KeypadDivide;
									goto IL_0EFF;
								}
							}
							else if (num2 != 2246981567U)
							{
								if (num2 != 2566336076U)
								{
									if (num2 == 3053690476U)
									{
										if (text3 == "[9]")
										{
											@event.character = '9';
											@event.keyCode = KeyCode.Keypad9;
											goto IL_0EFF;
										}
									}
								}
								else if (text3 == "tab")
								{
									@event.keyCode = KeyCode.Tab;
									goto IL_0EFF;
								}
							}
							else if (text3 == "return")
							{
								@event.character = '\n';
								@event.keyCode = KeyCode.Return;
								@event.modifiers &= ~EventModifiers.FunctionKey;
								goto IL_0EFF;
							}
						}
						else if (num2 <= 3056941880U)
						{
							if (num2 != 3055117499U)
							{
								if (num2 != 3056397427U)
								{
									if (num2 == 3056941880U)
									{
										if (text3 == "[-]")
										{
											@event.character = '-';
											@event.keyCode = KeyCode.KeypadMinus;
											goto IL_0EFF;
										}
									}
								}
								else if (text3 == "[.]")
								{
									@event.character = '.';
									@event.keyCode = KeyCode.KeypadPeriod;
									goto IL_0EFF;
								}
							}
							else if (text3 == "[6]")
							{
								@event.character = '6';
								@event.keyCode = KeyCode.Keypad6;
								goto IL_0EFF;
							}
						}
						else if (num2 != 3120653857U)
						{
							if (num2 != 3121786690U)
							{
								if (num2 == 3121933785U)
								{
									if (text3 == "[0]")
									{
										@event.character = '0';
										@event.keyCode = KeyCode.Keypad0;
										goto IL_0EFF;
									}
								}
							}
							else if (text3 == "[3]")
							{
								@event.character = '3';
								@event.keyCode = KeyCode.Keypad3;
								goto IL_0EFF;
							}
						}
						else if (text3 == "[8]")
						{
							@event.character = '8';
							@event.keyCode = KeyCode.Keypad8;
							goto IL_0EFF;
						}
					}
					else if (num2 <= 4197582936U)
					{
						if (num2 <= 3536372366U)
						{
							if (num2 != 3122375070U)
							{
								if (num2 != 3332609576U)
								{
									if (num2 == 3536372366U)
									{
										if (text3 == "home")
										{
											@event.keyCode = KeyCode.Home;
											@event.modifiers |= EventModifiers.FunctionKey;
											goto IL_0EFF;
										}
									}
								}
								else if (text3 == "insert")
								{
									@event.keyCode = KeyCode.Insert;
									@event.modifiers |= EventModifiers.FunctionKey;
									goto IL_0EFF;
								}
							}
							else if (text3 == "[7]")
							{
								@event.character = '7';
								@event.keyCode = KeyCode.Keypad7;
								goto IL_0EFF;
							}
						}
						else if (num2 != 3906143141U)
						{
							if (num2 != 3984432914U)
							{
								if (num2 == 4197582936U)
								{
									if (text3 == "f10")
									{
										@event.keyCode = KeyCode.F10;
										@event.modifiers |= EventModifiers.FunctionKey;
										goto IL_0EFF;
									}
								}
							}
							else if (text3 == "[esc]")
							{
								@event.keyCode = KeyCode.Escape;
								goto IL_0EFF;
							}
						}
						else if (text3 == "pgup")
						{
							@event.keyCode = KeyCode.PageDown;
							@event.modifiers |= EventModifiers.FunctionKey;
							goto IL_0EFF;
						}
					}
					else if (num2 <= 4227375619U)
					{
						if (num2 != 4213014532U)
						{
							if (num2 != 4214360555U)
							{
								if (num2 == 4227375619U)
								{
									if (text3 == "[enter]")
									{
										@event.character = '\n';
										@event.keyCode = KeyCode.KeypadEnter;
										goto IL_0EFF;
									}
								}
							}
							else if (text3 == "f11")
							{
								@event.keyCode = KeyCode.F11;
								@event.modifiers |= EventModifiers.FunctionKey;
								goto IL_0EFF;
							}
						}
						else if (text3 == "pgdown")
						{
							@event.keyCode = KeyCode.PageUp;
							@event.modifiers |= EventModifiers.FunctionKey;
							goto IL_0EFF;
						}
					}
					else if (num2 <= 4247915793U)
					{
						if (num2 != 4231138174U)
						{
							if (num2 == 4247915793U)
							{
								if (text3 == "f13")
								{
									@event.keyCode = KeyCode.F13;
									@event.modifiers |= EventModifiers.FunctionKey;
									goto IL_0EFF;
								}
							}
						}
						else if (text3 == "f12")
						{
							@event.keyCode = KeyCode.F12;
							@event.modifiers |= EventModifiers.FunctionKey;
							goto IL_0EFF;
						}
					}
					else if (num2 != 4264693412U)
					{
						if (num2 == 4281471031U)
						{
							if (text3 == "f15")
							{
								@event.keyCode = KeyCode.F15;
								@event.modifiers |= EventModifiers.FunctionKey;
								goto IL_0EFF;
							}
						}
					}
					else if (text3 == "f14")
					{
						@event.keyCode = KeyCode.F14;
						@event.modifiers |= EventModifiers.FunctionKey;
						goto IL_0EFF;
					}
				}
				bool flag4 = text.Length != 1;
				if (flag4)
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
					bool flag5 = @event.modifiers > EventModifiers.None;
					if (flag5)
					{
						@event.character = '\0';
					}
				}
				IL_0EFF:
				event2 = @event;
			}
			return event2;
		}

		public override int GetHashCode()
		{
			int num = 1;
			bool isKey = this.isKey;
			if (isKey)
			{
				num = (int)((ushort)this.keyCode);
			}
			bool isMouse = this.isMouse;
			if (isMouse)
			{
				num = this.mousePosition.GetHashCode();
			}
			return (num * 37) | (int)this.modifiers;
		}

		public override bool Equals(object obj)
		{
			bool flag = obj == null;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = this == obj;
				if (flag3)
				{
					flag2 = true;
				}
				else
				{
					bool flag4 = obj.GetType() != base.GetType();
					if (flag4)
					{
						flag2 = false;
					}
					else
					{
						Event @event = (Event)obj;
						bool flag5 = this.type != @event.type || (this.modifiers & ~EventModifiers.CapsLock) != (@event.modifiers & ~EventModifiers.CapsLock);
						if (flag5)
						{
							flag2 = false;
						}
						else
						{
							bool isKey = this.isKey;
							if (isKey)
							{
								flag2 = this.keyCode == @event.keyCode;
							}
							else
							{
								bool isMouse = this.isMouse;
								flag2 = isMouse && this.mousePosition == @event.mousePosition;
							}
						}
					}
				}
			}
			return flag2;
		}

		public override string ToString()
		{
			bool isKey = this.isKey;
			string text;
			if (isKey)
			{
				bool flag = this.character == '\0';
				if (flag)
				{
					text = UnityString.Format("Event:{0}   Character:\\0   Modifiers:{1}   KeyCode:{2}", new object[] { this.type, this.modifiers, this.keyCode });
				}
				else
				{
					text = string.Concat(new string[]
					{
						"Event:",
						this.type.ToString(),
						"   Character:",
						((int)this.character).ToString(),
						"   Modifiers:",
						this.modifiers.ToString(),
						"   KeyCode:",
						this.keyCode.ToString()
					});
				}
			}
			else
			{
				bool isMouse = this.isMouse;
				if (isMouse)
				{
					text = UnityString.Format("Event: {0}   Position: {1} Modifiers: {2}", new object[] { this.type, this.mousePosition, this.modifiers });
				}
				else
				{
					bool flag2 = this.type == EventType.ExecuteCommand || this.type == EventType.ValidateCommand;
					if (flag2)
					{
						text = UnityString.Format("Event: {0}  \"{1}\"", new object[] { this.type, this.commandName });
					}
					else
					{
						text = this.type.ToString() ?? "";
					}
				}
			}
			return text;
		}

		public void Use()
		{
			bool flag = this.type == EventType.Repaint || this.type == EventType.Layout;
			if (flag)
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
