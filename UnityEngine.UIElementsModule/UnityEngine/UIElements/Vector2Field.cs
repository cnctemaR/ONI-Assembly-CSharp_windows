using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Internal;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.UIElements
{
	[MovedFrom(true, "UnityEditor.UIElements", "UnityEditor.UIElementsModule", null)]
	public class Vector2Field : BaseCompositeField<Vector2, FloatField, float>
	{
		internal override BaseCompositeField<Vector2, FloatField, float>.FieldDescription[] DescribeFields()
		{
			BaseCompositeField<Vector2, FloatField, float>.FieldDescription[] array = new BaseCompositeField<Vector2, FloatField, float>.FieldDescription[2];
			array[0] = new BaseCompositeField<Vector2, FloatField, float>.FieldDescription("X", "unity-x-input", (Vector2 r) => r.x, delegate(ref Vector2 r, float v)
			{
				r.x = v;
			});
			array[1] = new BaseCompositeField<Vector2, FloatField, float>.FieldDescription("Y", "unity-y-input", (Vector2 r) => r.y, delegate(ref Vector2 r, float v)
			{
				r.y = v;
			});
			return array;
		}

		public Vector2Field()
			: this(null)
		{
		}

		public Vector2Field(string label)
			: base(label, 2)
		{
			base.AddToClassList(Vector2Field.ussClassName);
			base.labelElement.AddToClassList(Vector2Field.labelUssClassName);
			base.visualInput.AddToClassList(Vector2Field.inputUssClassName);
		}

		public new static readonly string ussClassName = "unity-vector2-field";

		public new static readonly string labelUssClassName = Vector2Field.ussClassName + "__label";

		public new static readonly string inputUssClassName = Vector2Field.ussClassName + "__input";

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : BaseCompositeField<Vector2, FloatField, float>.UxmlSerializedData, IUxmlSerializedDataCustomAttributeHandler
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
			}

			public override object CreateInstance()
			{
				return new Vector2Field();
			}

			void IUxmlSerializedDataCustomAttributeHandler.SerializeCustomAttributes(IUxmlAttributes bag, HashSet<string> handledAttributes)
			{
				int num = 0;
				float num2 = UxmlUtility.TryParseFloatAttribute("x", bag, ref num);
				float num3 = UxmlUtility.TryParseFloatAttribute("y", bag, ref num);
				bool flag = num > 0;
				if (flag)
				{
					base.Value = new Vector2(num2, num3);
					handledAttributes.Add("value");
					UxmlAsset uxmlAsset = bag as UxmlAsset;
					bool flag2 = uxmlAsset != null;
					if (flag2)
					{
						uxmlAsset.RemoveAttribute("x");
						uxmlAsset.RemoveAttribute("y");
						uxmlAsset.SetAttribute("value", UxmlUtility.ValueToString(base.Value));
					}
				}
			}
		}

		[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlFactory : UxmlFactory<Vector2Field, Vector2Field.UxmlTraits>
		{
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlTraits : BaseField<Vector2>.UxmlTraits
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				Vector2Field vector2Field = (Vector2Field)ve;
				vector2Field.SetValueWithoutNotify(new Vector2(this.m_XValue.GetValueFromBag(bag, cc), this.m_YValue.GetValueFromBag(bag, cc)));
			}

			private UxmlFloatAttributeDescription m_XValue = new UxmlFloatAttributeDescription
			{
				name = "x"
			};

			private UxmlFloatAttributeDescription m_YValue = new UxmlFloatAttributeDescription
			{
				name = "y"
			};
		}
	}
}
