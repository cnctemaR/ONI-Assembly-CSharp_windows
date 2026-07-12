using System;

namespace UnityEngine
{
	internal struct SliderHandler
	{
		public SliderHandler(Rect position, float currentValue, float size, float start, float end, GUIStyle slider, GUIStyle thumb, bool horiz, int id, GUIStyle thumbExtent = null)
		{
			this.position = position;
			this.currentValue = currentValue;
			this.size = size;
			this.start = start;
			this.end = end;
			this.slider = slider;
			this.thumb = thumb;
			this.thumbExtent = thumbExtent;
			this.horiz = horiz;
			this.id = id;
		}

		public float Handle()
		{
			bool flag = this.slider == null || this.thumb == null;
			float num;
			if (flag)
			{
				num = this.currentValue;
			}
			else
			{
				EventType eventType = this.CurrentEventType();
				EventType eventType2 = eventType;
				switch (eventType2)
				{
				case EventType.MouseDown:
					return this.OnMouseDown();
				case EventType.MouseUp:
					return this.OnMouseUp();
				case EventType.MouseMove:
					break;
				case EventType.MouseDrag:
					return this.OnMouseDrag();
				default:
					if (eventType2 == EventType.Repaint)
					{
						return this.OnRepaint();
					}
					break;
				}
				num = this.currentValue;
			}
			return num;
		}

		private float OnMouseDown()
		{
			Rect rect = this.ThumbSelectionRect();
			bool flag = GUIUtility.HitTest(rect, this.CurrentEvent());
			Rect zero = Rect.zero;
			zero.xMin = Math.Min(this.position.xMin, rect.xMin);
			zero.xMax = Math.Max(this.position.xMax, rect.xMax);
			zero.yMin = Math.Min(this.position.yMin, rect.yMin);
			zero.yMax = Math.Max(this.position.yMax, rect.yMax);
			bool flag2 = this.IsEmptySlider() || (!GUIUtility.HitTest(zero, this.CurrentEvent()) && !flag);
			float num;
			if (flag2)
			{
				num = this.currentValue;
			}
			else
			{
				GUI.scrollTroughSide = 0;
				GUIUtility.hotControl = this.id;
				this.CurrentEvent().Use();
				bool flag3 = flag;
				if (flag3)
				{
					this.StartDraggingWithValue(this.ClampedCurrentValue());
					num = this.currentValue;
				}
				else
				{
					GUI.changed = true;
					bool flag4 = this.SupportsPageMovements();
					if (flag4)
					{
						this.SliderState().isDragging = false;
						GUI.nextScrollStepTime = SystemClock.now.AddMilliseconds(250.0);
						GUI.scrollTroughSide = this.CurrentScrollTroughSide();
						num = this.PageMovementValue();
					}
					else
					{
						float num2 = this.ValueForCurrentMousePosition();
						this.StartDraggingWithValue(num2);
						num = this.Clamp(num2);
					}
				}
			}
			return num;
		}

		private float OnMouseDrag()
		{
			bool flag = GUIUtility.hotControl != this.id;
			float num;
			if (flag)
			{
				num = this.currentValue;
			}
			else
			{
				SliderState sliderState = this.SliderState();
				bool flag2 = !sliderState.isDragging;
				if (flag2)
				{
					num = this.currentValue;
				}
				else
				{
					GUI.changed = true;
					this.CurrentEvent().Use();
					float num2 = this.MousePosition() - sliderState.dragStartPos;
					float num3 = sliderState.dragStartValue + num2 / this.ValuesPerPixel();
					num = this.Clamp(num3);
				}
			}
			return num;
		}

		private float OnMouseUp()
		{
			bool flag = GUIUtility.hotControl == this.id;
			if (flag)
			{
				this.CurrentEvent().Use();
				GUIUtility.hotControl = 0;
			}
			return this.currentValue;
		}

		private float OnRepaint()
		{
			bool flag = GUIUtility.HitTest(this.position, this.CurrentEvent());
			this.slider.Draw(this.position, GUIContent.none, this.id, false, flag);
			bool flag2 = !this.IsEmptySlider() && this.currentValue >= Mathf.Min(this.start, this.end) && this.currentValue <= Mathf.Max(this.start, this.end);
			if (flag2)
			{
				bool flag3 = this.thumbExtent != null;
				if (flag3)
				{
					this.thumbExtent.Draw(this.ThumbExtRect(), GUIContent.none, this.id, false, flag);
				}
				this.thumb.Draw(this.ThumbRect(), GUIContent.none, this.id, false, flag);
			}
			bool flag4 = GUIUtility.hotControl != this.id || !flag || this.IsEmptySlider();
			float num;
			if (flag4)
			{
				num = this.currentValue;
			}
			else
			{
				bool flag5 = GUIUtility.HitTest(this.ThumbRect(), this.CurrentEvent());
				if (flag5)
				{
					bool flag6 = GUI.scrollTroughSide != 0;
					if (flag6)
					{
						GUIUtility.hotControl = 0;
					}
					num = this.currentValue;
				}
				else
				{
					GUI.InternalRepaintEditorWindow();
					bool flag7 = SystemClock.now < GUI.nextScrollStepTime;
					if (flag7)
					{
						num = this.currentValue;
					}
					else
					{
						bool flag8 = this.CurrentScrollTroughSide() != GUI.scrollTroughSide;
						if (flag8)
						{
							num = this.currentValue;
						}
						else
						{
							GUI.nextScrollStepTime = SystemClock.now.AddMilliseconds(30.0);
							bool flag9 = this.SupportsPageMovements();
							if (flag9)
							{
								this.SliderState().isDragging = false;
								GUI.changed = true;
								num = this.PageMovementValue();
							}
							else
							{
								num = this.ClampedCurrentValue();
							}
						}
					}
				}
			}
			return num;
		}

		private EventType CurrentEventType()
		{
			return this.CurrentEvent().GetTypeForControl(this.id);
		}

		private int CurrentScrollTroughSide()
		{
			float num = (this.horiz ? this.CurrentEvent().mousePosition.x : this.CurrentEvent().mousePosition.y);
			float num2 = (this.horiz ? this.ThumbRect().x : this.ThumbRect().y);
			return (num > num2) ? 1 : (-1);
		}

		private bool IsEmptySlider()
		{
			return this.start == this.end;
		}

		private bool SupportsPageMovements()
		{
			return this.size != 0f && GUI.usePageScrollbars;
		}

		private float PageMovementValue()
		{
			float num = this.currentValue;
			int num2 = ((this.start > this.end) ? (-1) : 1);
			bool flag = this.MousePosition() > this.PageUpMovementBound();
			if (flag)
			{
				num += this.size * (float)num2 * 0.9f;
			}
			else
			{
				num -= this.size * (float)num2 * 0.9f;
			}
			return this.Clamp(num);
		}

		private float PageUpMovementBound()
		{
			bool flag = this.horiz;
			float num;
			if (flag)
			{
				num = this.ThumbRect().xMax - this.position.x;
			}
			else
			{
				num = this.ThumbRect().yMax - this.position.y;
			}
			return num;
		}

		private Event CurrentEvent()
		{
			return Event.current;
		}

		private float ValueForCurrentMousePosition()
		{
			bool flag = this.horiz;
			float num;
			if (flag)
			{
				num = (this.MousePosition() - this.ThumbRect().width * 0.5f) / this.ValuesPerPixel() + this.start - this.size * 0.5f;
			}
			else
			{
				num = (this.MousePosition() - this.ThumbRect().height * 0.5f) / this.ValuesPerPixel() + this.start - this.size * 0.5f;
			}
			return num;
		}

		private float Clamp(float value)
		{
			return Mathf.Clamp(value, this.MinValue(), this.MaxValue());
		}

		private Rect ThumbSelectionRect()
		{
			return this.ThumbRect();
		}

		private void StartDraggingWithValue(float dragStartValue)
		{
			SliderState sliderState = this.SliderState();
			sliderState.dragStartPos = this.MousePosition();
			sliderState.dragStartValue = dragStartValue;
			sliderState.isDragging = true;
		}

		private SliderState SliderState()
		{
			return (SliderState)GUIUtility.GetStateObject(typeof(SliderState), this.id);
		}

		private Rect ThumbExtRect()
		{
			return new Rect(0f, 0f, this.thumbExtent.fixedWidth, this.thumbExtent.fixedHeight)
			{
				center = this.ThumbRect().center
			};
		}

		private Rect ThumbRect()
		{
			return this.horiz ? this.HorizontalThumbRect() : this.VerticalThumbRect();
		}

		private Rect VerticalThumbRect()
		{
			Rect rect = this.thumb.margin.Remove(this.slider.padding.Remove(this.position));
			float num = ((this.thumb.fixedWidth != 0f) ? this.thumb.fixedWidth : rect.width);
			float num2 = this.ThumbSize();
			float num3 = this.ValuesPerPixel();
			bool flag = this.start < this.end;
			Rect rect2;
			if (flag)
			{
				rect2 = new Rect(rect.x, (this.ClampedCurrentValue() - this.start) * num3 + rect.y, num, this.size * num3 + num2);
			}
			else
			{
				rect2 = new Rect(rect.x, (this.ClampedCurrentValue() + this.size - this.start) * num3 + rect.y, num, this.size * -num3 + num2);
			}
			return rect2;
		}

		private Rect HorizontalThumbRect()
		{
			Rect rect = this.thumb.margin.Remove(this.slider.padding.Remove(this.position));
			float num = ((this.thumb.fixedHeight != 0f) ? this.thumb.fixedHeight : rect.height);
			float num2 = this.ThumbSize();
			float num3 = this.ValuesPerPixel();
			bool flag = this.start < this.end;
			Rect rect2;
			if (flag)
			{
				rect2 = new Rect((this.ClampedCurrentValue() - this.start) * num3 + rect.x, rect.y, this.size * num3 + num2, num);
			}
			else
			{
				rect2 = new Rect((this.ClampedCurrentValue() + this.size - this.start) * num3 + rect.x, rect.y, this.size * -num3 + num2, num);
			}
			return rect2;
		}

		private float ClampedCurrentValue()
		{
			return this.Clamp(this.currentValue);
		}

		private float MousePosition()
		{
			bool flag = this.horiz;
			float num;
			if (flag)
			{
				num = this.CurrentEvent().mousePosition.x - this.position.x;
			}
			else
			{
				num = this.CurrentEvent().mousePosition.y - this.position.y;
			}
			return num;
		}

		private float ValuesPerPixel()
		{
			bool flag = this.horiz;
			float num;
			if (flag)
			{
				num = (this.position.width - (float)this.slider.padding.horizontal - this.ThumbSize()) / (this.end - this.start);
			}
			else
			{
				num = (this.position.height - (float)this.slider.padding.vertical - this.ThumbSize()) / (this.end - this.start);
			}
			return num;
		}

		private float ThumbSize()
		{
			bool flag = this.horiz;
			float num;
			if (flag)
			{
				num = ((this.thumb.fixedWidth != 0f) ? this.thumb.fixedWidth : ((float)this.thumb.padding.horizontal));
			}
			else
			{
				num = ((this.thumb.fixedHeight != 0f) ? this.thumb.fixedHeight : ((float)this.thumb.padding.vertical));
			}
			return num;
		}

		private float MaxValue()
		{
			return Mathf.Max(this.start, this.end) - this.size;
		}

		private float MinValue()
		{
			return Mathf.Min(this.start, this.end);
		}

		private readonly Rect position;

		private readonly float currentValue;

		private readonly float size;

		private readonly float start;

		private readonly float end;

		private readonly GUIStyle slider;

		private readonly GUIStyle thumb;

		private readonly GUIStyle thumbExtent;

		private readonly bool horiz;

		private readonly int id;
	}
}
