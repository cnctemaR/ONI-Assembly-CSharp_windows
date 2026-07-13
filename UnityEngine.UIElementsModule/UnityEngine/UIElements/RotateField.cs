using System;
using System.Diagnostics;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal class RotateField : BaseField<Rotate>, IValueField<Rotate>
	{
		public AngleField angleField
		{
			get
			{
				return this.m_AngleField;
			}
		}

		public Vector3Field axisField
		{
			get
			{
				return this.m_AxisField;
			}
		}

		public RotateField()
			: this(null)
		{
		}

		public RotateField(string label)
			: base(label, null)
		{
			this.m_AngleField = new AngleField();
			this.m_AxisField = new Vector3Field();
			base.visualInput.Add(this.m_AngleField);
			base.visualInput.Add(this.m_AxisField);
			this.m_AngleField.AddToClassList(RotateField.styleFieldUssClassName);
			this.m_AngleField.RegisterValueChangedCallback<Angle>(delegate(ChangeEvent<Angle> e)
			{
				bool flag = e.newValue != this.value.angle;
				if (flag)
				{
					Rotate value = this.value;
					value.angle = e.newValue;
					this.value = value;
				}
			});
			this.m_AxisField.RegisterValueChangedCallback<Vector3>(delegate(ChangeEvent<Vector3> e)
			{
				bool flag2 = e.newValue != this.value.axis;
				if (flag2)
				{
					Rotate value2 = this.value;
					value2.axis = e.newValue;
					this.value = value2;
				}
			});
			this.AddLabelDragger();
			this.SetValueWithoutNotify(Rotate.Initial());
		}

		public override void SetValueWithoutNotify(Rotate rotate)
		{
			base.SetValueWithoutNotify(rotate);
			this.m_AngleField.SetValueWithoutNotify(this.value.angle);
			this.m_AxisField.SetValueWithoutNotify(this.value.axis);
		}

		protected void AddLabelDragger()
		{
			this.m_Dragger = new FieldMouseDragger<Rotate>(this);
			this.EnableLabelDragger(!this.m_AngleField.isReadOnly);
		}

		private void EnableLabelDragger(bool enable)
		{
			bool flag = this.m_Dragger != null;
			if (flag)
			{
				this.m_Dragger.SetDragZone(enable ? base.labelElement : null);
				base.labelElement.EnableInClassList(BaseField<Rotate>.labelDraggerVariantUssClassName, enable);
			}
		}

		public void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, Rotate startValue)
		{
			this.m_AngleField.ApplyInputDeviceDelta(delta, speed, startValue.angle);
		}

		public void StartDragging()
		{
			this.m_AngleField.StartDragging();
		}

		public void StopDragging()
		{
			this.m_AngleField.StopDragging();
		}

		public static readonly string styleFieldUssClassName = "unity-style-field";

		private AngleField m_AngleField;

		private Vector3Field m_AxisField;

		private BaseFieldMouseDragger m_Dragger;

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : BaseField<Rotate>.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				BaseField<Rotate>.UxmlSerializedData.Register();
			}

			public override object CreateInstance()
			{
				return new RotateField();
			}
		}
	}
}
