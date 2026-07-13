using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.IntegerTime;
using UnityEngine.Bindings;

namespace UnityEngine.InputForUI
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal struct KeyEvent : IEventProperties
	{
		public DiscreteTime timestamp { readonly get; set; }

		public EventSource eventSource { readonly get; set; }

		public uint playerId { readonly get; set; }

		public EventModifiers eventModifiers { readonly get; set; }

		public override string ToString()
		{
			KeyEvent.Type type = this.type;
			KeyEvent.Type type2 = type;
			string text;
			if (type2 - KeyEvent.Type.KeyPressed > 2)
			{
				if (type2 != KeyEvent.Type.State)
				{
					throw new ArgumentOutOfRangeException();
				}
				text = string.Format("{0} Pressed:{1}", this.type, this.buttonsState);
			}
			else
			{
				text = string.Format("{0} {1}", this.type, this.keyCode);
			}
			return text;
		}

		public KeyEvent.Type type;

		public KeyCode keyCode;

		public KeyEvent.ButtonsState buttonsState;

		public enum Type
		{
			KeyPressed = 1,
			KeyRepeated,
			KeyReleased,
			State
		}

		public struct ButtonsState
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal static bool ShouldBeProcessed(KeyCode keyCode)
			{
				return keyCode <= KeyCode.Menu;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private unsafe bool GetUnchecked(uint index)
			{
				return (*((ref this.buttons.FixedElementField) + (UIntPtr)(index >> 3)) & (byte)(1 << (int)(index & 7U))) > 0;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private void SetUnchecked(uint index)
			{
				ref byte ptr = (ref this.buttons.FixedElementField) + (UIntPtr)(index >> 3);
				ptr |= (byte)(1 << (int)(index & 7U));
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private void ClearUnchecked(uint index)
			{
				ref byte ptr = (ref this.buttons.FixedElementField) + (UIntPtr)(index >> 3);
				ptr &= (byte)(~(byte)(1 << (int)(index & 7U)));
			}

			public bool IsPressed(KeyCode keyCode)
			{
				return KeyEvent.ButtonsState.ShouldBeProcessed(keyCode) && this.GetUnchecked((uint)keyCode);
			}

			public IEnumerable<KeyCode> GetAllPressed()
			{
				uint num;
				for (uint index = 0U; index <= 319U; index = num)
				{
					bool @unchecked = this.GetUnchecked(index);
					if (@unchecked)
					{
						yield return (KeyCode)index;
					}
					num = index + 1U;
				}
				yield break;
			}

			public void SetPressed(KeyCode keyCode, bool pressed)
			{
				bool flag = !KeyEvent.ButtonsState.ShouldBeProcessed(keyCode);
				if (!flag)
				{
					if (pressed)
					{
						this.SetUnchecked((uint)keyCode);
					}
					else
					{
						this.ClearUnchecked((uint)keyCode);
					}
				}
			}

			public unsafe void Reset()
			{
				int num = 0;
				while ((long)num < 40L)
				{
					*((ref this.buttons.FixedElementField) + num) = 0;
					num++;
				}
			}

			public override string ToString()
			{
				return string.Join<KeyCode>(",", this.GetAllPressed());
			}

			private const uint kMaxIndex = 319U;

			private const uint kSizeInBytes = 40U;

			[FixedBuffer(typeof(byte), 40)]
			private KeyEvent.ButtonsState.<buttons>e__FixedBuffer buttons;

			[UnsafeValueType]
			[CompilerGenerated]
			[StructLayout(LayoutKind.Sequential, Size = 40)]
			public struct <buttons>e__FixedBuffer
			{
				public byte FixedElementField;
			}
		}
	}
}
