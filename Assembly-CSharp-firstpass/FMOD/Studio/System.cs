using System;
using System.Runtime.InteropServices;

namespace FMOD.Studio
{
	public struct System
	{
		public static RESULT create(out FMOD.Studio.System studiosystem)
		{
			return FMOD.Studio.System.FMOD_Studio_System_Create(out studiosystem.handle, 69637U);
		}

		public RESULT setAdvancedSettings(ADVANCEDSETTINGS settings)
		{
			settings.cbsize = Marshal.SizeOf(typeof(ADVANCEDSETTINGS));
			return FMOD.Studio.System.FMOD_Studio_System_SetAdvancedSettings(this.handle, ref settings);
		}

		public RESULT getAdvancedSettings(out ADVANCEDSETTINGS settings)
		{
			settings.cbsize = Marshal.SizeOf(typeof(ADVANCEDSETTINGS));
			return FMOD.Studio.System.FMOD_Studio_System_GetAdvancedSettings(this.handle, out settings);
		}

		public RESULT initialize(int maxchannels, INITFLAGS studioFlags, INITFLAGS flags, IntPtr extradriverdata)
		{
			return FMOD.Studio.System.FMOD_Studio_System_Initialize(this.handle, maxchannels, studioFlags, flags, extradriverdata);
		}

		public RESULT release()
		{
			return FMOD.Studio.System.FMOD_Studio_System_Release(this.handle);
		}

		public RESULT update()
		{
			return FMOD.Studio.System.FMOD_Studio_System_Update(this.handle);
		}

		public RESULT getLowLevelSystem(out FMOD.System system)
		{
			return FMOD.Studio.System.FMOD_Studio_System_GetLowLevelSystem(this.handle, out system.handle);
		}

		public RESULT getEvent(string path, out EventDescription _event)
		{
			RESULT result;
			using (StringHelper.ThreadSafeEncoding freeHelper = StringHelper.GetFreeHelper())
			{
				result = FMOD.Studio.System.FMOD_Studio_System_GetEvent(this.handle, freeHelper.byteFromStringUTF8(path), out _event.handle);
			}
			return result;
		}

		public RESULT getBus(string path, out Bus bus)
		{
			RESULT result;
			using (StringHelper.ThreadSafeEncoding freeHelper = StringHelper.GetFreeHelper())
			{
				result = FMOD.Studio.System.FMOD_Studio_System_GetBus(this.handle, freeHelper.byteFromStringUTF8(path), out bus.handle);
			}
			return result;
		}

		public RESULT getVCA(string path, out VCA vca)
		{
			RESULT result;
			using (StringHelper.ThreadSafeEncoding freeHelper = StringHelper.GetFreeHelper())
			{
				result = FMOD.Studio.System.FMOD_Studio_System_GetVCA(this.handle, freeHelper.byteFromStringUTF8(path), out vca.handle);
			}
			return result;
		}

		public RESULT getBank(string path, out Bank bank)
		{
			RESULT result;
			using (StringHelper.ThreadSafeEncoding freeHelper = StringHelper.GetFreeHelper())
			{
				result = FMOD.Studio.System.FMOD_Studio_System_GetBank(this.handle, freeHelper.byteFromStringUTF8(path), out bank.handle);
			}
			return result;
		}

		public RESULT getEventByID(Guid guid, out EventDescription _event)
		{
			return FMOD.Studio.System.FMOD_Studio_System_GetEventByID(this.handle, ref guid, out _event.handle);
		}

		public RESULT getBusByID(Guid guid, out Bus bus)
		{
			return FMOD.Studio.System.FMOD_Studio_System_GetBusByID(this.handle, ref guid, out bus.handle);
		}

		public RESULT getVCAByID(Guid guid, out VCA vca)
		{
			return FMOD.Studio.System.FMOD_Studio_System_GetVCAByID(this.handle, ref guid, out vca.handle);
		}

		public RESULT getBankByID(Guid guid, out Bank bank)
		{
			return FMOD.Studio.System.FMOD_Studio_System_GetBankByID(this.handle, ref guid, out bank.handle);
		}

		public RESULT getSoundInfo(string key, out SOUND_INFO info)
		{
			RESULT result;
			using (StringHelper.ThreadSafeEncoding freeHelper = StringHelper.GetFreeHelper())
			{
				result = FMOD.Studio.System.FMOD_Studio_System_GetSoundInfo(this.handle, freeHelper.byteFromStringUTF8(key), out info);
			}
			return result;
		}

		public RESULT lookupID(string path, out Guid guid)
		{
			RESULT result;
			using (StringHelper.ThreadSafeEncoding freeHelper = StringHelper.GetFreeHelper())
			{
				result = FMOD.Studio.System.FMOD_Studio_System_LookupID(this.handle, freeHelper.byteFromStringUTF8(path), out guid);
			}
			return result;
		}

		public RESULT lookupPath(Guid guid, out string path)
		{
			path = null;
			RESULT result2;
			using (StringHelper.ThreadSafeEncoding freeHelper = StringHelper.GetFreeHelper())
			{
				IntPtr intPtr = Marshal.AllocHGlobal(256);
				int num = 0;
				RESULT result = FMOD.Studio.System.FMOD_Studio_System_LookupPath(this.handle, ref guid, intPtr, 256, out num);
				if (result == RESULT.ERR_TRUNCATED)
				{
					Marshal.FreeHGlobal(intPtr);
					intPtr = Marshal.AllocHGlobal(num);
					result = FMOD.Studio.System.FMOD_Studio_System_LookupPath(this.handle, ref guid, intPtr, num, out num);
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

		public RESULT getNumListeners(out int numlisteners)
		{
			return FMOD.Studio.System.FMOD_Studio_System_GetNumListeners(this.handle, out numlisteners);
		}

		public RESULT setNumListeners(int numlisteners)
		{
			return FMOD.Studio.System.FMOD_Studio_System_SetNumListeners(this.handle, numlisteners);
		}

		public RESULT getListenerAttributes(int listener, out ATTRIBUTES_3D attributes)
		{
			return FMOD.Studio.System.FMOD_Studio_System_GetListenerAttributes(this.handle, listener, out attributes);
		}

		public RESULT setListenerAttributes(int listener, ATTRIBUTES_3D attributes)
		{
			return FMOD.Studio.System.FMOD_Studio_System_SetListenerAttributes(this.handle, listener, ref attributes);
		}

		public RESULT getListenerWeight(int listener, out float weight)
		{
			return FMOD.Studio.System.FMOD_Studio_System_GetListenerWeight(this.handle, listener, out weight);
		}

		public RESULT setListenerWeight(int listener, float weight)
		{
			return FMOD.Studio.System.FMOD_Studio_System_SetListenerWeight(this.handle, listener, weight);
		}

		public RESULT loadBankFile(string name, LOAD_BANK_FLAGS flags, out Bank bank)
		{
			RESULT result;
			using (StringHelper.ThreadSafeEncoding freeHelper = StringHelper.GetFreeHelper())
			{
				result = FMOD.Studio.System.FMOD_Studio_System_LoadBankFile(this.handle, freeHelper.byteFromStringUTF8(name), flags, out bank.handle);
			}
			return result;
		}

		public RESULT loadBankMemory(byte[] buffer, LOAD_BANK_FLAGS flags, out Bank bank)
		{
			GCHandle gchandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
			IntPtr intPtr = gchandle.AddrOfPinnedObject();
			RESULT result = FMOD.Studio.System.FMOD_Studio_System_LoadBankMemory(this.handle, intPtr, buffer.Length, LOAD_MEMORY_MODE.LOAD_MEMORY, flags, out bank.handle);
			gchandle.Free();
			return result;
		}

		public RESULT loadBankCustom(BANK_INFO info, LOAD_BANK_FLAGS flags, out Bank bank)
		{
			info.size = Marshal.SizeOf<BANK_INFO>(info);
			return FMOD.Studio.System.FMOD_Studio_System_LoadBankCustom(this.handle, ref info, flags, out bank.handle);
		}

		public RESULT unloadAll()
		{
			return FMOD.Studio.System.FMOD_Studio_System_UnloadAll(this.handle);
		}

		public RESULT flushCommands()
		{
			return FMOD.Studio.System.FMOD_Studio_System_FlushCommands(this.handle);
		}

		public RESULT flushSampleLoading()
		{
			return FMOD.Studio.System.FMOD_Studio_System_FlushSampleLoading(this.handle);
		}

		public RESULT startCommandCapture(string path, COMMANDCAPTURE_FLAGS flags)
		{
			RESULT result;
			using (StringHelper.ThreadSafeEncoding freeHelper = StringHelper.GetFreeHelper())
			{
				result = FMOD.Studio.System.FMOD_Studio_System_StartCommandCapture(this.handle, freeHelper.byteFromStringUTF8(path), flags);
			}
			return result;
		}

		public RESULT stopCommandCapture()
		{
			return FMOD.Studio.System.FMOD_Studio_System_StopCommandCapture(this.handle);
		}

		public RESULT loadCommandReplay(string path, COMMANDREPLAY_FLAGS flags, out CommandReplay replay)
		{
			RESULT result;
			using (StringHelper.ThreadSafeEncoding freeHelper = StringHelper.GetFreeHelper())
			{
				result = FMOD.Studio.System.FMOD_Studio_System_LoadCommandReplay(this.handle, freeHelper.byteFromStringUTF8(path), flags, out replay.handle);
			}
			return result;
		}

		public RESULT getBankCount(out int count)
		{
			return FMOD.Studio.System.FMOD_Studio_System_GetBankCount(this.handle, out count);
		}

		public RESULT getBankList(out Bank[] array)
		{
			array = null;
			int num;
			RESULT result = FMOD.Studio.System.FMOD_Studio_System_GetBankCount(this.handle, out num);
			if (result != RESULT.OK)
			{
				return result;
			}
			if (num == 0)
			{
				array = new Bank[0];
				return result;
			}
			IntPtr[] array2 = new IntPtr[num];
			int num2;
			result = FMOD.Studio.System.FMOD_Studio_System_GetBankList(this.handle, array2, num, out num2);
			if (result != RESULT.OK)
			{
				return result;
			}
			if (num2 > num)
			{
				num2 = num;
			}
			array = new Bank[num2];
			for (int i = 0; i < num2; i++)
			{
				array[i].handle = array2[i];
			}
			return RESULT.OK;
		}

		public RESULT getCPUUsage(out CPU_USAGE usage)
		{
			return FMOD.Studio.System.FMOD_Studio_System_GetCPUUsage(this.handle, out usage);
		}

		public RESULT getBufferUsage(out BUFFER_USAGE usage)
		{
			return FMOD.Studio.System.FMOD_Studio_System_GetBufferUsage(this.handle, out usage);
		}

		public RESULT resetBufferUsage()
		{
			return FMOD.Studio.System.FMOD_Studio_System_ResetBufferUsage(this.handle);
		}

		public RESULT setCallback(SYSTEM_CALLBACK callback, SYSTEM_CALLBACK_TYPE callbackmask = SYSTEM_CALLBACK_TYPE.ALL)
		{
			return FMOD.Studio.System.FMOD_Studio_System_SetCallback(this.handle, callback, callbackmask);
		}

		public RESULT getUserData(out IntPtr userdata)
		{
			return FMOD.Studio.System.FMOD_Studio_System_GetUserData(this.handle, out userdata);
		}

		public RESULT setUserData(IntPtr userdata)
		{
			return FMOD.Studio.System.FMOD_Studio_System_SetUserData(this.handle, userdata);
		}

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_Create(out IntPtr studiosystem, uint headerversion);

		[DllImport("fmodstudio")]
		private static extern bool FMOD_Studio_System_IsValid(IntPtr studiosystem);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_SetAdvancedSettings(IntPtr studiosystem, ref ADVANCEDSETTINGS settings);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetAdvancedSettings(IntPtr studiosystem, out ADVANCEDSETTINGS settings);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_Initialize(IntPtr studiosystem, int maxchannels, INITFLAGS studioFlags, INITFLAGS flags, IntPtr extradriverdata);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_Release(IntPtr studiosystem);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_Update(IntPtr studiosystem);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetLowLevelSystem(IntPtr studiosystem, out IntPtr system);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetEvent(IntPtr studiosystem, byte[] path, out IntPtr description);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetBus(IntPtr studiosystem, byte[] path, out IntPtr bus);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetVCA(IntPtr studiosystem, byte[] path, out IntPtr vca);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetBank(IntPtr studiosystem, byte[] path, out IntPtr bank);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetEventByID(IntPtr studiosystem, ref Guid guid, out IntPtr description);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetBusByID(IntPtr studiosystem, ref Guid guid, out IntPtr bus);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetVCAByID(IntPtr studiosystem, ref Guid guid, out IntPtr vca);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetBankByID(IntPtr studiosystem, ref Guid guid, out IntPtr bank);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetSoundInfo(IntPtr studiosystem, byte[] key, out SOUND_INFO info);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_LookupID(IntPtr studiosystem, byte[] path, out Guid guid);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_LookupPath(IntPtr studiosystem, ref Guid guid, IntPtr path, int size, out int retrieved);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetNumListeners(IntPtr studiosystem, out int numlisteners);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_SetNumListeners(IntPtr studiosystem, int numlisteners);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetListenerAttributes(IntPtr studiosystem, int listener, out ATTRIBUTES_3D attributes);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_SetListenerAttributes(IntPtr studiosystem, int listener, ref ATTRIBUTES_3D attributes);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetListenerWeight(IntPtr studiosystem, int listener, out float weight);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_SetListenerWeight(IntPtr studiosystem, int listener, float weight);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_LoadBankFile(IntPtr studiosystem, byte[] filename, LOAD_BANK_FLAGS flags, out IntPtr bank);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_LoadBankMemory(IntPtr studiosystem, IntPtr buffer, int length, LOAD_MEMORY_MODE mode, LOAD_BANK_FLAGS flags, out IntPtr bank);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_LoadBankCustom(IntPtr studiosystem, ref BANK_INFO info, LOAD_BANK_FLAGS flags, out IntPtr bank);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_UnloadAll(IntPtr studiosystem);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_FlushCommands(IntPtr studiosystem);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_FlushSampleLoading(IntPtr studiosystem);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_StartCommandCapture(IntPtr studiosystem, byte[] path, COMMANDCAPTURE_FLAGS flags);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_StopCommandCapture(IntPtr studiosystem);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_LoadCommandReplay(IntPtr studiosystem, byte[] path, COMMANDREPLAY_FLAGS flags, out IntPtr commandReplay);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetBankCount(IntPtr studiosystem, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetBankList(IntPtr studiosystem, IntPtr[] array, int capacity, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetCPUUsage(IntPtr studiosystem, out CPU_USAGE usage);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetBufferUsage(IntPtr studiosystem, out BUFFER_USAGE usage);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_ResetBufferUsage(IntPtr studiosystem);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_SetCallback(IntPtr studiosystem, SYSTEM_CALLBACK callback, SYSTEM_CALLBACK_TYPE callbackmask);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetUserData(IntPtr studiosystem, out IntPtr userdata);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_SetUserData(IntPtr studiosystem, IntPtr userdata);

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
			return this.hasHandle() && FMOD.Studio.System.FMOD_Studio_System_IsValid(this.handle);
		}

		public IntPtr handle;
	}
}
