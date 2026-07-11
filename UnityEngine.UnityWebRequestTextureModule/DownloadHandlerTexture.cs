using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.Networking
{
	/// <summary>
	///   <para>A DownloadHandler subclass specialized for downloading images for use as Texture objects.</para>
	/// </summary>
	[NativeHeader("Modules/UnityWebRequestTexture/Public/DownloadHandlerTexture.h")]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class DownloadHandlerTexture : DownloadHandler
	{
		/// <summary>
		///   <para>Default constructor.</para>
		/// </summary>
		public DownloadHandlerTexture()
		{
			this.InternalCreateTexture(true);
		}

		/// <summary>
		///   <para>Constructor, allows TextureImporter.isReadable property to be set.</para>
		/// </summary>
		/// <param name="readable">Value to set for TextureImporter.isReadable.</param>
		public DownloadHandlerTexture(bool readable)
		{
			this.InternalCreateTexture(readable);
			this.mNonReadable = !readable;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Create(DownloadHandlerTexture obj, bool readable);

		private void InternalCreateTexture(bool readable)
		{
			this.m_Ptr = DownloadHandlerTexture.Create(this, readable);
		}

		/// <summary>
		///   <para>Called by DownloadHandler.data. Returns a copy of the downloaded image data as raw bytes.</para>
		/// </summary>
		/// <returns>
		///   <para>A copy of the downloaded data.</para>
		/// </returns>
		protected override byte[] GetData()
		{
			return DownloadHandler.InternalGetByteArray(this);
		}

		/// <summary>
		///   <para>Returns the downloaded Texture, or null. (Read Only)</para>
		/// </summary>
		public Texture2D texture
		{
			get
			{
				return this.InternalGetTexture();
			}
		}

		private Texture2D InternalGetTexture()
		{
			if (this.mHasTexture)
			{
				if (this.mTexture == null)
				{
					this.mTexture = new Texture2D(2, 2);
					this.mTexture.LoadImage(this.GetData(), this.mNonReadable);
				}
			}
			else if (this.mTexture == null)
			{
				this.mTexture = this.InternalGetTextureNative();
				this.mHasTexture = true;
			}
			return this.mTexture;
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Texture2D InternalGetTextureNative();

		/// <summary>
		///   <para>Returns the downloaded Texture, or null.</para>
		/// </summary>
		/// <param name="www">A finished UnityWebRequest object with DownloadHandlerTexture attached.</param>
		/// <returns>
		///   <para>The same as DownloadHandlerTexture.texture</para>
		/// </returns>
		public static Texture2D GetContent(UnityWebRequest www)
		{
			return DownloadHandler.GetCheckedDownloader<DownloadHandlerTexture>(www).texture;
		}

		private Texture2D mTexture;

		private bool mHasTexture;

		private bool mNonReadable;
	}
}
