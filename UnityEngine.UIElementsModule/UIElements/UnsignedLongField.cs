using System;
using System.Globalization;

namespace UnityEngine.UIElements
{
	public class UnsignedLongField : TextValueField<ulong>
	{
		private UnsignedLongField.UnsignedLongInput unsignedLongInput
		{
			get
			{
				return (UnsignedLongField.UnsignedLongInput)base.textInputBase;
			}
		}

		protected override string ValueToString(ulong v)
		{
			return v.ToString(base.formatString, CultureInfo.InvariantCulture.NumberFormat);
		}

		protected override ulong StringToValue(string str)
		{
			ulong num;
			return UINumericFieldsUtils.TryConvertStringToULong(str, base.textInputBase.originalText, out num) ? num : base.rawValue;
		}

		public UnsignedLongField()
			: this(null, 1000)
		{
		}

		public UnsignedLongField(int maxLength)
			: this(null, maxLength)
		{
		}

		public UnsignedLongField(string label, int maxLength = 1000)
			: base(label, maxLength, new UnsignedLongField.UnsignedLongInput())
		{
			base.AddToClassList(UnsignedLongField.ussClassName);
			base.labelElement.AddToClassList(UnsignedLongField.labelUssClassName);
			base.visualInput.AddToClassList(UnsignedLongField.inputUssClassName);
			base.AddLabelDragger<ulong>();
		}

		internal override bool CanTryParse(string textString)
		{
			ulong num;
			return ulong.TryParse(textString, out num);
		}

		public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, ulong startValue)
		{
			this.unsignedLongInput.ApplyInputDeviceDelta(delta, speed, startValue);
		}

		public new static readonly string ussClassName = "unity-unsigned-long-field";

		public new static readonly string labelUssClassName = UnsignedLongField.ussClassName + "__label";

		public new static readonly string inputUssClassName = UnsignedLongField.ussClassName + "__input";

		public new class UxmlFactory : UxmlFactory<UnsignedLongField, UnsignedLongField.UxmlTraits>
		{
		}

		public new class UxmlTraits : TextValueFieldTraits<ulong, UxmlUnsignedLongAttributeDescription>
		{
		}

		private class UnsignedLongInput : TextValueField<ulong>.TextValueInput
		{
			private UnsignedLongField parentUnsignedLongField
			{
				get
				{
					return (UnsignedLongField)base.parent;
				}
			}

			internal UnsignedLongInput()
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

			public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, ulong startValue)
			{
				double num = NumericFieldDraggerUtility.CalculateIntDragSensitivity(startValue);
				float num2 = NumericFieldDraggerUtility.Acceleration(speed == DeltaSpeed.Fast, speed == DeltaSpeed.Slow);
				ulong num3 = this.StringToValue(base.text);
				long num4 = (long)Math.Round((double)NumericFieldDraggerUtility.NiceDelta(delta, num2) * num);
				num3 = this.ClampToMinMaxULongValue(num4, num3);
				bool isDelayed = this.parentUnsignedLongField.isDelayed;
				if (isDelayed)
				{
					base.text = this.ValueToString(num3);
				}
				else
				{
					this.parentUnsignedLongField.value = num3;
				}
			}

			private ulong ClampToMinMaxULongValue(long niceDelta, ulong value)
			{
				ulong num = (ulong)Math.Abs(niceDelta);
				bool flag = niceDelta > 0L;
				ulong num2;
				if (flag)
				{
					bool flag2 = num > ulong.MaxValue - value;
					if (flag2)
					{
						num2 = ulong.MaxValue;
					}
					else
					{
						num2 = value + num;
					}
				}
				else
				{
					bool flag3 = num > value;
					if (flag3)
					{
						num2 = 0UL;
					}
					else
					{
						num2 = value - num;
					}
				}
				return num2;
			}

			protected override string ValueToString(ulong v)
			{
				return v.ToString(base.formatString);
			}

			protected override ulong StringToValue(string str)
			{
				ulong num;
				UINumericFieldsUtils.TryConvertStringToULong(str, base.originalText, out num);
				return num;
			}
		}
	}
}
