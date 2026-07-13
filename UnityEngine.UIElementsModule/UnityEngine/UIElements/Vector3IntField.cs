using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Internal;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.UIElements
{
	[MovedFrom(true, "UnityEditor.UIElements", "UnityEditor.UIElementsModule", null)]
	public class Vector3IntField : BaseCompositeField<Vector3Int, IntegerField, int>
	{
		internal override BaseCompositeField<Vector3Int, IntegerField, int>.FieldDescription[] DescribeFields()
		{
			BaseCompositeField<Vector3Int, IntegerField, int>.FieldDescription[] array = new BaseCompositeField<Vector3Int, IntegerField, int>.FieldDescription[3];
			array[0] = new BaseCompositeField<Vector3Int, IntegerField, int>.FieldDescription("X", "unity-x-input", (Vector3Int r) => r.x, delegate(ref Vector3Int r, int v)
			{
				r.x = v;
			});
			array[1] = new BaseCompositeField<Vector3Int, IntegerField, int>.FieldDescription("Y", "unity-y-input", (Vector3Int r) => r.y, delegate(ref Vector3Int r, int v)
			{
				r.y = v;
			});
			array[2] = new BaseCompositeField<Vector3Int, IntegerField, int>.FieldDescription("Z", "unity-z-input", (Vector3Int r) => r.z, delegate(ref Vector3Int r, int v)
			{
				r.z = v;
			});
			return array;
		}

		public Vector3IntField()
			: this(null)
		{
		}

		public Vector3IntField(string label)
			: base(label, 3)
		{
			base.AddToClassList(Vector3IntField.ussClassName);
			base.labelElement.AddToClassList(Vector3IntField.labelUssClassName);
			base.visualInput.AddToClassList(Vector3IntField.inputUssClassName);
		}

		public new static readonly string ussClassName = "unity-vector3-int-field";

		public new static readonly string labelUssClassName = Vector3IntField.ussClassName + "__label";

		public new static readonly string inputUssClassName = Vector3IntField.ussClassName + "__input";

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : BaseCompositeField<Vector3Int, IntegerField, int>.UxmlSerializedData, IUxmlSerializedDataCustomAttributeHandler
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
			}

			public override object CreateInstance()
			{
				return new Vector3IntField();
			}

			void IUxmlSerializedDataCustomAttributeHandler.SerializeCustomAttributes(IUxmlAttributes bag, HashSet<string> handledAttributes)
			{
				int num = 0;
				int num2 = UxmlUtility.TryParseIntAttribute("x", bag, ref num);
				int num3 = UxmlUtility.TryParseIntAttribute("y", bag, ref num);
				int num4 = UxmlUtility.TryParseIntAttribute("z", bag, ref num);
				bool flag = num > 0;
				if (flag)
				{
					base.Value = new Vector3Int(num2, num3, num4);
					handledAttributes.Add("value");
					UxmlAsset uxmlAsset = bag as UxmlAsset;
					bool flag2 = uxmlAsset != null;
					if (flag2)
					{
						uxmlAsset.RemoveAttribute("x");
						uxmlAsset.RemoveAttribute("y");
						uxmlAsset.RemoveAttribute("z");
						uxmlAsset.SetAttribute("value", UxmlUtility.ValueToString(base.Value));
					}
				}
			}
		}

		[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlFactory : UxmlFactory<Vector3IntField, Vector3IntField.UxmlTraits>
		{
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlTraits : BaseField<Vector3Int>.UxmlTraits
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				Vector3IntField vector3IntField = (Vector3IntField)ve;
				vector3IntField.SetValueWithoutNotify(new Vector3Int(this.m_XValue.GetValueFromBag(bag, cc), this.m_YValue.GetValueFromBag(bag, cc), this.m_ZValue.GetValueFromBag(bag, cc)));
			}

			private UxmlIntAttributeDescription m_XValue = new UxmlIntAttributeDescription
			{
				name = "x"
			};

			private UxmlIntAttributeDescription m_YValue = new UxmlIntAttributeDescription
			{
				name = "y"
			};

			private UxmlIntAttributeDescription m_ZValue = new UxmlIntAttributeDescription
			{
				name = "z"
			};
		}
	}
}
