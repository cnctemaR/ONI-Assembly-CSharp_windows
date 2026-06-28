using System;

namespace System.Drawing
{
	public sealed class SystemBrushes
	{
		private SystemBrushes()
		{
		}

		public static Brush ActiveBorder
		{
			get
			{
				if (SystemBrushes.active_border == null)
				{
					SystemBrushes.active_border = new SolidBrush(SystemColors.ActiveBorder);
					SystemBrushes.active_border.isModifiable = false;
				}
				return SystemBrushes.active_border;
			}
		}

		public static Brush ActiveCaption
		{
			get
			{
				if (SystemBrushes.active_caption == null)
				{
					SystemBrushes.active_caption = new SolidBrush(SystemColors.ActiveCaption);
					SystemBrushes.active_caption.isModifiable = false;
				}
				return SystemBrushes.active_caption;
			}
		}

		public static Brush ActiveCaptionText
		{
			get
			{
				if (SystemBrushes.active_caption_text == null)
				{
					SystemBrushes.active_caption_text = new SolidBrush(SystemColors.ActiveCaptionText);
					SystemBrushes.active_caption_text.isModifiable = false;
				}
				return SystemBrushes.active_caption_text;
			}
		}

		public static Brush AppWorkspace
		{
			get
			{
				if (SystemBrushes.app_workspace == null)
				{
					SystemBrushes.app_workspace = new SolidBrush(SystemColors.AppWorkspace);
					SystemBrushes.app_workspace.isModifiable = false;
				}
				return SystemBrushes.app_workspace;
			}
		}

		public static Brush Control
		{
			get
			{
				if (SystemBrushes.control == null)
				{
					SystemBrushes.control = new SolidBrush(SystemColors.Control);
					SystemBrushes.control.isModifiable = false;
				}
				return SystemBrushes.control;
			}
		}

		public static Brush ControlLight
		{
			get
			{
				if (SystemBrushes.control_light == null)
				{
					SystemBrushes.control_light = new SolidBrush(SystemColors.ControlLight);
					SystemBrushes.control_light.isModifiable = false;
				}
				return SystemBrushes.control_light;
			}
		}

		public static Brush ControlLightLight
		{
			get
			{
				if (SystemBrushes.control_light_light == null)
				{
					SystemBrushes.control_light_light = new SolidBrush(SystemColors.ControlLightLight);
					SystemBrushes.control_light_light.isModifiable = false;
				}
				return SystemBrushes.control_light_light;
			}
		}

		public static Brush ControlDark
		{
			get
			{
				if (SystemBrushes.control_dark == null)
				{
					SystemBrushes.control_dark = new SolidBrush(SystemColors.ControlDark);
					SystemBrushes.control_dark.isModifiable = false;
				}
				return SystemBrushes.control_dark;
			}
		}

		public static Brush ControlDarkDark
		{
			get
			{
				if (SystemBrushes.control_dark_dark == null)
				{
					SystemBrushes.control_dark_dark = new SolidBrush(SystemColors.ControlDarkDark);
					SystemBrushes.control_dark_dark.isModifiable = false;
				}
				return SystemBrushes.control_dark_dark;
			}
		}

		public static Brush ControlText
		{
			get
			{
				if (SystemBrushes.control_text == null)
				{
					SystemBrushes.control_text = new SolidBrush(SystemColors.ControlText);
					SystemBrushes.control_text.isModifiable = false;
				}
				return SystemBrushes.control_text;
			}
		}

		public static Brush Highlight
		{
			get
			{
				if (SystemBrushes.highlight == null)
				{
					SystemBrushes.highlight = new SolidBrush(SystemColors.Highlight);
					SystemBrushes.highlight.isModifiable = false;
				}
				return SystemBrushes.highlight;
			}
		}

		public static Brush HighlightText
		{
			get
			{
				if (SystemBrushes.highlight_text == null)
				{
					SystemBrushes.highlight_text = new SolidBrush(SystemColors.HighlightText);
					SystemBrushes.highlight_text.isModifiable = false;
				}
				return SystemBrushes.highlight_text;
			}
		}

		public static Brush Window
		{
			get
			{
				if (SystemBrushes.window == null)
				{
					SystemBrushes.window = new SolidBrush(SystemColors.Window);
					SystemBrushes.window.isModifiable = false;
				}
				return SystemBrushes.window;
			}
		}

		public static Brush WindowText
		{
			get
			{
				if (SystemBrushes.window_text == null)
				{
					SystemBrushes.window_text = new SolidBrush(SystemColors.WindowText);
					SystemBrushes.window_text.isModifiable = false;
				}
				return SystemBrushes.window_text;
			}
		}

		public static Brush InactiveBorder
		{
			get
			{
				if (SystemBrushes.inactive_border == null)
				{
					SystemBrushes.inactive_border = new SolidBrush(SystemColors.InactiveBorder);
					SystemBrushes.inactive_border.isModifiable = false;
				}
				return SystemBrushes.inactive_border;
			}
		}

		public static Brush Desktop
		{
			get
			{
				if (SystemBrushes.desktop == null)
				{
					SystemBrushes.desktop = new SolidBrush(SystemColors.Desktop);
					SystemBrushes.desktop.isModifiable = false;
				}
				return SystemBrushes.desktop;
			}
		}

		public static Brush HotTrack
		{
			get
			{
				if (SystemBrushes.hot_track == null)
				{
					SystemBrushes.hot_track = new SolidBrush(SystemColors.HotTrack);
					SystemBrushes.hot_track.isModifiable = false;
				}
				return SystemBrushes.hot_track;
			}
		}

		public static Brush InactiveCaption
		{
			get
			{
				if (SystemBrushes.inactive_caption == null)
				{
					SystemBrushes.inactive_caption = new SolidBrush(SystemColors.InactiveCaption);
					SystemBrushes.inactive_caption.isModifiable = false;
				}
				return SystemBrushes.inactive_caption;
			}
		}

		public static Brush Info
		{
			get
			{
				if (SystemBrushes.info == null)
				{
					SystemBrushes.info = new SolidBrush(SystemColors.Info);
					SystemBrushes.info.isModifiable = false;
				}
				return SystemBrushes.info;
			}
		}

		public static Brush Menu
		{
			get
			{
				if (SystemBrushes.menu == null)
				{
					SystemBrushes.menu = new SolidBrush(SystemColors.Menu);
					SystemBrushes.menu.isModifiable = false;
				}
				return SystemBrushes.menu;
			}
		}

		public static Brush ScrollBar
		{
			get
			{
				if (SystemBrushes.scroll_bar == null)
				{
					SystemBrushes.scroll_bar = new SolidBrush(SystemColors.ScrollBar);
					SystemBrushes.scroll_bar.isModifiable = false;
				}
				return SystemBrushes.scroll_bar;
			}
		}

		public static Brush FromSystemColor(Color c)
		{
			if (c.IsSystemColor)
			{
				return new SolidBrush(c)
				{
					isModifiable = false
				};
			}
			string text = string.Format("The color {0} is not a system color.", c);
			throw new ArgumentException(text);
		}

		public static Brush ButtonFace
		{
			get
			{
				if (SystemBrushes.button_face == null)
				{
					SystemBrushes.button_face = new SolidBrush(SystemColors.ButtonFace);
					SystemBrushes.button_face.isModifiable = false;
				}
				return SystemBrushes.button_face;
			}
		}

		public static Brush ButtonHighlight
		{
			get
			{
				if (SystemBrushes.button_highlight == null)
				{
					SystemBrushes.button_highlight = new SolidBrush(SystemColors.ButtonHighlight);
					SystemBrushes.button_highlight.isModifiable = false;
				}
				return SystemBrushes.button_highlight;
			}
		}

		public static Brush ButtonShadow
		{
			get
			{
				if (SystemBrushes.button_shadow == null)
				{
					SystemBrushes.button_shadow = new SolidBrush(SystemColors.ButtonShadow);
					SystemBrushes.button_shadow.isModifiable = false;
				}
				return SystemBrushes.button_shadow;
			}
		}

		public static Brush GradientActiveCaption
		{
			get
			{
				if (SystemBrushes.gradient_activecaption == null)
				{
					SystemBrushes.gradient_activecaption = new SolidBrush(SystemColors.GradientActiveCaption);
					SystemBrushes.gradient_activecaption.isModifiable = false;
				}
				return SystemBrushes.gradient_activecaption;
			}
		}

		public static Brush GradientInactiveCaption
		{
			get
			{
				if (SystemBrushes.gradient_inactivecaption == null)
				{
					SystemBrushes.gradient_inactivecaption = new SolidBrush(SystemColors.GradientInactiveCaption);
					SystemBrushes.gradient_inactivecaption.isModifiable = false;
				}
				return SystemBrushes.gradient_inactivecaption;
			}
		}

		public static Brush GrayText
		{
			get
			{
				if (SystemBrushes.graytext == null)
				{
					SystemBrushes.graytext = new SolidBrush(SystemColors.GrayText);
					SystemBrushes.graytext.isModifiable = false;
				}
				return SystemBrushes.graytext;
			}
		}

		public static Brush InactiveCaptionText
		{
			get
			{
				if (SystemBrushes.inactive_captiontext == null)
				{
					SystemBrushes.inactive_captiontext = new SolidBrush(SystemColors.InactiveCaptionText);
					SystemBrushes.inactive_captiontext.isModifiable = false;
				}
				return SystemBrushes.inactive_captiontext;
			}
		}

		public static Brush InfoText
		{
			get
			{
				if (SystemBrushes.infotext == null)
				{
					SystemBrushes.infotext = new SolidBrush(SystemColors.InfoText);
					SystemBrushes.infotext.isModifiable = false;
				}
				return SystemBrushes.infotext;
			}
		}

		public static Brush MenuBar
		{
			get
			{
				if (SystemBrushes.menubar == null)
				{
					SystemBrushes.menubar = new SolidBrush(SystemColors.MenuBar);
					SystemBrushes.menubar.isModifiable = false;
				}
				return SystemBrushes.menubar;
			}
		}

		public static Brush MenuHighlight
		{
			get
			{
				if (SystemBrushes.menu_highlight == null)
				{
					SystemBrushes.menu_highlight = new SolidBrush(SystemColors.MenuHighlight);
					SystemBrushes.menu_highlight.isModifiable = false;
				}
				return SystemBrushes.menu_highlight;
			}
		}

		public static Brush MenuText
		{
			get
			{
				if (SystemBrushes.menu_text == null)
				{
					SystemBrushes.menu_text = new SolidBrush(SystemColors.MenuText);
					SystemBrushes.menu_text.isModifiable = false;
				}
				return SystemBrushes.menu_text;
			}
		}

		public static Brush WindowFrame
		{
			get
			{
				if (SystemBrushes.window_fame == null)
				{
					SystemBrushes.window_fame = new SolidBrush(SystemColors.WindowFrame);
					SystemBrushes.window_fame.isModifiable = false;
				}
				return SystemBrushes.window_fame;
			}
		}

		private static SolidBrush active_border;

		private static SolidBrush active_caption;

		private static SolidBrush active_caption_text;

		private static SolidBrush app_workspace;

		private static SolidBrush control;

		private static SolidBrush control_dark;

		private static SolidBrush control_dark_dark;

		private static SolidBrush control_light;

		private static SolidBrush control_light_light;

		private static SolidBrush control_text;

		private static SolidBrush desktop;

		private static SolidBrush highlight;

		private static SolidBrush highlight_text;

		private static SolidBrush hot_track;

		private static SolidBrush inactive_border;

		private static SolidBrush inactive_caption;

		private static SolidBrush info;

		private static SolidBrush menu;

		private static SolidBrush scroll_bar;

		private static SolidBrush window;

		private static SolidBrush window_text;

		private static SolidBrush button_face;

		private static SolidBrush button_highlight;

		private static SolidBrush button_shadow;

		private static SolidBrush gradient_activecaption;

		private static SolidBrush gradient_inactivecaption;

		private static SolidBrush graytext;

		private static SolidBrush inactive_captiontext;

		private static SolidBrush infotext;

		private static SolidBrush menubar;

		private static SolidBrush menu_highlight;

		private static SolidBrush menu_text;

		private static SolidBrush window_fame;
	}
}
