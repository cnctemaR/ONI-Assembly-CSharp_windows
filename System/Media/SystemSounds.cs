using System;

namespace System.Media
{
	public sealed class SystemSounds
	{
		private SystemSounds()
		{
		}

		public static SystemSound Asterisk
		{
			get
			{
				return new SystemSound("Asterisk");
			}
		}

		public static SystemSound Beep
		{
			get
			{
				return new SystemSound("Beep");
			}
		}

		public static SystemSound Exclamation
		{
			get
			{
				return new SystemSound("Exclamation");
			}
		}

		public static SystemSound Hand
		{
			get
			{
				return new SystemSound("Hand");
			}
		}

		public static SystemSound Question
		{
			get
			{
				return new SystemSound("Question");
			}
		}
	}
}
