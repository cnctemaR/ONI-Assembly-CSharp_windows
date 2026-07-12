using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.UIElements
{
	[MovedFrom(true, "UnityEditor.UIElements", "UnityEditor.UIElementsModule", null)]
	public class RectField : BaseCompositeField<Rect, FloatField, float>
	{
		internal override BaseCompositeField<Rect, FloatField, float>.FieldDescription[] DescribeFields()
		{
			BaseCompositeField<Rect, FloatField, float>.FieldDescription[] array = new BaseCompositeField<Rect, FloatField, float>.FieldDescription[4];
			array[0] = new BaseCompositeField<Rect, FloatField, float>.FieldDescription("X", "unity-x-input", (Rect r) => r.x, delegate(ref Rect r, float v)
			{
				r.x = v;
			});
			array[1] = new BaseCompositeField<Rect, FloatField, float>.FieldDescription("Y", "unity-y-input", (Rect r) => r.y, delegate(ref Rect r, float v)
			{
				r.y = v;
			});
			array[2] = new BaseCompositeField<Rect, FloatField, float>.FieldDescription("W", "unity-width-input", (Rect r) => r.width, delegate(ref Rect r, float v)
			{
				r.width = v;
			});
			array[3] = new BaseCompositeField<Rect, FloatField, float>.FieldDescription("H", "unity-height-input", (Rect r) => r.height, delegate(ref Rect r, float v)
			{
				r.height = v;
			});
			return array;
		}

		public RectField()
			: this(null)
		{
		}

		public RectField(string label)
			: base(label, 2)
		{
			base.AddToClassList(RectField.ussClassName);
			base.AddToClassList(BaseCompositeField<Rect, FloatField, float>.twoLinesVariantUssClassName);
			base.labelElement.AddToClassList(RectField.labelUssClassName);
			base.visualInput.AddToClassList(RectField.inputUssClassName);
		}

		public new static readonly string ussClassName = "unity-rect-field";

		public new static readonly string labelUssClassName = RectField.ussClassName + "__label";

		public new static readonly string inputUssClassName = RectField.ussClassName + "__input";

		public new class UxmlFactory : UxmlFactory<RectField, RectField.UxmlTraits>
		{
		}

		public new class UxmlTraits : BaseField<Rect>.UxmlTraits
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				RectField rectField = (RectField)ve;
				rectField.SetValueWithoutNotify(new Rect(this.m_XValue.GetValueFromBag(bag, cc), this.m_YValue.GetValueFromBag(bag, cc), this.m_WValue.GetValueFromBag(bag, cc), this.m_HValue.GetValueFromBag(bag, cc)));
			}

			private UxmlFloatAttributeDescription m_XValue = new UxmlFloatAttributeDescription
			{
				name = "x"
			};

			private UxmlFloatAttributeDescription m_YValue = new UxmlFloatAttributeDescription
			{
				name = "y"
			};

			private UxmlFloatAttributeDescription m_WValue = new UxmlFloatAttributeDescription
			{
				name = "w"
			};

			private UxmlFloatAttributeDescription m_HValue = new UxmlFloatAttributeDescription
			{
				name = "h"
			};
		}
	}
}
