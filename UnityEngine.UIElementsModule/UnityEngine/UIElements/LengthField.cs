using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Pool;
using UnityEngine.UIElements.StyleSheets;
using UnityEngine.UIElements.StyleSheets.Syntax;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal class LengthField : TextValueField<Length>
	{
		private LengthField.LengthInput lengthInput
		{
			get
			{
				return (LengthField.LengthInput)base.textInputBase;
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
					base.NotifyPropertyChanged(in LengthField.showUnitAsDropdownProperty);
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

		protected List<string> styleKeywords
		{
			get
			{
				return this.m_StyleKeywords;
			}
		}

		public LengthField()
			: this(null, 1000)
		{
		}

		public LengthField(int maxLength)
			: this(null, maxLength)
		{
		}

		public LengthField(string label, int maxLength = 1000)
			: base(label, maxLength, new LengthField.LengthInput())
		{
			base.AddToClassList(LengthField.ussClassName);
			base.AddLabelDragger<Length>();
			VisualElement visualElement = new VisualElement();
			visualElement.name = LengthField.unitDropdownContainerUssClass;
			visualElement.AddToClassList(LengthField.unitDropdownContainerUssClass);
			this.m_StyleKeywords.AddRange(LengthField.KLAuto);
			this.PopulateAdditionalOptions(this.m_CachedRegularOptionsList);
			this.m_AllOptionsList.AddRange(this.m_CachedRegularOptionsList);
			this.m_AllOptionsList.AddRange(this.m_StyleKeywords);
			this.m_OptionsPopup = new PopupField<string>(this.m_AllOptionsList, 0, new Func<string, string>(LengthField.OnFormatSelectedValue), null);
			this.m_OptionsPopup.AddToClassList(LengthField.unitDropdownUssClass);
			visualElement.Add(this.m_OptionsPopup);
			this.lengthInput.parentLengthField = this;
			this.lengthInput.AddToClassList(LengthField.inputUssClassName);
			this.lengthInput.delegatesFocus = true;
			base.Add(visualElement);
			this.m_OptionsPopup.RegisterValueChangedCallback<string>(new EventCallback<ChangeEvent<string>>(this.OnPopupFieldValueChange));
			this.UpdateFields();
			this.showUnitAsDropdown = true;
		}

		public void PopulateStyleKeywords(List<string> keywordList)
		{
			bool flag = this.m_SyntaxTree == null;
			if (flag)
			{
				keywordList.AddRange(LengthField.KLAuto);
			}
			else
			{
				bool flag2 = LengthField.FindKeywordInExpression(this.m_SyntaxTree, LengthField.KeywordAuto);
				bool flag3 = LengthField.FindKeywordInExpression(this.m_SyntaxTree, LengthField.KeywordNone);
				bool flag4 = flag2;
				if (flag4)
				{
					keywordList.AddRange(LengthField.KLAuto);
				}
				else
				{
					bool flag5 = flag3;
					if (flag5)
					{
						keywordList.AddRange(LengthField.KLNone);
					}
					else
					{
						keywordList.AddRange(LengthField.KLInitial);
					}
				}
			}
		}

		public override void SetValueWithoutNotify(Length newValue)
		{
			bool flag = !this.IsValid(newValue);
			if (!flag)
			{
				base.SetValueWithoutNotify(newValue);
				this.SetOptionsPopupFromValue();
			}
		}

		public void SetValidation(StylePropertyValidation validation)
		{
			Syntax syntax = validation as Syntax;
			bool flag = syntax == null;
			if (!flag)
			{
				this.m_SyntaxTree = Syntax.GetSyntaxTree(syntax);
				this.UpdateOptionsMenu();
			}
		}

		public void SetValidation(in StylePropertyValidationCollection validation)
		{
			List<Syntax> list;
			using (CollectionPool<List<Syntax>, Syntax>.Get(out list))
			{
				foreach (StylePropertyValidation stylePropertyValidation in validation)
				{
					Syntax syntax = stylePropertyValidation as Syntax;
					bool flag = syntax != null;
					if (flag)
					{
						list.Add(syntax);
					}
				}
				this.m_SyntaxTree = Syntax.GetSyntaxTree(list);
				this.UpdateOptionsMenu();
			}
		}

		public void ClearValidation()
		{
			this.m_SyntaxTree = null;
			this.UpdateOptionsMenu();
		}

		public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, Length startValue)
		{
			this.lengthInput.ApplyInputDeviceDelta(delta, speed, startValue);
		}

		protected override string ValueToString(Length v)
		{
			bool flag = this.showUnitAsDropdown && !v.IsAuto() && !v.IsNone();
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

		protected override Length StringToValue(string str)
		{
			ReadOnlySpan<char> readOnlySpan = str.AsSpan().Trim();
			bool flag = readOnlySpan.Equals(LengthField.KeywordAuto, StringComparison.OrdinalIgnoreCase);
			Length length;
			if (flag)
			{
				length = Length.Auto();
			}
			else
			{
				bool flag2 = readOnlySpan.Equals(LengthField.KeywordNone, StringComparison.OrdinalIgnoreCase);
				if (flag2)
				{
					length = Length.None();
				}
				else
				{
					LengthUnit lengthUnit = this.value.unit;
					bool flag3 = lengthUnit != LengthUnit.Percent && lengthUnit > LengthUnit.Pixel;
					if (flag3)
					{
						lengthUnit = LengthUnit.Pixel;
					}
					bool flag4 = readOnlySpan.EndsWith(LengthField.UnitPercent, StringComparison.Ordinal);
					if (flag4)
					{
						ReadOnlySpan<char> readOnlySpan2 = readOnlySpan;
						int num = LengthField.UnitPercent.Length;
						readOnlySpan = readOnlySpan2.Slice(0, readOnlySpan2.Length - num);
						lengthUnit = LengthUnit.Percent;
					}
					else
					{
						bool flag5 = readOnlySpan.EndsWith(LengthField.UnitPixel, StringComparison.OrdinalIgnoreCase);
						if (flag5)
						{
							ReadOnlySpan<char> readOnlySpan2 = readOnlySpan;
							int num = LengthField.UnitPixel.Length;
							readOnlySpan = readOnlySpan2.Slice(0, readOnlySpan2.Length - num);
							lengthUnit = LengthUnit.Pixel;
						}
					}
					float num2;
					ExpressionEvaluator.Expression expression;
					length = (UINumericFieldsUtils.TryConvertStringToFloat(readOnlySpan.ToString(), base.textInputBase.originalText, out num2, out expression) ? new Length(num2, lengthUnit) : this.value);
				}
			}
			return length;
		}

		internal override bool CanTryParse(string textString)
		{
			double num;
			return double.TryParse(textString, out num);
		}

		private void UpdateFields()
		{
			base.text = this.ValueToString(this.value);
			this.m_OptionsPopup.EnableInClassList(LengthField.invisibleUnitDropdownUssClass, !this.showUnitAsDropdown);
		}

		private bool IsValid(Length newValue)
		{
			bool flag = this.m_SyntaxTree == null;
			return flag || this.m_StyleMatcher.Match(this.m_SyntaxTree, newValue.ToString()).success;
		}

		protected internal bool Validate(Length previousValue, Length newValue)
		{
			bool flag = !this.IsValid(newValue);
			bool flag2;
			if (flag)
			{
				this.value = previousValue;
				flag2 = false;
			}
			else
			{
				flag2 = true;
			}
			return flag2;
		}

		private void UpdateOptionsMenu()
		{
			this.m_CachedRegularOptionsList.Clear();
			this.PopulateAdditionalOptions(this.m_CachedRegularOptionsList);
			this.m_StyleKeywords.Clear();
			this.PopulateStyleKeywords(this.m_StyleKeywords);
			this.m_AllOptionsList.Clear();
			this.m_AllOptionsList.AddRange(this.m_CachedRegularOptionsList);
			this.m_AllOptionsList.AddRange(this.m_StyleKeywords);
			this.m_OptionsPopup.choices = this.m_AllOptionsList;
			bool flag = !this.IsValid(this.value);
			if (flag)
			{
				this.value = this.GetValidValue();
			}
			this.SetOptionsPopupFromValue();
		}

		internal void AddOption(string newOption)
		{
			this.m_OptionsPopup.choices.Add(newOption);
			bool flag = !this.IsValid(this.value);
			if (flag)
			{
				this.value = this.GetValidValue();
			}
			this.SetOptionsPopupFromValue();
		}

		private Length GetValidValue()
		{
			bool flag = this.m_Units.Count == 0;
			Length length;
			if (flag)
			{
				length = Length.Auto();
			}
			else
			{
				bool flag2 = this.m_Units.Contains(LengthUnit.Pixel);
				if (flag2)
				{
					length = 0f;
				}
				else
				{
					bool flag3 = this.m_Units.Contains(LengthUnit.Percent);
					if (flag3)
					{
						length = Length.Percent(0f);
					}
					else
					{
						length = Length.None();
					}
				}
			}
			return length;
		}

		private static bool FindKeywordInExpression(Expression expression, string keyword)
		{
			bool flag = expression.type == ExpressionType.Keyword && expression.keyword == keyword;
			bool flag2;
			if (flag)
			{
				flag2 = true;
			}
			else
			{
				bool flag3 = expression.subExpressions == null;
				if (flag3)
				{
					flag2 = false;
				}
				else
				{
					foreach (Expression expression2 in expression.subExpressions)
					{
						bool flag4 = LengthField.FindKeywordInExpression(expression2, keyword);
						if (flag4)
						{
							return true;
						}
					}
					flag2 = false;
				}
			}
			return flag2;
		}

		private void PopulateAdditionalOptions(List<string> additionalOptions)
		{
			bool flag = this.m_SyntaxTree == null;
			if (flag)
			{
				additionalOptions.AddRange(LengthField.KLDefaultUnits);
			}
			else
			{
				bool flag2 = this.FindUnitInExpression(this.m_SyntaxTree, DataType.Length);
				bool flag3 = this.FindUnitInExpression(this.m_SyntaxTree, DataType.Percentage);
				this.m_Units.Clear();
				bool flag4 = flag2;
				if (flag4)
				{
					this.m_Units.Add(LengthUnit.Pixel);
				}
				bool flag5 = flag3;
				if (flag5)
				{
					this.m_Units.Add(LengthUnit.Percent);
				}
				foreach (LengthUnit lengthUnit in this.m_Units)
				{
					additionalOptions.Add(lengthUnit.ToDisplayString());
				}
			}
		}

		private bool FindUnitInExpression(Expression expression, DataType dataType)
		{
			bool flag = expression.type == ExpressionType.Data && expression.dataType == dataType;
			bool flag2;
			if (flag)
			{
				flag2 = true;
			}
			else
			{
				bool flag3 = expression.subExpressions == null;
				if (flag3)
				{
					flag2 = false;
				}
				else
				{
					foreach (Expression expression2 in expression.subExpressions)
					{
						bool flag4 = this.FindUnitInExpression(expression2, dataType);
						if (flag4)
						{
							return true;
						}
					}
					flag2 = false;
				}
			}
			return flag2;
		}

		private static string OnFormatSelectedValue(string value)
		{
			return (Array.IndexOf<string>(LengthField.AllKeywords, value) < 0) ? value : LengthField.s_NoOptionString;
		}

		private void SetOptionsPopupFromValue()
		{
			bool flag = this.value.IsAuto();
			if (flag)
			{
				this.m_OptionsPopup.SetValueWithoutNotify(LengthField.KeywordAuto);
			}
			else
			{
				bool flag2 = this.value.IsNone();
				if (flag2)
				{
					this.m_OptionsPopup.SetValueWithoutNotify(LengthField.KeywordNone);
				}
			}
			string text = this.value.unit.ToDisplayString();
			bool flag3 = string.Compare(text, LengthField.s_NoOptionString, StringComparison.OrdinalIgnoreCase) != 0;
			bool flag4 = flag3;
			if (flag4)
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
				Length length;
				if (!(newValue == "auto"))
				{
					if (!(newValue == "none"))
					{
						if (!(newValue == "px"))
						{
							if (!(newValue == "%"))
							{
								length = this.value;
							}
							else
							{
								length = new Length(this.value.value, LengthUnit.Percent);
							}
						}
						else
						{
							length = new Length(this.value.value, LengthUnit.Pixel);
						}
					}
					else
					{
						length = Length.None();
					}
				}
				else
				{
					length = Length.Auto();
				}
				if (!true)
				{
				}
				this.value = length;
				evt.StopImmediatePropagation();
			}
		}

		public static readonly BindingId showUnitAsDropdownProperty = "showUnitAsDropdown";

		public new static readonly string ussClassName = "unity-style-field";

		public new static readonly string inputUssClassName = LengthField.ussClassName + "__visual-input";

		public static readonly string unitDropdownContainerUssClass = LengthField.ussClassName + "__options-popup-container";

		public static readonly string unitDropdownUssClass = LengthField.ussClassName + "__options-popup";

		public static readonly string invisibleUnitDropdownUssClass = LengthField.unitDropdownUssClass + "--invisible";

		public static readonly string KeywordInitial = "initial";

		public static readonly string KeywordAuto = "auto";

		public static readonly string KeywordNone = "none";

		public static readonly string UnitPixel = "px";

		public static readonly string UnitPercent = "%";

		private static readonly string[] KLInitial = new string[] { LengthField.KeywordInitial };

		private static readonly string[] KLDefaultUnits = new string[]
		{
			LengthField.UnitPixel,
			LengthField.UnitPercent
		};

		private static readonly string[] KLAuto = new string[]
		{
			LengthField.KeywordAuto,
			LengthField.KeywordInitial
		};

		private static readonly string[] KLNone = new string[]
		{
			LengthField.KeywordNone,
			LengthField.KeywordInitial
		};

		private static readonly string[] AllKeywords = new string[]
		{
			LengthField.KeywordAuto,
			LengthField.KeywordNone,
			LengthField.KeywordInitial
		};

		internal static readonly string s_NoOptionString = "-";

		private bool m_ShowUnitAsDropdown;

		private readonly List<LengthUnit> m_Units = new List<LengthUnit> { LengthUnit.Pixel };

		private readonly PopupField<string> m_OptionsPopup;

		private readonly List<string> m_StyleKeywords = new List<string>();

		private readonly List<string> m_CachedRegularOptionsList = new List<string>();

		private readonly List<string> m_AllOptionsList = new List<string>();

		private readonly StyleMatcher m_StyleMatcher = new StyleMatcher();

		private Expression m_SyntaxTree;

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : TextValueField<Length>.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			[RegisterUxmlCache]
			public new static void Register()
			{
				TextValueField<Length>.UxmlSerializedData.Register();
				UxmlDescriptionCache.RegisterType(typeof(LengthField.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("showUnitAsDropdown", "show-unit-as-dropdown", null, Array.Empty<string>())
				}, false);
			}

			public override object CreateInstance()
			{
				return new LengthField();
			}

			public override void Deserialize(object obj)
			{
				base.Deserialize(obj);
				LengthField lengthField = (LengthField)obj;
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.showUnitAsDropdown_UxmlAttributeFlags);
				if (flag)
				{
					lengthField.showUnitAsDropdown = this.showUnitAsDropdown;
				}
			}

			[SerializeField]
			private bool showUnitAsDropdown;

			[SerializeField]
			[HideInInspector]
			[UxmlIgnore]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags showUnitAsDropdown_UxmlAttributeFlags;
		}

		private class LengthInput : TextValueField<Length>.TextValueInput
		{
			internal LengthField parentLengthField { get; set; }

			internal LengthInput()
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

			public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, Length startValue)
			{
				Length length = this.StringToValue(base.text);
				length.unit = startValue.unit;
				bool flag = length.IsAuto() || length.IsNone();
				if (flag)
				{
					length = new Length(0f);
				}
				double num = (double)length.value;
				double num2 = (double)NumericFieldDraggerUtility.CalculateIntDragSensitivity((long)startValue.value);
				float num3 = NumericFieldDraggerUtility.Acceleration(speed == DeltaSpeed.Fast, speed == DeltaSpeed.Slow);
				num += (double)NumericFieldDraggerUtility.NiceDelta(delta, num3) * num2;
				num = Mathf.RoundBasedOnMinimumDifference(num, num2);
				length = new Length((float)num, length.unit);
				bool isDelayed = this.parentLengthField.isDelayed;
				if (isDelayed)
				{
					this.parentLengthField.text = this.ValueToString(length);
				}
				else
				{
					this.parentLengthField.value = length;
				}
			}

			protected override string ValueToString(Length v)
			{
				return this.parentLengthField.showUnitAsDropdown ? v.value.ToString(CultureInfo.InvariantCulture) : v.ToString();
			}

			protected override Length StringToValue(string str)
			{
				return Length.ParseString(str, this.parentLengthField.value);
			}
		}
	}
}
