using System;
using Unity.IntegerTime;

namespace UnityEngine.InputForUI
{
	internal struct PointerState
	{
		public PointerEvent.Button LastPressedButton { readonly get; private set; }

		public PointerEvent.ButtonsState ButtonsState
		{
			get
			{
				return this._buttonsState;
			}
		}

		public DiscreteTime NextPressTime { readonly get; private set; }

		public int ClickCount { readonly get; private set; }

		public Vector2 LastPosition { readonly get; private set; }

		public int LastDisplayIndex { readonly get; private set; }

		public bool LastPositionValid { readonly get; set; }

		public void OnButtonDown(DiscreteTime currentTime, PointerEvent.Button button)
		{
			bool flag = this.LastPressedButton != button || currentTime >= this.NextPressTime;
			if (flag)
			{
				this.ClickCount = 0;
			}
			this.LastPressedButton = button;
			this._buttonsState.Set(button, true);
			int clickCount = this.ClickCount;
			this.ClickCount = clickCount + 1;
			this.NextPressTime = currentTime + PointerState.kClickDelay;
		}

		public void OnButtonUp(DiscreteTime currentTime, PointerEvent.Button button)
		{
			bool flag = this.LastPressedButton != button;
			if (flag)
			{
				this.ClickCount = 1;
			}
			this._buttonsState.Set(button, false);
		}

		public void OnButtonChange(DiscreteTime currentTime, PointerEvent.Button button, bool previousState, bool newState)
		{
			bool flag = newState && !previousState;
			if (flag)
			{
				this.OnButtonDown(currentTime, button);
			}
			else
			{
				bool flag2 = !newState && previousState;
				if (flag2)
				{
					this.OnButtonUp(currentTime, button);
				}
			}
		}

		public void OnMove(DiscreteTime currentTime, Vector2 position, int displayIndex)
		{
			this.LastPosition = position;
			this.LastDisplayIndex = displayIndex;
			this.LastPositionValid = true;
		}

		public void Reset()
		{
			this.LastPressedButton = PointerEvent.Button.None;
			this.ButtonsState.Reset();
			this.NextPressTime = DiscreteTime.Zero;
			this.ClickCount = 0;
			this.LastPosition = Vector2.zero;
			this.LastDisplayIndex = 0;
			this.LastPositionValid = false;
		}

		private PointerEvent.ButtonsState _buttonsState;

		private static readonly DiscreteTime kClickDelay = new DiscreteTime((double)Event.GetDoubleClickTime() / 1000.0);
	}
}
