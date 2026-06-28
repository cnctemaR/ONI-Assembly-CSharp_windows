using System;
using System.ComponentModel;
using System.Globalization;

namespace System.Drawing
{
	public sealed class ColorTranslator
	{
		private ColorTranslator()
		{
		}

		public static Color FromHtml(string htmlColor)
		{
			if (htmlColor == null || htmlColor.Length == 0)
			{
				return Color.Empty;
			}
			string text = htmlColor.ToLowerInvariant();
			switch (text)
			{
			case "buttonface":
			case "threedface":
				return SystemColors.Control;
			case "buttonhighlight":
			case "threedlightshadow":
				return SystemColors.ControlLightLight;
			case "buttonshadow":
				return SystemColors.ControlDark;
			case "captiontext":
				return SystemColors.ActiveCaptionText;
			case "threeddarkshadow":
				return SystemColors.ControlDarkDark;
			case "threedhighlight":
				return SystemColors.ControlLight;
			case "background":
				return SystemColors.Desktop;
			case "buttontext":
				return SystemColors.ControlText;
			case "infobackground":
				return SystemColors.Info;
			case "lightgrey":
				return Color.LightGray;
			}
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(Color));
			return (Color)converter.ConvertFromString(htmlColor);
		}

		internal static Color FromBGR(int bgr)
		{
			Color color = Color.FromArgb(255, bgr & 255, (bgr >> 8) & 255, (bgr >> 16) & 255);
			Color color2 = KnownColors.FindColorMatch(color);
			return (!color2.IsEmpty) ? color2 : color;
		}

		public static Color FromOle(int oleColor)
		{
			return ColorTranslator.FromBGR(oleColor);
		}

		public static Color FromWin32(int win32Color)
		{
			return ColorTranslator.FromBGR(win32Color);
		}

		public static string ToHtml(Color c)
		{
			if (c.IsEmpty)
			{
				return string.Empty;
			}
			if (c.IsSystemColor)
			{
				KnownColor knownColor = c.ToKnownColor();
				switch (knownColor)
				{
				case KnownColor.ActiveBorder:
				case KnownColor.ActiveCaption:
				case KnownColor.AppWorkspace:
				case KnownColor.GrayText:
				case KnownColor.Highlight:
				case KnownColor.HighlightText:
				case KnownColor.InactiveBorder:
				case KnownColor.InactiveCaption:
				case KnownColor.InactiveCaptionText:
				case KnownColor.InfoText:
				case KnownColor.Menu:
				case KnownColor.MenuText:
				case KnownColor.ScrollBar:
				case KnownColor.Window:
				case KnownColor.WindowFrame:
				case KnownColor.WindowText:
					return KnownColors.GetName(knownColor).ToLower(CultureInfo.InvariantCulture);
				case KnownColor.ActiveCaptionText:
					return "captiontext";
				case KnownColor.Control:
					return "buttonface";
				case KnownColor.ControlDark:
					return "buttonshadow";
				case KnownColor.ControlDarkDark:
					return "threeddarkshadow";
				case KnownColor.ControlLight:
					return "buttonface";
				case KnownColor.ControlLightLight:
					return "buttonhighlight";
				case KnownColor.ControlText:
					return "buttontext";
				case KnownColor.Desktop:
					return "background";
				case KnownColor.HotTrack:
					return "highlight";
				case KnownColor.Info:
					return "infobackground";
				default:
					return string.Empty;
				}
			}
			else
			{
				if (!c.IsNamedColor)
				{
					return ColorTranslator.FormatHtml((int)c.R, (int)c.G, (int)c.B);
				}
				if (c == Color.LightGray)
				{
					return "LightGrey";
				}
				return c.Name;
			}
		}

		private static char GetHexNumber(int b)
		{
			return (char)((b <= 9) ? (48 + b) : (55 + b));
		}

		private static string FormatHtml(int r, int g, int b)
		{
			return new string(new char[]
			{
				'#',
				ColorTranslator.GetHexNumber((r >> 4) & 15),
				ColorTranslator.GetHexNumber(r & 15),
				ColorTranslator.GetHexNumber((g >> 4) & 15),
				ColorTranslator.GetHexNumber(g & 15),
				ColorTranslator.GetHexNumber((b >> 4) & 15),
				ColorTranslator.GetHexNumber(b & 15)
			});
		}

		public static int ToOle(Color c)
		{
			return ((int)c.B << 16) | ((int)c.G << 8) | (int)c.R;
		}

		public static int ToWin32(Color c)
		{
			return ((int)c.B << 16) | ((int)c.G << 8) | (int)c.R;
		}
	}
}
