using System;
using System.Diagnostics;
using System.Globalization;
using UnityEngine.Internal;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.UIElements
{
	[MovedFrom(true, "UnityEditor.UIElements", "UnityEditor.UIElementsModule", null)]
	public class FloatField : TextValueField<float>
	{
		private FloatField.FloatInput floatInput
		{
			get
			{
				return (FloatField.FloatInput)base.textInputBase;
			}
		}

		protected override string ValueToString(float v)
		{
			return v.ToString(base.formatString, CultureInfo.InvariantCulture.NumberFormat);
		}

		protected override float StringToValue(string str)
		{
			float num;
			ExpressionEvaluator.Expression expression;
			bool flag = UINumericFieldsUtils.TryConvertStringToFloat(str, base.textInputBase.originalText, out num, out expression);
			Action<ExpressionEvaluator.Expression> expressionEvaluated = this.expressionEvaluated;
			if (expressionEvaluated != null)
			{
				expressionEvaluated(expression);
			}
			return flag ? num : base.rawValue;
		}

		internal override void UpdateValueFromText()
		{
			string text = this.ValueToString(base.rawValue);
			bool flag = text != base.text;
			if (flag)
			{
				base.UpdateValueFromText();
			}
		}

		public FloatField()
			: this(null, 1000)
		{
		}

		public FloatField(int maxLength)
			: this(null, maxLength)
		{
		}

		public FloatField(string label, int maxLength = 1000)
			: base(label, maxLength, new FloatField.FloatInput())
		{
			base.AddToClassList(FloatField.ussClassName);
			base.labelElement.AddToClassList(FloatField.labelUssClassName);
			base.visualInput.AddToClassList(FloatField.inputUssClassName);
			base.AddLabelDragger<float>();
		}

		internal override bool CanTryParse(string textString)
		{
			float num;
			return float.TryParse(textString, out num);
		}

		public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, float startValue)
		{
			this.floatInput.ApplyInputDeviceDelta(delta, speed, startValue);
		}

		public new static readonly string ussClassName = "unity-float-field";

		public new static readonly string labelUssClassName = FloatField.ussClassName + "__label";

		public new static readonly string inputUssClassName = FloatField.ussClassName + "__input";

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : TextValueField<float>.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				TextValueField<float>.UxmlSerializedData.Register();
			}

			public override object CreateInstance()
			{
				return new FloatField();
			}
		}

		[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlFactory : UxmlFactory<FloatField, FloatField.UxmlTraits>
		{
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlTraits : TextValueFieldTraits<float, UxmlFloatAttributeDescription>
		{
		}

		private class FloatInput : TextValueField<float>.TextValueInput
		{
			private FloatField parentFloatField
			{
				get
				{
					return (FloatField)base.parent;
				}
			}

			internal FloatInput()
			{
				base.formatString = UINumericFieldsUtils.k_FloatFieldFormatString;
			}

			protected override string allowedCharacters
			{
				get
				{
					return this.parentFloatField.supportExpressions ? UINumericFieldsUtils.k_AllowedCharactersForFloat : UINumericFieldsUtils.k_AllowedCharactersForFloat_NoExpressions;
				}
			}

			public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, float startValue)
			{
				double num = NumericFieldDraggerUtility.CalculateFloatDragSensitivity((double)startValue);
				float num2 = NumericFieldDraggerUtility.Acceleration(speed == DeltaSpeed.Fast, speed == DeltaSpeed.Slow);
				double num3 = (double)this.StringToValue(base.text);
				num3 += (double)NumericFieldDraggerUtility.NiceDelta(delta, num2) * num;
				num3 = Mathf.RoundBasedOnMinimumDifference(num3, num);
				bool isDelayed = this.parentFloatField.isDelayed;
				if (isDelayed)
				{
					base.text = this.ValueToString(Mathf.ClampToFloat(num3));
				}
				else
				{
					this.parentFloatField.value = Mathf.ClampToFloat(num3);
				}
			}

			protected override string ValueToString(float v)
			{
				return v.ToString(base.formatString);
			}

			protected override float StringToValue(string str)
			{
				return this.parentFloatField.StringToValue(str);
			}
		}
	}
}
