using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.UIElements
{
	[MovedFrom(true, "UnityEditor.UIElements", "UnityEditor.UIElementsModule", null)]
	public class Vector4Field : BaseCompositeField<Vector4, FloatField, float>
	{
		internal override BaseCompositeField<Vector4, FloatField, float>.FieldDescription[] DescribeFields()
		{
			BaseCompositeField<Vector4, FloatField, float>.FieldDescription[] array = new BaseCompositeField<Vector4, FloatField, float>.FieldDescription[4];
			array[0] = new BaseCompositeField<Vector4, FloatField, float>.FieldDescription("X", "unity-x-input", (Vector4 r) => r.x, delegate(ref Vector4 r, float v)
			{
				r.x = v;
			});
			array[1] = new BaseCompositeField<Vector4, FloatField, float>.FieldDescription("Y", "unity-y-input", (Vector4 r) => r.y, delegate(ref Vector4 r, float v)
			{
				r.y = v;
			});
			array[2] = new BaseCompositeField<Vector4, FloatField, float>.FieldDescription("Z", "unity-z-input", (Vector4 r) => r.z, delegate(ref Vector4 r, float v)
			{
				r.z = v;
			});
			array[3] = new BaseCompositeField<Vector4, FloatField, float>.FieldDescription("W", "unity-w-input", (Vector4 r) => r.w, delegate(ref Vector4 r, float v)
			{
				r.w = v;
			});
			return array;
		}

		public Vector4Field()
			: this(null)
		{
		}

		public Vector4Field(string label)
			: base(label, 4)
		{
			base.AddToClassList(Vector4Field.ussClassName);
			base.labelElement.AddToClassList(Vector4Field.labelUssClassName);
			base.visualInput.AddToClassList(Vector4Field.inputUssClassName);
		}

		public new static readonly string ussClassName = "unity-vector4-field";

		public new static readonly string labelUssClassName = Vector4Field.ussClassName + "__label";

		public new static readonly string inputUssClassName = Vector4Field.ussClassName + "__input";

		public new class UxmlFactory : UxmlFactory<Vector4Field, Vector4Field.UxmlTraits>
		{
		}

		public new class UxmlTraits : BaseField<Vector4>.UxmlTraits
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				Vector4Field vector4Field = (Vector4Field)ve;
				vector4Field.SetValueWithoutNotify(new Vector4(this.m_XValue.GetValueFromBag(bag, cc), this.m_YValue.GetValueFromBag(bag, cc), this.m_ZValue.GetValueFromBag(bag, cc), this.m_WValue.GetValueFromBag(bag, cc)));
			}

			private UxmlFloatAttributeDescription m_XValue = new UxmlFloatAttributeDescription
			{
				name = "x"
			};

			private UxmlFloatAttributeDescription m_YValue = new UxmlFloatAttributeDescription
			{
				name = "y"
			};

			private UxmlFloatAttributeDescription m_ZValue = new UxmlFloatAttributeDescription
			{
				name = "z"
			};

			private UxmlFloatAttributeDescription m_WValue = new UxmlFloatAttributeDescription
			{
				name = "w"
			};
		}
	}
}
