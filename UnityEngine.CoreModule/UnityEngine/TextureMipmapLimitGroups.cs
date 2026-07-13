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
		public unsafe static bool HasGroup([NotNull] string groupName)
		{
			if (groupName == null)
			{
				ThrowHelper.ThrowArgumentNullException(groupName, "groupName");
			}
			bool flag;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(groupName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = groupName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = TextureMipmapLimitGroups.HasGroup_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasGroup_Injected(ref ManagedSpanWrapper groupName);
	}
}
