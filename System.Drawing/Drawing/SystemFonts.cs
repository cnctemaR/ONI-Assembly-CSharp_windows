using System;

namespace System.Drawing
{
	public sealed class SystemFonts
	{
		private SystemFonts()
		{
		}

		public static Font GetFontByName(string systemFontName)
		{
			if (systemFontName == "CaptionFont")
			{
				return SystemFonts.CaptionFont;
			}
			if (systemFontName == "DefaultFont")
			{
				return SystemFonts.DefaultFont;
			}
			if (systemFontName == "DialogFont")
			{
				return SystemFonts.DialogFont;
			}
			if (systemFontName == "IconTitleFont")
			{
				return SystemFonts.IconTitleFont;
			}
			if (systemFontName == "MenuFont")
			{
				return SystemFonts.MenuFont;
			}
			if (systemFontName == "MessageBoxFont")
			{
				return SystemFonts.MessageBoxFont;
			}
			if (systemFontName == "SmallCaptionFont")
			{
				return SystemFonts.SmallCaptionFont;
			}
			if (systemFontName == "StatusFont")
			{
				return SystemFonts.StatusFont;
			}
			return null;
		}

		public static Font CaptionFont
		{
			get
			{
				return new Font("Microsoft Sans Serif", 11f, "CaptionFont");
			}
		}

		public static Font DefaultFont
		{
			get
			{
				return new Font("Microsoft Sans Serif", 8.25f, "DefaultFont");
			}
		}

		public static Font DialogFont
		{
			get
			{
				return new Font("Tahoma", 8f, "DialogFont");
			}
		}

		public static Font IconTitleFont
		{
			get
			{
				return new Font("Microsoft Sans Serif", 11f, "IconTitleFont");
			}
		}

		public static Font MenuFont
		{
			get
			{
				return new Font("Microsoft Sans Serif", 11f, "MenuFont");
			}
		}

		public static Font MessageBoxFont
		{
			get
			{
				return new Font("Microsoft Sans Serif", 11f, "MessageBoxFont");
			}
		}

		public static Font SmallCaptionFont
		{
			get
			{
				return new Font("Microsoft Sans Serif", 11f, "SmallCaptionFont");
			}
		}

		public static Font StatusFont
		{
			get
			{
				return new Font("Microsoft Sans Serif", 11f, "StatusFont");
			}
		}
	}
}
