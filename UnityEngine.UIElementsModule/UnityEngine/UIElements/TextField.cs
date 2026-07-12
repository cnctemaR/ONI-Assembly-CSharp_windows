using System;

namespace UnityEngine.UIElements
{
	public class TextField : TextInputBaseField<string>
	{
		private TextField.TextInput textInput
		{
			get
			{
				return (TextField.TextInput)base.textInputBase;
			}
		}

		public bool multiline
		{
			get
			{
				return this.textInput.multiline;
			}
			set
			{
				this.textInput.multiline = value;
			}
		}

		public void SelectRange(int rangeCursorIndex, int selectionIndex)
		{
			this.textInput.SelectRange(rangeCursorIndex, selectionIndex);
		}

		public TextField()
			: this(null)
		{
		}

		public TextField(int maxLength, bool multiline, bool isPasswordField, char maskChar)
			: this(null, maxLength, multiline, isPasswordField, maskChar)
		{
		}

		public TextField(string label)
			: this(label, -1, false, false, '*')
		{
		}

		public TextField(string label, int maxLength, bool multiline, bool isPasswordField, char maskChar)
			: base(label, maxLength, maskChar, new TextField.TextInput())
		{
			base.AddToClassList(TextField.ussClassName);
			base.labelElement.AddToClassList(TextField.labelUssClassName);
			base.visualInput.AddToClassList(TextField.inputUssClassName);
			base.pickingMode = PickingMode.Ignore;
			this.SetValueWithoutNotify("");
			this.multiline = multiline;
			base.isPasswordField = isPasswordField;
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
				base.text = base.rawValue;
			}
		}

		public override void SetValueWithoutNotify(string newValue)
		{
			base.SetValueWithoutNotify(newValue);
			base.text = base.rawValue;
		}

		internal override void OnViewDataReady()
		{
			base.OnViewDataReady();
			string fullHierarchicalViewDataKey = base.GetFullHierarchicalViewDataKey();
			base.OverwriteFromViewData(this, fullHierarchicalViewDataKey);
			base.text = base.rawValue;
		}

		public new static readonly string ussClassName = "unity-text-field";

		public new static readonly string labelUssClassName = TextField.ussClassName + "__label";

		public new static readonly string inputUssClassName = TextField.ussClassName + "__input";

		public new class UxmlFactory : UxmlFactory<TextField, TextField.UxmlTraits>
		{
		}

		public new class UxmlTraits : TextInputBaseField<string>.UxmlTraits
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				TextField textField = (TextField)ve;
				textField.multiline = this.m_Multiline.GetValueFromBag(bag, cc);
				base.Init(ve, bag, cc);
			}

			private UxmlBoolAttributeDescription m_Multiline = new UxmlBoolAttributeDescription
			{
				name = "multiline"
			};
		}

		private class TextInput : TextInputBaseField<string>.TextInputBase
		{
			private TextField parentTextField
			{
				get
				{
					return (TextField)base.parent;
				}
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
					bool flag = !value;
					if (flag)
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

			protected override string StringToValue(string str)
			{
				return str;
			}

			public void SelectRange(int cursorIndex, int selectionIndex)
			{
				bool flag = base.editorEngine != null;
				if (flag)
				{
					base.editorEngine.cursorIndex = cursorIndex;
					base.editorEngine.selectIndex = selectionIndex;
				}
			}

			internal override void SyncTextEngine()
			{
				bool flag = this.parentTextField != null;
				if (flag)
				{
					base.editorEngine.multiline = this.multiline;
					base.editorEngine.isPasswordField = this.isPasswordField;
				}
				base.SyncTextEngine();
			}

			protected override void ExecuteDefaultActionAtTarget(EventBase evt)
			{
				base.ExecuteDefaultActionAtTarget(evt);
				bool flag = evt == null;
				if (!flag)
				{
					bool flag2 = evt.eventTypeId == EventBase<KeyDownEvent>.TypeId();
					if (flag2)
					{
						KeyDownEvent keyDownEvent = evt as KeyDownEvent;
						bool flag3 = !this.parentTextField.isDelayed || (!this.multiline && ((keyDownEvent != null && keyDownEvent.keyCode == KeyCode.KeypadEnter) || (keyDownEvent != null && keyDownEvent.keyCode == KeyCode.Return)));
						if (flag3)
						{
							this.parentTextField.value = base.text;
						}
						bool multiline = this.multiline;
						if (multiline)
						{
							char? c = ((keyDownEvent != null) ? new char?(keyDownEvent.character) : null);
							int? num = ((c != null) ? new int?((int)c.GetValueOrDefault()) : null);
							int num2 = 9;
							bool flag4 = ((num.GetValueOrDefault() == num2) & (num != null)) && keyDownEvent.modifiers == EventModifiers.None;
							if (flag4)
							{
								if (keyDownEvent != null)
								{
									keyDownEvent.StopPropagation();
								}
								if (keyDownEvent != null)
								{
									keyDownEvent.PreventDefault();
								}
							}
							else
							{
								c = ((keyDownEvent != null) ? new char?(keyDownEvent.character) : null);
								num = ((c != null) ? new int?((int)c.GetValueOrDefault()) : null);
								num2 = 3;
								bool flag5;
								if (!((num.GetValueOrDefault() == num2) & (num != null)) || keyDownEvent == null || !keyDownEvent.shiftKey)
								{
									c = ((keyDownEvent != null) ? new char?(keyDownEvent.character) : null);
									num = ((c != null) ? new int?((int)c.GetValueOrDefault()) : null);
									num2 = 10;
									flag5 = ((num.GetValueOrDefault() == num2) & (num != null)) && keyDownEvent != null && keyDownEvent.shiftKey;
								}
								else
								{
									flag5 = true;
								}
								bool flag6 = flag5;
								if (flag6)
								{
									base.parent.Focus();
								}
							}
						}
						else
						{
							char? c = ((keyDownEvent != null) ? new char?(keyDownEvent.character) : null);
							int? num = ((c != null) ? new int?((int)c.GetValueOrDefault()) : null);
							int num2 = 3;
							bool flag7;
							if (!((num.GetValueOrDefault() == num2) & (num != null)))
							{
								c = ((keyDownEvent != null) ? new char?(keyDownEvent.character) : null);
								num = ((c != null) ? new int?((int)c.GetValueOrDefault()) : null);
								num2 = 10;
								flag7 = (num.GetValueOrDefault() == num2) & (num != null);
							}
							else
							{
								flag7 = true;
							}
							bool flag8 = flag7;
							if (flag8)
							{
								base.parent.Focus();
							}
						}
					}
					else
					{
						bool flag9 = evt.eventTypeId == EventBase<ExecuteCommandEvent>.TypeId();
						if (flag9)
						{
							ExecuteCommandEvent executeCommandEvent = evt as ExecuteCommandEvent;
							string commandName = executeCommandEvent.commandName;
							bool flag10 = !this.parentTextField.isDelayed && (commandName == "Paste" || commandName == "Cut");
							if (flag10)
							{
								this.parentTextField.value = base.text;
							}
						}
						else
						{
							NavigationDirection navigationDirection;
							bool flag11 = base.eventInterpreter.IsActivationEvent(evt) || base.eventInterpreter.IsCancellationEvent(evt) || (base.eventInterpreter.IsNavigationEvent(evt, out navigationDirection) && navigationDirection != NavigationDirection.Previous && navigationDirection != NavigationDirection.Next);
							if (flag11)
							{
								evt.StopPropagation();
								evt.PreventDefault();
							}
						}
					}
				}
			}

			protected override void ExecuteDefaultAction(EventBase evt)
			{
				base.ExecuteDefaultAction(evt);
				bool flag;
				if (this.parentTextField.isDelayed)
				{
					long? num = ((evt != null) ? new long?(evt.eventTypeId) : null);
					long num2 = EventBase<BlurEvent>.TypeId();
					flag = (num.GetValueOrDefault() == num2) & (num != null);
				}
				else
				{
					flag = false;
				}
				bool flag2 = flag;
				if (flag2)
				{
					this.parentTextField.value = base.text;
				}
			}

			private bool m_Multiline;
		}
	}
}
