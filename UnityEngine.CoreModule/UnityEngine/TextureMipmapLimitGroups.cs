using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[StaticAccessor("GetQualitySettings()", StaticAccessorType.Dot)]
	[NativeHeader("Runtime/Graphics/QualitySettings.h")]
	public static class TextureMipmapLimitGroups
	{
		[NativeName("GetTextureMipmapLimitGroupNames")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetGroups();

		[NativeName("HasTextureMipmapLimitGroup")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasGroup([NotNull("ArgumentNullException")] string groupName);
	}
}
