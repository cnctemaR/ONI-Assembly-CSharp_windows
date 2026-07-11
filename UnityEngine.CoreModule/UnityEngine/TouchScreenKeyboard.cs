using System;
using UnityEngine.Internal;

namespace UnityEngine
{
	public class TouchScreenKeyboard
	{
		public static TouchScreenKeyboard Open(string text, [DefaultValue("TouchScreenKeyboardType.Default")] TouchScreenKeyboardType keyboardType, [DefaultValue("true")] bool autocorrection, [DefaultValue("false")] bool multiline, [DefaultValue("false")] bool secure, [DefaultValue("false")] bool alert, [DefaultValue("\"\"")] string textPlaceholder, [DefaultValue("0")] int characterLimit)
		{
			return null;
		}

		[ExcludeFromDocs]
		public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType, bool autocorrection, bool multiline, bool secure, bool alert, string textPlaceholder)
		{
			return null;
		}

		[ExcludeFromDocs]
		public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType, bool autocorrection, bool multiline, bool secure, bool alert)
		{
			return null;
		}

		[ExcludeFromDocs]
		public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType, bool autocorrection, bool multiline, bool secure)
		{
			return null;
		}

		[ExcludeFromDocs]
		public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType, bool autocorrection, bool multiline)
		{
			return null;
		}

		[ExcludeFromDocs]
		public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType, bool autocorrection)
		{
			return null;
		}

		[ExcludeFromDocs]
		public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType)
		{
			return null;
		}

		[ExcludeFromDocs]
		public static TouchScreenKeyboard Open(string text)
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

		public TouchScreenKeyboard.Status status
		{
			get
			{
				return TouchScreenKeyboard.Status.Done;
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

		public bool canSetSelection
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
			set
			{
			}
		}

		public int characterLimit
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		public enum Status
		{
			Visible,
			Done,
			Canceled,
			LostFocus
		}
	}
}
