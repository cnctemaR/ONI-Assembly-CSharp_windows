using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal class AngleField : TextValueField<Angle>
	{
		private AngleField.AngleInput angleInput
		{
			get
			{
				return (AngleField.AngleInput)base.textInputBase;
			}
		}

		[CreateProperty]
		public bool showUnitAsDropdown
		{
			get
			{
				return this.m_ShowUnitAsDropdown;
			}
			set
			{
				bool flag = this.m_ShowUnitAsDropdown == value;
				if (!flag)
				{
					this.m_ShowUnitAsDropdown = value;
					this.UpdateFields();
					base.NotifyPropertyChanged(in AngleField.showUnitAsDropdownProperty);
				}
			}
		}

		protected internal PopupField<string> optionsPopup
		{
			get
			{
				return this.m_OptionsPopup;
			}
		}

		public AngleField()
			: this(null, 1000)
		{
		}

		public AngleField(int maxAngle)
			: this(null, maxAngle)
		{
		}

		public AngleField(string label, int maxAngle = 1000)
			: base(label, maxAngle, new AngleField.AngleInput())
		{
			base.AddToClassList(AngleField.ussClassName);
			base.AddToClassList(AngleField.angleFieldUssClassName);
			base.AddLabelDragger<Angle>();
			VisualElement visualElement = new VisualElement();
			visualElement.name = AngleField.unitDropdownContainerUssClass;
			visualElement.AddToClassList(AngleField.unitDropdownContainerUssClass);
			this.m_AllOptionsList.AddRange(AngleField.KLDefaultUnits);
			this.m_AllOptionsList.AddRange(AngleField.AllKeywords);
			this.m_OptionsPopup = new PopupField<string>(this.m_AllOptionsList, 0, new Func<string, string>(AngleField.OnFormatSelectedValue), null);
			this.m_OptionsPopup.AddToClassList(AngleField.unitDropdownUssClass);
			visualElement.Add(this.m_OptionsPopup);
			this.angleInput.parentAngleField = this;
			this.angleInput.AddToClassList(AngleField.inputUssClassName);
			this.angleInput.delegatesFocus = true;
			base.Add(visualElement);
			this.m_OptionsPopup.RegisterValueChangedCallback<string>(new EventCallback<ChangeEvent<string>>(this.OnPopupFieldValueChange));
			this.UpdateFields();
			this.showUnitAsDropdown = true;
		}

		public override void SetValueWithoutNotify(Angle newValue)
		{
			base.SetValueWithoutNotify(newValue);
			this.SetOptionsPopupFromValue();
		}

		public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, Angle startValue)
		{
			this.angleInput.ApplyInputDeviceDelta(delta, speed, startValue);
		}

		protected override string ValueToString(Angle v)
		{
			bool flag = this.showUnitAsDropdown && !v.IsNone();
			string text;
			if (flag)
			{
				text = v.value.ToString(base.formatString, CultureInfo.InvariantCulture.NumberFormat);
			}
			else
			{
				text = v.ToString();
			}
			return text;
		}

		protected override Angle StringToValue(string str)
		{
			ReadOnlySpan<char> readOnlySpan = str.AsSpan().Trim();
			bool flag = readOnlySpan.Equals(AngleField.KeywordNone, StringComparison.OrdinalIgnoreCase);
			Angle angle;
			if (flag)
			{
				angle = Angle.None();
			}
			else
			{
				AngleUnit angleUnit = this.value.unit;
				bool flag2 = angleUnit != AngleUnit.Degree && angleUnit != AngleUnit.Gradian && angleUnit != AngleUnit.Radian && angleUnit != AngleUnit.Turn;
				if (flag2)
				{
					angleUnit = AngleUnit.Degree;
				}
				bool flag3 = readOnlySpan.EndsWith("deg", StringComparison.Ordinal);
				if (flag3)
				{
					ReadOnlySpan<char> readOnlySpan2 = readOnlySpan;
					int num = "deg".Length;
					readOnlySpan = readOnlySpan2.Slice(0, readOnlySpan2.Length - num);
					angleUnit = AngleUnit.Degree;
				}
				else
				{
					bool flag4 = readOnlySpan.EndsWith("grad", StringComparison.Ordinal);
					if (flag4)
					{
						ReadOnlySpan<char> readOnlySpan2 = readOnlySpan;
						int num = "grad".Length;
						readOnlySpan = readOnlySpan2.Slice(0, readOnlySpan2.Length - num);
						angleUnit = AngleUnit.Gradian;
					}
					else
					{
						bool flag5 = readOnlySpan.EndsWith("rad", StringComparison.Ordinal);
						if (flag5)
						{
							ReadOnlySpan<char> readOnlySpan2 = readOnlySpan;
							int num = "rad".Length;
							readOnlySpan = readOnlySpan2.Slice(0, readOnlySpan2.Length - num);
							angleUnit = AngleUnit.Radian;
						}
						else
						{
							bool flag6 = readOnlySpan.EndsWith("turn", StringComparison.Ordinal);
							if (flag6)
							{
								ReadOnlySpan<char> readOnlySpan2 = readOnlySpan;
								int num = "turn".Length;
								readOnlySpan = readOnlySpan2.Slice(0, readOnlySpan2.Length - num);
								angleUnit = AngleUnit.Turn;
							}
						}
					}
				}
				float num2;
				ExpressionEvaluator.Expression expression;
				angle = (UINumericFieldsUtils.TryConvertStringToFloat(readOnlySpan.ToString(), base.textInputBase.originalText, out num2, out expression) ? new Angle(num2, angleUnit) : this.value);
			}
			return angle;
		}

		internal override bool CanTryParse(string textString)
		{
			double num;
			return double.TryParse(textString, out num);
		}

		private void UpdateFields()
		{
			base.text = this.ValueToString(this.value);
			this.m_OptionsPopup.EnableInClassList(AngleField.invisibleUnitDropdownUssClass, !this.showUnitAsDropdown);
		}

		private static string OnFormatSelectedValue(string value)
		{
			return (Array.IndexOf<string>(AngleField.AllKeywords, value) < 0) ? value : AngleField.s_NoOptionString;
		}

		private void SetOptionsPopupFromValue()
		{
			bool flag = this.value.IsNone();
			if (flag)
			{
				this.m_OptionsPopup.SetValueWithoutNotify(AngleField.KeywordNone);
			}
			string text = this.value.unit.ToDisplayString();
			bool flag2 = string.Compare(text, AngleField.s_NoOptionString, StringComparison.OrdinalIgnoreCase) != 0;
			bool flag3 = flag2;
			if (flag3)
			{
				this.m_OptionsPopup.SetValueWithoutNotify(text);
			}
		}

		private void OnPopupFieldValueChange(ChangeEvent<string> evt)
		{
			bool flag = evt.target != this.optionsPopup;
			if (flag)
			{
				evt.StopImmediatePropagation();
			}
			else
			{
				string newValue = evt.newValue;
				if (!true)
				{
				}
				Angle angle;
				if (!(newValue == "deg"))
				{
					if (!(newValue == "grad"))
					{
						if (!(newValue == "rad"))
						{
							if (!(newValue == "turn"))
							{
								angle = this.value;
							}
							else
							{
								angle = new Angle(this.value.value, AngleUnit.Turn);
							}
						}
						else
						{
							angle = new Angle(this.value.value, AngleUnit.Radian);
						}
					}
					else
					{
						angle = new Angle(this.value.value, AngleUnit.Gradian);
					}
				}
				else
				{
					angle = new Angle(this.value.value, AngleUnit.Degree);
				}
				if (!true)
				{
				}
				this.value = angle;
				evt.StopImmediatePropagation();
			}
		}

		public static readonly BindingId showUnitAsDropdownProperty = "showUnitAsDropdown";

		public new static readonly string ussClassName = "unity-style-field";

		public static readonly string angleFieldUssClassName = "unity-angle-field";

		public new static readonly string inputUssClassName = AngleField.ussClassName + "__visual-input";

		public static readonly string unitDropdownContainerUssClass = AngleField.ussClassName + "__options-popup-container";

		public static readonly string unitDropdownUssClass = AngleField.ussClassName + "__options-popup";

		public static readonly string invisibleUnitDropdownUssClass = AngleField.unitDropdownUssClass + "--invisible";

		public static readonly string KeywordInitial = "initial";

		public static readonly string KeywordNone = "none";

		public const string UnitDegree = "deg";

		public const string UnitGrad = "grad";

		public const string UnitRad = "rad";

		public const string UnitTurn = "turn";

		private static readonly string[] KLDefaultUnits = new string[] { "deg", "grad", "rad", "turn" };

		private static readonly string[] AllKeywords = new string[]
		{
			AngleField.KeywordNone,
			AngleField.KeywordInitial
		};

		internal static readonly string s_NoOptionString = "-";

		private bool m_ShowUnitAsDropdown;

		private readonly PopupField<string> m_OptionsPopup;

		private readonly List<string> m_AllOptionsList = new List<string>();

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : TextValueField<Angle>.UxmlSerializedData
		{
			[RegisterUxmlCache]
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				TextValueField<Angle>.UxmlSerializedData.Register();
				UxmlDescriptionCache.RegisterType(typeof(AngleField.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("showUnitAsDropdown", "show-unit-as-dropdown", null, Array.Empty<string>())
				}, false);
			}

			public override object CreateInstance()
			{
				return new AngleField();
			}

			public override void Deserialize(object obj)
			{
				base.Deserialize(obj);
				AngleField angleField = (AngleField)obj;
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.showUnitAsDropdown_UxmlAttributeFlags);
				if (flag)
				{
					angleField.showUnitAsDropdown = this.showUnitAsDropdown;
				}
			}

			[SerializeField]
			private bool showUnitAsDropdown;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags showUnitAsDropdown_UxmlAttributeFlags;
		}

		private class AngleInput : TextValueField<Angle>.TextValueInput
		{
			internal AngleField parentAngleField { get; set; }

			internal AngleInput()
			{
				base.formatString = UINumericFieldsUtils.k_DoubleFieldFormatString;
			}

			protected override string allowedCharacters
			{
				get
				{
					return UINumericFieldsUtils.k_AllowedCharactersForFloat;
				}
			}

			public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, Angle startValue)
			{
				Angle angle = this.StringToValue(base.text);
				angle.unit = startValue.unit;
				bool flag = angle.IsNone();
				if (flag)
				{
					angle = new Angle(0f);
				}
				double num = (double)angle.value;
				double num2 = (double)NumericFieldDraggerUtility.CalculateIntDragSensitivity((long)startValue.value);
				float num3 = NumericFieldDraggerUtility.Acceleration(speed == DeltaSpeed.Fast, speed == DeltaSpeed.Slow);
				num += (double)NumericFieldDraggerUtility.NiceDelta(delta, num3) * num2;
				num = Mathf.RoundBasedOnMinimumDifference(num, num2);
				angle = new Angle((float)num, angle.unit);
				bool isDelayed = this.parentAngleField.isDelayed;
				if (isDelayed)
				{
					this.parentAngleField.text = this.ValueToString(angle);
				}
				else
				{
					this.parentAngleField.value = angle;
				}
			}

			protected override string ValueToString(Angle v)
			{
				return this.parentAngleField.showUnitAsDropdown ? v.value.ToString(CultureInfo.InvariantCulture) : v.ToString();
			}

			protected override Angle StringToValue(string str)
			{
				Angle angle;
				return Angle.TryParseString(str, out angle) ? angle : this.parentAngleField.value;
			}
		}
	}
}
