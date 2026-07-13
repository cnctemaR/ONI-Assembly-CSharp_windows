using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
	public abstract class AbstractProgressBar : BindableElement, INotifyValueChanged<float>
	{
		[CreateProperty]
		public string title
		{
			get
			{
				return this.m_Title.text;
			}
			set
			{
				string title = this.title;
				this.m_Title.text = value;
				bool flag = string.CompareOrdinal(title, this.title) != 0;
				if (flag)
				{
					base.NotifyPropertyChanged(in AbstractProgressBar.titleProperty);
				}
			}
		}

		[CreateProperty]
		public float lowValue
		{
			get
			{
				return this.m_LowValue;
			}
			set
			{
				float lowValue = this.lowValue;
				this.m_LowValue = value;
				this.SetProgress(this.m_Value);
				bool flag = !Mathf.Approximately(lowValue, this.lowValue);
				if (flag)
				{
					base.NotifyPropertyChanged(in AbstractProgressBar.lowValueProperty);
				}
			}
		}

		[CreateProperty]
		public float highValue
		{
			get
			{
				return this.m_HighValue;
			}
			set
			{
				float highValue = this.highValue;
				this.m_HighValue = value;
				this.SetProgress(this.m_Value);
				bool flag = !Mathf.Approximately(highValue, this.highValue);
				if (flag)
				{
					base.NotifyPropertyChanged(in AbstractProgressBar.highValueProperty);
				}
			}
		}

		public AbstractProgressBar()
		{
			base.AddToClassList(AbstractProgressBar.ussClassName);
			VisualElement visualElement = new VisualElement
			{
				name = AbstractProgressBar.ussClassName
			};
			this.m_Background = new VisualElement();
			this.m_Background.AddToClassList(AbstractProgressBar.backgroundUssClassName);
			visualElement.Add(this.m_Background);
			this.m_Progress = new VisualElement();
			this.m_Progress.AddToClassList(AbstractProgressBar.progressUssClassName);
			this.m_Background.Add(this.m_Progress);
			VisualElement visualElement2 = new VisualElement();
			visualElement2.AddToClassList(AbstractProgressBar.titleContainerUssClassName);
			this.m_Background.Add(visualElement2);
			this.m_Title = new Label();
			this.m_Title.AddToClassList(AbstractProgressBar.titleUssClassName);
			visualElement2.Add(this.m_Title);
			visualElement.AddToClassList(AbstractProgressBar.containerUssClassName);
			base.hierarchy.Add(visualElement);
			base.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnGeometryChanged), TrickleDown.NoTrickleDown);
		}

		private void OnGeometryChanged(GeometryChangedEvent e)
		{
			this.SetProgress(this.value);
		}

		[CreateProperty]
		public virtual float value
		{
			get
			{
				return this.m_Value;
			}
			set
			{
				bool flag = !EqualityComparer<float>.Default.Equals(this.m_Value, value);
				if (flag)
				{
					bool flag2 = base.panel != null;
					if (flag2)
					{
						using (ChangeEvent<float> pooled = ChangeEvent<float>.GetPooled(this.m_Value, value))
						{
							pooled.elementTarget = this;
							this.SetValueWithoutNotify(value);
							this.SendEvent(pooled);
							base.NotifyPropertyChanged(in AbstractProgressBar.valueProperty);
						}
					}
					else
					{
						this.SetValueWithoutNotify(value);
					}
				}
			}
		}

		public void SetValueWithoutNotify(float newValue)
		{
			this.m_Value = newValue;
			this.SetProgress(this.value);
		}

		private void SetProgress(float p)
		{
			bool flag = p < this.lowValue;
			float num;
			if (flag)
			{
				num = this.lowValue;
			}
			else
			{
				bool flag2 = p > this.highValue;
				if (flag2)
				{
					num = this.highValue;
				}
				else
				{
					num = p;
				}
			}
			num = this.CalculateOppositeProgressWidth(num);
			bool flag3 = num >= 0f;
			if (flag3)
			{
				this.m_Progress.style.right = num;
			}
		}

		private float CalculateOppositeProgressWidth(float width)
		{
			bool flag = this.m_Background == null || this.m_Progress == null;
			float num;
			if (flag)
			{
				num = 0f;
			}
			else
			{
				bool flag2 = float.IsNaN(this.m_Background.layout.width);
				if (flag2)
				{
					num = 0f;
				}
				else
				{
					float num2 = Mathf.Floor(this.m_Background.layout.width - 2f);
					float num3 = Mathf.Max(num2 * width / this.highValue, 0f);
					float num4 = num2 - num3;
					this.m_Progress.style.width = ((Mathf.Abs(num2 - num4) < 0.1f) ? new StyleLength(0f) : new StyleLength(StyleKeyword.Auto));
					num = num4;
				}
			}
			return num;
		}

		internal static readonly BindingId titleProperty = "title";

		internal static readonly BindingId lowValueProperty = "lowValue";

		internal static readonly BindingId highValueProperty = "highValue";

		internal static readonly BindingId valueProperty = "value";

		public static readonly string ussClassName = "unity-progress-bar";

		public static readonly string containerUssClassName = AbstractProgressBar.ussClassName + "__container";

		public static readonly string titleUssClassName = AbstractProgressBar.ussClassName + "__title";

		public static readonly string titleContainerUssClassName = AbstractProgressBar.ussClassName + "__title-container";

		public static readonly string progressUssClassName = AbstractProgressBar.ussClassName + "__progress";

		public static readonly string backgroundUssClassName = AbstractProgressBar.ussClassName + "__background";

		private readonly VisualElement m_Background;

		private readonly VisualElement m_Progress;

		private readonly Label m_Title;

		private float m_LowValue;

		private float m_HighValue = 100f;

		private float m_Value;

		private const float k_MinVisibleProgress = 0f;

		private const float k_AcceptedWidthEpsilon = 0.1f;

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : BindableElement.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				UxmlDescriptionCache.RegisterType(typeof(AbstractProgressBar.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("lowValue", "low-value", null, Array.Empty<string>()),
					new UxmlAttributeNames("highValue", "high-value", null, Array.Empty<string>()),
					new UxmlAttributeNames("value", "value", null, Array.Empty<string>()),
					new UxmlAttributeNames("title", "title", null, Array.Empty<string>())
				}, false);
			}

			public override void Deserialize(object obj)
			{
				base.Deserialize(obj);
				AbstractProgressBar abstractProgressBar = (AbstractProgressBar)obj;
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.lowValue_UxmlAttributeFlags);
				if (flag)
				{
					abstractProgressBar.lowValue = this.lowValue;
				}
				bool flag2 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.highValue_UxmlAttributeFlags);
				if (flag2)
				{
					abstractProgressBar.highValue = this.highValue;
				}
				bool flag3 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.value_UxmlAttributeFlags);
				if (flag3)
				{
					abstractProgressBar.value = this.value;
				}
				bool flag4 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.title_UxmlAttributeFlags);
				if (flag4)
				{
					abstractProgressBar.title = this.title;
				}
			}

			[SerializeField]
			private float lowValue;

			[HideInInspector]
			[UxmlIgnore]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags lowValue_UxmlAttributeFlags;

			[SerializeField]
			private float highValue;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags highValue_UxmlAttributeFlags;

			[SerializeField]
			private float value;

			[HideInInspector]
			[UxmlIgnore]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags value_UxmlAttributeFlags;

			[SerializeField]
			private string title;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags title_UxmlAttributeFlags;
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlTraits : BindableElement.UxmlTraits
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				AbstractProgressBar abstractProgressBar = ve as AbstractProgressBar;
				abstractProgressBar.lowValue = this.m_LowValue.GetValueFromBag(bag, cc);
				abstractProgressBar.highValue = this.m_HighValue.GetValueFromBag(bag, cc);
				abstractProgressBar.value = this.m_Value.GetValueFromBag(bag, cc);
				abstractProgressBar.title = this.m_Title.GetValueFromBag(bag, cc);
			}

			private UxmlFloatAttributeDescription m_LowValue = new UxmlFloatAttributeDescription
			{
				name = "low-value",
				defaultValue = 0f
			};

			private UxmlFloatAttributeDescription m_HighValue = new UxmlFloatAttributeDescription
			{
				name = "high-value",
				defaultValue = 100f
			};

			private UxmlFloatAttributeDescription m_Value = new UxmlFloatAttributeDescription
			{
				name = "value",
				defaultValue = 0f
			};

			private UxmlStringAttributeDescription m_Title = new UxmlStringAttributeDescription
			{
				name = "title",
				defaultValue = string.Empty
			};
		}
	}
}
