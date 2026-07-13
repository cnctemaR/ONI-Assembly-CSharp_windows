using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	public struct System
	{
		public RESULT release()
		{
			return FMOD.System.FMOD5_System_Release(this.handle);
		}

		public RESULT setOutput(OUTPUTTYPE output)
		{
			return FMOD.System.FMOD5_System_SetOutput(this.handle, output);
		}

		public RESULT getOutput(out OUTPUTTYPE output)
		{
			return FMOD.System.FMOD5_System_GetOutput(this.handle, out output);
		}

		public RESULT getNumDrivers(out int numdrivers)
		{
			return FMOD.System.FMOD5_System_GetNumDrivers(this.handle, out numdrivers);
		}

		public RESULT getDriverInfo(int id, out string name, int namelen, out Guid guid, out int systemrate, out SPEAKERMODE speakermode, out int speakermodechannels)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(namelen);
			RESULT result = FMOD.System.FMOD5_System_GetDriverInfo(this.handle, id, intPtr, namelen, out guid, out systemrate, out speakermode, out speakermodechannels);
			using (StringHelper.ThreadSafeEncoding freeHelper = StringHelper.GetFreeHelper())
			{
				name = freeHelper.stringFromNative(intPtr);
			}
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public RESULT getDriverInfo(int id, out Guid guid, out int systemrate, out SPEAKERMODE speakermode, out int speakermodechannels)
		{
			return FMOD.System.FMOD5_System_GetDriverInfo(this.handle, id, IntPtr.Zero, 0, out guid, out systemrate, out speakermode, out speakermodechannels);
		}

		public RESULT setDriver(int driver)
		{
			return FMOD.System.FMOD5_System_SetDriver(this.handle, driver);
		}

		public RESULT getDriver(out int driver)
		{
			return FMOD.System.FMOD5_System_GetDriver(this.handle, out driver);
		}

		public RESULT setSoftwareChannels(int numsoftwarechannels)
		{
			return FMOD.System.FMOD5_System_SetSoftwareChannels(this.handle, numsoftwarechannels);
		}

		public RESULT getSoftwareChannels(out int numsoftwarechannels)
		{
			return FMOD.System.FMOD5_System_GetSoftwareChannels(this.handle, out numsoftwarechannels);
		}

		public RESULT setSoftwareFormat(int samplerate, SPEAKERMODE speakermode, int numrawspeakers)
		{
			return FMOD.System.FMOD5_System_SetSoftwareFormat(this.handle, samplerate, speakermode, numrawspeakers);
		}

		public RESULT getSoftwareFormat(out int samplerate, out SPEAKERMODE speakermode, out int numrawspeakers)
		{
			return FMOD.System.FMOD5_System_GetSoftwareFormat(this.handle, out samplerate, out speakermode, out numrawspeakers);
		}

		public RESULT setDSPBufferSize(uint bufferlength, int numbuffers)
		{
			return FMOD.System.FMOD5_System_SetDSPBufferSize(this.handle, bufferlength, numbuffers);
		}

		public RESULT getDSPBufferSize(out uint bufferlength, out int numbuffers)
		{
			return FMOD.System.FMOD5_System_GetDSPBufferSize(this.handle, out bufferlength, out numbuffers);
		}

		public RESULT setFileSystem(FILE_OPEN_CALLBACK useropen, FILE_CLOSE_CALLBACK userclose, FILE_READ_CALLBACK userread, FILE_SEEK_CALLBACK userseek, FILE_ASYNCREAD_CALLBACK userasyncread, FILE_ASYNCCANCEL_CALLBACK userasynccancel, int blockalign)
		{
			return FMOD.System.FMOD5_System_SetFileSystem(this.handle, useropen, userclose, userread, userseek, userasyncread, userasynccancel, blockalign);
		}

		public RESULT attachFileSystem(FILE_OPEN_CALLBACK useropen, FILE_CLOSE_CALLBACK userclose, FILE_READ_CALLBACK userread, FILE_SEEK_CALLBACK userseek)
		{
			return FMOD.System.FMOD5_System_AttachFileSystem(this.handle, useropen, userclose, userread, userseek);
		}

		public RESULT setAdvancedSettings(ref ADVANCEDSETTINGS settings)
		{
			settings.cbSize = Marshal.SizeOf<ADVANCEDSETTINGS>();
			return FMOD.System.FMOD5_System_SetAdvancedSettings(this.handle, ref settings);
		}

		public RESULT getAdvancedSettings(ref ADVANCEDSETTINGS settings)
		{
			settings.cbSize = Marshal.SizeOf<ADVANCEDSETTINGS>();
			return FMOD.System.FMOD5_System_GetAdvancedSettings(this.handle, ref settings);
		}

		public RESULT setCallback(SYSTEM_CALLBACK callback, SYSTEM_CALLBACK_TYPE callbackmask = SYSTEM_CALLBACK_TYPE.ALL)
		{
			return FMOD.System.FMOD5_System_SetCallback(this.handle, callback, callbackmask);
		}

		public RESULT setPluginPath(string path)
		{
			RESULT result;
			using (StringHelper.ThreadSafeEncoding freeHelper = StringHelper.GetFreeHelper())
			{
				result = FMOD.System.FMOD5_System_SetPluginPath(this.handle, freeHelper.byteFromStringUTF8(path));
			}
			return result;
		}

		public RESULT loadPlugin(string filename, out uint handle, uint priority = 0U)
		{
			RESULT result;
			using (StringHelper.ThreadSafeEncoding freeHelper = StringHelper.GetFreeHelper())
			{
				result = FMOD.System.FMOD5_System_LoadPlugin(this.handle, freeHelper.byteFromStringUTF8(filename), out handle, priority);
			}
			return result;
		}

		public RESULT unloadPlugin(uint handle)
		{
			return FMOD.System.FMOD5_System_UnloadPlugin(this.handle, handle);
		}

		public RESULT getNumNestedPlugins(uint handle, out int count)
		{
			return FMOD.System.FMOD5_System_GetNumNestedPlugins(this.handle, handle, out count);
		}

		public RESULT getNestedPlugin(uint handle, int index, out uint nestedhandle)
		{
			return FMOD.System.FMOD5_System_GetNestedPlugin(this.handle, handle, index, out nestedhandle);
		}

		public RESULT getNumPlugins(PLUGINTYPE plugintype, out int numplugins)
		{
			return FMOD.System.FMOD5_System_GetNumPlugins(this.handle, plugintype, out numplugins);
		}

		public RESULT getPluginHandle(PLUGINTYPE plugintype, int index, out uint handle)
		{
			return FMOD.System.FMOD5_System_GetPluginHandle(this.handle, plugintype, index, out handle);
		}

		public RESULT getPluginInfo(uint handle, out PLUGINTYPE plugintype, out string name, int namelen, out uint version)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(namelen);
			RESULT result = FMOD.System.FMOD5_System_GetPluginInfo(this.handle, handle, out plugintype, intPtr, namelen, out version);
			using (StringHelper.ThreadSafeEncoding freeHelper = StringHelper.GetFreeHelper())
			{
				name = freeHelper.stringFromNative(intPtr);
			}
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public RESULT getPluginInfo(uint handle, out PLUGINTYPE plugintype, out uint version)
		{
			return FMOD.System.FMOD5_System_GetPluginInfo(this.handle, handle, out plugintype, IntPtr.Zero, 0, out version);
		}

		public RESULT setOutputByPlugin(uint handle)
		{
			return FMOD.System.FMOD5_System_SetOutputByPlugin(this.handle, handle);
		}

		public RESULT getOutputByPlugin(out uint handle)
		{
			return FMOD.System.FMOD5_System_GetOutputByPlugin(this.handle, out handle);
		}

		public RESULT createDSPByPlugin(uint handle, out DSP dsp)
		{
			return FMOD.System.FMOD5_System_CreateDSPByPlugin(this.handle, handle, out dsp.handle);
		}

		public RESULT getDSPInfoByPlugin(uint handle, out IntPtr description)
		{
			return FMOD.System.FMOD5_System_GetDSPInfoByPlugin(this.handle, handle, out description);
		}

		public RESULT registerDSP(ref DSP_DESCRIPTION description, out uint handle)
		{
			return FMOD.System.FMOD5_System_RegisterDSP(this.handle, ref description, out handle);
		}

		public RESULT init(int maxchannels, INITFLAGS flags, IntPtr extradriverdata)
		{
			return FMOD.System.FMOD5_System_Init(this.handle, maxchannels, flags, extradriverdata);
		}

		public RESULT close()
		{
			return FMOD.System.FMOD5_System_Close(this.handle);
		}

		public RESULT update()
		{
			return FMOD.System.FMOD5_System_Update(this.handle);
		}

		public RESULT setSpeakerPosition(SPEAKER speaker, float x, float y, bool active)
		{
			return FMOD.System.FMOD5_System_SetSpeakerPosition(this.handle, speaker, x, y, active);
		}

		public RESULT getSpeakerPosition(SPEAKER speaker, out float x, out float y, out bool active)
		{
			return FMOD.System.FMOD5_System_GetSpeakerPosition(this.handle, speaker, out x, out y, out active);
		}

		public RESULT setStreamBufferSize(uint filebuffersize, TIMEUNIT filebuffersizetype)
		{
			return FMOD.System.FMOD5_System_SetStreamBufferSize(this.handle, filebuffersize, filebuffersizetype);
		}

		public RESULT getStreamBufferSize(out uint filebuffersize, out TIMEUNIT filebuffersizetype)
		{
			return FMOD.System.FMOD5_System_GetStreamBufferSize(this.handle, out filebuffersize, out filebuffersizetype);
		}

		public RESULT set3DSettings(float dopplerscale, float distancefactor, float rolloffscale)
		{
			return FMOD.System.FMOD5_System_Set3DSettings(this.handle, dopplerscale, distancefactor, rolloffscale);
		}

		public RESULT get3DSettings(out float dopplerscale, out float distancefactor, out float rolloffscale)
		{
			return FMOD.System.FMOD5_System_Get3DSettings(this.handle, out dopplerscale, out distancefactor, out rolloffscale);
		}

		public RESULT set3DNumListeners(int numlisteners)
		{
			return FMOD.System.FMOD5_System_Set3DNumListeners(this.handle, numlisteners);
		}

		public RESULT get3DNumListeners(out int numlisteners)
		{
			return FMOD.System.FMOD5_System_Get3DNumListeners(this.handle, out numlisteners);
		}

		public RESULT set3DListenerAttributes(int listener, ref VECTOR pos, ref VECTOR vel, ref VECTOR forward, ref VECTOR up)
		{
			return FMOD.System.FMOD5_System_Set3DListenerAttributes(this.handle, listener, ref pos, ref vel, ref forward, ref up);
		}

		public RESULT get3DListenerAttributes(int listener, out VECTOR pos, out VECTOR vel, out VECTOR forward, out VECTOR up)
		{
			return FMOD.System.FMOD5_System_Get3DListenerAttributes(this.handle, listener, out pos, out vel, out forward, out up);
		}

		public RESULT set3DRolloffCallback(CB_3D_ROLLOFF_CALLBACK callback)
		{
			return FMOD.System.FMOD5_System_Set3DRolloffCallback(this.handle, callback);
		}

		public RESULT mixerSuspend()
		{
			return FMOD.System.FMOD5_System_MixerSuspend(this.handle);
		}

		public RESULT mixerResume()
		{
			return FMOD.System.FMOD5_System_MixerResume(this.handle);
		}

		public RESULT getDefaultMixMatrix(SPEAKERMODE sourcespeakermode, SPEAKERMODE targetspeakermode, float[] matrix, int matrixhop)
		{
			return FMOD.System.FMOD5_System_GetDefaultMixMatrix(this.handle, sourcespeakermode, targetspeakermode, matrix, matrixhop);
		}

		public RESULT getSpeakerModeChannels(SPEAKERMODE mode, out int channels)
		{
			return FMOD.System.FMOD5_System_GetSpeakerModeChannels(this.handle, mode, out channels);
		}

		public RESULT getVersion(out uint version)
		{
			uint num;
			return this.getVersion(out version, out num);
		}

		public RESULT getVersion(out uint version, out uint buildnumber)
		{
			return FMOD.System.FMOD5_System_GetVersion(this.handle, out version, out buildnumber);
		}

		public RESULT getOutputHandle(out IntPtr handle)
		{
			return FMOD.System.FMOD5_System_GetOutputHandle(this.handle, out handle);
		}

		public RESULT getChannelsPlaying(out int channels)
		{
			return FMOD.System.FMOD5_System_GetChannelsPlaying(this.handle, out channels, IntPtr.Zero);
		}

		public RESULT getChannelsPlaying(out int channels, out int realchannels)
		{
			return FMOD.System.FMOD5_System_GetChannelsPlaying(this.handle, out channels, out realchannels);
		}

		public RESULT getCPUUsage(out CPU_USAGE usage)
		{
			return FMOD.System.FMOD5_System_GetCPUUsage(this.handle, out usage);
		}

		public RESULT getFileUsage(out long sampleBytesRead, out long streamBytesRead, out long otherBytesRead)
		{
			return FMOD.System.FMOD5_System_GetFileUsage(this.handle, out sampleBytesRead, out streamBytesRead, out otherBytesRead);
		}

		public RESULT createSound(string name, MODE mode, ref CREATESOUNDEXINFO exinfo, out Sound sound)
		{
			RESULT result;
			using (StringHelper.ThreadSafeEncoding freeHelper = StringHelper.GetFreeHelper())
			{
				result = FMOD.System.FMOD5_System_CreateSound(this.handle, freeHelper.byteFromStringUTF8(name), mode, ref exinfo, out sound.handle);
			}
			return result;
		}

		public RESULT createSound(byte[] data, MODE mode, ref CREATESOUNDEXINFO exinfo, out Sound sound)
		{
			return FMOD.System.FMOD5_System_CreateSound(this.handle, data, mode, ref exinfo, out sound.handle);
		}

		public RESULT createSound(IntPtr name_or_data, MODE mode, ref CREATESOUNDEXINFO exinfo, out Sound sound)
		{
			return FMOD.System.FMOD5_System_CreateSound(this.handle, name_or_data, mode, ref exinfo, out sound.handle);
		}

		public RESULT createSound(string name, MODE mode, out Sound sound)
		{
			CREATESOUNDEXINFO createsoundexinfo = default(CREATESOUNDEXINFO);
			createsoundexinfo.cbsize = Marshal.SizeOf<CREATESOUNDEXINFO>();
			return this.createSound(name, mode, ref createsoundexinfo, out sound);
		}

		public RESULT createStream(string name, MODE mode, ref CREATESOUNDEXINFO exinfo, out Sound sound)
		{
			RESULT result;
			using (StringHelper.ThreadSafeEncoding freeHelper = StringHelper.GetFreeHelper())
			{
				result = FMOD.System.FMOD5_System_CreateStream(this.handle, freeHelper.byteFromStringUTF8(name), mode, ref exinfo, out sound.handle);
			}
			return result;
		}

		public RESULT createStream(byte[] data, MODE mode, ref CREATESOUNDEXINFO exinfo, out Sound sound)
		{
			return FMOD.System.FMOD5_System_CreateStream(this.handle, data, mode, ref exinfo, out sound.handle);
		}

		public RESULT createStream(IntPtr name_or_data, MODE mode, ref CREATESOUNDEXINFO exinfo, out Sound sound)
		{
			return FMOD.System.FMOD5_System_CreateStream(this.handle, name_or_data, mode, ref exinfo, out sound.handle);
		}

		public RESULT createStream(string name, MODE mode, out Sound sound)
		{
			CREATESOUNDEXINFO createsoundexinfo = default(CREATESOUNDEXINFO);
			createsoundexinfo.cbsize = Marshal.SizeOf<CREATESOUNDEXINFO>();
			return this.createStream(name, mode, ref createsoundexinfo, out sound);
		}

		public RESULT createDSP(ref DSP_DESCRIPTION description, out DSP dsp)
		{
			return FMOD.System.FMOD5_System_CreateDSP(this.handle, ref description, out dsp.handle);
		}

		public RESULT createDSPByType(DSP_TYPE type, out DSP dsp)
		{
			return FMOD.System.FMOD5_System_CreateDSPByType(this.handle, type, out dsp.handle);
		}

		public RESULT createDSPConnection(DSPCONNECTION_TYPE type, out DSPConnection connection)
		{
			return FMOD.System.FMOD5_System_CreateDSPConnection(this.handle, type, out connection.handle);
		}

		public RESULT createChannelGroup(string name, out ChannelGroup channelgroup)
		{
			RESULT result;
			using (StringHelper.ThreadSafeEncoding freeHelper = StringHelper.GetFreeHelper())
			{
				result = FMOD.System.FMOD5_System_CreateChannelGroup(this.handle, freeHelper.byteFromStringUTF8(name), out channelgroup.handle);
			}
			return result;
		}

		public RESULT createSoundGroup(string name, out SoundGroup soundgroup)
		{
			RESULT result;
			using (StringHelper.ThreadSafeEncoding freeHelper = StringHelper.GetFreeHelper())
			{
				result = FMOD.System.FMOD5_System_CreateSoundGroup(this.handle, freeHelper.byteFromStringUTF8(name), out soundgroup.handle);
			}
			return result;
		}

		public RESULT createReverb3D(out Reverb3D reverb)
		{
			return FMOD.System.FMOD5_System_CreateReverb3D(this.handle, out reverb.handle);
		}

		public RESULT playSound(Sound sound, ChannelGroup channelgroup, bool paused, out Channel channel)
		{
			return FMOD.System.FMOD5_System_PlaySound(this.handle, sound.handle, channelgroup.handle, paused, out channel.handle);
		}

		public RESULT playDSP(DSP dsp, ChannelGroup channelgroup, bool paused, out Channel channel)
		{
			return FMOD.System.FMOD5_System_PlayDSP(this.handle, dsp.handle, channelgroup.handle, paused, out channel.handle);
		}

		public RESULT getChannel(int channelid, out Channel channel)
		{
			return FMOD.System.FMOD5_System_GetChannel(this.handle, channelid, out channel.handle);
		}

		public RESULT getDSPInfoByType(DSP_TYPE type, out IntPtr description)
		{
			return FMOD.System.FMOD5_System_GetDSPInfoByType(this.handle, type, out description);
		}

		public RESULT getMasterChannelGroup(out ChannelGroup channelgroup)
		{
			return FMOD.System.FMOD5_System_GetMasterChannelGroup(this.handle, out channelgroup.handle);
		}

		public RESULT getMasterSoundGroup(out SoundGroup soundgroup)
		{
			return FMOD.System.FMOD5_System_GetMasterSoundGroup(this.handle, out soundgroup.handle);
		}

		public RESULT attachChannelGroupToPort(PORT_TYPE portType, ulong portIndex, ChannelGroup channelgroup, bool passThru = false)
		{
			return FMOD.System.FMOD5_System_AttachChannelGroupToPort(this.handle, portType, portIndex, channelgroup.handle, passThru);
		}

		public RESULT detachChannelGroupFromPort(ChannelGroup channelgroup)
		{
			return FMOD.System.FMOD5_System_DetachChannelGroupFromPort(this.handle, channelgroup.handle);
		}

		public RESULT setReverbProperties(int instance, ref REVERB_PROPERTIES prop)
		{
			return FMOD.System.FMOD5_System_SetReverbProperties(this.handle, instance, ref prop);
		}

		public RESULT getReverbProperties(int instance, out REVERB_PROPERTIES prop)
		{
			return FMOD.System.FMOD5_System_GetReverbProperties(this.handle, instance, out prop);
		}

		public RESULT lockDSP()
		{
			return FMOD.System.FMOD5_System_LockDSP(this.handle);
		}

		public RESULT unlockDSP()
		{
			return FMOD.System.FMOD5_System_UnlockDSP(this.handle);
		}

		public RESULT getRecordNumDrivers(out int numdrivers, out int numconnected)
		{
			return FMOD.System.FMOD5_System_GetRecordNumDrivers(this.handle, out numdrivers, out numconnected);
		}

		public RESULT getRecordDriverInfo(int id, out string name, int namelen, out Guid guid, out int systemrate, out SPEAKERMODE speakermode, out int speakermodechannels, out DRIVER_STATE state)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(namelen);
			RESULT result = FMOD.System.FMOD5_System_GetRecordDriverInfo(this.handle, id, intPtr, namelen, out guid, out systemrate, out speakermode, out speakermodechannels, out state);
			using (StringHelper.ThreadSafeEncoding freeHelper = StringHelper.GetFreeHelper())
			{
				name = freeHelper.stringFromNative(intPtr);
			}
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public RESULT getRecordDriverInfo(int id, out Guid guid, out int systemrate, out SPEAKERMODE speakermode, out int speakermodechannels, out DRIVER_STATE state)
		{
			return FMOD.System.FMOD5_System_GetRecordDriverInfo(this.handle, id, IntPtr.Zero, 0, out guid, out systemrate, out speakermode, out speakermodechannels, out state);
		}

		public RESULT getRecordPosition(int id, out uint position)
		{
			return FMOD.System.FMOD5_System_GetRecordPosition(this.handle, id, out position);
		}

		public RESULT recordStart(int id, Sound sound, bool loop)
		{
			return FMOD.System.FMOD5_System_RecordStart(this.handle, id, sound.handle, loop);
		}

		public RESULT recordStop(int id)
		{
			return FMOD.System.FMOD5_System_RecordStop(this.handle, id);
		}

		public RESULT isRecording(int id, out bool recording)
		{
			return FMOD.System.FMOD5_System_IsRecording(this.handle, id, out recording);
		}

		public RESULT createGeometry(int maxpolygons, int maxvertices, out Geometry geometry)
		{
			return FMOD.System.FMOD5_System_CreateGeometry(this.handle, maxpolygons, maxvertices, out geometry.handle);
		}

		public RESULT setGeometrySettings(float maxworldsize)
		{
			return FMOD.System.FMOD5_System_SetGeometrySettings(this.handle, maxworldsize);
		}

		public RESULT getGeometrySettings(out float maxworldsize)
		{
			return FMOD.System.FMOD5_System_GetGeometrySettings(this.handle, out maxworldsize);
		}

		public RESULT loadGeometry(IntPtr data, int datasize, out Geometry geometry)
		{
			return FMOD.System.FMOD5_System_LoadGeometry(this.handle, data, datasize, out geometry.handle);
		}

		public RESULT getGeometryOcclusion(ref VECTOR listener, ref VECTOR source, out float direct, out float reverb)
		{
			return FMOD.System.FMOD5_System_GetGeometryOcclusion(this.handle, ref listener, ref source, out direct, out reverb);
		}

		public RESULT setNetworkProxy(string proxy)
		{
			RESULT result;
			using (StringHelper.ThreadSafeEncoding freeHelper = StringHelper.GetFreeHelper())
			{
				result = FMOD.System.FMOD5_System_SetNetworkProxy(this.handle, freeHelper.byteFromStringUTF8(proxy));
			}
			return result;
		}

		public RESULT getNetworkProxy(out string proxy, int proxylen)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(proxylen);
			RESULT result = FMOD.System.FMOD5_System_GetNetworkProxy(this.handle, intPtr, proxylen);
			using (StringHelper.ThreadSafeEncoding freeHelper = StringHelper.GetFreeHelper())
			{
				proxy = freeHelper.stringFromNative(intPtr);
			}
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public RESULT setNetworkTimeout(int timeout)
		{
			return FMOD.System.FMOD5_System_SetNetworkTimeout(this.handle, timeout);
		}

		public RESULT getNetworkTimeout(out int timeout)
		{
			return FMOD.System.FMOD5_System_GetNetworkTimeout(this.handle, out timeout);
		}

		public RESULT setUserData(IntPtr userdata)
		{
			return FMOD.System.FMOD5_System_SetUserData(this.handle, userdata);
		}

		public RESULT getUserData(out IntPtr userdata)
		{
			return FMOD.System.FMOD5_System_GetUserData(this.handle, out userdata);
		}

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_Release(IntPtr system);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_SetOutput(IntPtr system, OUTPUTTYPE output);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetOutput(IntPtr system, out OUTPUTTYPE output);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetNumDrivers(IntPtr system, out int numdrivers);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetDriverInfo(IntPtr system, int id, IntPtr name, int namelen, out Guid guid, out int systemrate, out SPEAKERMODE speakermode, out int speakermodechannels);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_SetDriver(IntPtr system, int driver);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetDriver(IntPtr system, out int driver);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_SetSoftwareChannels(IntPtr system, int numsoftwarechannels);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetSoftwareChannels(IntPtr system, out int numsoftwarechannels);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_SetSoftwareFormat(IntPtr system, int samplerate, SPEAKERMODE speakermode, int numrawspeakers);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetSoftwareFormat(IntPtr system, out int samplerate, out SPEAKERMODE speakermode, out int numrawspeakers);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_SetDSPBufferSize(IntPtr system, uint bufferlength, int numbuffers);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetDSPBufferSize(IntPtr system, out uint bufferlength, out int numbuffers);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_SetFileSystem(IntPtr system, FILE_OPEN_CALLBACK useropen, FILE_CLOSE_CALLBACK userclose, FILE_READ_CALLBACK userread, FILE_SEEK_CALLBACK userseek, FILE_ASYNCREAD_CALLBACK userasyncread, FILE_ASYNCCANCEL_CALLBACK userasynccancel, int blockalign);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_AttachFileSystem(IntPtr system, FILE_OPEN_CALLBACK useropen, FILE_CLOSE_CALLBACK userclose, FILE_READ_CALLBACK userread, FILE_SEEK_CALLBACK userseek);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_SetAdvancedSettings(IntPtr system, ref ADVANCEDSETTINGS settings);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetAdvancedSettings(IntPtr system, ref ADVANCEDSETTINGS settings);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_SetCallback(IntPtr system, SYSTEM_CALLBACK callback, SYSTEM_CALLBACK_TYPE callbackmask);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_SetPluginPath(IntPtr system, byte[] path);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_LoadPlugin(IntPtr system, byte[] filename, out uint handle, uint priority);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_UnloadPlugin(IntPtr system, uint handle);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetNumNestedPlugins(IntPtr system, uint handle, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetNestedPlugin(IntPtr system, uint handle, int index, out uint nestedhandle);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetNumPlugins(IntPtr system, PLUGINTYPE plugintype, out int numplugins);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetPluginHandle(IntPtr system, PLUGINTYPE plugintype, int index, out uint handle);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetPluginInfo(IntPtr system, uint handle, out PLUGINTYPE plugintype, IntPtr name, int namelen, out uint version);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_SetOutputByPlugin(IntPtr system, uint handle);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetOutputByPlugin(IntPtr system, out uint handle);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_CreateDSPByPlugin(IntPtr system, uint handle, out IntPtr dsp);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetDSPInfoByPlugin(IntPtr system, uint handle, out IntPtr description);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_RegisterDSP(IntPtr system, ref DSP_DESCRIPTION description, out uint handle);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_Init(IntPtr system, int maxchannels, INITFLAGS flags, IntPtr extradriverdata);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_Close(IntPtr system);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_Update(IntPtr system);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_SetSpeakerPosition(IntPtr system, SPEAKER speaker, float x, float y, bool active);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetSpeakerPosition(IntPtr system, SPEAKER speaker, out float x, out float y, out bool active);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_SetStreamBufferSize(IntPtr system, uint filebuffersize, TIMEUNIT filebuffersizetype);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetStreamBufferSize(IntPtr system, out uint filebuffersize, out TIMEUNIT filebuffersizetype);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_Set3DSettings(IntPtr system, float dopplerscale, float distancefactor, float rolloffscale);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_Get3DSettings(IntPtr system, out float dopplerscale, out float distancefactor, out float rolloffscale);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_Set3DNumListeners(IntPtr system, int numlisteners);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_Get3DNumListeners(IntPtr system, out int numlisteners);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_Set3DListenerAttributes(IntPtr system, int listener, ref VECTOR pos, ref VECTOR vel, ref VECTOR forward, ref VECTOR up);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_Get3DListenerAttributes(IntPtr system, int listener, out VECTOR pos, out VECTOR vel, out VECTOR forward, out VECTOR up);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_Set3DRolloffCallback(IntPtr system, CB_3D_ROLLOFF_CALLBACK callback);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_MixerSuspend(IntPtr system);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_MixerResume(IntPtr system);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetDefaultMixMatrix(IntPtr system, SPEAKERMODE sourcespeakermode, SPEAKERMODE targetspeakermode, float[] matrix, int matrixhop);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetSpeakerModeChannels(IntPtr system, SPEAKERMODE mode, out int channels);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetVersion(IntPtr system, out uint version, out uint buildnumber);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetOutputHandle(IntPtr system, out IntPtr handle);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetChannelsPlaying(IntPtr system, out int channels, IntPtr zero);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetChannelsPlaying(IntPtr system, out int channels, out int realchannels);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetCPUUsage(IntPtr system, out CPU_USAGE usage);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetFileUsage(IntPtr system, out long sampleBytesRead, out long streamBytesRead, out long otherBytesRead);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_CreateSound(IntPtr system, byte[] name_or_data, MODE mode, ref CREATESOUNDEXINFO exinfo, out IntPtr sound);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_CreateSound(IntPtr system, IntPtr name_or_data, MODE mode, ref CREATESOUNDEXINFO exinfo, out IntPtr sound);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_CreateStream(IntPtr system, byte[] name_or_data, MODE mode, ref CREATESOUNDEXINFO exinfo, out IntPtr sound);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_CreateStream(IntPtr system, IntPtr name_or_data, MODE mode, ref CREATESOUNDEXINFO exinfo, out IntPtr sound);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_CreateDSP(IntPtr system, ref DSP_DESCRIPTION description, out IntPtr dsp);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_CreateDSPByType(IntPtr system, DSP_TYPE type, out IntPtr dsp);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_CreateDSPConnection(IntPtr system, DSPCONNECTION_TYPE type, out IntPtr connection);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_CreateChannelGroup(IntPtr system, byte[] name, out IntPtr channelgroup);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_CreateSoundGroup(IntPtr system, byte[] name, out IntPtr soundgroup);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_CreateReverb3D(IntPtr system, out IntPtr reverb);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_PlaySound(IntPtr system, IntPtr sound, IntPtr channelgroup, bool paused, out IntPtr channel);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_PlayDSP(IntPtr system, IntPtr dsp, IntPtr channelgroup, bool paused, out IntPtr channel);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetChannel(IntPtr system, int channelid, out IntPtr channel);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetDSPInfoByType(IntPtr system, DSP_TYPE type, out IntPtr description);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetMasterChannelGroup(IntPtr system, out IntPtr channelgroup);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetMasterSoundGroup(IntPtr system, out IntPtr soundgroup);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_AttachChannelGroupToPort(IntPtr system, PORT_TYPE portType, ulong portIndex, IntPtr channelgroup, bool passThru);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_DetachChannelGroupFromPort(IntPtr system, IntPtr channelgroup);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_SetReverbProperties(IntPtr system, int instance, ref REVERB_PROPERTIES prop);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetReverbProperties(IntPtr system, int instance, out REVERB_PROPERTIES prop);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_LockDSP(IntPtr system);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_UnlockDSP(IntPtr system);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetRecordNumDrivers(IntPtr system, out int numdrivers, out int numconnected);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetRecordDriverInfo(IntPtr system, int id, IntPtr name, int namelen, out Guid guid, out int systemrate, out SPEAKERMODE speakermode, out int speakermodechannels, out DRIVER_STATE state);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetRecordPosition(IntPtr system, int id, out uint position);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_RecordStart(IntPtr system, int id, IntPtr sound, bool loop);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_RecordStop(IntPtr system, int id);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_IsRecording(IntPtr system, int id, out bool recording);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_CreateGeometry(IntPtr system, int maxpolygons, int maxvertices, out IntPtr geometry);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_SetGeometrySettings(IntPtr system, float maxworldsize);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetGeometrySettings(IntPtr system, out float maxworldsize);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_LoadGeometry(IntPtr system, IntPtr data, int datasize, out IntPtr geometry);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetGeometryOcclusion(IntPtr system, ref VECTOR listener, ref VECTOR source, out float direct, out float reverb);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_SetNetworkProxy(IntPtr system, byte[] proxy);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetNetworkProxy(IntPtr system, IntPtr proxy, int proxylen);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_SetNetworkTimeout(IntPtr system, int timeout);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetNetworkTimeout(IntPtr system, out int timeout);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_SetUserData(IntPtr system, IntPtr userdata);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetUserData(IntPtr system, out IntPtr userdata);

		public System(IntPtr ptr)
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
