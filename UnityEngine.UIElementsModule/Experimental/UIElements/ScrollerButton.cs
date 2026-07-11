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

		/// <summary>
		///   <para>Instantiates a ScrollerButton using the data read from a UXML file.</para>
		/// </summary>
		public class ScrollerButtonFactory : UxmlFactory<ScrollerButton, ScrollerButton.ScrollerButtonUxmlTraits>
		{
		}

		/// <summary>
		///   <para>UxmlTraits for the ScrollerButton.</para>
		/// </summary>
		public class ScrollerButtonUxmlTraits : VisualElement.VisualElementUxmlTraits
		{
			/// <summary>
			///   <para>Constructor.</para>
			/// </summary>
			public ScrollerButtonUxmlTraits()
			{
				this.m_Delay = new UxmlLongAttributeDescription
				{
					name = "delay"
				};
				this.m_Interval = new UxmlLongAttributeDescription
				{
					name = "interval"
				};
			}

			/// <summary>
			///   <para>Returns an enumerable containing attribute descriptions for ScrollerButton properties that should be available in UXML.</para>
			/// </summary>
			public override IEnumerable<UxmlAttributeDescription> uxmlAttributesDescription
			{
				get
				{
					foreach (UxmlAttributeDescription attr in this.<get_uxmlAttributesDescription>__BaseCallProxy0())
					{
						yield return attr;
					}
					yield return this.m_Delay;
					yield return this.m_Interval;
					yield break;
				}
			}

			/// <summary>
			///   <para>Returns an empty enumerable, as buttons generally do not have children.</para>
			/// </summary>
			public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
			{
				get
				{
					yield break;
				}
			}

			/// <summary>
			///   <para>Initialize ScrollerButton properties using values from the attribute bag.</para>
			/// </summary>
			/// <param name="ve">The object to initialize.</param>
			/// <param name="bag">The attribute bag.</param>
			/// <param name="cc">The creation context; unused.</param>
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				((ScrollerButton)ve).clickable = new Clickable(null, this.m_Delay.GetValueFromBag(bag), this.m_Interval.GetValueFromBag(bag));
			}

			private UxmlLongAttributeDescription m_Delay;

			private UxmlLongAttributeDescription m_Interval;
		}
	}
}
