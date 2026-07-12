using System;
using System.Collections.Generic;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.UIElements
{
	[MovedFrom(true, "UnityEditor.UIElements", "UnityEditor.UIElementsModule", null)]
	public abstract class BaseCompositeField<TValueType, TField, TFieldValue> : BaseField<TValueType> where TField : TextValueField<TFieldValue>, new()
	{
		private VisualElement GetSpacer()
		{
			VisualElement visualElement = new VisualElement();
			visualElement.AddToClassList(BaseCompositeField<TValueType, TField, TFieldValue>.spacerUssClassName);
			visualElement.visible = false;
			visualElement.focusable = false;
			return visualElement;
		}

		internal List<TField> fields
		{
			get
			{
				return this.m_Fields;
			}
		}

		internal abstract BaseCompositeField<TValueType, TField, TFieldValue>.FieldDescription[] DescribeFields();

		internal int propertyIndex
		{
			get
			{
				return this.m_PropertyIndex;
			}
			set
			{
				this.m_PropertyIndex = value;
			}
		}

		internal bool forceUpdateDisplay
		{
			get
			{
				return this.m_ForceUpdateDisplay;
			}
			set
			{
				this.m_ForceUpdateDisplay = value;
			}
		}

		protected BaseCompositeField(string label, int fieldsByLine)
			: base(label, null)
		{
			base.delegatesFocus = false;
			base.visualInput.focusable = false;
			base.AddToClassList(BaseCompositeField<TValueType, TField, TFieldValue>.ussClassName);
			base.labelElement.AddToClassList(BaseCompositeField<TValueType, TField, TFieldValue>.labelUssClassName);
			base.visualInput.AddToClassList(BaseCompositeField<TValueType, TField, TFieldValue>.inputUssClassName);
			this.m_ShouldUpdateDisplay = true;
			this.m_Fields = new List<TField>();
			BaseCompositeField<TValueType, TField, TFieldValue>.FieldDescription[] array = this.DescribeFields();
			int num = 1;
			bool flag = fieldsByLine > 1;
			if (flag)
			{
				num = array.Length / fieldsByLine;
			}
			bool flag2 = false;
			bool flag3 = num > 1;
			if (flag3)
			{
				flag2 = true;
				base.AddToClassList(BaseCompositeField<TValueType, TField, TFieldValue>.multilineVariantUssClassName);
			}
			this.m_PropertyIndex = 0;
			for (int i = 0; i < num; i++)
			{
				VisualElement visualElement = null;
				bool flag4 = flag2;
				if (flag4)
				{
					visualElement = new VisualElement();
					visualElement.AddToClassList(BaseCompositeField<TValueType, TField, TFieldValue>.fieldGroupUssClassName);
				}
				bool flag5 = true;
				for (int j = i * fieldsByLine; j < i * fieldsByLine + fieldsByLine; j++)
				{
					BaseCompositeField<TValueType, TField, TFieldValue>.<>c__DisplayClass24_0 CS$<>8__locals1 = new BaseCompositeField<TValueType, TField, TFieldValue>.<>c__DisplayClass24_0();
					CS$<>8__locals1.<>4__this = this;
					CS$<>8__locals1.desc = array[j];
					BaseCompositeField<TValueType, TField, TFieldValue>.<>c__DisplayClass24_0 CS$<>8__locals2 = CS$<>8__locals1;
					TField tfield = new TField();
					tfield.name = CS$<>8__locals1.desc.ussName;
					CS$<>8__locals2.field = tfield;
					CS$<>8__locals1.field.delegatesFocus = true;
					CS$<>8__locals1.field.AddToClassList(BaseCompositeField<TValueType, TField, TFieldValue>.fieldUssClassName);
					bool flag6 = flag5;
					if (flag6)
					{
						CS$<>8__locals1.field.AddToClassList(BaseCompositeField<TValueType, TField, TFieldValue>.firstFieldVariantUssClassName);
						flag5 = false;
					}
					CS$<>8__locals1.field.label = CS$<>8__locals1.desc.name;
					CS$<>8__locals1.field.onValidateValue += delegate(TFieldValue newValue)
					{
						TValueType value = CS$<>8__locals1.<>4__this.value;
						CS$<>8__locals1.desc.write(ref value, newValue);
						TValueType tvalueType = CS$<>8__locals1.<>4__this.ValidatedValue(value);
						return CS$<>8__locals1.desc.read(tvalueType);
					};
					CS$<>8__locals1.field.RegisterValueChangedCallback<TFieldValue>(delegate(ChangeEvent<TFieldValue> e)
					{
						TValueType value2 = CS$<>8__locals1.<>4__this.value;
						CS$<>8__locals1.desc.write(ref value2, e.newValue);
						TFieldValue newValue = e.newValue;
						string text = newValue.ToString();
						string text2 = ((TField)((object)e.currentTarget)).text;
						bool flag11 = text != text2 || CS$<>8__locals1.field.CanTryParse(text2);
						if (flag11)
						{
							CS$<>8__locals1.<>4__this.m_ShouldUpdateDisplay = false;
						}
						CS$<>8__locals1.<>4__this.value = value2;
						CS$<>8__locals1.<>4__this.m_ShouldUpdateDisplay = true;
					});
					this.m_Fields.Add(CS$<>8__locals1.field);
					bool flag7 = flag2;
					if (flag7)
					{
						visualElement.Add(CS$<>8__locals1.field);
					}
					else
					{
						base.visualInput.hierarchy.Add(CS$<>8__locals1.field);
					}
				}
				bool flag8 = fieldsByLine < 3;
				if (flag8)
				{
					int num2 = 3 - fieldsByLine;
					for (int k = 0; k < num2; k++)
					{
						bool flag9 = flag2;
						if (flag9)
						{
							visualElement.Add(this.GetSpacer());
						}
						else
						{
							base.visualInput.hierarchy.Add(this.GetSpacer());
						}
					}
				}
				bool flag10 = flag2;
				if (flag10)
				{
					base.visualInput.hierarchy.Add(visualElement);
				}
			}
			this.UpdateDisplay();
		}

		private void UpdateDisplay()
		{
			bool flag = this.m_Fields.Count != 0;
			if (flag)
			{
				int num = 0;
				BaseCompositeField<TValueType, TField, TFieldValue>.FieldDescription[] array = this.DescribeFields();
				foreach (BaseCompositeField<TValueType, TField, TFieldValue>.FieldDescription fieldDescription in array)
				{
					this.m_Fields[num].SetValueWithoutNotify(fieldDescription.read(base.rawValue));
					num++;
				}
			}
		}

		public override void SetValueWithoutNotify(TValueType newValue)
		{
			bool flag = this.m_ForceUpdateDisplay || (this.m_ShouldUpdateDisplay && !EqualityComparer<TValueType>.Default.Equals(base.rawValue, newValue));
			base.SetValueWithoutNotify(newValue);
			bool flag2 = flag;
			if (flag2)
			{
				this.UpdateDisplay();
			}
			this.m_ForceUpdateDisplay = false;
		}

		internal override void OnViewDataReady()
		{
			this.m_ForceUpdateDisplay = true;
			base.OnViewDataReady();
		}

		protected override void UpdateMixedValueContent()
		{
			foreach (TField tfield in this.m_Fields)
			{
				tfield.showMixedValue = base.showMixedValue;
			}
		}

		private List<TField> m_Fields;

		private bool m_ShouldUpdateDisplay;

		private bool m_ForceUpdateDisplay;

		private int m_PropertyIndex;

		public new static readonly string ussClassName = "unity-composite-field";

		public new static readonly string labelUssClassName = BaseCompositeField<TValueType, TField, TFieldValue>.ussClassName + "__label";

		public new static readonly string inputUssClassName = BaseCompositeField<TValueType, TField, TFieldValue>.ussClassName + "__input";

		public static readonly string spacerUssClassName = BaseCompositeField<TValueType, TField, TFieldValue>.ussClassName + "__field-spacer";

		public static readonly string multilineVariantUssClassName = BaseCompositeField<TValueType, TField, TFieldValue>.ussClassName + "--multi-line";

		public static readonly string fieldGroupUssClassName = BaseCompositeField<TValueType, TField, TFieldValue>.ussClassName + "__field-group";

		public static readonly string fieldUssClassName = BaseCompositeField<TValueType, TField, TFieldValue>.ussClassName + "__field";

		public static readonly string firstFieldVariantUssClassName = BaseCompositeField<TValueType, TField, TFieldValue>.fieldUssClassName + "--first";

		public static readonly string twoLinesVariantUssClassName = BaseCompositeField<TValueType, TField, TFieldValue>.ussClassName + "--two-lines";

		internal struct FieldDescription
		{
			public FieldDescription(string name, string ussName, Func<TValueType, TFieldValue> read, BaseCompositeField<TValueType, TField, TFieldValue>.FieldDescription.WriteDelegate write)
			{
				this.name = name;
				this.ussName = ussName;
				this.read = read;
				this.write = write;
			}

			internal readonly string name;

			internal readonly string ussName;

			internal readonly Func<TValueType, TFieldValue> read;

			internal readonly BaseCompositeField<TValueType, TField, TFieldValue>.FieldDescription.WriteDelegate write;

			public delegate void WriteDelegate(ref TValueType val, TFieldValue fieldValue);
		}
	}
}
