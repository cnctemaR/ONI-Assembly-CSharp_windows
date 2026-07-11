using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	public struct Reverb3D
	{
		public RESULT release()
		{
			return Reverb3D.FMOD5_Reverb3D_Release(this.handle);
		}

		public RESULT set3DAttributes(ref VECTOR position, float mindistance, float maxdistance)
		{
			return Reverb3D.FMOD5_Reverb3D_Set3DAttributes(this.handle, ref position, mindistance, maxdistance);
		}

		public RESULT get3DAttributes(ref VECTOR position, ref float mindistance, ref float maxdistance)
		{
			return Reverb3D.FMOD5_Reverb3D_Get3DAttributes(this.handle, ref position, ref mindistance, ref maxdistance);
		}

		public RESULT setProperties(ref REVERB_PROPERTIES properties)
		{
			return Reverb3D.FMOD5_Reverb3D_SetProperties(this.handle, ref properties);
		}

		public RESULT getProperties(ref REVERB_PROPERTIES properties)
		{
			return Reverb3D.FMOD5_Reverb3D_GetProperties(this.handle, ref properties);
		}

		public RESULT setActive(bool active)
		{
			return Reverb3D.FMOD5_Reverb3D_SetActive(this.handle, active);
		}

		public RESULT getActive(out bool active)
		{
			return Reverb3D.FMOD5_Reverb3D_GetActive(this.handle, out active);
		}

		public RESULT setUserData(IntPtr userdata)
		{
			return Reverb3D.FMOD5_Reverb3D_SetUserData(this.handle, userdata);
		}

		public RESULT getUserData(out IntPtr userdata)
		{
			return Reverb3D.FMOD5_Reverb3D_GetUserData(this.handle, out userdata);
		}

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Reverb3D_Release(IntPtr reverb3d);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Reverb3D_Set3DAttributes(IntPtr reverb3d, ref VECTOR position, float mindistance, float maxdistance);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Reverb3D_Get3DAttributes(IntPtr reverb3d, ref VECTOR position, ref float mindistance, ref float maxdistance);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Reverb3D_SetProperties(IntPtr reverb3d, ref REVERB_PROPERTIES properties);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Reverb3D_GetProperties(IntPtr reverb3d, ref REVERB_PROPERTIES properties);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Reverb3D_SetActive(IntPtr reverb3d, bool active);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Reverb3D_GetActive(IntPtr reverb3d, out bool active);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Reverb3D_SetUserData(IntPtr reverb3d, IntPtr userdata);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Reverb3D_GetUserData(IntPtr reverb3d, out IntPtr userdata);

		public Reverb3D(IntPtr ptr)
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
