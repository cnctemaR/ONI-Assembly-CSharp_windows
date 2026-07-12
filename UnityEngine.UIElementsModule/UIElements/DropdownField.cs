using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	public class DropdownField : PopupField<string>
	{
		public DropdownField()
			: this(null)
		{
		}

		public DropdownField(string label)
			: base(label)
		{
		}

		public DropdownField(List<string> choices, string defaultValue, Func<string, string> formatSelectedValueCallback = null, Func<string, string> formatListItemCallback = null)
			: this(null, choices, defaultValue, formatSelectedValueCallback, formatListItemCallback)
		{
		}

		public DropdownField(string label, List<string> choices, string defaultValue, Func<string, string> formatSelectedValueCallback = null, Func<string, string> formatListItemCallback = null)
			: base(label, choices, defaultValue, formatSelectedValueCallback, formatListItemCallback)
		{
		}

		public DropdownField(List<string> choices, int defaultIndex, Func<string, string> formatSelectedValueCallback = null, Func<string, string> formatListItemCallback = null)
			: this(null, choices, defaultIndex, formatSelectedValueCallback, formatListItemCallback)
		{
		}

		public DropdownField(string label, List<string> choices, int defaultIndex, Func<string, string> formatSelectedValueCallback = null, Func<string, string> formatListItemCallback = null)
			: base(label, choices, defaultIndex, formatSelectedValueCallback, formatListItemCallback)
		{
		}

		public new class UxmlFactory : UxmlFactory<DropdownField, DropdownField.UxmlTraits>
		{
		}

		public new class UxmlTraits : BaseField<string>.UxmlTraits
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				DropdownField dropdownField = (DropdownField)ve;
				List<string> list = BaseField<string>.UxmlTraits.ParseChoiceList(this.m_Choices.GetValueFromBag(bag, cc));
				bool flag = list != null;
				if (flag)
				{
					dropdownField.choices = list;
				}
				dropdownField.index = this.m_Index.GetValueFromBag(bag, cc);
			}

			private UxmlIntAttributeDescription m_Index = new UxmlIntAttributeDescription
			{
				name = "index",
				defaultValue = -1
			};

			private UxmlStringAttributeDescription m_Choices = new UxmlStringAttributeDescription
			{
				name = "choices"
			};
		}
	}
}
