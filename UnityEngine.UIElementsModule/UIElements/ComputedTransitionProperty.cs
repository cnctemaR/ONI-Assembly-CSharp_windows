using System;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements
{
	internal struct ComputedTransitionProperty
	{
		public StylePropertyId id;

		public int durationMs;

		public int delayMs;

		public Func<float, float> easingCurve;
	}
}
