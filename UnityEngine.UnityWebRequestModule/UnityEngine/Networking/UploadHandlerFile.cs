using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.Networking
{
	/// <summary>
	///   <para>A specialized UploadHandler that reads data from a given file and sends raw bytes to the server as the request body.</para>
	/// </summary>
	[NativeHeader("Modules/UnityWebRequest/Public/UploadHandler/UploadHandlerFile.h")]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class UploadHandlerFile : UploadHandler
	{
		/// <summary>
		///   <para>Create a new upload handler to send data from the given file to the server.</para>
		/// </summary>
		/// <param name="filePath">A file containing data to send.</param>
		public UploadHandlerFile(string filePath)
		{
			this.m_Ptr = UploadHandlerFile.Create(this, filePath);
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Create(UploadHandlerFile self, string filePath);
	}
}
