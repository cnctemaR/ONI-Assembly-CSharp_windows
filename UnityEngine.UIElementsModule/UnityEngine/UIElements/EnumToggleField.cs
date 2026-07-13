using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal class EnumToggleField<T> : BaseField<T> where T : struct, Enum, IConvertible
	{
		public ToggleButtonGroup toggleButtonGroup
		{
			get
			{
				return this.m_ToggleButtonGroup;
			}
		}

		public EnumToggleField()
			: this(null, false)
		{
		}

		public EnumToggleField(bool useIcon = false)
			: this(null, useIcon)
		{
		}

		public EnumToggleField(string label, bool useIcon = false)
			: base(label, null)
		{
			this.m_ToggleButtonGroup = new ToggleButtonGroup();
			this.m_ToggleButtonGroup.AddToClassList(BaseField<T>.alignedFieldUssClassName);
			Type enumType = typeof(T);
			string text = EnumToggleField<T>.k_SpecialEnumNamesCases.GetValueOrDefault(enumType.Name, enumType.Name.ToKebabCase());
			this.m_ToggleButtonGroup.AddToClassList(ToggleButtonGroup.ussClassName + "_" + text + "-field");
			foreach (object obj in Enum.GetValues(enumType))
			{
				Enum @enum = (Enum)obj;
				string enumName = @enum.ToString();
				string text2 = StyleValueKeyword.Auto.ToString();
				Button button = new Button();
				bool flag = enumName == text2;
				if (flag)
				{
					button.name = "auto";
					button.text = text2.ToUpperInvariant();
				}
				else
				{
					text = EnumToggleField<T>.k_SpecialEnumNamesCases.GetValueOrDefault(enumName, enumName.ToKebabCase());
					button.name = text;
					if (useIcon)
					{
						button.iconImage = Background.FromTexture2D(new Texture2D(0, 0));
					}
					else
					{
						button.text = enumName;
					}
				}
				button.clicked += delegate
				{
					this.value = (T)((object)Enum.Parse(enumType, enumName, true));
				};
				this.m_ToggleButtonGroup.Add(button);
			}
			this.m_ToggleButtonGroup.userData = enumType;
			base.visualInput.Add(this.m_ToggleButtonGroup);
		}

		public void SetIconForEnumValue(T enumValue, Texture2D icon)
		{
			int num = Array.IndexOf(Enum.GetValues(typeof(T)), enumValue);
			bool flag = num >= 0;
			if (flag)
			{
				this.m_ToggleButtonGroup.GetButton(num).iconImage = Background.FromTexture2D(icon);
			}
		}

		public void SetTextForEnumValue(T enumValue, string text)
		{
			int num = Array.IndexOf(Enum.GetValues(typeof(T)), enumValue);
			bool flag = num >= 0;
			if (flag)
			{
				this.m_ToggleButtonGroup.GetButton(num).text = text;
			}
		}

		public override void SetValueWithoutNotify(T newValue)
		{
			base.SetValueWithoutNotify(newValue);
			ToggleButtonGroupState value = this.m_ToggleButtonGroup.value;
			value.ResetAllOptions();
			Type type = this.m_ToggleButtonGroup.userData as Type;
			bool flag = type != null;
			if (flag)
			{
				int num = Array.IndexOf(Enum.GetValues(type), newValue);
				value[num] = true;
			}
			this.m_ToggleButtonGroup.value = value;
		}

		private static readonly Dictionary<string, string> k_SpecialEnumNamesCases = new Dictionary<string, string>
		{
			{ "nowrap", "no-wrap" },
			{ "tabindex", "tab-index" }
		};

		private ToggleButtonGroup m_ToggleButtonGroup;

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : BaseField<T>.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			[RegisterUxmlCache]
			public new static void Register()
			{
				BaseField<T>.UxmlSerializedData.Register();
			}
		}
	}
}
