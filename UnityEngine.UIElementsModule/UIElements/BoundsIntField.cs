using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.UIElements
{
	[MovedFrom(true, "UnityEditor.UIElements", "UnityEditor.UIElementsModule", null)]
	public class BoundsIntField : BaseField<BoundsInt>
	{
		public BoundsIntField()
			: this(null)
		{
		}

		public BoundsIntField(string label)
			: base(label, null)
		{
			base.delegatesFocus = false;
			base.visualInput.focusable = false;
			base.AddToClassList(BoundsIntField.ussClassName);
			base.visualInput.AddToClassList(BoundsIntField.inputUssClassName);
			base.labelElement.AddToClassList(BoundsIntField.labelUssClassName);
			this.m_PositionField = new Vector3IntField("Position");
			this.m_PositionField.name = "unity-m_Position-input";
			this.m_PositionField.delegatesFocus = true;
			this.m_PositionField.AddToClassList(BoundsIntField.positionUssClassName);
			this.m_PositionField.RegisterValueChangedCallback<Vector3Int>(delegate(ChangeEvent<Vector3Int> e)
			{
				BoundsInt value = this.value;
				value.position = e.newValue;
				this.value = value;
			});
			base.visualInput.hierarchy.Add(this.m_PositionField);
			this.m_SizeField = new Vector3IntField("Size");
			this.m_SizeField.name = "unity-m_Size-input";
			this.m_SizeField.delegatesFocus = true;
			this.m_SizeField.AddToClassList(BoundsIntField.sizeUssClassName);
			this.m_SizeField.RegisterValueChangedCallback<Vector3Int>(delegate(ChangeEvent<Vector3Int> e)
			{
				BoundsInt value2 = this.value;
				value2.size = e.newValue;
				this.value = value2;
			});
			base.visualInput.hierarchy.Add(this.m_SizeField);
		}

		public override void SetValueWithoutNotify(BoundsInt newValue)
		{
			base.SetValueWithoutNotify(newValue);
			this.m_PositionField.SetValueWithoutNotify(base.rawValue.position);
			this.m_SizeField.SetValueWithoutNotify(base.rawValue.size);
		}

		protected override void UpdateMixedValueContent()
		{
			this.m_PositionField.showMixedValue = base.showMixedValue;
			this.m_SizeField.showMixedValue = base.showMixedValue;
		}

		private Vector3IntField m_PositionField;

		private Vector3IntField m_SizeField;

		public new static readonly string ussClassName = "unity-bounds-int-field";

		public new static readonly string labelUssClassName = BoundsIntField.ussClassName + "__label";

		public new static readonly string inputUssClassName = BoundsIntField.ussClassName + "__input";

		public static readonly string positionUssClassName = BoundsIntField.ussClassName + "__position-field";

		public static readonly string sizeUssClassName = BoundsIntField.ussClassName + "__size-field";

		public new class UxmlFactory : UxmlFactory<BoundsIntField, BoundsIntField.UxmlTraits>
		{
		}

		public new class UxmlTraits : BaseField<BoundsInt>.UxmlTraits
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				BoundsIntField boundsIntField = (BoundsIntField)ve;
				boundsIntField.SetValueWithoutNotify(new BoundsInt(new Vector3Int(this.m_PositionXValue.GetValueFromBag(bag, cc), this.m_PositionYValue.GetValueFromBag(bag, cc), this.m_PositionZValue.GetValueFromBag(bag, cc)), new Vector3Int(this.m_SizeXValue.GetValueFromBag(bag, cc), this.m_SizeYValue.GetValueFromBag(bag, cc), this.m_SizeZValue.GetValueFromBag(bag, cc))));
			}

			private UxmlIntAttributeDescription m_PositionXValue = new UxmlIntAttributeDescription
			{
				name = "px"
			};

			private UxmlIntAttributeDescription m_PositionYValue = new UxmlIntAttributeDescription
			{
				name = "py"
			};

			private UxmlIntAttributeDescription m_PositionZValue = new UxmlIntAttributeDescription
			{
				name = "pz"
			};

			private UxmlIntAttributeDescription m_SizeXValue = new UxmlIntAttributeDescription
			{
				name = "sx"
			};

			private UxmlIntAttributeDescription m_SizeYValue = new UxmlIntAttributeDescription
			{
				name = "sy"
			};

			private UxmlIntAttributeDescription m_SizeZValue = new UxmlIntAttributeDescription
			{
				name = "sz"
			};
		}
	}
}
