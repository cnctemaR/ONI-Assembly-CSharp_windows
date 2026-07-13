using System;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
	public class Foldout : BindableElement, INotifyValueChanged<bool>
	{
		internal Toggle toggle
		{
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			get
			{
				return this.m_Toggle;
			}
		}

		public override VisualElement contentContainer
		{
			get
			{
				return this.m_Container;
			}
		}

		public override bool focusable
		{
			get
			{
				return base.focusable;
			}
			set
			{
				base.focusable = value;
				this.m_Toggle.focusable = value;
			}
		}

		[CreateProperty]
		public bool toggleOnLabelClick
		{
			get
			{
				return this.m_Toggle.toggleOnTextClick;
			}
			set
			{
				bool flag = this.m_Toggle.toggleOnTextClick == value;
				if (!flag)
				{
					this.m_Toggle.toggleOnTextClick = value;
					base.NotifyPropertyChanged(in Foldout.toggleOnLabelClickProperty);
				}
			}
		}

		[CreateProperty]
		public string text
		{
			get
			{
				return this.m_Toggle.text;
			}
			set
			{
				string text = this.text;
				this.m_Toggle.text = value;
				VisualElement visualElement = this.m_Toggle.visualInput.Q(null, Toggle.textUssClassName);
				if (visualElement != null)
				{
					visualElement.AddToClassList(Foldout.textUssClassName);
				}
				bool flag = string.CompareOrdinal(text, this.text) != 0;
				if (flag)
				{
					base.NotifyPropertyChanged(in Foldout.textProperty);
				}
			}
		}

		[CreateProperty]
		public bool value
		{
			get
			{
				return this.m_Value;
			}
			set
			{
				bool flag = this.m_Value == value;
				if (!flag)
				{
					using (ChangeEvent<bool> pooled = ChangeEvent<bool>.GetPooled(this.m_Value, value))
					{
						pooled.elementTarget = this;
						this.SetValueWithoutNotify(value);
						this.SendEvent(pooled);
						base.SaveViewData();
						base.NotifyPropertyChanged(in Foldout.valueProperty);
					}
				}
			}
		}

		public void SetValueWithoutNotify(bool newValue)
		{
			this.m_Value = newValue;
			this.m_Toggle.SetValueWithoutNotify(this.m_Value);
			this.contentContainer.style.display = (newValue ? DisplayStyle.Flex : DisplayStyle.None);
			base.SetCheckedPseudoState(this.m_Value);
		}

		internal override void OnViewDataReady()
		{
			base.OnViewDataReady();
			string fullHierarchicalViewDataKey = base.GetFullHierarchicalViewDataKey();
			base.OverwriteFromViewData(this, fullHierarchicalViewDataKey);
			this.SetValueWithoutNotify(this.m_Value);
		}

		private void Apply(KeyboardNavigationOperation op, EventBase sourceEvent)
		{
			bool flag = this.Apply(op);
			if (flag)
			{
				sourceEvent.StopPropagation();
				this.focusController.IgnoreEvent(sourceEvent);
			}
		}

		private bool Apply(KeyboardNavigationOperation op)
		{
			bool flag;
			switch (op)
			{
			case KeyboardNavigationOperation.SelectAll:
			case KeyboardNavigationOperation.Cancel:
			case KeyboardNavigationOperation.Submit:
			case KeyboardNavigationOperation.Previous:
			case KeyboardNavigationOperation.Next:
			case KeyboardNavigationOperation.PageUp:
			case KeyboardNavigationOperation.PageDown:
			case KeyboardNavigationOperation.Begin:
			case KeyboardNavigationOperation.End:
				flag = false;
				break;
			case KeyboardNavigationOperation.MoveRight:
				this.SetValueWithoutNotify(true);
				flag = true;
				break;
			case KeyboardNavigationOperation.MoveLeft:
				this.SetValueWithoutNotify(false);
				flag = true;
				break;
			default:
				throw new ArgumentOutOfRangeException("op", op, null);
			}
			return flag;
		}

		public Foldout()
		{
			base.AddToClassList(Foldout.ussClassName);
			base.delegatesFocus = true;
			this.focusable = true;
			base.isEligibleToReceiveFocusFromDisabledChild = false;
			this.m_Container = new VisualElement
			{
				name = "unity-content"
			};
			this.m_Toggle.RegisterValueChangedCallback<bool>(delegate(ChangeEvent<bool> evt)
			{
				this.value = this.m_Toggle.value;
				evt.StopPropagation();
			});
			this.m_Toggle.AddToClassList(Foldout.toggleUssClassName);
			this.m_Toggle.visualInput.AddToClassList(Foldout.inputUssClassName);
			this.m_Toggle.visualInput.Q(null, Toggle.checkmarkUssClassName).AddToClassList(Foldout.checkmarkUssClassName);
			this.m_Toggle.AddManipulator(this.m_NavigationManipulator = new KeyboardNavigationManipulator(new Action<KeyboardNavigationOperation, EventBase>(this.Apply)));
			base.hierarchy.Add(this.m_Toggle);
			this.m_Container.AddToClassList(Foldout.contentUssClassName);
			base.hierarchy.Add(this.m_Container);
			base.RegisterCallback<AttachToPanelEvent>(new EventCallback<AttachToPanelEvent>(this.OnAttachToPanel), TrickleDown.NoTrickleDown);
			this.SetValueWithoutNotify(true);
		}

		private void OnAttachToPanel(AttachToPanelEvent evt)
		{
			for (int i = 0; i <= Foldout.ussFoldoutMaxDepth; i++)
			{
				base.RemoveFromClassList(Foldout.ussFoldoutDepthClassName + i.ToString());
			}
			base.RemoveFromClassList(Foldout.ussFoldoutDepthClassName + "max");
			this.m_Toggle.AssignInspectorStyleIfNecessary(Foldout.toggleInspectorUssClassName);
			int foldoutDepth = this.GetFoldoutDepth();
			bool flag = foldoutDepth > Foldout.ussFoldoutMaxDepth;
			if (flag)
			{
				base.AddToClassList(Foldout.ussFoldoutDepthClassName + "max");
			}
			else
			{
				base.AddToClassList(Foldout.ussFoldoutDepthClassName + foldoutDepth.ToString());
			}
		}

		internal static readonly BindingId textProperty = "text";

		internal static readonly BindingId toggleOnLabelClickProperty = "toggleOnLabelClick";

		internal static readonly BindingId valueProperty = "value";

		private readonly Toggle m_Toggle = new Toggle
		{
			acceptClicksIfDisabled = true
		};

		private VisualElement m_Container;

		[SerializeField]
		[DontCreateProperty]
		private bool m_Value;

		public static readonly string ussClassName = "unity-foldout";

		public static readonly string toggleUssClassName = Foldout.ussClassName + "__toggle";

		public static readonly string contentUssClassName = Foldout.ussClassName + "__content";

		public static readonly string inputUssClassName = Foldout.ussClassName + "__input";

		public static readonly string checkmarkUssClassName = Foldout.ussClassName + "__checkmark";

		public static readonly string textUssClassName = Foldout.ussClassName + "__text";

		internal static readonly string toggleInspectorUssClassName = Foldout.toggleUssClassName + "--inspector";

		internal static readonly string ussFoldoutDepthClassName = Foldout.ussClassName + "--depth-";

		internal static readonly int ussFoldoutMaxDepth = 4;

		private KeyboardNavigationManipulator m_NavigationManipulator;

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : BindableElement.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				UxmlDescriptionCache.RegisterType(typeof(Foldout.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("text", "text", null, Array.Empty<string>()),
					new UxmlAttributeNames("toggleOnLabelClick", "toggle-on-label-click", null, Array.Empty<string>()),
					new UxmlAttributeNames("value", "value", null, Array.Empty<string>())
				}, false);
			}

			public override object CreateInstance()
			{
				return new Foldout();
			}

			public override void Deserialize(object obj)
			{
				base.Deserialize(obj);
				Foldout foldout = (Foldout)obj;
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.text_UxmlAttributeFlags);
				if (flag)
				{
					foldout.text = this.text;
				}
				bool flag2 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.toggleOnLabelClick_UxmlAttributeFlags);
				if (flag2)
				{
					foldout.toggleOnLabelClick = this.toggleOnLabelClick;
				}
				bool flag3 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.value_UxmlAttributeFlags);
				if (flag3)
				{
					foldout.SetValueWithoutNotify(this.value);
				}
			}

			[SerializeField]
			[MultilineTextField]
			private string text;

			[UxmlIgnore]
			[HideInInspector]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags text_UxmlAttributeFlags;

			[SerializeField]
			private bool toggleOnLabelClick;

			[HideInInspector]
			[UxmlIgnore]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags toggleOnLabelClick_UxmlAttributeFlags;

			[SerializeField]
			private bool value;

			[HideInInspector]
			[SerializeField]
			[UxmlIgnore]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags value_UxmlAttributeFlags;
		}

		[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlFactory : UxmlFactory<Foldout, Foldout.UxmlTraits>
		{
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlTraits : BindableElement.UxmlTraits
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				Foldout foldout = ve as Foldout;
				bool flag = foldout != null;
				if (flag)
				{
					foldout.text = this.m_Text.GetValueFromBag(bag, cc);
					foldout.SetValueWithoutNotify(this.m_Value.GetValueFromBag(bag, cc));
				}
			}

			private UxmlStringAttributeDescription m_Text = new UxmlStringAttributeDescription
			{
				name = "text"
			};

			private UxmlBoolAttributeDescription m_Value = new UxmlBoolAttributeDescription
			{
				name = "value",
				defaultValue = true
			};
		}
	}
}
