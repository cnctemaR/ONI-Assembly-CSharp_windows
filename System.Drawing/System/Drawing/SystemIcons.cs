using System;

namespace System.Drawing
{
	public sealed class SystemIcons
	{
		static SystemIcons()
		{
			SystemIcons.icons[0] = new Icon("Mono.ico", true);
			SystemIcons.icons[1] = new Icon("Information.ico", true);
			SystemIcons.icons[2] = new Icon("Error.ico", true);
			SystemIcons.icons[3] = new Icon("Warning.ico", true);
			SystemIcons.icons[4] = new Icon("Question.ico", true);
			SystemIcons.icons[5] = new Icon("Shield.ico", true);
		}

		private SystemIcons()
		{
		}

		public static Icon Application
		{
			get
			{
				return SystemIcons.icons[0];
			}
		}

		public static Icon Asterisk
		{
			get
			{
				return SystemIcons.icons[1];
			}
		}

		public static Icon Error
		{
			get
			{
				return SystemIcons.icons[2];
			}
		}

		public static Icon Exclamation
		{
			get
			{
				return SystemIcons.icons[3];
			}
		}

		public static Icon Hand
		{
			get
			{
				return SystemIcons.icons[2];
			}
		}

		public static Icon Information
		{
			get
			{
				return SystemIcons.icons[1];
			}
		}

		public static Icon Question
		{
			get
			{
				return SystemIcons.icons[4];
			}
		}

		public static Icon Warning
		{
			get
			{
				return SystemIcons.icons[3];
			}
		}

		public static Icon WinLogo
		{
			get
			{
				return SystemIcons.icons[0];
			}
		}

		public static Icon Shield
		{
			get
			{
				return SystemIcons.icons[5];
			}
		}

		private static Icon[] icons = new Icon[6];

		private const int Application_Winlogo = 0;

		private const int Asterisk_Information = 1;

		private const int Error_Hand = 2;

		private const int Exclamation_Warning = 3;

		private const int Question_ = 4;

		private const int Shield_ = 5;
	}
}
