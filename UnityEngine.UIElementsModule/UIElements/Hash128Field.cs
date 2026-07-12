using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.UIElements
{
	[MovedFrom(true, "UnityEditor.UIElements", "UnityEditor.UIElementsModule", null)]
	public class Hash128Field : TextInputBaseField<Hash128>
	{
		private Hash128Field.Hash128Input integerInput
		{
			get
			{
				return (Hash128Field.Hash128Input)base.textInputBase;
			}
		}

		public Hash128Field()
			: this(null, -1)
		{
		}

		public Hash128Field(int maxLength)
			: this(null, maxLength)
		{
		}

		public Hash128Field(string label, int maxLength = -1)
			: base(label, maxLength, '\0', new Hash128Field.Hash128Input())
		{
			this.m_UpdateTextFromValue = true;
			this.SetValueWithoutNotify(default(Hash128));
			base.AddToClassList(Hash128Field.ussClassName);
			base.labelElement.AddToClassList(Hash128Field.labelUssClassName);
			base.visualInput.AddToClassList(Hash128Field.inputUssClassName);
		}

		public override Hash128 value
		{
			get
			{
				return base.value;
			}
			set
			{
				base.value = value;
				bool updateTextFromValue = this.m_UpdateTextFromValue;
				if (updateTextFromValue)
				{
					base.text = base.rawValue.ToString();
				}
			}
		}

		internal override void UpdateValueFromText()
		{
			this.m_UpdateTextFromValue = false;
			try
			{
				this.value = this.StringToValue(base.text);
			}
			finally
			{
				this.m_UpdateTextFromValue = true;
			}
		}

		internal override void UpdateTextFromValue()
		{
			base.text = this.ValueToString(base.rawValue);
		}

		public override void SetValueWithoutNotify(Hash128 newValue)
		{
			base.SetValueWithoutNotify(newValue);
			bool updateTextFromValue = this.m_UpdateTextFromValue;
			if (updateTextFromValue)
			{
				base.text = base.rawValue.ToString();
			}
		}

		protected override string ValueToString(Hash128 value)
		{
			return value.ToString();
		}

		protected override Hash128 StringToValue(string str)
		{
			return Hash128Field.Hash128Input.Parse(str);
		}

		[EventInterest(new Type[] { typeof(BlurEvent) })]
		protected override void ExecuteDefaultAction(EventBase evt)
		{
			base.ExecuteDefaultAction(evt);
			bool flag = evt == null || base.textEdition.isReadOnly;
			if (!flag)
			{
				bool flag2 = evt.eventTypeId == EventBase<BlurEvent>.TypeId();
				if (flag2)
				{
					bool flag3 = string.IsNullOrEmpty(base.text);
					if (flag3)
					{
						this.value = default(Hash128);
					}
					else
					{
						base.textInputBase.UpdateValueFromText();
						base.textInputBase.UpdateTextFromValue();
					}
				}
			}
		}

		internal bool m_UpdateTextFromValue;

		public new static readonly string ussClassName = "unity-hash128-field";

		public new static readonly string labelUssClassName = Hash128Field.ussClassName + "__label";

		public new static readonly string inputUssClassName = Hash128Field.ussClassName + "__input";

		public new class UxmlFactory : UxmlFactory<Hash128Field, Hash128Field.UxmlTraits>
		{
		}

		public new class UxmlTraits : TextValueFieldTraits<Hash128, UxmlHash128AttributeDescription>
		{
		}

		private class Hash128Input : TextInputBaseField<Hash128>.TextInputBase
		{
			private Hash128Field hash128Field
			{
				get
				{
					return (Hash128Field)base.parent;
				}
			}

			internal Hash128Input()
			{
				base.textEdition.AcceptCharacter = new Func<char, bool>(this.AcceptCharacter);
			}

			protected string allowedCharacters
			{
				get
				{
					return "0123456789abcdefABCDEF";
				}
			}

			internal override bool AcceptCharacter(char c)
			{
				return base.AcceptCharacter(c) && c != '\0' && this.allowedCharacters.IndexOf(c) != -1;
			}

			public string formatString
			{
				get
				{
					return UINumericFieldsUtils.k_IntFieldFormatString;
				}
			}

			protected string ValueToString(Hash128 value)
			{
				return value.ToString();
			}

			protected override Hash128 StringToValue(string str)
			{
				return Hash128Field.Hash128Input.Parse(str);
			}

			internal static Hash128 Parse(string str)
			{
				ulong num;
				bool flag = str.Length == 1 && ulong.TryParse(str, out num);
				Hash128 hash;
				if (flag)
				{
					hash = new Hash128(num, 0UL);
				}
				else
				{
					hash = Hash128.Parse(str);
				}
				return hash;
			}
		}
	}
}
