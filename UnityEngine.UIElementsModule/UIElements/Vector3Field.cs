using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.UIElements
{
	[MovedFrom(true, "UnityEditor.UIElements", "UnityEditor.UIElementsModule", null)]
	public class Vector3Field : BaseCompositeField<Vector3, FloatField, float>
	{
		internal override BaseCompositeField<Vector3, FloatField, float>.FieldDescription[] DescribeFields()
		{
			BaseCompositeField<Vector3, FloatField, float>.FieldDescription[] array = new BaseCompositeField<Vector3, FloatField, float>.FieldDescription[3];
			array[0] = new BaseCompositeField<Vector3, FloatField, float>.FieldDescription("X", "unity-x-input", (Vector3 r) => r.x, delegate(ref Vector3 r, float v)
			{
				r.x = v;
			});
			array[1] = new BaseCompositeField<Vector3, FloatField, float>.FieldDescription("Y", "unity-y-input", (Vector3 r) => r.y, delegate(ref Vector3 r, float v)
			{
				r.y = v;
			});
			array[2] = new BaseCompositeField<Vector3, FloatField, float>.FieldDescription("Z", "unity-z-input", (Vector3 r) => r.z, delegate(ref Vector3 r, float v)
			{
				r.z = v;
			});
			return array;
		}

		public Vector3Field()
			: this(null)
		{
		}

		public Vector3Field(string label)
			: base(label, 3)
		{
			base.AddToClassList(Vector3Field.ussClassName);
			base.labelElement.AddToClassList(Vector3Field.labelUssClassName);
			base.visualInput.AddToClassList(Vector3Field.inputUssClassName);
		}

		public new static readonly string ussClassName = "unity-vector3-field";

		public new static readonly string labelUssClassName = Vector3Field.ussClassName + "__label";

		public new static readonly string inputUssClassName = Vector3Field.ussClassName + "__input";

		public new class UxmlFactory : UxmlFactory<Vector3Field, Vector3Field.UxmlTraits>
		{
		}

		public new class UxmlTraits : BaseField<Vector3>.UxmlTraits
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				Vector3Field vector3Field = (Vector3Field)ve;
				vector3Field.SetValueWithoutNotify(new Vector3(this.m_XValue.GetValueFromBag(bag, cc), this.m_YValue.GetValueFromBag(bag, cc), this.m_ZValue.GetValueFromBag(bag, cc)));
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
		}
	}
}
