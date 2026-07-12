using System;
using System.Collections.Generic;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.UIElements
{
	[MovedFrom(true, "UnityEditor.UIElements", "UnityEditor.UIElementsModule", null)]
	public abstract class BasePopupField<TValueType, TValueChoice> : BaseField<TValueType>
	{
		protected TextElement textElement
		{
			get
			{
				return this.m_TextElement;
			}
		}

		internal abstract string GetValueToDisplay();

		internal abstract string GetListItemToDisplay(TValueType item);

		internal abstract void AddMenuItems(IGenericMenu menu);

		public virtual List<TValueChoice> choices
		{
			get
			{
				return this.m_Choices;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("value");
				}
				this.m_Choices = value;
				this.SetValueWithoutNotify(base.rawValue);
			}
		}

		public override void SetValueWithoutNotify(TValueType newValue)
		{
			base.SetValueWithoutNotify(newValue);
			((INotifyValueChanged<string>)this.m_TextElement).SetValueWithoutNotify(this.GetValueToDisplay());
		}

		public string text
		{
			get
			{
				return this.m_TextElement.text;
			}
		}

		internal BasePopupField()
			: this(null)
		{
		}

		internal BasePopupField(string label)
			: base(label, null)
		{
			base.AddToClassList(BasePopupField<TValueType, TValueChoice>.ussClassName);
			base.labelElement.AddToClassList(BasePopupField<TValueType, TValueChoice>.labelUssClassName);
			this.m_TextElement = new BasePopupField<TValueType, TValueChoice>.PopupTextElement
			{
				pickingMode = PickingMode.Ignore
			};
			this.m_TextElement.AddToClassList(BasePopupField<TValueType, TValueChoice>.textUssClassName);
			base.visualInput.AddToClassList(BasePopupField<TValueType, TValueChoice>.inputUssClassName);
			base.visualInput.Add(this.m_TextElement);
			this.m_ArrowElement = new VisualElement();
			this.m_ArrowElement.AddToClassList(BasePopupField<TValueType, TValueChoice>.arrowUssClassName);
			this.m_ArrowElement.pickingMode = PickingMode.Ignore;
			base.visualInput.Add(this.m_ArrowElement);
			this.choices = new List<TValueChoice>();
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
			bool flag = this.createMenuCallback != null;
			IGenericMenu genericMenu;
			if (flag)
			{
				genericMenu = this.createMenuCallback();
			}
			else
			{
				BaseVisualElementPanel elementPanel = base.elementPanel;
				IGenericMenu genericMenu2;
				if (elementPanel == null || elementPanel.contextType != ContextType.Player)
				{
					genericMenu2 = DropdownUtility.CreateDropdown();
				}
				else
				{
					IGenericMenu genericMenu3 = new GenericDropdownMenu();
					genericMenu2 = genericMenu3;
				}
				genericMenu = genericMenu2;
			}
			this.AddMenuItems(genericMenu);
			genericMenu.DropDown(base.visualInput.worldBound, this, true);
		}

		protected override void UpdateMixedValueContent()
		{
			bool showMixedValue = base.showMixedValue;
			if (showMixedValue)
			{
				((INotifyValueChanged<string>)this.m_TextElement).SetValueWithoutNotify(BaseField<TValueType>.mixedValueString);
			}
			this.textElement.EnableInClassList(BaseField<TValueType>.mixedValueLabelUssClassName, base.showMixedValue);
		}

		internal List<TValueChoice> m_Choices;

		private TextElement m_TextElement;

		private VisualElement m_ArrowElement;

		internal Func<TValueChoice, string> m_FormatSelectedValueCallback;

		internal Func<TValueChoice, string> m_FormatListItemCallback;

		internal Func<IGenericMenu> createMenuCallback;

		public new static readonly string ussClassName = "unity-base-popup-field";

		public static readonly string textUssClassName = BasePopupField<TValueType, TValueChoice>.ussClassName + "__text";

		public static readonly string arrowUssClassName = BasePopupField<TValueType, TValueChoice>.ussClassName + "__arrow";

		public new static readonly string labelUssClassName = BasePopupField<TValueType, TValueChoice>.ussClassName + "__label";

		public new static readonly string inputUssClassName = BasePopupField<TValueType, TValueChoice>.ussClassName + "__input";

		private class PopupTextElement : TextElement
		{
			protected internal override Vector2 DoMeasure(float desiredWidth, VisualElement.MeasureMode widthMode, float desiredHeight, VisualElement.MeasureMode heightMode)
			{
				string text = this.text;
				bool flag = string.IsNullOrEmpty(text);
				if (flag)
				{
					text = " ";
				}
				return base.MeasureTextSize(text, desiredWidth, widthMode, desiredHeight, heightMode);
			}
		}
	}
}
