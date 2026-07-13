using System;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.UIElements
{
	[MovedFrom(true, "UnityEditor.UIElements", "UnityEditor.UIElementsModule", null)]
	public class EnumField : BaseField<Enum>
	{
		internal Type type
		{
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			get
			{
				return this.m_EnumType;
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal bool includeObsoleteValues
		{
			get
			{
				return this.m_IncludeObsoleteValues;
			}
			set
			{
				this.m_IncludeObsoleteValues = value;
			}
		}

		internal string typeAsString
		{
			get
			{
				return UxmlUtility.TypeToString(this.m_EnumType);
			}
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			set
			{
				this.m_EnumType = UxmlUtility.ParseType(value, null);
				bool flag = this.m_EnumType == null;
				if (flag)
				{
					this.value = null;
					this.m_TextElement.text = string.Empty;
				}
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal string valueAsString
		{
			get
			{
				Enum value = this.value;
				return (value != null) ? value.ToString() : null;
			}
			set
			{
				bool flag = this.type != null;
				if (flag)
				{
					bool flag2 = !string.IsNullOrEmpty(value);
					if (flag2)
					{
						object obj;
						Enum @enum;
						bool flag3;
						if (Enum.TryParse(this.type, value, false, out obj))
						{
							@enum = obj as Enum;
							flag3 = @enum != null;
						}
						else
						{
							flag3 = false;
						}
						bool flag4 = flag3;
						if (flag4)
						{
							this.Init(@enum, this.includeObsoleteValues);
						}
						else
						{
							this.PopulateDataFromType(this.type);
							this.value = null;
						}
					}
					else
					{
						Enum enum2 = (Enum)Enum.ToObject(this.type, 0);
						this.Init(enum2, this.includeObsoleteValues);
					}
				}
				else
				{
					this.value = null;
				}
			}
		}

		[CreateProperty(ReadOnly = true)]
		public string text
		{
			get
			{
				return this.m_TextElement.text;
			}
		}

		private void Initialize(Enum defaultValue)
		{
			this.m_TextElement = new TextElement();
			this.m_TextElement.AddToClassList(EnumField.textUssClassName);
			this.m_TextElement.pickingMode = PickingMode.Ignore;
			base.visualInput.Add(this.m_TextElement);
			this.m_ArrowElement = new VisualElement();
			this.m_ArrowElement.AddToClassList(EnumField.arrowUssClassName);
			this.m_ArrowElement.pickingMode = PickingMode.Ignore;
			base.visualInput.Add(this.m_ArrowElement);
			bool flag = defaultValue != null;
			if (flag)
			{
				this.Init(defaultValue);
			}
		}

		public EnumField()
			: this(null, null)
		{
		}

		public EnumField(Enum defaultValue)
			: this(null, defaultValue)
		{
		}

		public EnumField(string label, Enum defaultValue = null)
			: base(label, null)
		{
			base.AddToClassList(EnumField.ussClassName);
			base.labelElement.AddToClassList(EnumField.labelUssClassName);
			base.visualInput.AddToClassList(EnumField.inputUssClassName);
			this.Initialize(defaultValue);
			base.RegisterCallback<PointerDownEvent>(new EventCallback<PointerDownEvent>(this.OnPointerDownEvent), TrickleDown.NoTrickleDown);
			base.RegisterCallback<PointerMoveEvent>(new EventCallback<PointerMoveEvent>(this.OnPointerMoveEvent), TrickleDown.NoTrickleDown);
			base.RegisterCallback<MouseDownEvent>(delegate(MouseDownEvent e)
			{
				bool flag = e.button == 0;
				if (flag)
				{
					e.StopPropagation();
				}
			}, TrickleDown.NoTrickleDown);
			base.RegisterCallback<NavigationSubmitEvent>(new EventCallback<NavigationSubmitEvent>(this.OnNavigationSubmit), TrickleDown.NoTrickleDown);
		}

		public void Init(Enum defaultValue)
		{
			this.Init(defaultValue, false);
		}

		public void Init(Enum defaultValue, bool includeObsoleteValues)
		{
			bool flag = defaultValue == null;
			if (flag)
			{
				throw new ArgumentNullException("defaultValue");
			}
			this.m_IncludeObsoleteValues = includeObsoleteValues;
			this.PopulateDataFromType(defaultValue.GetType());
			bool flag2 = !object.Equals(base.rawValue, defaultValue);
			if (flag2)
			{
				this.SetValueWithoutNotify(defaultValue);
			}
			else
			{
				this.UpdateValueLabel(defaultValue);
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void PopulateDataFromType(Type enumType)
		{
			this.m_EnumType = enumType;
			this.m_EnumData = EnumDataUtility.GetCachedEnumData(this.m_EnumType, this.includeObsoleteValues ? EnumDataUtility.CachedType.IncludeObsoleteExceptErrors : EnumDataUtility.CachedType.ExcludeObsolete, new Func<string, string>(NameFormatter.FormatVariableName));
		}

		public override void SetValueWithoutNotify(Enum newValue)
		{
			bool flag = !object.Equals(base.rawValue, newValue);
			if (flag)
			{
				base.SetValueWithoutNotify(newValue);
				bool flag2 = this.m_EnumType == null;
				if (!flag2)
				{
					this.UpdateValueLabel(newValue);
				}
			}
		}

		private void UpdateValueLabel(Enum value)
		{
			int num = Array.IndexOf<Enum>(this.m_EnumData.values, value);
			bool flag = (num >= 0) & (num < this.m_EnumData.values.Length);
			if (flag)
			{
				this.m_TextElement.text = this.m_EnumData.displayNames[num];
			}
			else
			{
				this.m_TextElement.text = string.Empty;
			}
		}

		private void OnPointerDownEvent(PointerDownEvent evt)
		{
			this.ProcessPointerDown<PointerDownEvent>(evt);
		}

		private void OnPointerMoveEvent(PointerMoveEvent evt)
		{
			bool flag = evt.button == 0;
			if (flag)
			{
				bool flag2 = (evt.pressedButtons & 1) != 0;
				if (flag2)
				{
					this.ProcessPointerDown<PointerMoveEvent>(evt);
				}
			}
		}

		private bool ContainsPointer(int pointerId)
		{
			VisualElement topElementUnderPointer = base.elementPanel.GetTopElementUnderPointer(pointerId);
			return this == topElementUnderPointer || base.visualInput == topElementUnderPointer;
		}

		private void ProcessPointerDown<T>(PointerEventBase<T> evt) where T : PointerEventBase<T>, new()
		{
			bool flag = evt.button == 0;
			if (flag)
			{
				bool flag2 = this.ContainsPointer(evt.pointerId);
				if (flag2)
				{
					base.schedule.Execute(new Action(this.ShowMenu));
					evt.StopPropagation();
				}
			}
		}

		private void OnNavigationSubmit(NavigationSubmitEvent evt)
		{
			this.ShowMenu();
			evt.StopPropagation();
		}

		internal void ShowMenu()
		{
			bool flag = this.m_EnumType == null;
			if (!flag)
			{
				AbstractGenericMenu abstractGenericMenu = ((this.createMenuCallback != null) ? this.createMenuCallback() : base.elementPanel.CreateMenu());
				int num = Array.IndexOf<Enum>(this.m_EnumData.values, this.value);
				for (int i = 0; i < this.m_EnumData.values.Length; i++)
				{
					bool flag2 = num == i;
					abstractGenericMenu.AddItem(this.m_EnumData.displayNames[i], flag2, delegate(object contentView)
					{
						this.ChangeValueFromMenu(contentView);
					}, this.m_EnumData.values[i]);
				}
				abstractGenericMenu.DropDown(base.visualInput.worldBound, this, DropdownMenuSizeMode.Fixed);
			}
		}

		private void ChangeValueFromMenu(object menuItem)
		{
			this.value = menuItem as Enum;
		}

		protected override void UpdateMixedValueContent()
		{
			bool showMixedValue = base.showMixedValue;
			if (showMixedValue)
			{
				this.m_TextElement.text = BaseField<Enum>.mixedValueString;
			}
			else
			{
				this.UpdateValueLabel(this.value);
			}
			this.m_TextElement.EnableInClassList(EnumField.labelUssClassName, base.showMixedValue);
			this.m_TextElement.EnableInClassList(BaseField<Enum>.mixedValueLabelUssClassName, base.showMixedValue);
		}

		internal static readonly BindingId textProperty = "text";

		private Type m_EnumType;

		private bool m_IncludeObsoleteValues;

		private TextElement m_TextElement;

		private VisualElement m_ArrowElement;

		private EnumData m_EnumData;

		internal Func<AbstractGenericMenu> createMenuCallback;

		public new static readonly string ussClassName = "unity-enum-field";

		public static readonly string textUssClassName = EnumField.ussClassName + "__text";

		public static readonly string arrowUssClassName = EnumField.ussClassName + "__arrow";

		public new static readonly string labelUssClassName = EnumField.ussClassName + "__label";

		public new static readonly string inputUssClassName = EnumField.ussClassName + "__input";

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : BaseField<Enum>.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				BaseField<Enum>.UxmlSerializedData.Register();
				UxmlDescriptionCache.RegisterType(typeof(EnumField.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("typeAsString", "type", typeof(Enum), Array.Empty<string>()),
					new UxmlAttributeNames("valueAsString", "value", null, Array.Empty<string>()),
					new UxmlAttributeNames("includeObsoleteValues", "include-obsolete-values", null, Array.Empty<string>())
				}, false);
			}

			public override object CreateInstance()
			{
				return new EnumField();
			}

			public override void Deserialize(object obj)
			{
				base.Deserialize(obj);
				EnumField enumField = (EnumField)obj;
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.includeObsoleteValues_UxmlAttributeFlags);
				if (flag)
				{
					enumField.includeObsoleteValues = this.includeObsoleteValues;
				}
				bool flag2 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.typeAsString_UxmlAttributeFlags);
				if (flag2)
				{
					enumField.typeAsString = this.typeAsString;
				}
				bool flag3 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.valueAsString_UxmlAttributeFlags);
				if (flag3)
				{
					enumField.valueAsString = this.valueAsString;
				}
				else
				{
					enumField.valueAsString = null;
				}
			}

			[UxmlAttribute("type")]
			[SerializeField]
			[UxmlTypeReference(typeof(Enum))]
			private string typeAsString;

			[UxmlAttribute("value")]
			[EnumFieldValueDecorator]
			[SerializeField]
			private string valueAsString;

			[SerializeField]
			private bool includeObsoleteValues;

			[UxmlIgnore]
			[HideInInspector]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags typeAsString_UxmlAttributeFlags;

			[HideInInspector]
			[UxmlIgnore]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags valueAsString_UxmlAttributeFlags;

			[UxmlIgnore]
			[SerializeField]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags includeObsoleteValues_UxmlAttributeFlags;
		}

		[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlFactory : UxmlFactory<EnumField, EnumField.UxmlTraits>
		{
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlTraits : BaseField<Enum>.UxmlTraits
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				Type type;
				Enum @enum;
				bool flag2;
				bool flag = EnumFieldHelpers.ExtractValue(bag, cc, out type, out @enum, out flag2);
				if (flag)
				{
					EnumField enumField = (EnumField)ve;
					enumField.Init(@enum, flag2);
				}
				else
				{
					bool flag3 = null != type;
					if (flag3)
					{
						EnumField enumField2 = (EnumField)ve;
						enumField2.m_EnumType = type;
						bool flag4 = enumField2.m_EnumType != null;
						if (flag4)
						{
							enumField2.PopulateDataFromType(enumField2.m_EnumType);
						}
						enumField2.value = null;
					}
					else
					{
						EnumField enumField3 = (EnumField)ve;
						enumField3.m_EnumType = null;
						enumField3.value = null;
					}
				}
			}

			private UxmlTypeAttributeDescription<Enum> m_Type = EnumFieldHelpers.type;

			private UxmlStringAttributeDescription m_Value = EnumFieldHelpers.value;

			private UxmlBoolAttributeDescription m_IncludeObsoleteValues = EnumFieldHelpers.includeObsoleteValues;
		}
	}
}
