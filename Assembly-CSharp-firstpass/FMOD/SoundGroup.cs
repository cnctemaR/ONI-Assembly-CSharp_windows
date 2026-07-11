using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	public struct SoundGroup
	{
		public RESULT release()
		{
			return SoundGroup.FMOD5_SoundGroup_Release(this.handle);
		}

		public RESULT getSystemObject(out FMOD.System system)
		{
			return SoundGroup.FMOD5_SoundGroup_GetSystemObject(this.handle, out system.handle);
		}

		public RESULT setMaxAudible(int maxaudible)
		{
			return SoundGroup.FMOD5_SoundGroup_SetMaxAudible(this.handle, maxaudible);
		}

		public RESULT getMaxAudible(out int maxaudible)
		{
			return SoundGroup.FMOD5_SoundGroup_GetMaxAudible(this.handle, out maxaudible);
		}

		public RESULT setMaxAudibleBehavior(SOUNDGROUP_BEHAVIOR behavior)
		{
			return SoundGroup.FMOD5_SoundGroup_SetMaxAudibleBehavior(this.handle, behavior);
		}

		public RESULT getMaxAudibleBehavior(out SOUNDGROUP_BEHAVIOR behavior)
		{
			return SoundGroup.FMOD5_SoundGroup_GetMaxAudibleBehavior(this.handle, out behavior);
		}

		public RESULT setMuteFadeSpeed(float speed)
		{
			return SoundGroup.FMOD5_SoundGroup_SetMuteFadeSpeed(this.handle, speed);
		}

		public RESULT getMuteFadeSpeed(out float speed)
		{
			return SoundGroup.FMOD5_SoundGroup_GetMuteFadeSpeed(this.handle, out speed);
		}

		public RESULT setVolume(float volume)
		{
			return SoundGroup.FMOD5_SoundGroup_SetVolume(this.handle, volume);
		}

		public RESULT getVolume(out float volume)
		{
			return SoundGroup.FMOD5_SoundGroup_GetVolume(this.handle, out volume);
		}

		public RESULT stop()
		{
			return SoundGroup.FMOD5_SoundGroup_Stop(this.handle);
		}

		public RESULT getName(out string name, int namelen)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(namelen);
			RESULT result = SoundGroup.FMOD5_SoundGroup_GetName(this.handle, intPtr, namelen);
			using (StringHelper.ThreadSafeEncoding freeHelper = StringHelper.GetFreeHelper())
			{
				name = freeHelper.stringFromNative(intPtr);
			}
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public RESULT getNumSounds(out int numsounds)
		{
			return SoundGroup.FMOD5_SoundGroup_GetNumSounds(this.handle, out numsounds);
		}

		public RESULT getSound(int index, out Sound sound)
		{
			return SoundGroup.FMOD5_SoundGroup_GetSound(this.handle, index, out sound.handle);
		}

		public RESULT getNumPlaying(out int numplaying)
		{
			return SoundGroup.FMOD5_SoundGroup_GetNumPlaying(this.handle, out numplaying);
		}

		public RESULT setUserData(IntPtr userdata)
		{
			return SoundGroup.FMOD5_SoundGroup_SetUserData(this.handle, userdata);
		}

		public RESULT getUserData(out IntPtr userdata)
		{
			return SoundGroup.FMOD5_SoundGroup_GetUserData(this.handle, out userdata);
		}

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_SoundGroup_Release(IntPtr soundgroup);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_SoundGroup_GetSystemObject(IntPtr soundgroup, out IntPtr system);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_SoundGroup_SetMaxAudible(IntPtr soundgroup, int maxaudible);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_SoundGroup_GetMaxAudible(IntPtr soundgroup, out int maxaudible);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_SoundGroup_SetMaxAudibleBehavior(IntPtr soundgroup, SOUNDGROUP_BEHAVIOR behavior);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_SoundGroup_GetMaxAudibleBehavior(IntPtr soundgroup, out SOUNDGROUP_BEHAVIOR behavior);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_SoundGroup_SetMuteFadeSpeed(IntPtr soundgroup, float speed);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_SoundGroup_GetMuteFadeSpeed(IntPtr soundgroup, out float speed);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_SoundGroup_SetVolume(IntPtr soundgroup, float volume);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_SoundGroup_GetVolume(IntPtr soundgroup, out float volume);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_SoundGroup_Stop(IntPtr soundgroup);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_SoundGroup_GetName(IntPtr soundgroup, IntPtr name, int namelen);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_SoundGroup_GetNumSounds(IntPtr soundgroup, out int numsounds);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_SoundGroup_GetSound(IntPtr soundgroup, int index, out IntPtr sound);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_SoundGroup_GetNumPlaying(IntPtr soundgroup, out int numplaying);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_SoundGroup_SetUserData(IntPtr soundgroup, IntPtr userdata);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_SoundGroup_GetUserData(IntPtr soundgroup, out IntPtr userdata);

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
