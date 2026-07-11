using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>A button that executes an action repeatedly while it is pressed.</para>
	/// </summary>
	public class RepeatButton : BaseTextElement
	{
		/// <summary>
		///   <para>Constructor.</para>
		/// </summary>
		/// <param name="clickEvent">The action to execute when the button is pressed.</param>
		/// <param name="delay">The initial delay before the action is executed for the first time.</param>
		/// <param name="interval">The interval between each execution of the action.</param>
		public RepeatButton()
		{
		}

		/// <summary>
		///   <para>Constructor.</para>
		/// </summary>
		/// <param name="clickEvent">The action to execute when the button is pressed.</param>
		/// <param name="delay">The initial delay before the action is executed for the first time.</param>
		/// <param name="interval">The interval between each execution of the action.</param>
		public RepeatButton(Action clickEvent, long delay, long interval)
		{
			this.SetAction(clickEvent, delay, interval);
		}

		/// <summary>
		///   <para>Set the action that should be executed when the button is pressed.</para>
		/// </summary>
		/// <param name="clickEvent">The action to execute.</param>
		/// <param name="delay">The initial delay before the action is executed for the first time.</param>
		/// <param name="interval">The interval between each execution of the action.</param>
		public void SetAction(Action clickEvent, long delay, long interval)
		{
			this.RemoveManipulator(this.m_Clickable);
			this.m_Clickable = new Clickable(clickEvent, delay, interval);
			this.AddManipulator(this.m_Clickable);
		}

		private Clickable m_Clickable;

		/// <summary>
		///   <para>Instantiates a RepeatButton using the data read from a UXML file.</para>
		/// </summary>
		public class RepeatButtonFactory : UxmlFactory<RepeatButton, RepeatButton.RepeatButtonUxmlTraits>
		{
		}

		/// <summary>
		///   <para>UxmlTraits for the RepeatButton.</para>
		/// </summary>
		public class RepeatButtonUxmlTraits : BaseTextElement.BaseTextElementUxmlTraits
		{
			/// <summary>
			///   <para>Constructor.</para>
			/// </summary>
			public RepeatButtonUxmlTraits()
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
			///   <para>Returns an enumerable containing attribute descriptions for RepeatButton properties that should be available in UXML.</para>
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
			///   <para>Initialize RepeatButton properties using values from the attribute bag.</para>
			/// </summary>
			/// <param name="ve">The object to initialize.</param>
			/// <param name="bag">The attribute bag.</param>
			/// <param name="cc">The creation context; unused.</param>
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				RepeatButton repeatButton = (RepeatButton)ve;
				repeatButton.SetAction(null, this.m_Delay.GetValueFromBag(bag), this.m_Interval.GetValueFromBag(bag));
			}

			private UxmlLongAttributeDescription m_Delay;

			private UxmlLongAttributeDescription m_Interval;
		}
	}
}
