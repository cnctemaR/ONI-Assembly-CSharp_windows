using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnityEngine.Experimental.UIElements
{
	public class Slider : VisualElement
	{
		public Slider()
			: this(0f, 10f, null, Slider.Direction.Horizontal, 10f)
		{
		}

		public Slider(float start, float end, Action<float> valueChanged, Slider.Direction direction = Slider.Direction.Horizontal, float pageSize = 10f)
		{
			this.direction = direction;
			this.pageSize = pageSize;
			this.lowValue = start;
			this.highValue = end;
			base.Add(new VisualElement
			{
				name = "TrackElement"
			});
			this.dragElement = new VisualElement
			{
				name = "DragElement"
			};
			this.dragElement.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.UpdateDragElementPosition), Capture.NoCapture);
			base.Add(this.dragElement);
			this.clampedDragger = new ClampedDragger(this, new Action(this.SetSliderValueFromClick), new Action(this.SetSliderValueFromDrag));
			this.AddManipulator(this.clampedDragger);
			this.valueChanged = valueChanged;
		}

		public VisualElement dragElement { get; private set; }

		public float lowValue
		{
			get
			{
				return this.m_LowValue;
			}
			set
			{
				if (!Mathf.Approximately(this.m_LowValue, value))
				{
					this.m_LowValue = value;
					this.ClampValue();
					this.UpdateDragElementPosition();
				}
			}
		}

		public float highValue
		{
			get
			{
				return this.m_HighValue;
			}
			set
			{
				if (!Mathf.Approximately(this.m_HighValue, value))
				{
					this.m_HighValue = value;
					this.ClampValue();
					this.UpdateDragElementPosition();
				}
			}
		}

		public float range
		{
			get
			{
				return Math.Abs(this.highValue - this.lowValue);
			}
		}

		public float pageSize { get; set; }

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<float> valueChanged;

		internal ClampedDragger clampedDragger { get; private set; }

		public float value
		{
			get
			{
				return (this.m_SliderValue != null) ? this.m_SliderValue.m_Value : 0f;
			}
			set
			{
				if (this.m_SliderValue == null)
				{
					this.m_SliderValue = new Slider.SliderValue
					{
						m_Value = this.lowValue
					};
				}
				float num = this.lowValue;
				float num2 = this.highValue;
				if (num > num2)
				{
					float num3 = num;
					num = num2;
					num2 = num3;
				}
				float num4 = Mathf.Clamp(value, num, num2);
				if (!Mathf.Approximately(this.m_SliderValue.m_Value, num4))
				{
					this.m_SliderValue.m_Value = num4;
					this.UpdateDragElementPosition();
					if (this.valueChanged != null)
					{
						this.valueChanged(this.m_SliderValue.m_Value);
					}
					base.Dirty(ChangeType.Repaint);
					base.SavePersistentData();
				}
			}
		}

		public Slider.Direction direction
		{
			get
			{
				return this.m_Direction;
			}
			set
			{
				this.m_Direction = value;
				if (this.m_Direction == Slider.Direction.Horizontal)
				{
					base.RemoveFromClassList("vertical");
					base.AddToClassList("horizontal");
				}
				else
				{
					base.RemoveFromClassList("horizontal");
					base.AddToClassList("vertical");
				}
			}
		}

		private void ClampValue()
		{
			this.value = this.value;
		}

		/// <summary>
		///   <para>Called when the persistent data is accessible and/or when the data or persistence key have changed (VisualElement is properly parented).</para>
		/// </summary>
		public override void OnPersistentDataReady()
		{
			base.OnPersistentDataReady();
			string fullHierarchicalPersistenceKey = base.GetFullHierarchicalPersistenceKey();
			this.m_SliderValue = base.GetOrCreatePersistentData<Slider.SliderValue>(this.m_SliderValue, fullHierarchicalPersistenceKey);
		}

		private void SetSliderValueFromDrag()
		{
			if (this.clampedDragger.dragDirection == ClampedDragger.DragDirection.Free)
			{
				Vector2 delta = this.clampedDragger.delta;
				if (this.direction == Slider.Direction.Horizontal)
				{
					this.ComputeValueAndDirectionFromDrag(base.layout.width, this.dragElement.style.width, this.m_DragElementStartPos.x + delta.x);
				}
				else
				{
					this.ComputeValueAndDirectionFromDrag(base.layout.height, this.dragElement.style.height, this.m_DragElementStartPos.y + delta.y);
				}
			}
		}

		private void ComputeValueAndDirectionFromDrag(float sliderLength, float dragElementLength, float dragElementPos)
		{
			float num = sliderLength - dragElementLength;
			if (Mathf.Abs(num) >= Mathf.Epsilon)
			{
				float num2 = Mathf.Max(0f, Mathf.Min(dragElementPos, num)) / num;
				this.value = Mathf.LerpUnclamped(this.lowValue, this.highValue, num2);
			}
		}

		private void SetSliderValueFromClick()
		{
			if (this.clampedDragger.dragDirection != ClampedDragger.DragDirection.Free)
			{
				if (this.clampedDragger.dragDirection == ClampedDragger.DragDirection.None)
				{
					if (this.pageSize == 0f)
					{
						float num = ((this.direction != Slider.Direction.Horizontal) ? this.dragElement.style.positionLeft.value : (this.clampedDragger.startMousePosition.x - this.dragElement.style.width / 2f));
						float num2 = ((this.direction != Slider.Direction.Horizontal) ? (this.clampedDragger.startMousePosition.y - this.dragElement.style.height / 2f) : this.dragElement.style.positionTop.value);
						this.dragElement.style.positionLeft = num;
						this.dragElement.style.positionTop = num2;
						this.m_DragElementStartPos = new Rect(num, num2, this.dragElement.style.width, this.dragElement.style.height);
						this.clampedDragger.dragDirection = ClampedDragger.DragDirection.Free;
						if (this.direction == Slider.Direction.Horizontal)
						{
							this.ComputeValueAndDirectionFromDrag(base.layout.width, this.dragElement.style.width, this.m_DragElementStartPos.x);
						}
						else
						{
							this.ComputeValueAndDirectionFromDrag(base.layout.height, this.dragElement.style.height, this.m_DragElementStartPos.y);
						}
						return;
					}
					this.m_DragElementStartPos = new Rect(this.dragElement.style.positionLeft, this.dragElement.style.positionTop, this.dragElement.style.width, this.dragElement.style.height);
				}
				if (this.direction == Slider.Direction.Horizontal)
				{
					this.ComputeValueAndDirectionFromClick(base.layout.width, this.dragElement.style.width, this.dragElement.style.positionLeft, this.clampedDragger.lastMousePosition.x);
				}
				else
				{
					this.ComputeValueAndDirectionFromClick(base.layout.height, this.dragElement.style.height, this.dragElement.style.positionTop, this.clampedDragger.lastMousePosition.y);
				}
			}
		}

		private void ComputeValueAndDirectionFromClick(float sliderLength, float dragElementLength, float dragElementPos, float dragElementLastPos)
		{
			float num = sliderLength - dragElementLength;
			if (Mathf.Abs(num) >= Mathf.Epsilon)
			{
				if (dragElementLastPos < dragElementPos && this.clampedDragger.dragDirection != ClampedDragger.DragDirection.LowToHigh)
				{
					this.clampedDragger.dragDirection = ClampedDragger.DragDirection.HighToLow;
					float num2 = Mathf.Max(0f, Mathf.Min(dragElementPos - this.pageSize, num)) / num;
					this.value = Mathf.LerpUnclamped(this.lowValue, this.highValue, num2);
				}
				else if (dragElementLastPos > dragElementPos + dragElementLength && this.clampedDragger.dragDirection != ClampedDragger.DragDirection.HighToLow)
				{
					this.clampedDragger.dragDirection = ClampedDragger.DragDirection.LowToHigh;
					float num3 = Mathf.Max(0f, Mathf.Min(dragElementPos + this.pageSize, num)) / num;
					this.value = Mathf.LerpUnclamped(this.lowValue, this.highValue, num3);
				}
			}
		}

		public void AdjustDragElement(float factor)
		{
			bool flag = factor < 1f;
			this.dragElement.visible = flag;
			if (flag)
			{
				IStyle style = this.dragElement.style;
				this.dragElement.visible = true;
				if (this.direction == Slider.Direction.Horizontal)
				{
					float specifiedValueOrDefault = style.minWidth.GetSpecifiedValueOrDefault(0f);
					style.width = Mathf.Max(base.layout.width * factor, specifiedValueOrDefault);
				}
				else
				{
					float specifiedValueOrDefault2 = style.minHeight.GetSpecifiedValueOrDefault(0f);
					style.height = Mathf.Max(base.layout.height * factor, specifiedValueOrDefault2);
				}
			}
		}

		private void UpdateDragElementPosition(GeometryChangedEvent evt)
		{
			if (!(evt.oldRect.size == evt.newRect.size))
			{
				this.UpdateDragElementPosition();
			}
		}

		private void UpdateDragElementPosition()
		{
			if (base.panel != null)
			{
				float num = (this.value - this.lowValue) / (this.highValue - this.lowValue);
				float num2 = this.dragElement.style.width;
				float num3 = this.dragElement.style.height;
				if (this.direction == Slider.Direction.Horizontal)
				{
					float num4 = base.layout.width - num2;
					this.dragElement.style.positionLeft = num * num4;
				}
				else
				{
					float num5 = base.layout.height - num3;
					this.dragElement.style.positionTop = num * num5;
				}
			}
		}

		protected internal override void ExecuteDefaultAction(EventBase evt)
		{
			base.ExecuteDefaultAction(evt);
			if (evt.GetEventTypeId() == EventBase<GeometryChangedEvent>.TypeId())
			{
				this.UpdateDragElementPosition((GeometryChangedEvent)evt);
			}
		}

		private float m_LowValue;

		private float m_HighValue;

		private Rect m_DragElementStartPos;

		private Slider.SliderValue m_SliderValue;

		private Slider.Direction m_Direction;

		internal const float kDefaultHighValue = 10f;

		internal const float kDefaultPageSize = 10f;

		/// <summary>
		///   <para>Instantiates a Slider using the data read from a UXML file.</para>
		/// </summary>
		public class SliderFactory : UxmlFactory<Slider, Slider.SliderUxmlTraits>
		{
		}

		/// <summary>
		///   <para>UxmlTraits for the Slider.</para>
		/// </summary>
		public class SliderUxmlTraits : VisualElement.VisualElementUxmlTraits
		{
			/// <summary>
			///   <para>Constructor.</para>
			/// </summary>
			public SliderUxmlTraits()
			{
				this.m_LowValue = new UxmlFloatAttributeDescription
				{
					name = "lowValue"
				};
				this.m_HighValue = new UxmlFloatAttributeDescription
				{
					name = "highValue",
					defaultValue = 10f
				};
				this.m_PageSize = new UxmlFloatAttributeDescription
				{
					name = "pageSize",
					defaultValue = 10f
				};
				this.m_Direction = new UxmlEnumAttributeDescription<Slider.Direction>
				{
					name = "direction",
					defaultValue = Slider.Direction.Vertical
				};
				this.m_Value = new UxmlFloatAttributeDescription
				{
					name = "value"
				};
			}

			/// <summary>
			///   <para>Returns an enumerable containing attribute descriptions for Slider properties that should be available in UXML.</para>
			/// </summary>
			public override IEnumerable<UxmlAttributeDescription> uxmlAttributesDescription
			{
				get
				{
					foreach (UxmlAttributeDescription attr in this.<get_uxmlAttributesDescription>__BaseCallProxy0())
					{
						yield return attr;
					}
					yield return this.m_LowValue;
					yield return this.m_HighValue;
					yield return this.m_PageSize;
					yield return this.m_Direction;
					yield return this.m_Value;
					yield break;
				}
			}

			/// <summary>
			///   <para>Returns an empty enumerable, as sliders generally do not have children.</para>
			/// </summary>
			public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
			{
				get
				{
					yield break;
				}
			}

			/// <summary>
			///   <para>Initialize Slider properties using values from the attribute bag.</para>
			/// </summary>
			/// <param name="ve">The object to initialize.</param>
			/// <param name="bag">The attribute bag.</param>
			/// <param name="cc">The creation context; unused.</param>
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				Slider slider = (Slider)ve;
				slider.lowValue = this.m_LowValue.GetValueFromBag(bag);
				slider.highValue = this.m_HighValue.GetValueFromBag(bag);
				slider.direction = this.m_Direction.GetValueFromBag(bag);
				slider.pageSize = this.m_PageSize.GetValueFromBag(bag);
				slider.value = this.m_Value.GetValueFromBag(bag);
			}

			private UxmlFloatAttributeDescription m_LowValue;

			private UxmlFloatAttributeDescription m_HighValue;

			private UxmlFloatAttributeDescription m_PageSize;

			private UxmlEnumAttributeDescription<Slider.Direction> m_Direction;

			private UxmlFloatAttributeDescription m_Value;
		}

		public enum Direction
		{
			Horizontal,
			Vertical
		}

		[Serializable]
		private class SliderValue
		{
			public float m_Value = 0f;
		}
	}
}
