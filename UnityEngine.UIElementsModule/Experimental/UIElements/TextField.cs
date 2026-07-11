using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>A textfield is a rectangular area where the user can edit a string.</para>
	/// </summary>
	public class TextField : TextInputFieldBase<string>
	{
		/// <summary>
		///   <para>Creates a new textfield.</para>
		/// </summary>
		/// <param name="maxLength">The maximum number of characters this textfield can hold. If 0, there is no limit.</param>
		/// <param name="multiline">Set this to true to allow multiple lines in the textfield and false if otherwise.</param>
		/// <param name="isPasswordField">Set this to true to mask the characters and false if otherwise.</param>
		/// <param name="maskChar">The character used for masking in a password field.</param>
		public TextField()
			: this(-1, false, false, '\0')
		{
		}

		/// <summary>
		///   <para>Creates a new textfield.</para>
		/// </summary>
		/// <param name="maxLength">The maximum number of characters this textfield can hold. If 0, there is no limit.</param>
		/// <param name="multiline">Set this to true to allow multiple lines in the textfield and false if otherwise.</param>
		/// <param name="isPasswordField">Set this to true to mask the characters and false if otherwise.</param>
		/// <param name="maskChar">The character used for masking in a password field.</param>
		public TextField(int maxLength, bool multiline, bool isPasswordField, char maskChar)
			: base(maxLength, maskChar)
		{
			this.m_Value = "";
			this.multiline = multiline;
			this.isPasswordField = isPasswordField;
		}

		/// <summary>
		///   <para>Set this to true to allow multiple lines in the textfield and false if otherwise.</para>
		/// </summary>
		public bool multiline
		{
			get
			{
				return this.m_Multiline;
			}
			set
			{
				this.m_Multiline = value;
				if (!value)
				{
					this.text = this.text.Replace("\n", "");
				}
			}
		}

		/// <summary>
		///   <para>Set this to true to mask the characters and false if otherwise.</para>
		/// </summary>
		public override bool isPasswordField
		{
			set
			{
				base.isPasswordField = value;
				if (value)
				{
					this.multiline = false;
				}
			}
		}

		/// <summary>
		///   <para>The string currently being exposed by the field.</para>
		/// </summary>
		public override string value
		{
			get
			{
				return this.m_Value;
			}
			set
			{
				this.m_Value = value;
				this.text = this.m_Value;
			}
		}

		/// <summary>
		///   <para>Called when the persistent data is accessible and/or when the data or persistence key have changed (VisualElement is properly parented).</para>
		/// </summary>
		public override void OnPersistentDataReady()
		{
			base.OnPersistentDataReady();
			string fullHierarchicalPersistenceKey = base.GetFullHierarchicalPersistenceKey();
			base.OverwriteFromPersistedData(this, fullHierarchicalPersistenceKey);
		}

		internal override void SyncTextEngine()
		{
			base.editorEngine.multiline = this.multiline;
			base.editorEngine.isPasswordField = this.isPasswordField;
			base.SyncTextEngine();
		}

		internal override void DoRepaint(IStylePainter painter)
		{
			if (this.isPasswordField)
			{
				string text = "".PadRight(this.text.Length, base.maskChar);
				if (!base.hasFocus)
				{
					painter.DrawBackground(this);
					painter.DrawBorder(this);
					if (!string.IsNullOrEmpty(text) && base.contentRect.width > 0f && base.contentRect.height > 0f)
					{
						TextStylePainterParameters defaultTextParameters = painter.GetDefaultTextParameters(this);
						defaultTextParameters.text = text;
						painter.DrawText(defaultTextParameters);
					}
				}
				else
				{
					base.DrawWithTextSelectionAndCursor(painter, text);
				}
			}
			else
			{
				base.DoRepaint(painter);
			}
		}

		protected internal override void ExecuteDefaultActionAtTarget(EventBase evt)
		{
			base.ExecuteDefaultActionAtTarget(evt);
			if (evt.GetEventTypeId() == EventBase<KeyDownEvent>.TypeId())
			{
				KeyDownEvent keyDownEvent = evt as KeyDownEvent;
				if (!base.isDelayed || keyDownEvent.character == '\n')
				{
					this.SetValueAndNotify(this.text);
				}
			}
			else if (evt.GetEventTypeId() == EventBase<ExecuteCommandEvent>.TypeId())
			{
				ExecuteCommandEvent executeCommandEvent = evt as ExecuteCommandEvent;
				string commandName = executeCommandEvent.commandName;
				if (!base.isDelayed && (commandName == "Paste" || commandName == "Cut"))
				{
					this.SetValueAndNotify(this.text);
				}
			}
		}

		protected internal override void ExecuteDefaultAction(EventBase evt)
		{
			base.ExecuteDefaultAction(evt);
			if (!base.isDelayed || evt.GetEventTypeId() == EventBase<BlurEvent>.TypeId())
			{
				this.SetValueAndNotify(this.text);
			}
		}

		private bool m_Multiline;

		protected string m_Value;

		/// <summary>
		///   <para>Instantiates a TextField using the data read from a UXML file.</para>
		/// </summary>
		public class TextFieldFactory : UxmlFactory<TextField, TextField.TextFieldUxmlTraits>
		{
		}

		/// <summary>
		///   <para>UxmlTraits for the TextField.</para>
		/// </summary>
		public class TextFieldUxmlTraits : TextInputFieldBase<string>.TextInputFieldBaseUxmlTraits
		{
			/// <summary>
			///   <para>Constructor.</para>
			/// </summary>
			public TextFieldUxmlTraits()
			{
				this.m_Multiline = new UxmlBoolAttributeDescription
				{
					name = "multiline"
				};
			}

			/// <summary>
			///   <para>Returns an enumerable containing attribute descriptions for TextField properties that should be available in UXML.</para>
			/// </summary>
			public override IEnumerable<UxmlAttributeDescription> uxmlAttributesDescription
			{
				get
				{
					foreach (UxmlAttributeDescription attr in this.<get_uxmlAttributesDescription>__BaseCallProxy0())
					{
						yield return attr;
					}
					yield return this.m_Multiline;
					yield break;
				}
			}

			/// <summary>
			///   <para>Initialize TextField properties using values from the attribute bag.</para>
			/// </summary>
			/// <param name="ve">The object to initialize.</param>
			/// <param name="bag">The attribute bag.</param>
			/// <param name="cc">The creation context; unused.</param>
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				TextField textField = (TextField)ve;
				textField.multiline = this.m_Multiline.GetValueFromBag(bag);
				textField.value = textField.text;
			}

			private UxmlBoolAttributeDescription m_Multiline;
		}
	}
}
