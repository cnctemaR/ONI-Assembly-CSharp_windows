using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.Networking
{
	/// <summary>
	///   <para>MovieTexture has been deprecated. Refer to the new movie playback solution VideoPlayer.</para>
	/// </summary>
	[NativeHeader("Runtime/Video/MovieTexture.h")]
	[NativeHeader("Modules/UnityWebRequestAudio/Public/DownloadHandlerMovieTexture.h")]
	[Obsolete("MovieTexture is deprecated. Use VideoPlayer instead.", false)]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class DownloadHandlerMovieTexture : DownloadHandler
	{
		/// <summary>
		///   <para>MovieTexture has been deprecated. Refer to the new movie playback solution VideoPlayer.</para>
		/// </summary>
		public DownloadHandlerMovieTexture()
		{
			this.InternalCreateDHMovieTexture();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Create(DownloadHandlerMovieTexture obj);

		private void InternalCreateDHMovieTexture()
		{
			this.m_Ptr = DownloadHandlerMovieTexture.Create(this);
		}

		/// <summary>
		///   <para>MovieTexture has been deprecated. Refer to the new movie playback solution VideoPlayer.</para>
		/// </summary>
		/// <returns>
		///   <para>Raw downloaded bytes.</para>
		/// </returns>
		protected override byte[] GetData()
		{
			return DownloadHandler.InternalGetByteArray(this);
		}

		protected override string GetText()
		{
			throw new NotSupportedException("String access is not supported for movies");
		}

		/// <summary>
		///   <para>MovieTexture has been deprecated. Refer to the new movie playback solution VideoPlayer.</para>
		/// </summary>
		public extern MovieTexture movieTexture
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>MovieTexture has been deprecated. Refer to the new movie playback solution VideoPlayer.</para>
		/// </summary>
		/// <param name="uwr">A UnityWebRequest with attached DownloadHandlerMovieTexture.</param>
		/// <returns>
		///   <para>A MovieTexture created out of downloaded bytes.</para>
		/// </returns>
		public static MovieTexture GetContent(UnityWebRequest uwr)
		{
			return DownloadHandler.GetCheckedDownloader<DownloadHandlerMovieTexture>(uwr).movieTexture;
		}
	}
}
