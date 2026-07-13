using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.InputForUI
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal struct EventModifiers
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsPressed(EventModifiers.Modifiers mod)
		{
			return (this._state & (uint)mod) > 0U;
		}

		public bool isShiftPressed
		{
			get
			{
				return this.IsPressed(EventModifiers.Modifiers.Shift);
			}
		}

		public bool isCtrlPressed
		{
			get
			{
				return this.IsPressed(EventModifiers.Modifiers.Ctrl);
			}
		}

		public bool isAltPressed
		{
			get
			{
				return this.IsPressed(EventModifiers.Modifiers.Alt);
			}
		}

		public bool isMetaPressed
		{
			get
			{
				return this.IsPressed(EventModifiers.Modifiers.Meta);
			}
		}

		public bool isCapsLockEnabled
		{
			get
			{
				return this.IsPressed(EventModifiers.Modifiers.CapsLock);
			}
		}

		public bool isNumLockEnabled
		{
			get
			{
				return this.IsPressed(EventModifiers.Modifiers.Numlock);
			}
		}

		public bool isFunctionKeyPressed
		{
			get
			{
				return this.IsPressed(EventModifiers.Modifiers.FunctionKey);
			}
		}

		public bool isNumericPressed
		{
			get
			{
				return this.IsPressed(EventModifiers.Modifiers.Numeric);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetPressed(EventModifiers.Modifiers modifier, bool pressed)
		{
			if (pressed)
			{
				this._state |= (uint)modifier;
			}
			else
			{
				this._state &= (uint)(~(uint)modifier);
			}
		}

		public void Reset()
		{
			this._state = 0U;
		}

		private static void Append(ref string str, string value)
		{
			str = (string.IsNullOrEmpty(str) ? value : (str + "," + value));
		}

		public override string ToString()
		{
			string empty = string.Empty;
			bool flag = this.IsPressed(EventModifiers.Modifiers.LeftShift);
			if (flag)
			{
				EventModifiers.Append(ref empty, "LeftShift");
			}
			bool flag2 = this.IsPressed(EventModifiers.Modifiers.RightShift);
			if (flag2)
			{
				EventModifiers.Append(ref empty, "RightShift");
			}
			bool flag3 = this.IsPressed(EventModifiers.Modifiers.LeftCtrl);
			if (flag3)
			{
				EventModifiers.Append(ref empty, "LeftCtrl");
			}
			bool flag4 = this.IsPressed(EventModifiers.Modifiers.RightCtrl);
			if (flag4)
			{
				EventModifiers.Append(ref empty, "RightCtrl");
			}
			bool flag5 = this.IsPressed(EventModifiers.Modifiers.LeftAlt);
			if (flag5)
			{
				EventModifiers.Append(ref empty, "LeftAlt");
			}
			bool flag6 = this.IsPressed(EventModifiers.Modifiers.RightAlt);
			if (flag6)
			{
				EventModifiers.Append(ref empty, "RightAlt");
			}
			bool flag7 = this.IsPressed(EventModifiers.Modifiers.LeftMeta);
			if (flag7)
			{
				EventModifiers.Append(ref empty, "LeftMeta");
			}
			bool flag8 = this.IsPressed(EventModifiers.Modifiers.RightMeta);
			if (flag8)
			{
				EventModifiers.Append(ref empty, "RightMeta");
			}
			bool flag9 = this.IsPressed(EventModifiers.Modifiers.CapsLock);
			if (flag9)
			{
				EventModifiers.Append(ref empty, "CapsLock");
			}
			bool flag10 = this.IsPressed(EventModifiers.Modifiers.Numlock);
			if (flag10)
			{
				EventModifiers.Append(ref empty, "Numlock");
			}
			bool flag11 = this.IsPressed(EventModifiers.Modifiers.FunctionKey);
			if (flag11)
			{
				EventModifiers.Append(ref empty, "FunctionKey");
			}
			bool flag12 = this.IsPressed(EventModifiers.Modifiers.Numeric);
			if (flag12)
			{
				EventModifiers.Append(ref empty, "Numeric");
			}
			return empty;
		}

		private uint _state;

		[Flags]
		public enum Modifiers : uint
		{
			LeftShift = 1U,
			RightShift = 2U,
			Shift = 3U,
			LeftCtrl = 4U,
			RightCtrl = 8U,
			Ctrl = 12U,
			LeftAlt = 16U,
			RightAlt = 32U,
			Alt = 48U,
			LeftMeta = 64U,
			RightMeta = 128U,
			Meta = 192U,
			CapsLock = 256U,
			Numlock = 512U,
			FunctionKey = 1024U,
			Numeric = 2048U
		}
	}
}
