using System;
using System.Collections.Generic;

namespace UnityEngine.InputForUI
{
	internal struct EventSanitizer
	{
		public void Reset()
		{
			this._sanitizers = new EventSanitizer.IEventSanitizer[0];
			foreach (EventSanitizer.IEventSanitizer eventSanitizer in this._sanitizers)
			{
				eventSanitizer.Reset();
			}
		}

		public void BeforeProviderUpdate()
		{
			bool flag = this._sanitizers == null;
			if (flag)
			{
				this.Reset();
			}
			foreach (EventSanitizer.IEventSanitizer eventSanitizer in this._sanitizers)
			{
				eventSanitizer.BeforeProviderUpdate();
			}
		}

		public void AfterProviderUpdate()
		{
			bool flag = this._sanitizers == null;
			if (flag)
			{
				this.Reset();
			}
			foreach (EventSanitizer.IEventSanitizer eventSanitizer in this._sanitizers)
			{
				eventSanitizer.AfterProviderUpdate();
			}
		}

		public void Inspect(in Event ev)
		{
			bool flag = this._sanitizers == null;
			if (flag)
			{
				this.Reset();
			}
			foreach (EventSanitizer.IEventSanitizer eventSanitizer in this._sanitizers)
			{
				eventSanitizer.Inspect(in ev);
			}
		}

		private EventSanitizer.IEventSanitizer[] _sanitizers;

		private interface IEventSanitizer
		{
			void Reset();

			void BeforeProviderUpdate();

			void AfterProviderUpdate();

			void Inspect(in Event ev);
		}

		private struct ClickCountEventSanitizer : EventSanitizer.IEventSanitizer
		{
			public void Reset()
			{
				this._activeButtons = new List<PointerEvent>();
				this.lastPushedIndex = 0;
			}

			public void BeforeProviderUpdate()
			{
			}

			public void AfterProviderUpdate()
			{
			}

			public void Inspect(in Event ev)
			{
				Event @event = ev;
				bool flag = @event.type != Event.Type.PointerEvent;
				if (!flag)
				{
					@event = ev;
					PointerEvent asPointerEvent = @event.asPointerEvent;
					PointerEvent.Type type = asPointerEvent.type;
					PointerEvent.Type type2 = type;
					if (type2 != PointerEvent.Type.ButtonPressed)
					{
						if (type2 == PointerEvent.Type.ButtonReleased)
						{
							PointerEvent pointerEvent = asPointerEvent;
							for (int i = 0; i < this._activeButtons.Count; i++)
							{
								PointerEvent pointerEvent2 = this._activeButtons[i];
								bool flag2 = pointerEvent2.eventSource != pointerEvent.eventSource || pointerEvent2.pointerIndex != pointerEvent.pointerIndex;
								if (!flag2)
								{
									bool flag3 = i == this.lastPushedIndex;
									if (flag3)
									{
										bool flag4 = pointerEvent2.clickCount != pointerEvent.clickCount;
										if (flag4)
										{
											Debug.LogWarning(string.Format("ButtonReleased click count doesn't match ButtonPressed click count, where '{0}' and '{1}'", pointerEvent2, pointerEvent));
										}
									}
									else
									{
										bool flag5 = pointerEvent.clickCount != 1;
										if (flag5)
										{
											Debug.LogWarning(string.Format("ButtonReleased for not the last pressed button should have click count == 1, but got '{0}'", pointerEvent));
										}
									}
									this._activeButtons.RemoveAt(i);
									return;
								}
							}
							Debug.LogWarning(string.Format("Can't find corresponding ButtonPressed for '{0}'", ev));
						}
					}
					else
					{
						this.lastPushedIndex = this._activeButtons.Count;
						this._activeButtons.Add(asPointerEvent);
					}
				}
			}

			void EventSanitizer.IEventSanitizer.Inspect(in Event ev)
			{
				this.Inspect(in ev);
			}

			private List<PointerEvent> _activeButtons;

			private int lastPushedIndex;
		}

		private class DefaultEventSystemSanitizer : EventSanitizer.IEventSanitizer
		{
			public void Reset()
			{
			}

			public void BeforeProviderUpdate()
			{
				this.m_MouseEventCount = 0;
				this.m_PenOrTouchEventCount = 0;
			}

			public void AfterProviderUpdate()
			{
				bool flag = this.m_MouseEventCount > 0 && this.m_PenOrTouchEventCount > 0;
				if (flag)
				{
					Debug.LogError("PointerEvents of source Mouse and [Pen or Touch] received in the same update. This is likely an error, and Mouse events should be discarded.");
				}
			}

			public void Inspect(in Event ev)
			{
				Event @event = ev;
				bool flag = @event.type == Event.Type.PointerEvent;
				if (flag)
				{
					@event = ev;
					PointerEvent asPointerEvent = @event.asPointerEvent;
					bool flag2 = asPointerEvent.type == PointerEvent.Type.ButtonPressed && asPointerEvent.button == PointerEvent.Button.None;
					if (flag2)
					{
						Debug.LogError("PointerEvent of type ButtonPressed must have button property set to a value other than None.");
					}
					bool flag3 = asPointerEvent.type == PointerEvent.Type.ButtonReleased && asPointerEvent.button == PointerEvent.Button.None;
					if (flag3)
					{
						Debug.LogError("PointerEvent of type ButtonReleased must have button property set to a value other than None.");
					}
					bool flag4 = asPointerEvent.eventSource == EventSource.Mouse;
					if (flag4)
					{
						this.m_MouseEventCount++;
						bool flag5 = !asPointerEvent.isPrimaryPointer;
						if (flag5)
						{
							Debug.LogError("PointerEvent of source Mouse is expected to have isPrimaryPointer set to true.");
						}
						bool flag6 = asPointerEvent.pointerIndex != 0;
						if (flag6)
						{
							Debug.LogError("PointerEvent of source Mouse is expected to have pointerIndex set to 0.");
						}
					}
					else
					{
						bool flag7 = asPointerEvent.eventSource == EventSource.Touch;
						if (flag7)
						{
							this.m_PenOrTouchEventCount++;
							bool flag8 = asPointerEvent.button != PointerEvent.Button.None && asPointerEvent.button != PointerEvent.Button.Primary;
							if (flag8)
							{
								Debug.LogError("PointerEvent of source Touch is expected to have button set to None or FingerInTouch.");
							}
						}
						else
						{
							bool flag9 = asPointerEvent.eventSource == EventSource.Pen;
							if (flag9)
							{
								this.m_PenOrTouchEventCount++;
								bool flag10 = asPointerEvent.button != PointerEvent.Button.None && asPointerEvent.button != PointerEvent.Button.Primary && asPointerEvent.button != PointerEvent.Button.PenBarrelButton && asPointerEvent.button != PointerEvent.Button.PenEraserInTouch;
								if (flag10)
								{
									Debug.LogError("PointerEvent of source Pen is expected to have button set to None, PenTipInTouch, PenBarrelButton, or PenEraserInTouch.");
								}
							}
						}
					}
				}
			}

			void EventSanitizer.IEventSanitizer.Inspect(in Event ev)
			{
				this.Inspect(in ev);
			}

			private int m_MouseEventCount;

			private int m_PenOrTouchEventCount;
		}
	}
}
