using System;
using Microsoft.Win32.SafeHandles;

namespace System.Security.Authentication.ExtendedProtection
{
	public abstract class ChannelBinding : SafeHandleZeroOrMinusOneIsInvalid
	{
		public abstract int Size { get; }

		protected ChannelBinding()
			: this(true)
		{
		}

		protected ChannelBinding(bool ownsHandle)
			: base(ownsHandle)
		{
		}
	}
}
