using System;
using System.Diagnostics;
using System.Globalization;
using UnityEngine.Internal;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.UIElements
{
	[MovedFrom(true, "UnityEditor.UIElements", "UnityEditor.UIElementsModule", null)]
	public class IntegerField : TextValueField<int>
	{
		private IntegerField.IntegerInput integerInput
		{
			get
			{
				return (IntegerField.IntegerInput)base.textInputBase;
			}
		}

		protected override string ValueToString(int v)
		{
			return v.ToString(base.formatString, CultureInfo.InvariantCulture.NumberFormat);
		}

		protected override int StringToValue(string str)
		{
			int num;
			ExpressionEvaluator.Expression expression;
			bool flag = UINumericFieldsUtils.TryConvertStringToInt(str, base.textInputBase.originalText, out num, out expression);
			Action<ExpressionEvaluator.Expression> expressionEvaluated = this.expressionEvaluated;
			if (expressionEvaluated != null)
			{
				expressionEvaluated(expression);
			}
			return flag ? num : base.rawValue;
		}

		public IntegerField()
			: this(null, 1000)
		{
		}

		public IntegerField(int maxLength)
			: this(null, maxLength)
		{
		}

		public IntegerField(string label, int maxLength = 1000)
			: base(label, maxLength, new IntegerField.IntegerInput())
		{
			base.AddToClassList(IntegerField.ussClassName);
			base.labelElement.AddToClassList(IntegerField.labelUssClassName);
			base.visualInput.AddToClassList(IntegerField.inputUssClassName);
			base.AddLabelDragger<int>();
		}

		internal override bool CanTryParse(string textString)
		{
			int num;
			return int.TryParse(textString, out num);
		}

		public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, int startValue)
		{
			this.integerInput.ApplyInputDeviceDelta(delta, speed, startValue);
		}

		public new static readonly string ussClassName = "unity-integer-field";

		public new static readonly string labelUssClassName = IntegerField.ussClassName + "__label";

		public new static readonly string inputUssClassName = IntegerField.ussClassName + "__input";

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : TextValueField<int>.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				TextValueField<int>.UxmlSerializedData.Register();
			}

			public override object CreateInstance()
			{
				return new IntegerField();
			}
		}

		[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlFactory : UxmlFactory<IntegerField, IntegerField.UxmlTraits>
		{
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlTraits : TextValueFieldTraits<int, UxmlIntAttributeDescription>
		{
		}

		private class IntegerInput : TextValueField<int>.TextValueInput
		{
			private IntegerField parentIntegerField
			{
				get
				{
					return (IntegerField)base.parent;
				}
			}

			internal IntegerInput()
			{
				base.formatString = UINumericFieldsUtils.k_IntFieldFormatString;
			}

			protected override string allowedCharacters
			{
				get
				{
					return this.parentIntegerField.supportExpressions ? UINumericFieldsUtils.k_AllowedCharactersForInt : UINumericFieldsUtils.k_AllowedCharactersForInt_NoExpressions;
				}
			}

			public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, int startValue)
			{
				double num = (double)NumericFieldDraggerUtility.CalculateIntDragSensitivity((long)startValue);
				float num2 = NumericFieldDraggerUtility.Acceleration(speed == DeltaSpeed.Fast, speed == DeltaSpeed.Slow);
				long num3 = (long)this.StringToValue(base.text);
				num3 += (long)Math.Round((double)NumericFieldDraggerUtility.NiceDelta(delta, num2) * num);
				bool isDelayed = this.parentIntegerField.isDelayed;
				if (isDelayed)
				{
					base.text = this.ValueToString(Mathf.ClampToInt(num3));
				}
				else
				{
					this.parentIntegerField.value = Mathf.ClampToInt(num3);
				}
			}

			protected override string ValueToString(int v)
			{
				return v.ToString(base.formatString);
			}

			protected override int StringToValue(string str)
			{
				return this.parentIntegerField.StringToValue(str);
			}
		}
	}
}
