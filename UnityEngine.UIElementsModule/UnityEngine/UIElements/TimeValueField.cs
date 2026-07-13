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
	internal class TimeValueField : TextValueField<TimeValue>
	{
		private TimeValueField.TimeValueInput timeValueInput
		{
			get
			{
				return (TimeValueField.TimeValueInput)base.textInputBase;
			}
		}

		[UxmlAttribute]
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
					base.NotifyPropertyChanged(in TimeValueField.showUnitAsDropdownProperty);
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

		public TimeValueField()
			: this(null, 1000)
		{
		}

		public TimeValueField(int maxTimeValue)
			: this(null, maxTimeValue)
		{
		}

		public TimeValueField(string label, int maxTimeValue = 1000)
			: base(label, maxTimeValue, new TimeValueField.TimeValueInput())
		{
			base.AddToClassList(TimeValueField.ussClassName);
			base.AddToClassList(TimeValueField.timeValueFieldUssClassName);
			base.AddLabelDragger<TimeValue>();
			VisualElement visualElement = new VisualElement();
			visualElement.name = TimeValueField.unitDropdownContainerUssClass;
			visualElement.AddToClassList(TimeValueField.unitDropdownContainerUssClass);
			this.m_AllOptionsList.AddRange(TimeValueField.KLDefaultUnits);
			this.m_AllOptionsList.AddRange(TimeValueField.AllKeywords);
			this.m_OptionsPopup = new PopupField<string>(this.m_AllOptionsList, 0, new Func<string, string>(TimeValueField.OnFormatSelectedValue), null);
			this.m_OptionsPopup.AddToClassList(TimeValueField.unitDropdownUssClass);
			visualElement.Add(this.m_OptionsPopup);
			this.timeValueInput.parentTimeValueField = this;
			this.timeValueInput.AddToClassList(TimeValueField.inputUssClassName);
			this.timeValueInput.delegatesFocus = true;
			base.Add(visualElement);
			this.m_OptionsPopup.RegisterValueChangedCallback<string>(new EventCallback<ChangeEvent<string>>(this.OnPopupFieldValueChange));
			this.UpdateFields();
			this.showUnitAsDropdown = true;
		}

		public override void SetValueWithoutNotify(TimeValue newValue)
		{
			base.SetValueWithoutNotify(newValue);
			this.SetOptionsPopupFromValue();
		}

		public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, TimeValue startValue)
		{
			this.timeValueInput.ApplyInputDeviceDelta(delta, speed, startValue);
		}

		protected override string ValueToString(TimeValue v)
		{
			return this.showUnitAsDropdown ? v.value.ToString(base.formatString, CultureInfo.InvariantCulture.NumberFormat) : v.ToString();
		}

		protected override TimeValue StringToValue(string str)
		{
			ReadOnlySpan<char> readOnlySpan = str.AsSpan().Trim();
			TimeUnit timeUnit = this.value.unit;
			bool flag = timeUnit != TimeUnit.Second && timeUnit != TimeUnit.Millisecond;
			if (flag)
			{
				timeUnit = TimeUnit.Second;
			}
			bool flag2 = readOnlySpan.EndsWith("ms", StringComparison.Ordinal);
			if (flag2)
			{
				ReadOnlySpan<char> readOnlySpan2 = readOnlySpan;
				int num = "ms".Length;
				readOnlySpan = readOnlySpan2.Slice(0, readOnlySpan2.Length - num);
				timeUnit = TimeUnit.Millisecond;
			}
			else
			{
				bool flag3 = readOnlySpan.EndsWith("s", StringComparison.Ordinal);
				if (flag3)
				{
					ReadOnlySpan<char> readOnlySpan2 = readOnlySpan;
					int num = "s".Length;
					readOnlySpan = readOnlySpan2.Slice(0, readOnlySpan2.Length - num);
					timeUnit = TimeUnit.Second;
				}
			}
			float num2;
			ExpressionEvaluator.Expression expression;
			return UINumericFieldsUtils.TryConvertStringToFloat(readOnlySpan.ToString(), base.textInputBase.originalText, out num2, out expression) ? new TimeValue(num2, timeUnit) : this.value;
		}

		internal override bool CanTryParse(string textString)
		{
			double num;
			return double.TryParse(textString, out num);
		}

		private void UpdateFields()
		{
			base.text = this.ValueToString(this.value);
			this.m_OptionsPopup.EnableInClassList(TimeValueField.invisibleUnitDropdownUssClass, !this.showUnitAsDropdown);
		}

		private static string OnFormatSelectedValue(string value)
		{
			return (Array.IndexOf<string>(TimeValueField.AllKeywords, value) < 0) ? value : TimeValueField.s_NoOptionString;
		}

		private void SetOptionsPopupFromValue()
		{
			string text = this.value.unit.ToDisplayString();
			bool flag = string.Compare(text, TimeValueField.s_NoOptionString, StringComparison.OrdinalIgnoreCase) != 0;
			bool flag2 = flag;
			if (flag2)
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
				TimeValue timeValue;
				if (!(newValue == "s"))
				{
					if (!(newValue == "ms"))
					{
						timeValue = this.value;
					}
					else
					{
						timeValue = new TimeValue(this.value.value, TimeUnit.Millisecond);
					}
				}
				else
				{
					timeValue = new TimeValue(this.value.value, TimeUnit.Second);
				}
				if (!true)
				{
				}
				this.value = timeValue;
				evt.StopImmediatePropagation();
			}
		}

		public static readonly BindingId showUnitAsDropdownProperty = "showUnitAsDropdown";

		public new static readonly string ussClassName = "unity-style-field";

		public static readonly string timeValueFieldUssClassName = "unity-time-value-field";

		public new static readonly string inputUssClassName = TimeValueField.ussClassName + "__visual-input";

		public static readonly string unitDropdownContainerUssClass = TimeValueField.ussClassName + "__options-popup-container";

		public static readonly string unitDropdownUssClass = TimeValueField.ussClassName + "__options-popup";

		public static readonly string invisibleUnitDropdownUssClass = TimeValueField.unitDropdownUssClass + "--invisible";

		public static readonly string KeywordInitial = "initial";

		public const string UnitSecond = "s";

		public const string UnitMillisecond = "ms";

		private static readonly string[] KLDefaultUnits = new string[] { "s", "ms" };

		private static readonly string[] AllKeywords = new string[] { TimeValueField.KeywordInitial };

		internal static readonly string s_NoOptionString = "-";

		private bool m_ShowUnitAsDropdown;

		private readonly PopupField<string> m_OptionsPopup;

		private readonly List<string> m_AllOptionsList = new List<string>();

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : TextValueField<TimeValue>.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			[RegisterUxmlCache]
			public new static void Register()
			{
				TextValueField<TimeValue>.UxmlSerializedData.Register();
				UxmlDescriptionCache.RegisterType(typeof(TimeValueField.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("showUnitAsDropdown", "show-unit-as-dropdown", null, Array.Empty<string>())
				}, false);
			}

			public override object CreateInstance()
			{
				return new TimeValueField();
			}

			public override void Deserialize(object obj)
			{
				base.Deserialize(obj);
				TimeValueField timeValueField = (TimeValueField)obj;
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.showUnitAsDropdown_UxmlAttributeFlags);
				if (flag)
				{
					timeValueField.showUnitAsDropdown = this.showUnitAsDropdown;
				}
			}

			[SerializeField]
			private bool showUnitAsDropdown;

			[SerializeField]
			[HideInInspector]
			[UxmlIgnore]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags showUnitAsDropdown_UxmlAttributeFlags;
		}

		private class TimeValueInput : TextValueField<TimeValue>.TextValueInput
		{
			internal TimeValueField parentTimeValueField { get; set; }

			internal TimeValueInput()
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

			public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, TimeValue startValue)
			{
				TimeValue timeValue = this.StringToValue(base.text);
				timeValue.unit = startValue.unit;
				double num = (double)timeValue.value;
				double num2 = (double)NumericFieldDraggerUtility.CalculateIntDragSensitivity((long)startValue.value);
				float num3 = NumericFieldDraggerUtility.Acceleration(speed == DeltaSpeed.Fast, speed == DeltaSpeed.Slow);
				num += (double)NumericFieldDraggerUtility.NiceDelta(delta, num3) * num2;
				num = Mathf.RoundBasedOnMinimumDifference(num, num2);
				timeValue = new TimeValue((float)num, timeValue.unit);
				bool isDelayed = this.parentTimeValueField.isDelayed;
				if (isDelayed)
				{
					this.parentTimeValueField.text = this.ValueToString(timeValue);
				}
				else
				{
					this.parentTimeValueField.value = timeValue;
				}
			}

			protected override string ValueToString(TimeValue v)
			{
				return this.parentTimeValueField.showUnitAsDropdown ? v.value.ToString(CultureInfo.InvariantCulture) : v.ToString();
			}

			protected override TimeValue StringToValue(string str)
			{
				TimeValue timeValue;
				return TimeValue.TryParseString(str, out timeValue) ? timeValue : this.parentTimeValueField.value;
			}
		}
	}
}
