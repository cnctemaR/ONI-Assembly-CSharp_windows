using System;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
	public class HelpBox : VisualElement
	{
		[CreateProperty]
		public string text
		{
			get
			{
				return this.m_Label.text;
			}
			set
			{
				string text = this.text;
				this.m_Label.text = value;
				bool flag = string.CompareOrdinal(text, this.text) != 0;
				if (flag)
				{
					base.NotifyPropertyChanged(in HelpBox.textProperty);
				}
			}
		}

		[CreateProperty]
		public HelpBoxMessageType messageType
		{
			get
			{
				return this.m_HelpBoxMessageType;
			}
			set
			{
				bool flag = value != this.m_HelpBoxMessageType;
				if (flag)
				{
					this.m_HelpBoxMessageType = value;
					this.UpdateIcon(value);
					base.NotifyPropertyChanged(in HelpBox.messageTypeProperty);
				}
			}
		}

		public HelpBox()
			: this(string.Empty, HelpBoxMessageType.None)
		{
		}

		public HelpBox(string text, HelpBoxMessageType messageType)
		{
			base.AddToClassList(HelpBox.ussClassName);
			this.m_HelpBoxMessageType = messageType;
			this.m_Label = new Label(text);
			this.m_Label.AddToClassList(HelpBox.labelUssClassName);
			base.Add(this.m_Label);
			this.m_Icon = new VisualElement();
			this.m_Icon.AddToClassList(HelpBox.iconUssClassName);
			this.UpdateIcon(messageType);
		}

		private string GetIconClass(HelpBoxMessageType messageType)
		{
			string text;
			switch (messageType)
			{
			case HelpBoxMessageType.Info:
				text = HelpBox.iconInfoUssClassName;
				break;
			case HelpBoxMessageType.Warning:
				text = HelpBox.iconwarningUssClassName;
				break;
			case HelpBoxMessageType.Error:
				text = HelpBox.iconErrorUssClassName;
				break;
			default:
				text = null;
				break;
			}
			return text;
		}

		private void UpdateIcon(HelpBoxMessageType messageType)
		{
			bool flag = !string.IsNullOrEmpty(this.m_IconClass);
			if (flag)
			{
				this.m_Icon.RemoveFromClassList(this.m_IconClass);
			}
			this.m_IconClass = this.GetIconClass(messageType);
			bool flag2 = this.m_IconClass == null;
			if (flag2)
			{
				this.m_Icon.RemoveFromHierarchy();
			}
			else
			{
				this.m_Icon.AddToClassList(this.m_IconClass);
				bool flag3 = this.m_Icon.parent == null;
				if (flag3)
				{
					base.Insert(0, this.m_Icon);
				}
			}
		}

		internal static readonly BindingId textProperty = "text";

		internal static readonly BindingId messageTypeProperty = "messageType";

		public static readonly string ussClassName = "unity-help-box";

		public static readonly string labelUssClassName = HelpBox.ussClassName + "__label";

		public static readonly string iconUssClassName = HelpBox.ussClassName + "__icon";

		public static readonly string iconInfoUssClassName = HelpBox.iconUssClassName + "--info";

		public static readonly string iconwarningUssClassName = HelpBox.iconUssClassName + "--warning";

		public static readonly string iconErrorUssClassName = HelpBox.iconUssClassName + "--error";

		private HelpBoxMessageType m_HelpBoxMessageType;

		private VisualElement m_Icon;

		private string m_IconClass;

		private Label m_Label;

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : VisualElement.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				UxmlDescriptionCache.RegisterType(typeof(HelpBox.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("text", "text", null, Array.Empty<string>()),
					new UxmlAttributeNames("messageType", "message-type", null, Array.Empty<string>())
				}, false);
			}

			public override object CreateInstance()
			{
				return new HelpBox();
			}

			public override void Deserialize(object obj)
			{
				base.Deserialize(obj);
				HelpBox helpBox = (HelpBox)obj;
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.text_UxmlAttributeFlags);
				if (flag)
				{
					helpBox.text = this.text;
				}
				bool flag2 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.messageType_UxmlAttributeFlags);
				if (flag2)
				{
					helpBox.messageType = this.messageType;
				}
			}

			[MultilineTextField]
			[SerializeField]
			private string text;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags text_UxmlAttributeFlags;

			[SerializeField]
			private HelpBoxMessageType messageType;

			[UxmlIgnore]
			[HideInInspector]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags messageType_UxmlAttributeFlags;
		}

		[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlFactory : UxmlFactory<HelpBox, HelpBox.UxmlTraits>
		{
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlTraits : VisualElement.UxmlTraits
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				HelpBox helpBox = ve as HelpBox;
				helpBox.text = this.m_Text.GetValueFromBag(bag, cc);
				helpBox.messageType = this.m_MessageType.GetValueFromBag(bag, cc);
			}

			private UxmlStringAttributeDescription m_Text = new UxmlStringAttributeDescription
			{
				name = "text"
			};

			private UxmlEnumAttributeDescription<HelpBoxMessageType> m_MessageType = new UxmlEnumAttributeDescription<HelpBoxMessageType>
			{
				name = "message-type",
				defaultValue = HelpBoxMessageType.None
			};
		}
	}
}
