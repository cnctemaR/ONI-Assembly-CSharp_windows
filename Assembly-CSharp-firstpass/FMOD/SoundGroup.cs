using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD
{
	public class SoundGroup : HandleBase
	{
		public SoundGroup(IntPtr raw)
			: base(raw)
		{
		}

		public RESULT release()
		{
			RESULT result = SoundGroup.FMOD5_SoundGroup_Release(base.getRaw());
			if (result == RESULT.OK)
			{
				this.rawPtr = IntPtr.Zero;
			}
			return result;
		}

		public RESULT getSystemObject(out FMOD.System system)
		{
			system = null;
			IntPtr intPtr;
			RESULT result = SoundGroup.FMOD5_SoundGroup_GetSystemObject(this.rawPtr, out intPtr);
			system = new FMOD.System(intPtr);
			return result;
		}

		public RESULT setMaxAudible(int maxaudible)
		{
			return SoundGroup.FMOD5_SoundGroup_SetMaxAudible(this.rawPtr, maxaudible);
		}

		public RESULT getMaxAudible(out int maxaudible)
		{
			return SoundGroup.FMOD5_SoundGroup_GetMaxAudible(this.rawPtr, out maxaudible);
		}

		public RESULT setMaxAudibleBehavior(SOUNDGROUP_BEHAVIOR behavior)
		{
			return SoundGroup.FMOD5_SoundGroup_SetMaxAudibleBehavior(this.rawPtr, behavior);
		}

		public RESULT getMaxAudibleBehavior(out SOUNDGROUP_BEHAVIOR behavior)
		{
			return SoundGroup.FMOD5_SoundGroup_GetMaxAudibleBehavior(this.rawPtr, out behavior);
		}

		public RESULT setMuteFadeSpeed(float speed)
		{
			return SoundGroup.FMOD5_SoundGroup_SetMuteFadeSpeed(this.rawPtr, speed);
		}

		public RESULT getMuteFadeSpeed(out float speed)
		{
			return SoundGroup.FMOD5_SoundGroup_GetMuteFadeSpeed(this.rawPtr, out speed);
		}

		public RESULT setVolume(float volume)
		{
			return SoundGroup.FMOD5_SoundGroup_SetVolume(this.rawPtr, volume);
		}

		public RESULT getVolume(out float volume)
		{
			return SoundGroup.FMOD5_SoundGroup_GetVolume(this.rawPtr, out volume);
		}

		public RESULT stop()
		{
			return SoundGroup.FMOD5_SoundGroup_Stop(this.rawPtr);
		}

		public RESULT getName(StringBuilder name, int namelen)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(name.Capacity);
			RESULT result = SoundGroup.FMOD5_SoundGroup_GetName(this.rawPtr, intPtr, namelen);
			StringMarshalHelper.NativeToBuilder(name, intPtr);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public RESULT getNumSounds(out int numsounds)
		{
			return SoundGroup.FMOD5_SoundGroup_GetNumSounds(this.rawPtr, out numsounds);
		}

		public RESULT getSound(int index, out Sound sound)
		{
			sound = null;
			IntPtr intPtr;
			RESULT result = SoundGroup.FMOD5_SoundGroup_GetSound(this.rawPtr, index, out intPtr);
			sound = new Sound(intPtr);
			return result;
		}

		public RESULT getNumPlaying(out int numplaying)
		{
			return SoundGroup.FMOD5_SoundGroup_GetNumPlaying(this.rawPtr, out numplaying);
		}

		public RESULT setUserData(IntPtr userdata)
		{
			return SoundGroup.FMOD5_SoundGroup_SetUserData(this.rawPtr, userdata);
		}

		public RESULT getUserData(out IntPtr userdata)
		{
			return SoundGroup.FMOD5_SoundGroup_GetUserData(this.rawPtr, out userdata);
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
	}
}
