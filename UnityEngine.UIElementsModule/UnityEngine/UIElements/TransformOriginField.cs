using System;
using System.Diagnostics;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal class TransformOriginField : BaseField<TransformOrigin>
	{
		public LengthField xField
		{
			get
			{
				return this.m_XField;
			}
		}

		public LengthField yField
		{
			get
			{
				return this.m_YField;
			}
		}

		public FloatField zField
		{
			get
			{
				return this.m_ZField;
			}
		}

		public TransformOriginField()
			: this(null)
		{
		}

		public TransformOriginField(string label)
			: base(label, null)
		{
			base.AddToClassList(TransformOriginField.ussClassName);
			base.visualInput.AddToClassList(TransformOriginField.inputUssClassName);
			this.m_XField = new LengthField("X", 1000)
			{
				classList = { TransformOriginField.fieldUssClassName }
			};
			this.m_YField = new LengthField("Y", 1000)
			{
				classList = { TransformOriginField.fieldUssClassName }
			};
			this.m_ZField = new FloatField("Z", 1000)
			{
				classList = { TransformOriginField.fieldUssClassName }
			};
			base.visualInput.Add(this.m_XField);
			base.visualInput.Add(this.m_YField);
			base.visualInput.Add(this.m_ZField);
			this.m_XField.RegisterValueChangedCallback<Length>(delegate(ChangeEvent<Length> e)
			{
				bool flag = e.newValue != this.value.x;
				if (flag)
				{
					TransformOrigin value = this.value;
					value.x = e.newValue;
					this.value = value;
				}
			});
			this.m_YField.RegisterValueChangedCallback<Length>(delegate(ChangeEvent<Length> e)
			{
				bool flag2 = e.newValue != this.value.y;
				if (flag2)
				{
					TransformOrigin value2 = this.value;
					value2.y = e.newValue;
					this.value = value2;
				}
			});
			this.m_ZField.RegisterValueChangedCallback<float>(delegate(ChangeEvent<float> e)
			{
				bool flag3 = e.newValue != this.value.z;
				if (flag3)
				{
					TransformOrigin value3 = this.value;
					value3.z = e.newValue;
					this.value = value3;
				}
			});
		}

		public override void SetValueWithoutNotify(TransformOrigin transformOrigin)
		{
			base.SetValueWithoutNotify(transformOrigin);
			this.m_XField.SetValueWithoutNotify(this.value.x);
			this.m_YField.SetValueWithoutNotify(this.value.y);
			this.m_ZField.SetValueWithoutNotify(this.value.z);
		}

		public new static readonly string ussClassName = "unity-transform-origin-field";

		public new static readonly string inputUssClassName = TransformOriginField.ussClassName + "__input";

		private static readonly string compositeUssClassName = "unity-composite-field";

		private static readonly string fieldUssClassName = TransformOriginField.compositeUssClassName + "__field";

		private LengthField m_XField;

		private LengthField m_YField;

		private FloatField m_ZField;

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : BaseField<TransformOrigin>.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				BaseField<TransformOrigin>.UxmlSerializedData.Register();
			}

			public override object CreateInstance()
			{
				return new TransformOriginField();
			}
		}
	}
}
