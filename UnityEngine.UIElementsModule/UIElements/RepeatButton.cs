using System;

namespace UnityEngine.UIElements
{
	public class RepeatButton : TextElement
	{
		internal bool acceptClicksIfDisabled
		{
			get
			{
				return this.m_AcceptClicksIfDisabled;
			}
			set
			{
				bool flag = this.m_AcceptClicksIfDisabled == value;
				if (!flag)
				{
					this.m_AcceptClicksIfDisabled = value;
					bool flag2 = this.m_Clickable != null;
					if (flag2)
					{
						this.m_Clickable.acceptClicksIfDisabled = value;
					}
				}
			}
		}

		public RepeatButton()
		{
			base.AddToClassList(RepeatButton.ussClassName);
		}

		public RepeatButton(Action clickEvent, long delay, long interval)
			: this()
		{
			this.SetAction(clickEvent, delay, interval);
		}

		public void SetAction(Action clickEvent, long delay, long interval)
		{
			this.RemoveManipulator(this.m_Clickable);
			this.m_Clickable = new Clickable(clickEvent, delay, interval);
			this.AddManipulator(this.m_Clickable);
		}

		internal void AddAction(Action clickEvent)
		{
			this.m_Clickable.clicked += clickEvent;
		}

		private Clickable m_Clickable;

		private bool m_AcceptClicksIfDisabled;

		public new static readonly string ussClassName = "unity-repeat-button";

		public new class UxmlFactory : UxmlFactory<RepeatButton, RepeatButton.UxmlTraits>
		{
		}

		public new class UxmlTraits : TextElement.UxmlTraits
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				RepeatButton repeatButton = (RepeatButton)ve;
				repeatButton.SetAction(null, this.m_Delay.GetValueFromBag(bag, cc), this.m_Interval.GetValueFromBag(bag, cc));
			}

			private UxmlLongAttributeDescription m_Delay = new UxmlLongAttributeDescription
			{
				name = "delay"
			};

			private UxmlLongAttributeDescription m_Interval = new UxmlLongAttributeDescription
			{
				name = "interval"
			};
		}
	}
}
