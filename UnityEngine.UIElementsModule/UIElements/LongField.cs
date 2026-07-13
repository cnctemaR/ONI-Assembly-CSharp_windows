using System;
using System.Globalization;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.UIElements
{
	[MovedFrom(true, "UnityEditor.UIElements", "UnityEditor.UIElementsModule", null)]
	public class LongField : TextValueField<long>
	{
		private LongField.LongInput longInput
		{
			get
			{
				return (LongField.LongInput)base.textInputBase;
			}
		}

		protected override string ValueToString(long v)
		{
			return v.ToString(base.formatString, CultureInfo.InvariantCulture.NumberFormat);
		}

		protected override long StringToValue(string str)
		{
			long num;
			ExpressionEvaluator.Expression expression;
			bool flag = UINumericFieldsUtils.TryConvertStringToLong(str, base.textInputBase.originalText, out num, out expression);
			Action<ExpressionEvaluator.Expression> expressionEvaluated = this.expressionEvaluated;
			if (expressionEvaluated != null)
			{
				expressionEvaluated(expression);
			}
			return flag ? num : base.rawValue;
		}

		public LongField()
			: this(null, 1000)
		{
		}

		public LongField(int maxLength)
			: this(null, maxLength)
		{
		}

		public LongField(string label, int maxLength = 1000)
			: base(label, maxLength, new LongField.LongInput())
		{
			base.AddToClassList(LongField.ussClassName);
			base.labelElement.AddToClassList(LongField.labelUssClassName);
			base.visualInput.AddToClassList(LongField.inputUssClassName);
			base.AddLabelDragger<long>();
		}

		internal override bool CanTryParse(string textString)
		{
			long num;
			return long.TryParse(textString, out num);
		}

		public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, long startValue)
		{
			this.longInput.ApplyInputDeviceDelta(delta, speed, startValue);
		}

		public new static readonly string ussClassName = "unity-long-field";

		public new static readonly string labelUssClassName = LongField.ussClassName + "__label";

		public new static readonly string inputUssClassName = LongField.ussClassName + "__input";

		public new class UxmlFactory : UxmlFactory<LongField, LongField.UxmlTraits>
		{
		}

		public new class UxmlTraits : TextValueFieldTraits<long, UxmlLongAttributeDescription>
		{
		}

		private class LongInput : TextValueField<long>.TextValueInput
		{
			private LongField parentLongField
			{
				get
				{
					return (LongField)base.parent;
				}
			}

			internal LongInput()
			{
				base.formatString = UINumericFieldsUtils.k_IntFieldFormatString;
			}

			protected override string allowedCharacters
			{
				get
				{
					return UINumericFieldsUtils.k_AllowedCharactersForInt;
				}
			}

			public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, long startValue)
			{
				double num = (double)NumericFieldDraggerUtility.CalculateIntDragSensitivity(startValue);
				float num2 = NumericFieldDraggerUtility.Acceleration(speed == DeltaSpeed.Fast, speed == DeltaSpeed.Slow);
				long num3 = this.StringToValue(base.text);
				long num4 = (long)Math.Round((double)NumericFieldDraggerUtility.NiceDelta(delta, num2) * num);
				num3 = this.ClampMinMaxLongValue(num4, num3);
				bool isDelayed = this.parentLongField.isDelayed;
				if (isDelayed)
				{
					base.text = this.ValueToString(num3);
				}
				else
				{
					this.parentLongField.value = num3;
				}
			}

			private long ClampMinMaxLongValue(long niceDelta, long value)
			{
				long num = Math.Abs(niceDelta);
				bool flag = niceDelta > 0L;
				long num2;
				if (flag)
				{
					bool flag2 = value > 0L && num > long.MaxValue - value;
					if (flag2)
					{
						num2 = long.MaxValue;
					}
					else
					{
						num2 = value + niceDelta;
					}
				}
				else
				{
					bool flag3 = value < 0L && value < long.MinValue + num;
					if (flag3)
					{
						num2 = long.MinValue;
					}
					else
					{
						num2 = value - num;
					}
				}
				return num2;
			}

			protected override string ValueToString(long v)
			{
				return v.ToString(base.formatString);
			}

			protected override long StringToValue(string str)
			{
				return this.parentLongField.StringToValue(str);
			}
		}
	}
}
