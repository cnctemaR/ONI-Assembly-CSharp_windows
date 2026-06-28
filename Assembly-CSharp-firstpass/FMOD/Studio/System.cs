using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD.Studio
{
	public class System : HandleBase
	{
		public System(IntPtr raw)
			: base(raw)
		{
		}

		public static RESULT create(out FMOD.Studio.System studiosystem)
		{
			studiosystem = null;
			IntPtr intPtr;
			RESULT result = FMOD.Studio.System.FMOD_Studio_System_Create(out intPtr, 67606U);
			RESULT result2;
			if (result != RESULT.OK)
			{
				result2 = result;
			}
			else
			{
				studiosystem = new FMOD.Studio.System(intPtr);
				result2 = result;
			}
			return result2;
		}

		public RESULT setAdvancedSettings(ADVANCEDSETTINGS settings)
		{
			settings.cbSize = Marshal.SizeOf(typeof(ADVANCEDSETTINGS));
			return FMOD.Studio.System.FMOD_Studio_System_SetAdvancedSettings(this.rawPtr, ref settings);
		}

		public RESULT getAdvancedSettings(out ADVANCEDSETTINGS settings)
		{
			settings.cbSize = Marshal.SizeOf(typeof(ADVANCEDSETTINGS));
			return FMOD.Studio.System.FMOD_Studio_System_GetAdvancedSettings(this.rawPtr, out settings);
		}

		public RESULT initialize(int maxchannels, INITFLAGS studioFlags, INITFLAGS flags, IntPtr extradriverdata)
		{
			return FMOD.Studio.System.FMOD_Studio_System_Initialize(this.rawPtr, maxchannels, studioFlags, flags, extradriverdata);
		}

		public RESULT release()
		{
			return FMOD.Studio.System.FMOD_Studio_System_Release(this.rawPtr);
		}

		public RESULT update()
		{
			return FMOD.Studio.System.FMOD_Studio_System_Update(this.rawPtr);
		}

		public RESULT getLowLevelSystem(out FMOD.System system)
		{
			system = null;
			IntPtr intPtr = 0;
			RESULT result = FMOD.Studio.System.FMOD_Studio_System_GetLowLevelSystem(this.rawPtr, out intPtr);
			RESULT result2;
			if (result != RESULT.OK)
			{
				result2 = result;
			}
			else
			{
				system = new FMOD.System(intPtr);
				result2 = result;
			}
			return result2;
		}

		public RESULT getEvent(string path, out EventDescription _event)
		{
			_event = null;
			IntPtr intPtr = 0;
			RESULT result = FMOD.Studio.System.FMOD_Studio_System_GetEvent(this.rawPtr, Encoding.UTF8.GetBytes(path + '\0'), out intPtr);
			RESULT result2;
			if (result != RESULT.OK)
			{
				result2 = result;
			}
			else
			{
				_event = new EventDescription(intPtr);
				result2 = result;
			}
			return result2;
		}

		public RESULT getBus(string path, out Bus bus)
		{
			bus = null;
			IntPtr intPtr = 0;
			RESULT result = FMOD.Studio.System.FMOD_Studio_System_GetBus(this.rawPtr, Encoding.UTF8.GetBytes(path + '\0'), out intPtr);
			RESULT result2;
			if (result != RESULT.OK)
			{
				result2 = result;
			}
			else
			{
				bus = new Bus(intPtr);
				result2 = result;
			}
			return result2;
		}

		public RESULT getVCA(string path, out VCA vca)
		{
			vca = null;
			IntPtr intPtr = 0;
			RESULT result = FMOD.Studio.System.FMOD_Studio_System_GetVCA(this.rawPtr, Encoding.UTF8.GetBytes(path + '\0'), out intPtr);
			RESULT result2;
			if (result != RESULT.OK)
			{
				result2 = result;
			}
			else
			{
				vca = new VCA(intPtr);
				result2 = result;
			}
			return result2;
		}

		public RESULT getBank(string path, out Bank bank)
		{
			bank = null;
			IntPtr intPtr = 0;
			RESULT result = FMOD.Studio.System.FMOD_Studio_System_GetBank(this.rawPtr, Encoding.UTF8.GetBytes(path + '\0'), out intPtr);
			RESULT result2;
			if (result != RESULT.OK)
			{
				result2 = result;
			}
			else
			{
				bank = new Bank(intPtr);
				result2 = result;
			}
			return result2;
		}

		public RESULT getEventByID(Guid guid, out EventDescription _event)
		{
			_event = null;
			IntPtr intPtr = 0;
			RESULT result = FMOD.Studio.System.FMOD_Studio_System_GetEventByID(this.rawPtr, guid.ToByteArray(), out intPtr);
			RESULT result2;
			if (result != RESULT.OK)
			{
				result2 = result;
			}
			else
			{
				_event = new EventDescription(intPtr);
				result2 = result;
			}
			return result2;
		}

		public RESULT getBusByID(Guid guid, out Bus bus)
		{
			bus = null;
			IntPtr intPtr = 0;
			RESULT result = FMOD.Studio.System.FMOD_Studio_System_GetBusByID(this.rawPtr, guid.ToByteArray(), out intPtr);
			RESULT result2;
			if (result != RESULT.OK)
			{
				result2 = result;
			}
			else
			{
				bus = new Bus(intPtr);
				result2 = result;
			}
			return result2;
		}

		public RESULT getVCAByID(Guid guid, out VCA vca)
		{
			vca = null;
			IntPtr intPtr = 0;
			RESULT result = FMOD.Studio.System.FMOD_Studio_System_GetVCAByID(this.rawPtr, guid.ToByteArray(), out intPtr);
			RESULT result2;
			if (result != RESULT.OK)
			{
				result2 = result;
			}
			else
			{
				vca = new VCA(intPtr);
				result2 = result;
			}
			return result2;
		}

		public RESULT getBankByID(Guid guid, out Bank bank)
		{
			bank = null;
			IntPtr intPtr = 0;
			RESULT result = FMOD.Studio.System.FMOD_Studio_System_GetBankByID(this.rawPtr, guid.ToByteArray(), out intPtr);
			RESULT result2;
			if (result != RESULT.OK)
			{
				result2 = result;
			}
			else
			{
				bank = new Bank(intPtr);
				result2 = result;
			}
			return result2;
		}

		public RESULT getSoundInfo(string key, out SOUND_INFO info)
		{
			int num = Marshal.SizeOf(typeof(SOUND_INFO_INTERNAL));
			IntPtr intPtr = Marshal.AllocHGlobal(num);
			RESULT result = FMOD.Studio.System.FMOD_Studio_System_GetSoundInfo(this.rawPtr, Encoding.UTF8.GetBytes(key + '\0'), intPtr);
			RESULT result2;
			if (result != RESULT.OK)
			{
				Marshal.FreeHGlobal(intPtr);
				info = new SOUND_INFO();
				result2 = result;
			}
			else
			{
				((SOUND_INFO_INTERNAL)Marshal.PtrToStructure(intPtr, typeof(SOUND_INFO_INTERNAL))).assign(out info);
				Marshal.FreeHGlobal(intPtr);
				result2 = result;
			}
			return result2;
		}

		public RESULT lookupID(string path, out Guid guid)
		{
			byte[] array = new byte[16];
			RESULT result = FMOD.Studio.System.FMOD_Studio_System_LookupID(this.rawPtr, Encoding.UTF8.GetBytes(path + '\0'), array);
			guid = new Guid(array);
			return result;
		}

		public RESULT lookupPath(Guid guid, out string path)
		{
			path = null;
			byte[] array = new byte[256];
			int num = 0;
			RESULT result = FMOD.Studio.System.FMOD_Studio_System_LookupPath(this.rawPtr, guid.ToByteArray(), array, array.Length, out num);
			if (result == RESULT.ERR_TRUNCATED)
			{
				array = new byte[num];
				result = FMOD.Studio.System.FMOD_Studio_System_LookupPath(this.rawPtr, guid.ToByteArray(), array, array.Length, out num);
			}
			if (result == RESULT.OK)
			{
				path = Encoding.UTF8.GetString(array, 0, num - 1);
			}
			return result;
		}

		public RESULT getNumListeners(out int numlisteners)
		{
			return FMOD.Studio.System.FMOD_Studio_System_GetNumListeners(this.rawPtr, out numlisteners);
		}

		public RESULT setNumListeners(int numlisteners)
		{
			return FMOD.Studio.System.FMOD_Studio_System_SetNumListeners(this.rawPtr, numlisteners);
		}

		public RESULT getListenerAttributes(int listener, out ATTRIBUTES_3D attributes)
		{
			return FMOD.Studio.System.FMOD_Studio_System_GetListenerAttributes(this.rawPtr, listener, out attributes);
		}

		public RESULT setListenerAttributes(int listener, ATTRIBUTES_3D attributes)
		{
			return FMOD.Studio.System.FMOD_Studio_System_SetListenerAttributes(this.rawPtr, listener, ref attributes);
		}

		public RESULT loadBankFile(string name, LOAD_BANK_FLAGS flags, out Bank bank)
		{
			bank = null;
			IntPtr intPtr = 0;
			RESULT result = FMOD.Studio.System.FMOD_Studio_System_LoadBankFile(this.rawPtr, Encoding.UTF8.GetBytes(name + '\0'), flags, out intPtr);
			RESULT result2;
			if (result != RESULT.OK)
			{
				result2 = result;
			}
			else
			{
				bank = new Bank(intPtr);
				result2 = result;
			}
			return result2;
		}

		public RESULT loadBankMemory(byte[] buffer, LOAD_BANK_FLAGS flags, out Bank bank)
		{
			bank = null;
			IntPtr intPtr = 0;
			GCHandle gchandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
			IntPtr intPtr2 = gchandle.AddrOfPinnedObject();
			RESULT result = FMOD.Studio.System.FMOD_Studio_System_LoadBankMemory(this.rawPtr, intPtr2, buffer.Length, LOAD_MEMORY_MODE.LOAD_MEMORY, flags, out intPtr);
			gchandle.Free();
			RESULT result2;
			if (result != RESULT.OK)
			{
				result2 = result;
			}
			else
			{
				bank = new Bank(intPtr);
				result2 = result;
			}
			return result2;
		}

		public RESULT loadBankCustom(BANK_INFO info, LOAD_BANK_FLAGS flags, out Bank bank)
		{
			bank = null;
			info.size = Marshal.SizeOf(info);
			IntPtr intPtr = 0;
			RESULT result = FMOD.Studio.System.FMOD_Studio_System_LoadBankCustom(this.rawPtr, ref info, flags, out intPtr);
			RESULT result2;
			if (result != RESULT.OK)
			{
				result2 = result;
			}
			else
			{
				bank = new Bank(intPtr);
				result2 = result;
			}
			return result2;
		}

		public RESULT unloadAll()
		{
			return FMOD.Studio.System.FMOD_Studio_System_UnloadAll(this.rawPtr);
		}

		public RESULT flushCommands()
		{
			return FMOD.Studio.System.FMOD_Studio_System_FlushCommands(this.rawPtr);
		}

		public RESULT flushSampleLoading()
		{
			return FMOD.Studio.System.FMOD_Studio_System_FlushSampleLoading(this.rawPtr);
		}

		public RESULT startCommandCapture(string path, COMMANDCAPTURE_FLAGS flags)
		{
			return FMOD.Studio.System.FMOD_Studio_System_StartCommandCapture(this.rawPtr, Encoding.UTF8.GetBytes(path + '\0'), flags);
		}

		public RESULT stopCommandCapture()
		{
			return FMOD.Studio.System.FMOD_Studio_System_StopCommandCapture(this.rawPtr);
		}

		public RESULT loadCommandReplay(string path, COMMANDREPLAY_FLAGS flags, out CommandReplay replay)
		{
			replay = null;
			IntPtr intPtr = 0;
			RESULT result = FMOD.Studio.System.FMOD_Studio_System_LoadCommandReplay(this.rawPtr, Encoding.UTF8.GetBytes(path + '\0'), flags, out intPtr);
			if (result == RESULT.OK)
			{
				replay = new CommandReplay(intPtr);
			}
			return result;
		}

		public RESULT getBankCount(out int count)
		{
			return FMOD.Studio.System.FMOD_Studio_System_GetBankCount(this.rawPtr, out count);
		}

		public RESULT getBankList(out Bank[] array)
		{
			array = null;
			int num;
			RESULT result = FMOD.Studio.System.FMOD_Studio_System_GetBankCount(this.rawPtr, out num);
			RESULT result2;
			if (result != RESULT.OK)
			{
				result2 = result;
			}
			else if (num == 0)
			{
				array = new Bank[0];
				result2 = result;
			}
			else
			{
				IntPtr[] array2 = new IntPtr[num];
				int num2;
				result = FMOD.Studio.System.FMOD_Studio_System_GetBankList(this.rawPtr, array2, num, out num2);
				if (result != RESULT.OK)
				{
					result2 = result;
				}
				else
				{
					if (num2 > num)
					{
						num2 = num;
					}
					array = new Bank[num2];
					for (int i = 0; i < num2; i++)
					{
						array[i] = new Bank(array2[i]);
					}
					result2 = RESULT.OK;
				}
			}
			return result2;
		}

		public RESULT getCPUUsage(out CPU_USAGE usage)
		{
			return FMOD.Studio.System.FMOD_Studio_System_GetCPUUsage(this.rawPtr, out usage);
		}

		public RESULT getBufferUsage(out BUFFER_USAGE usage)
		{
			return FMOD.Studio.System.FMOD_Studio_System_GetBufferUsage(this.rawPtr, out usage);
		}

		public RESULT resetBufferUsage()
		{
			return FMOD.Studio.System.FMOD_Studio_System_ResetBufferUsage(this.rawPtr);
		}

		public RESULT setCallback(SYSTEM_CALLBACK callback, SYSTEM_CALLBACK_TYPE callbackmask = SYSTEM_CALLBACK_TYPE.ALL)
		{
			return FMOD.Studio.System.FMOD_Studio_System_SetCallback(this.rawPtr, callback, callbackmask);
		}

		public RESULT getUserData(out IntPtr userData)
		{
			return FMOD.Studio.System.FMOD_Studio_System_GetUserData(this.rawPtr, out userData);
		}

		public RESULT setUserData(IntPtr userData)
		{
			return FMOD.Studio.System.FMOD_Studio_System_SetUserData(this.rawPtr, userData);
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
		private static extern RESULT FMOD_Studio_System_GetEventByID(IntPtr studiosystem, byte[] guid, out IntPtr description);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetBusByID(IntPtr studiosystem, byte[] guid, out IntPtr bus);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetVCAByID(IntPtr studiosystem, byte[] guid, out IntPtr vca);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetBankByID(IntPtr studiosystem, byte[] guid, out IntPtr bank);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetSoundInfo(IntPtr studiosystem, byte[] key, IntPtr info);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_LookupID(IntPtr studiosystem, byte[] path, [Out] byte[] guid);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_LookupPath(IntPtr studiosystem, byte[] guid, [Out] byte[] path, int size, out int retrieved);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetNumListeners(IntPtr studiosystem, out int numlisteners);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_SetNumListeners(IntPtr studiosystem, int numlisteners);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetListenerAttributes(IntPtr studiosystem, int listener, out ATTRIBUTES_3D attributes);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_SetListenerAttributes(IntPtr studiosystem, int listener, ref ATTRIBUTES_3D attributes);

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
		private static extern RESULT FMOD_Studio_System_GetUserData(IntPtr studiosystem, out IntPtr userData);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_SetUserData(IntPtr studiosystem, IntPtr userData);

		protected override bool isValidInternal()
		{
			return FMOD.Studio.System.FMOD_Studio_System_IsValid(this.rawPtr);
		}
	}
}
