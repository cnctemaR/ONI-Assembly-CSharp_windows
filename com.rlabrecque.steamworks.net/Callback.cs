using System;

namespace Steamworks
{
	public abstract class Callback
	{
		public abstract bool IsGameServer { get; }

		internal abstract Type GetCallbackType();

		internal abstract void OnRunCallback(IntPtr pvParam);

		internal abstract void SetUnregistered();
	}
}
