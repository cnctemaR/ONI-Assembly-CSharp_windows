using System;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
	public class Button : TextElement
	{
		public Clickable clickable
		{
			get
			{
				return this.m_Clickable;
			}
			set
			{
				bool flag = this.m_Clickable != null && this.m_Clickable.target == this;
				if (flag)
				{
					this.RemoveManipulator(this.m_Clickable);
				}
				this.m_Clickable = value;
				bool flag2 = this.m_Clickable != null;
				if (flag2)
				{
					this.AddManipulator(this.m_Clickable);
				}
			}
		}

		[Obsolete("onClick is obsolete. Use clicked instead (UnityUpgradable) -> clicked", true)]
		public event Action onClick
		{
			add
			{
				this.clicked += value;
			}
			remove
			{
				this.clicked -= value;
			}
		}

		public event Action clicked
		{
			add
			{
				bool flag = this.m_Clickable == null;
				if (flag)
				{
					this.clickable = new Clickable(value);
				}
				else
				{
					this.m_Clickable.clicked += value;
				}
			}
			remove
			{
				bool flag = this.m_Clickable != null;
				if (flag)
				{
					this.m_Clickable.clicked -= value;
				}
			}
		}

		private Object iconImageReference
		{
			get
			{
				return this.iconImage.GetSelectedImage();
			}
			set
			{
				this.iconImage = Background.FromObject(value);
			}
		}

		[CreateProperty]
		public Background iconImage
		{
			get
			{
				return this.m_IconImage;
			}
			set
			{
				bool flag = (value.IsEmpty() && this.m_ImageElement == null) || value == this.m_IconImage;
				if (!flag)
				{
					bool flag2 = value.IsEmpty();
					if (flag2)
					{
						this.m_IconImage = value;
						this.ResetButtonHierarchy();
						base.NotifyPropertyChanged(in Button.iconImageProperty);
					}
					else
					{
						bool flag3 = this.m_ImageElement == null;
						if (flag3)
						{
							this.UpdateButtonHierarchy();
						}
						bool flag4 = value.texture;
						if (flag4)
						{
							this.m_ImageElement.image = value.texture;
						}
						else
						{
							bool flag5 = value.sprite;
							if (flag5)
							{
								this.m_ImageElement.sprite = value.sprite;
							}
							else
							{
								bool flag6 = value.renderTexture;
								if (flag6)
								{
									this.m_ImageElement.image = value.renderTexture;
								}
								else
								{
									this.m_ImageElement.vectorImage = value.vectorImage;
								}
							}
						}
						this.m_IconImage = value;
						base.EnableInClassList(Button.iconOnlyUssClassName, string.IsNullOrEmpty(this.text));
						base.NotifyPropertyChanged(in Button.iconImageProperty);
					}
				}
			}
		}

		public override string text
		{
			get
			{
				return this.m_Text ?? string.Empty;
			}
			set
			{
				this.m_Text = value;
				base.EnableInClassList(Button.iconOnlyUssClassName, !this.m_IconImage.IsEmpty() && string.IsNullOrEmpty(this.text));
				bool flag = this.m_TextElement != null;
				if (flag)
				{
					base.text = string.Empty;
					bool flag2 = this.m_TextElement.text == this.m_Text;
					if (!flag2)
					{
						this.m_TextElement.text = this.m_Text;
					}
				}
				else
				{
					bool flag3 = base.text == this.m_Text;
					if (!flag3)
					{
						base.text = this.m_Text;
					}
				}
			}
		}

		public Button()
			: this(default(Background), null)
		{
		}

		public Button(Background iconImage, Action clickEvent = null)
			: this(clickEvent)
		{
			this.iconImage = iconImage;
		}

		public Button(Action clickEvent)
		{
			base.AddToClassList(Button.ussClassName);
			this.clickable = new Clickable(clickEvent);
			this.focusable = true;
			base.tabIndex = 0;
			base.RegisterCallback<NavigationSubmitEvent>(new EventCallback<NavigationSubmitEvent>(this.OnNavigationSubmit), TrickleDown.NoTrickleDown);
		}

		private void OnNavigationSubmit(NavigationSubmitEvent evt)
		{
			Clickable clickable = this.clickable;
			if (clickable != null)
			{
				clickable.SimulateSingleClick(evt, 100);
			}
			evt.StopPropagation();
		}

		protected internal override Vector2 DoMeasure(float desiredWidth, VisualElement.MeasureMode widthMode, float desiredHeight, VisualElement.MeasureMode heightMode)
		{
			string text = this.text;
			bool flag = string.IsNullOrEmpty(text);
			if (flag)
			{
				text = Button.NonEmptyString;
			}
			return base.MeasureTextSize(text, desiredWidth, widthMode, desiredHeight, heightMode);
		}

		private void UpdateButtonHierarchy()
		{
			bool flag = this.m_ImageElement == null;
			if (flag)
			{
				this.m_ImageElement = new Image
				{
					classList = { Button.imageUSSClassName }
				};
				base.Add(this.m_ImageElement);
				base.AddToClassList(Button.iconUssClassName);
			}
			bool flag2 = this.m_TextElement == null;
			if (flag2)
			{
				this.m_TextElement = new TextElement
				{
					text = this.text
				};
				this.m_Text = this.text;
				base.text = string.Empty;
				base.Add(this.m_TextElement);
			}
		}

		private void ResetButtonHierarchy()
		{
			bool flag = this.m_ImageElement != null;
			if (flag)
			{
				this.m_ImageElement.RemoveFromHierarchy();
				this.m_ImageElement = null;
				base.RemoveFromClassList(Button.iconUssClassName);
				base.RemoveFromClassList(Button.iconOnlyUssClassName);
			}
			bool flag2 = this.m_TextElement != null;
			if (flag2)
			{
				string text = this.m_TextElement.text;
				this.m_TextElement.RemoveFromHierarchy();
				this.m_TextElement = null;
				this.text = text;
			}
		}

		internal static readonly BindingId iconImageProperty = "iconImage";

		public new static readonly string ussClassName = "unity-button";

		public static readonly string iconUssClassName = Button.ussClassName + "--with-icon";

		public static readonly string iconOnlyUssClassName = Button.ussClassName + "--with-icon-only";

		public static readonly string imageUSSClassName = Button.ussClassName + "__image";

		private Clickable m_Clickable;

		private TextElement m_TextElement;

		private Image m_ImageElement;

		private Background m_IconImage;

		private string m_Text = string.Empty;

		private static readonly string NonEmptyString = " ";

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : TextElement.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				UxmlDescriptionCache.RegisterType(typeof(Button.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("iconImageReference", "icon-image", null, Array.Empty<string>())
				}, false);
			}

			public override object CreateInstance()
			{
				return new Button();
			}

			public override void Deserialize(object obj)
			{
				base.Deserialize(obj);
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.iconImageReference_UxmlAttributeFlags);
				if (flag)
				{
					Button button = (Button)obj;
					button.iconImageReference = this.iconImageReference;
				}
			}

			[ImageFieldValueDecorator("Icon Image")]
			[SerializeField]
			[UxmlAttributeBindingPath("iconImage")]
			[UxmlAttribute("icon-image")]
			private Object iconImageReference;

			[HideInInspector]
			[UxmlIgnore]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags iconImageReference_UxmlAttributeFlags;
		}

		[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlFactory : UxmlFactory<Button, Button.UxmlTraits>
		{
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlTraits : TextElement.UxmlTraits
		{
			public UxmlTraits()
			{
				base.focusable.defaultValue = true;
			}

			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				Button button = (Button)ve;
				button.iconImage = this.m_IconImage.GetValueFromBag(bag, cc);
			}

			private readonly UxmlImageAttributeDescription m_IconImage = new UxmlImageAttributeDescription
			{
				name = "icon-image"
			};
		}
	}
}
