using System;
using System.Diagnostics;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
	public class Slider : BaseSlider<float>
	{
		public Slider()
			: this(null, 0f, 10f, SliderDirection.Horizontal, 0f)
		{
		}

		public Slider(float start, float end, SliderDirection direction = SliderDirection.Horizontal, float pageSize = 0f)
			: this(null, start, end, direction, pageSize)
		{
		}

		public Slider(string label, float start = 0f, float end = 10f, SliderDirection direction = SliderDirection.Horizontal, float pageSize = 0f)
			: base(label, start, end, direction, pageSize)
		{
			base.AddToClassList(Slider.ussClassName);
			base.labelElement.AddToClassList(Slider.labelUssClassName);
			base.visualInput.AddToClassList(Slider.inputUssClassName);
		}

		public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, float startValue)
		{
			double num = NumericFieldDraggerUtility.CalculateFloatDragSensitivity((double)startValue, (double)base.lowValue, (double)base.highValue);
			float num2 = NumericFieldDraggerUtility.Acceleration(speed == DeltaSpeed.Fast, speed == DeltaSpeed.Slow);
			double num3 = (double)this.value;
			num3 += (double)NumericFieldDraggerUtility.NiceDelta(delta, num2) * num;
			this.value = (float)num3;
		}

		internal override float SliderLerpUnclamped(float a, float b, float interpolant)
		{
			float num = Mathf.LerpUnclamped(a, b, interpolant);
			float num2 = Mathf.Abs((base.highValue - base.lowValue) / (base.dragContainer.resolvedStyle.width - base.dragElement.resolvedStyle.width));
			int num3 = ((num2 == 0f) ? Mathf.Clamp((int)(5.0 - (double)Mathf.Log10(Mathf.Abs(num2))), 0, 15) : Mathf.Clamp(-Mathf.FloorToInt(Mathf.Log10(Mathf.Abs(num2))), 0, 15));
			return (float)Math.Round((double)num, num3, MidpointRounding.AwayFromZero);
		}

		internal override float SliderNormalizeValue(float currentValue, float lowerValue, float higherValue)
		{
			float num = higherValue - lowerValue;
			bool flag = Mathf.Approximately(num, 0f);
			float num2;
			if (flag)
			{
				num2 = 1f;
			}
			else
			{
				num2 = (currentValue - lowerValue) / num;
			}
			return num2;
		}

		internal override float SliderRange()
		{
			return Math.Abs(base.highValue - base.lowValue);
		}

		internal override float ParseStringToValue(string previousValue, string newValue)
		{
			float num;
			ExpressionEvaluator.Expression expression;
			bool flag = UINumericFieldsUtils.TryConvertStringToFloat(newValue, previousValue, out num, out expression);
			Action<ExpressionEvaluator.Expression> expressionEvaluated = this.expressionEvaluated;
			if (expressionEvaluated != null)
			{
				expressionEvaluated(expression);
			}
			return flag ? num : 0f;
		}

		internal override void ComputeValueFromKey(BaseSlider<float>.SliderKey sliderKey, bool isShift)
		{
			if (sliderKey != BaseSlider<float>.SliderKey.None)
			{
				if (sliderKey != BaseSlider<float>.SliderKey.Lowest)
				{
					if (sliderKey != BaseSlider<float>.SliderKey.Highest)
					{
						bool flag = sliderKey == BaseSlider<float>.SliderKey.LowerPage || sliderKey == BaseSlider<float>.SliderKey.HigherPage;
						float num = BaseSlider<float>.GetClosestPowerOfTen(Mathf.Abs((base.highValue - base.lowValue) * 0.01f));
						bool flag2 = flag;
						if (flag2)
						{
							num *= this.pageSize;
						}
						else if (isShift)
						{
							num *= 10f;
						}
						bool flag3 = sliderKey == BaseSlider<float>.SliderKey.Lower || sliderKey == BaseSlider<float>.SliderKey.LowerPage;
						if (flag3)
						{
							num = -num;
						}
						this.value = BaseSlider<float>.RoundToMultipleOf(this.value + num * 0.5001f, Mathf.Abs(num));
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

		internal const float kDefaultHighValue = 10f;

		public new static readonly string ussClassName = "unity-slider";

		public new static readonly string labelUssClassName = Slider.ussClassName + "__label";

		public new static readonly string inputUssClassName = Slider.ussClassName + "__input";

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : BaseSlider<float>.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				BaseSlider<float>.UxmlSerializedData.Register();
				UxmlDescriptionCache.RegisterType(typeof(Slider.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("pageSize", "page-size", null, Array.Empty<string>()),
					new UxmlAttributeNames("showInputField", "show-input-field", null, Array.Empty<string>()),
					new UxmlAttributeNames("direction", "direction", null, Array.Empty<string>()),
					new UxmlAttributeNames("inverted", "inverted", null, Array.Empty<string>())
				}, false);
			}

			public override object CreateInstance()
			{
				return new Slider();
			}

			public override void Deserialize(object obj)
			{
				base.Deserialize(obj);
				Slider slider = (Slider)obj;
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.direction_UxmlAttributeFlags);
				if (flag)
				{
					slider.direction = this.direction;
				}
				bool flag2 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.pageSize_UxmlAttributeFlags);
				if (flag2)
				{
					slider.pageSize = this.pageSize;
				}
				bool flag3 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.showInputField_UxmlAttributeFlags);
				if (flag3)
				{
					slider.showInputField = this.showInputField;
				}
				bool flag4 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.inverted_UxmlAttributeFlags);
				if (flag4)
				{
					slider.inverted = this.inverted;
				}
			}

			[SerializeField]
			private float pageSize;

			[SerializeField]
			private SliderDirection direction;

			[SerializeField]
			private bool showInputField;

			[SerializeField]
			private bool inverted;

			[HideInInspector]
			[SerializeField]
			[UxmlIgnore]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags pageSize_UxmlAttributeFlags;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags showInputField_UxmlAttributeFlags;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags direction_UxmlAttributeFlags;

			[SerializeField]
			[HideInInspector]
			[UxmlIgnore]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags inverted_UxmlAttributeFlags;
		}

		[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlFactory : UxmlFactory<Slider, Slider.UxmlTraits>
		{
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlTraits : BaseSlider<float>.UxmlTraits<UxmlFloatAttributeDescription>
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				Slider slider = (Slider)ve;
				slider.lowValue = this.m_LowValue.GetValueFromBag(bag, cc);
				slider.highValue = this.m_HighValue.GetValueFromBag(bag, cc);
				slider.direction = this.m_Direction.GetValueFromBag(bag, cc);
				slider.pageSize = this.m_PageSize.GetValueFromBag(bag, cc);
				slider.showInputField = this.m_ShowInputField.GetValueFromBag(bag, cc);
				slider.inverted = this.m_Inverted.GetValueFromBag(bag, cc);
				base.Init(ve, bag, cc);
			}

			private UxmlFloatAttributeDescription m_LowValue = new UxmlFloatAttributeDescription
			{
				name = "low-value"
			};

			private UxmlFloatAttributeDescription m_HighValue = new UxmlFloatAttributeDescription
			{
				name = "high-value",
				defaultValue = 10f
			};

			private UxmlFloatAttributeDescription m_PageSize = new UxmlFloatAttributeDescription
			{
				name = "page-size",
				defaultValue = 0f
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
