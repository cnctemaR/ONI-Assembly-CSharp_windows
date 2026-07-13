using System;
using System.Diagnostics;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal class ScaleField : BaseField<Scale>
	{
		public Vector3Field vectorField
		{
			get
			{
				return this.m_VectorField;
			}
		}

		public ScaleField()
			: this(null)
		{
		}

		public ScaleField(string label)
			: base(label, null)
		{
			this.m_VectorField = new Vector3Field();
			base.visualInput.Add(this.m_VectorField);
			this.m_VectorField.RegisterValueChangedCallback<Vector3>(delegate(ChangeEvent<Vector3> e)
			{
				bool flag = e.newValue != this.value.value;
				if (flag)
				{
					Scale value = this.value;
					value.value = e.newValue;
					this.value = value;
				}
			});
			this.SetValueWithoutNotify(Scale.Initial());
		}

		public override void SetValueWithoutNotify(Scale scale)
		{
			base.SetValueWithoutNotify(scale);
			this.m_VectorField.SetValueWithoutNotify(this.value.value);
		}

		private Vector3Field m_VectorField;

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : BaseField<Scale>.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				BaseField<Scale>.UxmlSerializedData.Register();
			}

			public override object CreateInstance()
			{
				return new ScaleField();
			}
		}
	}
}
