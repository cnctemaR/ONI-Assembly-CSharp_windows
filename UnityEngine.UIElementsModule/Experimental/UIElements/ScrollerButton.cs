using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	public class ScrollerButton : VisualElement
	{
		public ScrollerButton()
		{
		}

		public ScrollerButton(Action clickEvent, long delay, long interval)
		{
			this.clickable = new Clickable(clickEvent, delay, interval);
			this.AddManipulator(this.clickable);
		}

		public Clickable clickable;

		public new class UxmlFactory : UxmlFactory<ScrollerButton, ScrollerButton.UxmlTraits>
		{
		}

		public new class UxmlTraits : VisualElement.UxmlTraits
		{
			public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
			{
				get
				{
					yield break;
				}
			}

			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				((ScrollerButton)ve).clickable = new Clickable(null, this.m_Delay.GetValueFromBag(bag, cc), this.m_Interval.GetValueFromBag(bag, cc));
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
