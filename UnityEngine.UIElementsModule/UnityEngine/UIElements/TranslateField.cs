using System;
using System.Diagnostics;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal class TranslateField : BaseField<Translate>
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

		public TranslateField()
			: this(null)
		{
		}

		public TranslateField(string label)
			: base(label, null)
		{
			base.AddToClassList(TranslateField.ussClassName);
			this.m_XField = new LengthField("X", 1000)
			{
				classList = { TranslateField.compositeFieldUssClassName }
			};
			this.m_YField = new LengthField("Y", 1000)
			{
				classList = { TranslateField.compositeFieldUssClassName }
			};
			this.m_ZField = new FloatField("Z", 1000)
			{
				classList = { TranslateField.compositeFieldUssClassName }
			};
			base.visualInput.Add(this.m_XField);
			base.visualInput.Add(this.m_YField);
			base.visualInput.Add(this.m_ZField);
			this.m_XField.RegisterValueChangedCallback<Length>(delegate(ChangeEvent<Length> e)
			{
				bool flag = e.newValue != this.value.x;
				if (flag)
				{
					Translate value = this.value;
					value.x = e.newValue;
					this.value = value;
				}
			});
			this.m_YField.RegisterValueChangedCallback<Length>(delegate(ChangeEvent<Length> e)
			{
				bool flag2 = e.newValue != this.value.y;
				if (flag2)
				{
					Translate value2 = this.value;
					value2.y = e.newValue;
					this.value = value2;
				}
			});
			this.m_ZField.RegisterValueChangedCallback<float>(delegate(ChangeEvent<float> e)
			{
				bool flag3 = e.newValue != this.value.z;
				if (flag3)
				{
					Translate value3 = this.value;
					value3.z = e.newValue;
					this.value = value3;
				}
			});
		}

		public override void SetValueWithoutNotify(Translate t)
		{
			base.SetValueWithoutNotify(t);
			this.m_XField.SetValueWithoutNotify(this.value.x);
			this.m_YField.SetValueWithoutNotify(this.value.y);
			this.m_ZField.SetValueWithoutNotify(this.value.z);
		}

		public override string ToString()
		{
			return string.Format("(x:{0}, y:{1}, z:{2})", this.m_XField.value, this.m_YField.value, this.m_ZField.value);
		}

		public new static readonly string ussClassName = "unity-translate-field";

		private static readonly string compositeUssClassName = "unity-composite-field";

		private static readonly string compositeFieldUssClassName = TranslateField.compositeUssClassName + "__field";

		private LengthField m_XField;

		private LengthField m_YField;

		private FloatField m_ZField;

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : BaseField<Translate>.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				BaseField<Translate>.UxmlSerializedData.Register();
			}

			public override object CreateInstance()
			{
				return new TranslateField();
			}
		}
	}
}
