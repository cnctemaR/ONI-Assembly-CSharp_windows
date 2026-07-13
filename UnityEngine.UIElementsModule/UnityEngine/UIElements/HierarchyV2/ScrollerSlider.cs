using System;

namespace UnityEngine.UIElements.HierarchyV2
{
	internal class ScrollerSlider : BaseSlider<double>
	{
		public ScrollerSlider(double start, double end, SliderDirection direction, float pageSize)
			: base(null, start, end, direction, pageSize)
		{
		}

		internal override double SliderLerpUnclamped(double a, double b, float interpolant)
		{
			double num = a + (b - a) * (double)interpolant;
			double num2 = Math.Abs((base.highValue - base.lowValue) / (double)(base.dragContainer.resolvedStyle.width - base.dragElement.resolvedStyle.width));
			int num3 = ((num2 == 0.0) ? Math.Clamp((int)(5.0 - Math.Log10(Math.Abs(num2))), 0, 15) : ((int)Math.Clamp(-Math.Floor(Math.Log10(Math.Abs(num2))), 0.0, 15.0)));
			return Math.Round(num, num3, MidpointRounding.AwayFromZero);
		}

		internal override float SliderNormalizeValue(double currentValue, double lowerValue, double higherValue)
		{
			double num = higherValue - lowerValue;
			bool flag = Math.Abs(num) < 1E-05;
			float num2;
			if (flag)
			{
				num2 = 1f;
			}
			else
			{
				double num3 = (currentValue - lowerValue) / num;
				num2 = (float)Math.Clamp(num3, 0.0, 1.0);
			}
			return num2;
		}

		internal override double SliderRange()
		{
			return Math.Abs(base.highValue - base.lowValue);
		}

		internal override double ParseStringToValue(string previousValue, string newValue)
		{
			double num;
			ExpressionEvaluator.Expression expression;
			bool flag = UINumericFieldsUtils.TryConvertStringToDouble(newValue, previousValue, out num, out expression);
			double num2;
			if (flag)
			{
				num2 = num;
			}
			else
			{
				num2 = 0.0;
			}
			return num2;
		}

		private static double GetClosestPowerOfTen(double positiveNumber)
		{
			bool flag = positiveNumber <= 0.0;
			double num;
			if (flag)
			{
				num = 1.0;
			}
			else
			{
				num = Math.Pow(10.0, Math.Round(Math.Log10(positiveNumber)));
			}
			return num;
		}

		private static double RoundToMultipleOf(double value, double roundingValue)
		{
			bool flag = roundingValue == 0.0;
			double num;
			if (flag)
			{
				num = value;
			}
			else
			{
				num = Math.Round(value / roundingValue) * roundingValue;
			}
			return num;
		}

		internal override void ComputeValueFromKey(BaseSlider<double>.SliderKey sliderKey, bool isShift)
		{
			if (sliderKey != BaseSlider<double>.SliderKey.None)
			{
				if (sliderKey != BaseSlider<double>.SliderKey.Lowest)
				{
					if (sliderKey != BaseSlider<double>.SliderKey.Highest)
					{
						bool flag = sliderKey == BaseSlider<double>.SliderKey.LowerPage || sliderKey == BaseSlider<double>.SliderKey.HigherPage;
						double num = ScrollerSlider.GetClosestPowerOfTen(Math.Abs((base.highValue - base.lowValue) * 0.009999999776482582));
						bool flag2 = flag;
						if (flag2)
						{
							num *= (double)this.pageSize;
						}
						else if (isShift)
						{
							num *= 10.0;
						}
						bool flag3 = sliderKey == BaseSlider<double>.SliderKey.Lower || sliderKey == BaseSlider<double>.SliderKey.LowerPage;
						if (flag3)
						{
							num = -num;
						}
						this.value = ScrollerSlider.RoundToMultipleOf(this.value + num * 0.5001, Math.Abs(num));
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
	}
}
