using System;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
	public abstract class BaseBoolField : BaseField<bool>
	{
		internal Label boolFieldLabelElement
		{
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			get
			{
				return this.m_Label;
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal bool acceptClicksIfDisabled
		{
			get
			{
				return this.m_Clickable.acceptClicksIfDisabled;
			}
			set
			{
				this.m_Clickable.acceptClicksIfDisabled = value;
			}
		}

		[CreateProperty]
		public bool toggleOnLabelClick { get; set; } = true;

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal bool toggleOnTextClick { get; set; } = true;

		public BaseBoolField(string label)
			: base(label, null)
		{
			this.m_CheckMark = new VisualElement
			{
				name = "unity-checkmark",
				pickingMode = PickingMode.Ignore
			};
			base.visualInput.Add(this.m_CheckMark);
			base.visualInput.pickingMode = PickingMode.Position;
			base.labelElement.focusable = false;
			this.text = null;
			this.AddManipulator(this.m_Clickable = new Clickable(new Action<EventBase>(this.OnClickEvent)));
			base.RegisterCallback<NavigationSubmitEvent>(new EventCallback<NavigationSubmitEvent>(this.OnNavigationSubmit), TrickleDown.NoTrickleDown);
		}

		private void OnNavigationSubmit(NavigationSubmitEvent evt)
		{
			this.ToggleValue();
			evt.StopPropagation();
		}

		[CreateProperty]
		public string text
		{
			get
			{
				Label label = this.m_Label;
				return (label != null) ? label.text : null;
			}
			set
			{
				Label label = this.m_Label;
				bool flag = string.CompareOrdinal((label != null) ? label.text : null, value) == 0;
				if (!flag)
				{
					bool flag2 = !string.IsNullOrEmpty(value);
					if (flag2)
					{
						this.InitLabel();
						this.m_Label.text = value;
					}
					else
					{
						bool flag3 = this.m_Label != null;
						if (flag3)
						{
							this.m_Label.RemoveFromHierarchy();
							this.m_Label.text = value;
						}
					}
					base.NotifyPropertyChanged(in BaseBoolField.textProperty);
				}
			}
		}

		protected virtual void InitLabel()
		{
			bool flag = this.m_Label == null;
			if (flag)
			{
				this.m_Label = new Label();
			}
			else
			{
				bool flag2 = this.m_Label.parent != null;
				if (flag2)
				{
					return;
				}
			}
			bool flag3 = this.m_CheckMark.hierarchy.parent != base.visualInput;
			if (flag3)
			{
				base.visualInput.Add(this.m_Label);
			}
			else
			{
				int num = base.visualInput.IndexOf(this.m_CheckMark);
				base.visualInput.Insert(num + 1, this.m_Label);
			}
		}

		public override void SetValueWithoutNotify(bool newValue)
		{
			base.visualInput.SetCheckedPseudoState(newValue);
			base.SetCheckedPseudoState(newValue);
			base.SetValueWithoutNotify(newValue);
		}

		private void OnClickEvent(EventBase evt)
		{
			bool flag = evt.eventTypeId == EventBase<MouseUpEvent>.TypeId();
			if (flag)
			{
				IMouseEvent mouseEvent = (IMouseEvent)evt;
				bool flag2 = this.ShouldIgnoreClick(mouseEvent.mousePosition);
				if (!flag2)
				{
					bool flag3 = mouseEvent.button == 0;
					if (flag3)
					{
						this.ToggleValue();
					}
				}
			}
			else
			{
				bool flag4 = evt.eventTypeId == EventBase<PointerUpEvent>.TypeId() || evt.eventTypeId == EventBase<ClickEvent>.TypeId();
				if (flag4)
				{
					IPointerEvent pointerEvent = (IPointerEvent)evt;
					bool flag5 = this.ShouldIgnoreClick(pointerEvent.position);
					if (!flag5)
					{
						bool flag6 = pointerEvent.button == 0;
						if (flag6)
						{
							this.ToggleValue();
						}
					}
				}
			}
		}

		private bool ShouldIgnoreClick(Vector3 position)
		{
			bool flag = !this.toggleOnLabelClick && base.labelElement.worldBound.Contains(position);
			bool flag2;
			if (flag)
			{
				flag2 = true;
			}
			else
			{
				bool flag3;
				if (!this.toggleOnTextClick)
				{
					Label label = this.m_Label;
					flag3 = label != null && label.worldBound.Contains(position);
				}
				else
				{
					flag3 = false;
				}
				bool flag4 = flag3;
				flag2 = flag4;
			}
			return flag2;
		}

		protected virtual void ToggleValue()
		{
			this.value = !this.value;
		}

		protected override void UpdateMixedValueContent()
		{
			bool showMixedValue = base.showMixedValue;
			if (showMixedValue)
			{
				base.visualInput.SetCheckedPseudoState(false);
				base.SetCheckedPseudoState(false);
				this.m_CheckMark.RemoveFromHierarchy();
				base.visualInput.Add(base.mixedValueLabel);
				this.m_OriginalText = this.text;
				this.text = "";
			}
			else
			{
				base.mixedValueLabel.RemoveFromHierarchy();
				base.visualInput.Add(this.m_CheckMark);
				bool flag = this.m_OriginalText != null;
				if (flag)
				{
					this.text = this.m_OriginalText;
				}
			}
		}

		internal override void RegisterEditingCallbacks()
		{
			base.RegisterCallback<PointerUpEvent>(new EventCallback<PointerUpEvent>(base.StartEditing), TrickleDown.NoTrickleDown);
			base.RegisterCallback<FocusOutEvent>(new EventCallback<FocusOutEvent>(base.EndEditing), TrickleDown.NoTrickleDown);
		}

		internal override void UnregisterEditingCallbacks()
		{
			base.UnregisterCallback<PointerUpEvent>(new EventCallback<PointerUpEvent>(base.StartEditing), TrickleDown.NoTrickleDown);
			base.UnregisterCallback<FocusOutEvent>(new EventCallback<FocusOutEvent>(base.EndEditing), TrickleDown.NoTrickleDown);
		}

		internal static readonly BindingId textProperty = "text";

		internal static readonly BindingId toggleOnLabelClickProperty = "toggleOnLabelClick";

		protected Label m_Label;

		protected internal readonly VisualElement m_CheckMark;

		internal readonly Clickable m_Clickable;

		private string m_OriginalText;

		[ExcludeFromDocs]
		[Serializable]
		public new abstract class UxmlSerializedData : BaseField<bool>.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				BaseField<bool>.UxmlSerializedData.Register();
				UxmlDescriptionCache.RegisterType(typeof(BaseBoolField.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("toggleOnLabelClick", "toggle-on-label-click", null, Array.Empty<string>())
				}, false);
			}

			public override void Deserialize(object obj)
			{
				base.Deserialize(obj);
				BaseBoolField baseBoolField = (BaseBoolField)obj;
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.toggleOnLabelClick_UxmlAttributeFlags);
				if (flag)
				{
					baseBoolField.toggleOnLabelClick = this.toggleOnLabelClick;
				}
			}

			[SerializeField]
			private bool toggleOnLabelClick;

			[HideInInspector]
			[UxmlIgnore]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags toggleOnLabelClick_UxmlAttributeFlags;
		}
	}
}
