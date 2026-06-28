using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD.Studio
{
	public class EventInstance : HandleBase
	{
		public EventInstance(IntPtr raw)
			: base(raw)
		{
		}

		public RESULT getDescription(out EventDescription description)
		{
			description = null;
			IntPtr intPtr;
			RESULT result = EventInstance.FMOD_Studio_EventInstance_GetDescription(this.rawPtr, out intPtr);
			RESULT result2;
			if (result != RESULT.OK)
			{
				result2 = result;
			}
			else
			{
				description = new EventDescription(intPtr);
				result2 = result;
			}
			return result2;
		}

		public RESULT getVolume(out float volume)
		{
			return EventInstance.FMOD_Studio_EventInstance_GetVolume(this.rawPtr, out volume);
		}

		public RESULT setVolume(float volume)
		{
			return EventInstance.FMOD_Studio_EventInstance_SetVolume(this.rawPtr, volume);
		}

		public RESULT getPitch(out float pitch)
		{
			return EventInstance.FMOD_Studio_EventInstance_GetPitch(this.rawPtr, out pitch);
		}

		public RESULT setPitch(float pitch)
		{
			return EventInstance.FMOD_Studio_EventInstance_SetPitch(this.rawPtr, pitch);
		}

		public RESULT get3DAttributes(out ATTRIBUTES_3D attributes)
		{
			return EventInstance.FMOD_Studio_EventInstance_Get3DAttributes(this.rawPtr, out attributes);
		}

		public RESULT set3DAttributes(ATTRIBUTES_3D attributes)
		{
			return EventInstance.FMOD_Studio_EventInstance_Set3DAttributes(this.rawPtr, ref attributes);
		}

		public RESULT getListenerMask(out uint mask)
		{
			return EventInstance.FMOD_Studio_EventInstance_GetListenerMask(this.rawPtr, out mask);
		}

		public RESULT setListenerMask(uint mask)
		{
			return EventInstance.FMOD_Studio_EventInstance_SetListenerMask(this.rawPtr, mask);
		}

		public RESULT getProperty(EVENT_PROPERTY index, out float value)
		{
			return EventInstance.FMOD_Studio_EventInstance_GetProperty(this.rawPtr, index, out value);
		}

		public RESULT setProperty(EVENT_PROPERTY index, float value)
		{
			return EventInstance.FMOD_Studio_EventInstance_SetProperty(this.rawPtr, index, value);
		}

		public RESULT getPaused(out bool paused)
		{
			return EventInstance.FMOD_Studio_EventInstance_GetPaused(this.rawPtr, out paused);
		}

		public RESULT setPaused(bool paused)
		{
			return EventInstance.FMOD_Studio_EventInstance_SetPaused(this.rawPtr, paused);
		}

		public RESULT start()
		{
			return EventInstance.FMOD_Studio_EventInstance_Start(this.rawPtr);
		}

		public RESULT stop(STOP_MODE mode)
		{
			return EventInstance.FMOD_Studio_EventInstance_Stop(this.rawPtr, mode);
		}

		public RESULT getTimelinePosition(out int position)
		{
			return EventInstance.FMOD_Studio_EventInstance_GetTimelinePosition(this.rawPtr, out position);
		}

		public RESULT setTimelinePosition(int position)
		{
			return EventInstance.FMOD_Studio_EventInstance_SetTimelinePosition(this.rawPtr, position);
		}

		public RESULT getPlaybackState(out PLAYBACK_STATE state)
		{
			return EventInstance.FMOD_Studio_EventInstance_GetPlaybackState(this.rawPtr, out state);
		}

		public RESULT getChannelGroup(out ChannelGroup group)
		{
			group = null;
			IntPtr intPtr = 0;
			RESULT result = EventInstance.FMOD_Studio_EventInstance_GetChannelGroup(this.rawPtr, out intPtr);
			RESULT result2;
			if (result != RESULT.OK)
			{
				result2 = result;
			}
			else
			{
				group = new ChannelGroup(intPtr);
				result2 = result;
			}
			return result2;
		}

		public RESULT release()
		{
			return EventInstance.FMOD_Studio_EventInstance_Release(this.rawPtr);
		}

		public RESULT isVirtual(out bool virtualState)
		{
			return EventInstance.FMOD_Studio_EventInstance_IsVirtual(this.rawPtr, out virtualState);
		}

		public RESULT getParameter(string name, out ParameterInstance instance)
		{
			instance = null;
			IntPtr intPtr = 0;
			RESULT result = EventInstance.FMOD_Studio_EventInstance_GetParameter(this.rawPtr, Encoding.UTF8.GetBytes(name + '\0'), out intPtr);
			RESULT result2;
			if (result != RESULT.OK)
			{
				result2 = result;
			}
			else
			{
				instance = new ParameterInstance(intPtr);
				result2 = result;
			}
			return result2;
		}

		public RESULT getParameterCount(out int count)
		{
			return EventInstance.FMOD_Studio_EventInstance_GetParameterCount(this.rawPtr, out count);
		}

		public RESULT getParameterByIndex(int index, out ParameterInstance instance)
		{
			instance = null;
			IntPtr intPtr = 0;
			RESULT result = EventInstance.FMOD_Studio_EventInstance_GetParameterByIndex(this.rawPtr, index, out intPtr);
			RESULT result2;
			if (result != RESULT.OK)
			{
				result2 = result;
			}
			else
			{
				instance = new ParameterInstance(intPtr);
				result2 = result;
			}
			return result2;
		}

		public RESULT getParameterValue(string name, out float value)
		{
			return EventInstance.FMOD_Studio_EventInstance_GetParameterValue(this.rawPtr, Encoding.UTF8.GetBytes(name + '\0'), out value);
		}

		public RESULT setParameterValue(string name, float value)
		{
			return EventInstance.FMOD_Studio_EventInstance_SetParameterValue(this.rawPtr, Encoding.UTF8.GetBytes(name + '\0'), value);
		}

		public RESULT setParameterValue(ParameterID id, float value)
		{
			return EventInstance.FMOD_Studio_EventInstance_SetParameterValue(this.rawPtr, id.bytes, value);
		}

		public RESULT getParameterValueByIndex(int index, out float value)
		{
			return EventInstance.FMOD_Studio_EventInstance_GetParameterValueByIndex(this.rawPtr, index, out value);
		}

		public RESULT setParameterValueByIndex(int index, float value)
		{
			return EventInstance.FMOD_Studio_EventInstance_SetParameterValueByIndex(this.rawPtr, index, value);
		}

		public RESULT triggerCue()
		{
			return EventInstance.FMOD_Studio_EventInstance_TriggerCue(this.rawPtr);
		}

		public RESULT setCallback(EVENT_CALLBACK callback, EVENT_CALLBACK_TYPE callbackmask = EVENT_CALLBACK_TYPE.ALL)
		{
			return EventInstance.FMOD_Studio_EventInstance_SetCallback(this.rawPtr, callback, callbackmask);
		}

		public RESULT getUserData(out IntPtr userData)
		{
			return EventInstance.FMOD_Studio_EventInstance_GetUserData(this.rawPtr, out userData);
		}

		public RESULT setUserData(IntPtr userData)
		{
			return EventInstance.FMOD_Studio_EventInstance_SetUserData(this.rawPtr, userData);
		}

		[DllImport("fmodstudio")]
		private static extern bool FMOD_Studio_EventInstance_IsValid(IntPtr _event);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetDescription(IntPtr _event, out IntPtr description);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetVolume(IntPtr _event, out float volume);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_SetVolume(IntPtr _event, float volume);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetPitch(IntPtr _event, out float pitch);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_SetPitch(IntPtr _event, float pitch);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_Get3DAttributes(IntPtr _event, out ATTRIBUTES_3D attributes);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_Set3DAttributes(IntPtr _event, ref ATTRIBUTES_3D attributes);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetListenerMask(IntPtr _event, out uint mask);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_SetListenerMask(IntPtr _event, uint mask);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetProperty(IntPtr _event, EVENT_PROPERTY index, out float value);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_SetProperty(IntPtr _event, EVENT_PROPERTY index, float value);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetPaused(IntPtr _event, out bool paused);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_SetPaused(IntPtr _event, bool paused);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_Start(IntPtr _event);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_Stop(IntPtr _event, STOP_MODE mode);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetTimelinePosition(IntPtr _event, out int position);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_SetTimelinePosition(IntPtr _event, int position);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetPlaybackState(IntPtr _event, out PLAYBACK_STATE state);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetChannelGroup(IntPtr _event, out IntPtr group);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_Release(IntPtr _event);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_IsVirtual(IntPtr _event, out bool virtualState);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetParameter(IntPtr _event, byte[] name, out IntPtr parameter);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetParameterByIndex(IntPtr _event, int index, out IntPtr parameter);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetParameterCount(IntPtr _event, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetParameterValue(IntPtr _event, byte[] name, out float value);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_SetParameterValue(IntPtr _event, byte[] name, float value);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetParameterValueByIndex(IntPtr _event, int index, out float value);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_SetParameterValueByIndex(IntPtr _event, int index, float value);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_TriggerCue(IntPtr _event);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_SetCallback(IntPtr _event, EVENT_CALLBACK callback, EVENT_CALLBACK_TYPE callbackmask);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetUserData(IntPtr _event, out IntPtr userData);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_SetUserData(IntPtr _event, IntPtr userData);

		protected override bool isValidInternal()
		{
			return EventInstance.FMOD_Studio_EventInstance_IsValid(this.rawPtr);
		}
	}
}
