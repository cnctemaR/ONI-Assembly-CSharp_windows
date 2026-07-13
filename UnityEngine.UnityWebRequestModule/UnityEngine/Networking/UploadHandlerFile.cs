using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.Networking
{
	[NativeHeader("Modules/UnityWebRequest/Public/UploadHandler/UploadHandlerFile.h")]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class UploadHandlerFile : UploadHandler
	{
		[NativeThrows]
		private unsafe static IntPtr Create([UnityMarshalAs(NativeType.ScriptingObjectPtr)] UploadHandlerFile self, string filePath)
		{
			IntPtr intPtr;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(filePath, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = filePath.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				intPtr = UploadHandlerFile.Create_Injected(self, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return intPtr;
		}

		public UploadHandlerFile(string filePath)
		{
			this.m_Ptr = UploadHandlerFile.Create(this, filePath);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Create_Injected(UploadHandlerFile self, ref ManagedSpanWrapper filePath);

		internal new static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(UploadHandlerFile uploadHandler)
			{
				return uploadHandler.m_Ptr;
			}
		}
	}
}
