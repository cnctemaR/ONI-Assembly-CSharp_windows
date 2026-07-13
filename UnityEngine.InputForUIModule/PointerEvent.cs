using System;
using System.Runtime.CompilerServices;
using Unity.IntegerTime;
using UnityEngine.Bindings;

namespace UnityEngine.InputForUI
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal struct PointerEvent : IEventProperties
	{
		public bool isPrimaryPointer
		{
			get
			{
				return this.pointerIndex == 0;
			}
		}

		public Ray worldRay
		{
			get
			{
				return new Ray(this.worldPosition, this.worldOrientation * Vector3.forward);
			}
		}

		public float azimuth
		{
			get
			{
				return InputManagerProvider.TiltToAzimuth(this.tilt);
			}
		}

		public float altitude
		{
			get
			{
				return InputManagerProvider.TiltToAltitude(this.tilt);
			}
		}

		public bool isPressed
		{
			get
			{
				return this.buttonsState.Get(this.isInverted ? PointerEvent.Button.PenEraserInTouch : PointerEvent.Button.Primary);
			}
		}

		public DiscreteTime timestamp { readonly get; set; }

		public EventSource eventSource { readonly get; set; }

		public uint playerId { readonly get; set; }

		public EventModifiers eventModifiers { readonly get; set; }

		public override string ToString()
		{
			string text = ((this.eventSource == EventSource.Pen) ? string.Format(" tilt:({0:f1},{1:f1}) az:{2:f2} al:{3:f2} twist:{4} pressure:{5} isInverted:{6}", new object[]
			{
				this.tilt.x,
				this.tilt.y,
				this.azimuth,
				this.altitude,
				this.twist,
				this.pressure,
				this.isInverted ? 1 : 0
			}) : "");
			string text2 = ((this.eventSource == EventSource.Touch) ? string.Format(" finger:{0} tilt:({1:f1},{2:f1}) twist:{3} pressure:{4}", new object[]
			{
				this.pointerIndex,
				this.tilt.x,
				this.tilt.y,
				this.twist,
				this.pressure
			}) : "");
			string text3 = string.Format(" dsp:{0}", this.displayIndex);
			string text4 = text + text2 + text3;
			string text5;
			switch (this.type)
			{
			case PointerEvent.Type.PointerMoved:
				text5 = string.Format("{0} pos:{1} dlt:{2} btns:{3}{4}", new object[] { this.type, this.position, this.deltaPosition, this.buttonsState, text4 });
				break;
			case PointerEvent.Type.Scroll:
				text5 = string.Format("{0} pos:{1} scr:{2}{3}", new object[] { this.type, this.position, this.scroll, text4 });
				break;
			case PointerEvent.Type.ButtonPressed:
			case PointerEvent.Type.ButtonReleased:
				text5 = string.Format("{0} pos:{1} btn:{2} btns:{3} clk:{4}{5}", new object[] { this.type, this.position, this.button, this.buttonsState, this.clickCount, text4 });
				break;
			case PointerEvent.Type.State:
				text5 = string.Format("{0} pos:{1} btns:{2}{3}", new object[] { this.type, this.position, this.buttonsState, text4 });
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			return text5;
		}

		internal static PointerEvent.Button ButtonFromButtonIndex(int index)
		{
			return (PointerEvent.Button)((index <= 31) ? (1 << index) : 0);
		}

		public PointerEvent.Type type;

		public int pointerIndex;

		public Vector2 position;

		public Vector2 deltaPosition;

		public Vector3 worldPosition;

		public Quaternion worldOrientation;

		public float maxDistance;

		public Vector2 scroll;

		public int displayIndex;

		public Vector2 tilt;

		public float twist;

		public float pressure;

		public bool isInverted;

		public PointerEvent.Button button;

		public PointerEvent.ButtonsState buttonsState;

		public int clickCount;

		public enum Type
		{
			PointerMoved = 1,
			Scroll,
			ButtonPressed,
			ButtonReleased,
			State,
			TouchCanceled,
			TrackedCanceled = 6
		}

		[Flags]
		public enum Button : uint
		{
			None = 0U,
			Primary = 1U,
			FingerInTouch = 1U,
			PenTipInTouch = 1U,
			PenEraserInTouch = 2U,
			PenBarrelButton = 4U,
			MouseLeft = 1U,
			MouseRight = 2U,
			MouseMiddle = 4U,
			MouseForward = 8U,
			MouseBack = 16U
		}

		public struct ButtonsState
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Set(PointerEvent.Button button, bool pressed)
			{
				if (pressed)
				{
					this._state |= (uint)button;
				}
				else
				{
					this._state &= (uint)(~(uint)button);
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool Get(PointerEvent.Button button)
			{
				return (this._state & (uint)button) > 0U;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Reset()
			{
				this._state = 0U;
			}

			public override string ToString()
			{
				return string.Format("{0:x2}", this._state);
			}

			private uint _state;
		}
	}
}
