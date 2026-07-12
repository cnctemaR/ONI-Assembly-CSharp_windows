using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeType("Runtime/Graphics/DisplayInfo.h")]
	[UsedByNativeCode]
	public struct DisplayInfo : IEquatable<DisplayInfo>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(DisplayInfo other)
		{
			return this.handle == other.handle && this.width == other.width && this.height == other.height && this.refreshRate.Equals(other.refreshRate) && this.workArea.Equals(other.workArea) && this.name == other.name;
		}

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
	}
}
