using System;

namespace System.Net
{
	internal interface IWebConnectionState
	{
		WebConnectionGroup Group { get; }

		ServicePoint ServicePoint { get; }

		bool Busy { get; }

		DateTime IdleSince { get; }

		bool TrySetBusy();

		void SetIdle();
	}
}
