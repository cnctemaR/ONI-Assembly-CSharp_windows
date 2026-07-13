using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine.Bindings;

namespace UnityEngine.Networking
{
	[NativeHeader("Modules/UnityWebRequest/Public/DownloadHandler/DownloadHandlerVFS.h")]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class DownloadHandlerFile : DownloadHandler
	{
		[NativeThrows]
		private unsafe static IntPtr Create([UnityMarshalAs(NativeType.ScriptingObjectPtr)] DownloadHandlerFile obj, string path, bool append)
		{
			IntPtr intPtr;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(path, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = path.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				intPtr = DownloadHandlerFile.Create_Injected(obj, ref managedSpanWrapper, append);
			}
			finally
			{
				char* ptr = null;
			}
			return intPtr;
		}

		private void InternalCreateVFS(string path, bool append)
		{
			string directoryName = Path.GetDirectoryName(path);
			bool flag = !Directory.Exists(directoryName);
			if (flag)
			{
				Directory.CreateDirectory(directoryName);
			}
			this.m_Ptr = DownloadHandlerFile.Create(this, path, append);
		}

		public DownloadHandlerFile(string path)
		{
			this.InternalCreateVFS(path, false);
		}

		public DownloadHandlerFile(string path, bool append)
		{
			this.InternalCreateVFS(path, append);
		}

		protected override NativeArray<byte> GetNativeData()
		{
			throw new NotSupportedException("Raw data access is not supported");
		}

		protected override byte[] GetData()
		{
			throw new NotSupportedException("Raw data access is not supported");
		}

		protected override string GetText()
		{
			throw new NotSupportedException("String access is not supported");
		}

		public bool removeFileOnAbort
		{
			get
			{
				IntPtr intPtr = DownloadHandlerFile.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return DownloadHandlerFile.get_removeFileOnAbort_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = DownloadHandlerFile.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				DownloadHandlerFile.set_removeFileOnAbort_Injected(intPtr, value);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Create_Injected(DownloadHandlerFile obj, ref ManagedSpanWrapper path, bool append);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_removeFileOnAbort_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_removeFileOnAbort_Injected(IntPtr _unity_self, bool value);

		internal new static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(DownloadHandlerFile handler)
			{
				return handler.m_Ptr;
			}
		}
	}
}
