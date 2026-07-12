using System;

namespace System.Drawing
{
	public sealed class SystemPens
	{
		private SystemPens()
		{
		}

		public static Pen ActiveCaptionText
		{
			get
			{
				if (SystemPens.active_caption_text == null)
				{
					SystemPens.active_caption_text = new Pen(SystemColors.ActiveCaptionText);
					SystemPens.active_caption_text.isModifiable = false;
				}
				return SystemPens.active_caption_text;
			}
		}

		public static Pen Control
		{
			get
			{
				if (SystemPens.control == null)
				{
					SystemPens.control = new Pen(SystemColors.Control);
					SystemPens.control.isModifiable = false;
				}
				return SystemPens.control;
			}
		}

		public static Pen ControlDark
		{
			get
			{
				if (SystemPens.control_dark == null)
				{
					SystemPens.control_dark = new Pen(SystemColors.ControlDark);
					SystemPens.control_dark.isModifiable = false;
				}
				return SystemPens.control_dark;
			}
		}

		public static Pen ControlDarkDark
		{
			get
			{
				if (SystemPens.control_dark_dark == null)
				{
					SystemPens.control_dark_dark = new Pen(SystemColors.ControlDarkDark);
					SystemPens.control_dark_dark.isModifiable = false;
				}
				return SystemPens.control_dark_dark;
			}
		}

		public static Pen ControlLight
		{
			get
			{
				if (SystemPens.control_light == null)
				{
					SystemPens.control_light = new Pen(SystemColors.ControlLight);
					SystemPens.control_light.isModifiable = false;
				}
				return SystemPens.control_light;
			}
		}

		public static Pen ControlLightLight
		{
			get
			{
				if (SystemPens.control_light_light == null)
				{
					SystemPens.control_light_light = new Pen(SystemColors.ControlLightLight);
					SystemPens.control_light_light.isModifiable = false;
				}
				return SystemPens.control_light_light;
			}
		}

		public static Pen ControlText
		{
			get
			{
				if (SystemPens.control_text == null)
				{
					SystemPens.control_text = new Pen(SystemColors.ControlText);
					SystemPens.control_text.isModifiable = false;
				}
				return SystemPens.control_text;
			}
		}

		public static Pen GrayText
		{
			get
			{
				if (SystemPens.gray_text == null)
				{
					SystemPens.gray_text = new Pen(SystemColors.GrayText);
					SystemPens.gray_text.isModifiable = false;
				}
				return SystemPens.gray_text;
			}
		}

		public static Pen Highlight
		{
			get
			{
				if (SystemPens.highlight == null)
				{
					SystemPens.highlight = new Pen(SystemColors.Highlight);
					SystemPens.highlight.isModifiable = false;
				}
				return SystemPens.highlight;
			}
		}

		public static Pen HighlightText
		{
			get
			{
				if (SystemPens.highlight_text == null)
				{
					SystemPens.highlight_text = new Pen(SystemColors.HighlightText);
					SystemPens.highlight_text.isModifiable = false;
				}
				return SystemPens.highlight_text;
			}
		}

		public static Pen InactiveCaptionText
		{
			get
			{
				if (SystemPens.inactive_caption_text == null)
				{
					SystemPens.inactive_caption_text = new Pen(SystemColors.InactiveCaptionText);
					SystemPens.inactive_caption_text.isModifiable = false;
				}
				return SystemPens.inactive_caption_text;
			}
		}

		public static Pen InfoText
		{
			get
			{
				if (SystemPens.info_text == null)
				{
					SystemPens.info_text = new Pen(SystemColors.InfoText);
					SystemPens.info_text.isModifiable = false;
				}
				return SystemPens.info_text;
			}
		}

		public static Pen MenuText
		{
			get
			{
				if (SystemPens.menu_text == null)
				{
					SystemPens.menu_text = new Pen(SystemColors.MenuText);
					SystemPens.menu_text.isModifiable = false;
				}
				return SystemPens.menu_text;
			}
		}

		public static Pen WindowFrame
		{
			get
			{
				if (SystemPens.window_frame == null)
				{
					SystemPens.window_frame = new Pen(SystemColors.WindowFrame);
					SystemPens.window_frame.isModifiable = false;
				}
				return SystemPens.window_frame;
			}
		}

		public static Pen WindowText
		{
			get
			{
				if (SystemPens.window_text == null)
				{
					SystemPens.window_text = new Pen(SystemColors.WindowText);
					SystemPens.window_text.isModifiable = false;
				}
				return SystemPens.window_text;
			}
		}

		public static Pen FromSystemColor(Color c)
		{
			if (c.IsSystemColor)
			{
				return new Pen(c)
				{
					isModifiable = false
				};
			}
			throw new ArgumentException(string.Format("The color {0} is not a system color.", c));
		}

		public static Pen ActiveBorder
		{
			get
			{
				if (SystemPens.active_border == null)
				{
					SystemPens.active_border = new Pen(SystemColors.ActiveBorder);
					SystemPens.active_border.isModifiable = false;
				}
				return SystemPens.active_border;
			}
		}

		public static Pen ActiveCaption
		{
			get
			{
				if (SystemPens.active_caption == null)
				{
					SystemPens.active_caption = new Pen(SystemColors.ActiveCaption);
					SystemPens.active_caption.isModifiable = false;
				}
				return SystemPens.active_caption;
			}
		}

		public static Pen AppWorkspace
		{
			get
			{
				if (SystemPens.app_workspace == null)
				{
					SystemPens.app_workspace = new Pen(SystemColors.AppWorkspace);
					SystemPens.app_workspace.isModifiable = false;
				}
				return SystemPens.app_workspace;
			}
		}

		public static Pen ButtonFace
		{
			get
			{
				if (SystemPens.button_face == null)
				{
					SystemPens.button_face = new Pen(SystemColors.ButtonFace);
					SystemPens.button_face.isModifiable = false;
				}
				return SystemPens.button_face;
			}
		}

		public static Pen ButtonHighlight
		{
			get
			{
				if (SystemPens.button_highlight == null)
				{
					SystemPens.button_highlight = new Pen(SystemColors.ButtonHighlight);
					SystemPens.button_highlight.isModifiable = false;
				}
				return SystemPens.button_highlight;
			}
		}

		public static Pen ButtonShadow
		{
			get
			{
				if (SystemPens.button_shadow == null)
				{
					SystemPens.button_shadow = new Pen(SystemColors.ButtonShadow);
					SystemPens.button_shadow.isModifiable = false;
				}
				return SystemPens.button_shadow;
			}
		}

		public static Pen Desktop
		{
			get
			{
				if (SystemPens.desktop == null)
				{
					SystemPens.desktop = new Pen(SystemColors.Desktop);
					SystemPens.desktop.isModifiable = false;
				}
				return SystemPens.desktop;
			}
		}

		public static Pen GradientActiveCaption
		{
			get
			{
				if (SystemPens.gradient_activecaption == null)
				{
					SystemPens.gradient_activecaption = new Pen(SystemColors.GradientActiveCaption);
					SystemPens.gradient_activecaption.isModifiable = false;
				}
				return SystemPens.gradient_activecaption;
			}
		}

		public static Pen GradientInactiveCaption
		{
			get
			{
				if (SystemPens.gradient_inactivecaption == null)
				{
					SystemPens.gradient_inactivecaption = new Pen(SystemColors.GradientInactiveCaption);
					SystemPens.gradient_inactivecaption.isModifiable = false;
				}
				return SystemPens.gradient_inactivecaption;
			}
		}

		public static Pen HotTrack
		{
			get
			{
				if (SystemPens.hot_track == null)
				{
					SystemPens.hot_track = new Pen(SystemColors.HotTrack);
					SystemPens.hot_track.isModifiable = false;
				}
				return SystemPens.hot_track;
			}
		}

		public static Pen InactiveBorder
		{
			get
			{
				if (SystemPens.inactive_border == null)
				{
					SystemPens.inactive_border = new Pen(SystemColors.InactiveBorder);
					SystemPens.inactive_border.isModifiable = false;
				}
				return SystemPens.inactive_border;
			}
		}

		public static Pen InactiveCaption
		{
			get
			{
				if (SystemPens.inactive_caption == null)
				{
					SystemPens.inactive_caption = new Pen(SystemColors.InactiveCaption);
					SystemPens.inactive_caption.isModifiable = false;
				}
				return SystemPens.inactive_caption;
			}
		}

		public static Pen Info
		{
			get
			{
				if (SystemPens.info == null)
				{
					SystemPens.info = new Pen(SystemColors.Info);
					SystemPens.info.isModifiable = false;
				}
				return SystemPens.info;
			}
		}

		public static Pen Menu
		{
			get
			{
				if (SystemPens.menu == null)
				{
					SystemPens.menu = new Pen(SystemColors.Menu);
					SystemPens.menu.isModifiable = false;
				}
				return SystemPens.menu;
			}
		}

		public static Pen MenuBar
		{
			get
			{
				if (SystemPens.menu_bar == null)
				{
					SystemPens.menu_bar = new Pen(SystemColors.MenuBar);
					SystemPens.menu_bar.isModifiable = false;
				}
				return SystemPens.menu_bar;
			}
		}

		public static Pen MenuHighlight
		{
			get
			{
				if (SystemPens.menu_highlight == null)
				{
					SystemPens.menu_highlight = new Pen(SystemColors.MenuHighlight);
					SystemPens.menu_highlight.isModifiable = false;
				}
				return SystemPens.menu_highlight;
			}
		}

		public static Pen ScrollBar
		{
			get
			{
				if (SystemPens.scroll_bar == null)
				{
					SystemPens.scroll_bar = new Pen(SystemColors.ScrollBar);
					SystemPens.scroll_bar.isModifiable = false;
				}
				return SystemPens.scroll_bar;
			}
		}

		public static Pen Window
		{
			get
			{
				if (SystemPens.window == null)
				{
					SystemPens.window = new Pen(SystemColors.Window);
					SystemPens.window.isModifiable = false;
				}
				return SystemPens.window;
			}
		}

		private static Pen active_caption_text;

		private static Pen control;

		private static Pen control_dark;

		private static Pen control_dark_dark;

		private static Pen control_light;

		private static Pen control_light_light;

		private static Pen control_text;

		private static Pen gray_text;

		private static Pen highlight;

		private static Pen highlight_text;

		private static Pen inactive_caption_text;

		private static Pen info_text;

		private static Pen menu_text;

		private static Pen window_frame;

		private static Pen window_text;

		private static Pen active_border;

		private static Pen active_caption;

		private static Pen app_workspace;

		private static Pen button_face;

		private static Pen button_highlight;

		private static Pen button_shadow;

		private static Pen desktop;

		private static Pen gradient_activecaption;

		private static Pen gradient_inactivecaption;

		private static Pen hot_track;

		private static Pen inactive_border;

		private static Pen inactive_caption;

		private static Pen info;

		private static Pen menu;

		private static Pen menu_bar;

		private static Pen menu_highlight;

		private static Pen scroll_bar;

		private static Pen window;
	}
}
