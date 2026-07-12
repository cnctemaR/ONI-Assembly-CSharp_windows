using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.UIElements
{
	[MovedFrom(true, "UnityEditor.UIElements", "UnityEditor.UIElementsModule", null)]
	public class RectIntField : BaseCompositeField<RectInt, IntegerField, int>
	{
		internal override BaseCompositeField<RectInt, IntegerField, int>.FieldDescription[] DescribeFields()
		{
			BaseCompositeField<RectInt, IntegerField, int>.FieldDescription[] array = new BaseCompositeField<RectInt, IntegerField, int>.FieldDescription[4];
			array[0] = new BaseCompositeField<RectInt, IntegerField, int>.FieldDescription("X", "unity-x-input", (RectInt r) => r.x, delegate(ref RectInt r, int v)
			{
				r.x = v;
			});
			array[1] = new BaseCompositeField<RectInt, IntegerField, int>.FieldDescription("Y", "unity-y-input", (RectInt r) => r.y, delegate(ref RectInt r, int v)
			{
				r.y = v;
			});
			array[2] = new BaseCompositeField<RectInt, IntegerField, int>.FieldDescription("W", "unity-width-input", (RectInt r) => r.width, delegate(ref RectInt r, int v)
			{
				r.width = v;
			});
			array[3] = new BaseCompositeField<RectInt, IntegerField, int>.FieldDescription("H", "unity-height-input", (RectInt r) => r.height, delegate(ref RectInt r, int v)
			{
				r.height = v;
			});
			return array;
		}

		public RectIntField()
			: this(null)
		{
		}

		public RectIntField(string label)
			: base(label, 2)
		{
			base.AddToClassList(RectIntField.ussClassName);
			base.AddToClassList(BaseCompositeField<RectInt, IntegerField, int>.twoLinesVariantUssClassName);
			base.labelElement.AddToClassList(RectIntField.labelUssClassName);
			base.visualInput.AddToClassList(RectIntField.inputUssClassName);
		}

		public new static readonly string ussClassName = "unity-rect-int-field";

		public new static readonly string labelUssClassName = RectIntField.ussClassName + "__label";

		public new static readonly string inputUssClassName = RectIntField.ussClassName + "__input";

		public new class UxmlFactory : UxmlFactory<RectIntField, RectIntField.UxmlTraits>
		{
		}

		public new class UxmlTraits : BaseField<RectInt>.UxmlTraits
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				RectIntField rectIntField = (RectIntField)ve;
				rectIntField.SetValueWithoutNotify(new RectInt(this.m_XValue.GetValueFromBag(bag, cc), this.m_YValue.GetValueFromBag(bag, cc), this.m_WValue.GetValueFromBag(bag, cc), this.m_HValue.GetValueFromBag(bag, cc)));
			}

			private UxmlIntAttributeDescription m_XValue = new UxmlIntAttributeDescription
			{
				name = "x"
			};

			private UxmlIntAttributeDescription m_YValue = new UxmlIntAttributeDescription
			{
				name = "y"
			};

			private UxmlIntAttributeDescription m_WValue = new UxmlIntAttributeDescription
			{
				name = "w"
			};

			private UxmlIntAttributeDescription m_HValue = new UxmlIntAttributeDescription
			{
				name = "h"
			};
		}
	}
}
