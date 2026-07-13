using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.UIElements
{
	[MovedFrom(true, "UnityEditor.UIElements", "UnityEditor.UIElementsModule", null)]
	public class EnumField : BaseField<Enum>
	{
		internal Type type
		{
			get
			{
				return this.m_EnumType;
			}
		}

		internal bool includeObsoleteValues
		{
			get
			{
				return this.m_IncludeObsoleteValues;
			}
		}

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

		internal void PopulateDataFromType(Type enumType)
		{
			this.m_EnumType = enumType;
			this.m_EnumData = EnumDataUtility.GetCachedEnumData(this.m_EnumType, this.includeObsoleteValues ? EnumDataUtility.CachedType.IncludeObsoleteExceptErrors : EnumDataUtility.CachedType.ExcludeObsolete, null);
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
					this.ShowMenu();
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
				IGenericMenu genericMenu = ((this.createMenuCallback != null) ? this.createMenuCallback() : base.elementPanel.CreateMenu());
				int num = Array.IndexOf<Enum>(this.m_EnumData.values, this.value);
				for (int i = 0; i < this.m_EnumData.values.Length; i++)
				{
					bool flag2 = num == i;
					genericMenu.AddItem(this.m_EnumData.displayNames[i], flag2, delegate(object contentView)
					{
						this.ChangeValueFromMenu(contentView);
					}, this.m_EnumData.values[i]);
				}
				genericMenu.DropDown(base.visualInput.worldBound, this, true);
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

		private Type m_EnumType;

		private bool m_IncludeObsoleteValues;

		private TextElement m_TextElement;

		private VisualElement m_ArrowElement;

		private EnumData m_EnumData;

		internal Func<IGenericMenu> createMenuCallback;

		public new static readonly string ussClassName = "unity-enum-field";

		public static readonly string textUssClassName = EnumField.ussClassName + "__text";

		public static readonly string arrowUssClassName = EnumField.ussClassName + "__arrow";

		public new static readonly string labelUssClassName = EnumField.ussClassName + "__label";

		public new static readonly string inputUssClassName = EnumField.ussClassName + "__input";

		public new class UxmlFactory : UxmlFactory<EnumField, EnumField.UxmlTraits>
		{
		}

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
