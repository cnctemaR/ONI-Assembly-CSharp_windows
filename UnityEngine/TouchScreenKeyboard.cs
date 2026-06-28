using System;
using UnityEngine.Internal;

namespace UnityEngine
{
	public sealed class TouchScreenKeyboard
	{
		[ExcludeFromDocs]
		public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType, bool autocorrection, bool multiline, bool secure, bool alert)
		{
			string text2 = "";
			return TouchScreenKeyboard.Open(text, keyboardType, autocorrection, multiline, secure, alert, text2);
		}

		[ExcludeFromDocs]
		public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType, bool autocorrection, bool multiline, bool secure)
		{
			string text2 = "";
			bool flag = false;
			return TouchScreenKeyboard.Open(text, keyboardType, autocorrection, multiline, secure, flag, text2);
		}

		[ExcludeFromDocs]
		public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType, bool autocorrection, bool multiline)
		{
			string text2 = "";
			bool flag = false;
			bool flag2 = false;
			return TouchScreenKeyboard.Open(text, keyboardType, autocorrection, multiline, flag2, flag, text2);
		}

		[ExcludeFromDocs]
		public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType, bool autocorrection)
		{
			string text2 = "";
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			return TouchScreenKeyboard.Open(text, keyboardType, autocorrection, flag3, flag2, flag, text2);
		}

		[ExcludeFromDocs]
		public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType)
		{
			string text2 = "";
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = true;
			return TouchScreenKeyboard.Open(text, keyboardType, flag4, flag3, flag2, flag, text2);
		}

		[ExcludeFromDocs]
		public static TouchScreenKeyboard Open(string text)
		{
			string text2 = "";
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = true;
			TouchScreenKeyboardType touchScreenKeyboardType = TouchScreenKeyboardType.Default;
			return TouchScreenKeyboard.Open(text, touchScreenKeyboardType, flag4, flag3, flag2, flag, text2);
		}

		public static TouchScreenKeyboard Open(string text, [DefaultValue("TouchScreenKeyboardType.Default")] TouchScreenKeyboardType keyboardType, [DefaultValue("true")] bool autocorrection, [DefaultValue("false")] bool multiline, [DefaultValue("false")] bool secure, [DefaultValue("false")] bool alert, [DefaultValue("\"\"")] string textPlaceholder)
		{
			return null;
		}

		public string text
		{
			get
			{
				return string.Empty;
			}
			set
			{
			}
		}

		public static bool hideInput
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public bool active
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public bool done
		{
			get
			{
				return true;
			}
		}

		public bool wasCanceled
		{
			get
			{
				return false;
			}
		}

		private static Rect area
		{
			get
			{
				return default(Rect);
			}
		}

		private static bool visible
		{
			get
			{
				return false;
			}
		}

		public static bool isSupported
		{
			get
			{
				return false;
			}
		}

		public bool canGetSelection
		{
			get
			{
				return false;
			}
		}

		public RangeInt selection
		{
			get
			{
				return new RangeInt(0, 0);
			}
		}
	}
}
