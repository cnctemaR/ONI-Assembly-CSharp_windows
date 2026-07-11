using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>Class with utility methods and extension methods to deal with converting image data from or to PNG and JPEG formats.</para>
	/// </summary>
	[NativeHeader("Modules/ImageConversion/ScriptBindings/ImageConversion.bindings.h")]
	public static class ImageConversion
	{
		/// <summary>
		///   <para>Encodes this texture into PNG format.</para>
		/// </summary>
		/// <param name="tex">The texture to convert.</param>
		[NativeMethod(Name = "ImageConversionBindings::EncodeToPNG", IsFreeFunction = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern byte[] EncodeToPNG(this Texture2D tex);

		/// <summary>
		///   <para>Encodes this texture into JPG format.</para>
		/// </summary>
		/// <param name="tex">Text texture to convert.</param>
		/// <param name="quality">JPG quality to encode with, 1..100 (default 75).</param>
		[NativeMethod(Name = "ImageConversionBindings::EncodeToJPG", IsFreeFunction = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern byte[] EncodeToJPG(this Texture2D tex, int quality);

		/// <summary>
		///   <para>Encodes this texture into JPG format.</para>
		/// </summary>
		/// <param name="tex">Text texture to convert.</param>
		/// <param name="quality">JPG quality to encode with, 1..100 (default 75).</param>
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

		/// <summary>
		///   <para>Loads PNG/JPG image byte array into a texture.</para>
		/// </summary>
		/// <param name="data">The byte array containing the image data to load.</param>
		/// <param name="markNonReadable">Set to false by default, pass true to optionally mark the texture as non-readable.</param>
		/// <param name="tex">The texture to load the image into.</param>
		/// <returns>
		///   <para>Returns true if the data can be loaded, false otherwise.</para>
		/// </returns>
		[NativeMethod(Name = "ImageConversionBindings::LoadImage", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool LoadImage(this Texture2D tex, byte[] data, bool markNonReadable);

		public static bool LoadImage(this Texture2D tex, byte[] data)
		{
			return tex.LoadImage(data, false);
		}
	}
}
