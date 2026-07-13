using System;
using System.Diagnostics;
using System.Globalization;
using UnityEngine.Internal;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.UIElements
{
	[MovedFrom(true, "UnityEditor.UIElements", "UnityEditor.UIElementsModule", null)]
	public class DoubleField : TextValueField<double>
	{
		private DoubleField.DoubleInput doubleInput
		{
			get
			{
				return (DoubleField.DoubleInput)base.textInputBase;
			}
		}

		protected override string ValueToString(double v)
		{
			return v.ToString(base.formatString, CultureInfo.InvariantCulture.NumberFormat);
		}

		protected override double StringToValue(string str)
		{
			double num;
			ExpressionEvaluator.Expression expression;
			bool flag = UINumericFieldsUtils.TryConvertStringToDouble(str, base.textInputBase.originalText, out num, out expression);
			Action<ExpressionEvaluator.Expression> expressionEvaluated = this.expressionEvaluated;
			if (expressionEvaluated != null)
			{
				expressionEvaluated(expression);
			}
			return flag ? num : base.rawValue;
		}

		public DoubleField()
			: this(null, 1000)
		{
		}

		public DoubleField(int maxLength)
			: this(null, maxLength)
		{
		}

		public DoubleField(string label, int maxLength = 1000)
			: base(label, maxLength, new DoubleField.DoubleInput())
		{
			base.AddToClassList(DoubleField.ussClassName);
			base.labelElement.AddToClassList(DoubleField.labelUssClassName);
			base.visualInput.AddToClassList(DoubleField.inputUssClassName);
			base.AddLabelDragger<double>();
		}

		internal override bool CanTryParse(string textString)
		{
			double num;
			return double.TryParse(textString, out num);
		}

		public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, double startValue)
		{
			this.doubleInput.ApplyInputDeviceDelta(delta, speed, startValue);
		}

		public new static readonly string ussClassName = "unity-double-field";

		public new static readonly string labelUssClassName = DoubleField.ussClassName + "__label";

		public new static readonly string inputUssClassName = DoubleField.ussClassName + "__input";

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : TextValueField<double>.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				TextValueField<double>.UxmlSerializedData.Register();
			}

			public override object CreateInstance()
			{
				return new DoubleField();
			}
		}

		[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlFactory : UxmlFactory<DoubleField, DoubleField.UxmlTraits>
		{
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlTraits : TextValueFieldTraits<double, UxmlDoubleAttributeDescription>
		{
		}

		private class DoubleInput : TextValueField<double>.TextValueInput
		{
			private DoubleField parentDoubleField
			{
				get
				{
					return (DoubleField)base.parent;
				}
			}

			internal DoubleInput()
			{
				base.formatString = UINumericFieldsUtils.k_DoubleFieldFormatString;
			}

			protected override string allowedCharacters
			{
				get
				{
					return this.parentDoubleField.supportExpressions ? UINumericFieldsUtils.k_AllowedCharactersForFloat : UINumericFieldsUtils.k_AllowedCharactersForFloat_NoExpressions;
				}
			}

			public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, double startValue)
			{
				double num = NumericFieldDraggerUtility.CalculateFloatDragSensitivity(startValue);
				float num2 = NumericFieldDraggerUtility.Acceleration(speed == DeltaSpeed.Fast, speed == DeltaSpeed.Slow);
				double num3 = this.StringToValue(base.text);
				num3 += (double)NumericFieldDraggerUtility.NiceDelta(delta, num2) * num;
				num3 = Mathf.RoundBasedOnMinimumDifference(num3, num);
				bool isDelayed = this.parentDoubleField.isDelayed;
				if (isDelayed)
				{
					base.text = this.ValueToString(num3);
				}
				else
				{
					this.parentDoubleField.value = num3;
				}
			}

			protected override string ValueToString(double v)
			{
				return v.ToString(base.formatString);
			}

			protected override double StringToValue(string str)
			{
				return this.parentDoubleField.StringToValue(str);
			}
		}
	}
}
