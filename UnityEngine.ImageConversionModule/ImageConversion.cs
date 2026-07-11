using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Modules/ImageConversion/ScriptBindings/ImageConversion.bindings.h")]
	public static class ImageConversion
	{
		[NativeMethod(Name = "ImageConversionBindings::EncodeToTGA", IsFreeFunction = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern byte[] EncodeToTGA(this Texture2D tex);

		[NativeMethod(Name = "ImageConversionBindings::EncodeToPNG", IsFreeFunction = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern byte[] EncodeToPNG(this Texture2D tex);

		[NativeMethod(Name = "ImageConversionBindings::EncodeToJPG", IsFreeFunction = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern byte[] EncodeToJPG(this Texture2D tex, int quality);

		public static byte[] EncodeToJPG(this Texture2D tex)
		{
			return tex.EncodeToJPG(75);
		}

		[NativeMethod(Name = "ImageConversionBindings::EncodeToEXR", IsFreeFunction = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern byte[] EncodeToEXR(this Texture2D tex, Texture2D.EXRFlags flags);

		public static byte[] EncodeToEXR(this Texture2D tex)
		{
			return tex.EncodeToEXR(Texture2D.EXRFlags.None);
		}

		[NativeMethod(Name = "ImageConversionBindings::LoadImage", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool LoadImage([NotNull] this Texture2D tex, byte[] data, bool markNonReadable);

		public static bool LoadImage(this Texture2D tex, byte[] data)
		{
			return tex.LoadImage(data, false);
		}
	}
}
