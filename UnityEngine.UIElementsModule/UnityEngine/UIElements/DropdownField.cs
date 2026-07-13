using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
	public class DropdownField : PopupField<string>
	{
		internal int valueOverride { get; set; }

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

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : BaseField<string>.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				BaseField<string>.UxmlSerializedData.Register();
				UxmlDescriptionCache.RegisterType(typeof(DropdownField.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("index", "index", null, Array.Empty<string>()),
					new UxmlAttributeNames("choices", "choices", null, Array.Empty<string>()),
					new UxmlAttributeNames("valueOverride", "value", null, Array.Empty<string>())
				}, false);
			}

			public override object CreateInstance()
			{
				return new DropdownField();
			}

			public override void Deserialize(object obj)
			{
				base.Deserialize(obj);
				DropdownField dropdownField = (DropdownField)obj;
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.choices_UxmlAttributeFlags) && this.choices != null;
				if (flag)
				{
					dropdownField.choices = new List<string>(this.choices);
				}
				bool flag2 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.index_UxmlAttributeFlags) && this.index != -1;
				if (flag2)
				{
					dropdownField.index = this.index;
				}
				bool flag3 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.valueOverride_UxmlAttributeFlags);
				if (flag3)
				{
					dropdownField.valueOverride = this.valueOverride;
				}
			}

			[SerializeField]
			[Delayed]
			private int index;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags index_UxmlAttributeFlags;

			[SerializeField]
			private List<string> choices;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags choices_UxmlAttributeFlags;

			[UxmlAttribute("value")]
			[SerializeField]
			[HideInInspector]
			private int valueOverride;

			[HideInInspector]
			[SerializeField]
			[UxmlIgnore]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags valueOverride_UxmlAttributeFlags;
		}

		[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlFactory : UxmlFactory<DropdownField, DropdownField.UxmlTraits>
		{
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlTraits : BaseField<string>.UxmlTraits
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				DropdownField dropdownField = (DropdownField)ve;
				List<string> list = UxmlUtility.ParseStringListAttribute(this.m_Choices.GetValueFromBag(bag, cc));
				bool flag = list != null;
				if (flag)
				{
					dropdownField.choices = list;
				}
				dropdownField.index = this.m_Index.GetValueFromBag(bag, cc);
			}

			private UxmlIntAttributeDescription m_Index = new UxmlIntAttributeDescription
			{
				name = "index"
			};

			private UxmlStringAttributeDescription m_Choices = new UxmlStringAttributeDescription
			{
				name = "choices"
			};
		}
	}
}
