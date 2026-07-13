using System;

namespace UnityEngine.UIElements
{
	public class SliderInt : BaseSlider<int>
	{
		public SliderInt()
			: this(null, 0, 10, SliderDirection.Horizontal, 0f)
		{
		}

		public SliderInt(int start, int end, SliderDirection direction = SliderDirection.Horizontal, float pageSize = 0f)
			: this(null, start, end, direction, pageSize)
		{
		}

		public SliderInt(string label, int start = 0, int end = 10, SliderDirection direction = SliderDirection.Horizontal, float pageSize = 0f)
			: base(label, start, end, direction, pageSize)
		{
			base.AddToClassList(SliderInt.ussClassName);
			base.labelElement.AddToClassList(SliderInt.labelUssClassName);
			base.visualInput.AddToClassList(SliderInt.inputUssClassName);
		}

		public override float pageSize
		{
			get
			{
				return base.pageSize;
			}
			set
			{
				base.pageSize = (float)Mathf.RoundToInt(value);
			}
		}

		public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, int startValue)
		{
			double num = (double)NumericFieldDraggerUtility.CalculateIntDragSensitivity((long)startValue, (long)base.lowValue, (long)base.highValue);
			float num2 = NumericFieldDraggerUtility.Acceleration(speed == DeltaSpeed.Fast, speed == DeltaSpeed.Slow);
			long num3 = (long)this.value;
			num3 += (long)Math.Round((double)NumericFieldDraggerUtility.NiceDelta(delta, num2) * num);
			this.value = (int)num3;
		}

		internal override int SliderLerpUnclamped(int a, int b, float interpolant)
		{
			return Mathf.RoundToInt(Mathf.LerpUnclamped((float)a, (float)b, interpolant));
		}

		internal override float SliderNormalizeValue(int currentValue, int lowerValue, int higherValue)
		{
			return ((float)currentValue - (float)lowerValue) / ((float)higherValue - (float)lowerValue);
		}

		internal override int SliderRange()
		{
			return Math.Abs(base.highValue - base.lowValue);
		}

		internal override int ParseStringToValue(string previousValue, string newValue)
		{
			int num;
			ExpressionEvaluator.Expression expression;
			bool flag = UINumericFieldsUtils.TryConvertStringToInt(newValue, previousValue, out num, out expression);
			Action<ExpressionEvaluator.Expression> expressionEvaluated = this.expressionEvaluated;
			if (expressionEvaluated != null)
			{
				expressionEvaluated(expression);
			}
			return flag ? num : 0;
		}

		internal override void ComputeValueAndDirectionFromClick(float sliderLength, float dragElementLength, float dragElementPos, float dragElementLastPos)
		{
			bool flag = Mathf.Approximately(this.pageSize, 0f);
			if (flag)
			{
				base.ComputeValueAndDirectionFromClick(sliderLength, dragElementLength, dragElementPos, dragElementLastPos);
			}
			else
			{
				float num = sliderLength - dragElementLength;
				bool flag2 = Mathf.Abs(num) < 1E-30f;
				if (!flag2)
				{
					int num2 = (int)this.pageSize;
					bool flag3 = (base.lowValue > base.highValue && !base.inverted) || (base.lowValue < base.highValue && base.inverted) || (base.direction == SliderDirection.Vertical && !base.inverted);
					if (flag3)
					{
						num2 = -num2;
					}
					bool flag4 = dragElementLastPos < dragElementPos;
					bool flag5 = dragElementLastPos > dragElementPos + dragElementLength;
					bool flag6 = (base.inverted ? flag5 : flag4);
					bool flag7 = (base.inverted ? flag4 : flag5);
					bool flag8 = flag6 && base.clampedDragger.dragDirection != ClampedDragger<int>.DragDirection.LowToHigh;
					if (flag8)
					{
						base.clampedDragger.dragDirection = ClampedDragger<int>.DragDirection.HighToLow;
						this.value -= num2;
					}
					else
					{
						bool flag9 = flag7 && base.clampedDragger.dragDirection != ClampedDragger<int>.DragDirection.HighToLow;
						if (flag9)
						{
							base.clampedDragger.dragDirection = ClampedDragger<int>.DragDirection.LowToHigh;
							this.value += num2;
						}
					}
				}
			}
		}

		internal override void ComputeValueFromKey(BaseSlider<int>.SliderKey sliderKey, bool isShift)
		{
			if (sliderKey != BaseSlider<int>.SliderKey.None)
			{
				if (sliderKey != BaseSlider<int>.SliderKey.Lowest)
				{
					if (sliderKey != BaseSlider<int>.SliderKey.Highest)
					{
						bool flag = sliderKey == BaseSlider<int>.SliderKey.LowerPage || sliderKey == BaseSlider<int>.SliderKey.HigherPage;
						float num = BaseSlider<int>.GetClosestPowerOfTen(Mathf.Abs((float)(base.highValue - base.lowValue) * 0.01f));
						bool flag2 = num < 1f;
						if (flag2)
						{
							num = 1f;
						}
						bool flag3 = flag;
						if (flag3)
						{
							num *= this.pageSize;
						}
						else if (isShift)
						{
							num *= 10f;
						}
						bool flag4 = sliderKey == BaseSlider<int>.SliderKey.Lower || sliderKey == BaseSlider<int>.SliderKey.LowerPage;
						if (flag4)
						{
							num = -num;
						}
						this.value = Mathf.RoundToInt(BaseSlider<int>.RoundToMultipleOf((float)this.value + num * 0.5001f, Mathf.Abs(num)));
					}
					else
					{
						this.value = base.highValue;
					}
				}
				else
				{
					this.value = base.lowValue;
				}
			}
		}

		internal const int kDefaultHighValue = 10;

		public new static readonly string ussClassName = "unity-slider-int";

		public new static readonly string labelUssClassName = SliderInt.ussClassName + "__label";

		public new static readonly string inputUssClassName = SliderInt.ussClassName + "__input";

		public new class UxmlFactory : UxmlFactory<SliderInt, SliderInt.UxmlTraits>
		{
		}

		public new class UxmlTraits : BaseSlider<int>.UxmlTraits
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				SliderInt sliderInt = (SliderInt)ve;
				sliderInt.lowValue = this.m_LowValue.GetValueFromBag(bag, cc);
				sliderInt.highValue = this.m_HighValue.GetValueFromBag(bag, cc);
				sliderInt.direction = this.m_Direction.GetValueFromBag(bag, cc);
				sliderInt.pageSize = (float)this.m_PageSize.GetValueFromBag(bag, cc);
				sliderInt.showInputField = this.m_ShowInputField.GetValueFromBag(bag, cc);
				sliderInt.inverted = this.m_Inverted.GetValueFromBag(bag, cc);
				base.Init(ve, bag, cc);
			}

			private UxmlIntAttributeDescription m_LowValue = new UxmlIntAttributeDescription
			{
				name = "low-value"
			};

			private UxmlIntAttributeDescription m_HighValue = new UxmlIntAttributeDescription
			{
				name = "high-value",
				defaultValue = 10
			};

			private UxmlIntAttributeDescription m_PageSize = new UxmlIntAttributeDescription
			{
				name = "page-size",
				defaultValue = 0
			};

			private UxmlBoolAttributeDescription m_ShowInputField = new UxmlBoolAttributeDescription
			{
				name = "show-input-field",
				defaultValue = false
			};

			private UxmlEnumAttributeDescription<SliderDirection> m_Direction = new UxmlEnumAttributeDescription<SliderDirection>
			{
				name = "direction",
				defaultValue = SliderDirection.Horizontal
			};

			private UxmlBoolAttributeDescription m_Inverted = new UxmlBoolAttributeDescription
			{
				name = "inverted",
				defaultValue = false
			};
		}
	}
}
