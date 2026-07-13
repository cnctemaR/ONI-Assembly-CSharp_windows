using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.Bindings;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal class EmptyStylePropertyAnimationSystem : IStylePropertyAnimationSystem
	{
		public bool StartTransition(VisualElement owner, StylePropertyId prop, float startValue, float endValue, int durationMs, int delayMs, Func<float, float> easingCurve)
		{
			return false;
		}

		public bool StartTransition(VisualElement owner, StylePropertyId prop, int startValue, int endValue, int durationMs, int delayMs, Func<float, float> easingCurve)
		{
			return false;
		}

		public bool StartTransition(VisualElement owner, StylePropertyId prop, Length startValue, Length endValue, int durationMs, int delayMs, Func<float, float> easingCurve)
		{
			return false;
		}

		public bool StartTransition(VisualElement owner, StylePropertyId prop, Color startValue, Color endValue, int durationMs, int delayMs, Func<float, float> easingCurve)
		{
			return false;
		}

		public bool StartTransitionEnum(VisualElement owner, StylePropertyId prop, int startValue, int endValue, int durationMs, int delayMs, Func<float, float> easingCurve)
		{
			return false;
		}

		public bool StartTransition(VisualElement owner, StylePropertyId prop, Background startValue, Background endValue, int durationMs, int delayMs, Func<float, float> easingCurve)
		{
			return false;
		}

		public bool StartTransition(VisualElement owner, StylePropertyId prop, FontDefinition startValue, FontDefinition endValue, int durationMs, int delayMs, Func<float, float> easingCurve)
		{
			return false;
		}

		public bool StartTransition(VisualElement owner, StylePropertyId prop, Font startValue, Font endValue, int durationMs, int delayMs, Func<float, float> easingCurve)
		{
			return false;
		}

		public bool StartTransition(VisualElement owner, StylePropertyId prop, Cursor startValue, Cursor endValue, int durationMs, int delayMs, Func<float, float> easingCurve)
		{
			return false;
		}

		public bool StartTransition(VisualElement owner, StylePropertyId prop, TextShadow startValue, TextShadow endValue, int durationMs, int delayMs, Func<float, float> easingCurve)
		{
			return false;
		}

		public bool StartTransition(VisualElement owner, StylePropertyId prop, Scale startValue, Scale endValue, int durationMs, int delayMs, [JetBrains.Annotations.NotNull] Func<float, float> easingCurve)
		{
			return false;
		}

		public bool StartTransition(VisualElement owner, StylePropertyId prop, TransformOrigin startValue, TransformOrigin endValue, int durationMs, int delayMs, [JetBrains.Annotations.NotNull] Func<float, float> easingCurve)
		{
			return false;
		}

		public bool StartTransition(VisualElement owner, StylePropertyId prop, Translate startValue, Translate endValue, int durationMs, int delayMs, [JetBrains.Annotations.NotNull] Func<float, float> easingCurve)
		{
			return false;
		}

		public bool StartTransition(VisualElement owner, StylePropertyId prop, Rotate startValue, Rotate endValue, int durationMs, int delayMs, [JetBrains.Annotations.NotNull] Func<float, float> easingCurve)
		{
			return false;
		}

		public bool StartTransition(VisualElement owner, StylePropertyId prop, Ratio startValue, Ratio endValue, int durationMs, int delayMs, [JetBrains.Annotations.NotNull] Func<float, float> easingCurve)
		{
			return false;
		}

		public bool StartTransition(VisualElement owner, StylePropertyId prop, BackgroundPosition startValue, BackgroundPosition endValue, int durationMs, int delayMs, [JetBrains.Annotations.NotNull] Func<float, float> easingCurve)
		{
			return false;
		}

		public bool StartTransition(VisualElement owner, StylePropertyId prop, BackgroundRepeat startValue, BackgroundRepeat endValue, int durationMs, int delayMs, [JetBrains.Annotations.NotNull] Func<float, float> easingCurve)
		{
			return false;
		}

		public bool StartTransition(VisualElement owner, StylePropertyId prop, BackgroundSize startValue, BackgroundSize endValue, int durationMs, int delayMs, [JetBrains.Annotations.NotNull] Func<float, float> easingCurve)
		{
			return false;
		}

		public bool StartTransition(VisualElement owner, StylePropertyId prop, List<FilterFunction> startValue, List<FilterFunction> endValue, int durationMs, int delayMs, [JetBrains.Annotations.NotNull] Func<float, float> easingCurve)
		{
			return false;
		}

		public bool StartTransition(VisualElement owner, StylePropertyId prop, MaterialDefinition startValue, MaterialDefinition endValue, int durationMs, int delayMs, [JetBrains.Annotations.NotNull] Func<float, float> easingCurve)
		{
			return false;
		}

		public void CancelAllAnimations()
		{
		}

		public void CancelAllAnimations(VisualElement owner)
		{
		}

		public void CancelAnimation(VisualElement owner, StylePropertyId id)
		{
		}

		public bool HasRunningAnimation(VisualElement owner, StylePropertyId id)
		{
			return false;
		}

		public void UpdateAnimation(VisualElement owner, StylePropertyId id)
		{
		}

		public void GetAllAnimations(VisualElement owner, List<StylePropertyId> propertyIds)
		{
		}

		public void Update(double updateTimeInSeconds)
		{
		}
	}
}
