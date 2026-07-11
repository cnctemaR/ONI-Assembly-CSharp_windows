using System;

namespace FMOD
{
	public struct DSP_STATE_FUNCTIONS
	{
		public DSP_ALLOC_FUNC alloc;

		public DSP_REALLOC_FUNC realloc;

		public DSP_FREE_FUNC free;

		public DSP_GETSAMPLERATE_FUNC getsamplerate;

		public DSP_GETBLOCKSIZE_FUNC getblocksize;

		public IntPtr dft;

		public IntPtr pan;

		public DSP_GETSPEAKERMODE_FUNC getspeakermode;

		public DSP_GETCLOCK_FUNC getclock;

		public DSP_GETLISTENERATTRIBUTES_FUNC getlistenerattributes;

		public DSP_LOG_FUNC log;

		public DSP_GETUSERDATA_FUNC getuserdata;
	}
}
