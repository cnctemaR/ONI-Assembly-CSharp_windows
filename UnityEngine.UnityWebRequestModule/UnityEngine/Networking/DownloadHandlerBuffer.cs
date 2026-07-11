using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.Networking
{
	/// <summary>
	///   <para>A general-purpose DownloadHandler implementation which stores received data in a native byte buffer.</para>
	/// </summary>
	[NativeHeader("Modules/UnityWebRequest/Public/DownloadHandler/DownloadHandlerBuffer.h")]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class DownloadHandlerBuffer : DownloadHandler
	{
		/// <summary>
		///   <para>Default constructor.</para>
		/// </summary>
		public DownloadHandlerBuffer()
		{
			this.InternalCreateBuffer();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Create(DownloadHandlerBuffer obj);

		private void InternalCreateBuffer()
		{
			this.m_Ptr = DownloadHandlerBuffer.Create(this);
		}

		/// <summary>
		///   <para>Returns a copy of the contents of the native-memory data buffer as a byte array.</para>
		/// </summary>
		/// <returns>
		///   <para>A copy of the data which has been downloaded.</para>
		/// </returns>
		protected override byte[] GetData()
		{
			return this.InternalGetData();
		}

		private byte[] InternalGetData()
		{
			return DownloadHandler.InternalGetByteArray(this);
		}

		/// <summary>
		///   <para>Returns a copy of the native-memory buffer interpreted as a UTF8 string.</para>
		/// </summary>
		/// <param name="www">A finished UnityWebRequest object with DownloadHandlerBuffer attached.</param>
		/// <returns>
		///   <para>The same as DownloadHandlerBuffer.text</para>
		/// </returns>
		public static string GetContent(UnityWebRequest www)
		{
			return DownloadHandler.GetCheckedDownloader<DownloadHandlerBuffer>(www).text;
		}
	}
}
