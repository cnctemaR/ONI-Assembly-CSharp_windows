using System;

namespace UnityEngine.Experimental.UIElements.StyleSheets
{
	internal static class StyleValueUtils
	{
		public static bool ApplyAndCompare(ref StyleValue<float> current, StyleValue<float> other)
		{
			float value = current.value;
			return current.Apply(other, StylePropertyApplyMode.CopyIfEqualOrGreaterSpecificity) && value != other.value;
		}

		public static bool ApplyAndCompare(ref StyleValue<int> current, StyleValue<int> other)
		{
			int value = current.value;
			return current.Apply(other, StylePropertyApplyMode.CopyIfEqualOrGreaterSpecificity) && value != other.value;
		}

		public static bool ApplyAndCompare(ref StyleValue<bool> current, StyleValue<bool> other)
		{
			bool value = current.value;
			return current.Apply(other, StylePropertyApplyMode.CopyIfEqualOrGreaterSpecificity) && value != other.value;
		}

		public static bool ApplyAndCompare(ref StyleValue<Color> current, StyleValue<Color> other)
		{
			Color value = current.value;
			return current.Apply(other, StylePropertyApplyMode.CopyIfEqualOrGreaterSpecificity) && value != other.value;
		}

		public static bool ApplyAndCompare<T>(ref StyleValue<T> current, StyleValue<T> other) where T : struct, IEquatable<T>
		{
			T value = current.value;
			return current.Apply(other, StylePropertyApplyMode.CopyIfEqualOrGreaterSpecificity) && !value.Equals(other.value);
		}

		public static bool ApplyAndCompareObject<T>(ref StyleValue<T> current, StyleValue<T> other) where T : class
		{
			T value = current.value;
			return current.Apply(other, StylePropertyApplyMode.CopyIfEqualOrGreaterSpecificity) && value != other.value;
		}
	}
}
