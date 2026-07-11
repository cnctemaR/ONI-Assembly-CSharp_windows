using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.Networking
{
	[NativeHeader("Modules/UnityWebRequest/Public/DownloadHandler/DownloadHandlerVFS.h")]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class DownloadHandlerFile : DownloadHandler
	{
		public DownloadHandlerFile(string path)
		{
			this.InternalCreateVFS(path);
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Create(DownloadHandlerFile obj, string path);

		private void InternalCreateVFS(string path)
		{
			string directoryName = Path.GetDirectoryName(path);
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			this.m_Ptr = DownloadHandlerFile.Create(this, path);
		}

		protected override byte[] GetData()
		{
			throw new NotSupportedException("Raw data access is not supported");
		}

		protected override string GetText()
		{
			throw new NotSupportedException("String access is not supported");
		}

		public extern bool removeFileOnAbort
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
