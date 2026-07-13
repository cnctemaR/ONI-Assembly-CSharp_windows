using System;
using System.Diagnostics;
using System.Globalization;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
	public class UnsignedIntegerField : TextValueField<uint>
	{
		private UnsignedIntegerField.UnsignedIntegerInput integerInput
		{
			get
			{
				return (UnsignedIntegerField.UnsignedIntegerInput)base.textInputBase;
			}
		}

		protected override string ValueToString(uint v)
		{
			return v.ToString(base.formatString, CultureInfo.InvariantCulture.NumberFormat);
		}

		protected override uint StringToValue(string str)
		{
			uint num;
			ExpressionEvaluator.Expression expression;
			bool flag = UINumericFieldsUtils.TryConvertStringToUInt(str, base.textInputBase.originalText, out num, out expression);
			Action<ExpressionEvaluator.Expression> expressionEvaluated = this.expressionEvaluated;
			if (expressionEvaluated != null)
			{
				expressionEvaluated(expression);
			}
			return flag ? num : base.rawValue;
		}

		public UnsignedIntegerField()
			: this(null, 1000)
		{
		}

		public UnsignedIntegerField(int maxLength)
			: this(null, maxLength)
		{
		}

		public UnsignedIntegerField(string label, int maxLength = 1000)
			: base(label, maxLength, new UnsignedIntegerField.UnsignedIntegerInput())
		{
			base.AddToClassList(UnsignedIntegerField.ussClassName);
			base.labelElement.AddToClassList(UnsignedIntegerField.labelUssClassName);
			base.visualInput.AddToClassList(UnsignedIntegerField.inputUssClassName);
			base.AddLabelDragger<uint>();
		}

		internal override bool CanTryParse(string textString)
		{
			uint num;
			return uint.TryParse(textString, out num);
		}

		public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, uint startValue)
		{
			this.integerInput.ApplyInputDeviceDelta(delta, speed, startValue);
		}

		public new static readonly string ussClassName = "unity-unsigned-integer-field";

		public new static readonly string labelUssClassName = UnsignedIntegerField.ussClassName + "__label";

		public new static readonly string inputUssClassName = UnsignedIntegerField.ussClassName + "__input";

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : TextValueField<uint>.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				TextValueField<uint>.UxmlSerializedData.Register();
			}

			public override object CreateInstance()
			{
				return new UnsignedIntegerField();
			}
		}

		[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlFactory : UxmlFactory<UnsignedIntegerField, UnsignedIntegerField.UxmlTraits>
		{
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlTraits : TextValueFieldTraits<uint, UxmlUnsignedIntAttributeDescription>
		{
		}

		private class UnsignedIntegerInput : TextValueField<uint>.TextValueInput
		{
			private UnsignedIntegerField parentUnsignedIntegerField
			{
				get
				{
					return (UnsignedIntegerField)base.parent;
				}
			}

			internal UnsignedIntegerInput()
			{
				base.formatString = UINumericFieldsUtils.k_IntFieldFormatString;
			}

			protected override string allowedCharacters
			{
				get
				{
					return this.parentUnsignedIntegerField.supportExpressions ? UINumericFieldsUtils.k_AllowedCharactersForInt : UINumericFieldsUtils.k_AllowedCharactersForUInt_NoExpressions;
				}
			}

			public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, uint startValue)
			{
				double num = (double)NumericFieldDraggerUtility.CalculateIntDragSensitivity((long)((ulong)startValue));
				float num2 = NumericFieldDraggerUtility.Acceleration(speed == DeltaSpeed.Fast, speed == DeltaSpeed.Slow);
				long num3 = (long)((ulong)this.StringToValue(base.text));
				num3 += (long)Math.Round((double)NumericFieldDraggerUtility.NiceDelta(delta, num2) * num);
				bool isDelayed = this.parentUnsignedIntegerField.isDelayed;
				if (isDelayed)
				{
					base.text = this.ValueToString(Mathf.ClampToUInt(num3));
				}
				else
				{
					this.parentUnsignedIntegerField.value = Mathf.ClampToUInt(num3);
				}
			}

			protected override string ValueToString(uint v)
			{
				return v.ToString(base.formatString);
			}

			protected override uint StringToValue(string str)
			{
				return this.parentUnsignedIntegerField.StringToValue(str);
			}
		}
	}
}
