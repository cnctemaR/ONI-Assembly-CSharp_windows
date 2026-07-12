using System;

namespace System.IO
{
	internal struct DisableMediaInsertionPrompt : IDisposable
	{
		public static DisableMediaInsertionPrompt Create()
		{
			DisableMediaInsertionPrompt disableMediaInsertionPrompt = default(DisableMediaInsertionPrompt);
			disableMediaInsertionPrompt._disableSuccess = Interop.Kernel32.SetThreadErrorMode(1U, out disableMediaInsertionPrompt._oldMode);
			return disableMediaInsertionPrompt;
		}

		public void Dispose()
		{
			if (this._disableSuccess)
			{
				uint num;
				Interop.Kernel32.SetThreadErrorMode(this._oldMode, out num);
			}
		}

		private bool _disableSuccess;

		private uint _oldMode;
	}
}
