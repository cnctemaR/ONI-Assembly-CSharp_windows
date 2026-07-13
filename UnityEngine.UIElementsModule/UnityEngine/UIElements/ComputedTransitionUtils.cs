using System;
using System.Collections.Generic;
using UnityEngine.UIElements.Experimental;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements
{
	internal static class ComputedTransitionUtils
	{
		internal static void UpdateComputedTransitions(ref ComputedStyle computedStyle)
		{
			bool flag = computedStyle.computedTransitions == null;
			if (flag)
			{
				computedStyle.computedTransitions = ComputedTransitionUtils.GetOrComputeTransitionPropertyData(ref computedStyle);
			}
		}

		internal static bool HasTransitionProperty(this ComputedStyle computedStyle, StylePropertyId id)
		{
			for (int i = computedStyle.computedTransitions.Length - 1; i >= 0; i--)
			{
				ComputedTransitionProperty computedTransitionProperty = computedStyle.computedTransitions[i];
				bool flag = computedTransitionProperty.id == id || StylePropertyUtil.IsMatchingShorthand(computedTransitionProperty.id, id);
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		internal static bool GetTransitionProperty(this ComputedStyle computedStyle, StylePropertyId id, out ComputedTransitionProperty result)
		{
			for (int i = computedStyle.computedTransitions.Length - 1; i >= 0; i--)
			{
				ComputedTransitionProperty computedTransitionProperty = computedStyle.computedTransitions[i];
				bool flag = computedTransitionProperty.id == id || StylePropertyUtil.IsMatchingShorthand(computedTransitionProperty.id, id);
				if (flag)
				{
					result = computedTransitionProperty;
					return true;
				}
			}
			result = default(ComputedTransitionProperty);
			return false;
		}

		private static ComputedTransitionProperty[] GetOrComputeTransitionPropertyData(ref ComputedStyle computedStyle)
		{
			int transitionHashCode = ComputedTransitionUtils.GetTransitionHashCode(ref computedStyle);
			ComputedTransitionProperty[] array;
			bool flag = !StyleCache.TryGetValue(transitionHashCode, out array);
			if (flag)
			{
				ComputedTransitionUtils.ComputeTransitionPropertyData(ref computedStyle, ComputedTransitionUtils.s_ComputedTransitionsBuffer);
				array = new ComputedTransitionProperty[ComputedTransitionUtils.s_ComputedTransitionsBuffer.Count];
				ComputedTransitionUtils.s_ComputedTransitionsBuffer.CopyTo(array);
				ComputedTransitionUtils.s_ComputedTransitionsBuffer.Clear();
				StyleCache.SetValue(transitionHashCode, array);
			}
			return array;
		}

		private static int GetTransitionHashCode(ref ComputedStyle cs)
		{
			int num = 0;
			foreach (TimeValue timeValue in cs.transitionDelay)
			{
				num = (num * 397) ^ timeValue.GetHashCode();
			}
			foreach (TimeValue timeValue2 in cs.transitionDuration)
			{
				num = (num * 397) ^ timeValue2.GetHashCode();
			}
			foreach (StylePropertyName stylePropertyName in cs.transitionProperty)
			{
				num = (num * 397) ^ stylePropertyName.GetHashCode();
			}
			foreach (EasingFunction easingFunction in cs.transitionTimingFunction)
			{
				num = (num * 397) ^ easingFunction.GetHashCode();
			}
			return num;
		}

		internal static bool SameTransitionProperty(ref ComputedStyle x, ref ComputedStyle y)
		{
			bool flag = x.computedTransitions == y.computedTransitions && x.computedTransitions != null;
			return flag || (ComputedTransitionUtils.SameTransitionProperty(x.transitionProperty, y.transitionProperty) && ComputedTransitionUtils.SameTransitionProperty(x.transitionDuration, y.transitionDuration) && ComputedTransitionUtils.SameTransitionProperty(x.transitionDelay, y.transitionDelay));
		}

		private static bool SameTransitionProperty(List<StylePropertyName> a, List<StylePropertyName> b)
		{
			bool flag = a == b;
			bool flag2;
			if (flag)
			{
				flag2 = true;
			}
			else
			{
				bool flag3 = a == null || b == null;
				if (flag3)
				{
					flag2 = false;
				}
				else
				{
					bool flag4 = a.Count != b.Count;
					if (flag4)
					{
						flag2 = false;
					}
					else
					{
						int count = a.Count;
						for (int i = 0; i < count; i++)
						{
							bool flag5 = a[i] != b[i];
							if (flag5)
							{
								return false;
							}
						}
						flag2 = true;
					}
				}
			}
			return flag2;
		}

		private static bool SameTransitionProperty(List<TimeValue> a, List<TimeValue> b)
		{
			bool flag = a == b;
			bool flag2;
			if (flag)
			{
				flag2 = true;
			}
			else
			{
				bool flag3 = a == null || b == null;
				if (flag3)
				{
					flag2 = false;
				}
				else
				{
					bool flag4 = a.Count != b.Count;
					if (flag4)
					{
						flag2 = false;
					}
					else
					{
						int count = a.Count;
						for (int i = 0; i < count; i++)
						{
							bool flag5 = a[i] != b[i];
							if (flag5)
							{
								return false;
							}
						}
						flag2 = true;
					}
				}
			}
			return flag2;
		}

		private static void ComputeTransitionPropertyData(ref ComputedStyle computedStyle, List<ComputedTransitionProperty> outData)
		{
			List<StylePropertyName> transitionProperty = computedStyle.transitionProperty;
			bool flag = transitionProperty == null || transitionProperty.Count == 0;
			if (!flag)
			{
				List<TimeValue> transitionDuration = computedStyle.transitionDuration;
				List<TimeValue> transitionDelay = computedStyle.transitionDelay;
				List<EasingFunction> transitionTimingFunction = computedStyle.transitionTimingFunction;
				int count = transitionProperty.Count;
				for (int i = 0; i < count; i++)
				{
					StylePropertyId id = transitionProperty[i].id;
					bool flag2 = id == StylePropertyId.Unknown || !StylePropertyUtil.IsAnimatable(id);
					if (!flag2)
					{
						int num = ComputedTransitionUtils.ConvertTransitionTime(ComputedTransitionUtils.GetWrappingTransitionData<TimeValue>(transitionDuration, i, new TimeValue(0f)));
						int num2 = ComputedTransitionUtils.ConvertTransitionTime(ComputedTransitionUtils.GetWrappingTransitionData<TimeValue>(transitionDelay, i, new TimeValue(0f)));
						float num3 = (float)(Mathf.Max(0, num) + num2);
						bool flag3 = num3 <= 0f;
						if (!flag3)
						{
							EasingFunction wrappingTransitionData = ComputedTransitionUtils.GetWrappingTransitionData<EasingFunction>(transitionTimingFunction, i, EasingMode.Ease);
							outData.Add(new ComputedTransitionProperty
							{
								id = id,
								durationMs = num,
								delayMs = num2,
								easingCurve = ComputedTransitionUtils.ConvertTransitionFunction(wrappingTransitionData.mode)
							});
						}
					}
				}
			}
		}

		private static T GetWrappingTransitionData<T>(List<T> list, int i, T defaultValue)
		{
			return (list.Count == 0) ? defaultValue : list[i % list.Count];
		}

		private static int ConvertTransitionTime(TimeValue time)
		{
			return Mathf.RoundToInt((time.unit == TimeUnit.Millisecond) ? time.value : (time.value * 1000f));
		}

		private static Func<float, float> ConvertTransitionFunction(EasingMode mode)
		{
			Func<float, float> func;
			switch (mode)
			{
			default:
				func = (float t) => t * (1.8f + t * (-0.6f + t * -0.2f));
				break;
			case EasingMode.EaseIn:
				func = (float t) => Easing.InQuad(t);
				break;
			case EasingMode.EaseOut:
				func = (float t) => Easing.OutQuad(t);
				break;
			case EasingMode.EaseInOut:
				func = (float t) => Easing.InOutQuad(t);
				break;
			case EasingMode.Linear:
				func = (float t) => Easing.Linear(t);
				break;
			case EasingMode.EaseInSine:
				func = (float t) => Easing.InSine(t);
				break;
			case EasingMode.EaseOutSine:
				func = (float t) => Easing.OutSine(t);
				break;
			case EasingMode.EaseInOutSine:
				func = (float t) => Easing.InOutSine(t);
				break;
			case EasingMode.EaseInCubic:
				func = (float t) => Easing.InCubic(t);
				break;
			case EasingMode.EaseOutCubic:
				func = (float t) => Easing.OutCubic(t);
				break;
			case EasingMode.EaseInOutCubic:
				func = (float t) => Easing.InOutCubic(t);
				break;
			case EasingMode.EaseInCirc:
				func = (float t) => Easing.InCirc(t);
				break;
			case EasingMode.EaseOutCirc:
				func = (float t) => Easing.OutCirc(t);
				break;
			case EasingMode.EaseInOutCirc:
				func = (float t) => Easing.InOutCirc(t);
				break;
			case EasingMode.EaseInElastic:
				func = (float t) => Easing.InElastic(t);
				break;
			case EasingMode.EaseOutElastic:
				func = (float t) => Easing.OutElastic(t);
				break;
			case EasingMode.EaseInOutElastic:
				func = (float t) => Easing.InOutElastic(t);
				break;
			case EasingMode.EaseInBack:
				func = (float t) => Easing.InBack(t);
				break;
			case EasingMode.EaseOutBack:
				func = (float t) => Easing.OutBack(t);
				break;
			case EasingMode.EaseInOutBack:
				func = (float t) => Easing.InOutBack(t);
				break;
			case EasingMode.EaseInBounce:
				func = (float t) => Easing.InBounce(t);
				break;
			case EasingMode.EaseOutBounce:
				func = (float t) => Easing.OutBounce(t);
				break;
			case EasingMode.EaseInOutBounce:
				func = (float t) => Easing.InOutBounce(t);
				break;
			}
			return func;
		}

		private static List<ComputedTransitionProperty> s_ComputedTransitionsBuffer = new List<ComputedTransitionProperty>();
	}
}
