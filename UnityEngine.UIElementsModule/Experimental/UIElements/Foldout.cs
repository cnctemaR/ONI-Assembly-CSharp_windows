using System;
using UnityEngine.Experimental.UIElements.StyleEnums;
using UnityEngine.Internal;

namespace UnityEngine.Experimental.UIElements
{
	public class Foldout : BindableElement, INotifyValueChanged<bool>
	{
		public Foldout()
		{
			this.m_Toggle = new Toggle();
			this.m_Toggle.value = true;
			this.m_Toggle.OnValueChanged(delegate(ChangeEvent<bool> evt)
			{
				this.value = this.m_Toggle.value;
				evt.StopPropagation();
			});
			this.m_Toggle.AddToClassList(Foldout.s_ToggleClassName);
			base.shadow.Add(this.m_Toggle);
			this.m_Container = new VisualElement();
			this.m_Container.clippingOptions = VisualElement.ClippingOptions.ClipContents;
			this.m_Container.AddToClassList(Foldout.s_ContentContainerClassName);
			base.shadow.Add(this.m_Container);
			base.AddToClassList(Foldout.s_FoldoutClassName);
		}

		public override VisualElement contentContainer
		{
			get
			{
				return this.m_Container;
			}
		}

		public string text
		{
			get
			{
				return this.m_Toggle.text;
			}
			set
			{
				this.m_Toggle.text = value;
			}
		}

		public bool value
		{
			get
			{
				return this.m_Value;
			}
			set
			{
				if (this.m_Value != value)
				{
					using (ChangeEvent<bool> pooled = ChangeEvent<bool>.GetPooled(this.m_Value, value))
					{
						pooled.target = this;
						this.SetValueWithoutNotify(value);
						this.SendEvent(pooled);
					}
				}
			}
		}

		[ExcludeFromDocs]
		public void SetValueAndNotify(bool newValue)
		{
			this.value = newValue;
		}

		public void SetValueWithoutNotify(bool newValue)
		{
			this.m_Value = newValue;
			this.m_Toggle.value = this.m_Value;
			if (this.m_Value)
			{
				this.contentContainer.visible = true;
				this.contentContainer.style.positionType = PositionType.Relative;
			}
			else
			{
				this.contentContainer.visible = false;
				this.contentContainer.style.positionType = PositionType.Absolute;
			}
		}

		public void OnValueChanged(EventCallback<ChangeEvent<bool>> callback)
		{
			base.RegisterCallback<ChangeEvent<bool>>(callback, TrickleDown.NoTrickleDown);
		}

		public void RemoveOnValueChanged(EventCallback<ChangeEvent<bool>> callback)
		{
			base.UnregisterCallback<ChangeEvent<bool>>(callback, TrickleDown.NoTrickleDown);
		}

		private static readonly string s_FoldoutClassName = "unity-foldout";

		private static readonly string s_ToggleClassName = "unity-foldout-toggle";

		private static readonly string s_ContentContainerClassName = "unity-foldout-content";

		private Toggle m_Toggle;

		private VisualElement m_Container;

		private bool m_Value = true;

		public new class UxmlFactory : UxmlFactory<Foldout, BindableElement.UxmlTraits>
		{
		}
	}
}
