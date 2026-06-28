using System;

namespace FMOD
{
	public delegate RESULT FMOD_DSP_STATE_GETCLOCK(ref DSP_STATE dsp_state, out ulong clock, out uint offset, out uint length);
}
