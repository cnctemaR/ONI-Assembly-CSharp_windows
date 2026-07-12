using System;
using Unity;

namespace System
{
	[Serializable]
	public sealed class ConsoleCancelEventArgs : EventArgs
	{
		internal ConsoleCancelEventArgs(ConsoleSpecialKey type)
		{
			this._type = type;
		}

		public bool Cancel { get; set; }

		public ConsoleSpecialKey SpecialKey
		{
			get
			{
				return this._type;
			}
		}

		internal ConsoleCancelEventArgs()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private readonly ConsoleSpecialKey _type;
	}
}
