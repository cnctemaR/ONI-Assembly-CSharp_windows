using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	public class Toggle : BaseControl<bool>
	{
		public Toggle()
			: this(null)
		{
		}

		public Toggle(Action clickEvent)
		{
			this.clickEvent = clickEvent;
			this.m_Label = new Label();
			base.Add(this.m_Label);
			this.AddManipulator(new Clickable(new Action(this.OnClick)));
		}

		[Obsolete("Use value instead", false)]
		public bool on
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = value;
			}
		}

		/// <summary>
		///   <para>Optional text after the toggle.</para>
		/// </summary>
		public string text
		{
			get
			{
				return this.m_Label.text;
			}
			set
			{
				this.m_Label.text = value;
			}
		}

		/// <summary>
		///   <para>Return whether the toggle is on or not.</para>
		/// </summary>
		public override bool value
		{
			get
			{
				return (base.pseudoStates & PseudoStates.Checked) == PseudoStates.Checked;
			}
			set
			{
				if (value)
				{
					base.pseudoStates |= PseudoStates.Checked;
				}
				else
				{
					base.pseudoStates &= ~PseudoStates.Checked;
				}
			}
		}

		/// <summary>
		///   <para>Sets the event callback for this toggle button.</para>
		/// </summary>
		/// <param name="clickEvent">The action to be called when this Toggle is clicked.</param>
		public void OnToggle(Action clickEvent)
		{
			this.clickEvent = clickEvent;
		}

		private void OnClick()
		{
			this.value = !this.value;
			if (this.clickEvent != null)
			{
				this.clickEvent();
			}
		}

		protected internal override void ExecuteDefaultAction(EventBase evt)
		{
			base.ExecuteDefaultAction(evt);
			KeyDownEvent keyDownEvent = evt as KeyDownEvent;
			char? c = ((keyDownEvent != null) ? new char?(keyDownEvent.character) : null);
			if (((c == null) ? null : new int?((int)c.Value)) == 10)
			{
				this.OnClick();
			}
		}

		private Action clickEvent;

		private Label m_Label;

		/// <summary>
		///   <para>Instantiates a Toggle using the data read from a UXML file.</para>
		/// </summary>
		public class ToggleFactory : UxmlFactory<Toggle, Toggle.ToggleUxmlTraits>
		{
		}

		/// <summary>
		///   <para>UxmlTraits for the Toggle.</para>
		/// </summary>
		public class ToggleUxmlTraits : BaseControl<bool>.BaseControlUxmlTraits
		{
			/// <summary>
			///   <para>Constructor.</para>
			/// </summary>
			public ToggleUxmlTraits()
			{
				this.m_Value = new UxmlBoolAttributeDescription
				{
					name = "value"
				};
				this.m_Label = new UxmlStringAttributeDescription
				{
					name = "label"
				};
			}

			/// <summary>
			///   <para>Returns an enumerable containing attribute descriptions for Toggle properties that should be available in UXML.</para>
			/// </summary>
			public override IEnumerable<UxmlAttributeDescription> uxmlAttributesDescription
			{
				get
				{
					foreach (UxmlAttributeDescription attr in this.<get_uxmlAttributesDescription>__BaseCallProxy0())
					{
						yield return attr;
					}
					yield return this.m_Label;
					yield return this.m_Value;
					yield break;
				}
			}

			/// <summary>
			///   <para>Initialize Toggle properties using values from the attribute bag.</para>
			/// </summary>
			/// <param name="ve">The object to initialize.</param>
			/// <param name="bag">The attribute bag.</param>
			/// <param name="cc">The creation context; unused.</param>
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				((Toggle)ve).m_Label.text = this.m_Label.GetValueFromBag(bag);
				((Toggle)ve).value = this.m_Value.GetValueFromBag(bag);
			}

			private UxmlStringAttributeDescription m_Label;

			private UxmlBoolAttributeDescription m_Value;
		}
	}
}
