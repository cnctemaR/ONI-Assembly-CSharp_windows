using System;
using System.Runtime.InteropServices;

namespace FMOD.Studio
{
	public struct Bank
	{
		public RESULT getID(out Guid id)
		{
			return Bank.FMOD_Studio_Bank_GetID(this.handle, out id);
		}

		public RESULT getPath(out string path)
		{
			path = null;
			RESULT result2;
			using (StringHelper.ThreadSafeEncoding freeHelper = StringHelper.GetFreeHelper())
			{
				IntPtr intPtr = Marshal.AllocHGlobal(256);
				int num = 0;
				RESULT result = Bank.FMOD_Studio_Bank_GetPath(this.handle, intPtr, 256, out num);
				if (result == RESULT.ERR_TRUNCATED)
				{
					Marshal.FreeHGlobal(intPtr);
					intPtr = Marshal.AllocHGlobal(num);
					result = Bank.FMOD_Studio_Bank_GetPath(this.handle, intPtr, num, out num);
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

		public RESULT unload()
		{
			return Bank.FMOD_Studio_Bank_Unload(this.handle);
		}

		public RESULT loadSampleData()
		{
			return Bank.FMOD_Studio_Bank_LoadSampleData(this.handle);
		}

		public RESULT unloadSampleData()
		{
			return Bank.FMOD_Studio_Bank_UnloadSampleData(this.handle);
		}

		public RESULT getLoadingState(out LOADING_STATE state)
		{
			return Bank.FMOD_Studio_Bank_GetLoadingState(this.handle, out state);
		}

		public RESULT getSampleLoadingState(out LOADING_STATE state)
		{
			return Bank.FMOD_Studio_Bank_GetSampleLoadingState(this.handle, out state);
		}

		public RESULT getStringCount(out int count)
		{
			return Bank.FMOD_Studio_Bank_GetStringCount(this.handle, out count);
		}

		public RESULT getStringInfo(int index, out Guid id, out string path)
		{
			path = null;
			id = Guid.Empty;
			RESULT result2;
			using (StringHelper.ThreadSafeEncoding freeHelper = StringHelper.GetFreeHelper())
			{
				IntPtr intPtr = Marshal.AllocHGlobal(256);
				int num = 0;
				RESULT result = Bank.FMOD_Studio_Bank_GetStringInfo(this.handle, index, out id, intPtr, 256, out num);
				if (result == RESULT.ERR_TRUNCATED)
				{
					Marshal.FreeHGlobal(intPtr);
					intPtr = Marshal.AllocHGlobal(num);
					result = Bank.FMOD_Studio_Bank_GetStringInfo(this.handle, index, out id, intPtr, num, out num);
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

		public RESULT getEventCount(out int count)
		{
			return Bank.FMOD_Studio_Bank_GetEventCount(this.handle, out count);
		}

		public RESULT getEventList(out EventDescription[] array)
		{
			array = null;
			int num;
			RESULT result = Bank.FMOD_Studio_Bank_GetEventCount(this.handle, out num);
			if (result != RESULT.OK)
			{
				return result;
			}
			if (num == 0)
			{
				array = new EventDescription[0];
				return result;
			}
			IntPtr[] array2 = new IntPtr[num];
			int num2;
			result = Bank.FMOD_Studio_Bank_GetEventList(this.handle, array2, num, out num2);
			if (result != RESULT.OK)
			{
				return result;
			}
			if (num2 > num)
			{
				num2 = num;
			}
			array = new EventDescription[num2];
			for (int i = 0; i < num2; i++)
			{
				array[i].handle = array2[i];
			}
			return RESULT.OK;
		}

		public RESULT getBusCount(out int count)
		{
			return Bank.FMOD_Studio_Bank_GetBusCount(this.handle, out count);
		}

		public RESULT getBusList(out Bus[] array)
		{
			array = null;
			int num;
			RESULT result = Bank.FMOD_Studio_Bank_GetBusCount(this.handle, out num);
			if (result != RESULT.OK)
			{
				return result;
			}
			if (num == 0)
			{
				array = new Bus[0];
				return result;
			}
			IntPtr[] array2 = new IntPtr[num];
			int num2;
			result = Bank.FMOD_Studio_Bank_GetBusList(this.handle, array2, num, out num2);
			if (result != RESULT.OK)
			{
				return result;
			}
			if (num2 > num)
			{
				num2 = num;
			}
			array = new Bus[num2];
			for (int i = 0; i < num2; i++)
			{
				array[i].handle = array2[i];
			}
			return RESULT.OK;
		}

		public RESULT getVCACount(out int count)
		{
			return Bank.FMOD_Studio_Bank_GetVCACount(this.handle, out count);
		}

		public RESULT getVCAList(out VCA[] array)
		{
			array = null;
			int num;
			RESULT result = Bank.FMOD_Studio_Bank_GetVCACount(this.handle, out num);
			if (result != RESULT.OK)
			{
				return result;
			}
			if (num == 0)
			{
				array = new VCA[0];
				return result;
			}
			IntPtr[] array2 = new IntPtr[num];
			int num2;
			result = Bank.FMOD_Studio_Bank_GetVCAList(this.handle, array2, num, out num2);
			if (result != RESULT.OK)
			{
				return result;
			}
			if (num2 > num)
			{
				num2 = num;
			}
			array = new VCA[num2];
			for (int i = 0; i < num2; i++)
			{
				array[i].handle = array2[i];
			}
			return RESULT.OK;
		}

		public RESULT getUserData(out IntPtr userdata)
		{
			return Bank.FMOD_Studio_Bank_GetUserData(this.handle, out userdata);
		}

		public RESULT setUserData(IntPtr userdata)
		{
			return Bank.FMOD_Studio_Bank_SetUserData(this.handle, userdata);
		}

		[DllImport("fmodstudio")]
		private static extern bool FMOD_Studio_Bank_IsValid(IntPtr bank);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetID(IntPtr bank, out Guid id);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetPath(IntPtr bank, IntPtr path, int size, out int retrieved);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_Unload(IntPtr bank);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_LoadSampleData(IntPtr bank);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_UnloadSampleData(IntPtr bank);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetLoadingState(IntPtr bank, out LOADING_STATE state);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetSampleLoadingState(IntPtr bank, out LOADING_STATE state);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetStringCount(IntPtr bank, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetStringInfo(IntPtr bank, int index, out Guid id, IntPtr path, int size, out int retrieved);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetEventCount(IntPtr bank, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetEventList(IntPtr bank, IntPtr[] array, int capacity, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetBusCount(IntPtr bank, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetBusList(IntPtr bank, IntPtr[] array, int capacity, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetVCACount(IntPtr bank, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetVCAList(IntPtr bank, IntPtr[] array, int capacity, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetUserData(IntPtr bank, out IntPtr userdata);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_SetUserData(IntPtr bank, IntPtr userdata);

		public Bank(IntPtr ptr)
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
			return this.hasHandle() && Bank.FMOD_Studio_Bank_IsValid(this.handle);
		}

		public IntPtr handle;
	}
}
