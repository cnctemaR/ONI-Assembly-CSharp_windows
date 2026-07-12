using System;
using System.Runtime.InteropServices;

namespace FMOD.Studio
{
	public struct VCA
	{
		public RESULT getID(out GUID id)
		{
			return VCA.FMOD_Studio_VCA_GetID(this.handle, out id);
		}

		public RESULT getPath(out string path)
		{
			path = null;
			RESULT result2;
			using (StringHelper.ThreadSafeEncoding freeHelper = StringHelper.GetFreeHelper())
			{
				IntPtr intPtr = Marshal.AllocHGlobal(256);
				int num = 0;
				RESULT result = VCA.FMOD_Studio_VCA_GetPath(this.handle, intPtr, 256, out num);
				if (result == RESULT.ERR_TRUNCATED)
				{
					Marshal.FreeHGlobal(intPtr);
					intPtr = Marshal.AllocHGlobal(num);
					result = VCA.FMOD_Studio_VCA_GetPath(this.handle, intPtr, num, out num);
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

		public RESULT getVolume(out float volume)
		{
			float num;
			return this.getVolume(out volume, out num);
		}

		public RESULT getVolume(out float volume, out float finalvolume)
		{
			return VCA.FMOD_Studio_VCA_GetVolume(this.handle, out volume, out finalvolume);
		}

		public RESULT setVolume(float volume)
		{
			return VCA.FMOD_Studio_VCA_SetVolume(this.handle, volume);
		}

		[DllImport("fmodstudio")]
		private static extern bool FMOD_Studio_VCA_IsValid(IntPtr vca);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_VCA_GetID(IntPtr vca, out GUID id);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_VCA_GetPath(IntPtr vca, IntPtr path, int size, out int retrieved);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_VCA_GetVolume(IntPtr vca, out float volume, out float finalvolume);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_VCA_SetVolume(IntPtr vca, float volume);

		public VCA(IntPtr ptr)
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
			return this.hasHandle() && VCA.FMOD_Studio_VCA_IsValid(this.handle);
		}

		public IntPtr handle;
	}
}
