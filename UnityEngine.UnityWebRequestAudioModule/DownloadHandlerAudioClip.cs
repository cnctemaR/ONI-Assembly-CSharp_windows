using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.Networking
{
	/// <summary>
	///   <para>A DownloadHandler subclass specialized for downloading audio data for use as AudioClip objects.</para>
	/// </summary>
	[NativeHeader("Modules/UnityWebRequestAudio/Public/DownloadHandlerAudioClip.h")]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class DownloadHandlerAudioClip : DownloadHandler
	{
		/// <summary>
		///   <para>Constructor, specifies what kind of audio data is going to be downloaded.</para>
		/// </summary>
		/// <param name="url">The nominal (pre-redirect) URL at which the audio clip is located.</param>
		/// <param name="audioType">Value to set for AudioClip type.</param>
		public DownloadHandlerAudioClip(string url, AudioType audioType)
		{
			this.InternalCreateAudioClip(url, audioType);
		}

		public DownloadHandlerAudioClip(Uri uri, AudioType audioType)
		{
			this.InternalCreateAudioClip(uri.AbsoluteUri, audioType);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Create(DownloadHandlerAudioClip obj, string url, AudioType audioType);

		private void InternalCreateAudioClip(string url, AudioType audioType)
		{
			this.m_Ptr = DownloadHandlerAudioClip.Create(this, url, audioType);
		}

		/// <summary>
		///   <para>Called by DownloadHandler.data. Returns a copy of the downloaded clip data as raw bytes.</para>
		/// </summary>
		/// <returns>
		///   <para>A copy of the downloaded data.</para>
		/// </returns>
		protected override byte[] GetData()
		{
			return DownloadHandler.InternalGetByteArray(this);
		}

		protected override string GetText()
		{
			throw new NotSupportedException("String access is not supported for audio clips");
		}

		/// <summary>
		///   <para>Returns the downloaded AudioClip, or null. (Read Only)</para>
		/// </summary>
		public extern AudioClip audioClip
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Create streaming AudioClip.</para>
		/// </summary>
		public extern bool streamAudio
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Create AudioClip that is compressed in memory.</para>
		/// </summary>
		public extern bool compressed
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Returns the downloaded AudioClip, or null.</para>
		/// </summary>
		/// <param name="www">A finished UnityWebRequest object with DownloadHandlerAudioClip attached.</param>
		/// <returns>
		///   <para>The same as DownloadHandlerAudioClip.audioClip</para>
		/// </returns>
		public static AudioClip GetContent(UnityWebRequest www)
		{
			return DownloadHandler.GetCheckedDownloader<DownloadHandlerAudioClip>(www).audioClip;
		}
	}
}
