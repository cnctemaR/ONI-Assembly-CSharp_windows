using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.TextCore.Text
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static class TextGenerationInfo
	{
		public static int CurrentGenerationIteration { get; private set; }

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr Create(bool isPermanent);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Destroy(IntPtr ptr);

		public static void OnRepaintEnd()
		{
			TextGenerationInfo.CurrentGenerationIteration++;
			TextGenerationInfo.DestroyAllTempAllocations();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DestroyAllTempAllocations();

		[ThreadSafe]
		public static TextRenderingIndices GetTextRenderingIndices(IntPtr ptr, int glyphIndex)
		{
			TextRenderingIndices textRenderingIndices;
			TextGenerationInfo.GetTextRenderingIndices_Injected(ptr, glyphIndex, out textRenderingIndices);
			return textRenderingIndices;
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetGlyphCount(IntPtr ptr);

		public static NativeTextInfo GetTextInfo(IntPtr ptr)
		{
			NativeTextInfo nativeTextInfo;
			TextGenerationInfo.GetTextInfo_Injected(ptr, out nativeTextInfo);
			return nativeTextInfo;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetTextRenderingIndices_Injected(IntPtr ptr, int glyphIndex, out TextRenderingIndices ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetTextInfo_Injected(IntPtr ptr, out NativeTextInfo ret);
	}
}
