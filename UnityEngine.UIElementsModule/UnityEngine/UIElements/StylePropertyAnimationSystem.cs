using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.Assertions;
using UnityEngine.Bindings;
using UnityEngine.Pool;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal class StylePropertyAnimationSystem : IStylePropertyAnimationSystem
	{
		public StylePropertyAnimationSystem(BaseVisualElementPanel p)
		{
			this.m_CurrentTime = p.TimeSinceStartupSeconds();
		}

		private T GetOrCreate<T>(ref T values) where T : new()
		{
			T t = values;
			return (t != null) ? t : (values = new T());
		}

		private bool StartTransition<T>(VisualElement owner, StylePropertyId prop, T startValue, T endValue, int durationMs, int delayMs, Func<float, float> easingCurve, StylePropertyAnimationSystem.Values<T> values)
		{
			this.m_PropertyToValues[prop] = values;
			bool flag = values.StartTransition(owner, prop, startValue, endValue, (float)durationMs / 1000f, (float)delayMs / 1000f, easingCurve, this.CurrentTimeSeconds());
			this.UpdateTracking<T>(values);
			return flag;
		}

		public bool StartTransition(VisualElement owner, StylePropertyId prop, float startValue, float endValue, int durationMs, int delayMs, [JetBrains.Annotations.NotNull] Func<float, float> easingCurve)
		{
			return this.StartTransition<float>(owner, prop, startValue, endValue, durationMs, delayMs, easingCurve, this.GetOrCreate<StylePropertyAnimationSystem.ValuesFloat>(ref this.m_Floats));
		}

		public bool StartTransition(VisualElement owner, StylePropertyId prop, int startValue, int endValue, int durationMs, int delayMs, [JetBrains.Annotations.NotNull] Func<float, float> easingCurve)
		{
			return this.StartTransition<int>(owner, prop, startValue, endValue, durationMs, delayMs, easingCurve, this.GetOrCreate<StylePropertyAnimationSystem.ValuesInt>(ref this.m_Ints));
		}

		public bool StartTransition(VisualElement owner, StylePropertyId prop, Length startValue, Length endValue, int durationMs, int delayMs, [JetBrains.Annotations.NotNull] Func<float, float> easingCurve)
		{
			return this.StartTransition<Length>(owner, prop, startValue, endValue, durationMs, delayMs, easingCurve, this.GetOrCreate<StylePropertyAnimationSystem.ValuesLength>(ref this.m_Lengths));
		}

		public bool StartTransition(VisualElement owner, StylePropertyId prop, Color startValue, Color endValue, int durationMs, int delayMs, [JetBrains.Annotations.NotNull] Func<float, float> easingCurve)
		{
			return this.StartTransition<Color>(owner, prop, startValue, endValue, durationMs, delayMs, easingCurve, this.GetOrCreate<StylePropertyAnimationSystem.ValuesColor>(ref this.m_Colors));
		}

		public bool StartTransitionEnum(VisualElement owner, StylePropertyId prop, int startValue, int endValue, int durationMs, int delayMs, [JetBrains.Annotations.NotNull] Func<float, float> easingCurve)
		{
			return this.StartTransition<int>(owner, prop, startValue, endValue, durationMs, delayMs, easingCurve, this.GetOrCreate<StylePropertyAnimationSystem.ValuesEnum>(ref this.m_Enums));
		}

		public bool StartTransition(VisualElement owner, StylePropertyId prop, Background startValue, Background endValue, int durationMs, int delayMs, [JetBrains.Annotations.NotNull] Func<float, float> easingCurve)
		{
			return this.StartTransition<Background>(owner, prop, startValue, endValue, durationMs, delayMs, easingCurve, this.GetOrCreate<StylePropertyAnimationSystem.ValuesBackground>(ref this.m_Backgrounds));
		}

		public bool StartTransition(VisualElement owner, StylePropertyId prop, FontDefinition startValue, FontDefinition endValue, int durationMs, int delayMs, [JetBrains.Annotations.NotNull] Func<float, float> easingCurve)
		{
			return this.StartTransition<FontDefinition>(owner, prop, startValue, endValue, durationMs, delayMs, easingCurve, this.GetOrCreate<StylePropertyAnimationSystem.ValuesFontDefinition>(ref this.m_FontDefinitions));
		}

		public bool StartTransition(VisualElement owner, StylePropertyId prop, Font startValue, Font endValue, int durationMs, int delayMs, [JetBrains.Annotations.NotNull] Func<float, float> easingCurve)
		{
			return this.StartTransition<Font>(owner, prop, startValue, endValue, durationMs, delayMs, easingCurve, this.GetOrCreate<StylePropertyAnimationSystem.ValuesFont>(ref this.m_Fonts));
		}

		public bool StartTransition(VisualElement owner, StylePropertyId prop, TextShadow startValue, TextShadow endValue, int durationMs, int delayMs, [JetBrains.Annotations.NotNull] Func<float, float> easingCurve)
		{
			return this.StartTransition<TextShadow>(owner, prop, startValue, endValue, durationMs, delayMs, easingCurve, this.GetOrCreate<StylePropertyAnimationSystem.ValuesTextShadow>(ref this.m_TextShadows));
		}

		public bool StartTransition(VisualElement owner, StylePropertyId prop, Scale startValue, Scale endValue, int durationMs, int delayMs, [JetBrains.Annotations.NotNull] Func<float, float> easingCurve)
		{
			return this.StartTransition<Scale>(owner, prop, startValue, endValue, durationMs, delayMs, easingCurve, this.GetOrCreate<StylePropertyAnimationSystem.ValuesScale>(ref this.m_Scale));
		}

		public bool StartTransition(VisualElement owner, StylePropertyId prop, Rotate startValue, Rotate endValue, int durationMs, int delayMs, [JetBrains.Annotations.NotNull] Func<float, float> easingCurve)
		{
			return this.StartTransition<Rotate>(owner, prop, startValue, endValue, durationMs, delayMs, easingCurve, this.GetOrCreate<StylePropertyAnimationSystem.ValuesRotate>(ref this.m_Rotate));
		}

		public bool StartTransition(VisualElement owner, StylePropertyId prop, Translate startValue, Translate endValue, int durationMs, int delayMs, [JetBrains.Annotations.NotNull] Func<float, float> easingCurve)
		{
			return this.StartTransition<Translate>(owner, prop, startValue, endValue, durationMs, delayMs, easingCurve, this.GetOrCreate<StylePropertyAnimationSystem.ValuesTranslate>(ref this.m_Translate));
		}

		public bool StartTransition(VisualElement owner, StylePropertyId prop, Ratio startValue, Ratio endValue, int durationMs, int delayMs, [JetBrains.Annotations.NotNull] Func<float, float> easingCurve)
		{
			return this.StartTransition<Ratio>(owner, prop, startValue, endValue, durationMs, delayMs, easingCurve, this.GetOrCreate<StylePropertyAnimationSystem.ValuesRatio>(ref this.m_Ratio));
		}

		public bool StartTransition(VisualElement owner, StylePropertyId prop, TransformOrigin startValue, TransformOrigin endValue, int durationMs, int delayMs, [JetBrains.Annotations.NotNull] Func<float, float> easingCurve)
		{
			return this.StartTransition<TransformOrigin>(owner, prop, startValue, endValue, durationMs, delayMs, easingCurve, this.GetOrCreate<StylePropertyAnimationSystem.ValuesTransformOrigin>(ref this.m_TransformOrigin));
		}

		public bool StartTransition(VisualElement owner, StylePropertyId prop, BackgroundPosition startValue, BackgroundPosition endValue, int durationMs, int delayMs, [JetBrains.Annotations.NotNull] Func<float, float> easingCurve)
		{
			return this.StartTransition<BackgroundPosition>(owner, prop, startValue, endValue, durationMs, delayMs, easingCurve, this.GetOrCreate<StylePropertyAnimationSystem.ValuesBackgroundPosition>(ref this.m_BackgroundPosition));
		}

		public bool StartTransition(VisualElement owner, StylePropertyId prop, BackgroundRepeat startValue, BackgroundRepeat endValue, int durationMs, int delayMs, [JetBrains.Annotations.NotNull] Func<float, float> easingCurve)
		{
			return this.StartTransition<BackgroundRepeat>(owner, prop, startValue, endValue, durationMs, delayMs, easingCurve, this.GetOrCreate<StylePropertyAnimationSystem.ValuesBackgroundRepeat>(ref this.m_BackgroundRepeat));
		}

		public bool StartTransition(VisualElement owner, StylePropertyId prop, BackgroundSize startValue, BackgroundSize endValue, int durationMs, int delayMs, [JetBrains.Annotations.NotNull] Func<float, float> easingCurve)
		{
			return this.StartTransition<BackgroundSize>(owner, prop, startValue, endValue, durationMs, delayMs, easingCurve, this.GetOrCreate<StylePropertyAnimationSystem.ValuesBackgroundSize>(ref this.m_BackgroundSize));
		}

		public bool StartTransition(VisualElement owner, StylePropertyId prop, List<FilterFunction> startValue, List<FilterFunction> endValue, int durationMs, int delayMs, [JetBrains.Annotations.NotNull] Func<float, float> easingCurve)
		{
			return this.StartTransition<List<FilterFunction>>(owner, prop, startValue, endValue, durationMs, delayMs, easingCurve, this.GetOrCreate<StylePropertyAnimationSystem.ValuesListFilterFunction>(ref this.m_FilterFunctions));
		}

		public bool StartTransition(VisualElement owner, StylePropertyId prop, MaterialDefinition startValue, MaterialDefinition endValue, int durationMs, int delayMs, [JetBrains.Annotations.NotNull] Func<float, float> easingCurve)
		{
			return this.StartTransition<MaterialDefinition>(owner, prop, startValue, endValue, durationMs, delayMs, easingCurve, this.GetOrCreate<StylePropertyAnimationSystem.ValuesMaterialDefinition>(ref this.m_MaterialDefinition));
		}

		public void CancelAllAnimations()
		{
			foreach (StylePropertyAnimationSystem.Values values in this.m_AllValues)
			{
				values.CancelAllAnimations();
			}
		}

		public void CancelAllAnimations(VisualElement owner)
		{
			foreach (StylePropertyAnimationSystem.Values values in this.m_AllValues)
			{
				values.CancelAllAnimations(owner);
			}
			Assert.AreEqual(0, owner.styleAnimation.runningAnimationCount);
			Assert.AreEqual(0, owner.styleAnimation.completedAnimationCount);
		}

		public void CancelAnimation(VisualElement owner, StylePropertyId id)
		{
			StylePropertyAnimationSystem.Values values;
			bool flag = this.m_PropertyToValues.TryGetValue(id, out values);
			if (flag)
			{
				values.CancelAnimation(owner, id);
			}
		}

		public bool HasRunningAnimation(VisualElement owner, StylePropertyId id)
		{
			StylePropertyAnimationSystem.Values values;
			return this.m_PropertyToValues.TryGetValue(id, out values) && values.HasRunningAnimation(owner, id);
		}

		public void UpdateAnimation(VisualElement owner, StylePropertyId id)
		{
			StylePropertyAnimationSystem.Values values;
			bool flag = this.m_PropertyToValues.TryGetValue(id, out values);
			if (flag)
			{
				values.UpdateAnimation(owner, id);
			}
		}

		public void GetAllAnimations(VisualElement owner, List<StylePropertyId> propertyIds)
		{
			foreach (StylePropertyAnimationSystem.Values values in this.m_AllValues)
			{
				values.GetAllAnimations(owner, propertyIds);
			}
		}

		private void UpdateTracking<T>(StylePropertyAnimationSystem.Values<T> values)
		{
			bool flag = !values.isEmpty && !this.m_AllValues.Contains(values);
			if (flag)
			{
				this.m_AllValues.Add(values);
			}
		}

		private double CurrentTimeSeconds()
		{
			return this.m_CurrentTime;
		}

		public void Update(double updateTime)
		{
			this.m_CurrentTime = updateTime;
			int count = this.m_AllValues.Count;
			for (int i = 0; i < count; i++)
			{
				this.m_AllValues[i].Update(this.m_CurrentTime);
			}
		}

		private double m_CurrentTime = 0.0;

		private StylePropertyAnimationSystem.ValuesFloat m_Floats;

		private StylePropertyAnimationSystem.ValuesInt m_Ints;

		private StylePropertyAnimationSystem.ValuesLength m_Lengths;

		private StylePropertyAnimationSystem.ValuesColor m_Colors;

		private StylePropertyAnimationSystem.ValuesEnum m_Enums;

		private StylePropertyAnimationSystem.ValuesBackground m_Backgrounds;

		private StylePropertyAnimationSystem.ValuesFontDefinition m_FontDefinitions;

		private StylePropertyAnimationSystem.ValuesFont m_Fonts;

		private StylePropertyAnimationSystem.ValuesTextShadow m_TextShadows;

		private StylePropertyAnimationSystem.ValuesScale m_Scale;

		private StylePropertyAnimationSystem.ValuesRotate m_Rotate;

		private StylePropertyAnimationSystem.ValuesRatio m_Ratio;

		private StylePropertyAnimationSystem.ValuesTranslate m_Translate;

		private StylePropertyAnimationSystem.ValuesTransformOrigin m_TransformOrigin;

		private StylePropertyAnimationSystem.ValuesBackgroundPosition m_BackgroundPosition;

		private StylePropertyAnimationSystem.ValuesBackgroundRepeat m_BackgroundRepeat;

		private StylePropertyAnimationSystem.ValuesBackgroundSize m_BackgroundSize;

		private StylePropertyAnimationSystem.ValuesListFilterFunction m_FilterFunctions;

		private StylePropertyAnimationSystem.ValuesMaterialDefinition m_MaterialDefinition;

		private readonly List<StylePropertyAnimationSystem.Values> m_AllValues = new List<StylePropertyAnimationSystem.Values>();

		private readonly Dictionary<StylePropertyId, StylePropertyAnimationSystem.Values> m_PropertyToValues = new Dictionary<StylePropertyId, StylePropertyAnimationSystem.Values>();

		[Flags]
		private enum TransitionState
		{
			None = 0,
			Running = 1,
			Started = 2,
			Ended = 4,
			Canceled = 8
		}

		private struct AnimationDataSet<TTimingData, TStyleData>
		{
			private int capacity
			{
				get
				{
					return this.elements.Length;
				}
				set
				{
					Array.Resize<VisualElement>(ref this.elements, value);
					Array.Resize<StylePropertyId>(ref this.properties, value);
					Array.Resize<TTimingData>(ref this.timing, value);
					Array.Resize<TStyleData>(ref this.style, value);
				}
			}

			private void LocalInit()
			{
				this.elements = new VisualElement[2];
				this.properties = new StylePropertyId[2];
				this.timing = new TTimingData[2];
				this.style = new TStyleData[2];
				this.indices = new Dictionary<StylePropertyAnimationSystem.ElementPropertyPair, int>(StylePropertyAnimationSystem.ElementPropertyPair.Comparer);
			}

			public static StylePropertyAnimationSystem.AnimationDataSet<TTimingData, TStyleData> Create()
			{
				StylePropertyAnimationSystem.AnimationDataSet<TTimingData, TStyleData> animationDataSet = default(StylePropertyAnimationSystem.AnimationDataSet<TTimingData, TStyleData>);
				animationDataSet.LocalInit();
				return animationDataSet;
			}

			public bool IndexOf(VisualElement ve, StylePropertyId prop, out int index)
			{
				return this.indices.TryGetValue(new StylePropertyAnimationSystem.ElementPropertyPair(ve, prop), out index);
			}

			public void Add(VisualElement owner, StylePropertyId prop, TTimingData timingData, TStyleData styleData)
			{
				bool flag = this.count >= this.capacity;
				if (flag)
				{
					this.capacity *= 2;
				}
				int num = this.count;
				this.count = num + 1;
				int num2 = num;
				this.elements[num2] = owner;
				this.properties[num2] = prop;
				this.timing[num2] = timingData;
				this.style[num2] = styleData;
				this.indices.Add(new StylePropertyAnimationSystem.ElementPropertyPair(owner, prop), num2);
			}

			public void Remove(int cancelledIndex)
			{
				int num = this.count - 1;
				this.count = num;
				int num2 = num;
				this.indices.Remove(new StylePropertyAnimationSystem.ElementPropertyPair(this.elements[cancelledIndex], this.properties[cancelledIndex]));
				bool flag = cancelledIndex != num2;
				if (flag)
				{
					VisualElement visualElement = (this.elements[cancelledIndex] = this.elements[num2]);
					StylePropertyId stylePropertyId = (this.properties[cancelledIndex] = this.properties[num2]);
					this.timing[cancelledIndex] = this.timing[num2];
					this.style[cancelledIndex] = this.style[num2];
					this.indices[new StylePropertyAnimationSystem.ElementPropertyPair(visualElement, stylePropertyId)] = cancelledIndex;
				}
				this.elements[num2] = null;
				this.properties[num2] = StylePropertyId.Unknown;
				this.timing[num2] = default(TTimingData);
				this.style[num2] = default(TStyleData);
			}

			public void Replace(int index, TTimingData timingData, TStyleData styleData)
			{
				this.timing[index] = timingData;
				this.style[index] = styleData;
			}

			public void RemoveAll(VisualElement ve)
			{
				int num = this.count;
				for (int i = num - 1; i >= 0; i--)
				{
					bool flag = this.elements[i] == ve;
					if (flag)
					{
						this.Remove(i);
					}
				}
			}

			public void RemoveAll()
			{
				this.capacity = 2;
				int num = Mathf.Min(this.count, this.capacity);
				Array.Clear(this.elements, 0, num);
				Array.Clear(this.properties, 0, num);
				Array.Clear(this.timing, 0, num);
				Array.Clear(this.style, 0, num);
				this.count = 0;
				this.indices.Clear();
			}

			public void GetActivePropertiesForElement(VisualElement ve, List<StylePropertyId> outProperties)
			{
				int num = this.count;
				for (int i = num - 1; i >= 0; i--)
				{
					bool flag = this.elements[i] == ve;
					if (flag)
					{
						outProperties.Add(this.properties[i]);
					}
				}
			}

			private const int InitialSize = 2;

			public VisualElement[] elements;

			public StylePropertyId[] properties;

			public TTimingData[] timing;

			public TStyleData[] style;

			public int count;

			private Dictionary<StylePropertyAnimationSystem.ElementPropertyPair, int> indices;
		}

		private struct ElementPropertyPair
		{
			public ElementPropertyPair(VisualElement element, StylePropertyId property)
			{
				this.element = element;
				this.property = property;
			}

			public static readonly IEqualityComparer<StylePropertyAnimationSystem.ElementPropertyPair> Comparer = new StylePropertyAnimationSystem.ElementPropertyPair.EqualityComparer();

			public readonly VisualElement element;

			public readonly StylePropertyId property;

			private class EqualityComparer : IEqualityComparer<StylePropertyAnimationSystem.ElementPropertyPair>
			{
				public bool Equals(StylePropertyAnimationSystem.ElementPropertyPair x, StylePropertyAnimationSystem.ElementPropertyPair y)
				{
					return x.element == y.element && x.property == y.property;
				}

				public int GetHashCode(StylePropertyAnimationSystem.ElementPropertyPair obj)
				{
					return (obj.element.GetHashCode() * 397) ^ (int)obj.property;
				}
			}
		}

		private abstract class Values
		{
			public abstract void CancelAllAnimations();

			public abstract void CancelAllAnimations(VisualElement ve);

			public abstract void CancelAnimation(VisualElement ve, StylePropertyId id);

			public abstract bool HasRunningAnimation(VisualElement ve, StylePropertyId id);

			public abstract void UpdateAnimation(VisualElement ve, StylePropertyId id);

			public abstract void GetAllAnimations(VisualElement ve, List<StylePropertyId> outPropertyIds);

			public abstract void Update(double currentTime);

			protected abstract void UpdateValues();

			protected abstract void UpdateComputedStyle();

			protected abstract void UpdateComputedStyle(int i);
		}

		private abstract class Values<T> : StylePropertyAnimationSystem.Values
		{
			public bool isEmpty
			{
				get
				{
					return this.running.count + this.completed.count == 0;
				}
			}

			public abstract Func<T, T, bool> SameFunc { get; }

			protected virtual bool ConvertUnits(VisualElement owner, StylePropertyId prop, ref T a, ref T b)
			{
				return true;
			}

			protected virtual T Copy(T value)
			{
				return value;
			}

			protected Values()
			{
				this.running = StylePropertyAnimationSystem.AnimationDataSet<StylePropertyAnimationSystem.Values<T>.TimingData, StylePropertyAnimationSystem.Values<T>.StyleData>.Create();
				this.completed = StylePropertyAnimationSystem.AnimationDataSet<StylePropertyAnimationSystem.Values<T>.EmptyData, T>.Create();
				this.m_CurrentTime = 0.0;
			}

			private void SwapFrameStates()
			{
				StylePropertyAnimationSystem.Values<T>.TransitionEventsFrameState currentFrameEventsState = this.m_CurrentFrameEventsState;
				this.m_CurrentFrameEventsState = this.m_NextFrameEventsState;
				this.m_NextFrameEventsState = currentFrameEventsState;
			}

			private void QueueEvent(EventBase evt, StylePropertyAnimationSystem.ElementPropertyPair epp)
			{
				evt.elementTarget = epp.element;
				Queue<EventBase> pooledQueue;
				bool flag = !this.m_NextFrameEventsState.elementPropertyQueuedEvents.TryGetValue(epp, out pooledQueue);
				if (flag)
				{
					pooledQueue = StylePropertyAnimationSystem.Values<T>.TransitionEventsFrameState.GetPooledQueue();
					this.m_NextFrameEventsState.elementPropertyQueuedEvents.Add(epp, pooledQueue);
				}
				pooledQueue.Enqueue(evt);
				bool flag2 = this.m_NextFrameEventsState.panel == null;
				if (flag2)
				{
					this.m_NextFrameEventsState.panel = epp.element.panel;
				}
				this.m_NextFrameEventsState.RegisterChange();
			}

			private void ClearEventQueue(StylePropertyAnimationSystem.ElementPropertyPair epp)
			{
				Queue<EventBase> queue;
				bool flag = this.m_NextFrameEventsState.elementPropertyQueuedEvents.TryGetValue(epp, out queue);
				if (flag)
				{
					while (queue.Count > 0)
					{
						queue.Dequeue().Dispose();
						this.m_NextFrameEventsState.UnregisterChange();
					}
				}
			}

			private void QueueTransitionRunEvent(VisualElement ve, int runningIndex)
			{
				bool flag = !ve.HasParentEventInterests(EventCategory.StyleTransition);
				if (!flag)
				{
					StylePropertyId stylePropertyId = this.running.properties[runningIndex];
					StylePropertyAnimationSystem.ElementPropertyPair elementPropertyPair = new StylePropertyAnimationSystem.ElementPropertyPair(ve, stylePropertyId);
					StylePropertyAnimationSystem.TransitionState transitionState;
					bool flag2 = this.m_NextFrameEventsState.elementPropertyStateDelta.TryGetValue(elementPropertyPair, out transitionState);
					if (flag2)
					{
						this.m_NextFrameEventsState.elementPropertyStateDelta[elementPropertyPair] = transitionState | StylePropertyAnimationSystem.TransitionState.Running;
					}
					else
					{
						this.m_NextFrameEventsState.elementPropertyStateDelta.Add(elementPropertyPair, StylePropertyAnimationSystem.TransitionState.Running);
					}
					ref StylePropertyAnimationSystem.Values<T>.TimingData ptr = ref this.running.timing[runningIndex];
					float num = ((ptr.delay < 0f) ? Mathf.Min(Mathf.Max(-ptr.delay, 0f), ptr.duration) : 0f);
					TransitionRunEvent pooled = TransitionEventBase<TransitionRunEvent>.GetPooled(new StylePropertyName(stylePropertyId), (double)num);
					this.QueueEvent(pooled, elementPropertyPair);
				}
			}

			private void QueueTransitionStartEvent(VisualElement ve, int runningIndex)
			{
				bool flag = !ve.HasParentEventInterests(EventCategory.StyleTransition);
				if (!flag)
				{
					StylePropertyId stylePropertyId = this.running.properties[runningIndex];
					StylePropertyAnimationSystem.ElementPropertyPair elementPropertyPair = new StylePropertyAnimationSystem.ElementPropertyPair(ve, stylePropertyId);
					StylePropertyAnimationSystem.TransitionState transitionState;
					bool flag2 = this.m_NextFrameEventsState.elementPropertyStateDelta.TryGetValue(elementPropertyPair, out transitionState);
					if (flag2)
					{
						this.m_NextFrameEventsState.elementPropertyStateDelta[elementPropertyPair] = transitionState | StylePropertyAnimationSystem.TransitionState.Started;
					}
					else
					{
						this.m_NextFrameEventsState.elementPropertyStateDelta.Add(elementPropertyPair, StylePropertyAnimationSystem.TransitionState.Started);
					}
					ref StylePropertyAnimationSystem.Values<T>.TimingData ptr = ref this.running.timing[runningIndex];
					float num = ((ptr.delay < 0f) ? Mathf.Min(Mathf.Max(-ptr.delay, 0f), ptr.duration) : 0f);
					TransitionStartEvent pooled = TransitionEventBase<TransitionStartEvent>.GetPooled(new StylePropertyName(stylePropertyId), (double)num);
					this.QueueEvent(pooled, elementPropertyPair);
				}
			}

			private void QueueTransitionEndEvent(VisualElement ve, int runningIndex)
			{
				bool flag = !ve.HasParentEventInterests(EventCategory.StyleTransition);
				if (!flag)
				{
					StylePropertyId stylePropertyId = this.running.properties[runningIndex];
					StylePropertyAnimationSystem.ElementPropertyPair elementPropertyPair = new StylePropertyAnimationSystem.ElementPropertyPair(ve, stylePropertyId);
					StylePropertyAnimationSystem.TransitionState transitionState;
					bool flag2 = this.m_NextFrameEventsState.elementPropertyStateDelta.TryGetValue(elementPropertyPair, out transitionState);
					if (flag2)
					{
						this.m_NextFrameEventsState.elementPropertyStateDelta[elementPropertyPair] = transitionState | StylePropertyAnimationSystem.TransitionState.Ended;
					}
					else
					{
						this.m_NextFrameEventsState.elementPropertyStateDelta.Add(elementPropertyPair, StylePropertyAnimationSystem.TransitionState.Ended);
					}
					ref StylePropertyAnimationSystem.Values<T>.TimingData ptr = ref this.running.timing[runningIndex];
					TransitionEndEvent pooled = TransitionEventBase<TransitionEndEvent>.GetPooled(new StylePropertyName(stylePropertyId), (double)ptr.duration);
					this.QueueEvent(pooled, elementPropertyPair);
				}
			}

			private void QueueTransitionCancelEvent(VisualElement ve, int runningIndex, double panelElapsed)
			{
				bool flag = !ve.HasParentEventInterests(EventCategory.StyleTransition);
				if (!flag)
				{
					StylePropertyId stylePropertyId = this.running.properties[runningIndex];
					StylePropertyAnimationSystem.ElementPropertyPair elementPropertyPair = new StylePropertyAnimationSystem.ElementPropertyPair(ve, stylePropertyId);
					StylePropertyAnimationSystem.TransitionState transitionState;
					bool flag2 = this.m_NextFrameEventsState.elementPropertyStateDelta.TryGetValue(elementPropertyPair, out transitionState);
					bool flag4;
					if (flag2)
					{
						bool flag3 = transitionState == StylePropertyAnimationSystem.TransitionState.None || (transitionState & StylePropertyAnimationSystem.TransitionState.Canceled) == StylePropertyAnimationSystem.TransitionState.Canceled;
						if (flag3)
						{
							this.m_NextFrameEventsState.elementPropertyStateDelta[elementPropertyPair] = StylePropertyAnimationSystem.TransitionState.Canceled;
							this.ClearEventQueue(elementPropertyPair);
							flag4 = true;
						}
						else
						{
							this.m_NextFrameEventsState.elementPropertyStateDelta[elementPropertyPair] = StylePropertyAnimationSystem.TransitionState.None;
							this.ClearEventQueue(elementPropertyPair);
							flag4 = false;
						}
					}
					else
					{
						this.m_NextFrameEventsState.elementPropertyStateDelta.Add(elementPropertyPair, StylePropertyAnimationSystem.TransitionState.Canceled);
						flag4 = true;
					}
					bool flag5 = !flag4;
					if (!flag5)
					{
						ref StylePropertyAnimationSystem.Values<T>.TimingData ptr = ref this.running.timing[runningIndex];
						double num = (ptr.isStarted ? (panelElapsed - ptr.startTime) : 0.0);
						bool flag6 = ptr.delay < 0f;
						if (flag6)
						{
							num = (double)(-(double)ptr.delay) + num;
						}
						TransitionCancelEvent pooled = TransitionEventBase<TransitionCancelEvent>.GetPooled(new StylePropertyName(stylePropertyId), num);
						this.QueueEvent(pooled, elementPropertyPair);
					}
				}
			}

			private void SendTransitionCancelEvent(VisualElement ve, int runningIndex, double panelElapsed)
			{
				bool flag = !ve.HasParentEventInterests(EventBase<TransitionCancelEvent>.EventCategory);
				if (!flag)
				{
					ref StylePropertyAnimationSystem.Values<T>.TimingData ptr = ref this.running.timing[runningIndex];
					StylePropertyId stylePropertyId = this.running.properties[runningIndex];
					double num = (ptr.isStarted ? (panelElapsed - ptr.startTime) : 0.0);
					bool flag2 = ptr.delay < 0f;
					if (flag2)
					{
						num = (double)(-(double)ptr.delay) + num;
					}
					using (TransitionCancelEvent pooled = TransitionEventBase<TransitionCancelEvent>.GetPooled(new StylePropertyName(stylePropertyId), num))
					{
						pooled.elementTarget = ve;
						ve.SendEvent(pooled);
					}
				}
			}

			public sealed override void CancelAllAnimations()
			{
				int count = this.running.count;
				bool flag = count > 0;
				if (flag)
				{
					using (new EventDispatcherGate(this.running.elements[0].panel.dispatcher))
					{
						for (int i = 0; i < count; i++)
						{
							VisualElement visualElement = this.running.elements[i];
							this.SendTransitionCancelEvent(visualElement, i, this.m_CurrentTime);
							this.ForceComputedStyleEndValue(i);
							IStylePropertyAnimations styleAnimation = visualElement.styleAnimation;
							int num = styleAnimation.runningAnimationCount;
							styleAnimation.runningAnimationCount = num - 1;
						}
					}
					this.running.RemoveAll();
				}
				int count2 = this.completed.count;
				for (int j = 0; j < count2; j++)
				{
					VisualElement visualElement2 = this.completed.elements[j];
					IStylePropertyAnimations styleAnimation2 = visualElement2.styleAnimation;
					int num = styleAnimation2.completedAnimationCount;
					styleAnimation2.completedAnimationCount = num - 1;
				}
				this.completed.RemoveAll();
			}

			public sealed override void CancelAllAnimations(VisualElement ve)
			{
				int count = this.running.count;
				bool flag = count > 0;
				if (flag)
				{
					using (new EventDispatcherGate(this.running.elements[0].panel.dispatcher))
					{
						for (int i = 0; i < count; i++)
						{
							bool flag2 = this.running.elements[i] == ve;
							if (flag2)
							{
								this.SendTransitionCancelEvent(ve, i, this.m_CurrentTime);
								this.ForceComputedStyleEndValue(i);
								IStylePropertyAnimations styleAnimation = this.running.elements[i].styleAnimation;
								int num = styleAnimation.runningAnimationCount;
								styleAnimation.runningAnimationCount = num - 1;
							}
						}
					}
				}
				this.running.RemoveAll(ve);
				int count2 = this.completed.count;
				for (int j = 0; j < count2; j++)
				{
					bool flag3 = this.completed.elements[j] == ve;
					if (flag3)
					{
						IStylePropertyAnimations styleAnimation2 = this.completed.elements[j].styleAnimation;
						int num = styleAnimation2.completedAnimationCount;
						styleAnimation2.completedAnimationCount = num - 1;
					}
				}
				this.completed.RemoveAll(ve);
			}

			public sealed override void CancelAnimation(VisualElement ve, StylePropertyId id)
			{
				int num;
				bool flag = this.running.IndexOf(ve, id, out num);
				if (flag)
				{
					this.QueueTransitionCancelEvent(ve, num, this.m_CurrentTime);
					this.ForceComputedStyleEndValue(num);
					this.running.Remove(num);
					IStylePropertyAnimations styleAnimation = ve.styleAnimation;
					int num2 = styleAnimation.runningAnimationCount;
					styleAnimation.runningAnimationCount = num2 - 1;
				}
				int num3;
				bool flag2 = this.completed.IndexOf(ve, id, out num3);
				if (flag2)
				{
					this.completed.Remove(num3);
					IStylePropertyAnimations styleAnimation2 = ve.styleAnimation;
					int num2 = styleAnimation2.completedAnimationCount;
					styleAnimation2.completedAnimationCount = num2 - 1;
				}
			}

			public sealed override bool HasRunningAnimation(VisualElement ve, StylePropertyId id)
			{
				int num;
				return this.running.IndexOf(ve, id, out num);
			}

			public sealed override void UpdateAnimation(VisualElement ve, StylePropertyId id)
			{
				int num;
				bool flag = this.running.IndexOf(ve, id, out num);
				if (flag)
				{
					this.UpdateComputedStyle(num);
				}
			}

			public sealed override void GetAllAnimations(VisualElement ve, List<StylePropertyId> outPropertyIds)
			{
				this.running.GetActivePropertiesForElement(ve, outPropertyIds);
				this.completed.GetActivePropertiesForElement(ve, outPropertyIds);
			}

			private float ComputeReversingShorteningFactor(int oldIndex)
			{
				ref StylePropertyAnimationSystem.Values<T>.TimingData ptr = ref this.running.timing[oldIndex];
				return Mathf.Clamp01(Mathf.Abs(1f - (1f - ptr.easedProgress) * ptr.reversingShorteningFactor));
			}

			private float ComputeReversingDuration(float newTransitionDuration, float newReversingShorteningFactor)
			{
				return newTransitionDuration * newReversingShorteningFactor;
			}

			private float ComputeReversingDelay(float delay, float newReversingShorteningFactor)
			{
				return (delay < 0f) ? (delay * newReversingShorteningFactor) : delay;
			}

			public bool StartTransition(VisualElement owner, StylePropertyId prop, T startValue, T endValue, float duration, float delay, Func<float, float> easingCurve, double currentTime)
			{
				double num = currentTime + (double)delay;
				StylePropertyAnimationSystem.Values<T>.TimingData timingData = new StylePropertyAnimationSystem.Values<T>.TimingData
				{
					startTime = num,
					duration = duration,
					easingCurve = easingCurve,
					reversingShorteningFactor = 1f,
					delay = delay
				};
				StylePropertyAnimationSystem.Values<T>.StyleData styleData = new StylePropertyAnimationSystem.Values<T>.StyleData
				{
					startValue = this.Copy(startValue),
					endValue = this.Copy(endValue),
					currentValue = this.Copy(startValue),
					reversingAdjustedStartValue = this.Copy(startValue)
				};
				float num2 = Mathf.Max(0f, duration) + delay;
				bool flag = !this.ConvertUnits(owner, prop, ref styleData.startValue, ref styleData.endValue);
				bool flag2;
				if (flag)
				{
					flag2 = false;
				}
				else
				{
					int num3;
					bool flag3 = this.completed.IndexOf(owner, prop, out num3);
					if (flag3)
					{
						bool flag4 = this.SameFunc(endValue, this.completed.style[num3]);
						if (flag4)
						{
							return false;
						}
						bool flag5 = num2 <= 0f;
						if (flag5)
						{
							return false;
						}
						this.completed.Remove(num3);
						IStylePropertyAnimations styleAnimation = owner.styleAnimation;
						int num4 = styleAnimation.completedAnimationCount;
						styleAnimation.completedAnimationCount = num4 - 1;
					}
					int num5;
					bool flag6 = this.running.IndexOf(owner, prop, out num5);
					if (flag6)
					{
						bool flag7 = this.SameFunc(endValue, this.running.style[num5].endValue);
						if (flag7)
						{
							flag2 = false;
						}
						else
						{
							bool flag8 = this.SameFunc(endValue, this.running.style[num5].currentValue);
							if (flag8)
							{
								this.QueueTransitionCancelEvent(owner, num5, currentTime);
								this.running.Remove(num5);
								IStylePropertyAnimations styleAnimation2 = owner.styleAnimation;
								int num4 = styleAnimation2.runningAnimationCount;
								styleAnimation2.runningAnimationCount = num4 - 1;
								flag2 = false;
							}
							else
							{
								bool flag9 = num2 <= 0f;
								if (flag9)
								{
									this.QueueTransitionCancelEvent(owner, num5, currentTime);
									this.running.Remove(num5);
									IStylePropertyAnimations styleAnimation3 = owner.styleAnimation;
									int num4 = styleAnimation3.runningAnimationCount;
									styleAnimation3.runningAnimationCount = num4 - 1;
									flag2 = false;
								}
								else
								{
									styleData.startValue = this.Copy(this.running.style[num5].currentValue);
									bool flag10 = !this.ConvertUnits(owner, prop, ref styleData.startValue, ref styleData.endValue);
									if (flag10)
									{
										this.QueueTransitionCancelEvent(owner, num5, currentTime);
										this.running.Remove(num5);
										IStylePropertyAnimations styleAnimation4 = owner.styleAnimation;
										int num4 = styleAnimation4.runningAnimationCount;
										styleAnimation4.runningAnimationCount = num4 - 1;
										flag2 = false;
									}
									else
									{
										styleData.currentValue = this.Copy(styleData.startValue);
										bool flag11 = this.SameFunc(endValue, this.running.style[num5].reversingAdjustedStartValue);
										if (flag11)
										{
											float num6 = (timingData.reversingShorteningFactor = this.ComputeReversingShorteningFactor(num5));
											timingData.startTime = currentTime + (double)this.ComputeReversingDelay(delay, num6);
											timingData.duration = this.ComputeReversingDuration(duration, num6);
											styleData.reversingAdjustedStartValue = this.Copy(this.running.style[num5].endValue);
										}
										this.running.timing[num5].isStarted = false;
										this.QueueTransitionCancelEvent(owner, num5, currentTime);
										this.QueueTransitionRunEvent(owner, num5);
										this.running.Replace(num5, timingData, styleData);
										flag2 = true;
									}
								}
							}
						}
					}
					else
					{
						bool flag12 = num2 <= 0f;
						if (flag12)
						{
							flag2 = false;
						}
						else
						{
							bool flag13 = this.SameFunc(startValue, endValue);
							if (flag13)
							{
								flag2 = false;
							}
							else
							{
								this.running.Add(owner, prop, timingData, styleData);
								IStylePropertyAnimations styleAnimation5 = owner.styleAnimation;
								int num4 = styleAnimation5.runningAnimationCount;
								styleAnimation5.runningAnimationCount = num4 + 1;
								this.QueueTransitionRunEvent(owner, this.running.count - 1);
								flag2 = true;
							}
						}
					}
				}
				return flag2;
			}

			private void ForceComputedStyleEndValue(int runningIndex)
			{
				ref StylePropertyAnimationSystem.Values<T>.StyleData ptr = ref this.running.style[runningIndex];
				ptr.currentValue = ptr.endValue;
				this.UpdateComputedStyle(runningIndex);
			}

			public sealed override void Update(double currentTime)
			{
				this.m_CurrentTime = currentTime;
				this.UpdateProgress(currentTime);
				this.UpdateValues();
				this.UpdateComputedStyle();
				bool flag = this.m_NextFrameEventsState.StateChanged();
				if (flag)
				{
					this.ProcessEventQueue();
				}
			}

			private void ProcessEventQueue()
			{
				this.SwapFrameStates();
				IPanel panel = this.m_CurrentFrameEventsState.panel;
				EventDispatcher eventDispatcher = ((panel != null) ? panel.dispatcher : null);
				using (new EventDispatcherGate(eventDispatcher))
				{
					foreach (KeyValuePair<StylePropertyAnimationSystem.ElementPropertyPair, Queue<EventBase>> keyValuePair in this.m_CurrentFrameEventsState.elementPropertyQueuedEvents)
					{
						StylePropertyAnimationSystem.ElementPropertyPair key = keyValuePair.Key;
						Queue<EventBase> value = keyValuePair.Value;
						VisualElement element = keyValuePair.Key.element;
						while (value.Count > 0)
						{
							EventBase eventBase = value.Dequeue();
							element.SendEvent(eventBase);
							eventBase.Dispose();
						}
					}
					this.m_CurrentFrameEventsState.Clear();
				}
			}

			private void UpdateProgress(double currentTime)
			{
				int num = this.running.count;
				bool flag = num > 0;
				if (flag)
				{
					for (int i = 0; i < num; i++)
					{
						ref StylePropertyAnimationSystem.Values<T>.TimingData ptr = ref this.running.timing[i];
						bool flag2 = currentTime < ptr.startTime;
						if (flag2)
						{
							ptr.easedProgress = 0f;
						}
						else
						{
							double num2 = ptr.startTime + (double)ptr.duration;
							bool flag3 = currentTime >= num2 || num2 - currentTime < 0.0001;
							if (flag3)
							{
								ref StylePropertyAnimationSystem.Values<T>.StyleData ptr2 = ref this.running.style[i];
								ref VisualElement ptr3 = ref this.running.elements[i];
								ptr2.currentValue = ptr2.endValue;
								this.UpdateComputedStyle(i);
								this.completed.Add(ptr3, this.running.properties[i], StylePropertyAnimationSystem.Values<T>.EmptyData.Default, ptr2.endValue);
								IStylePropertyAnimations styleAnimation = ptr3.styleAnimation;
								int num3 = styleAnimation.runningAnimationCount;
								styleAnimation.runningAnimationCount = num3 - 1;
								IStylePropertyAnimations styleAnimation2 = ptr3.styleAnimation;
								num3 = styleAnimation2.completedAnimationCount;
								styleAnimation2.completedAnimationCount = num3 + 1;
								this.QueueTransitionEndEvent(ptr3, i);
								this.running.Remove(i);
								i--;
								num--;
							}
							else
							{
								bool flag4 = !ptr.isStarted;
								if (flag4)
								{
									ptr.isStarted = true;
									this.QueueTransitionStartEvent(this.running.elements[i], i);
								}
								float num4 = (float)(currentTime - ptr.startTime) / ptr.duration;
								ptr.easedProgress = ptr.easingCurve(num4);
							}
						}
					}
				}
			}

			private double m_CurrentTime = 0.0;

			private StylePropertyAnimationSystem.Values<T>.TransitionEventsFrameState m_CurrentFrameEventsState = new StylePropertyAnimationSystem.Values<T>.TransitionEventsFrameState();

			private StylePropertyAnimationSystem.Values<T>.TransitionEventsFrameState m_NextFrameEventsState = new StylePropertyAnimationSystem.Values<T>.TransitionEventsFrameState();

			public StylePropertyAnimationSystem.AnimationDataSet<StylePropertyAnimationSystem.Values<T>.TimingData, StylePropertyAnimationSystem.Values<T>.StyleData> running;

			public StylePropertyAnimationSystem.AnimationDataSet<StylePropertyAnimationSystem.Values<T>.EmptyData, T> completed;

			private class TransitionEventsFrameState
			{
				public static Queue<EventBase> GetPooledQueue()
				{
					return StylePropertyAnimationSystem.Values<T>.TransitionEventsFrameState.k_EventQueuePool.Get();
				}

				public void RegisterChange()
				{
					this.m_ChangesCount++;
				}

				public void UnregisterChange()
				{
					this.m_ChangesCount--;
				}

				public bool StateChanged()
				{
					return this.m_ChangesCount > 0;
				}

				public void Clear()
				{
					foreach (KeyValuePair<StylePropertyAnimationSystem.ElementPropertyPair, Queue<EventBase>> keyValuePair in this.elementPropertyQueuedEvents)
					{
						keyValuePair.Value.Clear();
						StylePropertyAnimationSystem.Values<T>.TransitionEventsFrameState.k_EventQueuePool.Release(keyValuePair.Value);
					}
					this.elementPropertyQueuedEvents.Clear();
					this.elementPropertyStateDelta.Clear();
					this.panel = null;
					this.m_ChangesCount = 0;
				}

				private static readonly ObjectPool<Queue<EventBase>> k_EventQueuePool = new ObjectPool<Queue<EventBase>>(() => new Queue<EventBase>(4), null, null, null, true, 10, 10000);

				public readonly Dictionary<StylePropertyAnimationSystem.ElementPropertyPair, StylePropertyAnimationSystem.TransitionState> elementPropertyStateDelta = new Dictionary<StylePropertyAnimationSystem.ElementPropertyPair, StylePropertyAnimationSystem.TransitionState>(StylePropertyAnimationSystem.ElementPropertyPair.Comparer);

				public readonly Dictionary<StylePropertyAnimationSystem.ElementPropertyPair, Queue<EventBase>> elementPropertyQueuedEvents = new Dictionary<StylePropertyAnimationSystem.ElementPropertyPair, Queue<EventBase>>(StylePropertyAnimationSystem.ElementPropertyPair.Comparer);

				public IPanel panel;

				private int m_ChangesCount;
			}

			public struct TimingData
			{
				public double startTime;

				public float duration;

				public Func<float, float> easingCurve;

				public float easedProgress;

				public float reversingShorteningFactor;

				public bool isStarted;

				public float delay;
			}

			public struct StyleData
			{
				public T startValue;

				public T endValue;

				public T reversingAdjustedStartValue;

				public T currentValue;
			}

			public struct EmptyData
			{
				public static StylePropertyAnimationSystem.Values<T>.EmptyData Default = default(StylePropertyAnimationSystem.Values<T>.EmptyData);
			}
		}

		private class ValuesFloat : StylePropertyAnimationSystem.Values<float>
		{
			public override Func<float, float, bool> SameFunc { get; } = new Func<float, float, bool>(StylePropertyAnimationSystem.ValuesFloat.IsSame);

			private static bool IsSame(float a, float b)
			{
				return Mathf.Approximately(a, b);
			}

			private static void Lerp(float a, float b, ref float result, float t)
			{
				result = Mathf.LerpUnclamped(a, b, t);
			}

			protected sealed override void UpdateValues()
			{
				int count = this.running.count;
				for (int i = 0; i < count; i++)
				{
					ref StylePropertyAnimationSystem.Values<float>.TimingData ptr = ref this.running.timing[i];
					ref StylePropertyAnimationSystem.Values<float>.StyleData ptr2 = ref this.running.style[i];
					StylePropertyAnimationSystem.ValuesFloat.Lerp(ptr2.startValue, ptr2.endValue, ref ptr2.currentValue, ptr.easedProgress);
				}
			}

			protected sealed override void UpdateComputedStyle()
			{
				int count = this.running.count;
				for (int i = 0; i < count; i++)
				{
					this.running.elements[i].computedStyle.ApplyPropertyAnimation(this.running.elements[i], this.running.properties[i], this.running.style[i].currentValue);
				}
			}

			protected sealed override void UpdateComputedStyle(int i)
			{
				this.running.elements[i].computedStyle.ApplyPropertyAnimation(this.running.elements[i], this.running.properties[i], this.running.style[i].currentValue);
			}
		}

		private class ValuesInt : StylePropertyAnimationSystem.Values<int>
		{
			public override Func<int, int, bool> SameFunc { get; } = new Func<int, int, bool>(StylePropertyAnimationSystem.ValuesInt.IsSame);

			private static bool IsSame(int a, int b)
			{
				return a == b;
			}

			private static int Lerp(int a, int b, float t)
			{
				return Mathf.RoundToInt(Mathf.LerpUnclamped((float)a, (float)b, t));
			}

			protected sealed override void UpdateValues()
			{
				int count = this.running.count;
				for (int i = 0; i < count; i++)
				{
					ref StylePropertyAnimationSystem.Values<int>.TimingData ptr = ref this.running.timing[i];
					ref StylePropertyAnimationSystem.Values<int>.StyleData ptr2 = ref this.running.style[i];
					ptr2.currentValue = StylePropertyAnimationSystem.ValuesInt.Lerp(ptr2.startValue, ptr2.endValue, ptr.easedProgress);
				}
			}

			protected sealed override void UpdateComputedStyle()
			{
				int count = this.running.count;
				for (int i = 0; i < count; i++)
				{
					this.running.elements[i].computedStyle.ApplyPropertyAnimation(this.running.elements[i], this.running.properties[i], this.running.style[i].currentValue);
				}
			}

			protected sealed override void UpdateComputedStyle(int i)
			{
				this.running.elements[i].computedStyle.ApplyPropertyAnimation(this.running.elements[i], this.running.properties[i], this.running.style[i].currentValue);
			}
		}

		private class ValuesLength : StylePropertyAnimationSystem.Values<Length>
		{
			public override Func<Length, Length, bool> SameFunc { get; } = new Func<Length, Length, bool>(StylePropertyAnimationSystem.ValuesLength.IsSame);

			private static bool IsSame(Length a, Length b)
			{
				return a.unit == b.unit && Mathf.Approximately(a.value, b.value);
			}

			protected sealed override bool ConvertUnits(VisualElement owner, StylePropertyId prop, ref Length a, ref Length b)
			{
				return owner.TryConvertLengthUnits(prop, ref a, ref b, 0);
			}

			internal static Length Lerp(Length a, Length b, float t)
			{
				return new Length(Mathf.LerpUnclamped(a.value, b.value, t), b.unit);
			}

			protected sealed override void UpdateValues()
			{
				int count = this.running.count;
				for (int i = 0; i < count; i++)
				{
					ref StylePropertyAnimationSystem.Values<Length>.TimingData ptr = ref this.running.timing[i];
					ref StylePropertyAnimationSystem.Values<Length>.StyleData ptr2 = ref this.running.style[i];
					ptr2.currentValue = StylePropertyAnimationSystem.ValuesLength.Lerp(ptr2.startValue, ptr2.endValue, ptr.easedProgress);
				}
			}

			protected sealed override void UpdateComputedStyle()
			{
				int count = this.running.count;
				for (int i = 0; i < count; i++)
				{
					this.running.elements[i].computedStyle.ApplyPropertyAnimation(this.running.elements[i], this.running.properties[i], this.running.style[i].currentValue);
				}
			}

			protected sealed override void UpdateComputedStyle(int i)
			{
				this.running.elements[i].computedStyle.ApplyPropertyAnimation(this.running.elements[i], this.running.properties[i], this.running.style[i].currentValue);
			}
		}

		private class ValuesColor : StylePropertyAnimationSystem.Values<Color>
		{
			public override Func<Color, Color, bool> SameFunc { get; } = new Func<Color, Color, bool>(StylePropertyAnimationSystem.ValuesColor.IsSame);

			private static bool IsSame(Color c, Color d)
			{
				return Mathf.Approximately(c.r, d.r) && Mathf.Approximately(c.g, d.g) && Mathf.Approximately(c.b, d.b) && Mathf.Approximately(c.a, d.a);
			}

			private static Color Lerp(Color a, Color b, float t)
			{
				return Color.LerpUnclamped(a, b, t);
			}

			protected sealed override void UpdateValues()
			{
				int count = this.running.count;
				for (int i = 0; i < count; i++)
				{
					ref StylePropertyAnimationSystem.Values<Color>.TimingData ptr = ref this.running.timing[i];
					ref StylePropertyAnimationSystem.Values<Color>.StyleData ptr2 = ref this.running.style[i];
					ptr2.currentValue = StylePropertyAnimationSystem.ValuesColor.Lerp(ptr2.startValue, ptr2.endValue, ptr.easedProgress);
				}
			}

			protected sealed override void UpdateComputedStyle()
			{
				int count = this.running.count;
				for (int i = 0; i < count; i++)
				{
					this.running.elements[i].computedStyle.ApplyPropertyAnimation(this.running.elements[i], this.running.properties[i], this.running.style[i].currentValue);
				}
			}

			protected sealed override void UpdateComputedStyle(int i)
			{
				this.running.elements[i].computedStyle.ApplyPropertyAnimation(this.running.elements[i], this.running.properties[i], this.running.style[i].currentValue);
			}
		}

		private abstract class ValuesDiscrete<T> : StylePropertyAnimationSystem.Values<T>
		{
			public override Func<T, T, bool> SameFunc { get; } = new Func<T, T, bool>(StylePropertyAnimationSystem.ValuesDiscrete<T>.IsSame);

			private static bool IsSame(T a, T b)
			{
				return EqualityComparer<T>.Default.Equals(a, b);
			}

			private static T Lerp(T a, T b, float t)
			{
				return (t < 0.5f) ? a : b;
			}

			protected sealed override void UpdateValues()
			{
				int count = this.running.count;
				for (int i = 0; i < count; i++)
				{
					ref StylePropertyAnimationSystem.Values<T>.TimingData ptr = ref this.running.timing[i];
					ref StylePropertyAnimationSystem.Values<T>.StyleData ptr2 = ref this.running.style[i];
					ptr2.currentValue = StylePropertyAnimationSystem.ValuesDiscrete<T>.Lerp(ptr2.startValue, ptr2.endValue, ptr.easedProgress);
				}
			}
		}

		private class ValuesEnum : StylePropertyAnimationSystem.ValuesDiscrete<int>
		{
			protected sealed override void UpdateComputedStyle()
			{
				int count = this.running.count;
				for (int i = 0; i < count; i++)
				{
					this.running.elements[i].computedStyle.ApplyPropertyAnimation(this.running.elements[i], this.running.properties[i], this.running.style[i].currentValue);
				}
			}

			protected sealed override void UpdateComputedStyle(int i)
			{
				this.running.elements[i].computedStyle.ApplyPropertyAnimation(this.running.elements[i], this.running.properties[i], this.running.style[i].currentValue);
			}
		}

		private class ValuesBackground : StylePropertyAnimationSystem.ValuesDiscrete<Background>
		{
			protected sealed override void UpdateComputedStyle()
			{
				int count = this.running.count;
				for (int i = 0; i < count; i++)
				{
					this.running.elements[i].computedStyle.ApplyPropertyAnimation(this.running.elements[i], this.running.properties[i], this.running.style[i].currentValue);
				}
			}

			protected sealed override void UpdateComputedStyle(int i)
			{
				this.running.elements[i].computedStyle.ApplyPropertyAnimation(this.running.elements[i], this.running.properties[i], this.running.style[i].currentValue);
			}
		}

		private class ValuesFontDefinition : StylePropertyAnimationSystem.ValuesDiscrete<FontDefinition>
		{
			protected sealed override void UpdateComputedStyle()
			{
				int count = this.running.count;
				for (int i = 0; i < count; i++)
				{
					this.running.elements[i].computedStyle.ApplyPropertyAnimation(this.running.elements[i], this.running.properties[i], this.running.style[i].currentValue);
				}
			}

			protected sealed override void UpdateComputedStyle(int i)
			{
				this.running.elements[i].computedStyle.ApplyPropertyAnimation(this.running.elements[i], this.running.properties[i], this.running.style[i].currentValue);
			}
		}

		private class ValuesFont : StylePropertyAnimationSystem.ValuesDiscrete<Font>
		{
			protected sealed override void UpdateComputedStyle()
			{
				int count = this.running.count;
				for (int i = 0; i < count; i++)
				{
					this.running.elements[i].computedStyle.ApplyPropertyAnimation(this.running.elements[i], this.running.properties[i], this.running.style[i].currentValue);
				}
			}

			protected sealed override void UpdateComputedStyle(int i)
			{
				this.running.elements[i].computedStyle.ApplyPropertyAnimation(this.running.elements[i], this.running.properties[i], this.running.style[i].currentValue);
			}
		}

		private class ValuesTextShadow : StylePropertyAnimationSystem.Values<TextShadow>
		{
			public override Func<TextShadow, TextShadow, bool> SameFunc { get; } = new Func<TextShadow, TextShadow, bool>(StylePropertyAnimationSystem.ValuesTextShadow.IsSame);

			private static bool IsSame(TextShadow a, TextShadow b)
			{
				return a == b;
			}

			private static TextShadow Lerp(TextShadow a, TextShadow b, float t)
			{
				return TextShadow.LerpUnclamped(a, b, t);
			}

			protected sealed override void UpdateValues()
			{
				int count = this.running.count;
				for (int i = 0; i < count; i++)
				{
					ref StylePropertyAnimationSystem.Values<TextShadow>.TimingData ptr = ref this.running.timing[i];
					ref StylePropertyAnimationSystem.Values<TextShadow>.StyleData ptr2 = ref this.running.style[i];
					ptr2.currentValue = StylePropertyAnimationSystem.ValuesTextShadow.Lerp(ptr2.startValue, ptr2.endValue, ptr.easedProgress);
				}
			}

			protected sealed override void UpdateComputedStyle()
			{
				int count = this.running.count;
				for (int i = 0; i < count; i++)
				{
					this.running.elements[i].computedStyle.ApplyPropertyAnimation(this.running.elements[i], this.running.properties[i], this.running.style[i].currentValue);
				}
			}

			protected sealed override void UpdateComputedStyle(int i)
			{
				this.running.elements[i].computedStyle.ApplyPropertyAnimation(this.running.elements[i], this.running.properties[i], this.running.style[i].currentValue);
			}
		}

		private class ValuesScale : StylePropertyAnimationSystem.Values<Scale>
		{
			public override Func<Scale, Scale, bool> SameFunc { get; } = new Func<Scale, Scale, bool>(StylePropertyAnimationSystem.ValuesScale.IsSame);

			private static bool IsSame(Scale a, Scale b)
			{
				return a == b;
			}

			protected sealed override void UpdateComputedStyle()
			{
				int count = this.running.count;
				for (int i = 0; i < count; i++)
				{
					this.running.elements[i].computedStyle.ApplyPropertyAnimation(this.running.elements[i], this.running.properties[i], this.running.style[i].currentValue);
				}
			}

			protected sealed override void UpdateComputedStyle(int i)
			{
				this.running.elements[i].computedStyle.ApplyPropertyAnimation(this.running.elements[i], this.running.properties[i], this.running.style[i].currentValue);
			}

			private static Scale Lerp(Scale a, Scale b, float t)
			{
				return new Scale(Vector3.LerpUnclamped(a.value, b.value, t));
			}

			protected sealed override void UpdateValues()
			{
				int count = this.running.count;
				for (int i = 0; i < count; i++)
				{
					ref StylePropertyAnimationSystem.Values<Scale>.TimingData ptr = ref this.running.timing[i];
					ref StylePropertyAnimationSystem.Values<Scale>.StyleData ptr2 = ref this.running.style[i];
					ptr2.currentValue = StylePropertyAnimationSystem.ValuesScale.Lerp(ptr2.startValue, ptr2.endValue, ptr.easedProgress);
				}
			}
		}

		private class ValuesRotate : StylePropertyAnimationSystem.Values<Rotate>
		{
			public override Func<Rotate, Rotate, bool> SameFunc { get; } = new Func<Rotate, Rotate, bool>(StylePropertyAnimationSystem.ValuesRotate.IsSame);

			private static bool IsSame(Rotate a, Rotate b)
			{
				return a == b;
			}

			protected sealed override void UpdateComputedStyle()
			{
				int count = this.running.count;
				for (int i = 0; i < count; i++)
				{
					this.running.elements[i].computedStyle.ApplyPropertyAnimation(this.running.elements[i], this.running.properties[i], this.running.style[i].currentValue);
				}
			}

			protected sealed override void UpdateComputedStyle(int i)
			{
				this.running.elements[i].computedStyle.ApplyPropertyAnimation(this.running.elements[i], this.running.properties[i], this.running.style[i].currentValue);
			}

			private static Rotate Lerp(Rotate a, Rotate b, float t)
			{
				return new Rotate(Mathf.LerpUnclamped(a.angle.ToDegrees(), b.angle.ToDegrees(), t), b.axis);
			}

			protected sealed override void UpdateValues()
			{
				int count = this.running.count;
				for (int i = 0; i < count; i++)
				{
					ref StylePropertyAnimationSystem.Values<Rotate>.TimingData ptr = ref this.running.timing[i];
					ref StylePropertyAnimationSystem.Values<Rotate>.StyleData ptr2 = ref this.running.style[i];
					ptr2.currentValue = StylePropertyAnimationSystem.ValuesRotate.Lerp(ptr2.startValue, ptr2.endValue, ptr.easedProgress);
				}
			}
		}

		private class ValuesRatio : StylePropertyAnimationSystem.Values<Ratio>
		{
			public override Func<Ratio, Ratio, bool> SameFunc { get; } = new Func<Ratio, Ratio, bool>(StylePropertyAnimationSystem.ValuesRatio.IsSame);

			private static bool IsSame(Ratio a, Ratio b)
			{
				return a == b;
			}

			protected sealed override bool ConvertUnits(VisualElement owner, StylePropertyId prop, ref Ratio a, ref Ratio b)
			{
				bool flag = b.IsAuto();
				bool flag2;
				if (flag)
				{
					flag2 = false;
				}
				else
				{
					bool flag3 = a.IsAuto();
					if (flag3)
					{
						bool flag4 = owner.resolvedStyle.height == 0f || float.IsNaN(owner.resolvedStyle.height);
						if (flag4)
						{
							flag2 = false;
						}
						else
						{
							a = new Ratio(owner.resolvedStyle.width / owner.resolvedStyle.height);
							flag2 = true;
						}
					}
					else
					{
						flag2 = true;
					}
				}
				return flag2;
			}

			protected sealed override void UpdateComputedStyle()
			{
				int count = this.running.count;
				for (int i = 0; i < count; i++)
				{
					this.running.elements[i].computedStyle.ApplyPropertyAnimation(this.running.elements[i], this.running.properties[i], this.running.style[i].currentValue);
				}
			}

			protected sealed override void UpdateComputedStyle(int i)
			{
				this.running.elements[i].computedStyle.ApplyPropertyAnimation(this.running.elements[i], this.running.properties[i], this.running.style[i].currentValue);
			}

			private static Ratio Lerp(Ratio a, Ratio b, float t)
			{
				return new Ratio(Mathf.LerpUnclamped(a.value, b.value, t));
			}

			protected sealed override void UpdateValues()
			{
				int count = this.running.count;
				for (int i = 0; i < count; i++)
				{
					ref StylePropertyAnimationSystem.Values<Ratio>.TimingData ptr = ref this.running.timing[i];
					ref StylePropertyAnimationSystem.Values<Ratio>.StyleData ptr2 = ref this.running.style[i];
					ptr2.currentValue = StylePropertyAnimationSystem.ValuesRatio.Lerp(ptr2.startValue, ptr2.endValue, ptr.easedProgress);
				}
			}
		}

		private class ValuesTranslate : StylePropertyAnimationSystem.Values<Translate>
		{
			public override Func<Translate, Translate, bool> SameFunc { get; } = new Func<Translate, Translate, bool>(StylePropertyAnimationSystem.ValuesTranslate.IsSame);

			private static bool IsSame(Translate a, Translate b)
			{
				return a == b;
			}

			protected sealed override bool ConvertUnits(VisualElement owner, StylePropertyId prop, ref Translate a, ref Translate b)
			{
				return owner.TryConvertTranslateUnits(ref a, ref b);
			}

			protected sealed override void UpdateComputedStyle()
			{
				int count = this.running.count;
				for (int i = 0; i < count; i++)
				{
					this.running.elements[i].computedStyle.ApplyPropertyAnimation(this.running.elements[i], this.running.properties[i], this.running.style[i].currentValue);
				}
			}

			protected sealed override void UpdateComputedStyle(int i)
			{
				this.running.elements[i].computedStyle.ApplyPropertyAnimation(this.running.elements[i], this.running.properties[i], this.running.style[i].currentValue);
			}

			private static Translate Lerp(Translate a, Translate b, float t)
			{
				return new Translate(StylePropertyAnimationSystem.ValuesLength.Lerp(a.x, b.x, t), StylePropertyAnimationSystem.ValuesLength.Lerp(a.y, b.y, t), Mathf.Lerp(a.z, b.z, t));
			}

			protected sealed override void UpdateValues()
			{
				int count = this.running.count;
				for (int i = 0; i < count; i++)
				{
					ref StylePropertyAnimationSystem.Values<Translate>.TimingData ptr = ref this.running.timing[i];
					ref StylePropertyAnimationSystem.Values<Translate>.StyleData ptr2 = ref this.running.style[i];
					ptr2.currentValue = StylePropertyAnimationSystem.ValuesTranslate.Lerp(ptr2.startValue, ptr2.endValue, ptr.easedProgress);
				}
			}
		}

		private class ValuesTransformOrigin : StylePropertyAnimationSystem.Values<TransformOrigin>
		{
			public override Func<TransformOrigin, TransformOrigin, bool> SameFunc { get; } = new Func<TransformOrigin, TransformOrigin, bool>(StylePropertyAnimationSystem.ValuesTransformOrigin.IsSame);

			private static bool IsSame(TransformOrigin a, TransformOrigin b)
			{
				return a == b;
			}

			protected sealed override bool ConvertUnits(VisualElement owner, StylePropertyId prop, ref TransformOrigin a, ref TransformOrigin b)
			{
				return owner.TryConvertTransformOriginUnits(ref a, ref b);
			}

			protected sealed override void UpdateComputedStyle()
			{
				int count = this.running.count;
				for (int i = 0; i < count; i++)
				{
					this.running.elements[i].computedStyle.ApplyPropertyAnimation(this.running.elements[i], this.running.properties[i], this.running.style[i].currentValue);
				}
			}

			protected sealed override void UpdateComputedStyle(int i)
			{
				this.running.elements[i].computedStyle.ApplyPropertyAnimation(this.running.elements[i], this.running.properties[i], this.running.style[i].currentValue);
			}

			private static TransformOrigin Lerp(TransformOrigin a, TransformOrigin b, float t)
			{
				return new TransformOrigin(StylePropertyAnimationSystem.ValuesLength.Lerp(a.x, b.x, t), StylePropertyAnimationSystem.ValuesLength.Lerp(a.y, b.y, t), Mathf.Lerp(a.z, b.z, t));
			}

			protected sealed override void UpdateValues()
			{
				int count = this.running.count;
				for (int i = 0; i < count; i++)
				{
					ref StylePropertyAnimationSystem.Values<TransformOrigin>.TimingData ptr = ref this.running.timing[i];
					ref StylePropertyAnimationSystem.Values<TransformOrigin>.StyleData ptr2 = ref this.running.style[i];
					ptr2.currentValue = StylePropertyAnimationSystem.ValuesTransformOrigin.Lerp(ptr2.startValue, ptr2.endValue, ptr.easedProgress);
				}
			}
		}

		private class ValuesBackgroundPosition : StylePropertyAnimationSystem.ValuesDiscrete<BackgroundPosition>
		{
			protected sealed override void UpdateComputedStyle()
			{
				int count = this.running.count;
				for (int i = 0; i < count; i++)
				{
					this.running.elements[i].computedStyle.ApplyPropertyAnimation(this.running.elements[i], this.running.properties[i], this.running.style[i].currentValue);
				}
			}

			protected sealed override void UpdateComputedStyle(int i)
			{
				this.running.elements[i].computedStyle.ApplyPropertyAnimation(this.running.elements[i], this.running.properties[i], this.running.style[i].currentValue);
			}
		}

		private class ValuesBackgroundRepeat : StylePropertyAnimationSystem.ValuesDiscrete<BackgroundRepeat>
		{
			protected sealed override void UpdateComputedStyle()
			{
				int count = this.running.count;
				for (int i = 0; i < count; i++)
				{
					this.running.elements[i].computedStyle.ApplyPropertyAnimation(this.running.elements[i], this.running.properties[i], this.running.style[i].currentValue);
				}
			}

			protected sealed override void UpdateComputedStyle(int i)
			{
				this.running.elements[i].computedStyle.ApplyPropertyAnimation(this.running.elements[i], this.running.properties[i], this.running.style[i].currentValue);
			}
		}

		private class ValuesBackgroundSize : StylePropertyAnimationSystem.Values<BackgroundSize>
		{
			public override Func<BackgroundSize, BackgroundSize, bool> SameFunc { get; } = new Func<BackgroundSize, BackgroundSize, bool>(StylePropertyAnimationSystem.ValuesBackgroundSize.IsSame);

			private static bool IsSame(BackgroundSize a, BackgroundSize b)
			{
				return a == b;
			}

			protected sealed override bool ConvertUnits(VisualElement owner, StylePropertyId prop, ref BackgroundSize a, ref BackgroundSize b)
			{
				return owner.TryConvertBackgroundSizeUnits(ref a, ref b);
			}

			protected sealed override void UpdateComputedStyle()
			{
				int count = this.running.count;
				for (int i = 0; i < count; i++)
				{
					this.running.elements[i].computedStyle.ApplyPropertyAnimation(this.running.elements[i], this.running.properties[i], this.running.style[i].currentValue);
				}
			}

			protected sealed override void UpdateComputedStyle(int i)
			{
				this.running.elements[i].computedStyle.ApplyPropertyAnimation(this.running.elements[i], this.running.properties[i], this.running.style[i].currentValue);
			}

			private static BackgroundSize Lerp(BackgroundSize a, BackgroundSize b, float t)
			{
				return new BackgroundSize(StylePropertyAnimationSystem.ValuesLength.Lerp(a.x, b.x, t), StylePropertyAnimationSystem.ValuesLength.Lerp(a.y, b.y, t));
			}

			protected sealed override void UpdateValues()
			{
				int count = this.running.count;
				for (int i = 0; i < count; i++)
				{
					ref StylePropertyAnimationSystem.Values<BackgroundSize>.TimingData ptr = ref this.running.timing[i];
					ref StylePropertyAnimationSystem.Values<BackgroundSize>.StyleData ptr2 = ref this.running.style[i];
					ptr2.currentValue = StylePropertyAnimationSystem.ValuesBackgroundSize.Lerp(ptr2.startValue, ptr2.endValue, ptr.easedProgress);
				}
			}
		}

		private class ValuesListFilterFunction : StylePropertyAnimationSystem.Values<List<FilterFunction>>
		{
			protected override List<FilterFunction> Copy(List<FilterFunction> value)
			{
				return new List<FilterFunction>(value);
			}

			public override Func<List<FilterFunction>, List<FilterFunction>, bool> SameFunc { get; } = new Func<List<FilterFunction>, List<FilterFunction>, bool>(StylePropertyAnimationSystem.ValuesListFilterFunction.IsSame);

			private static bool IsSame(List<FilterFunction> a, List<FilterFunction> b)
			{
				bool flag = a.Count != b.Count;
				bool flag2;
				if (flag)
				{
					flag2 = false;
				}
				else
				{
					for (int i = 0; i < a.Count; i++)
					{
						bool flag3 = a[i] != b[i];
						if (flag3)
						{
							return false;
						}
					}
					flag2 = true;
				}
				return flag2;
			}

			protected sealed override bool ConvertUnits(VisualElement owner, StylePropertyId prop, ref List<FilterFunction> a, ref List<FilterFunction> b)
			{
				int num = Math.Min(a.Count, b.Count);
				int i = 0;
				while (i < num)
				{
					bool flag = a[i].type != b[i].type;
					bool flag2;
					if (flag)
					{
						flag2 = false;
					}
					else
					{
						bool flag3 = a[i].type == FilterFunctionType.Custom && !StylePropertyAnimationSystem.ValuesListFilterFunction.AreFilterDefinitionsCompatible(a[i].customDefinition, b[i].customDefinition);
						if (!flag3)
						{
							i++;
							continue;
						}
						flag2 = false;
					}
					return flag2;
				}
				return true;
			}

			private static bool AreFilterDefinitionsCompatible(FilterFunctionDefinition filterDef1, FilterFunctionDefinition filterDef2)
			{
				bool flag = filterDef1 == null || filterDef2 == null;
				bool flag2;
				if (flag)
				{
					flag2 = false;
				}
				else
				{
					bool flag3 = filterDef1 == filterDef2;
					flag2 = flag3;
				}
				return flag2;
			}

			protected sealed override void UpdateComputedStyle()
			{
				int count = this.running.count;
				for (int i = 0; i < count; i++)
				{
					this.running.elements[i].computedStyle.ApplyPropertyAnimation(this.running.elements[i], this.running.properties[i], this.running.style[i].currentValue);
				}
			}

			protected sealed override void UpdateComputedStyle(int i)
			{
				this.running.elements[i].computedStyle.ApplyPropertyAnimation(this.running.elements[i], this.running.properties[i], this.running.style[i].currentValue);
			}

			private static FilterParameter LerpFilterParameters(FilterParameter a, FilterParameter b, float t)
			{
				bool flag = a.type != b.type;
				FilterParameter filterParameter;
				if (flag)
				{
					filterParameter = a;
				}
				else
				{
					FilterParameterType type = a.type;
					FilterParameterType filterParameterType = type;
					if (filterParameterType != FilterParameterType.Float)
					{
						if (filterParameterType != FilterParameterType.Color)
						{
							filterParameter = a;
						}
						else
						{
							filterParameter = new FilterParameter
							{
								type = FilterParameterType.Color,
								colorValue = Color.Lerp(a.colorValue, b.colorValue, t)
							};
						}
					}
					else
					{
						filterParameter = new FilterParameter
						{
							type = FilterParameterType.Float,
							floatValue = Mathf.Lerp(a.floatValue, b.floatValue, t)
						};
					}
				}
				return filterParameter;
			}

			private unsafe static void Lerp(List<FilterFunction> a, List<FilterFunction> b, ref List<FilterFunction> result, float t)
			{
				result.Clear();
				int num = ((a.Count >= b.Count) ? a.Count : b.Count);
				for (int i = 0; i < num; i++)
				{
					FilterFunction functionOrDefault = StylePropertyAnimationSystem.ValuesListFilterFunction.GetFunctionOrDefault(ref a, ref b, i);
					FilterFunction functionOrDefault2 = StylePropertyAnimationSystem.ValuesListFilterFunction.GetFunctionOrDefault(ref b, ref a, i);
					FilterFunction filterFunction = new FilterFunction
					{
						type = functionOrDefault.type,
						customDefinition = functionOrDefault.customDefinition
					};
					for (int j = 0; j < functionOrDefault.parameterCount; j++)
					{
						filterFunction.AddParameter(StylePropertyAnimationSystem.ValuesListFilterFunction.LerpFilterParameters(*functionOrDefault.parameters[j], *functionOrDefault2.parameters[j], t));
					}
					result.Add(filterFunction);
				}
			}

			private static FilterFunction GetFunctionOrDefault(ref List<FilterFunction> srcList, ref List<FilterFunction> refList, int index)
			{
				bool flag = index < srcList.Count;
				FilterFunction filterFunction;
				if (flag)
				{
					filterFunction = srcList[index];
				}
				else
				{
					FilterFunction filterFunction2 = refList[index];
					int parameterCount = filterFunction2.parameterCount;
					filterFunction2.ClearParameters();
					for (int i = 0; i < parameterCount; i++)
					{
						FilterParameter filterParameter = default(FilterParameter);
						FilterParameterDeclaration[] parameters = filterFunction2.GetDefinition().parameters;
						bool flag2 = i < parameters.Length;
						if (flag2)
						{
							filterParameter = parameters[i].interpolationDefaultValue;
						}
						filterFunction2.AddParameter(filterParameter);
					}
					filterFunction = filterFunction2;
				}
				return filterFunction;
			}

			protected sealed override void UpdateValues()
			{
				int count = this.running.count;
				for (int i = 0; i < count; i++)
				{
					ref StylePropertyAnimationSystem.Values<List<FilterFunction>>.TimingData ptr = ref this.running.timing[i];
					ref StylePropertyAnimationSystem.Values<List<FilterFunction>>.StyleData ptr2 = ref this.running.style[i];
					StylePropertyAnimationSystem.ValuesListFilterFunction.Lerp(ptr2.startValue, ptr2.endValue, ref ptr2.currentValue, ptr.easedProgress);
				}
			}
		}

		private class ValuesMaterialDefinition : StylePropertyAnimationSystem.Values<MaterialDefinition>
		{
			protected override MaterialDefinition Copy(MaterialDefinition value)
			{
				return new MaterialDefinition(value);
			}

			public override Func<MaterialDefinition, MaterialDefinition, bool> SameFunc { get; } = new Func<MaterialDefinition, MaterialDefinition, bool>(StylePropertyAnimationSystem.ValuesMaterialDefinition.IsSame);

			private static bool IsSame(MaterialDefinition a, MaterialDefinition b)
			{
				bool flag = a.material != b.material;
				bool flag2;
				if (flag)
				{
					flag2 = false;
				}
				else
				{
					List<MaterialPropertyValue> propertyValues = a.propertyValues;
					List<MaterialPropertyValue> propertyValues2 = b.propertyValues;
					int? num = ((propertyValues != null) ? new int?(propertyValues.Count) : null);
					int? num2 = ((propertyValues2 != null) ? new int?(propertyValues2.Count) : null);
					bool flag3 = !((num.GetValueOrDefault() == num2.GetValueOrDefault()) & (num != null == (num2 != null)));
					if (flag3)
					{
						flag2 = false;
					}
					else
					{
						int num3 = 0;
						for (;;)
						{
							int num4 = num3;
							num2 = ((propertyValues != null) ? new int?(propertyValues.Count) : null);
							if (!((num4 < num2.GetValueOrDefault()) & (num2 != null)))
							{
								goto Block_7;
							}
							bool flag4 = propertyValues[num3] != propertyValues2[num3];
							if (flag4)
							{
								break;
							}
							num3++;
						}
						return false;
						Block_7:
						flag2 = true;
					}
				}
				return flag2;
			}

			protected sealed override bool ConvertUnits(VisualElement owner, StylePropertyId prop, ref MaterialDefinition a, ref MaterialDefinition b)
			{
				bool flag = (a.material == null && b.material != null) || (b.material == null && a.material != null);
				bool flag2;
				if (flag)
				{
					flag2 = true;
				}
				else
				{
					bool flag3 = a.material != b.material;
					if (flag3)
					{
						flag2 = false;
					}
					else
					{
						List<MaterialPropertyValue> propertyValues = a.propertyValues;
						List<MaterialPropertyValue> propertyValues2 = b.propertyValues;
						int num = Math.Min((propertyValues != null) ? propertyValues.Count : 0, (propertyValues2 != null) ? propertyValues2.Count : 0);
						for (int i = 0; i < num; i++)
						{
							bool flag4 = propertyValues[i].type != propertyValues2[i].type || propertyValues[i].name != propertyValues2[i].name;
							if (flag4)
							{
								return false;
							}
						}
						flag2 = true;
					}
				}
				return flag2;
			}

			protected sealed override void UpdateComputedStyle()
			{
				int count = this.running.count;
				for (int i = 0; i < count; i++)
				{
					this.running.elements[i].computedStyle.ApplyPropertyAnimation(this.running.elements[i], this.running.properties[i], this.running.style[i].currentValue);
				}
			}

			protected sealed override void UpdateComputedStyle(int i)
			{
				this.running.elements[i].computedStyle.ApplyPropertyAnimation(this.running.elements[i], this.running.properties[i], this.running.style[i].currentValue);
			}

			private static MaterialPropertyValue LerpPropertyValues(MaterialPropertyValue a, MaterialPropertyValue b, float t)
			{
				bool flag = a.type != b.type;
				MaterialPropertyValue materialPropertyValue;
				if (flag)
				{
					materialPropertyValue = a;
				}
				else
				{
					MaterialPropertyValueType type = a.type;
					MaterialPropertyValueType materialPropertyValueType = type;
					if (materialPropertyValueType > MaterialPropertyValueType.Color)
					{
						if (materialPropertyValueType != MaterialPropertyValueType.Texture)
						{
							materialPropertyValue = a;
						}
						else
						{
							materialPropertyValue = ((t < 0.5f) ? a : b);
						}
					}
					else
					{
						materialPropertyValue = new MaterialPropertyValue
						{
							type = a.type,
							name = a.name,
							packedValue = Vector4.Lerp(a.packedValue, b.packedValue, t)
						};
					}
				}
				return materialPropertyValue;
			}

			private static MaterialPropertyValue GetValueOrDefault(List<MaterialPropertyValue> srcList, List<MaterialPropertyValue> refList, int index)
			{
				int? num = ((srcList != null) ? new int?(srcList.Count) : null);
				bool flag = (index < num.GetValueOrDefault()) & (num != null);
				MaterialPropertyValue materialPropertyValue;
				if (flag)
				{
					materialPropertyValue = srcList[index];
				}
				else
				{
					MaterialPropertyValue materialPropertyValue2 = refList[index];
					materialPropertyValue = new MaterialPropertyValue
					{
						type = materialPropertyValue2.type,
						name = materialPropertyValue2.name
					};
				}
				return materialPropertyValue;
			}

			private static void Lerp(MaterialDefinition a, MaterialDefinition b, ref MaterialDefinition result, float t)
			{
				bool flag = t > 0.999f;
				if (flag)
				{
					result = new MaterialDefinition(b);
				}
				else
				{
					List<MaterialPropertyValue> propertyValues = a.propertyValues;
					List<MaterialPropertyValue> propertyValues2 = b.propertyValues;
					int num = Math.Max((propertyValues != null) ? propertyValues.Count : 0, (propertyValues2 != null) ? propertyValues2.Count : 0);
					bool flag2 = result.material == null;
					if (flag2)
					{
						result.material = a.material ?? b.material;
						result.propertyValues = new List<MaterialPropertyValue>(num);
					}
					while (result.propertyValues.Count < num)
					{
						result.propertyValues.Add(default(MaterialPropertyValue));
					}
					for (int i = 0; i < num; i++)
					{
						MaterialPropertyValue valueOrDefault = StylePropertyAnimationSystem.ValuesMaterialDefinition.GetValueOrDefault(propertyValues, propertyValues2, i);
						MaterialPropertyValue valueOrDefault2 = StylePropertyAnimationSystem.ValuesMaterialDefinition.GetValueOrDefault(propertyValues2, propertyValues, i);
						result.propertyValues[i] = StylePropertyAnimationSystem.ValuesMaterialDefinition.LerpPropertyValues(valueOrDefault, valueOrDefault2, t);
					}
				}
			}

			protected sealed override void UpdateValues()
			{
				int count = this.running.count;
				for (int i = 0; i < count; i++)
				{
					ref StylePropertyAnimationSystem.Values<MaterialDefinition>.TimingData ptr = ref this.running.timing[i];
					ref StylePropertyAnimationSystem.Values<MaterialDefinition>.StyleData ptr2 = ref this.running.style[i];
					StylePropertyAnimationSystem.ValuesMaterialDefinition.Lerp(ptr2.startValue, ptr2.endValue, ref ptr2.currentValue, ptr.easedProgress);
				}
			}
		}
	}
}
