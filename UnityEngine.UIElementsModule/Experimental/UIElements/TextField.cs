using System;

namespace UnityEngine.Experimental.UIElements
{
	public class TextField : TextInputFieldBase<string>
	{
		public TextField()
			: this(-1, false, false, '\0')
		{
		}

		public TextField(int maxLength, bool multiline, bool isPasswordField, char maskChar)
			: base(maxLength, maskChar)
		{
			this.m_Value = "";
			this.multiline = multiline;
			this.isPasswordField = isPasswordField;
		}

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
					base.text = base.text.Replace("\n", "");
				}
			}
		}

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

		public override string value
		{
			get
			{
				return base.value;
			}
			set
			{
				base.value = value;
				base.text = this.m_Value;
			}
		}

		public override void SetValueWithoutNotify(string newValue)
		{
			base.SetValueWithoutNotify(newValue);
			base.text = this.m_Value;
		}

		public void SelectRange(int cursorIndex, int selectionIndex)
		{
			if (base.editorEngine != null)
			{
				base.editorEngine.cursorIndex = cursorIndex;
				base.editorEngine.selectIndex = selectionIndex;
			}
		}

		public override void OnPersistentDataReady()
		{
			base.OnPersistentDataReady();
			string fullHierarchicalPersistenceKey = base.GetFullHierarchicalPersistenceKey();
			base.OverwriteFromPersistedData(this, fullHierarchicalPersistenceKey);
			base.text = this.m_Value;
		}

		internal override void SyncTextEngine()
		{
			base.editorEngine.multiline = this.multiline;
			base.editorEngine.isPasswordField = this.isPasswordField;
			base.SyncTextEngine();
		}

		protected override void DoRepaint(IStylePainter painter)
		{
			IStylePainterInternal stylePainterInternal = (IStylePainterInternal)painter;
			if (this.isPasswordField)
			{
				string text = "".PadRight(base.text.Length, base.maskChar);
				if (!base.hasFocus)
				{
					if (!string.IsNullOrEmpty(text) && base.contentRect.width > 0f && base.contentRect.height > 0f)
					{
						TextStylePainterParameters @default = TextStylePainterParameters.GetDefault(this, base.text);
						@default.text = text;
						stylePainterInternal.DrawText(@default);
					}
				}
				else
				{
					base.DrawWithTextSelectionAndCursor(stylePainterInternal, text);
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
					this.value = base.text;
				}
			}
			else if (evt.GetEventTypeId() == EventBase<ExecuteCommandEvent>.TypeId())
			{
				ExecuteCommandEvent executeCommandEvent = evt as ExecuteCommandEvent;
				string commandName = executeCommandEvent.commandName;
				if (!base.isDelayed && (commandName == "Paste" || commandName == "Cut"))
				{
					this.value = base.text;
				}
			}
		}

		protected internal override void ExecuteDefaultAction(EventBase evt)
		{
			base.ExecuteDefaultAction(evt);
			if (base.isDelayed && evt.GetEventTypeId() == EventBase<BlurEvent>.TypeId())
			{
				this.value = base.text;
			}
		}

		private bool m_Multiline;

		public new class UxmlFactory : UxmlFactory<TextField, TextField.UxmlTraits>
		{
		}

		public new class UxmlTraits : TextInputFieldBase<string>.UxmlTraits
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				TextField textField = (TextField)ve;
				textField.multiline = this.m_Multiline.GetValueFromBag(bag, cc);
				textField.SetValueWithoutNotify(textField.text);
			}

			private UxmlBoolAttributeDescription m_Multiline = new UxmlBoolAttributeDescription
			{
				name = "multiline"
			};
		}
	}
}
