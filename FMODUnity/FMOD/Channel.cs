using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	public struct Channel : IChannelControl
	{
		public RESULT setFrequency(float frequency)
		{
			return Channel.FMOD5_Channel_SetFrequency(this.handle, frequency);
		}

		public RESULT getFrequency(out float frequency)
		{
			return Channel.FMOD5_Channel_GetFrequency(this.handle, out frequency);
		}

		public RESULT setPriority(int priority)
		{
			return Channel.FMOD5_Channel_SetPriority(this.handle, priority);
		}

		public RESULT getPriority(out int priority)
		{
			return Channel.FMOD5_Channel_GetPriority(this.handle, out priority);
		}

		public RESULT setPosition(uint position, TIMEUNIT postype)
		{
			return Channel.FMOD5_Channel_SetPosition(this.handle, position, postype);
		}

		public RESULT getPosition(out uint position, TIMEUNIT postype)
		{
			return Channel.FMOD5_Channel_GetPosition(this.handle, out position, postype);
		}

		public RESULT setChannelGroup(ChannelGroup channelgroup)
		{
			return Channel.FMOD5_Channel_SetChannelGroup(this.handle, channelgroup.handle);
		}

		public RESULT getChannelGroup(out ChannelGroup channelgroup)
		{
			return Channel.FMOD5_Channel_GetChannelGroup(this.handle, out channelgroup.handle);
		}

		public RESULT setLoopCount(int loopcount)
		{
			return Channel.FMOD5_Channel_SetLoopCount(this.handle, loopcount);
		}

		public RESULT getLoopCount(out int loopcount)
		{
			return Channel.FMOD5_Channel_GetLoopCount(this.handle, out loopcount);
		}

		public RESULT setLoopPoints(uint loopstart, TIMEUNIT loopstarttype, uint loopend, TIMEUNIT loopendtype)
		{
			return Channel.FMOD5_Channel_SetLoopPoints(this.handle, loopstart, loopstarttype, loopend, loopendtype);
		}

		public RESULT getLoopPoints(out uint loopstart, TIMEUNIT loopstarttype, out uint loopend, TIMEUNIT loopendtype)
		{
			return Channel.FMOD5_Channel_GetLoopPoints(this.handle, out loopstart, loopstarttype, out loopend, loopendtype);
		}

		public RESULT isVirtual(out bool isvirtual)
		{
			return Channel.FMOD5_Channel_IsVirtual(this.handle, out isvirtual);
		}

		public RESULT getCurrentSound(out Sound sound)
		{
			return Channel.FMOD5_Channel_GetCurrentSound(this.handle, out sound.handle);
		}

		public RESULT getIndex(out int index)
		{
			return Channel.FMOD5_Channel_GetIndex(this.handle, out index);
		}

		public RESULT getSystemObject(out FMOD.System system)
		{
			return Channel.FMOD5_Channel_GetSystemObject(this.handle, out system.handle);
		}

		public RESULT stop()
		{
			return Channel.FMOD5_Channel_Stop(this.handle);
		}

		public RESULT setPaused(bool paused)
		{
			return Channel.FMOD5_Channel_SetPaused(this.handle, paused);
		}

		public RESULT getPaused(out bool paused)
		{
			return Channel.FMOD5_Channel_GetPaused(this.handle, out paused);
		}

		public RESULT setVolume(float volume)
		{
			return Channel.FMOD5_Channel_SetVolume(this.handle, volume);
		}

		public RESULT getVolume(out float volume)
		{
			return Channel.FMOD5_Channel_GetVolume(this.handle, out volume);
		}

		public RESULT setVolumeRamp(bool ramp)
		{
			return Channel.FMOD5_Channel_SetVolumeRamp(this.handle, ramp);
		}

		public RESULT getVolumeRamp(out bool ramp)
		{
			return Channel.FMOD5_Channel_GetVolumeRamp(this.handle, out ramp);
		}

		public RESULT getAudibility(out float audibility)
		{
			return Channel.FMOD5_Channel_GetAudibility(this.handle, out audibility);
		}

		public RESULT setPitch(float pitch)
		{
			return Channel.FMOD5_Channel_SetPitch(this.handle, pitch);
		}

		public RESULT getPitch(out float pitch)
		{
			return Channel.FMOD5_Channel_GetPitch(this.handle, out pitch);
		}

		public RESULT setMute(bool mute)
		{
			return Channel.FMOD5_Channel_SetMute(this.handle, mute);
		}

		public RESULT getMute(out bool mute)
		{
			return Channel.FMOD5_Channel_GetMute(this.handle, out mute);
		}

		public RESULT setReverbProperties(int instance, float wet)
		{
			return Channel.FMOD5_Channel_SetReverbProperties(this.handle, instance, wet);
		}

		public RESULT getReverbProperties(int instance, out float wet)
		{
			return Channel.FMOD5_Channel_GetReverbProperties(this.handle, instance, out wet);
		}

		public RESULT setLowPassGain(float gain)
		{
			return Channel.FMOD5_Channel_SetLowPassGain(this.handle, gain);
		}

		public RESULT getLowPassGain(out float gain)
		{
			return Channel.FMOD5_Channel_GetLowPassGain(this.handle, out gain);
		}

		public RESULT setMode(MODE mode)
		{
			return Channel.FMOD5_Channel_SetMode(this.handle, mode);
		}

		public RESULT getMode(out MODE mode)
		{
			return Channel.FMOD5_Channel_GetMode(this.handle, out mode);
		}

		public RESULT setCallback(CHANNELCONTROL_CALLBACK callback)
		{
			return Channel.FMOD5_Channel_SetCallback(this.handle, callback);
		}

		public RESULT isPlaying(out bool isplaying)
		{
			return Channel.FMOD5_Channel_IsPlaying(this.handle, out isplaying);
		}

		public RESULT setPan(float pan)
		{
			return Channel.FMOD5_Channel_SetPan(this.handle, pan);
		}

		public RESULT setMixLevelsOutput(float frontleft, float frontright, float center, float lfe, float surroundleft, float surroundright, float backleft, float backright)
		{
			return Channel.FMOD5_Channel_SetMixLevelsOutput(this.handle, frontleft, frontright, center, lfe, surroundleft, surroundright, backleft, backright);
		}

		public RESULT setMixLevelsInput(float[] levels, int numlevels)
		{
			return Channel.FMOD5_Channel_SetMixLevelsInput(this.handle, levels, numlevels);
		}

		public RESULT setMixMatrix(float[] matrix, int outchannels, int inchannels, int inchannel_hop = 0)
		{
			return Channel.FMOD5_Channel_SetMixMatrix(this.handle, matrix, outchannels, inchannels, inchannel_hop);
		}

		public RESULT getMixMatrix(float[] matrix, out int outchannels, out int inchannels, int inchannel_hop = 0)
		{
			return Channel.FMOD5_Channel_GetMixMatrix(this.handle, matrix, out outchannels, out inchannels, inchannel_hop);
		}

		public RESULT getDSPClock(out ulong dspclock, out ulong parentclock)
		{
			return Channel.FMOD5_Channel_GetDSPClock(this.handle, out dspclock, out parentclock);
		}

		public RESULT setDelay(ulong dspclock_start, ulong dspclock_end, bool stopchannels = true)
		{
			return Channel.FMOD5_Channel_SetDelay(this.handle, dspclock_start, dspclock_end, stopchannels);
		}

		public RESULT getDelay(out ulong dspclock_start, out ulong dspclock_end)
		{
			return Channel.FMOD5_Channel_GetDelay(this.handle, out dspclock_start, out dspclock_end, IntPtr.Zero);
		}

		public RESULT getDelay(out ulong dspclock_start, out ulong dspclock_end, out bool stopchannels)
		{
			return Channel.FMOD5_Channel_GetDelay(this.handle, out dspclock_start, out dspclock_end, out stopchannels);
		}

		public RESULT addFadePoint(ulong dspclock, float volume)
		{
			return Channel.FMOD5_Channel_AddFadePoint(this.handle, dspclock, volume);
		}

		public RESULT setFadePointRamp(ulong dspclock, float volume)
		{
			return Channel.FMOD5_Channel_SetFadePointRamp(this.handle, dspclock, volume);
		}

		public RESULT removeFadePoints(ulong dspclock_start, ulong dspclock_end)
		{
			return Channel.FMOD5_Channel_RemoveFadePoints(this.handle, dspclock_start, dspclock_end);
		}

		public RESULT getFadePoints(ref uint numpoints, ulong[] point_dspclock, float[] point_volume)
		{
			return Channel.FMOD5_Channel_GetFadePoints(this.handle, ref numpoints, point_dspclock, point_volume);
		}

		public RESULT getDSP(int index, out DSP dsp)
		{
			return Channel.FMOD5_Channel_GetDSP(this.handle, index, out dsp.handle);
		}

		public RESULT addDSP(int index, DSP dsp)
		{
			return Channel.FMOD5_Channel_AddDSP(this.handle, index, dsp.handle);
		}

		public RESULT removeDSP(DSP dsp)
		{
			return Channel.FMOD5_Channel_RemoveDSP(this.handle, dsp.handle);
		}

		public RESULT getNumDSPs(out int numdsps)
		{
			return Channel.FMOD5_Channel_GetNumDSPs(this.handle, out numdsps);
		}

		public RESULT setDSPIndex(DSP dsp, int index)
		{
			return Channel.FMOD5_Channel_SetDSPIndex(this.handle, dsp.handle, index);
		}

		public RESULT getDSPIndex(DSP dsp, out int index)
		{
			return Channel.FMOD5_Channel_GetDSPIndex(this.handle, dsp.handle, out index);
		}

		public RESULT set3DAttributes(ref VECTOR pos, ref VECTOR vel)
		{
			return Channel.FMOD5_Channel_Set3DAttributes(this.handle, ref pos, ref vel);
		}

		public RESULT get3DAttributes(out VECTOR pos, out VECTOR vel)
		{
			return Channel.FMOD5_Channel_Get3DAttributes(this.handle, out pos, out vel);
		}

		public RESULT set3DMinMaxDistance(float mindistance, float maxdistance)
		{
			return Channel.FMOD5_Channel_Set3DMinMaxDistance(this.handle, mindistance, maxdistance);
		}

		public RESULT get3DMinMaxDistance(out float mindistance, out float maxdistance)
		{
			return Channel.FMOD5_Channel_Get3DMinMaxDistance(this.handle, out mindistance, out maxdistance);
		}

		public RESULT set3DConeSettings(float insideconeangle, float outsideconeangle, float outsidevolume)
		{
			return Channel.FMOD5_Channel_Set3DConeSettings(this.handle, insideconeangle, outsideconeangle, outsidevolume);
		}

		public RESULT get3DConeSettings(out float insideconeangle, out float outsideconeangle, out float outsidevolume)
		{
			return Channel.FMOD5_Channel_Get3DConeSettings(this.handle, out insideconeangle, out outsideconeangle, out outsidevolume);
		}

		public RESULT set3DConeOrientation(ref VECTOR orientation)
		{
			return Channel.FMOD5_Channel_Set3DConeOrientation(this.handle, ref orientation);
		}

		public RESULT get3DConeOrientation(out VECTOR orientation)
		{
			return Channel.FMOD5_Channel_Get3DConeOrientation(this.handle, out orientation);
		}

		public RESULT set3DCustomRolloff(ref VECTOR points, int numpoints)
		{
			return Channel.FMOD5_Channel_Set3DCustomRolloff(this.handle, ref points, numpoints);
		}

		public RESULT get3DCustomRolloff(out IntPtr points, out int numpoints)
		{
			return Channel.FMOD5_Channel_Get3DCustomRolloff(this.handle, out points, out numpoints);
		}

		public RESULT set3DOcclusion(float directocclusion, float reverbocclusion)
		{
			return Channel.FMOD5_Channel_Set3DOcclusion(this.handle, directocclusion, reverbocclusion);
		}

		public RESULT get3DOcclusion(out float directocclusion, out float reverbocclusion)
		{
			return Channel.FMOD5_Channel_Get3DOcclusion(this.handle, out directocclusion, out reverbocclusion);
		}

		public RESULT set3DSpread(float angle)
		{
			return Channel.FMOD5_Channel_Set3DSpread(this.handle, angle);
		}

		public RESULT get3DSpread(out float angle)
		{
			return Channel.FMOD5_Channel_Get3DSpread(this.handle, out angle);
		}

		public RESULT set3DLevel(float level)
		{
			return Channel.FMOD5_Channel_Set3DLevel(this.handle, level);
		}

		public RESULT get3DLevel(out float level)
		{
			return Channel.FMOD5_Channel_Get3DLevel(this.handle, out level);
		}

		public RESULT set3DDopplerLevel(float level)
		{
			return Channel.FMOD5_Channel_Set3DDopplerLevel(this.handle, level);
		}

		public RESULT get3DDopplerLevel(out float level)
		{
			return Channel.FMOD5_Channel_Get3DDopplerLevel(this.handle, out level);
		}

		public RESULT set3DDistanceFilter(bool custom, float customLevel, float centerFreq)
		{
			return Channel.FMOD5_Channel_Set3DDistanceFilter(this.handle, custom, customLevel, centerFreq);
		}

		public RESULT get3DDistanceFilter(out bool custom, out float customLevel, out float centerFreq)
		{
			return Channel.FMOD5_Channel_Get3DDistanceFilter(this.handle, out custom, out customLevel, out centerFreq);
		}

		public RESULT setUserData(IntPtr userdata)
		{
			return Channel.FMOD5_Channel_SetUserData(this.handle, userdata);
		}

		public RESULT getUserData(out IntPtr userdata)
		{
			return Channel.FMOD5_Channel_GetUserData(this.handle, out userdata);
		}

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetFrequency(IntPtr channel, float frequency);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetFrequency(IntPtr channel, out float frequency);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetPriority(IntPtr channel, int priority);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetPriority(IntPtr channel, out int priority);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetPosition(IntPtr channel, uint position, TIMEUNIT postype);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetPosition(IntPtr channel, out uint position, TIMEUNIT postype);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetChannelGroup(IntPtr channel, IntPtr channelgroup);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetChannelGroup(IntPtr channel, out IntPtr channelgroup);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetLoopCount(IntPtr channel, int loopcount);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetLoopCount(IntPtr channel, out int loopcount);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetLoopPoints(IntPtr channel, uint loopstart, TIMEUNIT loopstarttype, uint loopend, TIMEUNIT loopendtype);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetLoopPoints(IntPtr channel, out uint loopstart, TIMEUNIT loopstarttype, out uint loopend, TIMEUNIT loopendtype);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_IsVirtual(IntPtr channel, out bool isvirtual);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetCurrentSound(IntPtr channel, out IntPtr sound);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetIndex(IntPtr channel, out int index);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetSystemObject(IntPtr channel, out IntPtr system);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Stop(IntPtr channel);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetPaused(IntPtr channel, bool paused);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetPaused(IntPtr channel, out bool paused);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetVolume(IntPtr channel, float volume);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetVolume(IntPtr channel, out float volume);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetVolumeRamp(IntPtr channel, bool ramp);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetVolumeRamp(IntPtr channel, out bool ramp);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetAudibility(IntPtr channel, out float audibility);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetPitch(IntPtr channel, float pitch);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetPitch(IntPtr channel, out float pitch);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetMute(IntPtr channel, bool mute);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetMute(IntPtr channel, out bool mute);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetReverbProperties(IntPtr channel, int instance, float wet);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetReverbProperties(IntPtr channel, int instance, out float wet);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetLowPassGain(IntPtr channel, float gain);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetLowPassGain(IntPtr channel, out float gain);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetMode(IntPtr channel, MODE mode);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetMode(IntPtr channel, out MODE mode);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetCallback(IntPtr channel, CHANNELCONTROL_CALLBACK callback);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_IsPlaying(IntPtr channel, out bool isplaying);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetPan(IntPtr channel, float pan);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetMixLevelsOutput(IntPtr channel, float frontleft, float frontright, float center, float lfe, float surroundleft, float surroundright, float backleft, float backright);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetMixLevelsInput(IntPtr channel, float[] levels, int numlevels);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetMixMatrix(IntPtr channel, float[] matrix, int outchannels, int inchannels, int inchannel_hop);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetMixMatrix(IntPtr channel, float[] matrix, out int outchannels, out int inchannels, int inchannel_hop);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetDSPClock(IntPtr channel, out ulong dspclock, out ulong parentclock);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetDelay(IntPtr channel, ulong dspclock_start, ulong dspclock_end, bool stopchannels);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetDelay(IntPtr channel, out ulong dspclock_start, out ulong dspclock_end, IntPtr zero);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetDelay(IntPtr channel, out ulong dspclock_start, out ulong dspclock_end, out bool stopchannels);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_AddFadePoint(IntPtr channel, ulong dspclock, float volume);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetFadePointRamp(IntPtr channel, ulong dspclock, float volume);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_RemoveFadePoints(IntPtr channel, ulong dspclock_start, ulong dspclock_end);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetFadePoints(IntPtr channel, ref uint numpoints, ulong[] point_dspclock, float[] point_volume);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetDSP(IntPtr channel, int index, out IntPtr dsp);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_AddDSP(IntPtr channel, int index, IntPtr dsp);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_RemoveDSP(IntPtr channel, IntPtr dsp);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetNumDSPs(IntPtr channel, out int numdsps);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetDSPIndex(IntPtr channel, IntPtr dsp, int index);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetDSPIndex(IntPtr channel, IntPtr dsp, out int index);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Set3DAttributes(IntPtr channel, ref VECTOR pos, ref VECTOR vel);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Get3DAttributes(IntPtr channel, out VECTOR pos, out VECTOR vel);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Set3DMinMaxDistance(IntPtr channel, float mindistance, float maxdistance);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Get3DMinMaxDistance(IntPtr channel, out float mindistance, out float maxdistance);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Set3DConeSettings(IntPtr channel, float insideconeangle, float outsideconeangle, float outsidevolume);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Get3DConeSettings(IntPtr channel, out float insideconeangle, out float outsideconeangle, out float outsidevolume);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Set3DConeOrientation(IntPtr channel, ref VECTOR orientation);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Get3DConeOrientation(IntPtr channel, out VECTOR orientation);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Set3DCustomRolloff(IntPtr channel, ref VECTOR points, int numpoints);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Get3DCustomRolloff(IntPtr channel, out IntPtr points, out int numpoints);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Set3DOcclusion(IntPtr channel, float directocclusion, float reverbocclusion);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Get3DOcclusion(IntPtr channel, out float directocclusion, out float reverbocclusion);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Set3DSpread(IntPtr channel, float angle);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Get3DSpread(IntPtr channel, out float angle);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Set3DLevel(IntPtr channel, float level);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Get3DLevel(IntPtr channel, out float level);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Set3DDopplerLevel(IntPtr channel, float level);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Get3DDopplerLevel(IntPtr channel, out float level);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Set3DDistanceFilter(IntPtr channel, bool custom, float customLevel, float centerFreq);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Get3DDistanceFilter(IntPtr channel, out bool custom, out float customLevel, out float centerFreq);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetUserData(IntPtr channel, IntPtr userdata);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetUserData(IntPtr channel, out IntPtr userdata);

		public Channel(IntPtr ptr)
		{
			this.handle = ptr;
		}

		public bool hasHandle()
		{
			return this.handle != IntPtr.Zero;
		}

		public void clearHandle()
		{
			this.handle = IntPtr.Zero;
		}

		public IntPtr handle;
	}
}
