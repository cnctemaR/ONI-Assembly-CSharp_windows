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

		public static bool ApplyAndCompare(ref StyleValue<CursorStyle> current, StyleValue<CursorStyle> other)
		{
			CursorStyle value = current.value;
			return current.Apply(other, StylePropertyApplyMode.CopyIfEqualOrGreaterSpecificity) && value != other.value;
		}

		public static bool ApplyAndCompare<T>(ref StyleValue<T> current, StyleValue<T> other) where T : Object
		{
			T value = current.value;
			return current.Apply(other, StylePropertyApplyMode.CopyIfEqualOrGreaterSpecificity) && value != other.value;
		}
	}
}
