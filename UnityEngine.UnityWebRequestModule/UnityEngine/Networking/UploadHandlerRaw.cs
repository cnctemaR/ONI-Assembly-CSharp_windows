using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.Networking
{
	/// <summary>
	///   <para>A general-purpose UploadHandler subclass, using a native-code memory buffer.</para>
	/// </summary>
	[NativeHeader("Modules/UnityWebRequest/Public/UploadHandler/UploadHandlerRaw.h")]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class UploadHandlerRaw : UploadHandler
	{
		/// <summary>
		///   <para>General constructor. Contents of the input argument are copied into a native buffer.</para>
		/// </summary>
		/// <param name="data">Raw data to transmit to the remote server.</param>
		public UploadHandlerRaw(byte[] data)
		{
			if (data != null && data.Length == 0)
			{
				throw new ArgumentException("Cannot create a data handler without payload data");
			}
			this.m_Ptr = UploadHandlerRaw.Create(this, data);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Create(UploadHandlerRaw self, byte[] data);

		[NativeMethod("GetContentType")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern string InternalGetContentType();

		[NativeMethod("SetContentType")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalSetContentType(string newContentType);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern byte[] InternalGetData();

		[NativeMethod("GetProgress")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float InternalGetProgress();

		internal override string GetContentType()
		{
			return this.InternalGetContentType();
		}

		internal override void SetContentType(string newContentType)
		{
			this.InternalSetContentType(newContentType);
		}

		internal override byte[] GetData()
		{
			return this.InternalGetData();
		}

		internal override float GetProgress()
		{
			return this.InternalGetProgress();
		}
	}
}
