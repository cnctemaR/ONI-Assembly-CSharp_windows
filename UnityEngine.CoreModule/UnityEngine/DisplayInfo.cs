using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeType("Runtime/Graphics/DisplayInfo.h")]
	[UsedByNativeCode]
	public struct DisplayInfo : IEquatable<DisplayInfo>
	{
		public Resolution[] resolutions
		{
			get
			{
				throw new NotSupportedException("DisplayInfo.resolutions is currently not supported on this platform.");
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(DisplayInfo other)
		{
			return this.handle == other.handle && this.width == other.width && this.height == other.height && this.refreshRate.Equals(other.refreshRate) && this.workArea.Equals(other.workArea) && this.name == other.name && this.physicalDpi == other.physicalDpi;
		}

		public static void GetLayout(List<DisplayInfo> displayLayout)
		{
			Screen.GetDisplayLayout(displayLayout);
		}

		private static Resolution[] GetResolutions(DisplayInfo displayInfo)
		{
			throw new NotSupportedException("DisplayInfo.GetResolutions() is not supported on this platform.");
		}

		[NativeConditional("PLATFORM_SUPPORTS_DISPLAYINFO_API")]
		[FreeFunction("DisplayInfoScripting::GetLayout")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLayoutImpl(List<DisplayInfo> displayLayout);

		[NativeConditional("PLATFORM_SUPPORTS_DISPLAYINFO_API")]
		[FreeFunction("DisplayInfoScripting::GetResolutions")]
		private static Resolution[] GetResolutionsImpl(ulong handle)
		{
			Resolution[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				DisplayInfo.GetResolutionsImpl_Injected(handle, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				Resolution[] array;
				blittableArrayWrapper.Unmarshal<Resolution>(ref array);
				array2 = array;
			}
			return array2;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetResolutionsImpl_Injected(ulong handle, out BlittableArrayWrapper ret);

		[RequiredMember]
		internal ulong handle;

		[RequiredMember]
		public int width;

		[RequiredMember]
		public int height;

		[RequiredMember]
		public RefreshRate refreshRate;

		[RequiredMember]
		public RectInt workArea;

		[RequiredMember]
		public string name;

		[NativeName("dpi")]
		[RequiredMember]
		public float physicalDpi;
	}
}
