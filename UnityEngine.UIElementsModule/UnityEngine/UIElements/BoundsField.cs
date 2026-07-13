using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Internal;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.UIElements
{
	[MovedFrom(true, "UnityEditor.UIElements", "UnityEditor.UIElementsModule", null)]
	public class BoundsField : BaseField<Bounds>
	{
		public BoundsField()
			: this(null)
		{
		}

		public BoundsField(string label)
			: base(label, null)
		{
			base.delegatesFocus = false;
			base.visualInput.focusable = false;
			base.AddToClassList(BoundsField.ussClassName);
			base.visualInput.AddToClassList(BoundsField.inputUssClassName);
			base.labelElement.AddToClassList(BoundsField.labelUssClassName);
			this.m_CenterField = new Vector3Field("Center");
			this.m_CenterField.name = "unity-m_Center-input";
			this.m_CenterField.delegatesFocus = true;
			this.m_CenterField.AddToClassList(BoundsField.centerFieldUssClassName);
			this.m_CenterField.RegisterValueChangedCallback<Vector3>(delegate(ChangeEvent<Vector3> e)
			{
				Bounds value = this.value;
				value.center = e.newValue;
				this.value = value;
			});
			base.visualInput.hierarchy.Add(this.m_CenterField);
			this.m_ExtentsField = new Vector3Field("Extents");
			this.m_ExtentsField.name = "unity-m_Extent-input";
			this.m_ExtentsField.delegatesFocus = true;
			this.m_ExtentsField.AddToClassList(BoundsField.extentsFieldUssClassName);
			this.m_ExtentsField.RegisterValueChangedCallback<Vector3>(delegate(ChangeEvent<Vector3> e)
			{
				Bounds value2 = this.value;
				value2.extents = e.newValue;
				this.value = value2;
			});
			base.visualInput.hierarchy.Add(this.m_ExtentsField);
		}

		public override void SetValueWithoutNotify(Bounds newValue)
		{
			base.SetValueWithoutNotify(newValue);
			this.m_CenterField.SetValueWithoutNotify(base.rawValue.center);
			this.m_ExtentsField.SetValueWithoutNotify(base.rawValue.extents);
		}

		protected override void UpdateMixedValueContent()
		{
			this.m_CenterField.showMixedValue = base.showMixedValue;
			this.m_ExtentsField.showMixedValue = base.showMixedValue;
		}

		public new static readonly string ussClassName = "unity-bounds-field";

		public new static readonly string labelUssClassName = BoundsField.ussClassName + "__label";

		public new static readonly string inputUssClassName = BoundsField.ussClassName + "__input";

		public static readonly string centerFieldUssClassName = BoundsField.ussClassName + "__center-field";

		public static readonly string extentsFieldUssClassName = BoundsField.ussClassName + "__extents-field";

		private Vector3Field m_CenterField;

		private Vector3Field m_ExtentsField;

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : BaseField<Bounds>.UxmlSerializedData, IUxmlSerializedDataCustomAttributeHandler
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				BaseField<Bounds>.UxmlSerializedData.Register();
			}

			public override object CreateInstance()
			{
				return new BoundsField();
			}

			void IUxmlSerializedDataCustomAttributeHandler.SerializeCustomAttributes(IUxmlAttributes bag, HashSet<string> handledAttributes)
			{
				int num = 0;
				float num2 = UxmlUtility.TryParseFloatAttribute("cx", bag, ref num);
				float num3 = UxmlUtility.TryParseFloatAttribute("cy", bag, ref num);
				float num4 = UxmlUtility.TryParseFloatAttribute("cz", bag, ref num);
				float num5 = UxmlUtility.TryParseFloatAttribute("ex", bag, ref num);
				float num6 = UxmlUtility.TryParseFloatAttribute("ey", bag, ref num);
				float num7 = UxmlUtility.TryParseFloatAttribute("ez", bag, ref num);
				bool flag = num > 0;
				if (flag)
				{
					base.Value = new Bounds(new Vector3(num2, num3, num4), new Vector3(num5, num6, num7));
					handledAttributes.Add("value");
					UxmlAsset uxmlAsset = bag as UxmlAsset;
					bool flag2 = uxmlAsset != null;
					if (flag2)
					{
						uxmlAsset.RemoveAttribute("cx");
						uxmlAsset.RemoveAttribute("cy");
						uxmlAsset.RemoveAttribute("cz");
						uxmlAsset.RemoveAttribute("ex");
						uxmlAsset.RemoveAttribute("ey");
						uxmlAsset.RemoveAttribute("ez");
						uxmlAsset.SetAttribute("value", UxmlUtility.ValueToString(base.Value));
					}
				}
			}
		}

		[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlFactory : UxmlFactory<BoundsField, BoundsField.UxmlTraits>
		{
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlTraits : BaseField<Bounds>.UxmlTraits
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				BoundsField boundsField = (BoundsField)ve;
				boundsField.SetValueWithoutNotify(new Bounds(new Vector3(this.m_CenterXValue.GetValueFromBag(bag, cc), this.m_CenterYValue.GetValueFromBag(bag, cc), this.m_CenterZValue.GetValueFromBag(bag, cc)), new Vector3(this.m_ExtentsXValue.GetValueFromBag(bag, cc), this.m_ExtentsYValue.GetValueFromBag(bag, cc), this.m_ExtentsZValue.GetValueFromBag(bag, cc))));
			}

			private UxmlFloatAttributeDescription m_CenterXValue = new UxmlFloatAttributeDescription
			{
				name = "cx"
			};

			private UxmlFloatAttributeDescription m_CenterYValue = new UxmlFloatAttributeDescription
			{
				name = "cy"
			};

			private UxmlFloatAttributeDescription m_CenterZValue = new UxmlFloatAttributeDescription
			{
				name = "cz"
			};

			private UxmlFloatAttributeDescription m_ExtentsXValue = new UxmlFloatAttributeDescription
			{
				name = "ex"
			};

			private UxmlFloatAttributeDescription m_ExtentsYValue = new UxmlFloatAttributeDescription
			{
				name = "ey"
			};

			private UxmlFloatAttributeDescription m_ExtentsZValue = new UxmlFloatAttributeDescription
			{
				name = "ez"
			};
		}
	}
}
