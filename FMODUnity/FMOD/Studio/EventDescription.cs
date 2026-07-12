using System;
using System.Runtime.InteropServices;

namespace FMOD.Studio
{
	public struct EventDescription
	{
		public RESULT getID(out GUID id)
		{
			return EventDescription.FMOD_Studio_EventDescription_GetID(this.handle, out id);
		}

		public RESULT getPath(out string path)
		{
			path = null;
			RESULT result2;
			using (StringHelper.ThreadSafeEncoding freeHelper = StringHelper.GetFreeHelper())
			{
				IntPtr intPtr = Marshal.AllocHGlobal(256);
				int num = 0;
				RESULT result = EventDescription.FMOD_Studio_EventDescription_GetPath(this.handle, intPtr, 256, out num);
				if (result == RESULT.ERR_TRUNCATED)
				{
					Marshal.FreeHGlobal(intPtr);
					intPtr = Marshal.AllocHGlobal(num);
					result = EventDescription.FMOD_Studio_EventDescription_GetPath(this.handle, intPtr, num, out num);
				}
				if (result == RESULT.OK)
				{
					path = freeHelper.stringFromNative(intPtr);
				}
				Marshal.FreeHGlobal(intPtr);
				result2 = result;
			}
			return result2;
		}

		public RESULT getParameterDescriptionCount(out int count)
		{
			return EventDescription.FMOD_Studio_EventDescription_GetParameterDescriptionCount(this.handle, out count);
		}

		public RESULT getParameterDescriptionByIndex(int index, out PARAMETER_DESCRIPTION parameter)
		{
			return EventDescription.FMOD_Studio_EventDescription_GetParameterDescriptionByIndex(this.handle, index, out parameter);
		}

		public RESULT getParameterDescriptionByName(string name, out PARAMETER_DESCRIPTION parameter)
		{
			RESULT result;
			using (StringHelper.ThreadSafeEncoding freeHelper = StringHelper.GetFreeHelper())
			{
				result = EventDescription.FMOD_Studio_EventDescription_GetParameterDescriptionByName(this.handle, freeHelper.byteFromStringUTF8(name), out parameter);
			}
			return result;
		}

		public RESULT getParameterDescriptionByID(PARAMETER_ID id, out PARAMETER_DESCRIPTION parameter)
		{
			return EventDescription.FMOD_Studio_EventDescription_GetParameterDescriptionByID(this.handle, id, out parameter);
		}

		public RESULT getParameterLabelByIndex(int index, int labelindex, out string label)
		{
			label = null;
			RESULT result2;
			using (StringHelper.ThreadSafeEncoding freeHelper = StringHelper.GetFreeHelper())
			{
				IntPtr intPtr = Marshal.AllocHGlobal(256);
				int num = 0;
				RESULT result = EventDescription.FMOD_Studio_EventDescription_GetParameterLabelByIndex(this.handle, index, labelindex, intPtr, 256, out num);
				if (result == RESULT.ERR_TRUNCATED)
				{
					Marshal.FreeHGlobal(intPtr);
					result = EventDescription.FMOD_Studio_EventDescription_GetParameterLabelByIndex(this.handle, index, labelindex, IntPtr.Zero, 0, out num);
					intPtr = Marshal.AllocHGlobal(num);
					result = EventDescription.FMOD_Studio_EventDescription_GetParameterLabelByIndex(this.handle, index, labelindex, intPtr, num, out num);
				}
				if (result == RESULT.OK)
				{
					label = freeHelper.stringFromNative(intPtr);
				}
				Marshal.FreeHGlobal(intPtr);
				result2 = result;
			}
			return result2;
		}

		public RESULT getParameterLabelByName(string name, int labelindex, out string label)
		{
			label = null;
			RESULT result2;
			using (StringHelper.ThreadSafeEncoding freeHelper = StringHelper.GetFreeHelper())
			{
				IntPtr intPtr = Marshal.AllocHGlobal(256);
				int num = 0;
				byte[] array = freeHelper.byteFromStringUTF8(name);
				RESULT result = EventDescription.FMOD_Studio_EventDescription_GetParameterLabelByName(this.handle, array, labelindex, intPtr, 256, out num);
				if (result == RESULT.ERR_TRUNCATED)
				{
					Marshal.FreeHGlobal(intPtr);
					result = EventDescription.FMOD_Studio_EventDescription_GetParameterLabelByName(this.handle, array, labelindex, IntPtr.Zero, 0, out num);
					intPtr = Marshal.AllocHGlobal(num);
					result = EventDescription.FMOD_Studio_EventDescription_GetParameterLabelByName(this.handle, array, labelindex, intPtr, num, out num);
				}
				if (result == RESULT.OK)
				{
					label = freeHelper.stringFromNative(intPtr);
				}
				Marshal.FreeHGlobal(intPtr);
				result2 = result;
			}
			return result2;
		}

		public RESULT getParameterLabelByID(PARAMETER_ID id, int labelindex, out string label)
		{
			label = null;
			RESULT result2;
			using (StringHelper.ThreadSafeEncoding freeHelper = StringHelper.GetFreeHelper())
			{
				IntPtr intPtr = Marshal.AllocHGlobal(256);
				int num = 0;
				RESULT result = EventDescription.FMOD_Studio_EventDescription_GetParameterLabelByID(this.handle, id, labelindex, intPtr, 256, out num);
				if (result == RESULT.ERR_TRUNCATED)
				{
					Marshal.FreeHGlobal(intPtr);
					result = EventDescription.FMOD_Studio_EventDescription_GetParameterLabelByID(this.handle, id, labelindex, IntPtr.Zero, 0, out num);
					intPtr = Marshal.AllocHGlobal(num);
					result = EventDescription.FMOD_Studio_EventDescription_GetParameterLabelByID(this.handle, id, labelindex, intPtr, num, out num);
				}
				if (result == RESULT.OK)
				{
					label = freeHelper.stringFromNative(intPtr);
				}
				Marshal.FreeHGlobal(intPtr);
				result2 = result;
			}
			return result2;
		}

		public RESULT getUserPropertyCount(out int count)
		{
			return EventDescription.FMOD_Studio_EventDescription_GetUserPropertyCount(this.handle, out count);
		}

		public RESULT getUserPropertyByIndex(int index, out USER_PROPERTY property)
		{
			return EventDescription.FMOD_Studio_EventDescription_GetUserPropertyByIndex(this.handle, index, out property);
		}

		public RESULT getUserProperty(string name, out USER_PROPERTY property)
		{
			RESULT result;
			using (StringHelper.ThreadSafeEncoding freeHelper = StringHelper.GetFreeHelper())
			{
				result = EventDescription.FMOD_Studio_EventDescription_GetUserProperty(this.handle, freeHelper.byteFromStringUTF8(name), out property);
			}
			return result;
		}

		public RESULT getLength(out int length)
		{
			return EventDescription.FMOD_Studio_EventDescription_GetLength(this.handle, out length);
		}

		public RESULT getMinMaxDistance(out float min, out float max)
		{
			return EventDescription.FMOD_Studio_EventDescription_GetMinMaxDistance(this.handle, out min, out max);
		}

		public RESULT getSoundSize(out float size)
		{
			return EventDescription.FMOD_Studio_EventDescription_GetSoundSize(this.handle, out size);
		}

		public RESULT isSnapshot(out bool snapshot)
		{
			return EventDescription.FMOD_Studio_EventDescription_IsSnapshot(this.handle, out snapshot);
		}

		public RESULT isOneshot(out bool oneshot)
		{
			return EventDescription.FMOD_Studio_EventDescription_IsOneshot(this.handle, out oneshot);
		}

		public RESULT isStream(out bool isStream)
		{
			return EventDescription.FMOD_Studio_EventDescription_IsStream(this.handle, out isStream);
		}

		public RESULT is3D(out bool is3D)
		{
			return EventDescription.FMOD_Studio_EventDescription_Is3D(this.handle, out is3D);
		}

		public RESULT isDopplerEnabled(out bool doppler)
		{
			return EventDescription.FMOD_Studio_EventDescription_IsDopplerEnabled(this.handle, out doppler);
		}

		public RESULT hasSustainPoint(out bool sustainPoint)
		{
			return EventDescription.FMOD_Studio_EventDescription_HasSustainPoint(this.handle, out sustainPoint);
		}

		public RESULT createInstance(out EventInstance instance)
		{
			return EventDescription.FMOD_Studio_EventDescription_CreateInstance(this.handle, out instance.handle);
		}

		public RESULT getInstanceCount(out int count)
		{
			return EventDescription.FMOD_Studio_EventDescription_GetInstanceCount(this.handle, out count);
		}

		public RESULT getInstanceList(out EventInstance[] array)
		{
			array = null;
			int num;
			RESULT result = EventDescription.FMOD_Studio_EventDescription_GetInstanceCount(this.handle, out num);
			if (result != RESULT.OK)
			{
				return result;
			}
			if (num == 0)
			{
				array = new EventInstance[0];
				return result;
			}
			IntPtr[] array2 = new IntPtr[num];
			int num2;
			result = EventDescription.FMOD_Studio_EventDescription_GetInstanceList(this.handle, array2, num, out num2);
			if (result != RESULT.OK)
			{
				return result;
			}
			if (num2 > num)
			{
				num2 = num;
			}
			array = new EventInstance[num2];
			for (int i = 0; i < num2; i++)
			{
				array[i].handle = array2[i];
			}
			return RESULT.OK;
		}

		public RESULT loadSampleData()
		{
			return EventDescription.FMOD_Studio_EventDescription_LoadSampleData(this.handle);
		}

		public RESULT unloadSampleData()
		{
			return EventDescription.FMOD_Studio_EventDescription_UnloadSampleData(this.handle);
		}

		public RESULT getSampleLoadingState(out LOADING_STATE state)
		{
			return EventDescription.FMOD_Studio_EventDescription_GetSampleLoadingState(this.handle, out state);
		}

		public RESULT releaseAllInstances()
		{
			return EventDescription.FMOD_Studio_EventDescription_ReleaseAllInstances(this.handle);
		}

		public RESULT setCallback(EVENT_CALLBACK callback, EVENT_CALLBACK_TYPE callbackmask = EVENT_CALLBACK_TYPE.ALL)
		{
			return EventDescription.FMOD_Studio_EventDescription_SetCallback(this.handle, callback, callbackmask);
		}

		public RESULT getUserData(out IntPtr userdata)
		{
			return EventDescription.FMOD_Studio_EventDescription_GetUserData(this.handle, out userdata);
		}

		public RESULT setUserData(IntPtr userdata)
		{
			return EventDescription.FMOD_Studio_EventDescription_SetUserData(this.handle, userdata);
		}

		[DllImport("fmodstudio")]
		private static extern bool FMOD_Studio_EventDescription_IsValid(IntPtr eventdescription);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetID(IntPtr eventdescription, out GUID id);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetPath(IntPtr eventdescription, IntPtr path, int size, out int retrieved);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetParameterDescriptionCount(IntPtr eventdescription, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetParameterDescriptionByIndex(IntPtr eventdescription, int index, out PARAMETER_DESCRIPTION parameter);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetParameterDescriptionByName(IntPtr eventdescription, byte[] name, out PARAMETER_DESCRIPTION parameter);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetParameterDescriptionByID(IntPtr eventdescription, PARAMETER_ID id, out PARAMETER_DESCRIPTION parameter);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetParameterLabelByIndex(IntPtr eventdescription, int index, int labelindex, IntPtr label, int size, out int retrieved);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetParameterLabelByName(IntPtr eventdescription, byte[] name, int labelindex, IntPtr label, int size, out int retrieved);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetParameterLabelByID(IntPtr eventdescription, PARAMETER_ID id, int labelindex, IntPtr label, int size, out int retrieved);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetUserPropertyCount(IntPtr eventdescription, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetUserPropertyByIndex(IntPtr eventdescription, int index, out USER_PROPERTY property);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetUserProperty(IntPtr eventdescription, byte[] name, out USER_PROPERTY property);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetLength(IntPtr eventdescription, out int length);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetMinMaxDistance(IntPtr eventdescription, out float min, out float max);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetSoundSize(IntPtr eventdescription, out float size);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_IsSnapshot(IntPtr eventdescription, out bool snapshot);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_IsOneshot(IntPtr eventdescription, out bool oneshot);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_IsStream(IntPtr eventdescription, out bool isStream);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_Is3D(IntPtr eventdescription, out bool is3D);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_IsDopplerEnabled(IntPtr eventdescription, out bool doppler);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_HasSustainPoint(IntPtr eventdescription, out bool sustainPoint);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_CreateInstance(IntPtr eventdescription, out IntPtr instance);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetInstanceCount(IntPtr eventdescription, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetInstanceList(IntPtr eventdescription, IntPtr[] array, int capacity, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_LoadSampleData(IntPtr eventdescription);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_UnloadSampleData(IntPtr eventdescription);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetSampleLoadingState(IntPtr eventdescription, out LOADING_STATE state);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_ReleaseAllInstances(IntPtr eventdescription);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_SetCallback(IntPtr eventdescription, EVENT_CALLBACK callback, EVENT_CALLBACK_TYPE callbackmask);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetUserData(IntPtr eventdescription, out IntPtr userdata);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_SetUserData(IntPtr eventdescription, IntPtr userdata);

		public EventDescription(IntPtr ptr)
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

		public bool isValid()
		{
			return this.hasHandle() && EventDescription.FMOD_Studio_EventDescription_IsValid(this.handle);
		}

		public IntPtr handle;
	}
}
