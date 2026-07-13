using System;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
	public class GroupBox : BindableElement, IGroupBox
	{
		internal Label titleLabel
		{
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			get
			{
				return this.m_TitleLabel;
			}
		}

		[CreateProperty]
		public string text
		{
			get
			{
				Label titleLabel = this.m_TitleLabel;
				return (titleLabel != null) ? titleLabel.text : null;
			}
			set
			{
				string text = this.text;
				bool flag = !string.IsNullOrEmpty(value);
				if (flag)
				{
					bool flag2 = this.m_TitleLabel == null;
					if (flag2)
					{
						this.m_TitleLabel = new Label(value);
						this.m_TitleLabel.AddToClassList(GroupBox.labelUssClassName);
						base.Insert(0, this.m_TitleLabel);
					}
					this.m_TitleLabel.text = value;
				}
				else
				{
					bool flag3 = this.m_TitleLabel != null;
					if (flag3)
					{
						this.m_TitleLabel.RemoveFromHierarchy();
						this.m_TitleLabel = null;
					}
				}
				bool flag4 = string.CompareOrdinal(text, this.text) != 0;
				if (flag4)
				{
					base.NotifyPropertyChanged(in GroupBox.textProperty);
				}
			}
		}

		public GroupBox()
			: this(null)
		{
		}

		public GroupBox(string text)
		{
			base.AddToClassList(GroupBox.ussClassName);
			this.text = text;
		}

		void IGroupBox.OnOptionAdded(IGroupBoxOption option)
		{
		}

		void IGroupBox.OnOptionRemoved(IGroupBoxOption option)
		{
		}

		internal static readonly BindingId textProperty = "text";

		public static readonly string ussClassName = "unity-group-box";

		public static readonly string labelUssClassName = GroupBox.ussClassName + "__label";

		private Label m_TitleLabel;

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : BindableElement.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				UxmlDescriptionCache.RegisterType(typeof(GroupBox.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("text", "text", null, Array.Empty<string>())
				}, false);
			}

			public override object CreateInstance()
			{
				return new GroupBox();
			}

			public override void Deserialize(object obj)
			{
				base.Deserialize(obj);
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.text_UxmlAttributeFlags);
				if (flag)
				{
					GroupBox groupBox = (GroupBox)obj;
					groupBox.text = this.text;
				}
			}

			[SerializeField]
			[MultilineTextField]
			private string text;

			[UxmlIgnore]
			[HideInInspector]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags text_UxmlAttributeFlags;
		}

		[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlFactory : UxmlFactory<GroupBox, GroupBox.UxmlTraits>
		{
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlTraits : BindableElement.UxmlTraits
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				((GroupBox)ve).text = this.m_Text.GetValueFromBag(bag, cc);
			}

			private UxmlStringAttributeDescription m_Text = new UxmlStringAttributeDescription
			{
				name = "text"
			};
		}
	}
}
