using System;

namespace System
{
	[Serializable]
	public sealed class ConsoleCancelEventArgs : EventArgs
	{
		internal ConsoleCancelEventArgs(ConsoleSpecialKey key)
		{
			this.specialKey = key;
		}

		public bool Cancel
		{
			get
			{
				return this.cancel;
			}
			set
			{
				this.cancel = value;
			}
		}

		public ConsoleSpecialKey SpecialKey
		{
			get
			{
				return this.specialKey;
			}
		}

		private bool cancel;

		private ConsoleSpecialKey specialKey;
	}
}
