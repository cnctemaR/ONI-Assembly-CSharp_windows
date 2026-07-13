using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Internal;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.UIElements
{
	[MovedFrom(true, "UnityEditor.UIElements", "UnityEditor.UIElementsModule", null)]
	public class Vector2IntField : BaseCompositeField<Vector2Int, IntegerField, int>
	{
		internal override BaseCompositeField<Vector2Int, IntegerField, int>.FieldDescription[] DescribeFields()
		{
			BaseCompositeField<Vector2Int, IntegerField, int>.FieldDescription[] array = new BaseCompositeField<Vector2Int, IntegerField, int>.FieldDescription[2];
			array[0] = new BaseCompositeField<Vector2Int, IntegerField, int>.FieldDescription("X", "unity-x-input", (Vector2Int r) => r.x, delegate(ref Vector2Int r, int v)
			{
				r.x = v;
			});
			array[1] = new BaseCompositeField<Vector2Int, IntegerField, int>.FieldDescription("Y", "unity-y-input", (Vector2Int r) => r.y, delegate(ref Vector2Int r, int v)
			{
				r.y = v;
			});
			return array;
		}

		public Vector2IntField()
			: this(null)
		{
		}

		public Vector2IntField(string label)
			: base(label, 2)
		{
			base.AddToClassList(Vector2IntField.ussClassName);
			base.labelElement.AddToClassList(Vector2IntField.labelUssClassName);
			base.visualInput.AddToClassList(Vector2IntField.inputUssClassName);
		}

		public new static readonly string ussClassName = "unity-vector2-int-field";

		public new static readonly string labelUssClassName = Vector2IntField.ussClassName + "__label";

		public new static readonly string inputUssClassName = Vector2IntField.ussClassName + "__input";

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : BaseCompositeField<Vector2Int, IntegerField, int>.UxmlSerializedData, IUxmlSerializedDataCustomAttributeHandler
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
			}

			public override object CreateInstance()
			{
				return new Vector2IntField();
			}

			void IUxmlSerializedDataCustomAttributeHandler.SerializeCustomAttributes(IUxmlAttributes bag, HashSet<string> handledAttributes)
			{
				int num = 0;
				int num2 = UxmlUtility.TryParseIntAttribute("x", bag, ref num);
				int num3 = UxmlUtility.TryParseIntAttribute("y", bag, ref num);
				bool flag = num > 0;
				if (flag)
				{
					base.Value = new Vector2Int(num2, num3);
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
		public new class UxmlFactory : UxmlFactory<Vector2IntField, Vector2IntField.UxmlTraits>
		{
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlTraits : BaseField<Vector2Int>.UxmlTraits
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				Vector2IntField vector2IntField = (Vector2IntField)ve;
				vector2IntField.SetValueWithoutNotify(new Vector2Int(this.m_XValue.GetValueFromBag(bag, cc), this.m_YValue.GetValueFromBag(bag, cc)));
			}

			private UxmlIntAttributeDescription m_XValue = new UxmlIntAttributeDescription
			{
				name = "x"
			};

			private UxmlIntAttributeDescription m_YValue = new UxmlIntAttributeDescription
			{
				name = "y"
			};
		}
	}
}
