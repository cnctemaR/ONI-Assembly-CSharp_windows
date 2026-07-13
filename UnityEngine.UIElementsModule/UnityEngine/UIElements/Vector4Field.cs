using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Internal;
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

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : BaseCompositeField<Vector4, FloatField, float>.UxmlSerializedData, IUxmlSerializedDataCustomAttributeHandler
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
			}

			public override object CreateInstance()
			{
				return new Vector4Field();
			}

			void IUxmlSerializedDataCustomAttributeHandler.SerializeCustomAttributes(IUxmlAttributes bag, HashSet<string> handledAttributes)
			{
				int num = 0;
				float num2 = UxmlUtility.TryParseFloatAttribute("x", bag, ref num);
				float num3 = UxmlUtility.TryParseFloatAttribute("y", bag, ref num);
				float num4 = UxmlUtility.TryParseFloatAttribute("z", bag, ref num);
				float num5 = UxmlUtility.TryParseFloatAttribute("w", bag, ref num);
				bool flag = num > 0;
				if (flag)
				{
					base.Value = new Vector4(num2, num3, num4, num5);
					handledAttributes.Add("value");
					UxmlAsset uxmlAsset = bag as UxmlAsset;
					bool flag2 = uxmlAsset != null;
					if (flag2)
					{
						uxmlAsset.RemoveAttribute("x");
						uxmlAsset.RemoveAttribute("y");
						uxmlAsset.RemoveAttribute("z");
						uxmlAsset.RemoveAttribute("w");
						uxmlAsset.SetAttribute("value", UxmlUtility.ValueToString(base.Value));
					}
				}
			}
		}

		[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlFactory : UxmlFactory<Vector4Field, Vector4Field.UxmlTraits>
		{
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
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
