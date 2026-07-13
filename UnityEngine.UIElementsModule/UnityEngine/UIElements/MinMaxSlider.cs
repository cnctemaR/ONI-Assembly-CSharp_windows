using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
	public class MinMaxSlider : BaseField<Vector2>
	{
		internal VisualElement dragElement { get; private set; }

		internal VisualElement dragMinThumb { get; private set; }

		internal VisualElement dragMaxThumb { get; private set; }

		internal ClampedDragger<float> clampedDragger { get; private set; }

		internal Vector2 valueOverride
		{
			get
			{
				return this.value;
			}
			set
			{
				this.SetValueWithoutNotify(value);
			}
		}

		[CreateProperty]
		public float minValue
		{
			get
			{
				return this.value.x;
			}
			set
			{
				float minValue = this.minValue;
				base.value = this.ClampValues(new Vector2(value, base.rawValue.y));
				bool flag = !Mathf.Approximately(minValue, this.minValue);
				if (flag)
				{
					base.NotifyPropertyChanged(in MinMaxSlider.minValueProperty);
				}
			}
		}

		[CreateProperty]
		public float maxValue
		{
			get
			{
				return this.value.y;
			}
			set
			{
				float maxValue = this.maxValue;
				base.value = this.ClampValues(new Vector2(base.rawValue.x, value));
				bool flag = !Mathf.Approximately(maxValue, this.maxValue);
				if (flag)
				{
					base.NotifyPropertyChanged(in MinMaxSlider.maxValueProperty);
				}
			}
		}

		public override Vector2 value
		{
			get
			{
				return base.value;
			}
			set
			{
				base.value = this.ClampValues(value);
			}
		}

		public override void SetValueWithoutNotify(Vector2 newValue)
		{
			base.SetValueWithoutNotify(this.ClampValues(newValue));
			this.UpdateDragElementPosition();
		}

		[CreateProperty(ReadOnly = true)]
		public float range
		{
			get
			{
				return Math.Abs(this.highLimit - this.lowLimit);
			}
		}

		[CreateProperty]
		public float lowLimit
		{
			get
			{
				return this.m_MinLimit;
			}
			set
			{
				bool flag = !Mathf.Approximately(this.m_MinLimit, value);
				if (flag)
				{
					bool flag2 = value > this.m_MaxLimit;
					if (flag2)
					{
						throw new ArgumentException("lowLimit is greater than highLimit");
					}
					this.m_MinLimit = value;
					this.value = base.rawValue;
					this.UpdateDragElementPosition();
					bool flag3 = !string.IsNullOrEmpty(base.viewDataKey);
					if (flag3)
					{
						base.SaveViewData();
					}
					base.NotifyPropertyChanged(in MinMaxSlider.lowLimitProperty);
				}
			}
		}

		[CreateProperty]
		public float highLimit
		{
			get
			{
				return this.m_MaxLimit;
			}
			set
			{
				bool flag = !Mathf.Approximately(this.m_MaxLimit, value);
				if (flag)
				{
					bool flag2 = value < this.m_MinLimit;
					if (flag2)
					{
						throw new ArgumentException("highLimit is smaller than lowLimit");
					}
					this.m_MaxLimit = value;
					this.value = base.rawValue;
					this.UpdateDragElementPosition();
					bool flag3 = !string.IsNullOrEmpty(base.viewDataKey);
					if (flag3)
					{
						base.SaveViewData();
					}
					base.NotifyPropertyChanged(in MinMaxSlider.highLimitProperty);
				}
			}
		}

		public MinMaxSlider()
			: this(null, 0f, 10f, float.MinValue, float.MaxValue)
		{
		}

		public MinMaxSlider(float minValue, float maxValue, float minLimit, float maxLimit)
			: this(null, minValue, maxValue, minLimit, maxLimit)
		{
		}

		public MinMaxSlider(string label, float minValue = 0f, float maxValue = 10f, float minLimit = -3.4028235E+38f, float maxLimit = 3.4028235E+38f)
			: base(label, null)
		{
			this.m_MinLimit = float.MinValue;
			this.m_MaxLimit = float.MaxValue;
			this.lowLimit = minLimit;
			this.highLimit = maxLimit;
			Vector2 vector = this.ClampValues(new Vector2(minValue, maxValue));
			this.minValue = vector.x;
			this.maxValue = vector.y;
			base.AddToClassList(MinMaxSlider.ussClassName);
			base.labelElement.AddToClassList(MinMaxSlider.labelUssClassName);
			base.visualInput.AddToClassList(MinMaxSlider.inputUssClassName);
			base.pickingMode = PickingMode.Ignore;
			this.m_DragState = MinMaxSlider.DragState.NoThumb;
			base.visualInput.pickingMode = PickingMode.Position;
			VisualElement visualElement = new VisualElement
			{
				name = "unity-tracker"
			};
			visualElement.AddToClassList(MinMaxSlider.trackerUssClassName);
			base.visualInput.Add(visualElement);
			this.dragElement = new VisualElement
			{
				name = "unity-dragger"
			};
			this.dragElement.AddToClassList(MinMaxSlider.draggerUssClassName);
			this.dragElement.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.UpdateDragElementPosition), TrickleDown.NoTrickleDown);
			base.visualInput.Add(this.dragElement);
			this.dragMinThumb = new VisualElement
			{
				name = "unity-thumb-min"
			};
			this.dragMaxThumb = new VisualElement
			{
				name = "unity-thumb-max"
			};
			this.dragMinThumb.AddToClassList(MinMaxSlider.minThumbUssClassName);
			this.dragMaxThumb.AddToClassList(MinMaxSlider.maxThumbUssClassName);
			this.dragElement.Add(this.dragMinThumb);
			this.dragElement.Add(this.dragMaxThumb);
			this.clampedDragger = new ClampedDragger<float>(null, new Action(this.SetSliderValueFromClick), new Action(this.SetSliderValueFromDrag));
			base.visualInput.AddManipulator(this.clampedDragger);
			this.m_MinLimit = minLimit;
			this.m_MaxLimit = maxLimit;
			base.rawValue = this.ClampValues(new Vector2(minValue, maxValue));
			this.UpdateDragElementPosition();
			base.RegisterCallback<FocusInEvent>(new EventCallback<FocusInEvent>(this.OnFocusIn), TrickleDown.NoTrickleDown);
			base.RegisterCallback<BlurEvent>(new EventCallback<BlurEvent>(this.OnBlur), TrickleDown.NoTrickleDown);
			base.RegisterCallback<NavigationSubmitEvent>(new EventCallback<NavigationSubmitEvent>(this.OnNavigationSubmit), TrickleDown.NoTrickleDown);
			base.RegisterCallback<NavigationMoveEvent>(new EventCallback<NavigationMoveEvent>(this.OnNavigationMove), TrickleDown.NoTrickleDown);
		}

		private Vector2 ClampValues(Vector2 valueToClamp)
		{
			bool flag = this.m_MinLimit > this.m_MaxLimit;
			if (flag)
			{
				this.m_MinLimit = this.m_MaxLimit;
			}
			Vector2 vector = default(Vector2);
			bool flag2 = valueToClamp.y > this.m_MaxLimit;
			if (flag2)
			{
				valueToClamp.y = this.m_MaxLimit;
			}
			vector.x = Mathf.Clamp(valueToClamp.x, this.m_MinLimit, valueToClamp.y);
			vector.y = Mathf.Clamp(valueToClamp.y, valueToClamp.x, this.m_MaxLimit);
			return vector;
		}

		private void UpdateDragElementPosition(GeometryChangedEvent evt)
		{
			bool flag = evt.oldRect.size == evt.newRect.size;
			if (!flag)
			{
				this.UpdateDragElementPosition();
			}
		}

		private void UpdateDragElementPosition()
		{
			bool flag = base.panel == null;
			if (!flag)
			{
				float num = this.dragElement.resolvedStyle.borderLeftWidth + this.dragElement.resolvedStyle.marginLeft;
				float num2 = this.dragElement.resolvedStyle.borderRightWidth + this.dragElement.resolvedStyle.marginRight;
				float num3 = num2 + num;
				float num4 = this.dragMinThumb.resolvedStyle.width + this.dragMaxThumb.resolvedStyle.width + num3;
				float num5 = this.RoundToPanelPixelSize(this.SliderLerpUnclamped(this.dragMinThumb.resolvedStyle.width, base.visualInput.layout.width - this.dragMaxThumb.resolvedStyle.width - num3, this.SliderNormalizeValue(this.minValue, this.lowLimit, this.highLimit)));
				float num6 = this.RoundToPanelPixelSize(this.SliderLerpUnclamped(this.dragMinThumb.resolvedStyle.width + num3, base.visualInput.layout.width - this.dragMaxThumb.resolvedStyle.width, this.SliderNormalizeValue(this.maxValue, this.lowLimit, this.highLimit)));
				this.dragElement.style.width = num6 - num5;
				this.dragElement.style.left = num5;
				this.dragMinThumb.style.left = -this.dragMinThumb.resolvedStyle.width - num;
				this.dragMaxThumb.style.right = -this.dragMaxThumb.resolvedStyle.width - num2;
			}
		}

		internal float SliderLerpUnclamped(float a, float b, float interpolant)
		{
			return Mathf.LerpUnclamped(a, b, interpolant);
		}

		internal float SliderNormalizeValue(float currentValue, float lowerValue, float higherValue)
		{
			return (currentValue - lowerValue) / (higherValue - lowerValue);
		}

		private float ComputeValueFromPosition(float positionToConvert)
		{
			float num = this.SliderNormalizeValue(positionToConvert, 0f, base.visualInput.layout.width);
			return this.SliderLerpUnclamped(this.lowLimit, this.highLimit, num);
		}

		[EventInterest(new Type[] { typeof(GeometryChangedEvent) })]
		protected override void HandleEventBubbleUp(EventBase evt)
		{
			base.HandleEventBubbleUp(evt);
			bool flag = evt == null;
			if (!flag)
			{
				bool flag2 = evt.eventTypeId == EventBase<GeometryChangedEvent>.TypeId();
				if (flag2)
				{
					this.UpdateDragElementPosition((GeometryChangedEvent)evt);
				}
			}
		}

		private MinMaxSlider.DragState GetNavigationState()
		{
			bool flag = this.dragMinThumb.ClassListContains(MinMaxSlider.movableUssClassName);
			bool flag2 = this.dragMaxThumb.ClassListContains(MinMaxSlider.movableUssClassName);
			bool flag3 = flag;
			MinMaxSlider.DragState dragState;
			if (flag3)
			{
				dragState = (flag2 ? MinMaxSlider.DragState.MiddleThumb : MinMaxSlider.DragState.MinThumb);
			}
			else
			{
				bool flag4 = flag2;
				if (flag4)
				{
					dragState = MinMaxSlider.DragState.MaxThumb;
				}
				else
				{
					dragState = MinMaxSlider.DragState.NoThumb;
				}
			}
			return dragState;
		}

		private void SetNavigationState(MinMaxSlider.DragState newState)
		{
			this.dragMinThumb.EnableInClassList(MinMaxSlider.movableUssClassName, newState == MinMaxSlider.DragState.MinThumb || newState == MinMaxSlider.DragState.MiddleThumb);
			this.dragMaxThumb.EnableInClassList(MinMaxSlider.movableUssClassName, newState == MinMaxSlider.DragState.MaxThumb || newState == MinMaxSlider.DragState.MiddleThumb);
			this.dragElement.EnableInClassList(MinMaxSlider.movableUssClassName, newState == MinMaxSlider.DragState.MiddleThumb);
		}

		private void OnFocusIn(FocusInEvent evt)
		{
			bool flag = this.GetNavigationState() == MinMaxSlider.DragState.NoThumb;
			if (flag)
			{
				this.SetNavigationState(MinMaxSlider.DragState.MinThumb);
			}
		}

		private void OnBlur(BlurEvent evt)
		{
			this.SetNavigationState(MinMaxSlider.DragState.NoThumb);
		}

		private void OnNavigationSubmit(NavigationSubmitEvent evt)
		{
			MinMaxSlider.DragState dragState = this.GetNavigationState() + 1;
			bool flag = dragState > MinMaxSlider.DragState.NoThumb;
			if (flag)
			{
				dragState = MinMaxSlider.DragState.MinThumb;
			}
			this.SetNavigationState(dragState);
		}

		private void OnNavigationMove(NavigationMoveEvent evt)
		{
			MinMaxSlider.DragState navigationState = this.GetNavigationState();
			bool flag = navigationState == MinMaxSlider.DragState.NoThumb;
			if (!flag)
			{
				bool flag2 = evt.direction != NavigationMoveEvent.Direction.Left && evt.direction != NavigationMoveEvent.Direction.Right;
				if (!flag2)
				{
					this.ComputeValueFromKey(evt.direction == NavigationMoveEvent.Direction.Left, evt.shiftKey, navigationState);
					evt.StopPropagation();
					FocusController focusController = this.focusController;
					if (focusController != null)
					{
						focusController.IgnoreEvent(evt);
					}
				}
			}
		}

		private void ComputeValueFromKey(bool leftDirection, bool isShift, MinMaxSlider.DragState moveState)
		{
			float num = BaseSlider<float>.GetClosestPowerOfTen(Mathf.Abs((this.highLimit - this.lowLimit) * 0.01f));
			if (isShift)
			{
				num *= 10f;
			}
			if (leftDirection)
			{
				num = -num;
			}
			switch (moveState)
			{
			case MinMaxSlider.DragState.MinThumb:
			{
				float num2 = BaseSlider<float>.RoundToMultipleOf(this.value.x + num * 0.5001f, Mathf.Abs(num));
				num2 = Math.Clamp(num2, this.lowLimit, this.value.y);
				this.value = new Vector2(num2, this.value.y);
				break;
			}
			case MinMaxSlider.DragState.MaxThumb:
			{
				float num3 = BaseSlider<float>.RoundToMultipleOf(this.value.y + num * 0.5001f, Mathf.Abs(num));
				num3 = Math.Clamp(num3, this.value.x, this.highLimit);
				this.value = new Vector2(this.value.x, num3);
				break;
			}
			case MinMaxSlider.DragState.MiddleThumb:
			{
				float num4 = this.value.y - this.value.x;
				bool flag = num > 0f;
				if (flag)
				{
					float num5 = BaseSlider<float>.RoundToMultipleOf(this.value.y + num * 0.5001f, Mathf.Abs(num));
					num5 = Math.Clamp(num5, this.value.x, this.highLimit);
					this.value = new Vector2(num5 - num4, num5);
				}
				else
				{
					float num6 = BaseSlider<float>.RoundToMultipleOf(this.value.x + num * 0.5001f, Mathf.Abs(num));
					num6 = Math.Clamp(num6, this.lowLimit, this.value.y);
					this.value = new Vector2(num6, num6 + num4);
				}
				break;
			}
			}
		}

		private void SetSliderValueFromDrag()
		{
			bool flag = this.clampedDragger.dragDirection != ClampedDragger<float>.DragDirection.Free;
			if (!flag)
			{
				float x = this.m_DragElementStartPos.x;
				float num = x + this.clampedDragger.delta.x;
				this.ComputeValueFromDraggingThumb(x, num);
			}
		}

		private void SetSliderValueFromClick()
		{
			bool flag = this.clampedDragger.dragDirection == ClampedDragger<float>.DragDirection.Free;
			if (!flag)
			{
				Vector2 vector = base.visualInput.LocalToWorld(this.clampedDragger.startMousePosition);
				bool flag2 = this.dragMinThumb.worldBound.Contains(vector);
				if (flag2)
				{
					this.m_DragState = MinMaxSlider.DragState.MinThumb;
				}
				else
				{
					bool flag3 = this.dragMaxThumb.worldBound.Contains(vector);
					if (flag3)
					{
						this.m_DragState = MinMaxSlider.DragState.MaxThumb;
					}
					else
					{
						bool flag4 = this.clampedDragger.startMousePosition.x > this.dragElement.layout.xMin && this.clampedDragger.startMousePosition.x < this.dragElement.layout.xMax;
						if (flag4)
						{
							this.m_DragState = MinMaxSlider.DragState.MiddleThumb;
						}
						else
						{
							this.m_DragState = MinMaxSlider.DragState.NoThumb;
						}
					}
				}
				bool flag5 = this.m_DragState == MinMaxSlider.DragState.NoThumb;
				if (flag5)
				{
					float num = this.ComputeValueFromPosition(this.clampedDragger.startMousePosition.x);
					bool flag6 = this.clampedDragger.startMousePosition.x < this.dragElement.layout.x;
					if (flag6)
					{
						this.m_DragState = MinMaxSlider.DragState.MinThumb;
						this.value = new Vector2(num, this.value.y);
					}
					else
					{
						this.m_DragState = MinMaxSlider.DragState.MaxThumb;
						this.value = new Vector2(this.value.x, num);
					}
				}
				this.SetNavigationState(this.m_DragState);
				this.m_ValueStartPos = this.value;
				this.clampedDragger.dragDirection = ClampedDragger<float>.DragDirection.Free;
				this.m_DragElementStartPos = this.clampedDragger.startMousePosition;
			}
		}

		private void ComputeValueFromDraggingThumb(float dragElementStartPos, float dragElementEndPos)
		{
			float num = this.ComputeValueFromPosition(dragElementStartPos);
			float num2 = this.ComputeValueFromPosition(dragElementEndPos);
			float num3 = num2 - num;
			this.SetNavigationState(this.m_DragState);
			switch (this.m_DragState)
			{
			case MinMaxSlider.DragState.MinThumb:
			{
				float num4 = this.m_ValueStartPos.x + num3;
				bool flag = num4 > this.maxValue;
				if (flag)
				{
					num4 = this.maxValue;
				}
				else
				{
					bool flag2 = num4 < this.lowLimit;
					if (flag2)
					{
						num4 = this.lowLimit;
					}
				}
				this.value = new Vector2(num4, this.maxValue);
				break;
			}
			case MinMaxSlider.DragState.MaxThumb:
			{
				float num5 = this.m_ValueStartPos.y + num3;
				bool flag3 = num5 < this.minValue;
				if (flag3)
				{
					num5 = this.minValue;
				}
				else
				{
					bool flag4 = num5 > this.highLimit;
					if (flag4)
					{
						num5 = this.highLimit;
					}
				}
				this.value = new Vector2(this.minValue, num5);
				break;
			}
			case MinMaxSlider.DragState.MiddleThumb:
			{
				Vector2 value = this.value;
				value.x = this.m_ValueStartPos.x + num3;
				value.y = this.m_ValueStartPos.y + num3;
				float num6 = this.m_ValueStartPos.y - this.m_ValueStartPos.x;
				bool flag5 = value.x < this.lowLimit;
				if (flag5)
				{
					value.x = this.lowLimit;
					value.y = this.lowLimit + num6;
				}
				else
				{
					bool flag6 = value.y > this.highLimit;
					if (flag6)
					{
						value.y = this.highLimit;
						value.x = this.highLimit - num6;
					}
				}
				this.value = value;
				break;
			}
			}
		}

		protected override void UpdateMixedValueContent()
		{
		}

		internal override void RegisterEditingCallbacks()
		{
			base.visualInput.RegisterCallback<PointerDownEvent>(new EventCallback<PointerDownEvent>(base.StartEditing), TrickleDown.TrickleDown);
			base.visualInput.RegisterCallback<PointerUpEvent>(new EventCallback<PointerUpEvent>(base.EndEditing), TrickleDown.NoTrickleDown);
		}

		internal override void UnregisterEditingCallbacks()
		{
			base.visualInput.UnregisterCallback<PointerDownEvent>(new EventCallback<PointerDownEvent>(base.StartEditing), TrickleDown.TrickleDown);
			base.visualInput.UnregisterCallback<PointerUpEvent>(new EventCallback<PointerUpEvent>(base.EndEditing), TrickleDown.NoTrickleDown);
		}

		internal static readonly BindingId minValueProperty = "minValue";

		internal static readonly BindingId maxValueProperty = "maxValue";

		internal static readonly BindingId rangeProperty = "range";

		internal static readonly BindingId lowLimitProperty = "lowLimit";

		internal static readonly BindingId highLimitProperty = "highLimit";

		private Vector2 m_DragElementStartPos;

		private Vector2 m_ValueStartPos;

		private MinMaxSlider.DragState m_DragState;

		private float m_MinLimit;

		private float m_MaxLimit;

		internal const float kDefaultHighValue = 10f;

		public new static readonly string ussClassName = "unity-min-max-slider";

		public new static readonly string labelUssClassName = MinMaxSlider.ussClassName + "__label";

		public new static readonly string inputUssClassName = MinMaxSlider.ussClassName + "__input";

		public static readonly string trackerUssClassName = MinMaxSlider.ussClassName + "__tracker";

		public static readonly string draggerUssClassName = MinMaxSlider.ussClassName + "__dragger";

		public static readonly string minThumbUssClassName = MinMaxSlider.ussClassName + "__min-thumb";

		public static readonly string maxThumbUssClassName = MinMaxSlider.ussClassName + "__max-thumb";

		public static readonly string movableUssClassName = MinMaxSlider.ussClassName + "--movable";

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : BaseField<Vector2>.UxmlSerializedData, IUxmlSerializedDataCustomAttributeHandler
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				BaseField<Vector2>.UxmlSerializedData.Register();
				UxmlDescriptionCache.RegisterType(typeof(MinMaxSlider.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("valueOverride", "value", null, Array.Empty<string>()),
					new UxmlAttributeNames("lowLimit", "low-limit", null, Array.Empty<string>()),
					new UxmlAttributeNames("highLimit", "high-limit", null, Array.Empty<string>())
				}, false);
			}

			void IUxmlSerializedDataCustomAttributeHandler.SerializeCustomAttributes(IUxmlAttributes bag, HashSet<string> handledAttributes)
			{
				int num = 0;
				float num2 = UxmlUtility.TryParseFloatAttribute("min-value", bag, ref num);
				float num3 = UxmlUtility.TryParseFloatAttribute("max-value", bag, ref num);
				bool flag = num > 0;
				if (flag)
				{
					this.valueOverride = new Vector2(num2, num3);
					handledAttributes.Add("value");
					UxmlAsset uxmlAsset = bag as UxmlAsset;
					bool flag2 = uxmlAsset != null;
					if (flag2)
					{
						uxmlAsset.RemoveAttribute("min-value");
						uxmlAsset.RemoveAttribute("max-value");
						uxmlAsset.SetAttribute("value", UxmlUtility.ValueToString(base.Value));
					}
				}
			}

			public override object CreateInstance()
			{
				return new MinMaxSlider();
			}

			public override void Deserialize(object obj)
			{
				base.Deserialize(obj);
				MinMaxSlider minMaxSlider = (MinMaxSlider)obj;
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.lowLimit_UxmlAttributeFlags);
				if (flag)
				{
					minMaxSlider.lowLimit = this.lowLimit;
				}
				bool flag2 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.highLimit_UxmlAttributeFlags);
				if (flag2)
				{
					minMaxSlider.highLimit = this.highLimit;
				}
				bool flag3 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.valueOverride_UxmlAttributeFlags);
				if (flag3)
				{
					minMaxSlider.valueOverride = this.valueOverride;
				}
			}

			[UxmlAttributeBindingPath("value")]
			[SerializeField]
			[Delayed]
			[UxmlAttribute("value")]
			private Vector2 valueOverride;

			[SerializeField]
			[Delayed]
			private float lowLimit;

			[SerializeField]
			[Delayed]
			private float highLimit;

			[SerializeField]
			[HideInInspector]
			[UxmlIgnore]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags valueOverride_UxmlAttributeFlags;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags lowLimit_UxmlAttributeFlags;

			[HideInInspector]
			[UxmlIgnore]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags highLimit_UxmlAttributeFlags;
		}

		[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlFactory : UxmlFactory<MinMaxSlider, MinMaxSlider.UxmlTraits>
		{
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlTraits : BaseField<Vector2>.UxmlTraits
		{
			public UxmlTraits()
			{
				this.m_PickingMode.defaultValue = PickingMode.Ignore;
			}

			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				MinMaxSlider minMaxSlider = (MinMaxSlider)ve;
				minMaxSlider.lowLimit = this.m_LowLimit.GetValueFromBag(bag, cc);
				minMaxSlider.highLimit = this.m_HighLimit.GetValueFromBag(bag, cc);
				Vector2 vector = new Vector2(this.m_MinValue.GetValueFromBag(bag, cc), this.m_MaxValue.GetValueFromBag(bag, cc));
				minMaxSlider.value = vector;
			}

			private UxmlFloatAttributeDescription m_MinValue = new UxmlFloatAttributeDescription
			{
				name = "min-value",
				defaultValue = 0f
			};

			private UxmlFloatAttributeDescription m_MaxValue = new UxmlFloatAttributeDescription
			{
				name = "max-value",
				defaultValue = 10f
			};

			private UxmlFloatAttributeDescription m_LowLimit = new UxmlFloatAttributeDescription
			{
				name = "low-limit",
				defaultValue = float.MinValue
			};

			private UxmlFloatAttributeDescription m_HighLimit = new UxmlFloatAttributeDescription
			{
				name = "high-limit",
				defaultValue = float.MaxValue
			};
		}

		private enum DragState
		{
			MinThumb,
			MaxThumb,
			MiddleThumb,
			NoThumb
		}
	}
}
